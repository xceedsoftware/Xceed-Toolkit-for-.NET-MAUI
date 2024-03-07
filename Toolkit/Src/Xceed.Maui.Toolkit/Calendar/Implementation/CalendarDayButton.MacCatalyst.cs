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


namespace Xceed.Maui.Toolkit
{
  public partial class CalendarDayButton
  {
    #region Private Members

    private const string VisualState_RangePreSelected = "RangePreSelected";
    private const string VisualState_RangePreUnSelected = "RangePreUnSelected";

    #endregion

    #region Partial Methods

    partial void UpdateVisualState()
    {
      if( !this.IsEnabled )
      {
        VisualStateManager.GoToState( this, VisualStateManager.CommonStates.Disabled );
      }
      else if( this.IsPointerOver )
      {
        var isSelectingForward = true;
        if( m_calendar != null
          && ( m_calendar.SelectionMode == CalendarSelectionMode.MultiRange || m_calendar.SelectionMode == CalendarSelectionMode.Range )
          && m_calendar.DateRangeStart != null )
        {
          var start = m_calendar.DateRangeStart.Value;
          var end = ( ( MonthViewDayModel )this.BindingContext ).Date;
          if( DateTime.Compare( m_calendar.DateRangeStart.Value, ( DateTime )this.Content ) > 0 )
          {
            start = ( ( MonthViewDayModel )this.BindingContext ).Date;
            end = m_calendar.DateRangeStart.Value;
            isSelectingForward = false;
          }
          foreach( var calendarDayBtn in m_calendar.GetCalendarDayButtons() )
          {
            if( isSelectingForward )
            {
              if( DateTime.Compare( start, ( ( MonthViewDayModel )calendarDayBtn.BindingContext ).Date ) < 0
                && DateTime.Compare( end, ( ( MonthViewDayModel )calendarDayBtn.BindingContext ).Date ) >= 0
                && !( ( MonthViewDayModel )calendarDayBtn.BindingContext ).IsBlackedOut )
              {
                calendarDayBtn.SetSpecificVisualState( calendarDayBtn.IsSelected ? CalendarDayButton.VisualState_RangePreUnSelected : CalendarDayButton.VisualState_RangePreSelected );
              }
              else
              {
                calendarDayBtn.UpdateVisualStateMacInternal();
              }
            }
            else
            {
              if( DateTime.Compare( start, ( ( MonthViewDayModel )calendarDayBtn.BindingContext ).Date ) <= 0
                && DateTime.Compare( end, ( ( MonthViewDayModel )calendarDayBtn.BindingContext ).Date ) > 0
                && !( ( MonthViewDayModel )calendarDayBtn.BindingContext ).IsBlackedOut )
              {
                calendarDayBtn.SetSpecificVisualState( calendarDayBtn.IsSelected ? CalendarDayButton.VisualState_RangePreUnSelected : CalendarDayButton.VisualState_RangePreSelected );
              }
              else
              {
                calendarDayBtn.UpdateVisualStateMacInternal();
              }
            }
          }
        }
        else
        {
          if( !this.IsBlackedOut )
          {
            VisualStateManager.GoToState( this, VisualStateManager.CommonStates.PointerOver );
          }
          else
          {
            VisualStateManager.GoToState( this, CalendarDayButton.VisualState_BlackoutDay );
          }
        }
      }
      else if( this.IsBlackedOut )
      {
        VisualStateManager.GoToState( this, CalendarDayButton.VisualState_BlackoutDay );
      }
      else if( this.IsToday && m_calendar != null && m_calendar.IsTodayHighlighted )
      {
        VisualStateManager.GoToState( this, CalendarDayButton.VisualState_Today );
      }
      else if( this.IsSelected )
      {
        VisualStateManager.GoToState( this, VisualStateManager.CommonStates.Selected );
      }
      else if( this.IsInactive )
      {
        VisualStateManager.GoToState( this, CalendarDayButton.VisualState_Inactive );
      }
      else
      {
        VisualStateManager.GoToState( this, VisualStateManager.CommonStates.Normal );
      }
    }

    #endregion

    #region Internal Methods

    internal void UpdateVisualStateMacInternal()
    {
      if( !this.IsEnabled )
      {
        VisualStateManager.GoToState( this, VisualStateManager.CommonStates.Disabled );
      }
      else if( this.IsBlackedOut )
      {
        VisualStateManager.GoToState( this, CalendarDayButton.VisualState_BlackoutDay );
      }
      else if( this.IsPointerOver )
      {
        VisualStateManager.GoToState( this, VisualStateManager.CommonStates.PointerOver );
      }
      else if( this.IsToday && m_calendar != null && m_calendar.IsTodayHighlighted )
      {
        VisualStateManager.GoToState( this, CalendarDayButton.VisualState_Today );
      }
      else if( this.IsSelected )
      {
        VisualStateManager.GoToState( this, VisualStateManager.CommonStates.Selected );
      }
      else if( this.IsInactive )
      {
        VisualStateManager.GoToState( this, CalendarDayButton.VisualState_Inactive );
      }
      else
      {
        VisualStateManager.GoToState( this, VisualStateManager.CommonStates.Normal );
      }
    }

    #endregion
  }
}
