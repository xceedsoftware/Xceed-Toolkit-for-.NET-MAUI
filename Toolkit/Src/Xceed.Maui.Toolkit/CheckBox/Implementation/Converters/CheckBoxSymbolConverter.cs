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
  public class CheckBoxSymbolConverter : IMultiValueConverter
  {
    public object Convert( object[] values, Type targetType, object parameter, CultureInfo culture )
    {
      if( ( values.Length != 5 ) || ( values[ 1 ] == null ) )
        return null;

      var ischecked = (bool?)values[ 0 ];
      var isThreeState = (bool)values[ 1 ];
      var uncheckedSymbol = (DataTemplate)values[ 2 ];
      var checkedSymbol = (DataTemplate)values[ 3 ];
      var indeterminateSymbol = (DataTemplate)values[ 4 ];

      return !ischecked.HasValue
            ? isThreeState ? indeterminateSymbol : uncheckedSymbol
            : ischecked.Value ? checkedSymbol : uncheckedSymbol;
    }

    public object[] ConvertBack( object value, Type[] targetTypes, object parameter, CultureInfo culture )
    {
      throw new NotImplementedException();
    }
  }
}
