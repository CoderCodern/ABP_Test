using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Collections.Extensions;
using Abp.UI;
using CloudinaryDotNet;
using Microsoft.EntityFrameworkCore;
using TestABP.Configuration;
using TestABP.CoreDependencies.IOC;
using TestABP.Domain.CloudFolders.Dto;
using TestABP.Domain.CloudFolders.Validators;
using TestABP.Domain.Shared.Models;
using TestABP.Utils;

namespace TestABP.Domain.CloudFolders
{
    public class CloudFolderManager : ApplicationService
    {
        private readonly IWorkScope _workScope;
        private readonly CloudinaryConfig _cloudinaryConfig;

        public CloudFolderManager(
            IWorkScope workScope,
            CloudinaryConfig cloudinaryConfig
            )
        {
            _workScope = workScope;
            _cloudinaryConfig = cloudinaryConfig;
        }

        public async Task<TreeFolderDto> GetAll(FolderFilterDto input)
        {
            try
            {
                var folders = await _workScope.GetAll<CloudFolder>().ToListAsync();
                var mappedFolders = folders
                    .Select(x => new CloudFolderDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        IsActive = x.IsActive,
                        Level = x.Level,
                        IsLeaf = x.IsLeaf,
                        ParentId = x.ParentId,
                        Code = x.Code,
                        CombineName = x.CombineName
                    })
                    .OrderBy(x => CommonUtil.GetNaturalSortKey(x.Code))
                    .ToList();

                if (input.IsGetAll())
                {
                    var treeFolderDto = new TreeFolderDto
                    {
                        Childrens = mappedFolders.GenerateTree(c => c.Id, c => c.ParentId)
                    };

                    Logger.Info("Get list tree CloudFolder successfully! ");
                    return treeFolderDto;
                }

                var mappedIds = mappedFolders
                    .WhereIf(input.IsLeaf.HasValue, x => x.IsLeaf == input.IsLeaf)
                    .WhereIf(input.IsActive.HasValue, x => x.IsActive == input.IsActive)
                    .WhereIf(!string.IsNullOrEmpty(input.SearchText),
                             (x => x.Name.ToLower().Contains(input.SearchText.Trim().ToLower()) ||
                             (x.Code.ToLower().Contains(input.SearchText.Trim().ToLower()))))
                    .Select(x => x.Id)
                    .ToList();

                var folderIds = new List<long>();
                foreach (var id in mappedIds)
                {
                    folderIds.AddRange(GetAllNodeAndLeafIdsByProvidedId(id, folders, true));
                }
                folderIds = folderIds.Distinct().ToList();

                var filteredTreeFolder = new TreeFolderDto
                {
                    Childrens = mappedFolders
                                .Where(x => folderIds.Contains(x.Id))
                                .ToList()
                                .GenerateTree(c => c.Id, c => c.ParentId)
                };

                Logger.Info("Get list tree CloudFolder successfully! ");
                return filteredTreeFolder;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
                return new TreeFolderDto();
            }
        }

        public async Task<CloudFolderDto> GetDetail(long folderId)
        {
            try
            {
                var folder = await _workScope.GetAsync<CloudFolder>(folderId);

                if (folder == null)
                    throw new UserFriendlyException($"Folder with Id {folderId} does not exist!");

                var folderDto = new CloudFolderDto()
                {
                    Id = folder.Id,
                    Name = folder.Name,
                    IsActive = folder.IsActive,
                    Level = folder.Level,
                    IsLeaf = folder.IsLeaf,
                    ParentId = folder.ParentId,
                    Code = folder.Code,
                    CombineName = folder.CombineName,
                };

                Logger.Info("Get detail CloudFolder successfully!");
                return folderDto;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
                return new CloudFolderDto();
            }
        }

