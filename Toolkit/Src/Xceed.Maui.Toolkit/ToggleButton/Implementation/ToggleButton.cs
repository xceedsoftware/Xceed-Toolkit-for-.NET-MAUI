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
  public partial class ToggleButton : Button
  {
    #region Private Members

    private const string VisualState_Checked = "Checked";

    #endregion

    #region Partial Methods

    partial void SetVisualStateAfterUnPressed();

    #endregion

    #region Public Properties

    #region IsChecked

    public static readonly BindableProperty IsCheckedProperty = BindableProperty.Create( nameof( IsChecked ), typeof( bool ), typeof( ToggleButton ), false, propertyChanged: OnIsCheckedChanged );

    public bool IsChecked
    {
      get => ( bool )GetValue( IsCheckedProperty );
      set => SetValue( IsCheckedProperty, value );
    }

    private static void OnIsCheckedChanged( BindableObject bindable, object oldValue, object newValue )
    {
      if( bindable is ToggleButton toggle )
      {
        toggle.OnIsCheckedChanged( ( bool )oldValue, ( bool )newValue );
      }
    }

    protected virtual void OnIsCheckedChanged( bool oldValue, bool newValue )
    {
      if( newValue )
      {
        VisualStateManager.GoToState( this, ToggleButton.VisualState_Checked );
        this.RaiseCheckedEvent( this, new CheckedChangedEventArgs( newValue ) );
      }
      else
      {
        VisualStateManager.GoToState( this, VisualStateManager.CommonStates.Normal );
        this.SetVisualStateAfterUnPressed();
        this.RaiseUnCheckedEvent( this, new CheckedChangedEventArgs( newValue ) );
      }
    }

    #endregion

    #endregion

    #region Events

    public event EventHandler<CheckedChangedEventArgs> Checked;

    public void RaiseCheckedEvent( object sender, CheckedChangedEventArgs e )
    {
      this.Checked?.Invoke( sender, e );
    }

    public event EventHandler<CheckedChangedEventArgs> UnChecked;

    public void RaiseUnCheckedEvent( object sender, CheckedChangedEventArgs e )
    {
      this.UnChecked?.Invoke( sender, e );
    }

    #endregion

    #region Event Handlers

    protected override void OnIsPressedChanged( bool oldValue, bool newValue )
    {
      base.OnIsPressedChanged( oldValue, newValue );

      if( !newValue )
      {
        this.IsChecked = !this.IsChecked;
      }
    }

    #endregion
  }
}
