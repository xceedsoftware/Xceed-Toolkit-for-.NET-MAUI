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


#nullable enable

using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls.Primitives;

namespace Xceed.Maui.Toolkit
{
  public partial class PopupHandler : ViewHandler<IPopup, Microsoft.UI.Xaml.Controls.Primitives.Popup>
  {
    #region Private Members

    private bool m_isContentWidthSetByUser;
    private bool m_isContentHeightSetByUser;

    #endregion


    #region Override Methods

    protected override Microsoft.UI.Xaml.Controls.Primitives.Popup CreatePlatformView()
    {
      return new Microsoft.UI.Xaml.Controls.Primitives.Popup();
    }

    protected override void ConnectHandler( Microsoft.UI.Xaml.Controls.Primitives.Popup platformView )
    {
      if( platformView == null )
        throw new ArgumentNullException( "platformView" );

      if( ( this.VirtualView != null ) && ( this.VirtualView.Content != null ) && this.VirtualView.Handler is PopupHandler handler )
      {
        var mauiContext = this.MauiContext;
        if( mauiContext == null )
          throw new NullReferenceException( "mauiContext is null." );

        var platformContent = handler.VirtualView.Content?.ToPlatform( mauiContext );
        platformView.Child = platformContent;
        if( platformView.Child is FrameworkElement content )
        {
          m_isContentWidthSetByUser = !double.IsNaN( content.Width );
          m_isContentHeightSetByUser = !double.IsNaN( content.Height );
        }
      }

      platformView.Closed += this.PlatformView_Closed;

      base.ConnectHandler( platformView );
    }

    protected override void DisconnectHandler( Microsoft.UI.Xaml.Controls.Primitives.Popup platformView )
    {
      if( platformView == null )
        throw new ArgumentNullException( "platformView" );

      var layoutPanel = platformView?.Parent as LayoutPanel;
      if( layoutPanel != null )
      {
        layoutPanel.Children.Remove( platformView );
      }

#pragma warning disable CS8602
      platformView.Closed -= this.PlatformView_Closed;
#pragma warning restore CS8602

      base.DisconnectHandler( platformView );
    }

    #endregion

    #region Public Methods

    public static void MapAnchor( PopupHandler handler, IPopup popup )
    {
      if( handler == null )
        throw new ArgumentNullException( "handler" );
      if( handler.PlatformView == null )
        throw new ArgumentNullException( "handler.PlatformView" );
      if( popup == null )
        throw new ArgumentNullException( "popup" );

      var mauiContext = handler.MauiContext;
      if( mauiContext != null )
      {
        handler.PlatformView.PlacementTarget = popup.Anchor?.ToPlatform( mauiContext );
        handler.PlatformView.DesiredPlacement = PopupHandler.GetDesiredAnchorPlacement( popup );
      }
    }

    public static void MapIsModal( PopupHandler handler, IPopup popup )
    {
      if( handler == null )
        throw new ArgumentNullException( "handler" );
      if( handler.PlatformView == null )
        throw new ArgumentNullException( "handler.PlatformView" );
      if( popup == null )
        throw new ArgumentNullException( "popup" );

      handler.PlatformView.IsLightDismissEnabled = !popup.IsModal;
    }

    public static void MapClose( PopupHandler handler, IPopup popup, object? result )
    {
      if( handler == null )
        throw new ArgumentNullException( "handler" );
      if( handler.MauiContext == null )
        throw new ArgumentNullException( "handler.MauiContext" );
      if( popup == null )
        throw new ArgumentNullException( "popup" );

      var platformView = handler.PlatformView;
      if( platformView == null )
        throw new ArgumentNullException( "platformView" );

      var parent = PopupHandler.GetParent( popup ).ToPlatform( handler.MauiContext );
      parent.IsHitTestVisible = true;
      platformView.IsOpen = false;

      // Disconnect handler only if Popup wasn't part of the original xaml and was added with something like: var myPopup = new Popup().
      if( !Popup.IsFromXaml( popup ) )
      {
        handler.DisconnectHandler( handler.PlatformView );
      }
    }

