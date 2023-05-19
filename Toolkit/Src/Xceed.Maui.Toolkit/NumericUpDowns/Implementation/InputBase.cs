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
  public abstract class InputBase : Control
  {
    #region Public Properties 

    #region CultureInfo

    public static readonly BindableProperty CultureInfoProperty = BindableProperty.Create( "CultureInfo", typeof( CultureInfo ), typeof( InputBase ), CultureInfo.CurrentCulture, propertyChanged: OnCultureInfoChanged );

    public CultureInfo CultureInfo
    {
      get => ( CultureInfo )GetValue( CultureInfoProperty );
      set => SetValue( CultureInfoProperty, value );
    }

    private static void OnCultureInfoChanged( BindableObject bindable, object oldValue, object newValue )
    {
      var inputBase = bindable as InputBase;
      if( inputBase != null )
      {
        inputBase.OnCultureInfoChanged( ( CultureInfo )oldValue, ( CultureInfo )newValue );
      }
    }

    protected virtual void OnCultureInfoChanged( CultureInfo oldValue, CultureInfo newValue )
    {
    }

    #endregion

    #region Text

    public static readonly BindableProperty TextProperty = BindableProperty.Create( "Text", typeof( string ), typeof( InputBase ), null, propertyChanged: OnTextChanged );

    public string Text
    {
      get => ( string )GetValue( TextProperty );
      set => SetValue( TextProperty, value );
    }

    private static void OnTextChanged( BindableObject bindable, object oldValue, object newValue )
    {
      var inputBase = bindable as InputBase;
      if( inputBase != null )
      {
        inputBase.OnTextChanged( ( string )oldValue, ( string )newValue );
      }
    }

    protected virtual void OnTextChanged( string oldValue, string newValue )
    {

    }

    #endregion

    #region Watermark

    public static readonly BindableProperty WatermarkProperty = BindableProperty.Create( "Watermark", typeof( string ), typeof( InputBase ), null );

    public string Watermark
    {
      get => ( string )GetValue( WatermarkProperty );
      set => SetValue( WatermarkProperty, value );
    }

    #endregion

    #region WatermarkColor

    public static readonly BindableProperty WatermarkColorProperty = BindableProperty.Create( "WatermarkColor", typeof( Color ), typeof( InputBase ) );

    public Color WatermarkColor
    {
      get => ( Color )GetValue( WatermarkColorProperty );
      set => SetValue( WatermarkColorProperty, value );
    }

    #endregion

    #endregion
  }
}
