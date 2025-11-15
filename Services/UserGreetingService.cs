using System;

namespace ivaadg.Services
{
    public static class UserGreetingService
    {
        /// <summary>
        /// Получить приветствие в зависимости от времени суток
        /// 10:00-12:00 – Утро
        /// 12:01-17:00 – День
        /// 17:01-19:00 – Вечер
        /// </summary>
        public static string GetTimeOfDayGreeting()
        {
            var currentTime = DateTime.Now.TimeOfDay;
            var hour = currentTime.Hours;
            var minute = currentTime.Minutes;

            // 10:00 - 12:00 (включительно)
            if ((hour == 10 || hour == 11) || (hour == 12 && minute == 0))
            {
                return "Доброе утро";
            }
            // 12:01 - 17:00 (включительно)
            else if ((hour == 12 && minute > 0) || (hour >= 13 && hour < 17) || (hour == 17 && minute == 0))
            {
                return "Добрый день";
            }
            // 17:01 - 19:00 (включительно)
            else if ((hour == 17 && minute > 0) || hour == 18 || (hour == 19 && minute == 0))
            {
                return "Добрый вечер";
            }
            else
            {
                return "Здравствуйте";
            }
        }

        /// <summary>
        /// Проверить, находится ли текущее время в рабочих часах (10:00 - 19:00)
        /// </summary>
        public static bool IsWorkingHours()
        {
            var currentTime = DateTime.Now.TimeOfDay;
            var workStart = new TimeSpan(10, 0, 0);  // 10:00
            var workEnd = new TimeSpan(19, 0, 0);    // 19:00

            return currentTime >= workStart && currentTime <= workEnd;
        }

        /// <summary>
        /// Получить полное приветствие для пользователя
        /// </summary>
        public static string GetFullGreeting(string fullName)
        {
            return $"{GetTimeOfDayGreeting()}!\n{fullName}";
        }
    }
}
