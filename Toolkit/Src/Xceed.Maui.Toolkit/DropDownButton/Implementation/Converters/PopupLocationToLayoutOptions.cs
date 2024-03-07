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
  public class PopupLocationToLayoutOptions : IValueConverter
  {
    public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
    {
      var popupLocation = (PopupLocation)value;
      var isHorizontal = (parameter.ToString() == "H");

      switch( popupLocation )
      {
        case PopupLocation.BottomLeft: return isHorizontal ? LayoutOptions.Start : LayoutOptions.End;
        case PopupLocation.Bottom: return isHorizontal ? LayoutOptions.Center : LayoutOptions.End;
        case PopupLocation.BottomRight: return isHorizontal ? LayoutOptions.End : LayoutOptions.End;
        case PopupLocation.TopLeft: return isHorizontal ? LayoutOptions.Start : LayoutOptions.Start;
        case PopupLocation.Top: return isHorizontal ? LayoutOptions.Center : LayoutOptions.Start;
        case PopupLocation.TopRight: return isHorizontal ? LayoutOptions.End : LayoutOptions.Start;
        case PopupLocation.Right: return isHorizontal ? LayoutOptions.End : LayoutOptions.Center;
        case PopupLocation.Left: return isHorizontal ? LayoutOptions.Start : LayoutOptions.Center;
        default: throw new InvalidDataException( "Unknown popupLocation." );
      }
    }

    public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
    {
      throw new NotImplementedException();
    }
  }
}
