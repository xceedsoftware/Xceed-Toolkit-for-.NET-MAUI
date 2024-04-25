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


// This file is inspired from Microsoft under the MIT License.
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Xceed.Maui.Toolkit
{
  public sealed class CalendarBlackoutDatesCollection : ObservableCollection<CalendarDateRange>
  {
    #region Private Members

    private Thread m_dispatcherThread;
    private Calendar m_owner;

    #endregion

    #region Constructors
    public CalendarBlackoutDatesCollection( Calendar owner )
    {
      m_owner = owner;
      m_dispatcherThread = Thread.CurrentThread;
    }
    #endregion

    #region Public methods

    public bool Contains( DateOnly date )
    {
      return this.GetContainingDateRange( date ) != null;
    }

    #endregion

    #region Protected Methods
    protected override void ClearItems()
    {
      if( !this.IsValidThread() )
        throw new NotSupportedException( "This type of collection does not support changes to its source from a thread different from the Dispatcher thread." );

      foreach( var item in base.Items )
      {
        this.UnRegisterItem( item );
      }

      base.ClearItems();
    }

    protected override void InsertItem( int index, CalendarDateRange item )
    {
      if( !this.IsValidThread() )
        throw new NotSupportedException( "This type of collection does not support changes to its source from a thread different from the Dispatcher thread." );

      if( !this.IsValid( item ) )
        throw new ArgumentOutOfRangeException( "Value is not valid." );
      else
      {
        if( !this.Contains( item ) )
        {
          this.RegisterItem( item );
          base.InsertItem( index, item );
        }
      }
    }

    protected override void RemoveItem( int index )
    {
      if( !this.IsValidThread() )
        throw new NotSupportedException( "This type of collection does not support changes to its source from a thread different from the Dispatcher thread." );

      if( ( index >= 0 ) && ( index < base.Count ) )
      {
        this.UnRegisterItem( base.Items[ index ] );
      }

      base.RemoveItem( index );
    }

    protected override void SetItem( int index, CalendarDateRange item )
    {
      if( !this.IsValidThread() )
        throw new NotSupportedException( "This type of collection does not support changes to its source from a thread different from the Dispatcher thread." );

      if( !this.IsValid( item ) )
        throw new ArgumentOutOfRangeException( "Value is not valid." );
      else
      {
        CalendarDateRange item2 = null;
        if( ( index >= 0 ) && ( index < base.Count ) )
        {
          item2 = base.Items[ index ];
        }

        base.SetItem( index, item );

        this.UnRegisterItem( item2 );
        this.RegisterItem( base.Items[ index ] );
      }
    }

    #endregion

    #region Private Methods

    private void RegisterItem( CalendarDateRange item )
    {
      if( item != null )
      {
        item.Changing += this.Item_Changing;
        item.PropertyChanged += this.Item_PropertyChanged;
      }
    }

    private void UnRegisterItem( CalendarDateRange item )
    {
      if( item != null )
      {
        item.Changing -= this.Item_Changing;
        item.PropertyChanged -= this.Item_PropertyChanged;
      }
    }

    private bool IsValid( CalendarDateRange item )
    {
      return this.IsValid( item.Start, item.End );
    }

    private bool IsValid( DateOnly start, DateOnly end )
    {
      foreach( DateOnly selectedDate in m_owner.SelectedDates )
      {
        if( ( selectedDate >= start)
          && ( selectedDate <= end ) )
          return false;
      }

      return true;
    }

    private bool IsValidThread()
    {
      return Thread.CurrentThread == m_dispatcherThread;
    }

    private CalendarDateRange GetContainingDateRange( DateOnly date )
    {
      for( int i = 0; i < base.Count; i++ )
      {
        if( ( date >= base[ i ].Start ) 
          && ( date <= base[ i ].End ) )
          return base[ i ];
      }

      return null;
    }

    #endregion


    #region Events

    internal event EventHandler<EventArgs> CalendarDateRangeChanged;

    private void RaiseCalendarDateRangeChanged()
    {
      this.CalendarDateRangeChanged?.Invoke( this, EventArgs.Empty );
    }

    #endregion

    #region Events Handlers

    private void Item_Changing( object sender, CalendarDateRangeChangingEventArgs e )
    {
      var calendarDateRange = sender as CalendarDateRange;
      if( calendarDateRange != null && !this.IsValid( e.Start, e.End ) )
        throw new ArgumentOutOfRangeException( "Value is not valid." );
    }

    private void Item_PropertyChanged( object sender, PropertyChangedEventArgs e )
    {
      if( sender is CalendarDateRange )
      {
        this.RaiseCalendarDateRangeChanged();
      }
    }

    #endregion
  }
}
