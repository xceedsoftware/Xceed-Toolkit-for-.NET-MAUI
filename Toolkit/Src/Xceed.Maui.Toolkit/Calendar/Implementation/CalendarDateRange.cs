/***************************************************************************************
 
  Xceed Toolkit for MAUI is a multiplatform toolkit offered by Xceed Software Inc., 
  makers of the popular WPF Toolkit.

  COPYRIGHT (C) 2023 Xceed Software Inc. ALL RIGHTS RESERVED.

  This program is provided to you under the terms of a Xceed Community License 
  For more details about the Xceed Community License please visit the products GitHub or NuGet page .

  DISCLAIMER: This code is provided as-is and without warranty of any kind, express or implied. The 
  author(s) and owner(s) of this code shall not be liable for any damages or losses resulting from 
  the use or inability to use the code. 

 
  *************************************************************************************/


using System.ComponentModel;

namespace Xceed.Maui.Toolkit
{
  public sealed class CalendarDateRange : INotifyPropertyChanged
  {
    #region Private Members

    private DateOnly m_end;
    private DateOnly m_start;

    #endregion

    #region Constructors

    public CalendarDateRange()
      : this( DateOnly.MinValue, DateOnly.MaxValue )
    {
    }

    public CalendarDateRange( DateOnly day )
      : this( day, day )
    {
    }

    public CalendarDateRange( DateOnly start, DateOnly end )
    {
      m_start = start;
      m_end = end;
    }

    #endregion

    #region Public Properties

    #region End

    public DateOnly End
    {
      get
      {
        return m_end;
      }
      set
      {
        var date = CalendarDateRange.GetGreater( m_start, value );
        if( date != this.End )
        {
          this.OnChanging( new CalendarDateRangeChangingEventArgs( m_start, date ) );
          m_end = value;
          this.OnPropertyChanged( new PropertyChangedEventArgs( nameof( End ) ) );
        }
      }
    }

    #endregion

    #region Start

    public DateOnly Start
    {
      get
      {
        return m_start;
      }
      set
      {
        if( m_start != value )
        {
          var end = this.End;
          var date = CalendarDateRange.GetGreater( value, m_end );
          this.OnChanging( new CalendarDateRangeChangingEventArgs( value, date ) );
          m_start = value;
          this.OnPropertyChanged( new PropertyChangedEventArgs( nameof( Start ) ) );
          if( date != end )
          {
            m_end = date;
            this.OnPropertyChanged( new PropertyChangedEventArgs( nameof( End ) ) );
          }
        }
      }
    }

    #endregion

    #endregion

    #region Private Methods

    private static DateOnly GetGreater( DateOnly start, DateOnly end )
    {
      if( start > end )
        return start;
      return end;
    }

    #endregion

    #region Events

    public event PropertyChangedEventHandler PropertyChanged;
    internal event EventHandler<CalendarDateRangeChangingEventArgs> Changing;

    #endregion

    #region Events Handlers

    private void OnChanging( CalendarDateRangeChangingEventArgs e )
    {
      this.Changing?.Invoke( this, e );
    }

    private void OnPropertyChanged( PropertyChangedEventArgs e )
    {
      this.PropertyChanged?.Invoke( this, e );
    }

    #endregion
  }
}
