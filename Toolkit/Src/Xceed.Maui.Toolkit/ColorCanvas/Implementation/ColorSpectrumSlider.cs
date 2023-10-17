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
  public partial class ColorSpectrumSlider : Control
  {
    #region Private Members

    private const string PART_SpectrumDisplay = "PART_SpectrumDisplay";
    private const string PART_HiddenSlider = "PART_HiddenSlider";
    private const string PART_Thumb = "PART_Thumb";

    private Grid m_spectrumDisplay;
    private Slider m_slider;
    private Grid m_thumb;
    private LinearGradientBrush m_pickerBrush;

    #endregion //Private Members

    #region Properties

    #region SelectedColor

    public static readonly BindableProperty SelectedColorProperty = BindableProperty.Create( nameof( SelectedColor ), typeof( Color ), typeof( ColorSpectrumSlider ) );
    public Color SelectedColor
    {
      get => ( Color )GetValue( SelectedColorProperty );
      set => SetValue( SelectedColorProperty, value );
    }

    #endregion

    #region Value

    public static readonly BindableProperty ValueProperty = BindableProperty.Create( nameof( Value ), typeof( double ), typeof( ColorSpectrumSlider ), defaultValue: default( double ), propertyChanged: OnValueChanged );

    public double Value
    {
      get => ( double )GetValue( ValueProperty );
      set => SetValue( ValueProperty, value );
    }

    private static void OnValueChanged( BindableObject bindable, object oldValue, object newValue )
    {
      var spectrum = ( ColorSpectrumSlider )bindable;
      spectrum?.OnValueChanged( ( double )oldValue, ( double )newValue );
    }

    protected void OnValueChanged( double oldValue, double newValue )
    {
      if( ( newValue >= 0 ) && ( newValue <= 360 ) )
      {
        var color = ColorUtilities.ConvertHsvToRgb( 360 - newValue, 1, 1 );
        this.SelectedColor = color;
        this.RaiseValueChangedEvent( this, new ValueChangedEventArgs( oldValue, newValue ) );
      }
      else
      {
        throw new InvalidDataException( "Value must be between 0 and 360" );
      }
    }

    #endregion

    #endregion //Properties

    #region Partial Methods

    partial void ApplyTemplateForPlatform( Slider oldSlider, Slider newSlider );

    #endregion

    #region Base Class Overrides

    protected override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      var oldSlider = m_slider;

      if( m_slider != null )
      {
        m_slider.ValueChanged -= this.Slider_ValueChanged;
      }

      m_spectrumDisplay = GetTemplateChild( PART_SpectrumDisplay ) as Grid;
      m_slider = GetTemplateChild( PART_HiddenSlider ) as Slider;
      m_thumb = GetTemplateChild( PART_Thumb ) as Grid;

      if( m_slider != null )
      {
        m_slider.ValueChanged += this.Slider_ValueChanged;
      }

      this.ApplyTemplateForPlatform( oldSlider, m_slider );

      this.CreateSpectrum();
      this.OnValueChanged( Double.NaN, this.Value );
    }

    private void Slider_ValueChanged( object sender, ValueChangedEventArgs e )
    {
      if( m_thumb != null )
      {
        m_thumb.TranslationX = ( ( (Slider)sender ).Value * ( 135 - 24 ) ) / m_slider.Maximum;
      }
    }
    #endregion

    #region Private Methods
    private void CreateSpectrum()
    {
      m_pickerBrush = new LinearGradientBrush();
      m_pickerBrush.StartPoint = new Point( 1, 0 );
      m_pickerBrush.EndPoint = new Point( 0, 0 );

      var colorsList = ColorUtilities.GenerateHsvSpectrum();

      double stopIncrement = ( double )1 / ( colorsList.Count - 1 );

      int i;
      for( i = 0; i < colorsList.Count; i++ )
      {
        m_pickerBrush.GradientStops.Add( new GradientStop( colorsList[ i ], ( float )( i * stopIncrement ) ) );
      }

      m_pickerBrush.GradientStops[ i - 1 ].Offset = 1.0f;
      if( m_spectrumDisplay != null )
      {
        m_spectrumDisplay.Background = m_pickerBrush;
      }
    }

    #endregion //Methods

    #region Events

    public event EventHandler<ValueChangedEventArgs> ValueChanged;

    public void RaiseValueChangedEvent( object sender, ValueChangedEventArgs e )
    {
      this.ValueChanged?.Invoke( sender, e );
    }

    #endregion  //Events
  }
}
