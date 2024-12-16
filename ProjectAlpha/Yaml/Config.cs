
namespace ProjectAlpha.Yaml
{
    public class Config
    {
        /// <summary>
        /// Start time of the data processing.
        /// </summary>
        public DateTime StartTime { get; set; } = DateTime.Now.AddDays(-1);

        /// <summary>
        /// End time of the data processing.
        /// </summary>
        public DateTime EndTime { get; set; } = DateTime.Now;

        /// <summary>
        /// Whether the program is using metric or imperial units.
        /// </summary>
        public bool UseMetricUnits { get; set; } = true;

        /// <summary>
        /// Whether the windspeed km/h will comvert to m/s.
        /// </summary>
        public bool SpeedInMetersPerSecond { get; set; } = true;

        /// <summary>
        /// Whether the time should be converted to AM/PM.
        /// </summary>
        public bool Use24HourFormat { get; set; } = true;

        /// <summary>
        /// Stations urls to be scrapped.
        /// </summary>
        public List<string> StationUrls { get; set; } = ["https://www.wunderground.com/dashboard/pws/IPRAGU891", "https://www.wunderground.com/dashboard/pws/IPRAGU1347"];

        /// <summary>
        /// Parameterless constructor.
        /// </summary>
        public Config()
        {

        }
    }
}
