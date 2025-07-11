using Godot;
using TO.Commons.Enums;

namespace TO.Commons.Configs
{
    /// <summary>
    /// 时间系统配置
    /// </summary>
    public static partial class TimeConfig
    {
        /// <summary>
        /// 每小时分钟数
        /// </summary>
        public const int MinutesPerHour = 60;
        
        /// <summary>
        /// 每天小时数
        /// </summary>
        public const int HoursPerDay = 24;
        
        /// <summary>
        /// 每月天数
        /// </summary>
        public const int DaysPerMonth = 15;
        
        /// <summary>
        /// 每年月数
        /// </summary>
        public const int MonthsPerYear = 12;
        
        public struct TimePeriodConfig(float dawn, float day, float dusk, float night)
        {
            public float DawnStartHour = dawn;
            public float DayStartHour = day;
            public float DuskStartHour = dusk;
            public float NightStartHour = night;
        }

        private static readonly Dictionary<Season, TimePeriodConfig> SeasonalConfigs = new()
        {
            // 默认配置（适用于所有季节）
            [Season.Spring] = new TimePeriodConfig(5.0f, 6.0f, 18.0f, 20.0f),
            [Season.Summer] = new TimePeriodConfig(4.5f, 5.5f, 19.0f, 21.0f), // 夏季白天更长
            [Season.Autumn] = new TimePeriodConfig(5.5f, 6.5f, 17.5f, 19.5f), // 秋季白天稍短
            [Season.Winter] = new TimePeriodConfig(6.0f, 7.0f, 17.0f, 19.0f)  // 冬季白天最短
        };

        /// <summary>
        /// 获取当前季节的时间配置
        /// </summary>
        public static TimePeriodConfig GetCurrentConfig(Season season)
        {
            return SeasonalConfigs[season];
        }

        /// <summary>
        /// 设置自定义季节配置（可覆盖默认值）
        /// </summary>
        public static void SetSeasonConfig(Season season, TimePeriodConfig config)
        {
            SeasonalConfigs[season] = config;
        }
        
        
        public const int SpringStartMonth = 3; // 3月
        public const int SummerStartMonth = 6; // 6月
        public const int AutumnStartMonth = 9; // 9月
        public const int WinterStartMonth = 12; // 12月
        
     

        private static readonly Dictionary<Season, string> SeasonalLighting = new()
        {
            [Season.Spring] = "Spring",
            [Season.Summer] = "Summer",
            [Season.Autumn] = "Autumn",
            [Season.Winter] = "Winter"
        };

        /// <summary>
        /// 获取季节光照配置
        /// </summary>
        public static string GetSeasonLighting(Season season)
        {
            return SeasonalLighting[season];
        }

       
    }
}
