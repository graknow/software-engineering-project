using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace MealShareDotNet.Client.Converters;

public class StandardUnitConverter : IValueConverter
{
    public static readonly StandardUnitConverter Instance = new();

    public enum UnitType
    {
        MASS = 0,
        VOLUME,
        QUANTITY
    }

    public readonly IDictionary<string, long> VolumeConversions = new Dictionary<string, long> {
        { "Tsp", 50 },
        { "Tbsp", 148 },
        { "Cup", 2366 },
        { "Gal", 37854 },
        { "Pt", 4732 },
        { "L", 1000 },
        { "mL", 10 },
    };

    public readonly IDictionary<string, long> MassConversions = new Dictionary<string, long> {
        { "kg", 10000 },
        { "Oz", 2835 },
        { "g", 10 },
        { "Lb", 45359 },
    };

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (long.TryParse(value as string, out long standardMeasure) && parameter is string unit
            && targetType.IsAssignableTo(typeof(long)))
        {
            if (VolumeConversions.TryGetValue(unit, out long volumeScale))
            {
                return Math.Round((double)standardMeasure / volumeScale, MidpointRounding.AwayFromZero);
            }
            else if (MassConversions.TryGetValue(unit, out long massScale))
            {
                return Math.Round((double) standardMeasure / massScale, MidpointRounding.AwayFromZero);
            }
            else
            {
                return "err";
            }
        }

        return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
    }

    public string BestUnitMatch(long value, UnitType t)
    {
        IDictionary<string, long> conversionTable;

        if (t == UnitType.MASS)
        {
            conversionTable = MassConversions;
        }
        else if (t == UnitType.VOLUME)
        {
            conversionTable = VolumeConversions;
        }
        else
        {
            return "ct";
        }

        string bestMatch = "";
        decimal bestMatchValue = 0;

        foreach (var (unit, scale) in conversionTable)
        {
            var unitValue = value / scale;

            if (string.IsNullOrEmpty(bestMatch))
            {
                bestMatch = unit;
            }
            else if (unitValue > 1 && bestMatchValue > 1)
            {
                if (unitValue < bestMatchValue)
                {
                    bestMatch = unit;   
                }
            }
            else if (unitValue > 1 && bestMatchValue < 1)
            {
                bestMatch = unit;
            }
            else if (unitValue < 1 && bestMatchValue < 1)
            {
                if (unitValue > bestMatchValue)
                {
                    bestMatch = unit;
                }
            }
        }

        return bestMatch;
    }

    public string GetUnitMeasurement(long standardMeasure, string unit)
    {
        if (VolumeConversions.TryGetValue(unit, out long volumeScale))
        {
            return string.Format("{0:0.##}", (double)standardMeasure / volumeScale);
        }
        else if (MassConversions.TryGetValue(unit, out long massScale))
        {
            // i once flipped these around and saw something i shouldnt have
            return string.Format("{0:0.##}", (double)standardMeasure / massScale);
        }
        else
        {
            return "err";
        }
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}