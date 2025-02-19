﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Pentomino.Helpers;


public class CommandBase
{
    public event EventHandler? CanExecuteChanged;

    public void RaiseCanExecuteChanged()
    {
        this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}


public class Command<T> : CommandBase, ICommand
{
    private readonly Func<T, bool>? m_canExecute = null;

    private readonly Action<T> m_execute;

    public bool CanExecute(object? parameter)
    {
        if (parameter is not null && parameter is not T)
            throw new ArgumentException("parameter is not T", nameof(parameter));

        return m_canExecute?.Invoke((T) parameter!) ?? true;
    }

    public void Execute(object? parameter)
    {
        if (parameter is not null && parameter is not T)
            throw new ArgumentException("parameter is not T", nameof(parameter));

        m_execute.Invoke((T) parameter!);
    }


    public Command(Action<T> execute)
    {
        m_execute = execute;
    }


    public Command(Func<T, bool> canExecute, Action<T> execute)
        : this(execute)
    {
        m_canExecute = canExecute;
    }

    public static Command<T> NeverExecute { get; } = new((p) => false, (p) => { });

    public static Command<T> AlwaysExecuteNoOp { get; } = new((p) => { });
}


public class Command : Command<object?>
{
    public Command(Action execute)
        : base((object? o) => execute?.Invoke())
    {
    }


    public Command(Func<bool>? canExecute, Action execute)
        : base((object? o) => canExecute?.Invoke() ?? true, (object? o) => execute.Invoke())
    {
    }


    public new static Command NeverExecute { get; } = new(() => false, () => { });

    public new static Command AlwaysExecuteNoOp { get; } = new(() => { });
}

