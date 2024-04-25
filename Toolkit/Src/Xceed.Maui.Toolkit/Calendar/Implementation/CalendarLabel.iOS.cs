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


using static Xceed.Maui.Toolkit.Border;

namespace Xceed.Maui.Toolkit
{
  public partial class CalendarLabel
  {
    #region Private Members

    private TouchRecognizer m_touchRecognizer;

    #endregion

    #region Partial Methods

    partial void InitializeForPlatform( object sender, EventArgs e )
    {
      var label = sender as CalendarLabel;
      if( label != null )
      {
        var contentView = label.Handler?.PlatformView as Microsoft.Maui.Platform.MauiLabel;
        if( contentView != null )
        {
          m_touchRecognizer = new TouchRecognizer();
          m_touchRecognizer.Pressed += this.OnPressed;
          m_touchRecognizer.Released += this.OnReleased;
          contentView.AddGestureRecognizer( m_touchRecognizer );
        }
      }
    }

    partial void UninitializeForPlatform( object sender, HandlerChangingEventArgs e )
    {
      var label = sender as CalendarLabel;
      if( label != null )
      {
        var contentView = label.Handler?.PlatformView as Microsoft.Maui.Platform.MauiLabel;
        if( contentView != null )
        {
          contentView.RemoveGestureRecognizer( m_touchRecognizer );
        }
      }
    }

    #endregion

    #region Event Handlers

    private void OnPressed( object sender, EventArgs args )
    {
      this.PointerDown();
    }

    private void OnReleased( object sender, EventArgs args )
    {
      this.PointerUp();
    }

    #endregion
  }
}
