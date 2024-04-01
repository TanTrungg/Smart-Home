using ISHE_Utility.Exceptions;
using System.Globalization;

namespace ISHE_Utility.Helpers.FormatDate
{
    public static class FormatDate
    {
        public static DateTime CheckFormatDate(string date)
        {
            DateTime result;
            if (!DateTime.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
            {
                throw new ConflictException("Vui lòng nhập đúng định dạng ngày (yyyy-MM-dd).");
            }
            return result;
        }
    }
}
