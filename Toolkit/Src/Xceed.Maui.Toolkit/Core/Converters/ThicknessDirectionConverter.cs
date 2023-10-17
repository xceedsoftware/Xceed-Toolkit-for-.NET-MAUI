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
  public class ThicknessDirectionConverter : IValueConverter
  {
    public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
    {
      var thickness = (Thickness)value;
      var isHorizontal = (parameter.ToString() == "horizontal");

      return isHorizontal
             ? new Thickness( thickness.Left, 0, thickness.Right, 0 )
             : new Thickness( 0, thickness.Top, 0, thickness.Bottom );
    }

    public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
    {
      throw new NotImplementedException();
    }
  }
}
