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
  public class FilePickerButtonCornerRadiusConverter : IMultiValueConverter
  {
    public object Convert( object[] values, Type targetType, object parameter, CultureInfo culture )
    {
      if( ( values == null ) || ( values.Length != 2 ) || ( values[ 0 ] == null ) || ( values[ 1 ] == null ) )
        return new CornerRadius(0);
      var cornerRadius = ( CornerRadius )values[ 0 ];
      var isUnderline = ( bool )values[ 1 ];
      if ( isUnderline )
      {
        return new CornerRadius( 0, cornerRadius.TopRight, 0, 0 );
      }
      else
      {
        return new CornerRadius( 0, cornerRadius.TopRight, 0, cornerRadius.BottomRight );
      }
    }

    public object[] ConvertBack( object value, Type[] targetTypes, object parameter, CultureInfo culture )
    {
      throw new NotImplementedException();
    }
  }
}
