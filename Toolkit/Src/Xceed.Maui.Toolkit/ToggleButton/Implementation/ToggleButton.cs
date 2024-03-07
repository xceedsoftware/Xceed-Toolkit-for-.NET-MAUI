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
    private const string VisualState_Indeterminate = "Indeterminate";

    #endregion

    #region Constructors

    public ToggleButton()
    {
      this.Loaded += this.ToggleButton_Loaded;
      this.Focused += this.ToggleButton_Focused;
      this.Unfocused += this.ToggleButton_Focused;
    }

    #endregion

    #region Partial Methods

    partial void SetVisualStateAfterUnPressed();

    partial void FocusAction();

    #endregion

    #region Public Properties

    #region IsChecked

    public static readonly BindableProperty IsCheckedProperty = BindableProperty.Create( nameof( IsChecked ), typeof( bool? ), typeof( ToggleButton ), false, propertyChanged: OnIsCheckedChanged );

    public bool? IsChecked
    {
      get => ( bool? )GetValue( IsCheckedProperty );
      set => SetValue( IsCheckedProperty, value );
    }

    private static void OnIsCheckedChanged( BindableObject bindable, object oldValue, object newValue )
    {
      if( bindable is ToggleButton toggle )
      {
        toggle.OnIsCheckedChanged( ( bool? )oldValue, ( bool? )newValue );
      }
    }

    protected virtual void OnIsCheckedChanged( bool? oldValue, bool? newValue )
    {
      if( newValue.HasValue )
      {
        if( newValue.Value )
        {
          VisualStateManager.GoToState( this, ToggleButton.VisualState_Checked );
          this.RaiseCheckedEvent( this, EventArgs.Empty );
        }
        else
        {
          VisualStateManager.GoToState( this, VisualStateManager.CommonStates.Normal );
          this.SetVisualStateAfterUnPressed();
          this.RaiseUnCheckedEvent( this, EventArgs.Empty );
        }
      }
      else
      {
        if( this.IsThreeState )
        {
          VisualStateManager.GoToState( this, ToggleButton.VisualState_Indeterminate );
          this.SetVisualStateAfterUnPressed();
          this.RaiseIndeterminateEvent( this, EventArgs.Empty );
        }
        else 
        {
          VisualStateManager.GoToState( this, VisualStateManager.CommonStates.Normal );
          this.SetVisualStateAfterUnPressed();
          this.RaiseUnCheckedEvent( this, EventArgs.Empty );
        }
      }
    }

    #endregion

    #region IsThreeState

    public static readonly BindableProperty IsThreeStateProperty = BindableProperty.Create( nameof( IsThreeState ), typeof( bool ), typeof( ToggleButton ), false );

    public bool IsThreeState
    {
      get => (bool)GetValue( IsThreeStateProperty );
      set => SetValue( IsThreeStateProperty, value );
    }

    #endregion

    #endregion

    #region Internal Methods

    protected internal virtual void Initialize()
    { 
    }

    #endregion

    #region Events

    public event EventHandler<EventArgs> Checked;

    public void RaiseCheckedEvent( object sender, EventArgs e )
    {
      if( this.IsEnabled )
      {
        this.Checked?.Invoke( sender, e );
      }
    }

    public event EventHandler<EventArgs> UnChecked;

    public void RaiseUnCheckedEvent( object sender, EventArgs e )
    {
      if( this.IsEnabled )
      {
        this.UnChecked?.Invoke( sender, e );
      }
    }

    public event EventHandler<EventArgs> Indeterminate;

    public void RaiseIndeterminateEvent( object sender, EventArgs e )
    {
      if( this.IsEnabled )
      {
        this.Indeterminate?.Invoke( sender, e );
      }
    }

    #endregion

    #region Event Handlers

    private void ToggleButton_Loaded( object sender, EventArgs e )
    {
      if( this.IsEnabled )
      {
        if( this.IsChecked.HasValue )
        {
          if( this.IsChecked.Value )
          {
            VisualStateManager.GoToState( this, ToggleButton.VisualState_Checked );
          }
        }
        else
        {
          if( this.IsThreeState )
          {
            VisualStateManager.GoToState( this, ToggleButton.VisualState_Indeterminate );
          }
        }
      }

      this.Initialize();
    }

    private void ToggleButton_Focused( object sender, FocusEventArgs e )
    {
      this.FocusAction();
    }

    protected internal override void Button_PointerUp()
    {
      if( this.IsEnabled && this.IsPressed )
      {
        if( this.IsThreeState )
        {
          this.IsChecked = !this.IsChecked.HasValue
                          ? false
                          : !this.IsChecked.Value ? true : null;
        }
        else
        {
          this.IsChecked = ( this.IsChecked == null) ? true : !this.IsChecked;
        }
      }

      base.Button_PointerUp();
    }

    protected override void OnIsPressedChanged( bool oldValue, bool newValue )
    {
      if( newValue )
      {
        VisualStateManager.GoToState( this, Button.VisualState_Pressed );

        this.RaisePointerDownEvent( this, EventArgs.Empty );
      }
      else
      {
        this.RaisePointerUpEvent( this, EventArgs.Empty );
      }
    }

    #endregion
  }
}
