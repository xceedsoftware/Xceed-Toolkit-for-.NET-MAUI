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

namespace Xceed.Maui.Toolkit
{
  public enum SpinnerLocation
  {
    Left,
    Right
  }

  public enum ValidSpinDirections
  {
    None = 0,
    Increase = 1,
    Decrease = 2
  }

  public partial class ButtonSpinner : ContentControl
  {
    #region Private Members

    private RepeatButton m_increaseButton;
    private RepeatButton m_decreaseButton;

    #endregion

    #region Constructors

    public ButtonSpinner()
    {
      this.HandlerChanged += this.Border_HandlerChanged;
      this.HandlerChanging += this.Border_HandlerChanging;
    }

    #endregion

    #region Partial Methods

    partial void InitializeForPlatform( object sender, EventArgs e );
    partial void UninitializeForPlatform( object sender, HandlerChangingEventArgs e );

    #endregion

    #region Public Properties 

    #region AllowSpin

    public static readonly BindableProperty AllowSpinProperty = BindableProperty.Create( nameof( AllowSpin ), typeof( bool ), typeof( ButtonSpinner ), true, propertyChanged: OnAllowSpinChanged );

    public bool AllowSpin
    {
      get => ( bool )GetValue( AllowSpinProperty );
      set => SetValue( AllowSpinProperty, value );
    }

    private static void OnAllowSpinChanged( BindableObject bindable, object oldValue, object newValue )
    {
      var spinner = bindable as ButtonSpinner;
      if( spinner != null )
      {
        spinner.OnAllowSpinChanged( ( bool )oldValue, ( bool )newValue );
      }
    }

    protected virtual void OnAllowSpinChanged( bool oldValue, bool newValue )
    {
      this.SetSpinnerDirections();
    }

    #endregion

    #region IsPointerOver (Windows and Mac only)

    public static readonly BindableProperty IsPointerOverProperty = BindableProperty.Create( "IsPointerOver", typeof( bool ), typeof( ButtonSpinner ), false, propertyChanged: OnIsPointerOverChanged );

    public bool IsPointerOver
    {
      get => (bool)GetValue( IsPointerOverProperty );
      private set => SetValue( IsPointerOverProperty, value );
    }

    private static void OnIsPointerOverChanged( BindableObject bindable, object oldValue, object newValue )
    {
      var button = bindable as ButtonSpinner;
      if( button != null )
      {
        button.OnIsPointerOverChanged( (bool)oldValue, (bool)newValue );
      }
    }

    protected virtual void OnIsPointerOverChanged( bool oldValue, bool newValue )
    {
    }

    #endregion

    #region SpinnerDownContentTemplate

    public static readonly BindableProperty SpinnerDownContentTemplateProperty = BindableProperty.Create( nameof( SpinnerDownContentTemplate ), typeof( DataTemplate ), typeof( ButtonSpinner ), null );

    public DataTemplate SpinnerDownContentTemplate
    {
      get => ( DataTemplate )GetValue( SpinnerDownContentTemplateProperty );
      set => SetValue( SpinnerDownContentTemplateProperty, value );
    }

    #endregion

    #region SpinnerLocation

    public static readonly BindableProperty SpinnerLocationProperty = BindableProperty.Create( nameof( SpinnerLocation ), typeof( SpinnerLocation ), typeof( ButtonSpinner ), SpinnerLocation.Right );

    public SpinnerLocation SpinnerLocation
    {
      get => ( SpinnerLocation )GetValue( SpinnerLocationProperty );
      set => SetValue( SpinnerLocationProperty, value );
    }

    #endregion

    #region SpinnerUpContentTemplate

    public static readonly BindableProperty SpinnerUpContentTemplateProperty = BindableProperty.Create( nameof( SpinnerUpContentTemplate ), typeof( DataTemplate ), typeof( ButtonSpinner ), null );

    public DataTemplate SpinnerUpContentTemplate
    {
      get => ( DataTemplate )GetValue( SpinnerUpContentTemplateProperty );
      set => SetValue( SpinnerUpContentTemplateProperty, value );
    }

    #endregion

    #region ValidSpinDirection

    public static readonly BindableProperty ValidSpinDirectionsProperty = BindableProperty.Create( nameof( ValidSpinDirections ), typeof( ValidSpinDirections ), typeof( ButtonSpinner ), ValidSpinDirections.Increase | ValidSpinDirections.Decrease, propertyChanged: OnValidSpinDirectionsChanged );

