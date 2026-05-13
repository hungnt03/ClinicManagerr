using System.Globalization;
using System.Text.RegularExpressions;

namespace ClinicManager.Utils
{
    public static class DateTimeExtensions
    {
        public static (DateTime StartDate, DateTime EndDate) GetMonthRange(this int month, int year)
        {
            var startDate = new DateTime(year, month, 1);

            var endDate = startDate
                .AddMonths(1)
                .AddTicks(-1);

            return (startDate, endDate);
        }

        public static (DateTime StartDate, DateTime EndDate) GetMonthRange(this DateTime date)
        {
            var startDate = new DateTime(date.Year, date.Month, 1);

            var endDate = startDate
                .AddMonths(1)
                .AddTicks(-1);

            return (startDate, endDate);
        }

        public static DateTime? ExtractDate(this string text)
        {
            if (string.IsNullOrEmpty(text)) return null;
            var match = Regex.Match(text, @"\b\d{1,2}/\d{1,2}/\d{4}\b");
            if (match.Success)
            {
                if (DateTime.TryParse(match.Value, out var date))
                {
                    return date;
                }
            }
            return null;
        }

    }
}
