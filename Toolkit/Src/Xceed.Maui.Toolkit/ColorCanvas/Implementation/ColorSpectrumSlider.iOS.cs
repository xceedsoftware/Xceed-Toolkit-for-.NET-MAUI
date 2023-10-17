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


using UIKit;
using Foundation;

namespace Xceed.Maui.Toolkit
{
  public partial class ColorSpectrumSlider
  {
    #region Private Members

    private TapGestureRecognizer m_tapRecognizer;

    #endregion

    #region Partial Methods

    partial void ApplyTemplateForPlatform( Slider oldSlider, Slider newSlider )
    {
      if( oldSlider != null )
      {
        oldSlider.GestureRecognizers.Remove( m_tapRecognizer );
        m_tapRecognizer.Tapped -= this.Slider_Tapped;
      }

      if( newSlider != null )
      {
        m_tapRecognizer = new TapGestureRecognizer();
        m_tapRecognizer.Tapped += this.Slider_Tapped;
        newSlider.GestureRecognizers.Add( m_tapRecognizer );
      }
    }

    #endregion

    #region Event Handlers

    private void Slider_Tapped( object sender, TappedEventArgs e )
    {
      var slider = sender as Slider;
      if( slider == null )
        return;

      var point = e.GetPosition( this );
      if( !point.HasValue )
        return;

      var pointValue = point.Value;

      if( pointValue.Y < 0 )
      {
        pointValue.Y = 0;
      }

      if( pointValue.Y > this.Height )
      {
        pointValue.Y = this.Height;
      }

      this.Value = ( pointValue.Y / this.Height ) * slider.Maximum;
    }

    #endregion
  }
}
