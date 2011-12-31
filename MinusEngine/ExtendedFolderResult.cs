using System;
using System.Text;

namespace MinusEngine
{
    public class ExtendedFolderResult
    {
        public ExtendedFolderResult(Int32 flagCount, Int32 viewCount, String displayName, String name, String creator,
                                    String url, DateTime created, Boolean isSaved, Boolean isFlagged, Int32 featured,
                                    Int32 score, Int32 fileCount, Int32 starCount, Int32 isPublic, Boolean active,
                                    Boolean isReshared, Boolean isStarred, String thumbnailUrl, Int32 createdAgo,
                                    String files)
        {
            FlagCount = flagCount;
            ViewCount = viewCount;
            DisplayName = displayName;
            Name = name;
            Creator = creator;
            Url = url;
            Created = created;
            IsSaved = isSaved;
            IsFlagged = isFlagged;
            Featured = featured;
            Score = score;
            FileCount = fileCount;
            StarCount = starCount;
            IsPublic = isPublic;
            Active = active;
            IsReshared = isReshared;
            IsStarred = isStarred;
            ThumbnailUrl = thumbnailUrl;
            CreatedAgo = createdAgo;
            Files = files;
        }

        #region Fields

        public Int32 FlagCount { get; set; }
        public Int32 ViewCount { get; set; }
        public String DisplayName { get; set; }
        public String Name { get; set; }
        public String Creator { get; set; }
        public String Url { get; set; }
        public DateTime Created { get; set; }
        public Boolean IsSaved { get; set; }
        public Boolean IsFlagged { get; set; }
        public Int32 Featured { get; set; }
        public Int32 Score { get; set; }
        public Int32 FileCount { get; set; }
        public Int32 StarCount { get; set; }
        public Int32 IsPublic { get; set; }
        public Boolean Active { get; set; }
        public Boolean IsReshared { get; set; }
        public Boolean IsStarred { get; set; }
        public String ThumbnailUrl { get; set; }
        public Int32 CreatedAgo { get; set; }
        public String Files { get; set; }

        #endregion

        #region Low level overrides

        public override string ToString()
        {
            return new StringBuilder("ExtendedFolderResult{")
                .Append("FlagCount=").Append(FlagCount)
                .Append(", ViewCount=").Append(ViewCount)
                .Append(", DisplayName=").Append(DisplayName)
                .Append(", Name=").Append(Name)
                .Append(", Creator=").Append(Creator)
                .Append(", Url=").Append(Url)
                .Append(", Created=").Append(Created)
                .Append(", IsSaved=").Append(IsSaved)
                .Append(", IsFlagged=").Append(IsFlagged)
                .Append(", Featured=").Append(Featured)
                .Append(", Score=").Append(Score)
                .Append(", FileCount=").Append(FileCount)
                .Append(", StarCount=").Append(StarCount)
                .Append(", IsPublic=").Append(IsPublic)
                .Append(", Active=").Append(Active)
                .Append(", IsReshared=").Append(IsReshared)
                .Append(", IsStarred=").Append(IsStarred)
                .Append(", ThumbnailUrl=").Append(ThumbnailUrl)
                .Append(", CreatedAgo=").Append(CreatedAgo)
                .Append(", Files=").Append(Files)
                .Append('}').ToString();
        }

        #endregion
    }
}