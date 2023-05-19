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


namespace Xceed.Maui.Toolkit
{
  public static class AppBuilderExtensions
  {
    #region Private Members

    private static FluentDesignAccentColor AccentColor = FluentDesign.DefaultAccentColor;
    private static bool IsFluentDesignThemeLoaded;

    #endregion

    #region Public Methods

    // This call is necessary from the user's MauiProgram.CreateMauiApp()
    // to be able to use the "http://schemas.xceed.com/maui/xaml/toolkit" namespace in xaml.
    // The XAML compiler will only locate this namespace from our Toolkit if called from the user's application.
    // ref : https://learn.microsoft.com/en-us/dotnet/maui/xaml/namespaces/custom-namespace-schemas?view=net-maui-7.0
    public static MauiAppBuilder UseXceedMauiToolkit( this MauiAppBuilder builder )
    {
      Task.Factory.StartNew( AppBuilderExtensions.SetDefaultStyles );

      return builder;
    }

    // This call is necessary from the user's MauiProgram.CreateMauiApp()
    // to be able to use the "http://schemas.xceed.com/maui/xaml/toolkit" namespace in xaml.
    // The XAML compiler will only locate this namespace from our Toolkit if called from the user's application.
    // ref : https://learn.microsoft.com/en-us/dotnet/maui/xaml/namespaces/custom-namespace-schemas?view=net-maui-7.0
    public static MauiAppBuilder UseXceedMauiToolkit( this MauiAppBuilder builder, FluentDesignAccentColor accentColor )
    {
      AppBuilderExtensions.AccentColor = accentColor;
      return builder.UseXceedMauiToolkit();
    }

    #endregion

    #region Internal Methods

    internal static void SetDefaultStylesCore()
    {
      var appMergedDictionaries = Application.Current?.Resources?.MergedDictionaries;
      if( appMergedDictionaries != null )
      {

        if( !appMergedDictionaries.Any( dict => dict.GetType() == typeof( FluentDesign ) ) )
        {
          var newStyles = new FluentDesign() { AccentColor = AppBuilderExtensions.AccentColor };
          appMergedDictionaries.Add( newStyles );
        }
      }
    }

    internal static void ResetIsFluentDesignLoaded()
    {
      AppBuilderExtensions.IsFluentDesignThemeLoaded = false;
    }

    internal static bool IsFluentDesignLoaded( Control control )
    {
      if( control == null )
        throw new ArgumentNullException( nameof( control ) );

      if( AppBuilderExtensions.IsFluentDesignThemeLoaded )
        return true;

      var data = control.Parent as VisualElement;
      while( data != null && !AppBuilderExtensions.IsFluentDesignThemeLoaded )
      {
        var mergedDictionaries = data.Resources?.MergedDictionaries;
        if( mergedDictionaries != null && mergedDictionaries.Any( dict => dict.GetType() == typeof( FluentDesign ) ) )
        {
          AppBuilderExtensions.IsFluentDesignThemeLoaded = true;
        }

        data = data.Parent as VisualElement;
      }

      if( !AppBuilderExtensions.IsFluentDesignThemeLoaded )
      {
        var appMergedDictionaries = Application.Current?.Resources?.MergedDictionaries;
        if( appMergedDictionaries != null )
        {
          AppBuilderExtensions.IsFluentDesignThemeLoaded = appMergedDictionaries.Any( dict => dict.GetType() == typeof( FluentDesign ) );
        }
      }

      return AppBuilderExtensions.IsFluentDesignThemeLoaded;
    }

    #endregion

    #region Private Methods

    private static async void SetDefaultStyles()
    {
      await AppBuilderExtensions.ApplicationExists();   

      Application.Current.PageAppearing += AppBuilderExtensions.ApplicationMainAppearing;
    }

    private static async Task<bool> ApplicationExists()
    {
      await Task.Factory.StartNew( () =>
      {
        while( Application.Current == null )
        {
        }
      } );

      return true;
    }

    #endregion

    #region Event Handlers

    private static void ApplicationMainAppearing( object sender, Page e )
    {
      Application.Current.PageAppearing -= AppBuilderExtensions.ApplicationMainAppearing;

      AppBuilderExtensions.SetDefaultStylesCore();
    }

    #endregion
  }
}
