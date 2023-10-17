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


using System.Collections.ObjectModel;
using System.Reflection;
using Xceed.Maui.Toolkit.Themes;

namespace Xceed.Maui.Toolkit.LiveExplorer
{
    public sealed class FluentColorHelper
  {
    public static ObservableCollection<ColorItem> AvailableFluentColors { get; } = FluentColorHelper.CreateAvailableColors();

    private static ObservableCollection<ColorItem> CreateAvailableColors()
    {
      var availableColors = new ObservableCollection<ColorItem>();

      foreach( var item in FluentColorHelper.GetFluentColors() )
      {
        var colorItem = new ColorItem( item.Value, item.Key );
        if( !availableColors.Contains( colorItem ) )
        {
          availableColors.Add( colorItem );
        }
      }

      return availableColors;
    }

    private static Dictionary<string, Color> GetFluentColors()
    {
      var colorProperties = typeof( FluentColors ).GetFields( BindingFlags.Public | BindingFlags.Static );
      return colorProperties.ToDictionary( p => p.Name, p => ( Color )p.GetValue( null ) );
    }
  }
}
