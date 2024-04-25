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
  public partial class CalendarLabel : Label
  {
    #region Constructors

    public CalendarLabel()
    {
      this.HandlerChanged += this.CalendarDayButton_HandlerChanged;
      this.HandlerChanging += this.CalendarDayButton_HandlerChanging;
    }

    #endregion

    #region Properties

    #region IsPointerOver

    public static readonly BindableProperty IsPointerOverProperty = BindableProperty.Create( nameof( IsPointerOver ), typeof( bool ), typeof( CalendarLabel ), defaultValue: false, propertyChanged: OnIsPointerOverChanged );

    public bool IsPointerOver
    {
      get => (bool)GetValue( IsPointerOverProperty );
      internal set => SetValue( IsPointerOverProperty, value );
    }

    private static void OnIsPointerOverChanged( BindableObject bindable, object oldValue, object newValue )
    {
      if( bindable is CalendarLabel calendarLabel )
      {
        calendarLabel.OnIsPointerOverChanged( (bool)oldValue, (bool)newValue );
      }
    }

    protected virtual void OnIsPointerOverChanged( bool oldValue, bool newValue )
    {
    }

    #endregion

    #region IsPressed

    public static readonly BindableProperty IsPressedProperty = BindableProperty.Create( nameof( IsPressed ), typeof( bool ), typeof( CalendarLabel ), defaultValue: false, propertyChanged: OnIsPressedChanged );

    public bool IsPressed
    {
      get => (bool)GetValue( IsPressedProperty );
      internal set => SetValue( IsPressedProperty, value );
    }

    private static void OnIsPressedChanged( BindableObject bindable, object oldValue, object newValue )
    {
      if( bindable is CalendarLabel calendarLabel )
      {
        calendarLabel.OnIsPressedChanged( (bool)oldValue, (bool)newValue );
      }
    }

    protected virtual void OnIsPressedChanged( bool oldValue, bool newValue )
    {
    }

    #endregion

    #endregion

    #region Partial Methods

    partial void InitializeForPlatform( object sender, EventArgs e );

    partial void UninitializeForPlatform( object sender, HandlerChangingEventArgs e );

    #endregion

    #region Internal Methods

    internal void PointerDown()
    {
      if( this.IsEnabled )
      {
        this.IsPressed = true;
      }
    }

    internal void PointerUp()
    {
      if( this.IsEnabled )
      {
        if( this.IsPressed )
        {
          this.RaiseClickedEvent( this, EventArgs.Empty );
        }
      }

      this.IsPressed = false;
    }

    #endregion

    #region Event Handlers

    private void CalendarDayButton_HandlerChanged( object sender, EventArgs e )
    {
      this.InitializeForPlatform( sender, e );
    }

    private void CalendarDayButton_HandlerChanging( object sender, HandlerChangingEventArgs e )
    {
      this.UninitializeForPlatform( sender, e );
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

    #endregion
  }
}
