using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pentomino.Helpers;



public static class EventExtensions
{
    public static void OnPropertyChanged(this INotifyPropertyChanged source, string? propertyName, Action<object?, PropertyChangedEventArgs> fnWhenChanged)
    {
        source.PropertyChanged += (sender, args) =>
        {
            if (string.IsNullOrEmpty(args.PropertyName) || string.IsNullOrEmpty(propertyName) || args.PropertyName.Equals(propertyName, StringComparison.InvariantCulture))
                fnWhenChanged(sender, args);
        };
    }
}


