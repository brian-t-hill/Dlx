using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace Pentomino.ValueConverters;


public class IntToTrueConverter : IValueConverter
{
    public int ValueToTrue { get; set; } = 1;

    public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (value.AsInt() == this.ValueToTrue);
    }

    public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (value.AsBool() ? this.ValueToTrue : 0);
    }
}


