using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pentomino.Helpers;


[AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
public class NotifiesWithPropertyAttribute : Attribute
{
    public string Trigger { get; } = string.Empty;

    public NotifiesWithPropertyAttribute(string trigger)
    {
        this.Trigger = trigger;
    }
}