        public async Task<CreateCloudFolderDto> Create(CreateCloudFolderDto input)
        {
            await ValidateCreate(input);

            var mappedFolder = ObjectMapper.Map<CloudFolder>(input);
            mappedFolder.IsActive = true;
            mappedFolder.IsLeaf = true;

            try
            {
                // Create Folder in Database
                if (input.ParentId.HasValue)
                {
                    var parentFolder = await _workScope.GetAsync<CloudFolder>(input.ParentId.Value);
                    if (parentFolder != null)
                    {
                        parentFolder.IsLeaf = false;
                        mappedFolder.Level = parentFolder.Level + 1;
                        mappedFolder.CombineName = parentFolder.CombineName + "/" + mappedFolder.Name;
                        // Save changes after modifying entities
                        await _workScope.UpdateAsync(parentFolder);
                    }
                    else
                        throw new UserFriendlyException("Parent folder not found!");
                }
                else
                {
                    mappedFolder.Level = 1;
                    mappedFolder.CombineName = mappedFolder.Name;
                }

                var idCreateFolder = await _workScope.InsertAndGetIdAsync<CloudFolder>(mappedFolder);

                if (idCreateFolder == 0)
                    throw new UserFriendlyException($"There is something wrong with creating Folder!");

                // Create Folder in Cloudinary
                if (input.IsSyncToCloud == true)
                {
                    var cloudinary = new Cloudinary(_cloudinaryConfig.CloudinaryAccountConfig());
                    var isCloudExisted = await CheckExistedFolder(cloudinary, mappedFolder.CombineName);
                    if (isCloudExisted)
                        throw new UserFriendlyException($"A folder with that name already exists in Cloudinary!");
                    else
                        await cloudinary.CreateFolderAsync(mappedFolder.CombineName);
                }

                return input;
            }
            catch (UserFriendlyException)
            {

                throw;
            }
        }

        private async Task ValidateCreate(CreateCloudFolderDto input)
        {
            var validator = new CreateCloudFolderValidation();
            var validationResult = validator.Validate(input);

            if (!validationResult.IsValid)
            {
                var errorMessage = string.Join(Environment.NewLine, validationResult.Errors.Select(x => x.ErrorMessage));
                Logger.Error(errorMessage);
                throw new UserFriendlyException(errorMessage);
            }

            var isExisted = await _workScope.GetAll<CloudFolder>()
                        .AnyAsync(x => x.Name == input.Name || x.Code == input.Code);
            if (isExisted)
                throw new UserFriendlyException($"Folder with Name '{input.Name}' or Code '{input.Code}' already exists!");
        }

        public async Task<UpdateCloudFolderDto> Update(UpdateCloudFolderDto input)
        {
            var folder = await _workScope.GetAsync<CloudFolder>(input.Id);
            await ValidateUpdate(folder, input);

            var folders = await _workScope.GetAll<CloudFolder>().ToListAsync();

            try
            {
                if (input.Code != folder.Code && !folder.IsLeaf)
                    await RenameChildCodes(folders, input);

                var oldCloudName = folder.CombineName;
                var splitName = folder.CombineName.Split('/');
                var countSlash = splitName.Length - 1;

                if (input.Name != folder.Name)
                {
                    var splitCombineName = folder.CombineName.Split('/');
                    splitCombineName[countSlash] = input.Name;
                    folder.CombineName = string.Join('/', splitCombineName);

                    //await UpdateCloudinaryFolder(oldCloudName, cloudFolder.CombineName);

                    await _workScope.UpdateAsync<CloudFolder>(folder);

                    await RenameChildCombineName(folders, input, folder.CombineName);
                }

                folder.Name = input.Name;
                folder.Code = input.Code;

                await _workScope.UpdateAsync<CloudFolder>(folder);

                return input;
            }
            catch (UserFriendlyException)
            {
                throw;
            }
        }

        private async Task ValidateUpdate(CloudFolder folder, UpdateCloudFolderDto input)
        {
            if (folder == null)
                throw new UserFriendlyException($"Cannot found folder with Id = {input.Id}");

            var validator = new UpdateCloudFolderValidation();
            var validationResult = validator.Validate(input);

            if (!validationResult.IsValid)
            {
                var errorMessage = string.Join(Environment.NewLine, validationResult.Errors.Select(x => x.ErrorMessage));
                Logger.Error(errorMessage);
                throw new UserFriendlyException(errorMessage);
            }

            var duplicatedFolder = await _workScope.GetAll<CloudFolder>()
                                    .AnyAsync(x => (x.Name == input.Name || x.Code == input.Code) && x.Id != input.Id);
            if (duplicatedFolder)
                throw new UserFriendlyException(String.Format("Name or Code already exist in Cloud Folder!"));
        }

