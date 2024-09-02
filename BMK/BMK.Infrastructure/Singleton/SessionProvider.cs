using BMK.Infrastructure.Model;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BMK.Infrastructure.Singleton
{
    public class SessionProvider : ISessionProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SessionProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public T GetObject<T>(string key)
        {
            var value = _httpContextAccessor.HttpContext.Session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }
        public void SetObject<T>(string key, T value)
        {
            var serializedValue = JsonSerializer.Serialize(value);
            _httpContextAccessor.HttpContext.Session.SetString(key, serializedValue);
        }
    }
    public interface ISessionProvider
    {
        T GetObject<T>(string key);
        void SetObject<T>(string key, T value);
    }
}
