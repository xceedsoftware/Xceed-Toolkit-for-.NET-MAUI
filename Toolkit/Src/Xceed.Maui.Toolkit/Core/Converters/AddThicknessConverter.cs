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
  public class AddThicknessConverter : IMultiValueConverter
  {
    public object Convert( object[] values, Type targetType, object parameter, CultureInfo culture )
    {
      if( (values == null) || (values.Length != 2) || (values[ 0 ] == null) || (values[ 1 ] == null) )
        return Thickness.Zero;

      var thickness1 = ( Thickness )values[ 0 ];
      var thickness2 = ( Thickness )values[ 1 ];

      return new Thickness( thickness1.Left + thickness2.Left,
                            thickness1.Top + thickness2.Top,
                            thickness1.Right + thickness2.Right,
                            thickness1.Bottom + thickness2.Bottom );
    }

    public object[] ConvertBack( object value, Type[] targetTypes, object parameter, CultureInfo culture )
    {
      throw new NotImplementedException();
    }
  }
}
