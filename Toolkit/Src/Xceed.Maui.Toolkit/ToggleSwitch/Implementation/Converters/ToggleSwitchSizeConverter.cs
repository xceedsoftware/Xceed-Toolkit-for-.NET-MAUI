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
  public class ToggleSwitchSizeConverter : IMultiValueConverter
  {
    public object Convert( object[] values, Type targetType, object parameter, CultureInfo culture )
    {
      if( values.Any( val => val is null ) || (values.Length != 4) )
        return 0d;

      var width = ( double )values[ 0 ];
      if( width < 0 )
        return 0;
      var borders = ( Thickness )values[ 1 ];
      var thumbWidth = ( double )values[ 2 ];
      if( thumbWidth < 0 )
        return 0;
      var thumbMargin = ( Thickness )values[ 3 ];
      if( thumbWidth < 0 )
        return 0;
      var innerWidth = ( width - borders.HorizontalThickness - ( thumbWidth / 2 ) - thumbMargin.Right ) * 2;

      return Math.Max( 0, innerWidth );
    }

    public object[] ConvertBack( object value, Type[] targetTypes, object parameter, CultureInfo culture )
    {
      throw new NotSupportedException();
    }
  }
}
