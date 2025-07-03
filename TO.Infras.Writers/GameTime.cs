using Newtonsoft.Json;
using TO.Commons.Configs;
using TO.Commons.Enums;

namespace Infras.Writers
{
    
    /// <summary>
    /// 轻量级游戏时间结构
    /// 保持不可变性同时优化性能
    /// </summary>
    [Serializable]
    public struct GameTime 
    {
        [JsonProperty("year")]
        public int Year { get; }
        
        [JsonProperty("month")] 
        public int Month { get; }
        
        [JsonProperty("day")]
        public int Day { get; }
        
        [JsonProperty("hour")]
        public int Hour { get; }
        
        [JsonProperty("minute")]
        public int Minute { get; }

        /// <summary>
        /// 获取当前季节（北半球气象季节）
        /// </summary>
        [JsonIgnore]
        public Season Season
        {
            get
            {
                return Month switch
                {
                    3 or 4 or 5 => Season.Spring,
                    6 or 7 or 8 => Season.Summer,
                    9 or 10 or 11 => Season.Autumn,
                    12 or 1 or 2 => Season.Winter,
                    _ => throw new ArgumentOutOfRangeException(nameof(Month), "Invalid month value")
                };
            }
        }

        public GameTime(int year = 0, int month = 1, int day = 1, int hour = 6, int minute = 0)
        {
            // 参数验证
            if (month is < 1 or > TimeConfig.MonthsPerYear)
                throw new ArgumentOutOfRangeException(nameof(month));
            if (day is < 1 or > TimeConfig.DaysPerMonth)
                throw new ArgumentOutOfRangeException(nameof(day));
            if (hour is < 0 or >= TimeConfig.HoursPerDay)
                throw new ArgumentOutOfRangeException(nameof(hour));
            if (minute is < 0 or >= TimeConfig.MinutesPerHour)
                throw new ArgumentOutOfRangeException(nameof(minute));

            Year = year;
            Month = month;
            Day = day;
            Hour = hour;
            Minute = minute;
        }
        

        /// <summary>
        /// 格式化时间字符串
        /// </summary>
        /// <param name="format">格式字符串</param>
        public string ToString(string format)
        {
            return $"{Year}-{Month:D2}-{Day:D2} {Hour:D2}:{Minute:D2}";
        }

        public override string ToString()
        {
            return $"{Year}-{Month:D2}-{Day:D2} {Hour:D2}:{Minute:D2}";
        }

        private long GetTotalMinutes()
        {
            
            return (long)Year * TimeConfig.MonthsPerYear * TimeConfig.DaysPerMonth *
                   TimeConfig.HoursPerDay * TimeConfig.MinutesPerHour
                   + (Month - 1L) * TimeConfig.DaysPerMonth * TimeConfig.HoursPerDay *
                TimeConfig.MinutesPerHour
                + (Day - 1L) * TimeConfig.HoursPerDay * TimeConfig.MinutesPerHour
                + Hour * TimeConfig.MinutesPerHour
                + Minute;
        }

        /// <summary>
        /// 计算完整时间差值(返回新的GameTime)
        /// </summary>
        public GameTime GetDifference(GameTime other)
        {
            long diffMinutes = Math.Abs(GetTotalMinutes() - other.GetTotalMinutes());
            int years = (int)(diffMinutes / (TimeConfig.MonthsPerYear * TimeConfig.DaysPerMonth *
                                             TimeConfig.HoursPerDay * TimeConfig.MinutesPerHour));
            diffMinutes %= TimeConfig.MonthsPerYear * TimeConfig.DaysPerMonth * TimeConfig.HoursPerDay *
                           TimeConfig.MinutesPerHour;

            int months =
                (int)(diffMinutes / (TimeConfig.DaysPerMonth * TimeConfig.HoursPerDay * TimeConfig.MinutesPerHour)) +
                1;
            diffMinutes %= TimeConfig.DaysPerMonth * TimeConfig.HoursPerDay * TimeConfig.MinutesPerHour;

            int days = (int)(diffMinutes / (TimeConfig.HoursPerDay * TimeConfig.MinutesPerHour)) + 1;
            diffMinutes %= TimeConfig.HoursPerDay * TimeConfig.MinutesPerHour;

            int hours = (int)(diffMinutes / TimeConfig.MinutesPerHour);
            int minutes = (int)(diffMinutes % TimeConfig.MinutesPerHour);

            return new GameTime(years, months, days, hours, minutes);
        }

        /// <summary>
        /// 获取总分钟差值
        /// </summary>
        public int GetTotalMinuteDifference(GameTime other)
        {
            return (int)Math.Abs(GetTotalMinutes() - other.GetTotalMinutes());
        }

        /// <summary>
        /// 获取总小时差值
        /// </summary>
        public int GetTotalHourDifference(GameTime other)
        {
            return GetTotalMinuteDifference(other) / TimeConfig.MinutesPerHour;
        }

        /// <summary>
        /// 获取总天数差值
        /// </summary>
        public int GetTotalDayDifference(GameTime other)
        {
            return GetTotalMinuteDifference(other) / 
                   (TimeConfig.HoursPerDay * TimeConfig.MinutesPerHour);
        }

        /// <summary>
        /// 获取年份差值
        /// </summary>
        public int GetYearDifference(GameTime other)
        {
            return GetDifference(other).Year;
        }

        /// <summary>
        /// 获取月份差值
        /// </summary>
        public int GetMonthDifference(GameTime other)
        {
            return GetDifference(other).Month;
        }

        /// <summary>
        /// 获取天数差值
        /// </summary>
        public int GetDayDifference(GameTime other)
        {
            return GetDifference(other).Day;
        }

        /// <summary>
        /// 获取小时差值
        /// </summary>
        public int GetHourDifference(GameTime other)
        {
            return GetDifference(other).Hour;
        }

        /// <summary>
        /// 获取分钟差值
        /// </summary>
        public int GetMinuteDifference(GameTime other)
        {
            return GetDifference(other).Minute;
        }
    }
}