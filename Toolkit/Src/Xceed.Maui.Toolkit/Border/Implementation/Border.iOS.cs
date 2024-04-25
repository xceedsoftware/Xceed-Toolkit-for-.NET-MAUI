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
using Foundation;

namespace Xceed.Maui.Toolkit
{
  public partial class Border
  {
    #region Private Members

    private TouchRecognizer m_touchRecognizer;

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
          m_touchRecognizer = new TouchRecognizer();
          m_touchRecognizer.Pressed += this.OnPressed;
          m_touchRecognizer.Released += this.OnReleased;
          contentView.AddGestureRecognizer( m_touchRecognizer );
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
          contentView.RemoveGestureRecognizer( m_touchRecognizer );
        }
      }
    }

    #endregion

    #region Event Handlers

    private void OnPressed( object sender, EventArgs args )
    {
      this.RaisePointerDownEvent( this, EventArgs.Empty );
    }

    private void OnReleased( object sender, EventArgs args )
    {
      this.RaisePointerUpEvent( this, EventArgs.Empty );
    }

    #endregion

    #region Internal Classes

    internal class TouchRecognizer : UIGestureRecognizer
    {
      public event EventHandler Pressed;
      public event EventHandler Released;

      public override void TouchesBegan( NSSet touches, UIEvent uiEvent )
      {
        base.TouchesBegan( touches, uiEvent );

        this.State = UIGestureRecognizerState.Possible;

        this.Pressed?.Invoke( this, EventArgs.Empty );
      }

      public override void TouchesEnded( NSSet touches, UIEvent uiEvent )
      {
        base.TouchesEnded( touches, uiEvent );

        this.State = UIGestureRecognizerState.Cancelled;

        this.Released?.Invoke( this, EventArgs.Empty );
      }
    }

    #endregion
  }
}
