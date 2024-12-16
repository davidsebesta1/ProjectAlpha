
namespace ProjectAlpha.DataSerialization.Interfaces
{
    /// <summary>
    /// Interface for serializing object into a CSV row.
    /// </summary>
    public interface ICsvSerializable
    {
        /// <summary>
        /// Header to be used in the CSV file as a first row.
        /// </summary>
        public static abstract string CsvHeader { get; }

        /// <summary>
        /// Get the string sepresentation for CSV.
        /// </summary>
        /// <returns></returns>
        public abstract string GetCsvRow();
    }
}
