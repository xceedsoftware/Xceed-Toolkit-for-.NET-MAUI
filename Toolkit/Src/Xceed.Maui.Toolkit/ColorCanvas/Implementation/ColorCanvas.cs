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


using Color = Microsoft.Maui.Graphics.Color;
using Point = Microsoft.Maui.Graphics.Point;

namespace Xceed.Maui.Toolkit
{
  public partial class ColorCanvas : Control
  {
    #region Private Members

    private const string PART_ColorShadingCanvas = "PART_ColorShadingCanvas";
    private const string PART_ColorShadeSelector = "PART_ColorShadeSelector";
    private const string PART_SpectrumSlider = "PART_SpectrumSlider";
    private const string PART_HexadecimalTextBox = "PART_HexadecimalTextBox";

    private GraphicsView m_colorShadingCanvas;
    private Grid m_colorShadeSelector;
    private ColorSpectrumSlider m_spectrumSlider;
    private TextBox m_hexadecimalTextBox;
    private Point? m_currentColorPosition;
    private bool m_avoidSelectedColorChanged;
    private bool m_changingRGBA;
    private bool m_updateSpectrumSliderValue = true;

    #endregion //Private Members

    #region Partial Methods

    partial void ApplyTemplateForPlatform( GraphicsView oldColorShadingCanvas, GraphicsView newColorShadingCanvas );

    #endregion

    #region Properties

    #region ARGB

    #region A

    public static readonly BindableProperty AProperty = BindableProperty.Create( nameof( A ), typeof( byte ), typeof( ColorCanvas ), defaultValue: (byte)255, propertyChanged: OnAChanged );

    public byte A
    {
      get
      {
        return (byte)GetValue( AProperty );
      }
      set
      {
        SetValue( AProperty, value );
      }
    }

    private static void OnAChanged( BindableObject bindable, object oldValue, object newValue )
    {
      if( bindable is ColorCanvas colorCanvas )
      {
        colorCanvas.OnAChanged( (byte)oldValue, (byte)newValue );
      }
    }

    protected virtual void OnAChanged( byte oldValue, byte newValue )
    {
      m_changingRGBA = true;
      if( !m_avoidSelectedColorChanged )
      {
        this.UpdateSelectedColor();
      }
      m_changingRGBA = false;
    }

    #endregion //A

    #region R

    public static readonly BindableProperty RProperty = BindableProperty.Create( nameof( R ), typeof( byte ), typeof( ColorCanvas ), defaultValue: (byte)0, propertyChanged: OnRChanged );

    public byte R
    {
      get
      {
        return (byte)GetValue( RProperty );
      }
      set
      {
        SetValue( RProperty, value );
      }
    }

    private static void OnRChanged( BindableObject bindable, object oldValue, object newValue )
    {
      if( bindable is ColorCanvas colorCanvas )
      {
        colorCanvas.OnRChanged( (byte)oldValue, (byte)newValue );
      }
    }

    protected virtual void OnRChanged( byte oldValue, byte newValue )
    {
      m_changingRGBA = true;
      if( !m_avoidSelectedColorChanged )
      {
        this.UpdateSelectedColor();
      }
      m_changingRGBA = false;
    }

    #endregion //R

    #region G

    public static readonly BindableProperty GProperty = BindableProperty.Create( nameof( G ), typeof( byte ), typeof( ColorCanvas ), defaultValue: (byte)0, propertyChanged: OnGChanged );

    public byte G
    {
      get
      {
        return (byte)GetValue( GProperty );
      }
      set
      {
        SetValue( GProperty, value );
      }
    }

    private static void OnGChanged( BindableObject bindable, object oldValue, object newValue )
    {
      if( bindable is ColorCanvas colorCanvas )
      {
        colorCanvas.OnGChanged( (byte)oldValue, (byte)newValue );
      }
    }

    protected virtual void OnGChanged( byte oldValue, byte newValue )
    {
      m_changingRGBA = true;
      if( !m_avoidSelectedColorChanged )
      {
        this.UpdateSelectedColor();
      }
      m_changingRGBA = false;
    }

    #endregion //G

    #region B

    public static readonly BindableProperty BProperty = BindableProperty.Create( nameof( B ), typeof( byte ), typeof( ColorCanvas ), defaultValue: (byte)0, propertyChanged: OnBChanged );

