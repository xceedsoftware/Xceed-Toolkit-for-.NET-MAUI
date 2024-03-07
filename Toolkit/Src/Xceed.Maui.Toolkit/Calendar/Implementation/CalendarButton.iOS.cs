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
        oldMainBorder.PointerDown -= this.Border_PointerDown;
        oldMainBorder.PointerUp -= this.Border_PointerUp;
      }
      if( newMainBorder != null )
      {
        newMainBorder.PointerDown += this.MainBorder_PointerDown;
        newMainBorder.PointerUp += this.MainBorder_PointerUp;
      }

      this.SetVisualStateAfterPointerEvent();
    }

    partial void UpdateVisualState()
    {
      if( !this.IsEnabled )
      {
        VisualStateManager.GoToState( this, VisualStateManager.CommonStates.Disabled );
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
    private void MainBorder_PointerUp( object sender, EventArgs e )
    {
      base.Button_PointerUp();

      if( this.IsEnabled )
      {
        this.SetVisualStateAfterPointerEvent();
      }
    }

    private void MainBorder_PointerDown( object sender, EventArgs e )
    {
      base.Button_PointerDown();

      if( this.IsEnabled )
      {
        this.SetVisualStateAfterPointerEvent();
      }
    }

    #endregion
  }
}
