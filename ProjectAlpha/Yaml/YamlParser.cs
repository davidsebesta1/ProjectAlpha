using ProjectAlpha.Logging;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ProjectAlpha.Yaml
{
    /// <summary>
    /// Class made for parsing of a config file in yaml.
    /// </summary>
    public static class YamlParser
    {
        public static readonly IDeserializer Deserializer = new DeserializerBuilder().WithTypeConverter(new CustomDateTimeConverter()).WithNamingConvention(PascalCaseNamingConvention.Instance).Build();
        public static readonly ISerializer Serializer = new SerializerBuilder().WithTypeConverter(new CustomDateTimeConverter()).WithNamingConvention(PascalCaseNamingConvention.Instance).Build();

        /// <summary>
        /// Attempts to read and deserialize the config file or creates a new one and saved it.
        /// </summary>
        /// <typeparam name="T">Type of the config file.</typeparam>
        /// <param name="path">Absolute path to the file.</param>
        /// <returns>The configuration file as an object.</returns>
        public static async Task<T> TryReadConfig<T>(string path) where T : class
        {
            try
            {
                T obj = await ReadAndCreate<T>(path);
                return obj;
            }
            catch (FileNotFoundException)
            {
                T obj = (T)Activator.CreateInstance(typeof(T))!;
                await Save(obj, path);

                await Logger.Log("Warning, a new config instance has been created. You might want to stop the program and configure it?", ConsoleColor.Yellow);
                return obj;
            }
        }

        private static async Task<T> ReadAndCreate<T>(string path) where T : class
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("Path must not be null");

            if (!Path.Exists(path))
                throw new FileNotFoundException("Specified file not found");

            using (StreamReader reader = new StreamReader(path))
            {
                string content = await reader.ReadToEndAsync();
                return Deserializer.Deserialize<T>(content);
            }
        }

        private static async Task Save<T>(T conf, string path) where T : class
        {
            string serialized = Serializer.Serialize(conf);
            using (StreamWriter writer = new StreamWriter(File.OpenWrite(path)))
            {
                await writer.WriteAsync(serialized);
            }
        }
    }
}
