using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pentomino.ValueConverters;


static class ConverterHelpers
{
    public static T AsT<T>(this object? value) where T : struct
    {
        if (value is null)
            return default;

        T t = default;

        try
        {
            t = (T) Convert.ChangeType(value, typeof(T));
        }
        catch (Exception)
        {
        }

        return t;
    }


    public static bool AsBool(this object? value)
    {
        return value.AsT<bool>();
    }


    public static int AsInt(this object? value)
    {
        return value.AsT<int>();
    }

}

