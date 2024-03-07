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
  public enum PopupLocation
  {
    BottomLeft,
    Bottom,
    BottomRight,
    TopLeft,
    Top,
    TopRight,
    Left,
    Right
  }

  public partial class DropDownButton : ContentControl
  {
    #region Private Members

    private const string VisualState_Opened = "Opened";
    private const string VisualState_Pressed = "Pressed";

    private ToggleButton m_toggleButton;

    #endregion

    #region Partial Methods

    partial void SetVisualStateAfterPointerLeave();

    #endregion

    #region Public Properties

    #region DropDownContent

    public static readonly BindableProperty DropDownContentProperty = BindableProperty.Create( nameof( DropDownContent ), typeof( View ), typeof( DropDownButton ), null );

    public View DropDownContent
    {
      get => (View)GetValue( DropDownContentProperty );
      set => SetValue( DropDownContentProperty, value );
    }

    #endregion

    #region DropDownPosition

    public static readonly BindableProperty DropDownPositionProperty = BindableProperty.Create( nameof( DropDownPosition ), typeof( PopupLocation ), typeof( DropDownButton ), PopupLocation.BottomLeft );

    public PopupLocation DropDownPosition
    {
      get => (PopupLocation)GetValue( DropDownPositionProperty );
      set => SetValue( DropDownPositionProperty, value );
    }

    #endregion

    #region IsOpen

    public static readonly BindableProperty IsOpenProperty = BindableProperty.Create( nameof( IsOpen ), typeof( bool ), typeof( DropDownButton ), defaultValue: false, propertyChanged: OnIsOpenChanged );

    public bool IsOpen
    {
      get => (bool)GetValue( IsOpenProperty );
      set => SetValue( IsOpenProperty, value );
    }

    private static void OnIsOpenChanged( BindableObject bindable, object oldValue, object newValue )
    {
      if( bindable is DropDownButton button )
      {
        button.OnIsOpenChanged( (bool)oldValue, (bool)newValue );
      }
    }

    protected virtual void OnIsOpenChanged( bool oldValue, bool newValue )
    {
      if( newValue )
      {
        this.RaiseOpenedEvent( this, EventArgs.Empty );
        VisualStateManager.GoToState( this, DropDownButton.VisualState_Opened );
      }
      else
      {
        this.RaiseClosedEvent( this, EventArgs.Empty );
        VisualStateManager.GoToState( this, VisualStateManager.CommonStates.Normal );
      }
    }

    #endregion

    #region MaxDropDownHeightRequest

    public static readonly BindableProperty MaxDropDownHeightRequestProperty = BindableProperty.Create( nameof( MaxDropDownHeightRequest ), typeof( double ), typeof( DropDownButton ), 500d );

    public double MaxDropDownHeightRequest
    {
      get => (double)GetValue( MaxDropDownHeightRequestProperty );
      set => SetValue( MaxDropDownHeightRequestProperty, value );
    }

    #endregion

    #endregion

    #region Override Methods

    protected override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      if( m_toggleButton != null )
      {
        m_toggleButton.PointerDown -= this.ToggleButton_PointerDown;
        m_toggleButton.PointerLeave -= this.ToggleButton_PointerLeave;
      }
      m_toggleButton = this.GetTemplateChild( "PART_ToggleButton" ) as ToggleButton;
      if( m_toggleButton != null )
      {
        m_toggleButton.PointerDown += this.ToggleButton_PointerDown;
        m_toggleButton.PointerLeave += this.ToggleButton_PointerLeave;
      }
    }

    #endregion

    #region Event Handlers

    // For Mac and Windows.
    private void ToggleButton_PointerLeave( object sender, EventArgs e )
    {
      this.SetVisualStateAfterPointerLeave();
    }

    private void ToggleButton_PointerDown( object sender, EventArgs e )
    {
      VisualStateManager.GoToState( this, DropDownButton.VisualState_Pressed );
    }

    #endregion

    #region Events

    public event EventHandler Opened;

    internal void RaiseOpenedEvent( object sender, EventArgs e )
    {
      if( this.IsEnabled )
      {
        this.Opened?.Invoke( sender, e );
      }
    }

    public event EventHandler Closed;

    internal void RaiseClosedEvent( object sender, EventArgs e )
    {
      if( this.IsEnabled )
      {
        this.Closed?.Invoke( sender, e );
      }
    }

    #endregion
  }
}
