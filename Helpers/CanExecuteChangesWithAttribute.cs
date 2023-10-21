using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pentomino.Helpers;


[AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
public class CanExecuteChangesWithAttribute : Attribute
{
    public string Trigger { get; } = string.Empty;

    public CanExecuteChangesWithAttribute(string trigger)
    {
        this.Trigger = trigger;
    }
}

