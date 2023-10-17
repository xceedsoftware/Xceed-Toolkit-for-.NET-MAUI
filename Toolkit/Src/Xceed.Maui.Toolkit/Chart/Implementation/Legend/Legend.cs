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


using System.Collections;

namespace Xceed.Maui.Toolkit
{
  public class Legend : Control
  {
    #region Public Properties

    #region ItemsSource

    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create( nameof( ItemsSource ), typeof( IEnumerable ), typeof( Legend ), null );

    public IEnumerable ItemsSource
    {
      get => ( IEnumerable )GetValue( ItemsSourceProperty );
      internal set => SetValue( ItemsSourceProperty, value );
    }

    #endregion

    #region SeriesItemTemplate

    public static readonly BindableProperty SeriesItemTemplateProperty = BindableProperty.Create( nameof( SeriesItemTemplate ), typeof( DataTemplate ), typeof( Legend ), null );

    public DataTemplate SeriesItemTemplate
    {
      get => ( DataTemplate )GetValue( SeriesItemTemplateProperty );
      set => SetValue( SeriesItemTemplateProperty, value );
    }

    #endregion

    #region Title

    public static readonly BindableProperty TitleProperty = BindableProperty.Create( nameof( Title ), typeof( string ), typeof( Legend ), null );

    public string Title
    {
      get => ( string )GetValue( TitleProperty );
      set => SetValue( TitleProperty, value );
    }

    #endregion

    #endregion
  }
}
