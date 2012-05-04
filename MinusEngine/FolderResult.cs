using System;
using System.Text;
using Newtonsoft.Json;

namespace MinusEngine
{
    public class FolderResult
    {
        public FolderResult(String files, String viewCount, DateTime dateUpdated, String name, String creator, String url,
                            String thumbnailUrl, Int32 fileCount, Boolean isPublic, String id)
        {
            Files = files;
            ViewCount = viewCount;
            DateUpdated = dateUpdated;
            Name = name;
            Creator = creator;
            URL = url;
            ThumbnailUrl = thumbnailUrl;
            FileCount = fileCount;
            IsPublic = isPublic;
            ID = id;
        }

        #region Fields

        [JsonProperty("files")]
        public String Files { get; set; }

        [JsonProperty("view_count")]
        public String ViewCount { get; set; }

        [JsonProperty("date_last_updated")]
        public DateTime DateUpdated { get; set; }

        [JsonProperty("name")]
        public String Name { get; set; }

        [JsonProperty("creator")]
        public String Creator { get; set; }

        [JsonProperty("url")]
        public String URL { get; set; }

        [JsonProperty("thumbnail_url")]
        public String ThumbnailUrl { get; set; }

        [JsonProperty("file_count")]
        public Int32 FileCount { get; set; }

        [JsonProperty("is_public")]
        public Boolean IsPublic { get; set; }

        [JsonProperty("id")]
        public String ID { get; set; }

        #endregion

        #region Low level overrides

        public override string ToString()
        {
            return new StringBuilder("FolderResult{")
                .Append("Files=").Append(Files)
                .Append(", ViewCount=").Append(ViewCount)
                .Append(", DateLastUpdated=").Append(DateUpdated)
                .Append(", Name=").Append(Name)
                .Append(", Creator=").Append(Creator)
                .Append(", Url=").Append(URL)
                .Append(", ItemOrder=").Append(ItemOrdering)
                .Append(", LastUpdatedAgo=").Append(LastUpdated)
                .Append(", CreatedAgo=").Append(CreatedAgo)
                .Append(", ThumbnailUrl=").Append(ThumbnailUrl)
                .Append(", FileCount=").Append(FileCount)
                .Append(", IsPublic=").Append(IsPublic)
                .Append(", ID=").Append(ID)
                .Append('}').ToString();
        }

        #endregion
    }
}