    public byte B
    {
      get
      {
        return (byte)GetValue( BProperty );
      }
      set
      {
        SetValue( BProperty, value );
      }
    }

    private static void OnBChanged( BindableObject bindable, object oldValue, object newValue )
    {
      if( bindable is ColorCanvas colorCanvas )
      {
        colorCanvas.OnBChanged( (byte)oldValue, (byte)newValue );
      }
    }

    protected virtual void OnBChanged( byte oldValue, byte newValue )
    {
      m_changingRGBA = true;
      if( !m_avoidSelectedColorChanged )
      {
        this.UpdateSelectedColor();
      }
      m_changingRGBA = false;
    }

    #endregion //B

    #endregion //RGB

    #region HexadecimalString

    public static readonly BindableProperty HexadecimalStringProperty = BindableProperty.Create( nameof( HexadecimalString ), typeof( string ), typeof( ColorCanvas ), defaultValue: "", propertyChanged: OnHexadecimalStringChanged, coerceValue: OnCoerceHexadecimalString );

    public string HexadecimalString
    {
      get
      {
        return (string)GetValue( HexadecimalStringProperty );
      }
      set
      {
        SetValue( HexadecimalStringProperty, value );
      }
    }

    private static void OnHexadecimalStringChanged( BindableObject bindable, object oldValue, object newValue )
    {
      if( bindable is ColorCanvas colorCanvas )
      {
        colorCanvas.OnHexadecimalStringChanged( (string)oldValue, (string)newValue );
      }
    }

    protected virtual void OnHexadecimalStringChanged( string oldValue, string newValue )
    {
      var newColorString = this.GetFormatedColorString( newValue );
      var currentColorString = this.GetFormatedColorString( this.SelectedColor );
      if( !currentColorString.Equals( newColorString ) )
      {
        this.UpdateSelectedColor( !string.IsNullOrEmpty( newColorString ) ? Color.FromArgb( newColorString ) : null );
      }
      this.SetHexadecimalTextBoxTextProperty( newValue );
    }

    private static object OnCoerceHexadecimalString( BindableObject bindable, object value )
    {
      var colorCanvas = (ColorCanvas)bindable;
      if( colorCanvas == null )
        return value;

      return colorCanvas.OnCoerceHexadecimalString( value );
    }

    private object OnCoerceHexadecimalString( object newValue )
    {
      var value = newValue as string;
      string retValue = value;

      try
      {
        if( !string.IsNullOrEmpty( retValue ) )
        {
          // User has entered an hexadecimal value (without the "#" character)... add it.
          if( Int32.TryParse( retValue, System.Globalization.NumberStyles.HexNumber, null, out int outValue ) )
          {
            retValue = "#" + retValue;
          }
        }
      }
      catch
      {
        //When HexadecimalString is changed via Code-Behind and hexadecimal format is bad, throw.
        throw new InvalidDataException( "Color provided is not in the correct format." );
      }

      return retValue;
    }

    #endregion //HexadecimalString

    #region SelectedColor

    public static readonly BindableProperty SelectedColorProperty = BindableProperty.Create( nameof( SelectedColor ), typeof( Color ), typeof( ColorCanvas ), propertyChanged: OnSelectedColorChanged, defaultValue: null );
#nullable enable
    public Color? SelectedColor
    {
      get
      {
        return ( Color? )GetValue( SelectedColorProperty );
#nullable restore
      }
      set
      {
        SetValue( SelectedColorProperty, value );
      }
    }

    private static void OnSelectedColorChanged( BindableObject bindable, object oldValue, object newValue )
    {
      if( bindable is ColorCanvas colorCanvas )
      {
#nullable enable
        colorCanvas.OnSelectedColorChanged( ( Color? )oldValue, ( Color? )newValue );
#nullable restore
      }
    }
#nullable enable
    protected virtual void OnSelectedColorChanged( Color? oldValue, Color? newValue )
    {
#nullable restore
      if( !m_avoidSelectedColorChanged )
      {
        this.SetHexadecimalStringProperty( this.GetFormatedColorString( newValue ), false );
      }
      if( !m_changingRGBA )
      {
        this.UpdateRGBValues( newValue );
      }
      if( m_updateSpectrumSliderValue )
      {
        this.UpdateColorShadeSelectorPosition( newValue );
      }

#nullable enable
      this.RaiseSelectedColorChangedEvent( this, new ValueChangedEventArgs<Color?>( oldValue, newValue ) );
#nullable restore
    }