        #region Tree Data List Handling

        private List<long> GetAllNodeAndLeafIdsByProvidedId(long id, List<CloudFolder> folders, bool isGetParent = false)
        {
            var resultIds = new List<long>();
            var targetFolder = folders.FirstOrDefault(x => x.Id == id);

            if (targetFolder == null)
                return resultIds;

            resultIds.Add(targetFolder.Id);

            if (targetFolder.IsLeaf || isGetParent)
            {
                resultIds.AddRange(GetAllParentIds(targetFolder.Id, folders));
            }

            if (!targetFolder.IsLeaf)
            {
                var childFolders = folders.Where(x => x.ParentId == id).ToList();
                foreach (var child in childFolders)
                {
                    resultIds.AddRange(GetAllChildIds(child.Id, folders));
                }
            }

            return resultIds.Distinct().ToList();
        }

        private List<long> GetAllParentIds(long id, List<CloudFolder> folders)
        {
            var result = new List<long>();
            var current = folders.FirstOrDefault(x => x.Id == id);

            while (current?.ParentId != null)
            {
                var parent = folders.FirstOrDefault(x => x.Id == current.ParentId);
                if (parent == null)
                    break;

                result.Add(parent.Id);
                current = parent;
            }

            return result;
        }

        private List<long> GetAllChildIds(long id, List<CloudFolder> folders)
        {
            var result = new List<long> { id };

            var childIds = folders.Where(x => x.ParentId == id).Select(x => x.Id).ToList();
            foreach (var childId in childIds)
            {
                result.AddRange(GetAllChildIds(childId, folders));
            }

            return result;
        }

        private async Task RenameChildCodes(List<CloudFolder> folders, UpdateCloudFolderDto input)
        {
            var splitCode = input.Code.Split('.');
            var level = splitCode.Length - 1;

            await UpdateChildFolders(folders, input.Id, child =>
            {
                var parts = child.Code.Split(".");
                if (parts.Length > level)
                {
                    parts[level] = splitCode[level];
                    child.Code = string.Join(".", parts);
                }
            });
        }

        private async Task RenameChildCombineName(
            List<CloudFolder> folders, 
            UpdateCloudFolderDto input, 
            string combineName)
        {
            var splitName = combineName.Split('/');
            var level = splitName.Length - 1;

            await UpdateChildFolders(folders, input.Id, child =>
            {
                var parts = child.CombineName.Split('/');
                if (parts.Length > level)
                {
                    parts[level] = splitName[level].Trim();
                    child.CombineName = string.Join("/", parts);
                }
            });
        }

        private async Task UpdateChildFolders(
            List<CloudFolder> folders,
            long parentId,
            Action<CloudFolder> updateAction)
        {
            var childIds = GetAllChildIds(parentId, folders).Distinct();
            var childFolders = folders
                .Where(x => childIds.Contains(x.Id) && x.Id != parentId)
                .ToList();

            foreach (var child in childFolders)
            {
                updateAction(child);
            }

            await _workScope.UpdateRangeAsync<CloudFolder>(childFolders);
        }

        #endregion

        #region Cloudinary Handling

        private async Task<bool> CheckExistedFolder(Cloudinary cloudinary, string name)
        {
            var subFoldersResult = await cloudinary.SubFoldersAsync(name);

            if (subFoldersResult != null && subFoldersResult.Folders != null)
                return true;
            else
                return false;
        }

        public async Task<string> UpdateCloudinaryFolder(string fromPath, string toPath)
        {
            try
            {
                // Update from Test3/Test3.1 to Test3/Test3.2 (successfully)
                // Update from Test3/Test3.1 (have asset inside child folder) to Test4/Test4.1 (create new folder Test4/Test4.1 with asset inside and old one turns to Test3)
                var cloudinary = new Cloudinary(_cloudinaryConfig.CloudinaryAccountConfig());
                var updateFolderResult = await cloudinary.RenameFolderAsync(fromPath, toPath);

                if (updateFolderResult != null && updateFolderResult.To.Path!= null)
                {
                    return updateFolderResult.To.Path;
                }
                else
                {
                    return updateFolderResult.From.Path;
                }
            }
            catch (UserFriendlyException)
            {
                throw;
            }
        }

        #endregion
    }
}
