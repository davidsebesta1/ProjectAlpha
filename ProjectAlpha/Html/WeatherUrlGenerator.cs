
namespace ProjectAlpha.Html
{
    /// <summary>
    /// Generator for the weather urls per day.
    /// </summary>
    public class WeatherUrlGenerator
    {
        private readonly string _baseUrl;
        private readonly DateTime _startDate;
        private readonly DateTime _endDate;
        private DateTime _currentDate;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="baseUrl">Base url up to the station ID.</param>
        /// <param name="startDate">Start time.</param>
        /// <param name="endDate">End time.</param>
        public WeatherUrlGenerator(string baseUrl, DateTime startDate, DateTime endDate)
        {
            _baseUrl = baseUrl;
            _startDate = startDate;
            _endDate = endDate;

            _currentDate = _startDate;
        }

        /// <summary>
        /// Gets the station urls from start date to end date.
        /// </summary>
        /// <param name="start">Start date.</param>
        /// <param name="end">End date.</param>
        /// <returns>Urls per day.</returns>
        public async IAsyncEnumerable<WeatherUrl> GetUrls()
        {
            while (_currentDate <= _endDate)
            {
                await Task.Delay(2000 + Random.Shared.Next(500));

                string curDateString = _currentDate.ToString("yyyy-MM-dd");
                yield return new WeatherUrl($"{_baseUrl}/table/{curDateString}/{curDateString}/daily", _currentDate);

                _currentDate = _currentDate.AddDays(1);
            }
        }

    }

    /// <summary>
    /// Struct for holding weather url data.
    /// </summary>
    public readonly struct WeatherUrl : IEquatable<WeatherUrl>
    {
        /// <summary>
        /// Full url including day.
        /// </summary>
        public readonly string Url;

        /// <summary>
        /// Date for which this url was created.
        /// </summary>
        public readonly DateTime Date;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="url">Url.</param>
        /// <param name="time">Date.</param>
        public WeatherUrl(string url, DateTime time)
        {
            Url = url;
            Date = time;
        }

        public override bool Equals(object? obj)
        {
            return obj is WeatherUrl url && Equals(url);
        }

        public bool Equals(WeatherUrl other)
        {
            return Url == other.Url && Date == other.Date;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Url, Date);
        }

        public override string? ToString()
        {
            return Url + " at date " + Date.ToString("dd-MM-yyyy");
        }

        public static bool operator ==(WeatherUrl left, WeatherUrl right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(WeatherUrl left, WeatherUrl right)
        {
            return !(left == right);
        }
    }
}
