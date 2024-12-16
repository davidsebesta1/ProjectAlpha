using System.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace ProjectAlpha.Html
{
    /// <summary>
    /// A class responsible for validating url and sending request for the html file.
    /// </summary>
    public class DataDownloader : IDisposable
    {
        private readonly HttpClient _httpClient;
        private static readonly Regex _baseUrlRegex = new Regex("(https:\\/\\/)?(www.wunderground.com\\/dashboard\\/pws\\/[A-Za-z0-9]*)$");

        /// <summary>
        /// Constructor.
        /// </summary>
        public DataDownloader()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.0.0 Safari/537.36");
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xhtml+xml"));
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml", 0.9));
            _httpClient.DefaultRequestHeaders.AcceptLanguage.ParseAdd("en-US,en;q=0.9"); ;
        }

        /// <summary>
        /// Gets whether the provided base url amtches the base url regex.
        /// </summary>
        /// <param name="url">Base url.</param>
        /// <returns>Whether the base url matches the rege.x</returns>
        public bool IsValidBaseUrl(string url)
        {
            return _baseUrlRegex.IsMatch(url);
        }

        /// <summary>
        /// Sends the request to the weather data webpage.
        /// </summary>
        /// <param name="url">Target url.</param>
        /// <returns>Response as a string.</returns>
        public async Task<string> SendRequestAsync(string url)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Sends the requests and gets the result as a byte array.
        /// </summary>
        /// <param name="url">Target url.</param>
        /// <returns>Pooled char array.</returns>
        public async Task<TextReader> SendRequestReaderAsync(string url)
        {
            return new StreamReader(await _httpClient.GetStreamAsync(url));
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _httpClient.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
