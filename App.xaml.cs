using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Pentomino;

public partial class PentominoApp : Application
{
    public static new PentominoApp Current => (PentominoApp) Application.Current;


    private readonly DispatcherTimer m_oneSecondDispatcherTimer = new() { Interval = TimeSpan.FromSeconds(1.0) };

    private EventHandler? m_oneSecondTimerEvent = null;

    public event EventHandler OneSecondTimer
    {
        add
        {
            if (value is null)
                return;

            if (m_oneSecondTimerEvent is null)
                m_oneSecondDispatcherTimer.Start();  // start the timer, now that somebody is listening.

            m_oneSecondTimerEvent += value;
        }

        remove
        {
            if (value is null)
                return;

            m_oneSecondTimerEvent -= value;
            if (m_oneSecondTimerEvent is null)
                m_oneSecondDispatcherTimer.Stop();  // stop the timer, now that nobody is listening.
        }
    }


    private void OnSecondDispatcherTimerTick(object? sender, EventArgs e)
    {
        m_oneSecondTimerEvent?.Invoke(this, e);
    }


    private void OnStartup(object sender, StartupEventArgs args)
    {
        m_oneSecondDispatcherTimer.Tick += this.OnSecondDispatcherTimerTick;
    }

}
