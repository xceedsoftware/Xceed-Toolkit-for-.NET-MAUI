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
  public partial class Button
  {
    #region Priate Members

    private const string VisualState_PointerOver = "PointerOver";

    #endregion

    #region Partial Methods

    partial void ApplyTemplateForPlatform( Border oldBorder, Border newBorder )
    {
      if( oldBorder != null )
      {
        oldBorder.PointerEnter -= this.Border_PointerEnter;
        oldBorder.PointerLeave -= this.Border_PointerLeave;
      }

      if( m_border != null )
      {
        m_border.PointerEnter += this.Border_PointerEnter;
        m_border.PointerLeave += this.Border_PointerLeave;
      }
    }

    partial void InitializeForPlatform( object sender, EventArgs e )
    {
      var button = sender as Button;
      if( button != null )
      {
        var contentPanel = button.Handler?.PlatformView as ContentPanel;
        if( contentPanel != null )
        {
          contentPanel.IsTabStop = true;
          contentPanel.KeyDown += this.ContentPanel_KeyDown;
          contentPanel.KeyUp += this.ContentPanel_KeyUp;
        }
      }
    }

    partial void UninitializeForPlatform( object sender, HandlerChangingEventArgs e )
    {
      var button = sender as Button;
      if( button != null )
      {
        var contentPanel = button.Handler?.PlatformView as ContentPanel;
        if( contentPanel != null )
        {
          contentPanel.KeyDown -= this.ContentPanel_KeyDown;
          contentPanel.KeyUp -= this.ContentPanel_KeyUp;
        }
      }
    }

    partial void SetVisualStateAfterUnPressed()
    {
      VisualStateManager.GoToState( this, this.IsPointerOver ? VisualStateManager.CommonStates.PointerOver : VisualStateManager.CommonStates.Normal );
    }

    #endregion

    #region Event Handlers

    private void Border_PointerEnter( object sender, EventArgs e )
    {
      this.IsPointerOver = true;
      VisualStateManager.GoToState( this, VisualStateManager.CommonStates.PointerOver );  // Necessary if located in popup.
    }

    private void Border_PointerLeave( object sender, EventArgs e )
    {
      this.IsPressed = false;
      this.IsPointerOver = false;
      VisualStateManager.GoToState( this, VisualStateManager.CommonStates.Normal );   // Necessary if located in popup.
    }

    private void ContentPanel_KeyUp( object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e )
    {
      if( ( e.Key == Windows.System.VirtualKey.Space ) || ( e.Key == Windows.System.VirtualKey.Enter ) )
      {
        this.Button_PointerUp();
      }
    }

    private void ContentPanel_KeyDown( object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e )
    {
      if( ( e.Key == Windows.System.VirtualKey.Space ) || ( e.Key == Windows.System.VirtualKey.Enter ) )
      {
        this.Button_PointerDown();
      }
    }

    #endregion
  }
}
