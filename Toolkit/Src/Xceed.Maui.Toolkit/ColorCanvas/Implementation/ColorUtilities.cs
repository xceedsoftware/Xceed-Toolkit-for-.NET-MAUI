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


using System.Reflection;

namespace Xceed.Maui.Toolkit
{
  internal static class ColorUtilities
  {
    public static readonly Dictionary<string, Color> KnownColors = ColorUtilities.GetKnownColors();

    public static string GetColorName( this Color color )
    {
      var colorName = ColorUtilities.KnownColors.Where( kvp => kvp.Value.Equals( color ) ).Select( kvp => kvp.Key ).FirstOrDefault();

      if( String.IsNullOrEmpty( colorName ) )
      {
        colorName = color.ToString();
      }

      return colorName;
    }

    public static string FormatColorString( string stringToFormat, bool isUsingAlphaChannel )
    {
      if( !isUsingAlphaChannel && ( stringToFormat.Length == 9 ) )
        return stringToFormat.Remove( 1, 2 );

      return stringToFormat;
    }

    private static Dictionary<string, Color> GetKnownColors()
    {
      var colorProperties = typeof( Colors ).GetProperties( BindingFlags.Static | BindingFlags.Public );
      return colorProperties.ToDictionary( p => p.Name, p => ( Color )p.GetValue( null, null ) );
    }

    public static HsvColor ConvertRgbToHsv( int r, int g, int b )
    {
      double delta, min;
      double h = 0, s, v;

      min = Math.Min( Math.Min( r, g ), b );
      v = Math.Max( Math.Max( r, g ), b );
      delta = v - min;

      s = ( v == 0.0 ) ? 0 : delta / v;

      if( s == 0 )
      {
        h = 0.0;
      }
      else
      {
        if( r == v )
        {
          h = ( g - b ) / delta;
        }
        else if( g == v )
        {
          h = 2 + ( b - r ) / delta;
        }
        else if( b == v )
        {
          h = 4 + ( r - g ) / delta;
        }

        h *= 60;
        if( h < 0.0 )
        {
          h = h + 360;
        }
      }

      return new HsvColor
      {
        H = h,
        S = s,
        V = v / 255
      };
    }


    public static Color ConvertHsvToRgb( double h, double s, double v )
    {
      double r = 0, g = 0, b = 0;

      if( s == 0 )
      {
        r = v;
        g = v;
        b = v;
      }
      else
      {
        int i;
        double f, p, q, t;

        h = ( h == 360 ) ? 0 : h / 60;        

        i = ( int )Math.Truncate( h );
        f = h - i;

        p = v * ( 1.0 - s );
        q = v * ( 1.0 - ( s * f ) );
        t = v * ( 1.0 - ( s * ( 1.0 - f ) ) );

        switch( i )
        {
          case 0:
            {
              r = v;
              g = t;
              b = p;
              break;
            }
          case 1:
            {
              r = q;
              g = v;
              b = p;
              break;
            }
          case 2:
            {
              r = p;
              g = v;
              b = t;
              break;
            }
          case 3:
            {
              r = p;
              g = q;
              b = v;
              break;
            }
          case 4:
            {
              r = t;
              g = p;
              b = v;
              break;
            }
          default:
            {
              r = v;
              g = p;
              b = q;
              break;
            }
        }
      }

      return Color.FromRgba( ( byte )( Math.Round( r * 255 ) ), ( byte )( Math.Round( g * 255 ) ), ( byte )( Math.Round( b * 255 ) ), ( byte )255 );
    }

    public static List<Color> GenerateHsvSpectrum()
    {
      var colorsList = new List<Color>();

      int hStep = 60;

      for( int h = 0; h < 360; h += hStep )
      {
        colorsList.Add( ColorUtilities.ConvertHsvToRgb( h, 1, 1 ) );
      }

      colorsList.Add( ColorUtilities.ConvertHsvToRgb( 0, 1, 1 ) );

      return colorsList;
    }
  }
}
