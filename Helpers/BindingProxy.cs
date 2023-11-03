using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Pentomino.Helpers;

// This class was originally adapted from one written by Thomas Levesque and published at:
// https://www.thomaslevesque.com/2011/03/21/wpf-how-to-bind-to-data-when-the-datacontext-is-not-inherited/.
//
// Thomas indicates in his blog that "there is no restriction whatsoever on the use of this code, you can use it
// in any project you want."


// The point of this class is to make properties available for binding when they might otherwise be hidden.  There
// are several circumstances in which this might happen.  One that I run into a lot comes up when I use a UserControl.
// I like to bind a view model to its DataContext.  However, that makes all the other bindings come from its own
// view model.  If I want to bind some of its properties to the host object's view model, I can't.  At least, not
// directly.  The solution is to use a BindingProxy.  Put a BindingProxy object in the outer Resources and set its
// Data member to the thing that you want to bind.  Then instead of trying to bind to an unreachable property, bind
// to the object in the ResourceDictionary.

public class BindingProxy : Freezable
{
    #region Overrides of Freezable

    protected override Freezable CreateInstanceCore()
    {
        return new BindingProxy();
    }

    #endregion


    public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(object), typeof(BindingProxy), new UIPropertyMetadata(null));

    public object Data
    {
        get => GetValue(DataProperty);
        set => SetValue(DataProperty, value);
    }

}

