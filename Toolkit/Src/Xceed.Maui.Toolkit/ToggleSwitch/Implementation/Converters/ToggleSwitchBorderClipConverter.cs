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
using System.Globalization;

namespace Xceed.Maui.Toolkit
{
  public class ToggleSwitchBorderClipConverter : IMultiValueConverter
  {
    public object Convert( object[] values, Type targetType, object parameter, CultureInfo culture )
    {
      if( values.Length == 3 && values[ 0 ] is double && values[ 1 ] is double && values[ 2 ] is CornerRadius )
      {
        var width = ( double )values[ 0 ];
        var height = ( double )values[ 1 ];
        if( width >= 0 || height >= 0 )
        {
          var radius = ( CornerRadius )values[ 2 ];
          var clip = new RoundRectangleGeometry( radius, new Rect( 0, 0, width, height ) );
          return clip;
        }
      }

      return BindableProperty.UnsetValue;
    }
    public object[] ConvertBack( object value, Type[] targetTypes, object parameter, CultureInfo culture )
    {
      throw new NotSupportedException();
    }
  }
}
