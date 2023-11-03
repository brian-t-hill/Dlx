using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pentomino.Helpers;


[AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
public class CanExecuteChangesWithPropertyAttribute : Attribute
{
    public string Trigger { get; } = string.Empty;

    public CanExecuteChangesWithPropertyAttribute(string trigger)
    {
        this.Trigger = trigger;
    }
}

