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


using Microsoft.Maui.Controls;
using UIKit;
using Foundation;

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
        if( ( this.ParentCalendar != null )
          && ( this.ParentCalendar.SelectionMode == CalendarSelectionMode.MultiRange || this.ParentCalendar.SelectionMode == CalendarSelectionMode.Range )
          && ( this.ParentCalendar.DateRangeStart != null ) )
        {
          var start = this.ParentCalendar.DateRangeStart.Value;
          var end = ( (DateOnly)this.BindingContext );
          if( this.ParentCalendar.DateRangeStart.Value > ( (DateOnly)this.BindingContext ) )
          {
            start = ( (DateOnly)this.BindingContext );
            end = this.ParentCalendar.DateRangeStart.Value;
            isSelectingForward = false;
          }
          foreach( var calendarDayBtn in this.ParentCalendar.GetCalendarDayButtons() )
          {
            if( isSelectingForward )
            {
              if( ( start < ( (DateOnly)calendarDayBtn.BindingContext ) )
                && ( end >= ( (DateOnly)calendarDayBtn.BindingContext ) )
                && !calendarDayBtn.IsBlackedOut )
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
              if( ( start <= ( (DateOnly)calendarDayBtn.BindingContext ) )
                && ( end > ( (DateOnly)calendarDayBtn.BindingContext ) )
                && !calendarDayBtn.IsBlackedOut )
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
      else if( ( this.ParentCalendar != null ) && this.IsToday && this.ParentCalendar.IsTodayHighlighted )
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
      else if( ( this.ParentCalendar != null ) && this.IsToday && this.ParentCalendar.IsTodayHighlighted )
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
