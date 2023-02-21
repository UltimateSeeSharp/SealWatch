using System.Runtime.CompilerServices;

namespace SealWatch.Code.Extensions;

internal static class DateFormatter
{
    internal static DateTime RemoveTime(this DateTime dateTime)
    {
        return new(dateTime.Year, dateTime.Month, dateTime.Day);
    }
}