    #endregion //SelectedColor

    #region UsingAlphaChannel

    public static readonly BindableProperty UsingAlphaChannelProperty = BindableProperty.Create( nameof( UsingAlphaChannel ), typeof( bool ), typeof( ColorCanvas ), defaultValue: true, defaultBindingMode: BindingMode.TwoWay, propertyChanged: OnUsingAlphaChannelPropertyChanged );

    public bool UsingAlphaChannel
    {
      get
      {
        return ( bool )GetValue( UsingAlphaChannelProperty );
      }
      set
      {
        SetValue( UsingAlphaChannelProperty, value );
      }
    }

    private static void OnUsingAlphaChannelPropertyChanged( BindableObject bindable, object oldValue, object newValue )
    {
      if( bindable is ColorCanvas colorCanvas )
      {
        colorCanvas.OnUsingAlphaChannelChanged();
      }
    }

    protected virtual void OnUsingAlphaChannelChanged()
    {
      this.SetHexadecimalStringProperty( this.GetFormatedColorString( this.SelectedColor ), false );
      this.UpdateSelectedColor();
    }

    #endregion //UsingAlphaChannel

    #endregion //Properties

    #region Base Class Overrides

    protected override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      var oldColorShadingCanvas = m_colorShadingCanvas;

      if( m_colorShadingCanvas != null )
      {
        m_colorShadingCanvas.EndInteraction -= this.ColorShadingCanvas_EndInteraction;
        m_colorShadingCanvas.DragInteraction -= this.ColorShadingCanvas_DragInteraction;
        m_colorShadingCanvas.SizeChanged -= this.ColorShadingCanvas_SizeChanged;
      }

      if( m_spectrumSlider != null )
      {
        m_spectrumSlider.ValueChanged -= this.SpectrumSlider_ValueChanged;
      }

      if( m_hexadecimalTextBox != null )
      {
        m_hexadecimalTextBox.Unfocused -= this.HexadecimalTextBox_Unfocused;
        m_hexadecimalTextBox.Completed -= this.HexadecimalTextBox_Completed;
      }

      m_colorShadingCanvas = this.GetTemplateChild( PART_ColorShadingCanvas ) as GraphicsView;
      m_colorShadeSelector = this.GetTemplateChild( PART_ColorShadeSelector ) as Grid;
      m_spectrumSlider = this.GetTemplateChild( PART_SpectrumSlider ) as ColorSpectrumSlider;
      m_hexadecimalTextBox = this.GetTemplateChild( PART_HexadecimalTextBox ) as TextBox;

      if( m_colorShadingCanvas != null )
      {
        m_colorShadingCanvas.EndInteraction += this.ColorShadingCanvas_EndInteraction;
        m_colorShadingCanvas.DragInteraction += this.ColorShadingCanvas_DragInteraction;
        m_colorShadingCanvas.SizeChanged += this.ColorShadingCanvas_SizeChanged;
      }

      if( m_spectrumSlider != null )
      {
        m_spectrumSlider.ValueChanged += this.SpectrumSlider_ValueChanged;
      }

      if( m_hexadecimalTextBox != null )
      {
        m_hexadecimalTextBox.Unfocused += this.HexadecimalTextBox_Unfocused;
        m_hexadecimalTextBox.Completed += this.HexadecimalTextBox_Completed;
      }

      this.ApplyTemplateForPlatform( oldColorShadingCanvas, m_colorShadingCanvas );

      this.UpdateRGBValues( this.SelectedColor );

      this.UpdateColorShadeSelectorPosition( this.SelectedColor );

      // When changing theme, HexadecimalString needs to be set since it is not binded.
      this.SetHexadecimalTextBoxTextProperty( this.GetFormatedColorString( this.SelectedColor ) );
    }

    #endregion //Base Class Overrides

    #region Event Handlers    

    private void HexadecimalTextBox_Completed( object sender, EventArgs e )
    {
      this.SetHexadecimalStringProperty( ( ( TextBox )sender ).Text, true );
    }

    private void HexadecimalTextBox_Unfocused( object sender, EventArgs e )
    {
      this.SetHexadecimalStringProperty( ( ( TextBox )sender ).Text, true );
    }

