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


using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Xceed.Maui.Toolkit
{
  public enum ClickMode
  {
    Press,
    Release
  }

  public partial class Button : ContentControl
  {
    #region Internal Members

    internal const string VisualState_Pressed = "Pressed";

    #endregion

    #region Private Members

    private Border m_border;

    #endregion

    #region Constructors

    public Button()
    {
      this.Loaded += this.Button_Loaded;
      this.HandlerChanged += this.Button_HandlerChanged;
      this.HandlerChanging += this.Button_HandlerChanging;
    }

    #endregion

    #region Partial Methods

    partial void ApplyTemplateForPlatform( Border oldBorder, Border newBorder );

    partial void InitializeForPlatform( object sender, EventArgs e );

    partial void UninitializeForPlatform( object sender, HandlerChangingEventArgs e );

    partial void SetVisualStateAfterUnPressed();

    #endregion

    #region Public Properties

    #region Command

    public static readonly BindableProperty CommandProperty = BindableProperty.Create( nameof( Command ), typeof( ICommand ), typeof( Button ), null, propertyChanged: OnCommandChanged );
    public ICommand Command
    {
      get => ( ICommand )GetValue( CommandProperty );
      set => SetValue( CommandProperty, value );
    }

    private static void OnCommandChanged( BindableObject bindable, object oldValue, object newValue )
    {
      var button = bindable as Button;
      if( button != null )
      {
        button.OnCommandChanged( ( ICommand )oldValue, ( ICommand )newValue );
      }
    }

    protected virtual void OnCommandChanged( ICommand oldValue, ICommand newValue )
    {
      if( oldValue != null )
      {
        oldValue.CanExecuteChanged -= this.CanExecuteCommandChanged;
      }
      if( newValue != null )
      {
        newValue.CanExecuteChanged += this.CanExecuteCommandChanged;
      }

      this.IsEnabled = this.IsEnabled && ( newValue != null ) && newValue.CanExecute( this.CommandParameter );
    }

    #endregion

    #region CommandParameter

    public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create( nameof( CommandParameter ), typeof( object ), typeof( Button ), null );
    public object CommandParameter
    {
      get => ( object )GetValue( CommandParameterProperty );
      set => SetValue( CommandParameterProperty, value );
    }

    #endregion

    #region ClickMode

    public static readonly BindableProperty ClickModeProperty = BindableProperty.Create( nameof( ClickMode ), typeof( ClickMode ), typeof( Button ), ClickMode.Release );

    public ClickMode ClickMode
    {
      get => ( ClickMode )GetValue( ClickModeProperty );
      set => SetValue( ClickModeProperty, value );
    }

    #endregion

    #region IsPointerOver (Windows and Mac only)

    public static readonly BindableProperty IsPointerOverProperty = BindableProperty.Create( nameof( IsPointerOver ), typeof( bool ), typeof( Button ), false, propertyChanged: OnIsPointerOverChanged );

    public bool IsPointerOver
    {
      get => (bool)GetValue( IsPointerOverProperty );
      internal set => SetValue( IsPointerOverProperty, value );
    }

    private static void OnIsPointerOverChanged( BindableObject bindable, object oldValue, object newValue )
    {
      var button = bindable as Button;
      if( button != null )
      {
        button.OnIsPointerOverChanged( (bool)oldValue, (bool)newValue );
      }
    }

    protected virtual void OnIsPointerOverChanged( bool oldValue, bool newValue )
    {
      if( newValue )
      {
        this.RaisePointerEnterEvent( this, EventArgs.Empty );
      }
      else
      {
        this.RaisePointerLeaveEvent( this, EventArgs.Empty );
      }
    }

    #endregion

    #region IsPressed

    public static readonly BindableProperty IsPressedProperty = BindableProperty.Create( nameof(IsPressed), typeof( bool ), typeof( Button ), false, propertyChanged: OnIsPressedChanged );

    public bool IsPressed
    {
      get => ( bool )GetValue( IsPressedProperty );
      internal set => SetValue( IsPressedProperty, value );
    }

    private static void OnIsPressedChanged( BindableObject bindable, object oldValue, object newValue )
    {
      var button = bindable as Button;
      if( button != null )
      {
        button.OnIsPressedChanged( ( bool )oldValue, ( bool )newValue );
      }
    }

    protected virtual void OnIsPressedChanged( bool oldValue, bool newValue )
    {
      if( newValue )
      {
        VisualStateManager.GoToState( this, Button.VisualState_Pressed );

        this.RaisePointerDownEvent( this, EventArgs.Empty );
      }
      else
      {
        VisualStateManager.GoToState( this, VisualStateManager.CommonStates.Normal );
        // In windows, we could have another state in this situation.
        this.SetVisualStateAfterUnPressed();

        this.RaisePointerUpEvent( this, EventArgs.Empty );
      }
    }

    #endregion

    #endregion

    #region Protected Methods

    protected override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      var oldBorder = m_border;

      if( m_border != null )
      {
        m_border.PointerDown -= this.Border_PointerDown;
        m_border.PointerUp -= this.Border_PointerUp;
      }
      m_border = GetTemplateChild( "PART_Border" ) as Border;
      if( m_border != null )
      {
        m_border.PointerDown += this.Border_PointerDown;
        m_border.PointerUp += this.Border_PointerUp;
      }

      this.ApplyTemplateForPlatform( oldBorder, m_border );
    }

    protected override void OnPropertyChanging( [CallerMemberName] string propertyName = null )
    {
      if( propertyName == "BackgroundColor" )
        throw new InvalidDataException( "BackgroundColor is not available in Button. Use the Background property instead." );

      if( propertyName == "IsEnabled" )
      {
        this.IsPressed = false;
      }

      base.OnPropertyChanging( propertyName );
    }

    #endregion

    #region Internal Methods

    protected internal void RaiseClickEvent()
    {
      this.RaiseClickedEvent( this, EventArgs.Empty );

      if( ( this.Command != null ) && this.Command.CanExecute( this.CommandParameter ) )
      {
        this.Command.Execute( this.CommandParameter );
      }
    }

    protected internal virtual void Button_PointerDown()
    {
      if( this.IsEnabled )
      {
        if( ( this.ClickMode == ClickMode.Press ) && !this.IsPressed )
        {
          this.RaiseClickEvent();
        }

        this.IsPressed = true;
      }
    }

    protected internal virtual void Button_PointerUp()
    {
      if( this.IsEnabled )
      {
        if( ( this.ClickMode == ClickMode.Release ) && this.IsPressed )
        {
          this.RaiseClickEvent();
        }
      }

      this.IsPressed = false;
    }

    #endregion

    #region Events

    public event EventHandler<EventArgs> Clicked;

    internal void RaiseClickedEvent( object sender, EventArgs e )
    {
      if( this.IsEnabled )
      {
        this.Clicked?.Invoke( sender, e );
      }
    }

    public event EventHandler<EventArgs> PointerDown;

    internal void RaisePointerDownEvent( object sender, EventArgs e )
    {
      if( this.IsEnabled )
      {
        this.PointerDown?.Invoke( sender, e );
      }
    }

    public event EventHandler<EventArgs> PointerUp;

    internal void RaisePointerUpEvent( object sender, EventArgs e )
    {
      if( this.IsEnabled )
      {
        this.PointerUp?.Invoke( sender, e );
      }
    }

    public event EventHandler PointerEnter;  //Windows and Mac only

    internal void RaisePointerEnterEvent( object sender, EventArgs e )
    {
      if( this.IsEnabled )
      {
        this.PointerEnter?.Invoke( sender, e );
      }
    }

    public event EventHandler PointerLeave;   //Windows and Mac only

    internal void RaisePointerLeaveEvent( object sender, EventArgs e )
    {
      if( this.IsEnabled )
      {
        this.PointerLeave?.Invoke( sender, e );
      }
    }

    #endregion

    #region Event Handlers

    private void Button_Loaded( object sender, EventArgs e )
    {
      // Update the Visual so that the theming will be applied.
      VisualStateManager.GoToState( this, VisualStateManager.CommonStates.Disabled );
      if( this.IsEnabled )
      {
        VisualStateManager.GoToState( this, VisualStateManager.CommonStates.Normal );
      }
    }

    private void Button_HandlerChanged( object sender, EventArgs e )
    {
      this.InitializeForPlatform( sender, e );
    }

    private void Button_HandlerChanging( object sender, HandlerChangingEventArgs e )
    {
      this.UninitializeForPlatform( sender, e );
    }

    private void Border_PointerDown( object sender, EventArgs e )
    {
      this.Button_PointerDown();
    }

    private void Border_PointerUp( object sender, EventArgs e )
    {
      this.Button_PointerUp();
    }   

    private void CanExecuteCommandChanged( object sender, EventArgs e )
    {
      this.IsEnabled = ( this.Command != null ) && this.Command.CanExecute( this.CommandParameter );
    }

    #endregion
  }
}
