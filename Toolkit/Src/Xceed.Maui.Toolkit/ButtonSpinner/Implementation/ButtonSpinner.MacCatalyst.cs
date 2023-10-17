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


using UIKit;

namespace Xceed.Maui.Toolkit
{
  // All the code in this file is only included on MacOS.
  public partial class ButtonSpinner
  {
    #region Private Members

    private UIHoverGestureRecognizer m_uiHoverGestureRecognizer;

    #endregion

    #region Partial Methods

    partial void InitializeForPlatform( object sender, EventArgs e )
    {
      var border = sender as Border;
      if( border != null )
      {
        var contentView = border.Handler?.PlatformView as Microsoft.Maui.Platform.ContentView;
        if( contentView != null )
        {
          m_uiHoverGestureRecognizer = new UIHoverGestureRecognizer( this.OnPointerHover );
          contentView.AddGestureRecognizer( m_uiHoverGestureRecognizer );
        }
      }
    }

    partial void UninitializeForPlatform( object sender, HandlerChangingEventArgs e )
    {
      var border = sender as Border;
      if( border != null )
      {
        var contentView = border.Handler?.PlatformView as Microsoft.Maui.Platform.ContentView;
        if( contentView != null )
        {
          contentView.RemoveGestureRecognizer( m_uiHoverGestureRecognizer );
        }
      }
    }

    #endregion

    #region Event Handlers

    private void OnPointerHover( UIHoverGestureRecognizer recognizer )
    {
      if( recognizer == null )
        return;

      if( recognizer.State == UIGestureRecognizerState.Began )
      {
        this.IsPointerOver = true;
      }
      else if( recognizer.State == UIGestureRecognizerState.Ended )
      {
        this.IsPointerOver = false;
      }
    }

    #endregion
  }
}
