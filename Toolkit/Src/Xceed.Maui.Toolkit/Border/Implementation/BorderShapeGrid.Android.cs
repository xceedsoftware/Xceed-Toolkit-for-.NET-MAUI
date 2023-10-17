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
  public partial class BorderShapeGrid
  {
    #region Partial Methods

    partial void InitializeForPlatform( object sender, EventArgs e )
    {
      var borderShapeGrid = sender as BorderShapeGrid;
      if( borderShapeGrid != null )
      {
        var layoutViewGroup = borderShapeGrid.Handler?.PlatformView as LayoutViewGroup;
        if( layoutViewGroup != null )
        {
          layoutViewGroup.Touch += this.LayoutViewGroup_Touch;
        }
      }
    }

    partial void UninitializeForPlatform( object sender, HandlerChangingEventArgs e )
    {
      var borderShapeGrid = sender as BorderShapeGrid;
      if( borderShapeGrid != null )
      {
        var layoutViewGroup = borderShapeGrid.Handler?.PlatformView as LayoutViewGroup;
        if( layoutViewGroup != null )
        {
          layoutViewGroup.Touch -= this.LayoutViewGroup_Touch;
        }
      }
    }

    #endregion

    #region Event Handlers

    private void LayoutViewGroup_Touch( object sender, Android.Views.View.TouchEventArgs e )
    {
      var actionMask = e.Event?.ActionMasked;
      if( actionMask != null )
      {
        var border = BorderShapeGrid.GetParentBorder( this );
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

    private static Border GetParentBorder( BorderShapeGrid view )
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
