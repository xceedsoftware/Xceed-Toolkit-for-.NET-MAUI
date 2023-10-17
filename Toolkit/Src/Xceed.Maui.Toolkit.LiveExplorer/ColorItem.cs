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


using System.Windows.Input;

namespace Xceed.Maui.Toolkit.LiveExplorer
{
  public class ColorItem
  {
    public Color Color
    {
      get;
      set;
    }
    public string Name
    {
      get;
      set;
    }

    public ICommand SelectedColorCommand
    {
      get;
      private set;
    }

    public ColorItem( Color color, string name )
    {
      this.Color = color;
      this.Name = name;
      this.SelectedColorCommand = new Command(
     execute: ( arg ) =>
     {
       if( Enum.TryParse( arg.ToString(), out FluentDesignAccentColor color ) )
       {
         ICollection<ResourceDictionary> mergedDictionaries = Application.Current.Resources.MergedDictionaries;
         if( mergedDictionaries != null )
         {
           mergedDictionaries.Clear();
           mergedDictionaries.Add( new FluentDesign() { AccentColor = color } );
         }
       }
     } );
    }

    public override bool Equals( object obj )
    {
      var ci = obj as ColorItem;
      if( ci == null )
        return false;
      return ( ci.Color.Equals( Color ) && ci.Name.Equals( Name ) );
    }

    public override int GetHashCode()
    {
      return this.Color.GetHashCode() ^ this.Name.GetHashCode();
    }
  }

}
