using HtmlAgilityPack;
using ProjectAlpha.DataSerialization;
using ProjectAlpha.Logging;
using System.Globalization;

namespace ProjectAlpha.Html
{
    /// <summary>
    /// Class responsible for parsing html data.
    /// </summary>
    public class WeatherHtmlParser : IDisposable
    {
        /// <summary>
        /// Date time used for exact date parsing from the web as it is in format H:MM AM/PM.
        /// </summary>
        public static string DateTimeFormatParse => "h:mm tt";

        /// <summary>
        /// XPath to the html table.
        /// </summary>
        public static string XPath => "/html[1]/body[1]/app-root[1]/app-dashboard[1]/one-column-layout[1]/wu-header[1]/sidenav[1]/mat-sidenav-container[1]/mat-sidenav-content[1]/div[2]/section[1]/section[1]/div[1]/div[1]/section[1]/div[1]/div[1]/div[1]/lib-history[1]/div[2]/lib-history-table[1]/div[1]/div[1]/div[1]/table[1]/tbody[1]";

        private string _baseUrl;
        private DataDownloader _downloader;
        private WeatherUrlGenerator _urlGenerator;

        public WeatherHtmlParser(string baseUrl, DataDownloader downloader)
        {
            _baseUrl = baseUrl;
            _downloader = downloader;
            _urlGenerator = new WeatherUrlGenerator(_baseUrl, Program.Config.StartTime, Program.Config.EndTime);
        }

        /// <summary>
        /// Yields each available data withing start and end date range.
        /// </summary>
        /// <returns></returns>
        public async IAsyncEnumerable<WeatherData> GetWeatherData()
        {
            await foreach (WeatherUrl url in _urlGenerator.GetUrls())
            {
                await Logger.Log($"Downloading data...", ConsoleColor.Magenta);

                HtmlDocument htmlSnippet = new HtmlDocument();
                using (TextReader reader = await _downloader.SendRequestReaderAsync(url.Url))
                {
                    htmlSnippet.Load(reader);
                }

                await foreach (WeatherData data in Parse(htmlSnippet.DocumentNode, url.Date))
                {
                    if (data.TimeTaken >= Program.Config.StartTime && data.TimeTaken <= Program.Config.EndTime)
                        yield return data;
                }

            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _baseUrl = null;
            _downloader = null;
            _urlGenerator = null;

            GC.SuppressFinalize(this);
        }

        private async IAsyncEnumerable<WeatherData> Parse(HtmlNode root, DateTime day)
        {
            HtmlNode item = root.SelectSingleNode(XPath);

            int rowNum = 0;

            await Logger.Log($"Starting parsing for day {day.ToString("dd-MM-yyyy")}");
            foreach (HtmlNode row in item.ChildNodes)
            {
                rowNum++;
                if (row.NodeType != HtmlNodeType.Element)
                    continue;

                WeatherData data;
                try
                {
                    data = ParseFromRow(row, day);
                }
                catch (Exception ex)
                {
                    await Logger.LogError(ex);
                    data = default;
                }


                if (data != default)
                    yield return data;
            }

            await Logger.Log($"Finished parsing {day.ToString("dd-MM-yyyy")}");
        }

        private WeatherData ParseFromRow(HtmlNode row, DateTime day)
        {
            DateTime time = DateTime.ParseExact(row.ChildNodes[0].FirstChild.InnerText, DateTimeFormatParse, CultureInfo.InvariantCulture);
            time = new DateTime(day.Year, day.Month, day.Day, time.Hour, time.Minute, time.Second);

            double temperature = double.Parse(row.ChildNodes[1].FirstChild.FirstChild.ChildNodes[3].InnerText, CultureInfo.InvariantCulture);
            if (Program.Config.UseMetricUnits)
                temperature = UnitConverter.ConvertTemperature(temperature, TemperatureType.Metric);

            byte humidityPercent = byte.Parse(row.ChildNodes[3].FirstChild.FirstChild.ChildNodes[3].InnerText);
            string windDirection = row.ChildNodes[4].FirstChild.InnerText;

            double windSpeed = double.Parse(row.ChildNodes[5].FirstChild.FirstChild.ChildNodes[3].InnerText, CultureInfo.InvariantCulture);
            if (Program.Config.SpeedInMetersPerSecond)
                windSpeed = UnitConverter.ConvertWindSpeed(windSpeed, WindSpeedType.MetersPerSecond);


            double pressure = double.Parse(row.ChildNodes[7].FirstChild.FirstChild.ChildNodes[3].InnerText, CultureInfo.InvariantCulture);
            if (Program.Config.UseMetricUnits)
                pressure = UnitConverter.ConvertPressure(pressure, PressureType.hPa);


            return new WeatherData(time, temperature, humidityPercent, windDirection, windSpeed, pressure);
        }
    }
}
