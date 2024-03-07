using Pentomino.ViewModels;

using System.Windows;


namespace Pentomino.Views;


public partial class GetMonthAndDayDialog : Window
{
    public GetMonthAndDayDialog()
    {
        InitializeComponent();
        this.DataContext = this.ViewModel;
    }


    public GetMonthAndDayDialogViewModel ViewModel { get; } = new();


    public static bool GetMonthAndDate(Window parent, out int month, out int date)
    {
        month = 0;
        date = 0;

        GetMonthAndDayDialog dialog = new GetMonthAndDayDialog { Owner = parent };
        if (dialog.ShowDialog() == true)
        {
            month = dialog.ViewModel.Month;
            date = dialog.ViewModel.Date;

            return true;
        }

        return false;
    }


    private void OnOk(object sender, RoutedEventArgs e)
    {
        this.DialogResult = true;
    }

    private void OnCancel(object sender, RoutedEventArgs e)
    {
        this.DialogResult = false;
    }

}
