using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherBotV4Bot.Helpers
{
    public static class Constants
    {
        public readonly static string WeatherArgs = "WeatherEntities";
        public readonly static string LocationLabel = "Location";
        public readonly static string LocationPatternLabel = "Location_PatternAny";

        public static string OpenWeatherMapURL = $"http://api.openweathermap.org/data/2.5/weather";
        public readonly static string OpenWeatherMapKey = "6dd2ceb3f54fa1c4e5f0eb76f233c7cc";
    }
}
