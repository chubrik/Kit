using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace Kit
{
    public static class JsonHelper
    {
        private static readonly JsonSerializerOptions _options =
            new JsonSerializerOptions { Encoder = JavaScriptEncoder.Create(UnicodeRanges.All), PropertyNameCaseInsensitive = true };

        public static string Serialize<T>(T obj) => JsonSerializer.Serialize(obj, _options);

        public static void Serialize<T>(T obj, Stream target) =>
            Task.Factory.StartNew(() => JsonSerializer.SerializeAsync(target, obj, _options).Wait()).Wait();

        public static dynamic Deserialize(string json) => Deserialize<object>(json);

        public static dynamic Deserialize(Stream source) => Deserialize<object>(source);

        public static T Deserialize<T>(string json) where T : class, new() => JsonSerializer.Deserialize<T>(json, _options);

        public static T Deserialize<T>(Stream source) where T : class, new() =>
            Task.Factory.StartNew(() => JsonSerializer.DeserializeAsync<T>(source, _options).Result).Result;
    }
}
