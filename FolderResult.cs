using System;
using System.Text;
using Newtonsoft.Json;

namespace MinusEngine
{
    public class FolderResult
    {
        public FolderResult()
        {

        }

        public FolderResult(String files, String viewCount, String dateUpdated, String name, String creator, String url, String created, String[] itemOrdering, int lastUpdatedAgo, int createdAgo, String thumbnailUrl, int fileCount, Boolean isPublic, String id)
        {
            this.Files = files;
            this.ViewCount = viewCount;
            this.DateUpdated = dateUpdated;
            this.Name = name;
            this.Creator = creator;
            this.URL = url;
            this.Created = created;
            this.ItemOrdering = itemOrdering;
            this.LastUpdated = lastUpdatedAgo;
            this.CreatedAgo = createdAgo;
            this.ThumbnailUrl = thumbnailUrl;
            this.FileCount = fileCount;
            this.IsPublic = isPublic;
            this.ID = id;
        }

        [JsonProperty("files")]
        public String Files { get; set; }

        [JsonProperty("view_count")]
        public String ViewCount { get; set; }

        [JsonProperty("date_last_updated")]
        public String DateUpdated { get; set; }

        [JsonProperty("name")]
        public String Name { get; set; }

        [JsonProperty("creator")]
        public String Creator { get; set; }

        [JsonProperty("url")]
        public String URL { get; set; }

        [JsonProperty("created")]
        public String Created { get; set; }

        [JsonProperty("item_ordering")]
        public String[] ItemOrdering { get; set; }

        [JsonProperty("last_updated")]
        public int LastUpdated { get; set; }

        [JsonProperty("created_ago")]
        public int CreatedAgo { get; set; }

        [JsonProperty("thumbnail_url")]
        public String ThumbnailUrl { get; set; }

        [JsonProperty("file_count")]
        public int FileCount { get; set; }

        [JsonProperty("is_public")]
        public Boolean IsPublic { get; set; }

        [JsonProperty("id")]
        public String ID { get; set; }

        #region Low level overrides
        public override string ToString()
        {
            return new StringBuilder("UploadItemResult{")
                .Append("Files=").Append(this.Files)
                .Append(", ViewCount=").Append(this.ViewCount)
                .Append(", DateLastUpdated=").Append
                (this.DateUpdated)
                .Append(", Name=").Append(this.Name)
                .Append(", Creator=").Append(this.Creator)
                .Append(", Url=").Append(this.URL)
                .Append(", ItemOrder=").Append
                (this.ItemOrdering)
                .Append(", LastUpdatedAgo=").Append
                (this.LastUpdated)
                .Append(", CreatedAgo=").Append
                (this.CreatedAgo)
                .Append(", ThumbnailUrl=").Append
                (this.ThumbnailUrl)
                .Append(", FileCount=").Append(this.FileCount)
                .Append(", IsPublic=").Append(this.IsPublic)
                .Append(", ID=").Append(this.ID)
                .Append('}').ToString();
        }
        #endregion
    }
}
