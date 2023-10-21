using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pentomino.ValueConverters;


static class ConverterHelpers
{
    public static bool AsBool(this object value)
    {
        if (value == null)
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
}

