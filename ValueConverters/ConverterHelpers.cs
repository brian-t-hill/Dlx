using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pentomino.ValueConverters;


static class ConverterHelpers
{
    public static bool AsBool(this object? value)
    {
        if (value is null)
            return false;

        bool boolValue = false;

        try
        {
            boolValue = (bool) value;
        }
        catch (Exception)
        {
            boolValue = (0 != (int) value);
        }

        return boolValue;
    }


    public static int AsInt(this object? value)
    {
        if (value is null)
            return 0;

        int intValue = 0;

        try
        {
            intValue = (int) value;
        }
        catch (Exception)
        {
        }

        return intValue;
    }

}

