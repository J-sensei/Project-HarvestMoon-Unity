using UnityEngine;

namespace GameDateTime
{
    /// <summary>
    /// Season of the game, increment after maximum day is reached
    /// </summary>
    public enum Season
    {
        Spring,
        Summer,
        Fall,
        Winter
    }

    /// <summary>
    /// Information about in game time
    /// </summary>
    [System.Serializable]
    public class GameTime
    {
        public const int MAX_DAY = 3;
        public const int MAX_HOUR = 24;
        public const int MAX_MINUTE = 60;

        [Tooltip("Current season of the game time")]
        [SerializeField] private Season _currentSeason;
        [Tooltip("Current year count of the game time")]
        [SerializeField] private int _year;
        [Tooltip("Current day of the game time")]
        [SerializeField] private int _day;
        [Tooltip("Current hour of the game time")]
        [SerializeField] private int _hour;
        [Tooltip("Current minute of the game time")]
        [SerializeField] private int _minute;

        public Season CurrentSeason
        {
            get { return _currentSeason; }
        }
        public int Year
        {
            get { return _year; }
        }
        public int Day
        {
            get { return _day; }
        }
        public int Hour
        {
            get { return _hour; }
        }
        public int Minute
        {
            get { return _minute; }
        }

        public GameTime(int year, Season season, int day, int hour, int minute)
        {
            _year = year;
            _currentSeason = season;
            _day = day; 
            _hour = hour;
            _minute = minute;
        }

        /// <summary>
        /// Reset to next day
        /// </summary>
        public void Reset()
        {
            // Still in same day
            if(_hour >= 6 && _hour <= 24)
            {
                _day++;
            }

            _hour = 5;
            _minute = 59;
        }

        /// <summary>
        /// Increate the time by a minute
        /// </summary>
        public void IncreaseTime()
        {
            _minute++;
            if(_minute >= MAX_MINUTE)
            {
                _minute = 0;
                _hour++;
            }

            if(_hour >= MAX_HOUR)
            {
                _hour = 0;
                _day++;
            }

            if(_day > MAX_DAY)
            {
                _day = 1;

                if(_currentSeason == Season.Winter)
                {
                    _year++;
                    _currentSeason = Season.Spring;
                }
                else
                {
                    _currentSeason++;
                }
            }
        }

        /// <summary>
        /// Get the correct format of the string to display the time UI
        /// </summary>
        /// <returns></returns>
        public string TimeString()
        {
            int hrs = _hour;
            int min = _minute;

            string prefix = "AM";
            if(hrs > 12)
            {
                prefix = "PM";
                hrs -= 12;
            }

            return hrs.ToString("00") + ":" + min.ToString("00") + " " + prefix;
        }

        #region Convertion
        public static int HoursToMinutes(int hours)
        {
            return hours * MAX_MINUTE;
        }
        public static int DayToHours(int days)
        {
            return days * MAX_HOUR;
        }
        public static int SeasonsToDays(Season season)
        {
            int s = (int)season;
            return s * MAX_DAY;
        }
        #endregion
    }
}
