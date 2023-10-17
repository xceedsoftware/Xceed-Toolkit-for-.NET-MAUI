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
  public partial class ToggleSwitch : Control
  {
    #region Constants

    private const string PART_InnerContent = "PART_InnerContent";
    private const string PART_Thumb = "PART_Thumb";
    private const string PART_MainBorder = "PART_MainBorder";
    private const string PART_Slider = "PART_Slider";

    private const uint Animation_time = 150;
    private const int Animation_scaling_time = 50;
    private const double ThumbScale = 1.1;
    private const uint Short_animation_time = 1;
    private const double ThumbHeightFactor = 0.64;

    #endregion //Constants

    #region Members

    private Slider m_slider;
    private Grid m_innerContent;
    private Border m_thumbControl;
    private Border m_mainBorder;
    private double m_startDraggingValue = -1;
    private double m_startDraggingForClicked = -1;
    private bool m_isDragging = false;
    private double m_translateInXAxis;
    private bool m_IsAnimating;

    #endregion //Members

    #region Constructors

    public ToggleSwitch()
    {
      this.SizeChanged += this.ToggleSwitch_SizeChanged;
      this.Loaded += this.ToggleSwitch_Loaded;
      this.HandlerChanged += this.ToggleSwitch_HandlerChanged;
      this.HandlerChanging += this.ToggleSwitch_HandlerChanging;
    }

    #endregion //Constructors

    #region Partial Methods

    partial void InitializeForPlatform( object sender, EventArgs e );

    partial void UninitializeForPlatform( object sender, HandlerChangingEventArgs e );

    #endregion

    #region Public Properties

    #region CheckedBackground

    public static readonly BindableProperty CheckedBackgroundProperty = BindableProperty.Create( nameof( CheckedBackground ), typeof( Brush ), typeof( ToggleSwitch ) );
    public Brush CheckedBackground
    {
      get
      {
        return ( Brush )GetValue( CheckedBackgroundProperty );
      }
      set
      {
        SetValue( CheckedBackgroundProperty, value );
      }
    }

    #endregion //CheckedBackground

    #region CheckedContent

    public static readonly BindableProperty CheckedContentProperty = BindableProperty.Create( nameof( CheckedContent ), typeof( object ), typeof( ToggleSwitch ) );
    public object CheckedContent
    {
      get
      {
        return ( object )GetValue( CheckedContentProperty );
      }
      set
      {
        SetValue( CheckedContentProperty, value );
      }
    }

    #endregion //CheckedContent

    #region IsChecked

    public static readonly BindableProperty IsCheckedProperty = BindableProperty.Create( nameof( IsChecked ), typeof( bool ), typeof( ToggleSwitch ), defaultValue: false, propertyChanged: OnIsCheckedValueChanged, defaultBindingMode: BindingMode.TwoWay );
    public bool IsChecked
    {
      get
      {
        return ( bool )GetValue( IsCheckedProperty );
      }
      set
      {
        SetValue( IsCheckedProperty, value );
      }
    }

    private static void OnIsCheckedValueChanged( BindableObject bindable, object oldValue, object newValue )
    {
      var toggleSwitch = bindable as ToggleSwitch;
      if( toggleSwitch != null )
      {
        toggleSwitch.OnIsCheckedValueChanged( ( bool )oldValue, ( bool )newValue );
      }
    }

    protected virtual void OnIsCheckedValueChanged( bool oldValue, bool newValue )
    {
      this.SetSliderValues( newValue, this.IsCheckedLeft );
      this.UpdateCanvasPosition( true );
      this.RaiseIsCheckedValueChangedEvent( this, new CheckedChangedEventArgs( newValue ) );
    }

    #endregion //IsChecked

    #region IsCheckedLeft

    public static readonly BindableProperty IsCheckedLeftProperty = BindableProperty.Create( nameof( IsCheckedLeft ), typeof( bool ), typeof( ToggleSwitch ), defaultValue: false, propertyChanged: OnIsCheckedLeftChanged, coerceValue: OnCoerceIsCheckedLeft );
    public bool IsCheckedLeft
    {
      get
      {
        return ( bool )GetValue( IsCheckedLeftProperty );
      }
      set
      {
        SetValue( IsCheckedLeftProperty, value );
      }
    }

    private static void OnIsCheckedLeftChanged( BindableObject bindable, object oldValue, object newValue )
    {
      var toggleSwitch = bindable as ToggleSwitch;
      if( toggleSwitch != null )
      {
        toggleSwitch.OnIsCheckedLeftValueChanged( ( bool )oldValue, ( bool )newValue );
      }
    }

    protected virtual void OnIsCheckedLeftValueChanged( bool oldValue, bool newValue )
    {
      this.SetSliderValues( this.IsChecked, newValue );
      this.UpdateCanvasPosition( false );
      this.RaiseIsCheckedLeftValueChangedEvent( this, new CheckedChangedEventArgs( newValue ) );
    }

    private static object OnCoerceIsCheckedLeft( BindableObject bindable, object value )
    {
      if( bindable is ToggleSwitch toggleSwitch )
        return toggleSwitch.OnCoerceCheckedLeft( ( bool )value );

      return value;
    }

    private object OnCoerceCheckedLeft( bool value )
    {
      this.SetSliderValues( this.IsChecked, value );

      return value;
    }

    #endregion //IsCheckedLeft    

    #region ThumbBackground

    public static readonly BindableProperty ThumbBackgroundProperty = BindableProperty.Create( nameof( ThumbBackground ), typeof( Brush ), typeof( ToggleSwitch ) );
    public Brush ThumbBackground
    {
      get
      {
        return ( Brush )GetValue( ThumbBackgroundProperty );
      }
      set
      {
        SetValue( ThumbBackgroundProperty, value );
      }
    }

    #endregion //ThumbBackground

    #region ThumbBorderBrush

    public static readonly BindableProperty ThumbBorderBrushProperty = BindableProperty.Create( nameof( ThumbBorderBrush ), typeof( Brush ), typeof( ToggleSwitch ) );
    public Brush ThumbBorderBrush
    {
      get
      {
        return ( Brush )GetValue( ThumbBorderBrushProperty );
      }
      set
      {
        SetValue( ThumbBorderBrushProperty, value );
      }
    }

    #endregion //ThumbBorderBrush

    #region ThumbBorderThickness

    public static readonly BindableProperty ThumbBorderThicknessProperty = BindableProperty.Create( nameof( ThumbBorderThickness ), typeof( Thickness ), typeof( ToggleSwitch ) );
    public Thickness ThumbBorderThickness
    {
      get
      {
        return ( Thickness )GetValue( ThumbBorderThicknessProperty );
      }
      set
      {
        SetValue( ThumbBorderThicknessProperty, value );
      }
    }

    #endregion //ThumbBorderThickness

    #region ThumbCheckedBackground

    public static readonly BindableProperty ThumbCheckedBackgroundProperty = BindableProperty.Create( nameof( ThumbCheckedBackground ), typeof( Brush ), typeof( ToggleSwitch ) );
    public Brush ThumbCheckedBackground
    {
      get
      {
        return ( Brush )GetValue( ThumbCheckedBackgroundProperty );
      }
      set
      {
        SetValue( ThumbCheckedBackgroundProperty, value );
      }
    }

    #endregion //ThumbBackground

    #region ThumbCheckedContent

    public static readonly BindableProperty ThumbCheckedContentProperty = BindableProperty.Create( nameof( ThumbCheckedContent ), typeof( object ), typeof( ToggleSwitch ) );
    public object ThumbCheckedContent
    {
      get
      {
        return ( object )GetValue( ThumbCheckedContentProperty );
      }
      set
      {
        SetValue( ThumbCheckedContentProperty, value );
      }
    }

    #endregion //ThumbCheckedContent

    #region ThumbUncheckedContent

    public static readonly BindableProperty ThumbUncheckedContentProperty = BindableProperty.Create( nameof( ThumbUncheckedContent ), typeof( object ), typeof( ToggleSwitch ) );
    public object ThumbUncheckedContent
    {
      get
      {
        return ( object )GetValue( ThumbUncheckedContentProperty );
      }
      set
      {
        SetValue( ThumbUncheckedContentProperty, value );
      }
    }

    #endregion //ThumbUncheckedContent

    #region UncheckedContent

    public static readonly BindableProperty UncheckedContentProperty = BindableProperty.Create( nameof( UncheckedContent ), typeof( object ), typeof( ToggleSwitch ) );
    public object UncheckedContent
    {
      get
      {
        return ( object )GetValue( UncheckedContentProperty );
      }
      set
      {
        SetValue( UncheckedContentProperty, value );
      }
    }

    #endregion //UncheckedContent

    #endregion

    #region Base Class Overrides

    protected override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      if( m_slider != null )
      {
        m_slider.ValueChanged -= this.Slider_ValueChanged;
        m_slider.DragStarted -= this.Slider_DragStarted;
        m_slider.DragCompleted -= this.Slider_DragCompleted;
      }
      m_slider = this.GetTemplateChild( PART_Slider ) as Slider;
      if( m_slider != null )
      {
        m_slider.ValueChanged += this.Slider_ValueChanged;
        m_slider.DragStarted += this.Slider_DragStarted;
        m_slider.DragCompleted += this.Slider_DragCompleted;
      }

      m_thumbControl = this.GetTemplateChild( PART_Thumb ) as Border;

      if( m_mainBorder != null )
      {
        m_mainBorder.PointerUp -= this.MainBorder_PointerUp;
        m_mainBorder.PointerEnter -= this.MainBorder_PointerEnter;
        m_mainBorder.PointerLeave -= this.MainBorder_PointerLeave;
      }
      m_mainBorder = this.GetTemplateChild( PART_MainBorder ) as Border;
      if( m_mainBorder != null )
      {
        m_mainBorder.PointerUp += this.MainBorder_PointerUp;
        m_mainBorder.PointerEnter += this.MainBorder_PointerEnter;
        m_mainBorder.PointerLeave += this.MainBorder_PointerLeave;
      }

      m_innerContent = this.GetTemplateChild( PART_InnerContent ) as Grid;

      this.UpdateCanvasPosition( false );
    }

    #endregion //Base Class Overrides

    #region Event Handlers

    private void ToggleSwitch_Loaded( object sender, EventArgs e )
    {
      this.SetSliderValues( this.IsChecked, this.IsCheckedLeft );
      this.UpdateCanvasPosition( false );
    }

    private void ToggleSwitch_HandlerChanged( object sender, EventArgs e )
    {
      this.InitializeForPlatform( sender, e );
    }

    private void ToggleSwitch_HandlerChanging( object sender, HandlerChangingEventArgs e )
    {
      this.UninitializeForPlatform( sender, e );
    }

    private void MainBorder_PointerUp( object sender, EventArgs e )
    {
      this.ToggleSwitch_ChangeIsCheckedProperty();
    }

    private void ToggleSwitch_SizeChanged( object sender, EventArgs e )
    {
      this.ResetThumbSize();
      this.UpdateCanvasPosition( false );
    }

    private void Slider_DragStarted( object sender, EventArgs e )
    {
      m_isDragging = true;
      if( m_thumbControl != null )
      {
        m_thumbControl.WidthRequest = ( this.IsChecked != this.IsCheckedLeft )
                                      ? this.Height * ThumbHeightFactor + m_thumbControl.Margin.Left
                                      : this.Height * ThumbHeightFactor + m_thumbControl.Margin.Right;
      }
    }

    private void Slider_DragCompleted( object sender, EventArgs e )
    {
      if( m_isDragging )
      {
        double change = this.CalculateHorizontalChange();
        double cap = this.Width - m_thumbControl.Height - this.BorderThickness.HorizontalThickness;

        if( change < 0 )
        {
          this.SetSliderValues( this.IsChecked, this.IsCheckedLeft );
          this.UpdateCanvasToOriginalPosition( true );
          m_isDragging = false;
          this.SetDefaultValuesForDragging();
          this.ResetThumbSize();
          return;
        }

        if( change == 0
          || m_startDraggingForClicked == ( ( Slider )sender ).Value
          || m_startDraggingForClicked == 0 )  
        {
          this.ToggleSwitch_ChangeIsCheckedProperty();
        }
        else
        {
          if( change > cap )
          {
            change = cap;
          }
          double changePercent = change / cap * 100;
          if( changePercent >= 50 )
          {
            this.ToggleSwitch_ChangeIsCheckedProperty();
          }
          else
          {
            this.SetSliderValues( this.IsChecked, this.IsCheckedLeft );
            this.UpdateCanvasToOriginalPosition( true );
          }
        }
      }
      m_isDragging = false;
      this.SetDefaultValuesForDragging();
      this.ResetThumbSize();
    }   

    private void Slider_ValueChanged( object sender, ValueChangedEventArgs e )
    {
      if( !m_isDragging )
      {
        if( m_startDraggingForClicked == -1 )
        {
          m_startDraggingForClicked = e.NewValue;
        }
      }

      if( m_isDragging )
      {
        if( this.IsCheckedLeft != this.IsChecked )
        {
          this.MoveContent( e.NewValue, e.OldValue, ( ( Slider )sender ).Maximum, true );
        }
        else
        {
          this.MoveContent( e.NewValue, e.OldValue, ( ( Slider )sender ).Minimum, false );
        }
      }
    }

    private void MainBorder_PointerLeave( object sender, EventArgs e )
    {

      if( this.IsEnabled )
      {
        new Animation( v => m_thumbControl.Scale = v, start: ThumbScale, end: 1, Easing.Linear )           
                .Commit( this, "Leaving", 1, Animation_scaling_time, Easing.Linear );
      }
    }

    private void MainBorder_PointerEnter( object sender, EventArgs e )
    {

      if( this.IsEnabled )
      {
        new Animation( v => m_thumbControl.Scale = v, start: 1, end: ThumbScale, Easing.Linear )
            .Commit( this, "Entering", 1, Animation_scaling_time, Easing.Linear );
      }
    }

    #endregion //Event Handlers

    #region Internal Methods

    internal void ToggleSwitch_ChangeIsCheckedProperty()
    {
      this.Dispatcher.Dispatch( () =>
      {
        if( !m_IsAnimating )
        {
          this.IsChecked = !this.IsChecked;
          this.SetDefaultValuesForDragging();
        }
      } );
    }

    internal void ToggleSwitch_ChangeIsCheckedLeftProperty()
    {
      this.IsCheckedLeft = !this.IsCheckedLeft;
    }

    #endregion

    #region Private Methods

    private void ResetThumbSize()
    {
      if( m_thumbControl != null )
      {
        m_thumbControl.WidthRequest = this.Height * ThumbHeightFactor;
      }
    }

    private void SetDefaultValuesForDragging()
    {
      m_startDraggingForClicked = m_startDraggingValue = -1;
    }

    private void MoveContent( double actualValue, double startDraggingValue, double min_maxValue, bool moveToLeft )
    {
      if( m_innerContent != null && m_thumbControl != null )
      {
        if( m_startDraggingForClicked == -1 )
        {
          m_startDraggingForClicked = actualValue;
        }
        if( startDraggingValue != min_maxValue )
        {
          if( m_startDraggingValue == -1 )
          {
            m_startDraggingValue = startDraggingValue;
          }
          if( moveToLeft )
          {
            if( min_maxValue - ( m_startDraggingValue - actualValue ) < this.Width - this.BorderThickness.HorizontalThickness - m_thumbControl.Margin.Left && min_maxValue - ( m_startDraggingValue - actualValue ) > m_thumbControl.Width )
            {
              m_innerContent.TranslationX = min_maxValue - ( m_startDraggingValue - actualValue ) - ( m_innerContent.Width / 2 ) - ( m_thumbControl.Height / 2 );
              m_thumbControl.TranslationX = -( m_startDraggingValue - actualValue ) + m_thumbControl.Margin.Left;
            }
          }
          else
          {
            if( min_maxValue - ( m_startDraggingValue - actualValue ) < this.Width - m_thumbControl.Width - this.BorderThickness.HorizontalThickness && min_maxValue - ( m_startDraggingValue - actualValue ) > m_thumbControl.Margin.Right )
            {
              m_innerContent.TranslationX = min_maxValue - ( m_startDraggingValue - actualValue ) - ( m_innerContent.Width / 2 ) + ( m_thumbControl.Height / 2 );
              m_thumbControl.TranslationX = min_maxValue - ( m_startDraggingValue - actualValue ) - m_thumbControl.Margin.Right;
            }
          }
        }
      }
    }

    private double CalculateHorizontalChange()
    {
      if( this.IsCheckedLeft != this.IsChecked )
      {
        return ( m_startDraggingValue != -1 )
              ? ( m_startDraggingValue - m_slider.Value )
              : m_slider.Maximum - m_slider.Value;
      }
      else
      {
        return ( m_startDraggingValue != -1 )
              ? m_slider.Minimum - ( m_startDraggingValue - m_slider.Value )
              : m_slider.Value;
      }
    }

    private void SetSliderValues( bool isChecked, bool isCheckedLeft )
    {
      if( m_slider != null )
      {
        m_slider.Value = ( isChecked != isCheckedLeft )
                        ? m_slider.Maximum
                        : m_slider.Minimum;
      }
    }

    private void UpdateCanvasPosition( bool pAnimate )
    {
      if( m_innerContent != null )
      {
        this.GetBackgroundAnimation( pAnimate );
        this.GetThumbAnimation( pAnimate );
      }
    }

    private async void UpdateCanvasToOriginalPosition( bool pAnimate )
    {
      this.GetBackgroundAnimation( pAnimate );
      await m_thumbControl.TranslateTo( 0, 0, pAnimate ? Animation_time : Short_animation_time );
    }

    private async void GetBackgroundAnimation( bool animate )
    {
      if( m_innerContent == null )
        return;

      if( this.IsCheckedLeft != this.IsChecked )
      {
        await m_innerContent.TranslateTo( 0, 0, animate ? Animation_time : Short_animation_time );
      }
      else
      {
        m_translateInXAxis = this.Width - ( this.Height * ThumbHeightFactor ) - this.BorderThickness.HorizontalThickness - m_thumbControl.Margin.HorizontalThickness;
        await m_innerContent.TranslateTo( -m_translateInXAxis, 0, animate ? Animation_time : Short_animation_time );
      }
    }

    private async void GetThumbAnimation( bool animate )
    {
      if( m_thumbControl == null )
        return;

      if( this.IsCheckedLeft != this.IsChecked )
      {
        m_IsAnimating = true;
        await m_thumbControl.TranslateTo( m_translateInXAxis, 0, animate ? Animation_time : Short_animation_time );
        m_IsAnimating = false;
        m_thumbControl.HorizontalOptions = LayoutOptions.End;
        m_thumbControl.TranslationX = 0;
      }
      else
      {
        m_IsAnimating = true;
        await m_thumbControl.TranslateTo( -( m_translateInXAxis ), 0, animate ? Animation_time : Short_animation_time );
        m_IsAnimating = false;
        m_thumbControl.HorizontalOptions = LayoutOptions.Start;
        m_thumbControl.TranslationX = 0;
      }
    }

    #endregion //Private Methods

    #region Events

    public event EventHandler<CheckedChangedEventArgs> IsCheckedValueChanged;

    public void RaiseIsCheckedValueChangedEvent( object sender, CheckedChangedEventArgs e )
    {
      if( this.IsEnabled )
      {
        this.IsCheckedValueChanged?.Invoke( sender, e );
      }
    }

    public event EventHandler<CheckedChangedEventArgs> IsCheckedLeftValueChanged;

    public void RaiseIsCheckedLeftValueChangedEvent( object sender, CheckedChangedEventArgs e )
    {
      if( this.IsEnabled )
      {
        this.IsCheckedLeftValueChanged?.Invoke( sender, e );
      }
    }

    #endregion //Events
  }
}