    public ValidSpinDirections ValidSpinDirections
    {
      get => ( ValidSpinDirections )GetValue( ValidSpinDirectionsProperty );
      set => SetValue( ValidSpinDirectionsProperty, value );
    }

    private static void OnValidSpinDirectionsChanged( BindableObject bindable, object oldValue, object newValue )
    {
      var spinner = bindable as ButtonSpinner;
      if( spinner != null )
      {
        spinner.OnValidSpinDirectionsChanged( ( ValidSpinDirections )oldValue, ( ValidSpinDirections )newValue );
      }
    }

    protected virtual void OnValidSpinDirectionsChanged( ValidSpinDirections oldValue, ValidSpinDirections newValue )
    {
      this.SetSpinnerDirections();
    }

    #endregion

    #endregion

    #region Protected Methods

    protected override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      if( m_increaseButton != null )
      {
        m_increaseButton.Clicked -= this.OnSpinnerIncreaseClicked;
      }
      m_increaseButton = GetTemplateChild( "PART_IncreaseButton" ) as RepeatButton;
      if( m_increaseButton != null )
      {
        m_increaseButton.Clicked += this.OnSpinnerIncreaseClicked;
      }

      if( m_decreaseButton != null )
      {
        m_decreaseButton.Clicked -= this.OnSpinnerDecreaseClicked;
      }
      m_decreaseButton = GetTemplateChild( "PART_DecreaseButton" ) as RepeatButton;
      if( m_decreaseButton != null )
      {
        m_decreaseButton.Clicked += this.OnSpinnerDecreaseClicked;
      }

      this.SetSpinnerDirections();
    }

    protected override void OnPropertyChanged( [CallerMemberName] string propertyName = null )
    {
      if( propertyName == "BackgroundColor" )
        throw new InvalidDataException( "BackgroundColor is not available in ButtonSpinner. Use the Background property instead." );

      base.OnPropertyChanged( propertyName );

      if( propertyName == "IsEnabled" )
      {
        this.SetSpinnerDirections();
      }
    }

    protected override void OnContentChanged( object oldValue, object newValue )
    {
      base.OnContentChanged( oldValue, newValue );

      if( ( ( m_increaseButton != null ) && m_increaseButton.IsPressed )
          || ( ( m_decreaseButton != null ) && m_decreaseButton.IsPressed ) )
        throw new InvalidDataException( "Modifying the ButtonSpinner.Content property may results in unexpected behaviors on some platforms." +
                                       " Try to set a Label/Entry/other control as ButtonSpinner's Content and modify that control instead." );
    }

    #endregion

    #region Internal Methods

    internal void SetSpinnerDirections()
    {
      if( m_increaseButton != null )
      {
        m_increaseButton.IsEnabled = this.IsEnabled && this.AllowSpin && ( ( this.ValidSpinDirections & ValidSpinDirections.Increase ) == ValidSpinDirections.Increase );
      }

      if( m_decreaseButton != null )
      {
        m_decreaseButton.IsEnabled = this.IsEnabled && this.AllowSpin && ( ( this.ValidSpinDirections & ValidSpinDirections.Decrease ) == ValidSpinDirections.Decrease );
      }
    }

    internal void IncreaseClicked()
    {
      if( this.IsEnabled && this.AllowSpin )
      {
        this.RaiseSpinnedEvent( this, new SpinEventArgs( SpinDirection.Increase ) );
      }
    }

    internal void DecreaseClicked()
    {
      if( this.IsEnabled && this.AllowSpin )
      {
        this.RaiseSpinnedEvent( this, new SpinEventArgs( SpinDirection.Decrease ) );
      }
    }

    #endregion

    #region Events

    public event EventHandler<SpinEventArgs> Spinned;

    public void RaiseSpinnedEvent( object sender, SpinEventArgs e )
    {
      if( this.IsEnabled )
      {
        this.Spinned?.Invoke( sender, e );
      }
    }

    #endregion

    #region Event Handlers

    private void Border_HandlerChanged( object sender, EventArgs e )
    {
      this.InitializeForPlatform( sender, e );
    }

    private void Border_HandlerChanging( object sender, HandlerChangingEventArgs e )
    {
      this.UninitializeForPlatform( sender, e );
    }

    private void OnSpinnerIncreaseClicked( object sender, EventArgs e )
    {
      this.IncreaseClicked();
    }

    private void OnSpinnerDecreaseClicked( object sender, EventArgs e )
    {
      this.DecreaseClicked();
    }

    #endregion
  }
}
