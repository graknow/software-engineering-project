using System;

namespace MealShareDotNet.Client.Extensions;

public static class DateOnlyExtensions
{
    public static DateOnly StartOfWeek(this DateOnly d, DayOfWeek startOfWeek)
    {
        int diff = (7 + (d.DayOfWeek - startOfWeek)) % 7;
        return d.AddDays(-1 * diff);
    }
}