using Pentomino.Helpers;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Pentomino.ViewModels;


public class GetMonthAndDayDialogViewModel : PropertyChangeNotifier
{
    public GetMonthAndDayDialogViewModel()
    {
        DateTimeOffset now = DateTimeOffset.Now;

        this.Month = now.Month - 1;
        this.Date = now.Day - 1;
    }


    private int m_month = 0;

    public int Month
    {
        get => m_month;
        set => this.SetProperty(ref m_month, value);
    }


    private int m_date = 0;

    public int Date
    {
        get => m_date;
        set => this.SetProperty(ref m_date, value);
    }


    public IEnumerable<string> AvailableMonths
    {
        get
        {
            yield return LocalizableStrings.idsMonthName_January;
            yield return LocalizableStrings.idsMonthName_February;
            yield return LocalizableStrings.idsMonthName_March;
            yield return LocalizableStrings.idsMonthName_April;
            yield return LocalizableStrings.idsMonthName_May;
            yield return LocalizableStrings.idsMonthName_June;
            yield return LocalizableStrings.idsMonthName_July;
            yield return LocalizableStrings.idsMonthName_August;
            yield return LocalizableStrings.idsMonthName_September;
            yield return LocalizableStrings.idsMonthName_October;
            yield return LocalizableStrings.idsMonthName_November;
            yield return LocalizableStrings.idsMonthName_December;
        }
    }


    public IEnumerable<string> AvailableDates => Enumerable.Range(1, 31).Select(d => d.ToString());

}

