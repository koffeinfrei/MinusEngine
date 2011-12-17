using System;
using System.Text;
using Newtonsoft.Json;

namespace MinusEngine
{
    public class CreateFolderResult
    {
         public CreateFolderResult()
        {
            
        }

        public CreateFolderResult(String files, String viewCount, String dateUpdated, String name, String creator, String url, String thumbnail, String fileCount, Boolean isPublic, String id)
        {
            Files = files;
            ViewCount = viewCount;
            DateUpdated = dateUpdated;
            Name = name;
            Creator = creator;
            Url = url;
            Thumbnail = thumbnail;
            FileCount = fileCount;
            IsPublic = isPublic;
            FolderID = id;
        }

        #region Fields

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
        public String Url { get; set; }

        [JsonProperty("thumbnail_url")]
        public String Thumbnail { get; set; }

        [JsonProperty("file_count")]
        public String FileCount { get; set; }

        [JsonProperty("is_public")]
        public Boolean IsPublic { get; set; }

        [JsonProperty("id")]
        public String FolderID { get; set; }

        #endregion

        #region Low Level Overide

        public override string ToString()
        {
            return new StringBuilder("GetFolderResult{Files=")
                .Append(Files).Append(", ViewCount=")
                .Append(ViewCount).Append(", DateLastUpdated=")
                .Append(DateUpdated).Append(", Name=")
                .Append(Name).Append(", Creator=")
                .Append(", Url=").Append(Url)
                .Append(", ThumbnailURL=").Append(Thumbnail)
                .Append(", FileCount=").Append(FileCount)
                .Append(", IsPublic=").Append(IsPublic)
                .Append(", FolderID=").Append(FolderID)
                .Append('}').ToString();
        }

        #endregion
    }
}
