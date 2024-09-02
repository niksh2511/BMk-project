using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BMK.Models.ViewModels
{
    public class TokenResponse
    {
        [JsonPropertyName("x_refresh_token_expires_in")]
        public int x_refresh_token_expires_in { get; set; }

        [JsonPropertyName("refresh_Token")]
        public string refresh_Token { get; set; }

        [JsonPropertyName("access_token")]
        public string Access_Token { get; set; }

        [JsonPropertyName("token_type")]
        public string token_type { get; set; }

        [JsonPropertyName("expires_in")]
        public int expires_in { get; set; }
        
      
       
    }
}
