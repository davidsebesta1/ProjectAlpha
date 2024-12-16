using ProjectAlpha.DataSerialization;
using ProjectAlpha.DataSerialization.Interfaces;
using ProjectAlpha.Pool;
using System.Text;

namespace ProjectAlpha
{
    public readonly struct WeatherData : ICsvSerializable, IEquatable<WeatherData>
    {
        /// <inheritdoc/>
        public static string CsvHeader => $"Time_Taken,Temperature_{(Program.Config.UseMetricUnits ? "C" : "F")},Humidity_Percent,WindDirection,WindSpeed_{(Program.Config.SpeedInMetersPerSecond ? "MetersPerSecond" : "KilometersPerHour")},Pressure_{(Program.Config.UseMetricUnits ? "hPa" : "PSI")}";

        public readonly DateTime TimeTaken;
        public readonly double Temperature;
        public readonly byte HumidityPercent;
        public readonly string WindDirection;
        public readonly double WindSpeed;
        public readonly double Pressure;

        /// <summary>
        /// Constructor.
        /// </summary>
        public WeatherData(DateTime timeTaken, double temperature, byte humidity, string windDirection, double windSpeed, double pressure)
        {
            TimeTaken = timeTaken;
            Temperature = temperature;
            HumidityPercent = humidity;
            WindDirection = windDirection;
            WindSpeed = windSpeed;
            Pressure = pressure;
        }

        /// <inheritdoc/>
        public string GetCsvRow()
        {
            StringBuilder sb = StringBuilderPool.Get();
            sb.Append(UnitConverter.ConvertTime(TimeTaken, UnitConverter.UsedTimeFormat));
            sb.Append(',');

            sb.Append(Temperature.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture));
            sb.Append(',');

            sb.Append(HumidityPercent);
            sb.Append(',');

            sb.Append(WindDirection);
            sb.Append(',');

            if (Program.Config.SpeedInMetersPerSecond)
                sb.Append(WindSpeed.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture));
            else
                sb.Append(WindSpeed.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture));

            sb.Append(',');

            if (Program.Config.UseMetricUnits)
                sb.Append(Pressure.ToString("0", System.Globalization.CultureInfo.InvariantCulture));
            else
                sb.Append(Pressure.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture));

            return StringBuilderPool.ReturnToString(sb);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(TimeTaken, Temperature, HumidityPercent, WindDirection, WindSpeed, Pressure);
        }

        public override string? ToString()
        {
            return $"Taken at: {TimeTaken}";
        }

        public override bool Equals(object? obj)
        {
            return obj is WeatherData data && Equals(data);
        }

        public bool Equals(WeatherData other)
        {
            return TimeTaken == other.TimeTaken &&
                   Temperature == other.Temperature &&
                   HumidityPercent == other.HumidityPercent &&
                   WindDirection == other.WindDirection &&
                   WindSpeed == other.WindSpeed &&
                   Pressure == other.Pressure;
        }

        public static bool operator ==(WeatherData left, WeatherData right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(WeatherData left, WeatherData right)
        {
            return !(left == right);
        }
    }
}
