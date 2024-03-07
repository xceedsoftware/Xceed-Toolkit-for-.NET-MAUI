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
  public partial class CalendarButton
  {
    #region Partial Methods
    partial void ApplyTemplateForPlatform( Border oldMainBorder, Border newMainBorder )
    {
      if( oldMainBorder != null )
      {
        oldMainBorder.PointerEnter -= this.Border_PointerEnter;
        oldMainBorder.PointerLeave -= this.Border_PointerLeave;
      }
      if( newMainBorder != null )
      {
        newMainBorder.PointerEnter += this.Border_PointerEnter;
        newMainBorder.PointerLeave += this.Border_PointerLeave;
      }

      this.SetVisualStateAfterPointerEvent();
    }

    partial void UpdateVisualState()
    {
      if( !this.IsEnabled )
      {
        VisualStateManager.GoToState( this, VisualStateManager.CommonStates.Disabled );
      }
      else if( this.IsPointerOver )
      {
        VisualStateManager.GoToState( this, VisualStateManager.CommonStates.PointerOver );
      }
      else if( this.HasSelectedDays )
      {
        VisualStateManager.GoToState( this, VisualStateManager.CommonStates.Selected );
      }
      else if( this.IsInactive )
      {
        VisualStateManager.GoToState( this, CalendarButton.VisualState_Inactive );
      }
      else
      {
        VisualStateManager.GoToState( this, VisualStateManager.CommonStates.Normal );
      }
    }

    #endregion

    #region Event Handlers
    private void Border_PointerEnter( object sender, EventArgs e )
    {
      this.IsPointerOver = true;

      if( this.IsEnabled )
      {
        this.SetVisualStateAfterPointerEvent();
      }
    }

    private void Border_PointerLeave( object sender, EventArgs e )
    {
      this.IsPointerOver = false;

      if( this.IsEnabled )
      {
        this.SetVisualStateAfterPointerEvent();
      }
    }

    #endregion
  }
}
