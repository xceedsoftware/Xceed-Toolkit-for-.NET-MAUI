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


using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;

namespace Xceed.Maui.Toolkit
{
  internal class BorderShapeGridLayoutManager : ILayoutManager
  {
    #region Private Members

    private readonly BorderShapeGrid m_borderShapeGrid;

    #endregion

    #region Constructors

    internal BorderShapeGridLayoutManager( BorderShapeGrid borderShapeGrid )
    {
      m_borderShapeGrid = borderShapeGrid;
    }

    #endregion

    #region Public Methods

    public Size Measure( double widthConstraint, double heightConstraint )
    {
      var finalSize = Size.Zero;
      var borderThickness = m_borderShapeGrid.BorderThickness;
      var padding = m_borderShapeGrid.Padding;
      var margin = new Size( borderThickness.HorizontalThickness + padding.HorizontalThickness, borderThickness.VerticalThickness + padding.VerticalThickness );

      if( double.IsInfinity( widthConstraint ) && ( m_borderShapeGrid.Bounds.Width >= 0 ) )
      {
        widthConstraint = m_borderShapeGrid.Bounds.Width;
      }
      if( double.IsInfinity( heightConstraint ) && ( m_borderShapeGrid.Bounds.Height >= 0 ) )
      {
        heightConstraint = m_borderShapeGrid.Bounds.Height;
      }

      // Android/Mac/iOs needs this to respect the MaxWidth/MaxHeight.
      widthConstraint = BorderShapeGridLayoutManager.ClipMinMax( widthConstraint, m_borderShapeGrid.MinimumWidthRequest, m_borderShapeGrid.MaximumWidthRequest );
      heightConstraint = BorderShapeGridLayoutManager.ClipMinMax( heightConstraint, m_borderShapeGrid.MinimumHeightRequest, m_borderShapeGrid.MaximumHeightRequest );

      foreach( var child in m_borderShapeGrid.Children )
      {
        if( child is Shape )
        {
          child.Measure( double.PositiveInfinity, double.PositiveInfinity );
        }
        else
        {
          var size = child.Measure( widthConstraint - margin.Width, heightConstraint - margin.Height );
          size += margin;

          finalSize = new Size( Math.Max( size.Width, finalSize.Width ),
                                Math.Max( size.Height, finalSize.Height ) );
        }
      }

      return finalSize;
    }

    public Size ArrangeChildren( Rect bounds )
    {
      // Android/Mac/iOs needs this to respect the MaxWidth/MaxHeight.
      bounds.Width = BorderShapeGridLayoutManager.ClipMinMax( bounds.Width, m_borderShapeGrid.MinimumWidthRequest, m_borderShapeGrid.MaximumWidthRequest );
      bounds.Height = BorderShapeGridLayoutManager.ClipMinMax( bounds.Height, m_borderShapeGrid.MinimumHeightRequest, m_borderShapeGrid.MaximumHeightRequest );

      foreach( var child in m_borderShapeGrid.Children )
      {
        if( child is Shape )
        {
          child.Arrange( bounds );
        }
        else
        {
          var borderThickness = m_borderShapeGrid.BorderThickness;
          var padding = m_borderShapeGrid.Padding;

          child.Arrange( new Rect( borderThickness.Left + padding.Left,
                                   borderThickness.Top + padding.Top,
                                   Math.Max( 0, bounds.Width - borderThickness.HorizontalThickness - padding.HorizontalThickness ),
                                   Math.Max( 0, bounds.Height - borderThickness.VerticalThickness - padding.VerticalThickness ) ) );
        }
      }

      return bounds.Size;
    }

    #endregion

    #region Internal Methods

    internal static double ClipMinMax( double currentValue, double minValue, double maxValue )
    {
      if( !double.IsNaN( maxValue ) )
      {
        currentValue = Math.Min( currentValue, maxValue );
      }
      if( !double.IsNaN( minValue ) )
      {
        currentValue = Math.Max( currentValue, minValue );
      }

      return currentValue;
    }

    #endregion
  }
}
