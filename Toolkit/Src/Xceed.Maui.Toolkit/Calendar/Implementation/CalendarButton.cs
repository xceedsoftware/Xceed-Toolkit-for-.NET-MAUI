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
  public partial class CalendarButton : CalendarLabel
  {
    #region Private Members

    private const string VisualState_Inactive = "Inactive";

    #endregion

    #region Constructors
    public CalendarButton()
    {
      this.Loaded += this.CalendarButton_Loaded;
    }

    #endregion

    #region Public Properties

    #region HasSelectedDays

    public static readonly BindableProperty HasSelectedDaysProperty = BindableProperty.Create( nameof( HasSelectedDays ), typeof( bool ), typeof( CalendarButton ), defaultValue: false, propertyChanged: OnHasSelectedDaysChanged );

    public bool HasSelectedDays
    {
      get
      {
        return ( bool )GetValue( HasSelectedDaysProperty );
      }
      internal set
      {
        SetValue( HasSelectedDaysProperty, value );
      }
    }

    private static void OnHasSelectedDaysChanged( BindableObject bindable, object oldValue, object newValue )
    {
      if( bindable is CalendarButton btn )
      {
        btn.OnHasSelectedDaysChanged( ( bool )oldValue, ( bool )newValue );
      }
    }

    protected virtual void OnHasSelectedDaysChanged( bool oldValue, bool newValue )
    {
      VisualStateManager.GoToState( this, newValue ? VisualStateManager.CommonStates.Selected : VisualStateManager.CommonStates.Normal );
    }

    #endregion

    #region IsInactive

    public static readonly BindableProperty IsInactiveProperty = BindableProperty.Create( nameof( IsInactive ), typeof( bool ), typeof( CalendarButton ), defaultValue: false );

    public bool IsInactive
    {
      get
      {
        return ( bool )GetValue( IsInactiveProperty );
      }
      internal set
      {
        SetValue( IsInactiveProperty, value );
      }
    }

    #endregion

    #endregion

    #region Partial Methods

    partial void UpdateVisualState();

    #endregion

    #region Protected Methods

    protected override void OnIsPointerOverChanged( bool oldValue, bool newValue )
    {
      base.OnIsPointerOverChanged( oldValue, newValue );

      if( this.IsEnabled )
      {
        this.SetVisualStateAfterPointerEvent();
      }
    }

    protected override void OnIsPressedChanged( bool oldValue, bool newValue )
    {
      base.OnIsPressedChanged( oldValue, newValue );

      if( this.IsEnabled )
      {
        this.SetVisualStateAfterPointerEvent();
      }
    }

    #endregion

    #region Internal Methods

    internal void RequestUpdateVisualState()
    {
      this.UpdateVisualState();
    }

    #endregion

    #region Private Methods

    private void SetVisualStateAfterPointerEvent()
    {
      this.Dispatcher.Dispatch( () => this.UpdateVisualState() );
    }

    #endregion

    #region Event Handlers

    private void CalendarButton_Loaded( object sender, EventArgs e )
    {
      this.UpdateVisualState();
    }

    #endregion
  }
}
