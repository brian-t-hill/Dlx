using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pentomino.Helpers;



[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class CalledWhenPropertyChangesAttribute : Attribute
{
    public string Trigger { get; } = string.Empty;

    public CalledWhenPropertyChangesAttribute(string trigger)
    {
        this.Trigger = trigger;
    }
}

