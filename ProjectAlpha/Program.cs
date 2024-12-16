/// <author>
/// David Šebesta
/// </author>

using ProjectAlpha.Html;
using ProjectAlpha.Logging;
using ProjectAlpha.Yaml;
using System.Reflection;
using YamlDotNet.Core;

namespace ProjectAlpha
{
    public class Program
    {
        /// <summary>
        /// Instance of the config.
        /// </summary>
        public static Config Config { get; private set; }

        /// <summary>
        /// Path to the directory the final executable file is.
        /// </summary>
        public static string WorkingDirectoryPath { get; private set; }

        /// <summary>
        /// Entry point.
        /// </summary>
        public static async Task Main(string[] args)
        {
            await Logger.Log("Loading config");

            await LoadConfigAsync();

            await Logger.Log("Done!");

            await Logger.Log("Processing stations...");

            await ProcessStations();

            await Logger.Log("Done!", ConsoleColor.Green);
        }

        /// <summary>
        /// Loads and validates the config file.
        /// </summary>
        public static async Task LoadConfigAsync()
        {
            string dllLocation;
            string configPath;
            string workingDir = string.Empty;

            try
            {
                dllLocation = Extensions.GetExecutablePath();
                WorkingDirectoryPath = Path.GetDirectoryName(dllLocation);
                configPath = Path.Combine(workingDir, "config.yaml");
                Config = await YamlParser.TryReadConfig<Config>(configPath);
            }
            catch (YamlException yamlException)
            {
                await Logger.Log($"Error at reading config, fix any errors and try again!\nMessage: {yamlException.Message}");
                Environment.Exit(1);
            }
            catch (Exception ex)
            {
                await Logger.Log($"Fatal at reading config: {ex.Message}");
                Environment.Exit(1);
            }

            await Logger.Log("Checking config integrity...");

            if (Config.EndTime < Config.StartTime)
            {
                await Logger.Log($"Configuration error! End time is earlier than start time.");
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Processes all stations.
        /// </summary>
        public static async Task ProcessStations()
        {
            try
            {
                List<Task> tasks = new List<Task>(Config.StationUrls.Count);
                using (DataDownloader downloader = new DataDownloader())
                {
                    foreach (string url in Config.StationUrls)
                    {
                        tasks.Add(Task.Run(async () =>
                        {
                            if (!downloader.IsValidBaseUrl(url))
                            {
                                await Logger.Log($"Url {url} is not a valid weather station url. Please fix it.", ConsoleColor.Yellow);
                            }

                            string stationName = url.Split('/').Last();
                            string resultFileName = $"weather-{stationName}-{Config.StartTime.ToString("dd/MM/yyyy")}-{Config.EndTime.ToString("dd/MM/yyyy")}.csv";
                            string targetFilePath = Path.Combine(WorkingDirectoryPath, resultFileName);

                            await Logger.Log($"Starting processing {url}", ConsoleColor.Magenta);
                            using (WeatherHtmlParser parser = new WeatherHtmlParser(url, downloader))
                            {
                                using (StreamWriter sw = new StreamWriter(File.OpenWrite(targetFilePath)))
                                {
                                    await sw.WriteLineAsync(WeatherData.CsvHeader);
                                    await foreach (WeatherData weather in parser.GetWeatherData())
                                    {
                                        await sw.WriteLineAsync(weather.GetCsvRow());
                                    }
                                }
                            }
                        }));
                    }

                    await Task.WhenAll(tasks);
                }
            }
            catch (HttpRequestException e)
            {
                await Logger.Log($"Error occured while downloading data: {e.Message}. Make sure you are connected to the internet and the webpage is available from your location.");
            }
            catch (Exception e)
            {
                await Logger.LogError(e);
            }
        }
    }
}
