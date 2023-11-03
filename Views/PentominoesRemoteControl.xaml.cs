using Pentomino.ViewModels;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Pentomino.Views;


public partial class PentominoesRemoteControl : UserControl
{
    public PentominoesRemoteControl()
    {
        InitializeComponent();

        m_slider.ValueChanged += (s, e) =>
        {
            double rounded = Math.Round(e.NewValue);

            if (rounded != e.NewValue)
                m_slider.Value = rounded; 
        };
    }


    public PentominoesRemoteControlViewModel ViewModel => (PentominoesRemoteControlViewModel) this.DataContext;


}

