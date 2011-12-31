using System;
using System.Text;
using Newtonsoft.Json;

namespace MinusEngine
{
    public class FileResult
    {
        public FileResult(String mimetype, DateTime uploaded, String name, String caption, String url, String urlPreview,
                          String title, String urlThumbnailMedium, Int32 height, Int32 width, Int32 uploadedAgo,
                          Int32 fileSize, String urlRawFile, String folder, String urlThumbnail, String fileId)
        {
            Mimetype = mimetype;
            Uploaded = uploaded;
            Name = name;
            Caption = caption;
            Url = url;
            UrlPreview = urlPreview;
            Title = title;
            UrlThumbnailMedium = urlThumbnailMedium;
            Height = height;
            Width = width;
            UploadedAgo = uploadedAgo;
            FileSize = fileSize;
            UrlRawFile = urlRawFile;
            Folder = folder;
            UrlThumbnail = urlThumbnail;
            FileID = fileId;
        }

        #region Fields

        [JsonProperty("mimetype")]
        public String Mimetype { get; set; }

        [JsonProperty("uploaded")]
        public DateTime Uploaded { get; set; }

        [JsonProperty("name")]
        public String Name { get; set; }

        [JsonProperty("caption")]
        public String Caption { get; set; }

        [JsonProperty("url")]
        public String Url { get; set; }

        [JsonProperty("url_preview")]
        public String UrlPreview { get; set; }

        [JsonProperty("title")]
        public String Title { get; set; }

        [JsonProperty("url_thumbnail_medium")]
        public String UrlThumbnailMedium { get; set; }

        [JsonProperty("height")]
        public Int32 Height { get; set; }

        [JsonProperty("width")]
        public Int32 Width { get; set; }

        [JsonProperty("uploaded_ago")]
        public Int32 UploadedAgo { get; set; }

        [JsonProperty("filesize")]
        public Int32 FileSize { get; set; }

        [JsonProperty("url_rawfile")]
        public String UrlRawFile { get; set; }

        [JsonProperty("folder")]
        public String Folder { get; set; }

        [JsonProperty("url_thumbnail")]
        public String UrlThumbnail { get; set; }

        [JsonProperty("id")]
        public String FileID { get; set; }

        #endregion

        #region Low level overrides

        public override string ToString()
        {
            return new StringBuilder("UploadItemResult{")
                .Append("Mimetype=").Append(Mimetype)
                .Append(", Uploaded=").Append(Uploaded)
                .Append(", Name=").Append(Name)
                .Append(", Caption=").Append(Caption)
                .Append(", Url=").Append(Url)
                .Append(", UrlPreview=").Append(UrlPreview)
                .Append(", Title=").Append(Title)
                .Append(", UrlThumbnailMedium=").Append(UrlThumbnailMedium)
                .Append(", Height=").Append(Height)
                .Append(", Width=").Append(Width)
                .Append(", UploadedAgo=").Append(UploadedAgo)
                .Append(", FileSize=").Append(FileSize)
                .Append(", UrlRawFile=").Append(UrlRawFile)
                .Append(", Folder=").Append(Folder)
                .Append(", UrlThumbnail=").Append(UrlThumbnail)
                .Append(", FileId=").Append(FileID)
                .Append('}').ToString();
        }

        #endregion
    }
}