    public static void MapOpen( PopupHandler handler, IPopup popup, object? result )
    {
      if( handler == null )
        throw new ArgumentNullException( "handler" );
      if( handler.MauiContext == null )
        throw new ArgumentNullException( "handler.MauiContext" );
      if( popup == null )
        throw new ArgumentNullException( "popup" );

      var platformView = handler.PlatformView;
      if( platformView == null )
        throw new ArgumentNullException( "platformView" );

      var parent = PopupHandler.GetParent( popup ).ToPlatform( handler.MauiContext );
      parent.IsHitTestVisible = !popup.IsModal;
      handler.PlatformView.XamlRoot = parent.XamlRoot;
      handler.PlatformView.IsHitTestVisible = true;
      handler.PlatformView.IsOpen = true;

      PopupHandler.SetSize( handler, popup );
      PopupHandler.SetLayout( platformView, popup );
    }

    public static void MapUpdateSize( PopupHandler handler, IPopup popup, object? result )
    {
      PopupHandler.SetSize( handler, popup );
    }

    #endregion

    #region Private Methods

    // This method will position the popup around an anchor defined in XAML, based on Alignment.
    private static PopupPlacementMode GetDesiredAnchorPlacement( IPopup popup )
    {
      if( popup == null )
        throw new ArgumentNullException( "popup" );

      switch(popup.HorizontalLayoutAlignment, popup.VerticalLayoutAlignment)
      {
        case (Microsoft.Maui.Primitives.LayoutAlignment.Start, Microsoft.Maui.Primitives.LayoutAlignment.Start ):
        case (Microsoft.Maui.Primitives.LayoutAlignment.Start, Microsoft.Maui.Primitives.LayoutAlignment.Fill ):
        case (Microsoft.Maui.Primitives.LayoutAlignment.Fill, Microsoft.Maui.Primitives.LayoutAlignment.Start ):
        case (Microsoft.Maui.Primitives.LayoutAlignment.Fill, Microsoft.Maui.Primitives.LayoutAlignment.Fill ):
          return PopupPlacementMode.TopEdgeAlignedLeft;
        case (Microsoft.Maui.Primitives.LayoutAlignment.Start, Microsoft.Maui.Primitives.LayoutAlignment.Center ):
        case (Microsoft.Maui.Primitives.LayoutAlignment.Fill, Microsoft.Maui.Primitives.LayoutAlignment.Center ):
          return PopupPlacementMode.Left;
        case (Microsoft.Maui.Primitives.LayoutAlignment.Start, Microsoft.Maui.Primitives.LayoutAlignment.End ):
        case (Microsoft.Maui.Primitives.LayoutAlignment.Fill, Microsoft.Maui.Primitives.LayoutAlignment.End ):
          return PopupPlacementMode.BottomEdgeAlignedLeft;
        case (Microsoft.Maui.Primitives.LayoutAlignment.Center, Microsoft.Maui.Primitives.LayoutAlignment.Start ):
        case (Microsoft.Maui.Primitives.LayoutAlignment.Center, Microsoft.Maui.Primitives.LayoutAlignment.Fill ):
          return PopupPlacementMode.Top;
        case (Microsoft.Maui.Primitives.LayoutAlignment.Center, Microsoft.Maui.Primitives.LayoutAlignment.End ):
          return PopupPlacementMode.Bottom;
        case (Microsoft.Maui.Primitives.LayoutAlignment.End, Microsoft.Maui.Primitives.LayoutAlignment.Start ):
        case (Microsoft.Maui.Primitives.LayoutAlignment.End, Microsoft.Maui.Primitives.LayoutAlignment.Fill ):
          return PopupPlacementMode.TopEdgeAlignedRight;
        case (Microsoft.Maui.Primitives.LayoutAlignment.End, Microsoft.Maui.Primitives.LayoutAlignment.Center ):
          return PopupPlacementMode.Right;
        case (Microsoft.Maui.Primitives.LayoutAlignment.End, Microsoft.Maui.Primitives.LayoutAlignment.End ):
          return PopupPlacementMode.BottomEdgeAlignedRight;
        default:
          return PopupPlacementMode.Bottom;
      }
    }

    private static void SetLayout( Microsoft.UI.Xaml.Controls.Primitives.Popup platformPopup, IPopup popup )
    {
      if( platformPopup == null )
        throw new ArgumentNullException( "platformPopup" );

      if( popup == null )
        throw new ArgumentNullException( "mauiPopup" );

      if( popup.Anchor != null )
        return;

      platformPopup.HorizontalOffset = 0;
      platformPopup.VerticalOffset = 0;

      // Popup was added from code-behind : var myPopup = new Popup().
      if( !Popup.IsFromXaml( popup ) )
      {
        PopupHandler.SetLayoutFromWindow( platformPopup, popup );
      }
      // Popup is part of XAML.
      else
      {
        PopupHandler.SetLayoutWithoutAnchorFromParentXaml( platformPopup, popup );
      }
    }

