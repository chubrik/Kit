using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace Kit.Http
{
    public static class HttpResponseExtensions
    {
        public static dynamic? ReadJson(this IHttpResponse response) =>
            Task.Run(() => response.ReadJsonAsync<object>()).Result;

        public static Task<dynamic?> ReadJsonAsync(this IHttpResponse response) =>
            response.ReadJsonAsync<object>();

        public static T? ReadJson<T>(this IHttpResponse response) where T : class =>
            Task.Run(() => response.ReadJsonAsync<T>()).Result;

        public static async Task<T?> ReadJsonAsync<T>(this IHttpResponse response) where T : class
        {
            using var stream = await response.ReadStreamAsync();
            using var streamReader = new StreamReader(stream);
            using var jsonTextReader = new JsonTextReader(streamReader);
            return new JsonSerializer().Deserialize<T>(jsonTextReader);
        }
    }
}
