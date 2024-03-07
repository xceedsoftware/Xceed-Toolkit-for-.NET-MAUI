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

    private DateTime m_end;
    private DateTime m_start;

    #endregion

    #region Constructors

    public CalendarDateRange()
      : this( DateTime.MinValue, DateTime.MaxValue )
    {
    }

    public CalendarDateRange( DateTime day )
      : this( day, day )
    {
    }

    public CalendarDateRange( DateTime start, DateTime end )
    {
      m_start = start;
      m_end = end;
    }

    #endregion

    #region Public Properties

    #region End

    public DateTime End
    {
      get
      {
        return m_end;
      }
      set
      {
        var dateTime = CalendarDateRange.GetGreater( m_start, value );
        if( dateTime != End )
        {
          this.OnChanging( new CalendarDateRangeChangingEventArgs( m_start, dateTime ) );
          m_end = value;
          this.OnPropertyChanged( new PropertyChangedEventArgs( nameof( End ) ) );
        }
      }
    }

    #endregion

    #region Start

    public DateTime Start
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
          var dateTime = CalendarDateRange.GetGreater( value, m_end );
          this.OnChanging( new CalendarDateRangeChangingEventArgs( value, dateTime ) );
          m_start = value;
          this.OnPropertyChanged( new PropertyChangedEventArgs( nameof( Start ) ) );
          if( dateTime != end )
          {
            m_end = dateTime;
            this.OnPropertyChanged( new PropertyChangedEventArgs( nameof( End ) ) );
          }
        }
      }
    }

    #endregion

    #endregion

    #region Private Methods

    private static DateTime GetGreater( DateTime start, DateTime end )
    {
      if( DateTime.Compare( start, end ) > 0 )
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