    // This method will position the popup inside the popup's parent (defined in XAML), based on Alignment.
    private static void SetLayoutWithoutAnchorFromParentXaml( Microsoft.UI.Xaml.Controls.Primitives.Popup platformPopup, IPopup popup )
    {
      if( platformPopup == null )
        throw new ArgumentNullException( "platformPopup" );
      if( popup == null )
        throw new ArgumentNullException( "popup" );
      if( popup.Handler?.MauiContext == null )
        throw new InvalidDataException( "MauiContext is null.");

      var parent = PopupHandler.GetParent( popup ) as VisualElement;
      if( parent == null )
        throw new InvalidDataException( "parent can't be found for Popup." );

      var parentWidth = parent.Width;
      var parentHeight = parent.Height;
      var popupParent = parent.ToPlatform( popup.Handler.MauiContext );
      var parentPosition = popupParent.TransformToVisual( null ).TransformPoint( new Windows.Foundation.Point( 0, 0 ) );

      var contentSize = new Size( platformPopup.Width, platformPopup.Height );
      var offsetX = 0d;
      var offsetY = 0d;

      switch(popup.HorizontalLayoutAlignment, popup.VerticalLayoutAlignment)
      {
        case (Microsoft.Maui.Primitives.LayoutAlignment.Start, Microsoft.Maui.Primitives.LayoutAlignment.Center ):
        case (Microsoft.Maui.Primitives.LayoutAlignment.Fill, Microsoft.Maui.Primitives.LayoutAlignment.Center ):
          {
            offsetY = ( parentHeight / 2 ) - ( contentSize.Height / 2 );
          }
          break;
        case (Microsoft.Maui.Primitives.LayoutAlignment.Start, Microsoft.Maui.Primitives.LayoutAlignment.End ):
        case (Microsoft.Maui.Primitives.LayoutAlignment.Fill, Microsoft.Maui.Primitives.LayoutAlignment.End ):
          {
            offsetY = parentHeight - contentSize.Height;
          }
          break;
        case (Microsoft.Maui.Primitives.LayoutAlignment.Center, Microsoft.Maui.Primitives.LayoutAlignment.Start ):
        case (Microsoft.Maui.Primitives.LayoutAlignment.Center, Microsoft.Maui.Primitives.LayoutAlignment.Fill ):
          {
            offsetX = ( parentWidth / 2 ) - ( contentSize.Width / 2 );
          }
          break;
        case (Microsoft.Maui.Primitives.LayoutAlignment.Center, Microsoft.Maui.Primitives.LayoutAlignment.Center ):
          {
            offsetX = ( parentWidth / 2 ) - ( contentSize.Width / 2 );
            offsetY = ( parentHeight / 2 ) - ( contentSize.Height / 2 );
          }
          break;
        case (Microsoft.Maui.Primitives.LayoutAlignment.Center, Microsoft.Maui.Primitives.LayoutAlignment.End ):
          {
            offsetX = ( parentWidth / 2 ) - ( contentSize.Width / 2 );
            offsetY = parentHeight - contentSize.Height;
          }
          break;
        case (Microsoft.Maui.Primitives.LayoutAlignment.End, Microsoft.Maui.Primitives.LayoutAlignment.Start ):
        case (Microsoft.Maui.Primitives.LayoutAlignment.End, Microsoft.Maui.Primitives.LayoutAlignment.Fill ):
          {
            offsetX = parentWidth - contentSize.Width;
          }
          break;
        case (Microsoft.Maui.Primitives.LayoutAlignment.End, Microsoft.Maui.Primitives.LayoutAlignment.Center ):
          {
            offsetX = parentWidth - contentSize.Width;
            offsetY = ( parentHeight / 2 ) - ( contentSize.Height / 2 );
          }
          break;
        case (Microsoft.Maui.Primitives.LayoutAlignment.End, Microsoft.Maui.Primitives.LayoutAlignment.End ):
          {
            offsetX = parentWidth - contentSize.Width;
            offsetY = parentHeight - contentSize.Height;
          }
          break;
      }

      platformPopup.HorizontalOffset = parentPosition.X + offsetX;
      platformPopup.VerticalOffset = parentPosition.Y + offsetY;
    }

