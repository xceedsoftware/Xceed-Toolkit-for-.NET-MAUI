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
  public partial class ColorCanvas
  {
    #region Private Members

    private const string PART_rSlider = "PART_rSlider";
    private const string PART_gSlider = "PART_gSlider";
    private const string PART_bSlider = "PART_bSlider";
    private const string PART_aSlider = "PART_aSlider";

    private TapGestureRecognizer m_colorShadingCanvasRecognizer;
    private TapGestureRecognizer m_rgbaSliderRecognizer;
    private Slider m_redSlider;
    private Slider m_greenSlider;
    private Slider m_blueSlider;
    private Slider m_alphaSlider;

    #endregion

    #region Partial Methods

    partial void ApplyTemplateForPlatform( GraphicsView oldColorShadingCanvas, GraphicsView newColorShadingCanvas )
    {
      if( oldColorShadingCanvas != null )
      {
        oldColorShadingCanvas.GestureRecognizers.Remove( m_colorShadingCanvasRecognizer );
      }

      if( m_colorShadingCanvasRecognizer != null )
      {
        m_colorShadingCanvasRecognizer.Tapped -= this.ColorShadingCanvas_Tapped;
      }

      if( newColorShadingCanvas != null )
      {
        m_colorShadingCanvasRecognizer = new TapGestureRecognizer();
        m_colorShadingCanvasRecognizer.Tapped += this.ColorShadingCanvas_Tapped;
        newColorShadingCanvas.GestureRecognizers.Add( m_colorShadingCanvasRecognizer );
      }

      if( m_redSlider != null )
      {
        m_redSlider.GestureRecognizers.Remove( m_rgbaSliderRecognizer );
      }
      if( m_greenSlider != null )
      {
        m_greenSlider.GestureRecognizers.Remove( m_rgbaSliderRecognizer );
      }
      if( m_blueSlider != null )
      {
        m_blueSlider.GestureRecognizers.Remove( m_rgbaSliderRecognizer );
      }
      if( m_alphaSlider != null )
      {
        m_alphaSlider.GestureRecognizers.Remove( m_rgbaSliderRecognizer );
      }

      if( m_rgbaSliderRecognizer != null )
      {
        m_rgbaSliderRecognizer.Tapped -= this.RgbaSlider_Tapped;
      }

      m_rgbaSliderRecognizer = new TapGestureRecognizer();
      m_rgbaSliderRecognizer.Tapped += this.RgbaSlider_Tapped;

      m_redSlider = this.GetTemplateChild( PART_rSlider ) as Slider;
      if( m_redSlider != null )
      {
        m_redSlider.GestureRecognizers.Add( m_rgbaSliderRecognizer );
      }
      m_greenSlider = this.GetTemplateChild( PART_gSlider ) as Slider;
      if( m_greenSlider != null )
      {
        m_greenSlider.GestureRecognizers.Add( m_rgbaSliderRecognizer );
      }
      m_blueSlider = this.GetTemplateChild( PART_bSlider ) as Slider;
      if( m_blueSlider != null )
      {
        m_blueSlider.GestureRecognizers.Add( m_rgbaSliderRecognizer );
      }
      m_alphaSlider = this.GetTemplateChild( PART_aSlider ) as Slider;
      if( m_alphaSlider != null )
      {
        m_alphaSlider.GestureRecognizers.Add( m_rgbaSliderRecognizer );
      }
    }

    #endregion

    #region Event Handlers

    private void ColorShadingCanvas_Tapped( object sender, TappedEventArgs e )
    {
      var position = e.GetPosition( (GraphicsView)sender );
      if( position.HasValue )
      {
        this.ColorShadingCanvasTouching( position.Value );
      }
    }

    private void RgbaSlider_Tapped( object sender, TappedEventArgs e )
    {
      var slider = sender as Slider;
      if( slider == null )
        return;

      var point = e.GetPosition( slider );
      if( !point.HasValue )
        return;

      var pointValue = point.Value;

      if( pointValue.X < 0 )
      {
        pointValue.X = 0;
      }

      if( pointValue.X > slider.Width )
      {
        pointValue.X = slider.Width;
      }

      slider.Value = ( pointValue.X / slider.Width ) * slider.Maximum;
    }

    #endregion
  }
}