    private void ColorShadingCanvas_EndInteraction( object sender, TouchEventArgs e )
    {
      this.ColorShadingCanvasTouching( e.Touches.FirstOrDefault() );
    }

    private void ColorShadingCanvas_DragInteraction( object sender, TouchEventArgs e )
    {
      this.ColorShadingCanvasTouching( e.Touches.FirstOrDefault() );
    }

    private void ColorShadingCanvas_SizeChanged( object sender, EventArgs e )
    {
      if( m_currentColorPosition != null )
      {
        var _newPoint = new Point
        {
          X = ( ( Point )m_currentColorPosition ).X * ( ( GraphicsView )sender ).Width,
          Y = ( ( Point )m_currentColorPosition ).Y * ( ( GraphicsView )sender ).Height
        };
        this.UpdateColorShadeSelectorPositionAndCalculateColor( _newPoint, false );
      }
    }

    private void SpectrumSlider_ValueChanged( object sender, ValueChangedEventArgs e )
    {
      if( ( m_currentColorPosition != null ) && ( this.SelectedColor != null ) )
      {
        this.CalculateColor( ( Point )m_currentColorPosition );
      }
    }

    #endregion //Event Handlers

    #region Events

#nullable enable
    public event EventHandler<ValueChangedEventArgs<Color?>>? SelectedColorChanged;

    public void RaiseSelectedColorChangedEvent( object sender, ValueChangedEventArgs<Color?> e )
#nullable restore
    {
      if( this.IsEnabled )
      {
        this.SelectedColorChanged?.Invoke( sender, e );
      }
    }

    #endregion //Events

    #region Internal Methods

    internal void SetHexadecimalStringProperty( string newValue, bool modifyFromUI )
    {
      bool bad_format = false; ;
      if( modifyFromUI )
      {
        try
        {
          if( !string.IsNullOrEmpty( newValue ) )
          {
            int outValue;
            // User has entered an hexadecimal value (without the "#" character)... add it.
            if( Int32.TryParse( newValue, System.Globalization.NumberStyles.HexNumber, null, out outValue ) )
            {
              newValue = "#" + newValue;
            }
            if( this.UsingAlphaChannel )
            {
              if( newValue.Length < 9 )
              {
                bad_format = true;
              }
            }
            else
            {
              if( newValue.Length < 7 )
              {
                bad_format = true;
              }
            }
            Color.Parse( newValue );
          }
          if( !bad_format )
          {
            this.HexadecimalString = newValue;
          }
          else
          {
            this.SetHexadecimalTextBoxTextProperty( this.HexadecimalString );
          }
        }
        catch
        {
          //When HexadecimalString is changed via UI and hexadecimal format is bad, keep the previous HexadecimalString.
          this.SetHexadecimalTextBoxTextProperty( this.HexadecimalString );
        }
      }
      else
      {
        //When HexadecimalString is changed via Code-Behind, hexadecimal format will be evaluated in OnCoerceHexadecimalString()
        this.HexadecimalString = newValue;
      }
    }

    #endregion

    #region Private Methods

    private void ColorShadingCanvasTouching( Point p )
    {
      if( m_colorShadingCanvas != null )
      {
        this.UpdateColorShadeSelectorPositionAndCalculateColor( p, true );
      }
    }

    private void UpdateSelectedColor()
    {
      var currentR = this.R;
      var currentG = this.G;
      var currentB = this.B;
      if( this.UsingAlphaChannel )
      {
        if( this.SelectedColor != Color.FromRgba( currentR, currentG, currentB, this.A ) )
        {
          var currentA = this.A;
          m_avoidSelectedColorChanged = true;
          this.SelectedColor = null;
          m_avoidSelectedColorChanged = false;
          this.SelectedColor = Color.FromRgba( currentR, currentG, currentB, currentA );
        }
      }
      else
      {
        if( this.SelectedColor != Color.FromRgb( currentR, currentG, currentB ) )
        {
          m_avoidSelectedColorChanged = true;
          this.SelectedColor = null;
          m_avoidSelectedColorChanged = false;
          this.SelectedColor = Color.FromRgb( currentR, currentG, currentB );
        }
      }
    }

#nullable enable
    private void UpdateSelectedColor( Color? color )
    {
#nullable restore
      if( color != null )
      {
        if( this.SelectedColor != color )
        {
          m_avoidSelectedColorChanged = true;
          this.SelectedColor = null;
          m_avoidSelectedColorChanged = false;
          this.SelectedColor = color;
        }
      }
      else
      {
        this.SelectedColor = null;
      }
    }

#nullable enable
    private void UpdateRGBValues( Color? color )
    {
#nullable restore

      m_avoidSelectedColorChanged = true;

      this.A = ( color != null ) ? (byte)( color.Alpha * 255f ) : (byte)255;
      this.R = ( color != null ) ? (byte)( color.Red * 255f ) : (byte)0;
      this.G = ( color != null ) ? (byte)( color.Green * 255f ) : (byte)0;
      this.B = ( color != null ) ? (byte)( color.Blue * 255f ) : (byte)0;

      m_avoidSelectedColorChanged = false;
    }