    // This method will position the popup inside the Application, based on Alignment.
    private static void SetLayoutFromWindow( Microsoft.UI.Xaml.Controls.Primitives.Popup platformPopup, IPopup mauiPopup )
    {
      if( platformPopup == null )
        throw new ArgumentNullException( "platformPopup" );
      if( mauiPopup == null )
        throw new ArgumentNullException( "mauiPopup" );

      if( mauiPopup.Anchor != null )
        return;

      var windowWidth = 0d;
      var windowHeight = 0d;

      var parentWindow = ( mauiPopup as Popup )?.Window?.Handler?.PlatformView as MauiWinUIWindow;
      if( parentWindow != null )
      {
        windowWidth = parentWindow.Bounds.Width;
        windowHeight = parentWindow.Bounds.Height;
      }
      else
      {
        var mainWindow = Microsoft.Maui.Controls.Application.Current?.Windows[ 0 ];
        if( mainWindow != null )
        {
          windowWidth = mainWindow.Width;
          windowHeight = mainWindow.Height;
        }
        else
          throw new InvalidDataException( "Can't find MainWindow." );
      }

      var contentSize = new Size( platformPopup.Width, platformPopup.Height );

      switch(mauiPopup.HorizontalLayoutAlignment, mauiPopup.VerticalLayoutAlignment)
      {
        case (Microsoft.Maui.Primitives.LayoutAlignment.Start, Microsoft.Maui.Primitives.LayoutAlignment.Start ):
        case (Microsoft.Maui.Primitives.LayoutAlignment.Start, Microsoft.Maui.Primitives.LayoutAlignment.Fill ):
        case (Microsoft.Maui.Primitives.LayoutAlignment.Fill, Microsoft.Maui.Primitives.LayoutAlignment.Start ):
          {
            platformPopup.HorizontalOffset = 0;
            platformPopup.VerticalOffset = 0;
          }
          break;
        case (Microsoft.Maui.Primitives.LayoutAlignment.Start, Microsoft.Maui.Primitives.LayoutAlignment.Center ):
        case (Microsoft.Maui.Primitives.LayoutAlignment.Fill, Microsoft.Maui.Primitives.LayoutAlignment.Center ):
          {
            platformPopup.HorizontalOffset = 0;
            platformPopup.VerticalOffset = ( windowHeight / 2 ) - ( contentSize.Height / 2 );
          }
          break;
        case (Microsoft.Maui.Primitives.LayoutAlignment.Start, Microsoft.Maui.Primitives.LayoutAlignment.End ):
        case (Microsoft.Maui.Primitives.LayoutAlignment.Fill, Microsoft.Maui.Primitives.LayoutAlignment.End ):
          {
            platformPopup.HorizontalOffset = 0;
            platformPopup.VerticalOffset = windowHeight - contentSize.Height;
          }
          break;
        case (Microsoft.Maui.Primitives.LayoutAlignment.Center, Microsoft.Maui.Primitives.LayoutAlignment.Start ):
        case (Microsoft.Maui.Primitives.LayoutAlignment.Center, Microsoft.Maui.Primitives.LayoutAlignment.Fill ):
          {
            platformPopup.HorizontalOffset = ( windowWidth / 2 ) - ( contentSize.Width / 2 );
            platformPopup.VerticalOffset = 0;
          }
          break;
        case (Microsoft.Maui.Primitives.LayoutAlignment.Center, Microsoft.Maui.Primitives.LayoutAlignment.Center ):
          {
            platformPopup.HorizontalOffset = ( windowWidth / 2 ) - ( contentSize.Width / 2 );
            platformPopup.VerticalOffset = ( windowHeight / 2 ) - ( contentSize.Height / 2 );
          }
          break;
        case (Microsoft.Maui.Primitives.LayoutAlignment.Center, Microsoft.Maui.Primitives.LayoutAlignment.End ):
          {
            platformPopup.HorizontalOffset = ( windowWidth / 2 ) - ( contentSize.Width / 2 );
            platformPopup.VerticalOffset = windowHeight - contentSize.Height;
          }
          break;
        case (Microsoft.Maui.Primitives.LayoutAlignment.End, Microsoft.Maui.Primitives.LayoutAlignment.Start ):
        case (Microsoft.Maui.Primitives.LayoutAlignment.End, Microsoft.Maui.Primitives.LayoutAlignment.Fill ):
          {
            platformPopup.HorizontalOffset = windowWidth - contentSize.Width;
            platformPopup.VerticalOffset = 0;
          }
          break;
        case (Microsoft.Maui.Primitives.LayoutAlignment.End, Microsoft.Maui.Primitives.LayoutAlignment.Center ):
          {
            platformPopup.HorizontalOffset = windowWidth - contentSize.Width;
            platformPopup.VerticalOffset = ( windowHeight / 2 ) - ( contentSize.Height / 2 );
          }
          break;
        case (Microsoft.Maui.Primitives.LayoutAlignment.End, Microsoft.Maui.Primitives.LayoutAlignment.End ):
          {
            platformPopup.HorizontalOffset = windowWidth - contentSize.Width;
            platformPopup.VerticalOffset = windowHeight - contentSize.Height;
          }
          break;
        default:
          {
            platformPopup.HorizontalOffset = 0;
            platformPopup.VerticalOffset = 0;
          }
          break;
      }
    }

