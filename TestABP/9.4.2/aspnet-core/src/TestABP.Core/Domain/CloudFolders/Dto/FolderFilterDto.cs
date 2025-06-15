namespace TestABP.Domain.CloudFolders.Dto
{
    public class FolderFilterDto
    {
        public bool? IsActive { get; set; }
        public bool? IsLeaf { get; set; }
        public string? SearchText { get; set; }

        public bool IsGetAll()
        {
            return !IsActive.HasValue && string.IsNullOrEmpty(SearchText) && !IsLeaf.HasValue;
        }
    }
}
