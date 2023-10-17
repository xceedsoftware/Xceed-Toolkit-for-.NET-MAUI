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

namespace Xceed.Maui.Toolkit
{
  public class ExpanderCornerRadiusConverter : IMultiValueConverter
  {
    public object Convert( object[] values, Type targetType, object parameter, CultureInfo culture )
    {
      if( values.Any( val => val is null ) )
        return 0d;

      var cornerRadius = ( CornerRadius )values[ 0 ];
      var isExpanded = ( bool )values[ 1 ];
      var direction = ( ExpandDirectionEnum )values[ 2 ];

      if( direction == ExpandDirectionEnum.Down )
        return isExpanded ? new CornerRadius( cornerRadius.TopLeft, cornerRadius.TopRight, 0, 0 ) : cornerRadius;

      return isExpanded ? new CornerRadius( 0, 0, cornerRadius.BottomLeft, cornerRadius.BottomRight ) : cornerRadius;
    }
    public object[] ConvertBack( object value, Type[] targetTypes, object parameter, CultureInfo culture )
    {
      throw new NotImplementedException();
    }
  }
}
