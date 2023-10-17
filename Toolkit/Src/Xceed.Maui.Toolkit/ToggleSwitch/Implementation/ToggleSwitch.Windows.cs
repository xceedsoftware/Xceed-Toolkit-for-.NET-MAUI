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

namespace Xceed.Maui.Toolkit
{
  // All the code in this file is only included on Windows.
  public partial class ToggleSwitch
  {
    #region Partial Methods

    partial void InitializeForPlatform( object sender, EventArgs e )
    {
      var toggleSwitch = sender as ToggleSwitch;
      if( toggleSwitch != null )
      {
        var contentPanel = toggleSwitch.Handler?.PlatformView as ContentPanel;
        if( contentPanel != null )
        {
          contentPanel.KeyUp += this.ContentPanel_KeyUp;
        }
      }
    }

    partial void UninitializeForPlatform( object sender, HandlerChangingEventArgs e )
    {
      var toggleSwitch = sender as ToggleSwitch;
      if( toggleSwitch != null )
      {
        var contentPanel = toggleSwitch.Handler?.PlatformView as ContentPanel;
        if( contentPanel != null )
        {
          contentPanel.KeyUp -= this.ContentPanel_KeyUp;
        }
      }
    }

    #endregion

    #region Event Handlers

    private void ContentPanel_KeyUp( object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e )
    {
      if( e.Key == Windows.System.VirtualKey.Space )
      {
        this.ToggleSwitch_ChangeIsCheckedProperty();
      }
    }

    #endregion
  }
}
