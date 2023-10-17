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


using System.Globalization;

namespace Xceed.Maui.Toolkit.Converters
{
  public class FocusUnderlineCornerRadiusConverter : IMultiValueConverter
  {
    public object Convert( object[] values, Type targetType, object parameter, CultureInfo culture )
    {
      if( values.Count() != 2 || values[ 0 ] == null || values[ 1 ] == null )
        return 0d;

      var cornerRadius = (CornerRadius)values[ 0 ];
      var borderThickness = (Thickness)values[ 1 ];

      var bottomLeftDiff = cornerRadius.BottomLeft - borderThickness.Left;
      var bottomLeft = ( bottomLeftDiff > 0 )
                       ? ( bottomLeftDiff > borderThickness.Left ) ? cornerRadius.BottomLeft : bottomLeftDiff + ( bottomLeftDiff / 2 )
                       : ( cornerRadius.BottomLeft / Math.Max( 1, borderThickness.Left ) ) * ( cornerRadius.BottomLeft / 2 );

      var bottomRightDiff = cornerRadius.BottomRight - borderThickness.Right;
      var bottomRight = ( bottomRightDiff > 0 )
                       ? ( bottomRightDiff > borderThickness.Right ) ? cornerRadius.BottomRight : bottomRightDiff + ( bottomRightDiff / 2 )
                       : ( cornerRadius.BottomRight / Math.Max( 1, borderThickness.Right ) ) * ( cornerRadius.BottomRight / 2 );

      return new CornerRadius( 0d, 0d, bottomLeft, bottomRight );

    }

    public object[] ConvertBack( object value, Type[] targetTypes, object parameter, CultureInfo culture )
    {
      throw new NotImplementedException();
    }
  }
}
