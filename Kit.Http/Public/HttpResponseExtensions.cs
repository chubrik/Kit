using System.Threading.Tasks;

namespace Kit.Http
{
    public static class HttpResponseExtensions
    {
        public static dynamic ReadObject(this IHttpResponse response) =>
            Task.Run(() => response.ReadObjectAsync<object>()).Result;

        public static Task<dynamic> ReadObjectAsync(this IHttpResponse response) =>
            response.ReadObjectAsync<object>();

        public static T ReadObject<T>(this IHttpResponse response) where T : class, new() =>
            Task.Run(() => response.ReadObjectAsync<T>()).Result;

        public static async Task<T> ReadObjectAsync<T>(this IHttpResponse response) where T : class, new()
        {
            using var readStream = await response.ReadStreamAsync();
            return JsonHelper.Deserialize<T>(readStream);
        }
    }
}
