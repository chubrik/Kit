using Newtonsoft.Json;
using System;
using System.IO;

namespace Kit
{
    public static class JsonHelper
    {
        private static readonly JsonSerializer _serializer = new JsonSerializer();

        public static string Serialize(object obj) => JsonConvert.SerializeObject(obj);

        public static void Serialize(object obj, Stream target)
        {
            using var streamWriter = new StreamWriter(target);
            using var jsonTextWriter = new JsonTextWriter(streamWriter);
            _serializer.Serialize(jsonTextWriter, obj);
            jsonTextWriter.Close();
        }

        public static T Deserialize<T>(string value) where T : class
        {
            var obj = JsonConvert.DeserializeObject<T>(value);

            if (obj == null)
                throw new InvalidOperationException($"Wrong json content");

            return obj;
        }

        public static T Deserialize<T>(Stream source) where T : class
        {
            using var streamReader = new StreamReader(source);
            using var jsonTextReader = new JsonTextReader(streamReader);
            var obj = _serializer.Deserialize<T>(jsonTextReader);

            if (obj == null)
                throw new InvalidOperationException($"Wrong json content");

            return obj;
        }
    }
}
