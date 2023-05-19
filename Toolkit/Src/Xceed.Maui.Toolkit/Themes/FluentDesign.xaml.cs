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


namespace Xceed.Maui.Toolkit;

public enum FluentDesignAccentColor
{
  Blue,
  BrownGray,
  Camouflage,
  CoolBlue,
  DarkGray,
  DarkLily,
  DarkMint,
  DarkOrange,
  DarkPurple,
  Deck,
  Desert,
  Gold,
  Gray,
  GrayBlue,
  Green,
  GreenBlue,
  GreenGrass,
  GreenLiddy,
  GreenMeadow,
  GreenSport,
  LightBlue,
  LightGold,
  LightLily,
  LightMint,
  LightMoss,
  LightOrange,
  LightOrchid,
  LightOxide,
  LightPink,
  LightPlum,
  LightPurple,
  LightRed,
  LightViolet,
  MetalicBlue,
  Moss,
  Orchid,
  Oxide,
  Pink,
  Plum,
  Red,
  RedBrick,
  RedMod,
  Sage,
  SeaBlue,
  SeaFoam,
  SteelBlue,
  Storm,
  Violet
}

public partial class FluentDesign : ResourceDictionary
{
  #region Internal Members

  internal const FluentDesignAccentColor DefaultAccentColor = FluentDesignAccentColor.Blue;

  #endregion

  #region Private Members

  private FluentDesignAccentColor m_accentColor;

  #endregion

  public FluentDesign()
  {
    this.AccentColor = FluentDesign.DefaultAccentColor;
  }

  #region Public Properties

  #region FluentDesignAccentColor

  public FluentDesignAccentColor AccentColor
  {
    get
    {
      return m_accentColor;
    }
    set
    {
      m_accentColor = value;
      this.UpdateAccentColor();
    }
  }

  #endregion

  #endregion

  #region Private Methods

