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

namespace Xceed.Maui.Toolkit
{
  public enum CalendarSelectionMode
  {
    Single,
    Multiple,
    Range,
    MultiRange,
    None
  }

  public sealed class CalendarSelectedDatesCollection : ObservableCollection<DateTime>
  {
    #region Private Members

    private Collection<DateTime> m_newItems;
    private Collection<DateTime> m_oldItems;
    private Thread m_dispatcherThread;
    private bool m_isAddingRange;
    private Calendar m_calendar;

    #endregion    

    #region Constructors
    public CalendarSelectedDatesCollection( Calendar calendar )
    {
      m_dispatcherThread = Thread.CurrentThread;
      m_calendar = calendar;
      this.NewItems = [];
      this.OldItems = [];
    }

    #endregion 

    #region Internal Properties

    internal bool IsAddingRange { get => m_isAddingRange; set => m_isAddingRange = value; }
    internal Collection<DateTime> NewItems { get => m_newItems; set => m_newItems = value; }
    internal Collection<DateTime> OldItems { get => m_oldItems; set => m_oldItems = value; }

    #endregion

    #region Public Methods   

    public void AddRange( DateTime start, DateTime end )
    {
      this.IsAddingRange = true;
      if( m_calendar.SelectionMode == CalendarSelectionMode.Range && base.Count > 0 )
      {
        this.ClearInternal();
      }

      foreach( DateTime item in CalendarSelectedDatesCollection.GetDaysInRange( start, end ) )
      {
        this.Add( item );
      }

      this.EndAddRange();
    }

    #endregion

    #region Protected Methods

    protected override void ClearItems()
    {
      this.ValidateThread();

      this.ClearInternal();

      this.RaiseItemsCleared();

      if( this.OldItems.Count > 0 )
      {
        Collection<DateTime> addedItems = [];
        this.RaiseSelectionChanged( this.OldItems, addedItems );
        this.OldItems.Clear();
      }
    }

    protected override void InsertItem( int index, DateTime item )
    {
      this.ValidateThread();

      if( !this.Contains( item ) )
      {
        Collection<DateTime> collection = [];
        var flag = this.CheckSelectionMode();
        if( !Calendar.IsValidDateSelection( m_calendar, item ) )
          throw new ArgumentOutOfRangeException( "Invalid value for SelectedDate property" );
        else
        {
          if( flag )
          {
            index = 0;
            flag = false;
          }

          base.InsertItem( index, item );

          if( !this.IsAddingRange )
          {
            collection.Add( item );
            this.RaiseSelectionChanged( this.OldItems, collection );
            this.OldItems.Clear();
          }
          else
          {
            this.NewItems.Add( item );
          }
        }
      }
    }

    protected override void RemoveItem( int index )
    {
      this.ValidateThread();

      if( index >= base.Count )
      {
        base.RemoveItem( index );
        return;
      }

      Collection<DateTime> addedItems = [];
      Collection<DateTime> collection = [];
      var num = DateTimeHelper.CompareYearAndMonth( base[ index ], m_calendar.DisplayedDateInternal );
      collection.Add( base[ index ] );
      base.RemoveItem( index );

      this.RaiseSelectionChanged( collection, addedItems );
    }

    #endregion

    #region Internal Methods

    internal void ClearInternal()
    {
      if( base.Count <= 0 )
        return;

      using( IEnumerator<DateTime> enumerator = this.GetEnumerator() )
      {
        while( enumerator.MoveNext() )
        {
          var current = enumerator.Current;
          this.OldItems.Add( current );
        }
      }

      base.ClearItems();
    }

    internal void EndAddRange()
    {
      this.IsAddingRange = false;
      this.RaiseSelectionChanged( this.OldItems, this.NewItems );
      this.OldItems.Clear();
      this.NewItems.Clear();
    }

    internal static IEnumerable<DateTime> GetDaysInRange( DateTime start, DateTime end )
    {
      int increment = DateTime.Compare( end, start ) < 0 ? -1 : 1;
      DateTime? rangeStart = start;
      do
      {
        yield return rangeStart.Value;
        rangeStart = DateTimeHelper.AddDays( rangeStart.Value, increment );
      }
      while( rangeStart.HasValue && DateTime.Compare( end, rangeStart.Value ) != -increment );
    }


    #endregion

    #region Private Methods

    private void ValidateThread()
    {
      if( Thread.CurrentThread != m_dispatcherThread )
        throw new NotSupportedException( "This type of collection does not support changes to its source from a thread different from the Dispatcher thread." );
    }

    private bool CheckSelectionMode()
    {
      if( m_calendar.SelectionMode == CalendarSelectionMode.None )
        throw new InvalidOperationException( "The SelectedDate property cannot be set when the selection mode is None." );

      if( ( m_calendar.SelectionMode == CalendarSelectionMode.Single ) && ( base.Count > 0 ) )
        throw new InvalidOperationException( "The SelectedDates collection can be changed only in a multiple selection mode. Use the SelectedDate in a single selection mode." );

      if( ( m_calendar.SelectionMode == CalendarSelectionMode.Range ) && !this.IsAddingRange && ( base.Count > 0 ) )
      {
        this.ClearInternal();
        return true;
      }

      return false;
    }

    #endregion

    #region Events

    internal event EventHandler<ValueChangedEventArgs<Collection<DateTime>>> SelectedDatesChanged;

    private void RaiseSelectionChanged( Collection<DateTime> oldItems, Collection<DateTime> newItems )
    {
      this.SelectedDatesChanged?.Invoke( this, new ValueChangedEventArgs<Collection<DateTime>>( oldItems, newItems ) );
    }

    internal event EventHandler<EventArgs> ItemsCleared;

    private void RaiseItemsCleared()
    {
      this.ItemsCleared?.Invoke( this, EventArgs.Empty );
    }

    #endregion
  }
}
