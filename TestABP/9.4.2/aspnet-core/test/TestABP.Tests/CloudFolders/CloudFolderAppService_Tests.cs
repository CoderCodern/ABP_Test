using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.ObjectMapping;
using NSubstitute;
using Shouldly;
using TestABP.APIs.CloudFolders;
using TestABP.CoreDependencies.IOC;
using TestABP.Domain.CloudFolders;
using TestABP.Domain.CloudFolders.Dto;
using TestABP.Tests.CoreDependencies.Extensions;
using Xunit;

namespace TestABP.Tests.CloudFolders
{
    public class CloudFolderAppService_Tests : TestABPTestBase
    {
        #region Create Mock Test Environment

        private readonly ICloudFolderAppService _cloudFolderAppService;

        private static List<CloudFolder> CreateFakeFolders()
        {
            return new List<CloudFolder>
            {
                new CloudFolder { Id = 1, Name = "Root", IsActive = true, IsLeaf = false, ParentId = null, Level = 0, Code = "001", CombineName = "Root" },
                new CloudFolder { Id = 2, Name = "Child A", IsActive = true, IsLeaf = true, ParentId = 1, Level = 1, Code = "001.001", CombineName = "Root > Child A" },
                new CloudFolder { Id = 3, Name = "Child B", IsActive = true, IsLeaf = true, ParentId = 1, Level = 1, Code = "001.002", CombineName = "Root > Child B" },
            };
        }

        private static IWorkScope SetupMockWorkScope(List<CloudFolder> folders)
        {
            var mockQueryable = new TestAsyncEnumerable<CloudFolder>(folders);

            var workScope = Substitute.For<IWorkScope>();

            // Mock GetAll<T>()
            workScope.GetAll<CloudFolder>().Returns(mockQueryable);

            // Mock GetAsync<T>(id)
            workScope.GetAsync<CloudFolder>(Arg.Any<long>())
                .Returns(callInfo =>
                {
                    var id = callInfo.ArgAt<long>(0);
                    return Task.FromResult(folders.FirstOrDefault(f => f.Id == id));
                });

            // UpdateAsync<T>(entity)
            workScope.UpdateAsync(Arg.Any<CloudFolder>())
                .Returns(callInfo =>
                {
                    var folder = callInfo.ArgAt<CloudFolder>(0);
                    return Task.FromResult(folder); // Return the same folder to simulate an update
                });

            // InsertAndGetIdAsync<T>(entity)
            workScope.InsertAndGetIdAsync<CloudFolder>(Arg.Any<CloudFolder>())
                .Returns(4); // Simulate successful insert with ID 4

            return workScope;
        }

        private static ICloudFolderAppService CreateTestAppService()
        {
            var folders = CreateFakeFolders();
            var workScope = SetupMockWorkScope(folders); // <-- Supports both GetAll + GetAsync
            var manager = new CloudFolderManager(workScope);
            return new CloudFolderAppService(manager);
        }

        public CloudFolderAppService_Tests()
        {
            _cloudFolderAppService = CreateTestAppService();
        }

        #endregion

        #region Test Cases

        [Fact]
        public async Task GetAll_Test()
        {
            var result = await _cloudFolderAppService.GetAll(new FolderFilterDto
            {
                IsActive = true,
                IsLeaf = null,
                SearchText = null
            });

            var childrenList = result.Childrens.ToList();

            childrenList.Count.ShouldBe(1); // Only 1 root
            childrenList[0].Item.Name.ShouldBe("Root");

            // Access children of root
            var rootChildren = childrenList[0].Children.ToList();

            rootChildren.Count.ShouldBe(2);
            rootChildren[0].Item.Name.ShouldBe("Child A");
            rootChildren[1].Item.Name.ShouldBe("Child B");

            var firstChildChildren = childrenList[0].Children.ToList();
            firstChildChildren.Count.ShouldBe(2);
        }

        [Fact]
        public async Task GetDetail_Test()
        {
            // Act
            var result = await _cloudFolderAppService.GetDetail(2); // Get Child A

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(2);
            result.Name.ShouldBe("Child A");
            result.IsActive.ShouldBeTrue();
            result.IsLeaf.ShouldBeTrue();
            result.ParentId.ShouldBe(1);
            result.Level.ShouldBe(1);
            result.Code.ShouldBe("001.001");
            result.CombineName.ShouldBe("Root > Child A");
        }

        /*[Fact]
        public async Task Create_Test()
        {
            var newFolderDto = new CreateCloudFolderDto
            {
                Name = "Child C",
                Code = "001.003",
                ParentId = 1 // Root folder
            };

            // Act
            var result = await _cloudFolderAppService.Create(newFolderDto);

            // Assert
            result.ShouldNotBeNull();
            result.Name.ShouldBe("Child C");
            result.Code.ShouldBe("001.003");
            result.ParentId.ShouldBe(1);
        }*/

        #endregion
    }
}
