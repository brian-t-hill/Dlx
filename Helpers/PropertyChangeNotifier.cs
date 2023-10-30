using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Pentomino.Helpers;


// Note:  This class incorporates ideas that I originally picked up from Brian Genisio, many years ago.
// He originally published his view model class on codeplex.com, but I think that site may have since
// shut down.  He indicated that he considered the class in public domain, so I felt free to let it
// inspire me.  I've made various changes over the years and this particular implementation is a much
// simplified version.  Thanks, Brian Genisio, for your suggestions to use reflection and attributes
// automate property change triggers.


public class PropertyChangeNotifier : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
    {
        if (!EqualityComparer<T>.Default.Equals(storage, value))
        {
            storage = value;

            this.RaisePropertyChanged(propertyName);
        }
    }


    protected void RaisePropertyChanged([CallerMemberName] string? property = null)
    {
        property ??= string.Empty;

        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        if (m_propertiesByTriggers.TryGetValue(property, out HashSet<string>? propertiesToRaise) && propertiesToRaise.Any())
        {
            foreach (string propertyToRaise in propertiesToRaise)
            {
                // REVIEW$:  We do not have circular dependency detection.

                this.RaisePropertyChanged(propertyToRaise);
            }
        }

        if (m_functionsByTriggers.TryGetValue(property, out HashSet<string>? functionsToCall) && functionsToCall.Any())
        {
            foreach (string functionToCall in functionsToCall)
            {
                this.GetType().InvokeMember(functionToCall, BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, this, null);
            }
        }

        if (m_commandsByTriggers.TryGetValue(property, out HashSet<string>? commandsToRaise) && commandsToRaise.Any())
        {
            foreach (string commandToRaise in commandsToRaise)
            {
                if (this.GetType().GetRuntimeProperty(commandToRaise)?.GetValue(this) is CommandBase commandBase)
                    commandBase.RaiseCanExecuteChanged();
            }
        }

    }


    private readonly Dictionary<string, HashSet<string>> m_propertiesByTriggers = new();
    private readonly Dictionary<string, HashSet<string>> m_functionsByTriggers = new();
    private readonly Dictionary<string, HashSet<string>> m_commandsByTriggers = new();


    private void CollectPropertyTriggers()
    {
        m_propertiesByTriggers.Clear();

        var propertiesAndTriggers =
            GetType()
            .GetRuntimeProperties()
            .SelectMany<PropertyInfo, (string Property, string Trigger)>(pi => pi.GetCustomAttributes<NotifiesWithAttribute>()
                .Select<NotifiesWithAttribute, (string Property, string Trigger)>(nwa => (pi.Name, nwa.Trigger)));

        foreach (var (property, trigger) in propertiesAndTriggers)
        {
            if (string.IsNullOrWhiteSpace(property) || string.IsNullOrWhiteSpace(trigger))
                continue;

            if (!m_propertiesByTriggers.TryGetValue(trigger, out HashSet<string>? properties))
            {
                properties = new HashSet<string>();
                m_propertiesByTriggers[trigger] = properties;
            }

            properties.Add(property);
        }
    }


    private void CollectFunctionTriggers()
    {
        m_functionsByTriggers.Clear();

        var functionsAndTriggers =
            GetType()
            .GetRuntimeMethods()
            .Where(mi => mi.GetParameters().Length == 0)
            .SelectMany<MethodInfo, (string Function, string Trigger)>(mi => mi.GetCustomAttributes<CalledWhenPropertyChangesAttribute>()
                .Select<CalledWhenPropertyChangesAttribute, (string Function, string Trigger)>(nwa => (mi.Name, nwa.Trigger)));

        foreach (var (function, trigger) in functionsAndTriggers)
        {
            if (string.IsNullOrWhiteSpace(function) || string.IsNullOrWhiteSpace(trigger))
                continue;

            if (!m_functionsByTriggers.TryGetValue(trigger, out HashSet<string>? functions))
            {
                functions = new HashSet<string>();
                m_functionsByTriggers[trigger] = functions;
            }

            functions.Add(function);
        }
    }


    private void CollectCommandTriggers()
    {
        m_commandsByTriggers.Clear();

        var commandsAndTriggers =
            GetType()
            .GetRuntimeProperties()
            .SelectMany<PropertyInfo, (string Property, string Trigger)>(pi => pi.GetCustomAttributes<CanExecuteChangesWithAttribute>()
                .Select<CanExecuteChangesWithAttribute, (string Property, string Trigger)>(cecwa => (pi.Name, cecwa.Trigger)));

        foreach (var (property, trigger) in commandsAndTriggers)
        {
            if (string.IsNullOrWhiteSpace(property) || string.IsNullOrWhiteSpace(trigger))
                continue;

            if (!m_commandsByTriggers.TryGetValue(trigger, out HashSet<string>? properties))
            {
                properties = new HashSet<string>();
                m_commandsByTriggers[trigger] = properties;
            }

            properties.Add(property);
        }
    }


    public PropertyChangeNotifier()
    {
        this.CollectPropertyTriggers();
        this.CollectFunctionTriggers();
        this.CollectCommandTriggers();
    }
}


