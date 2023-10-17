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
  public class ExpanderSizeConverter : IMultiValueConverter
  {
    public object Convert( object[] values, Type targetType, object parameter, CultureInfo culture )
    {
      if( values.Any( val => val is null ) )
        return 0d;

      if( parameter.ToString() == "Expander.Height" )
      {
        var height = ( double )values[ 0 ];
        var opacity = ( double )values[ 1 ];
        return height * opacity;
      }
      return 0d;
    }

    public object[] ConvertBack( object values, Type[] targetTypes, object parameter, CultureInfo culture )
    {
      return null;
    }
  }
}
