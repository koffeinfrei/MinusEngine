using System;
using System.Text;
using Newtonsoft.Json;

namespace MinusEngine
{
// ReSharper disable InconsistentNaming
    public class oAuthResult
// ReSharper restore InconsistentNaming
    {
        public oAuthResult()
        {
            
        }

        public oAuthResult(String accessToken, String tokenType, String expire, String refreshToken, String scope)
        {
            AccessToken = accessToken;
            TokenType = tokenType;
            Expire = expire;
            RefreshToken = refreshToken;
            Scope = scope;
        }

        [JsonProperty("access_token")]
        public String AccessToken { get; set; }

        [JsonProperty("token_type")]
        public String TokenType { get; set; }

        [JsonProperty("expire_in")]
        public String Expire { get; set; }

        [JsonProperty("refresh_token")]
        public String RefreshToken { get; set; }

        [JsonProperty("scope")]
        public String Scope { get; set; }

        public override string ToString()
        {
            return new StringBuilder("oAuthResult{AccessToken=")
                .Append(AccessToken)
                .Append(", TokenType=")
                .Append(TokenType)
                .Append(", Expire=")
                .Append(Expire)
                .Append(", RefreshToken=")
                .Append(RefreshToken)
                .Append(", Scope=")
                .Append(Scope)
                .Append('}').ToString();
        }
    }
}
