using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace MinusEngine
{
    public class GetFollowResult
    {
        #region Fields

        [JsonProperty("username")]
        public String Username { get; set; }

        [JsonProperty("folders")]
        public String Folders { get; set; }

        [JsonProperty("display_name")]
        public String DisplayName { get; set; }

        [JsonProperty("description")]
        public String Description { get; set; }

        [JsonProperty("url")]
        public String Url { get; set; }

        [JsonProperty("visits")]
        public Int32 Visits { get; set; }

        [JsonProperty("avatar")]
        public String Avatar { get; set; }

        [JsonProperty("fb_username")]
        public String FbUsername { get; set; }

        [JsonProperty("followers")]
        public String Followers { get; set; }

        [JsonProperty("karma")]
        public Int32 Karma { get; set; }

        [JsonProperty("following")]
        public String Following { get; set; }

        [JsonProperty("shared")]
        public Int32 Shared { get; set; }

        [JsonProperty("fb_profile_link")]
        public String FbProfileLink { get; set; }

        [JsonProperty("slug")]
        public String Slug { get; set; }

        [JsonProperty("twitter_screen_name")]
        public String TwitterScreenName { get; set; }

        #endregion

        public GetFollowResult(String username, String folders, String displayName, String description, String url, Int32 visits, String avatar, String fbUsername, String followers, Int32 karma, String following, Int32 shared, String fbProfileLink, String slug, String twitterScreenName)
        {
            Username = username;
            Folders = folders;
            DisplayName = displayName;
            Description = description;
            Url = url;
            Visits = visits;
            Avatar = avatar;
            FbUsername = fbUsername;
            Followers = followers;
            Karma = karma;
            Following = following;
            Shared = shared;
            FbProfileLink = fbProfileLink;
            Slug = slug;
            TwitterScreenName = twitterScreenName;
        }

        #region Low level overrides

        public override string ToString()
        {
            return new StringBuilder("GetFollowersResult{")
                .Append("Username=").Append(Username)
                .Append(", Folders=").Append(Folders)
                .Append(", DisplayName=").Append(DisplayName)
                .Append(", Description=").Append(Description)
                .Append(", Url=").Append(Url)
                .Append(", Visits=").Append(Visits)
                .Append(", Avatar=").Append(Avatar)
                .Append(", FaceBookUsername=").Append(FbUsername)
                .Append(", Followers=").Append(Followers)
                .Append(", Karma=").Append(Karma)
                .Append(", Following=").Append(Following)
                .Append(", Shared=").Append(Shared)
                .Append(", FaceBookProfileLink=").Append(FbProfileLink)
                .Append(", Slug=").Append(Slug)
                .Append(", TwitterScreenName=").Append(TwitterScreenName)
                .Append('}').ToString();
        }

        #endregion
    }
}