  private void UpdateAccentColor()
  {
    var appMergedDictionaries = Application.Current?.Resources?.MergedDictionaries;
    if( appMergedDictionaries != null )
    {
      var brushesToRemove = appMergedDictionaries.Where( resDict => resDict is IBrushesResourceDictionary );
      brushesToRemove.ToList().ForEach( brush => appMergedDictionaries.Remove( brush ) );

      ResourceDictionary resourceDictionary;
      switch( m_accentColor )
      {
        case FluentDesignAccentColor.Blue:
          resourceDictionary = new Xceed.Maui.Toolkit.Themes.Brushes.Blue();
          break;
        case FluentDesignAccentColor.BrownGray:
          resourceDictionary = new Xceed.Maui.Toolkit.Themes.Brushes.BrownGray();
          break;
        case FluentDesignAccentColor.Camouflage:
          resourceDictionary = new Xceed.Maui.Toolkit.Themes.Brushes.Camouflage();
          break;
        case FluentDesignAccentColor.CoolBlue:
          resourceDictionary = new Xceed.Maui.Toolkit.Themes.Brushes.CoolBlue();
          break;
        case FluentDesignAccentColor.DarkGray:
          resourceDictionary = new Xceed.Maui.Toolkit.Themes.Brushes.DarkGray();
          break;
        case FluentDesignAccentColor.DarkLily:
          resourceDictionary = new Xceed.Maui.Toolkit.Themes.Brushes.DarkLily();
          break;
        case FluentDesignAccentColor.DarkMint:
          resourceDictionary = new Xceed.Maui.Toolkit.Themes.Brushes.DarkMint();
          break;
        case FluentDesignAccentColor.DarkOrange:
          resourceDictionary = new Xceed.Maui.Toolkit.Themes.Brushes.DarkOrange();
          break;
        case FluentDesignAccentColor.DarkPurple:
          resourceDictionary = new Xceed.Maui.Toolkit.Themes.Brushes.DarkPurple();
          break;
        case FluentDesignAccentColor.Deck:
          resourceDictionary = new Xceed.Maui.Toolkit.Themes.Brushes.Deck();
          break;
        case FluentDesignAccentColor.Desert:
          resourceDictionary = new Xceed.Maui.Toolkit.Themes.Brushes.Desert();
          break;
        case FluentDesignAccentColor.Gold:
          resourceDictionary = new Xceed.Maui.Toolkit.Themes.Brushes.Gold();
          break;
        case FluentDesignAccentColor.Gray:
          resourceDictionary = new Xceed.Maui.Toolkit.Themes.Brushes.Gray();
          break;
        case FluentDesignAccentColor.GrayBlue:
          resourceDictionary = new Xceed.Maui.Toolkit.Themes.Brushes.GrayBlue();
          break;
        case FluentDesignAccentColor.Green:
          resourceDictionary = new Xceed.Maui.Toolkit.Themes.Brushes.Green();
          break;
        case FluentDesignAccentColor.GreenBlue:
          resourceDictionary = new Xceed.Maui.Toolkit.Themes.Brushes.GreenBlue();
          break;
        case FluentDesignAccentColor.GreenGrass:
          resourceDictionary = new Xceed.Maui.Toolkit.Themes.Brushes.GreenGrass();
          break;
        case FluentDesignAccentColor.GreenLiddy:
          resourceDictionary = new Xceed.Maui.Toolkit.Themes.Brushes.GreenLiddy();
          break;
        case FluentDesignAccentColor.GreenMeadow:
          resourceDictionary = new Xceed.Maui.Toolkit.Themes.Brushes.GreenMeadow();
          break;
        case FluentDesignAccentColor.GreenSport:
          resourceDictionary = new Xceed.Maui.Toolkit.Themes.Brushes.GreenSport();
          break;
        case FluentDesignAccentColor.LightBlue:
          resourceDictionary = new Xceed.Maui.Toolkit.Themes.Brushes.LightBlue();
          break;
        case FluentDesignAccentColor.LightGold:
          resourceDictionary = new Xceed.Maui.Toolkit.Themes.Brushes.LightGold();
          break;
        case FluentDesignAccentColor.LightLily:
          resourceDictionary = new Xceed.Maui.Toolkit.Themes.Brushes.LightLily();
          break;
        case FluentDesignAccentColor.LightMint:
          resourceDictionary = new Xceed.Maui.Toolkit.Themes.Brushes.LightMint();
          break;
        case FluentDesignAccentColor.LightMoss:
          resourceDictionary = new Xceed.Maui.Toolkit.Themes.Brushes.LightMoss();
          break;
        case FluentDesignAccentColor.LightOrange:
          resourceDictionary = new Xceed.Maui.Toolkit.Themes.Brushes.LightOrange();
          break;
        case FluentDesignAccentColor.LightOrchid:
          resourceDictionary = new Xceed.Maui.Toolkit.Themes.Brushes.LightOrchid();
          break;
        case FluentDesignAccentColor.LightOxide:
          resourceDictionary = new Xceed.Maui.Toolkit.Themes.Brushes.LightOxide();
          break;
        case FluentDesignAccentColor.LightPink:
          resourceDictionary = new Xceed.Maui.Toolkit.Themes.Brushes.LightPink();
          break;
        case FluentDesignAccentColor.LightPlum:
          resourceDictionary = new Xceed.Maui.Toolkit.Themes.Brushes.LightPlum();
          break;
        case FluentDesignAccentColor.LightPurple:
          resourceDictionary = new Xceed.Maui.Toolkit.Themes.Brushes.LightPurple();
          break;
        case FluentDesignAccentColor.LightRed:
          resourceDictionary = new Xceed.Maui.Toolkit.Themes.Brushes.LightRed();
          break;
        case FluentDesignAccentColor.LightViolet:
          resourceDictionary = new Xceed.Maui.Toolkit.Themes.Brushes.LightViolet();
          break;
        case FluentDesignAccentColor.MetalicBlue:
          resourceDictionary = new Xceed.Maui.Toolkit.Themes.Brushes.MetalicBlue();
          break;
        case FluentDesignAccentColor.Moss:
          resourceDictionary = new Xceed.Maui.Toolkit.Themes.Brushes.Moss();
          break;
        case FluentDesignAccentColor.Orchid:
          resourceDictionary = new Xceed.Maui.Toolkit.Themes.Brushes.Orchid();
          break;
        case FluentDesignAccentColor.Oxide:
          resourceDictionary = new Xceed.Maui.Toolkit.Themes.Brushes.Oxide();
          break;
        case FluentDesignAccentColor.Pink:
          resourceDictionary = new Xceed.Maui.Toolkit.Themes.Brushes.Pink();
          break;
        case FluentDesignAccentColor.Plum:
          resourceDictionary = new Xceed.Maui.Toolkit.Themes.Brushes.Plum();
          break;
        case FluentDesignAccentColor.Red:
          resourceDictionary = new Xceed.Maui.Toolkit.Themes.Brushes.Red();
          break;
        case FluentDesignAccentColor.RedBrick:
          resourceDictionary = new Xceed.Maui.Toolkit.Themes.Brushes.RedBrick();
          break;
        case FluentDesignAccentColor.RedMod:
          resourceDictionary = new Xceed.Maui.Toolkit.Themes.Brushes.RedMod();
          break;
        case FluentDesignAccentColor.Sage:
          resourceDictionary = new Xceed.Maui.Toolkit.Themes.Brushes.Sage();
          break;
        case FluentDesignAccentColor.SeaBlue:
          resourceDictionary = new Xceed.Maui.Toolkit.Themes.Brushes.SeaBlue();
          break;
        case FluentDesignAccentColor.SeaFoam:
          resourceDictionary = new Xceed.Maui.Toolkit.Themes.Brushes.SeaFoam();
          break;
        case FluentDesignAccentColor.SteelBlue:
          resourceDictionary = new Xceed.Maui.Toolkit.Themes.Brushes.SteelBlue();
          break;
        case FluentDesignAccentColor.Storm:
          resourceDictionary = new Xceed.Maui.Toolkit.Themes.Brushes.Storm();
          break;
        default:
          resourceDictionary = new Xceed.Maui.Toolkit.Themes.Brushes.Violet();
          break;
      }

      appMergedDictionaries.Add( resourceDictionary );
    }

    this.InitializeComponent();
  }


  #endregion
}
