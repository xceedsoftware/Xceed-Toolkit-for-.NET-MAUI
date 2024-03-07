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
  public partial class CalendarDayButton : Button
  {
    #region Private Members

    private const string VisualState_Inactive = "Inactive";
    private const string VisualState_Today = "Today";
    private const string VisualState_BlackoutDay = "BlackoutDay";

    private Calendar m_calendar;

    #endregion

    #region Constructors

    public CalendarDayButton()
    {
      this.Loaded += this.CalendarDayButton_Loaded;
      this.PointerEnter += this.CalendarDayButton_PointerEnter;
      this.PointerLeave += this.CalendarDayButton_PointerLeave;
      this.PointerDown += this.CalendarDayButton_PointerDown;
      this.PointerUp += this.CalendarDayButton_PointerUp;
    }

    #endregion

    #region Public Properties

    #region IsBlackedOut

    public static readonly BindableProperty IsBlackedOutProperty = BindableProperty.Create( nameof( IsBlackedOut ), typeof( bool ), typeof( CalendarDayButton ), defaultValue: false );
    public bool IsBlackedOut
    {
      get => ( bool )GetValue( IsBlackedOutProperty );
      internal set => SetValue( IsBlackedOutProperty, value );
    }

    #endregion

    #region IsInactive

    public static readonly BindableProperty IsInactiveProperty = BindableProperty.Create( nameof( IsInactive ), typeof( bool ), typeof( CalendarDayButton ), defaultValue: false );
    public bool IsInactive
    {
      get => ( bool )GetValue( IsInactiveProperty );
      internal set => SetValue( IsInactiveProperty, value );
    }

    #endregion

    #region IsSelected

    public static readonly BindableProperty IsSelectedProperty = BindableProperty.Create( nameof( IsSelected ), typeof( bool ), typeof( CalendarDayButton ), defaultValue: false, defaultBindingMode: BindingMode.TwoWay );
    public bool IsSelected
    {
      get => ( bool )GetValue( IsSelectedProperty );
      internal set => SetValue( IsSelectedProperty, value );
    }

    #endregion

    #region IsToday

    public static readonly BindableProperty IsTodayProperty = BindableProperty.Create( nameof( IsToday ), typeof( bool ), typeof( CalendarDayButton ), defaultValue: false );

    public bool IsToday
    {
      get => ( bool )GetValue( IsTodayProperty );
      internal set => SetValue( IsTodayProperty, value );
    }

    #endregion

    #endregion

    #region Partial Methods

    partial void UpdateVisualState();

    #endregion

    #region Protected Methods

    protected override void OnIsPressedChanged( bool oldValue, bool newValue )
    {
      if( !this.IsBlackedOut )
      {
        base.OnIsPressedChanged( oldValue, newValue );
      }
    }

    protected override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      this.SetVisualStateAfterPointerEvent();
    }

    #endregion

    #region Internal Methods

    internal void NotifyNeedsVisualStateUpdate()
    {
      this.UpdateVisualState();
    }

    internal void SetContentInternal( string value )
    {
      this.SetValue( ContentControl.ContentProperty, value );
    }

    #endregion

    #region Private Methods

    private static Calendar GetParentCalendar( CalendarDayButton view )
    {
      if( view == null )
        return null;

      var parent = view.Parent;
      while( parent != null )
      {
        if( parent is Calendar )
          return parent as Calendar;

        parent = parent.Parent;
      }

      return null;
    }

    private void SetSpecificVisualState( string state )
    {
      VisualStateManager.GoToState( this, state );
    }

    private void SetVisualStateAfterPointerEvent()
    {
      this.Dispatcher.Dispatch( () => this.UpdateVisualState() );
    }

    #endregion

    #region Event Handlers

    private void CalendarDayButton_Loaded( object sender, EventArgs e )
    {
      m_calendar = CalendarDayButton.GetParentCalendar( this );
      this.UpdateVisualState();
    }

    private void CalendarDayButton_PointerEnter( object sender, EventArgs e )
    {
      if( this.IsEnabled )
      {
        this.SetVisualStateAfterPointerEvent();
      }
    }

    private void CalendarDayButton_PointerLeave( object sender, EventArgs e )
    {
      if( this.IsEnabled )
      {
        this.SetVisualStateAfterPointerEvent();
      }
    }

    private void CalendarDayButton_PointerUp( object sender, EventArgs e )
    {
      if( this.IsEnabled )
      {
        this.SetVisualStateAfterPointerEvent();
      }
    }

    private void CalendarDayButton_PointerDown( object sender, EventArgs e )
    {
      if( this.IsEnabled )
      {
        this.SetVisualStateAfterPointerEvent();
      }
    }

    #endregion    
  }
}
