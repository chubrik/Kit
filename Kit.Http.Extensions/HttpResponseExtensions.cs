using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace Kit.Http
{
    public static class HttpResponseExtensions
    {
        public static dynamic GetJson(this IHttpResponse response) =>
            Task.Run(() => response.GetJsonAsync<object>()).Result;

        public static Task<dynamic> GetJsonAsync(this IHttpResponse response) =>
            response.GetJsonAsync<object>();

        public static T GetJson<T>(this IHttpResponse response) where T : class =>
            Task.Run(() => response.GetJsonAsync<T>()).Result;

        public static async Task<T> GetJsonAsync<T>(this IHttpResponse response) where T : class
        {
            using var stream = await response.ReadStreamAsync();
            using var streamReader = new StreamReader(stream);
            using var jsonTextReader = new JsonTextReader(streamReader);
            return new JsonSerializer().Deserialize<T>(jsonTextReader);
        }
    }
}