    private void UpdateColorShadeSelectorPositionAndCalculateColor( Point p, bool calculateColor )
    {
      if( ( m_colorShadingCanvas == null ) || ( m_colorShadeSelector == null ) )
        return;

      if( p.Y < 0 )
      {
        p.Y = 0;
      }

      if( p.X < 0 )
      {
        p.X = 0;
      }

      if( p.X > m_colorShadingCanvas.Width )
      {
        p.X = m_colorShadingCanvas.Width;
      }

      if( p.Y > m_colorShadingCanvas.Height )
      {
        p.Y = m_colorShadingCanvas.Height;
      }

      m_colorShadeSelector.TranslationX = p.X - ( m_colorShadeSelector.Width / 2 );
      m_colorShadeSelector.TranslationY = p.Y - ( m_colorShadeSelector.Height / 2 );

      p.X /= m_colorShadingCanvas.Width;
      p.Y /= m_colorShadingCanvas.Height;

      m_currentColorPosition = p;

      if( calculateColor )
      {
        this.CalculateColor( p );
      }
    }

    private void CalculateColor( Point p )
    {
      if( m_spectrumSlider == null )
        return;

      HsvColor hsv = new( 360 - m_spectrumSlider.Value, 1, 1 )
      {
        S = p.X,
        V = 1 - p.Y
      };
      var currentColor = ColorUtilities.ConvertHsvToRgb( hsv.H, hsv.S, hsv.V );
      currentColor = this.UsingAlphaChannel
                    ? Color.FromRgba( currentColor.Red, currentColor.Green, currentColor.Blue, (float)( this.A / 255f ) )
                    : Color.FromRgb( currentColor.Red, currentColor.Green, currentColor.Blue );

      m_updateSpectrumSliderValue = false;
      this.SelectedColor = currentColor;
      m_updateSpectrumSliderValue = true;

      this.SetHexadecimalStringProperty( this.GetFormatedColorString( this.SelectedColor ), false );
    }

#nullable enable
    private void UpdateColorShadeSelectorPosition( Color? color )
    {
#nullable restore
      if( ( m_spectrumSlider == null ) || ( m_colorShadingCanvas == null ) || ( color == null ) )
        return;

      m_currentColorPosition = null;

      var hsv = ColorUtilities.ConvertRgbToHsv( ( int )( color.Red * 255 ), ( int )( color.Green * 255 ), ( int )( color.Blue * 255 ) );

      if( m_updateSpectrumSliderValue )
      {
        m_spectrumSlider.Value = 360 - hsv.H;
      }

      var p = new Point( hsv.S, 1 - hsv.V );

      m_currentColorPosition = p;

      m_colorShadeSelector.TranslationX = ( p.X * m_colorShadingCanvas.Width ) - ( m_colorShadeSelector.Width / 2 );
      m_colorShadeSelector.TranslationY = ( p.Y * m_colorShadingCanvas.Height ) - ( m_colorShadeSelector.Height / 2 );
    }

#nullable enable
    private string GetFormatedColorString( Color? colorToFormat )
    {
#nullable restore
      if( colorToFormat == null )
        return string.Empty;

      return colorToFormat.ToArgbHex( this.UsingAlphaChannel );
    }

    private string GetFormatedColorString( string stringToFormat )
    {
      return ColorUtilities.FormatColorString( stringToFormat, this.UsingAlphaChannel );
    }

    private void SetHexadecimalTextBoxTextProperty( string newValue )
    {
      if( m_hexadecimalTextBox != null )
      {
        m_hexadecimalTextBox.Text = newValue;
      }
    }

    #endregion //Methods
  }
}
