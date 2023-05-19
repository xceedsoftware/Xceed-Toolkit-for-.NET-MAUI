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
  public partial class ToggleButton
  {
    #region Private Members

    private const string VisualState_PointerOver = "PointerOver";

    #endregion

    #region Partial Methods

    partial void SetVisualStateAfterUnPressed()
    {
      this.Dispatcher.Dispatch( new Action( () =>
      {
        if( this.IsPointerOver )
        {
          VisualStateManager.GoToState( this, ToggleButton.VisualState_PointerOver );
        }
        else if( this.IsChecked )
        {
          VisualStateManager.GoToState( this, ToggleButton.VisualState_Checked );
        }
        else
        {
          VisualStateManager.GoToState( this, VisualStateManager.CommonStates.Normal );
        }
      } ) );
    }

    #endregion

    #region Event Handlers

    protected override void OnIsPointerOverChanged( bool oldValue, bool newValue )
    {
      base.OnIsPointerOverChanged( oldValue, newValue );
      if( this.IsEnabled )
      {
        this.SetVisualStateAfterUnPressed();
      }
    }

    #endregion
  }
}
