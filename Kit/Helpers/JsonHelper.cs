using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace Chubrik.Kit
{
    public static class JsonHelper
    {
        private static readonly JsonSerializerOptions _options =
            new() { Encoder = JavaScriptEncoder.Create(UnicodeRanges.All), PropertyNameCaseInsensitive = true };

        public static string Serialize<T>(T obj) => JsonSerializer.Serialize(obj, _options);

        public static void Serialize<T>(T obj, Stream target) =>
            Task.Factory.StartNew(() => JsonSerializer.SerializeAsync(target, obj, _options).Wait()).Wait();

        public static T Deserialize<T>(string json) where T : class, new() =>
            JsonSerializer.Deserialize<T>(json, _options) ?? throw new InvalidOperationException();

        public static T Deserialize<T>(Stream source) where T : class, new() =>
            Task.Factory.StartNew(() => JsonSerializer.DeserializeAsync<T>(source, _options).Result).Result
                ?? throw new InvalidOperationException();
    }
}
