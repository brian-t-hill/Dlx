using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace Pentomino.ValueConverters;


public class BooleanToVisibilityConverter : IValueConverter
{
    public Visibility TrueVisibility { get; set; } = Visibility.Visible;

    public Visibility FalseVisibility { get; set; } = Visibility.Collapsed;


    public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (value.AsBool() ? this.TrueVisibility : this.FalseVisibility);
    }

    public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        Visibility visibility = (Visibility) value;

        if (this.TrueVisibility == visibility)
            return true;

        return false;
    }
}



public class VisibleIfMatchesConverter<T> : BooleanToVisibilityConverter where T : struct
{
    public IEquatable<T>? ParameterToMatch { get; set; } = null;

    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (this.ParameterToMatch is null || value is not T t)
            return false;

        return base.Convert(this.ParameterToMatch.Equals(t), targetType, parameter, culture);
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

}


public class VisibleIfMatchesIntConverter : VisibleIfMatchesConverter<int>
{
}


