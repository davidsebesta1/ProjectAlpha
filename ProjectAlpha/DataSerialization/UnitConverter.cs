
namespace ProjectAlpha.DataSerialization
{
    /// <summary>
    /// A static class to convert between metric and imperial units.
    /// </summary>
    public static class UnitConverter
    {
        public const double SpeedConversionConstant = 3.6d;
        public const double PressureConversionConstant = 68.9476d;

        public static string DateTimeFormat12H => "dd/MM/yyyy h:mm tt";
        public static string DateTimeFormat24H => "dd/MM/yyyy HH:mm";

        public static TimeType UsedTimeFormat => Program.Config.Use24HourFormat ? TimeType.TwentyFourHours : TimeType.TwelveHours;

        /// <summary>
        /// Converts temperature between Fahrenheit and Celsius.
        /// </summary>
        /// <param name="value">Original value.</param>
        /// <param name="targetType">Target type.</param>
        /// <returns>Converted and rounded to 2 decimal places.</returns>
        public static double ConvertTemperature(double value, TemperatureType targetType)
        {
            return targetType switch
            {
                TemperatureType.Metric => Math.Round((value - 32) * (5 / 9d), 2),
                TemperatureType.Imperial => Math.Round((value * (9 / 5d)) + 32, 2),
                _ => throw new ArgumentException("Error at converting temperature, unknown unit type."),
            };
        }

        /// <summary>
        /// Converts wind speed between m/s and km/h.
        /// </summary>
        /// <param name="value">Original value.</param>
        /// <param name="targetType">Target type.</param>
        /// <returns>Converted and rounded value.</returns>
        public static double ConvertWindSpeed(double value, WindSpeedType targetType)
        {
            return targetType switch
            {
                WindSpeedType.MetersPerSecond => Math.Round(value / SpeedConversionConstant, 3),
                WindSpeedType.KilometersPerHour => Math.Round(value * SpeedConversionConstant, 2),
                _ => throw new ArgumentException("Error at converting wind speed, unknown unit type."),
            };
        }

        /// <summary>
        /// Converts pressure between PSI and hPa.
        /// </summary>
        /// <param name="value">Original value.</param>
        /// <param name="targetType">Target type.</param>
        /// <returns>Converted and rounded value.</returns>
        public static double ConvertPressure(double value, PressureType targetType)
        {
            return targetType switch
            {
                PressureType.PSI => Math.Round(value / PressureConversionConstant, 2),
                PressureType.hPa => Math.Ceiling(value * PressureConversionConstant),
                _ => throw new ArgumentException("Error at converting pressure, unknown unit type."),
            };
        }

        /// <summary>
        /// Converts the <see cref="DateTime"/> to 12 or 24H format.
        /// </summary>
        /// <param name="time">Original value.</param>
        /// <param name="targetType">Target type.</param>
        /// <returns>String represention of the time.</returns>
        public static string ConvertTime(DateTime time, TimeType targetType)
        {
            return targetType switch
            {
                TimeType.TwelveHours => time.ToString(DateTimeFormat12H),
                TimeType.TwentyFourHours => time.ToString(DateTimeFormat24H),
                _ => throw new ArgumentException("Error at converting time, unknown unit type."),
            };
        }
    }

    public enum TemperatureType
    {
        Metric,
        Imperial
    }

    public enum WindSpeedType
    {
        MetersPerSecond,
        KilometersPerHour
    }

    public enum PressureType
    {
        PSI,
        hPa
    }

    public enum TimeType
    {
        TwelveHours,
        TwentyFourHours
    }
}
