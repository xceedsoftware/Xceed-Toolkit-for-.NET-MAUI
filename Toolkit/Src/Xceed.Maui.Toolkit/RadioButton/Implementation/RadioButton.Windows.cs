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
  public partial class RadioButton
  {
    #region Private Members

    private const string VisualState_PointerOver = "PointerOver";
    private const string VisualState_PointerOverChecked = "PointerOverChecked";
    private const string VisualState_FocusedChecked = "FocusedChecked";
    private const double AnimatedEllipse_PointerOverCheckedPercent = 0.38d;

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

    partial void SetPlatformSpecificVisualState()
    {
      this.Dispatcher.Dispatch( new Action( () =>
      {
        if( this.IsPointerOver )
        {
          if( this.IsChecked.HasValue )
          {
            if( this.IsChecked.Value )
            {
              this.AnimateEllipseStrokeThickness( "ToPointerOverChecked", RadioButton.AnimatedEllipse_PointerOverCheckedPercent );
            }

            VisualStateManager.GoToState( this, this.IsChecked.Value ? RadioButton.VisualState_PointerOverChecked : RadioButton.VisualState_PointerOver );
          }
          else
          {
            VisualStateManager.GoToState( this, RadioButton.VisualState_PointerOver );
          }
        }
        else if( this.IsFocused )
        {
          if( !this.IsChecked.HasValue )
          {
            VisualStateManager.GoToState( this, VisualStateManager.CommonStates.Normal );
          }
          else
          {
            if( this.IsChecked.Value )
            {
              this.AnimateEllipseStrokeThickness( "ToChecked", RadioButton.AnimatedEllipse_CheckedPercent );
            }
            VisualStateManager.GoToState( this, this.IsChecked.Value ? RadioButton.VisualState_FocusedChecked : VisualStateManager.CommonStates.Focused );
          }
        }
        else if( !this.IsChecked.HasValue )
        {
          VisualStateManager.GoToState( this, VisualStateManager.CommonStates.Normal );
        }
        else if( this.IsChecked.Value )
        {
          this.AnimateEllipseStrokeThickness( "ToChecked", RadioButton.AnimatedEllipse_CheckedPercent );

          VisualStateManager.GoToState( this, RadioButton.VisualState_Checked );
        }
        else
        {
          VisualStateManager.GoToState( this, VisualStateManager.CommonStates.Normal );
        }
      } ) );
    }

    protected override void OnIsPointerOverChanged( bool oldValue, bool newValue )
    {
      if( newValue )
      {
        this.RaisePointerEnterEvent( this, EventArgs.Empty );
      }
      else
      {
        this.RaisePointerLeaveEvent( this, EventArgs.Empty );
      }

      if( this.IsEnabled )
      {
        this.SetPlatformSpecificVisualState();
      }
    }

    #endregion

    #region Event Handlers

    private void Border_PointerEnter( object sender, EventArgs e )
    {
      this.IsPointerOver = true;
    }

    private void Border_PointerLeave( object sender, EventArgs e )
    {
      this.IsPointerOver = false;
    }

    #endregion
  }
}
