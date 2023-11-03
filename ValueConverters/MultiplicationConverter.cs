using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;
using System.Numerics;

namespace Pentomino.ValueConverters;


public class MultiplicationConverter<T> : IValueConverter where T : struct, IMultiplyOperators<T, T, T>, IDivisionOperators<T, T, T>
{
    public T Factor { get; set; } = default;

    public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value.AsT<T>() * this.Factor;
    }

    public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value.AsT<T>() / this.Factor;
    }
}


public class MultiplyWithParameterConverter<T> : IValueConverter where T : struct, IMultiplyOperators<T, T, T>, IDivisionOperators<T, T, T>
{
    public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value.AsT<T>() * parameter.AsT<T>();
    }

    public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value.AsT<T>() / parameter.AsT<T>();
    }
}


public class IntMultiplyWithParameterConverter : MultiplyWithParameterConverter<int>
{
}


public class DoubleMultiplyWithParameterConverter : MultiplyWithParameterConverter<double>
{
}

