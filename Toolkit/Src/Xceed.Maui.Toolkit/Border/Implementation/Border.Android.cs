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
  public partial class Border
  {
    #region Private Members

    private ContentPresenter m_contentPresenter;

    #endregion

    #region Protected Methods

    protected override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      m_contentPresenter = this.GetTemplateChild( "PART_Content" ) as ContentPresenter;
    }

    protected override Size MeasureOverride( double widthConstraint, double heightConstraint )
    {
      var size = new Size( widthConstraint, heightConstraint );

      // Measure the Content so that HorizontalOptions can work when not Fill and Width is not set.
      if( (this.HorizontalOptions.Alignment != LayoutAlignment.Fill) && ( this.WidthRequest == -1 ) )
      {
        if( m_contentPresenter != null )
        {
          var contentPresenterWidth = m_contentPresenter.Measure( double.PositiveInfinity, double.PositiveInfinity ).Request.Width;

          size.Width = contentPresenterWidth;
        }
        size.Width += this.BorderThickness.HorizontalThickness + this.Padding.HorizontalThickness + this.Margin.HorizontalThickness;
      }

      // Measure the Content so that VerticalOptions can work when not Fill and Height is not set.
      if( (this.VerticalOptions.Alignment != LayoutAlignment.Fill) && (this.HeightRequest == -1) )
      {
        if( m_contentPresenter != null )
        {
          var contentPresenterHeight = m_contentPresenter.Measure( double.PositiveInfinity, double.PositiveInfinity ).Request.Height;

          size.Height = contentPresenterHeight;
        }
        size.Height += this.BorderThickness.VerticalThickness + this.Padding.VerticalThickness + this.Margin.VerticalThickness;
      }

      return base.MeasureOverride( size.Width, size.Height );
    }

    #endregion
  }
}
