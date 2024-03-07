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
using Windows.System;

namespace Xceed.Maui.Toolkit
{
  // All the code in this file is only included on Windows.
  public partial class ButtonSpinner
  {
    #region Partial Methods

    partial void InitializeForPlatform( object sender, EventArgs e )
    {
      var buttonSpinner = sender as ButtonSpinner;
      if( buttonSpinner != null )
      {
        var contentPanel = buttonSpinner.Handler?.PlatformView as ContentPanel;
        if( contentPanel != null )
        {
          contentPanel.PointerEntered += this.PointerEntered;
          contentPanel.PointerExited += this.PointerExited;
          contentPanel.PreviewKeyDown += this.OnPreviewKeyDown;
        }
      }
    }

    partial void UninitializeForPlatform( object sender, HandlerChangingEventArgs e )
    {
      var buttonSpinner = sender as ButtonSpinner;
      if( buttonSpinner != null )
      {
        var contentPanel = buttonSpinner.Handler?.PlatformView as ContentPanel;
        if( contentPanel != null )
        {
          contentPanel.PointerEntered -= this.PointerEntered;
          contentPanel.PointerExited -= this.PointerExited;
          contentPanel.PreviewKeyDown -= this.OnPreviewKeyDown;
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

    private void OnPreviewKeyDown( object sender, KeyRoutedEventArgs e )
    {
      switch( e.Key )
      {
        case VirtualKey.Up:
          {
            if( this.AllowSpin )
            {
              this.RaiseSpinnedEvent( this, new SpinEventArgs( SpinDirection.Increase ) );
              e.Handled = true;
            }
          }
          break;
        case VirtualKey.Down:
          {
            if( this.AllowSpin )
            {
              this.RaiseSpinnedEvent( this, new SpinEventArgs( SpinDirection.Decrease ) );
              e.Handled = true;
            }
          }
          break;
      }
    }

    #endregion
  }
}