    private static void SetSize( PopupHandler handler, IPopup popup )
    {
      if( handler == null )
        throw new ArgumentNullException( "handler" );
      if( popup == null )
        throw new ArgumentNullException( "popup" );
      if( handler.PlatformView == null )
        throw new ArgumentNullException( "handler.PlatformView" );

      if( handler.PlatformView.Child is FrameworkElement content )
      {
        handler.PlatformView.Width = double.NaN;
        handler.PlatformView.Height = double.NaN;
        if( !handler.m_isContentWidthSetByUser )
        {
          content.Width = double.NaN;
          content.MaxWidth = double.PositiveInfinity;
        }
        if( !handler.m_isContentHeightSetByUser )
        {
          content.Height = double.NaN;
          content.MaxHeight = double.PositiveInfinity;
        }
        content.Measure( new Windows.Foundation.Size( double.PositiveInfinity, double.PositiveInfinity ) );


        if( double.IsNaN( content.Width ) && !double.IsNaN( popup.Width ) )
        {
          content.Width = popup.Width;
        }
        if( !double.IsFinite( content.MaxWidth ) && double.IsFinite( popup.MaximumWidth ) )
        {
          content.MaxWidth = popup.MaximumWidth;
        }
        if( double.IsNaN( content.Height ) && !double.IsNaN( popup.Height ) )
        {
          content.Height = popup.Height;
        }
        if( !double.IsFinite( content.MaxHeight ) && double.IsFinite( popup.MaximumHeight ) )
        {
          content.MaxHeight = popup.MaximumHeight;
        }

        var parentWidth = double.NaN;
        var parentHeight = double.NaN;
        var parent = PopupHandler.GetParent( popup );
        if( parent == null )
          throw new InvalidDataException( "parent can't be found for Popup." );

        if( handler.MauiContext == null )
          throw new InvalidDataException( "MauiContext is null." );

        var popupParent = parent.ToPlatform( handler.MauiContext );
        if( popupParent != null )
        {
          parentWidth = popupParent.Width;
          parentHeight = popupParent.Height;
        }
        else if( popup.Window?.Handler.PlatformView is MauiWinUIWindow popupParentWindow )
        {
          parentWidth = popupParentWindow.Bounds.Width;
          parentHeight = popupParentWindow.Bounds.Height;
        }

        if( !double.IsNaN( parentWidth ) )
        {
          content.Width = Math.Min( content.Width, parentWidth );
        }
        if( !double.IsNaN( parentHeight ) )
        {
          content.Height = Math.Min( content.Height, parentHeight );
        }

        if( double.IsNaN( content.Width ) )
        {
          content.Width = content.DesiredSize.Width;
        }
        if( double.IsNaN( content.Height ) )
        {
          content.Height = content.DesiredSize.Height;
        }

        handler.PlatformView.Width = content.Width;
        handler.PlatformView.Height = content.Height;
      }
    }

    #endregion

    #region Event Handlers

    private void PlatformView_Closed( object? sender, object e )
    {
      if( this.VirtualView != null )
      {
        // Make sure the Popup.IsOpen property is closed, event when clicking outside of popup.
        this.VirtualView.IsOpen = false;
      }
    }

    #endregion
  }
}
