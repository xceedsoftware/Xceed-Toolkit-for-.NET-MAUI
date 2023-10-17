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
  public partial class CheckBox
  {
    #region Private Members

    private const string VisualState_PointerOver = "PointerOver";
    private const string VisualState_PointerOverChecked = "PointerOverChecked";
    private const string VisualState_PointerOverIndeterminate = "PointerOverIndeterminate";
    private const string VisualState_Checked = "Checked";
    private const string VisualState_Indeterminate = "Indeterminate";
    private const string VisualState_FocusedChecked = "FocusedChecked";
    private const string VisualState_FocusedIndeterminate = "FocusedIndeterminate";

    #endregion

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
    }

    #endregion

    #region Private Methods

    private void SetVisualStateAfterPointerEvent()
    {
      this.Dispatcher.Dispatch( new Action( () =>
      {
        if( this.IsPointerOver )
        {
          if( this.IsChecked.HasValue )
          {
            VisualStateManager.GoToState( this, this.IsChecked.Value ? CheckBox.VisualState_PointerOverChecked : CheckBox.VisualState_PointerOver );
          }
          else
          {
            VisualStateManager.GoToState( this, this.IsThreeState ? CheckBox.VisualState_PointerOverIndeterminate : CheckBox.VisualState_PointerOver );
          }
        }
        else if( this.IsFocused )
        {
          if( !this.IsChecked.HasValue )
          {
            VisualStateManager.GoToState( this, CheckBox.VisualState_FocusedIndeterminate );
          }
          else
          {
            VisualStateManager.GoToState( this, this.IsChecked.Value ? CheckBox.VisualState_FocusedChecked : VisualStateManager.CommonStates.Focused );
          }
        }
        else if( !this.IsChecked.HasValue )
        {
          VisualStateManager.GoToState( this, this.IsThreeState ? CheckBox.VisualState_Indeterminate : VisualStateManager.CommonStates.Normal );
        }
        else if( this.IsChecked.Value )
        {
          VisualStateManager.GoToState( this, CheckBox.VisualState_Checked );
        }
        else
        {
          VisualStateManager.GoToState( this, VisualStateManager.CommonStates.Normal );
        }
      } ) );
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
