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


using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml.Input;

namespace Xceed.Maui.Toolkit
{
  // All the code in this file is only included on Windows.
  public partial class ButtonSpinner
  {
    #region Public Properties

    #region IsPointerOver

    public static readonly BindableProperty IsPointerOverProperty = BindableProperty.Create( "IsPointerOver", typeof( bool ), typeof( ButtonSpinner ), false, propertyChanged: OnIsPointerOverChanged );

    public bool IsPointerOver
    {
      get => ( bool )GetValue( IsPointerOverProperty );
      private set => SetValue( IsPointerOverProperty, value );
    }

    private static void OnIsPointerOverChanged( BindableObject bindable, object oldValue, object newValue )
    {
      var button = bindable as ButtonSpinner;
      if( button != null )
      {
        button.OnIsPointerOverChanged( ( bool )oldValue, ( bool )newValue );
      }
    }

    protected virtual void OnIsPointerOverChanged( bool oldValue, bool newValue )
    {
    }

    #endregion

    #endregion

    #region Partial Methods

    partial void InitializeForPlatform( object sender, EventArgs e )
    {
      var border = sender as Border;
      if( border != null )
      {
        var contentPanel = border.Handler?.PlatformView as ContentPanel;
        if( contentPanel != null )
        {
          contentPanel.PointerEntered += this.PointerEntered;
          contentPanel.PointerExited += this.PointerExited;
        }
      }
    }

    partial void UninitializeForPlatform( object sender, HandlerChangingEventArgs e )
    {
      var border = sender as Border;
      if( border != null )
      {
        var contentPanel = border.Handler?.PlatformView as ContentPanel;
        if( contentPanel != null )
        {
          contentPanel.PointerEntered -= this.PointerEntered;
          contentPanel.PointerExited -= this.PointerExited;
        }
      }
    }

    #endregion

    #region Event Handlers

    private void PointerEntered( object sender, PointerRoutedEventArgs e )
    {
      this.IsPointerOver = true;
    }

    private void PointerExited( object sender, PointerRoutedEventArgs e )
    {
      this.IsPointerOver = false;
    }

    #endregion
  }
}
