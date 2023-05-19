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
  public partial class BorderGraphicsView
  {
    #region Partial Methods
    partial void InitializeForPlatform( object sender, EventArgs e )
    {
      var borderGraphicsView = sender as BorderGraphicsView;
      if( borderGraphicsView != null )
      {
        var platformTouchGraphicsView = borderGraphicsView.Handler?.PlatformView as PlatformTouchGraphicsView;
        if( platformTouchGraphicsView != null )
        {
          platformTouchGraphicsView.Touch += this.PlatformTouchGraphicsView_Touch;
        }
      }
    }
    partial void UninitializeForPlatform( object sender, HandlerChangingEventArgs e )
    {
      var borderGraphicsView = sender as BorderGraphicsView;
      if( borderGraphicsView != null )
      {
        var platformTouchGraphicsView = borderGraphicsView.Handler?.PlatformView as PlatformTouchGraphicsView;
        if( platformTouchGraphicsView != null )
        {
          platformTouchGraphicsView.Touch -= this.PlatformTouchGraphicsView_Touch;
        }
      }
    }

    #endregion

    #region Event Handlers

    private void PlatformTouchGraphicsView_Touch( object sender, Android.Views.View.TouchEventArgs e )
    {
      var actionMask = e.Event?.ActionMasked;
      if( actionMask != null )
      {
        var border = BorderGraphicsView.GetParentBorder( this );
        if( border != null )
        {
          if( actionMask == Android.Views.MotionEventActions.Down )
          {
            border.RaisePointerDownEvent( this, EventArgs.Empty );
          }
          else
          {
            border.RaisePointerUpEvent( this, EventArgs.Empty );
          }
        }
      }
    }

    #endregion

    #region Private Methods
    private static Border GetParentBorder( BorderGraphicsView view )
    {
      if( view == null )
        return null;

      var parent = view.Parent;
      while( parent != null )
      {
        if( parent is Border )
          return parent as Border;

        parent = parent.Parent;
      }

      return null;
    }

    #endregion
  }
}
