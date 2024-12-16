using ProjectAlpha.DataSerialization.Interfaces;

namespace ProjectAlpha.DataSerialization
{
    /// <summary>
    /// Class used for serializing items into a CSV file.
    /// </summary>
    public static class CsvSerializer
    {
        /// <summary>
        /// Writes into a file at specified file.
        /// </summary>
        /// <typeparam name="T">Type of object to be serialized.</typeparam>
        /// <param name="path">Absolute path to the file.</param>
        /// <param name="items">Items that are serialized as each row.</param>
        public static async Task WriteIntoCsv<T>(string path, IEnumerable<T> items) where T : ICsvSerializable
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                await sw.WriteLineAsync(T.CsvHeader);
                foreach (T item in items)
                {
                    await sw.WriteLineAsync(item.GetCsvRow());
                }
            }
        }
    }
}
