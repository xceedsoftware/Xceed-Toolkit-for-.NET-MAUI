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


using System.Drawing;
using System.Runtime.InteropServices;
using CoreGraphics;
using Foundation;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using UIKit;

#nullable enable

namespace Xceed.Maui.Toolkit
{
  partial class PopupHandler : ViewHandler<IPopup, UIKit.UIView>
  {
    #region Private Members

    private UIViewController? m_controller;
    private PopupPresentationDelegate? m_popupPresentationDelegate;

    #endregion

    #region Overrides

    protected override UIKit.UIView CreatePlatformView()
    {
      m_controller = new UIViewController();

      return new UIKit.UIView( new RectangleF( 0, 0, 0, 0 ) );
    }

    protected override void ConnectHandler( UIKit.UIView platformView )
    {
      if( ( this.VirtualView != null ) && ( this.VirtualView.Content != null ) && this.VirtualView.Handler is PopupHandler handler )
      {
        var mauiContext = this.MauiContext;
        if( mauiContext == null )
          throw new NullReferenceException( "mauiContext is null." );

        var platformContent = handler.VirtualView.Content?.ToPlatform( mauiContext );

        if( m_controller != null )
        {
          m_controller.Add( platformContent );
          PopupHandler.SetSize( handler, this.VirtualView );
          //m_controller.View.BackgroundColor = UIColor.Clear;
          //m_controller.ModalPresentationStyle = UIModalPresentationStyle.OverCurrentContext;
          //m_controller.ModalTransitionStyle = UIModalTransitionStyle.CrossDissolve;

          m_controller.ModalPresentationStyle = UIModalPresentationStyle.Popover;
          //m_controller.View.BackgroundColor = UIColor.FromRGBA(0,255,0,0);
          //m_controller.PopoverPresentationController.PopoverBackgroundViewType = typeof(TransparentPopoverBackgroundView);
          //m_controller.PopoverPresentationController.BackgroundColor = UIColor.Red;
          //m_controller.PopoverPresentationController.PermittedArrowDirections = UIPopoverArrowDirection.Left;
          //m_controller.View.BackgroundColor = UIColor.Clear;

          m_popupPresentationDelegate = new PopupPresentationDelegate();
          m_popupPresentationDelegate.DismissEvent += this.PopupDelegate_DismissEvent;
        }
      }

      base.ConnectHandler( platformView );
    }

    protected override void DisconnectHandler( UIKit.UIView platformView )
    {
      if( platformView == null )
        throw new ArgumentNullException( "platformView" );

      platformView.RemoveFromSuperview();

      if( m_popupPresentationDelegate != null )
      {
        m_popupPresentationDelegate.DismissEvent -= this.PopupDelegate_DismissEvent;
      }

      base.DisconnectHandler( platformView );
    }

    #endregion

    #region Public Methods

    public static void MapAnchor( PopupHandler handler, IPopup popup )
    {
      // Done in MapOpen
    }

    public static void MapIsModal( PopupHandler handler, IPopup popup )
    {
      if( popup == null )
        throw new ArgumentNullException( "popup" );

      var controller = handler?.m_controller;
      if( controller != null )
      {
        controller.ModalInPresentation = popup.IsModal;
      }
    }

    public static void MapClose( PopupHandler handler, IPopup popup, object? result )
    {
      if( handler == null )
        throw new ArgumentNullException( "handler" );

      if( handler.m_controller?.PresentationController is UIPopoverPresentationController presentationController )
      {
        presentationController.Delegate = null;
      }

      var currentViewController = WindowStateManager.Default.GetCurrentUIViewController();
      currentViewController?.DismissViewController( false, null );

      // Disconnect handler only if Popup wasn't part of the original xaml and was added with something like: var myPopup = new Popup().
      if( !Popup.IsFromXaml( popup ) )
      {
        handler.DisconnectHandler( handler.PlatformView );
        handler.PlatformView?.Dispose();
      }
    }

    public static void MapOpen( PopupHandler handler, IPopup popup, object? args )
    {
      if( handler == null )
        throw new ArgumentNullException( "handler" );
      if( handler.MauiContext == null )
        throw new ArgumentNullException( "handler.MauiContext" );
      if( popup == null )
        throw new ArgumentNullException( "popup" );

      if( ( handler.m_controller == null ) || ( handler.m_controller.PopoverPresentationController == null ) )
        return;

      PopupHandler.SetSize( handler, popup );
      PopupHandler.SetLayout( handler, popup );

      var currentViewController = WindowStateManager.Default.GetCurrentUIViewController();
      if( (currentViewController != null) && (currentViewController.View != null) )
      {
        var presentationController = handler.m_controller.PresentationController as UIPopoverPresentationController;
        if( presentationController != null )
        {
          presentationController.SourceView = currentViewController.View;
          presentationController.Delegate = handler.m_popupPresentationDelegate;
          // For Mac
          if( popup.Anchor != null )
          {
            presentationController.PassthroughViews = new UIView[] { popup.Anchor.ToPlatform( handler.MauiContext ) };
          }

          // For iOS.
          presentationController.PopoverBackgroundViewType = typeof( TransparentPopoverBackgroundView );
          presentationController.PopoverLayoutMargins = new UIEdgeInsets( 0.0001f, 0.0001f, 0.0001f, 0.0001f );
        }

        //  UIView currentFirstResponder = FindFirstResponder(currentViewController.View);

        currentViewController?.PresentViewController( handler.m_controller, false, null );
      }
    }

    //     private static UIView FindFirstResponder(UIView view)
    // {
    //     if (view.IsFirstResponder)
    //     {
    //         return view;
    //     }

    //     foreach (UIView subview in view.Subviews)
    //     {
    //         var firstResponder = FindFirstResponder(subview);
    //         if (firstResponder != null)
    //         {
    //             return firstResponder;
    //         }
    //     }

    //     return null;
    // }

    public static void MapUpdateSize( PopupHandler handler, IPopup popup, object? result )
    {
      PopupHandler.SetSize( handler, popup );
    }

    #endregion

    #region Private Methods

    private static void SetLayout( PopupHandler handler, IPopup popup )
    {
      if( popup.Anchor != null )
      {
        PopupHandler.SetLayoutFromAnchor( handler, popup );
      }
      else
      {
        // Popup was added from code-behind : var myPopup = new Popup().
        if( !Popup.IsFromXaml( popup ) )
        {
          PopupHandler.SetLayoutWithoutAnchorFromWindow( handler, popup );
        }
        // Popup is part of XAML.
        else
        {
          PopupHandler.SetLayoutWithoutAnchorFromParentXaml( handler, popup );
        }
      }
    }

    private static void SetSize( PopupHandler handler, IPopup popup )
    {
      if( handler == null )
        throw new ArgumentNullException( "handler" );
      if( popup == null )
        throw new ArgumentNullException( "popup" );

      if( (popup.Content == null) || (handler.m_controller == null) )
        return;

      var mauiContext = handler.MauiContext;
      if( mauiContext == null )
        throw new NullReferenceException( "mauiContext is null." );

      var platformContent = popup.Content.ToPlatform( mauiContext );
      if( platformContent == null )
        return;

      var wantedWidth = -1d;
      var wantedHeight = -1d;
      if( !double.IsNaN( popup.Content.Width ) )
      {
        wantedWidth = popup.Content.Width;
      }
      if( !double.IsNaN( popup.Content.Height ) )
      {
        wantedHeight = popup.Content.Height;
      }

      if( ( wantedWidth == -1 ) && !double.IsNaN( popup.Width ) )
      {
        wantedWidth = popup.Width;
      }
      if( ( wantedHeight == -1 ) && !double.IsNaN( popup.Height ) )
      {
        wantedHeight = popup.Height;
      }

      if( ( wantedWidth == -1 ) || ( wantedHeight == -1 ) )
      {
        //var platformWindow = popup.Window?.ToPlatform( mauiContext );
        //var frameBounds = ( platformWindow != null ) ? platformWindow.Bounds : UIScreen.MainScreen.Bounds;

        //var contentWidth = platformContent.Frame.Width;
        //var contentHeight = platformContent.Frame.Height;

        var popupContentMeasuredSize = popup.Content.Measure( double.PositiveInfinity, double.PositiveInfinity );
        //var wantedSize = platformContent.SizeThatFits(new CGSize(contentWidth != 0 ? contentWidth : frameBounds.Width, contentHeight != 0 ? contentHeight : frameBounds.Height));
        wantedWidth = popupContentMeasuredSize.Width;
        wantedHeight = popupContentMeasuredSize.Height;
      }

      if( double.IsFinite( popup.MaximumWidth ) )
      {
        wantedWidth = Math.Min( wantedWidth, popup.MaximumWidth );
      }
      if( double.IsFinite( popup.MaximumHeight ) )
      {
        wantedHeight = Math.Min( wantedHeight, popup.MaximumHeight );
      }

      //var parentWidth = double.NaN;
      //var parentHeight = double.NaN;
      //var parent = popup.Parent is PopupContainer ? popup.Parent.Parent : popup.Parent;
      //var popupParent = parent?.ToPlatform(handler.MauiContext);
      //if (popupParent != null)
      //{
      //    parentWidth = popupParent.Bounds.Width;
      //    parentHeight = popupParent.Bounds.Height;
      //}
      //else
      //{
      //    var screenSize = UIScreen.MainScreen.Bounds;
      //    parentWidth = screenSize.Width;
      //    parentHeight = screenSize.Height;
      //}

      //if (!double.IsNaN(parentWidth))
      //{
      //    wantedWidth = (int)Math.Min(wantedWidth, parentWidth);
      //}
      //if (!double.IsNaN(parentHeight))
      //{
      //    wantedHeight = (int)Math.Min(wantedHeight, parentHeight);
      //}

      handler.m_controller.PreferredContentSize = new CGSize( wantedWidth, wantedHeight );
      platformContent.Frame = new CGRect( CGPoint.Empty, handler.m_controller.PreferredContentSize );
    }

    private static void SetLayoutFromAnchor( PopupHandler handler, IPopup popup )
    {
      if( handler == null )
        throw new ArgumentNullException( "handler" );
      if( popup == null )
        throw new ArgumentNullException( "popup" );

      if( popup.Anchor == null )
        return;

      var presentationController = handler.m_controller?.PresentationController as UIPopoverPresentationController;
      if( presentationController != null )
      {
        var mauiContext = handler.MauiContext;
        if( mauiContext == null )
          throw new NullReferenceException( "mauiContext is null." );

        var platformAnchor = popup.Anchor.ToPlatform( mauiContext );
        var platformAnchorBounds = platformAnchor.Bounds;
        var popupSize = handler.m_controller?.PreferredContentSize;
        if( popupSize == null )
          return;

        NFloat positionX = 0f;
        NFloat positionY = 0f;
        var direction = UIPopoverArrowDirection.Up;
        switch(popup.HorizontalLayoutAlignment, popup.VerticalLayoutAlignment)
        {
          case (Microsoft.Maui.Primitives.LayoutAlignment.Start, Microsoft.Maui.Primitives.LayoutAlignment.Start ):
          case (Microsoft.Maui.Primitives.LayoutAlignment.Start, Microsoft.Maui.Primitives.LayoutAlignment.Fill ):
          case (Microsoft.Maui.Primitives.LayoutAlignment.Fill, Microsoft.Maui.Primitives.LayoutAlignment.Start ):
          case (Microsoft.Maui.Primitives.LayoutAlignment.Fill, Microsoft.Maui.Primitives.LayoutAlignment.Fill ):
            {
              direction = UIPopoverArrowDirection.Down;
            }
            break;
          case (Microsoft.Maui.Primitives.LayoutAlignment.Center, Microsoft.Maui.Primitives.LayoutAlignment.Start ):
          case (Microsoft.Maui.Primitives.LayoutAlignment.Center, Microsoft.Maui.Primitives.LayoutAlignment.Fill ):
            {
              positionX = ( platformAnchorBounds.Width / 2 ) - ( popupSize.Value.Width / 2 );
              direction = UIPopoverArrowDirection.Down;
            }
            break;
          case (Microsoft.Maui.Primitives.LayoutAlignment.End, Microsoft.Maui.Primitives.LayoutAlignment.Start ):
          case (Microsoft.Maui.Primitives.LayoutAlignment.End, Microsoft.Maui.Primitives.LayoutAlignment.Fill ):
            {
              positionX = platformAnchorBounds.Width - popupSize.Value.Width;
              direction = UIPopoverArrowDirection.Down;
            }
            break;
          case (Microsoft.Maui.Primitives.LayoutAlignment.Start, Microsoft.Maui.Primitives.LayoutAlignment.Center ):
          case (Microsoft.Maui.Primitives.LayoutAlignment.Fill, Microsoft.Maui.Primitives.LayoutAlignment.Center ):
            {
              positionY = ( platformAnchorBounds.Height / 2 ) - ( popupSize.Value.Height / 2 );
              direction = UIPopoverArrowDirection.Right;
            }
            break;
          case (Microsoft.Maui.Primitives.LayoutAlignment.End, Microsoft.Maui.Primitives.LayoutAlignment.Center ):
            {
              positionX = platformAnchorBounds.Width;
              positionY = ( platformAnchorBounds.Height / 2 ) - ( popupSize.Value.Height / 2 );
              direction = UIPopoverArrowDirection.Left;
            }
            break;
          case (Microsoft.Maui.Primitives.LayoutAlignment.Start, Microsoft.Maui.Primitives.LayoutAlignment.End ):
          case (Microsoft.Maui.Primitives.LayoutAlignment.Fill, Microsoft.Maui.Primitives.LayoutAlignment.End ):
            {
              positionY = platformAnchorBounds.Height;
            }
            break;
          case (Microsoft.Maui.Primitives.LayoutAlignment.Center, Microsoft.Maui.Primitives.LayoutAlignment.End ):
            {
              positionX = ( platformAnchorBounds.Width / 2 ) - ( popupSize.Value.Width / 2 );
              positionY = platformAnchorBounds.Height;
            }
            break;
          case (Microsoft.Maui.Primitives.LayoutAlignment.End, Microsoft.Maui.Primitives.LayoutAlignment.End ):
            {
              positionX = platformAnchorBounds.Width - popupSize.Value.Width;
              positionY = platformAnchorBounds.Height;
            }
            break;
          default:
            {
              positionX = ( platformAnchorBounds.Width / 2 ) - ( popupSize.Value.Width / 2 );
              positionY = platformAnchorBounds.Height;
            }
            break;

        }

#pragma warning disable CA1416
        presentationController.SourceItem = platformAnchor;
#pragma warning restore CA1416

        var isTopBottom = ( direction == UIPopoverArrowDirection.Down ) || ( direction == UIPopoverArrowDirection.Up );
        presentationController.SourceRect = new CGRect( positionX,
                                                       positionY,
                                                       isTopBottom ? popupSize.Value.Width : 1,
                                                       !isTopBottom ? popupSize.Value.Height : 1 );
        presentationController.PermittedArrowDirections = direction;
      }
    }

    private static void SetLayoutWithoutAnchorFromWindow( PopupHandler handler, IPopup popup )
    {
      if( handler == null )
        throw new ArgumentNullException( "handler" );
      if( popup == null )
        throw new ArgumentNullException( "popup" );

      var presentationController = handler.m_controller?.PresentationController as UIPopoverPresentationController;
      if( presentationController != null )
      {
        var mauiContext = handler.MauiContext;
        if( mauiContext == null )
          throw new NullReferenceException( "mauiContext is null." );

        var platformWindow = popup.Window?.ToPlatform( mauiContext );
        var frameBounds = ( platformWindow != null ) ? platformWindow.Bounds : UIScreen.MainScreen.Bounds;

        var popupSize = handler.m_controller?.PreferredContentSize;
        if( popupSize == null )
          return;

        NFloat positionX = 0f;
        NFloat positionY = 0f;
#if !IOS
        var direction = UIPopoverArrowDirection.Up;
#endif
        switch(popup.HorizontalLayoutAlignment, popup.VerticalLayoutAlignment)
        {
          case (Microsoft.Maui.Primitives.LayoutAlignment.Start, Microsoft.Maui.Primitives.LayoutAlignment.Start ):
          case (Microsoft.Maui.Primitives.LayoutAlignment.Start, Microsoft.Maui.Primitives.LayoutAlignment.Fill ):
          case (Microsoft.Maui.Primitives.LayoutAlignment.Fill, Microsoft.Maui.Primitives.LayoutAlignment.Start ):
          case (Microsoft.Maui.Primitives.LayoutAlignment.Fill, Microsoft.Maui.Primitives.LayoutAlignment.Fill ):
            break;
          case (Microsoft.Maui.Primitives.LayoutAlignment.Center, Microsoft.Maui.Primitives.LayoutAlignment.Start ):
          case (Microsoft.Maui.Primitives.LayoutAlignment.Center, Microsoft.Maui.Primitives.LayoutAlignment.Fill ):
            {
              positionX = ( frameBounds.Width / 2 ) - ( popupSize.Value.Width / 2 );
            }
            break;
          case (Microsoft.Maui.Primitives.LayoutAlignment.End, Microsoft.Maui.Primitives.LayoutAlignment.Start ):
          case (Microsoft.Maui.Primitives.LayoutAlignment.End, Microsoft.Maui.Primitives.LayoutAlignment.Fill ):
            {
              positionX = frameBounds.Width - popupSize.Value.Width;
            }
            break;
          case (Microsoft.Maui.Primitives.LayoutAlignment.Start, Microsoft.Maui.Primitives.LayoutAlignment.Center ):
          case (Microsoft.Maui.Primitives.LayoutAlignment.Fill, Microsoft.Maui.Primitives.LayoutAlignment.Center ):
            {
              positionY = ( frameBounds.Height / 2 ) - ( popupSize.Value.Height / 2 );
            }
            break;
          case (Microsoft.Maui.Primitives.LayoutAlignment.Center, Microsoft.Maui.Primitives.LayoutAlignment.Center ):
            {
              positionX = ( frameBounds.Width / 2 ) - ( popupSize.Value.Width / 2 );
              positionY = ( frameBounds.Height / 2 ) - ( popupSize.Value.Height / 2 );
            }
            break;
          case (Microsoft.Maui.Primitives.LayoutAlignment.End, Microsoft.Maui.Primitives.LayoutAlignment.Center ):
            {
              positionX = frameBounds.Width - popupSize.Value.Width;
              positionY = ( frameBounds.Height / 2 ) - ( popupSize.Value.Height / 2 );
#if !IOS
              direction = UIPopoverArrowDirection.Left;
#endif
            }
            break;
          case (Microsoft.Maui.Primitives.LayoutAlignment.Start, Microsoft.Maui.Primitives.LayoutAlignment.End ):
          case (Microsoft.Maui.Primitives.LayoutAlignment.Fill, Microsoft.Maui.Primitives.LayoutAlignment.End ):
            {
              positionY = frameBounds.Height - popupSize.Value.Height;
            }
            break;
          case (Microsoft.Maui.Primitives.LayoutAlignment.Center, Microsoft.Maui.Primitives.LayoutAlignment.End ):
            {
              positionX = ( frameBounds.Width / 2 ) - ( popupSize.Value.Width / 2 );
              positionY = frameBounds.Height - popupSize.Value.Height;
            }
            break;
          case (Microsoft.Maui.Primitives.LayoutAlignment.End, Microsoft.Maui.Primitives.LayoutAlignment.End ):
            {
              positionX = frameBounds.Width - popupSize.Value.Width;
              positionY = frameBounds.Height - popupSize.Value.Height;
            }
            break;
          default:
            {
              positionX = ( frameBounds.Width / 2 ) - ( popupSize.Value.Width / 2 );
              positionY = ( frameBounds.Height / 2 ) - ( popupSize.Value.Height / 2 );
            }
            break;

        }
#if IOS
                presentationController.SourceRect = new CGRect(positionX, positionY, popupSize.Value.Width, popupSize.Value.Height);
                presentationController.PermittedArrowDirections = 0;
#else
        presentationController.SourceRect = new CGRect( positionX, positionY, popupSize.Value.Width, 1 );
        presentationController.PermittedArrowDirections = direction;
#endif
      }
    }

    private static void SetLayoutWithoutAnchorFromParentXaml( PopupHandler handler, IPopup popup )
    {
      if( handler == null )
        throw new ArgumentNullException( "handler" );
      if( popup == null )
        throw new ArgumentNullException( "popup" );

      var presentationController = handler.m_controller?.PresentationController as UIPopoverPresentationController;
      if( presentationController != null )
      {
        var mauiContext = handler.MauiContext;
        if( mauiContext == null )
          throw new NullReferenceException( "mauiContext is null." );

        var parent = PopupHandler.GetParent( popup ) as VisualElement;
        if( parent == null )
          throw new InvalidDataException( "parent must be a VisualElement" );

        var parentView = parent.ToPlatform( mauiContext );
        if( parentView != null )
        {
          var popupSize = handler.m_controller?.PreferredContentSize;
          if( popupSize == null )
            return;

          var parentPosition = parentView.ConvertPointToView( new CGPoint( 0, 0 ), null );

          NFloat parentPosX = parentPosition.X;
          NFloat parentPosY = parentPosition.Y;
          NFloat parentWidth = parentView.Frame.Width;
          NFloat parentHeight = parentView.Frame.Height;
          var direction = UIPopoverArrowDirection.Up;

          NFloat offsetX = 0;
          NFloat offsetY = 0;

          switch(popup.HorizontalLayoutAlignment, popup.VerticalLayoutAlignment)
          {
            case (Microsoft.Maui.Primitives.LayoutAlignment.Center, Microsoft.Maui.Primitives.LayoutAlignment.Start ):
            case (Microsoft.Maui.Primitives.LayoutAlignment.Center, Microsoft.Maui.Primitives.LayoutAlignment.Fill ):
              {
                offsetX = ( parentWidth / 2 ) - ( popupSize.Value.Width / 2 );
              }
              break;
            case (Microsoft.Maui.Primitives.LayoutAlignment.End, Microsoft.Maui.Primitives.LayoutAlignment.Start ):
            case (Microsoft.Maui.Primitives.LayoutAlignment.End, Microsoft.Maui.Primitives.LayoutAlignment.Fill ):
              {
                offsetX = parentWidth - popupSize.Value.Width;
              }
              break;
            case (Microsoft.Maui.Primitives.LayoutAlignment.Start, Microsoft.Maui.Primitives.LayoutAlignment.Center ):
            case (Microsoft.Maui.Primitives.LayoutAlignment.Fill, Microsoft.Maui.Primitives.LayoutAlignment.Center ):
              {
                offsetY = ( parentHeight / 2 ) - ( popupSize.Value.Height / 2 );
              }
              break;
            case (Microsoft.Maui.Primitives.LayoutAlignment.Center, Microsoft.Maui.Primitives.LayoutAlignment.Center ):
              {
                offsetX = ( parentWidth / 2 ) - ( popupSize.Value.Width / 2 );
                offsetY = ( parentHeight / 2 ) - ( popupSize.Value.Height / 2 );

              }
              break;
            case (Microsoft.Maui.Primitives.LayoutAlignment.End, Microsoft.Maui.Primitives.LayoutAlignment.Center ):
              {
                offsetX = parentWidth - popupSize.Value.Width;
                offsetY = ( parentHeight / 2 ) - ( popupSize.Value.Height / 2 );
              }
              break;
            case (Microsoft.Maui.Primitives.LayoutAlignment.Start, Microsoft.Maui.Primitives.LayoutAlignment.End ):
            case (Microsoft.Maui.Primitives.LayoutAlignment.Fill, Microsoft.Maui.Primitives.LayoutAlignment.End ):
              {
                offsetY = parentHeight - popupSize.Value.Height;
              }
              break;
            case (Microsoft.Maui.Primitives.LayoutAlignment.Center, Microsoft.Maui.Primitives.LayoutAlignment.End ):
              {
                offsetX = ( parentWidth / 2 ) - ( popupSize.Value.Width / 2 );
                offsetY = parentHeight - popupSize.Value.Height;
              }
              break;
            case (Microsoft.Maui.Primitives.LayoutAlignment.End, Microsoft.Maui.Primitives.LayoutAlignment.End ):
              {
                offsetX = parentWidth - popupSize.Value.Width;
                offsetY = parentHeight - popupSize.Value.Height;
              }
              break;
          }

          var isTopBottom = ( direction == UIPopoverArrowDirection.Down ) || ( direction == UIPopoverArrowDirection.Up );
          presentationController.SourceRect = new CGRect( parentPosX + offsetX,
                                                         parentPosY + offsetY,
                                                         isTopBottom ? popupSize.Value.Width : 1,
                                                         !isTopBottom ? popupSize.Value.Height : 1 );
          presentationController.PermittedArrowDirections = direction;
        }
      }
    }

#endregion

    #region Event Handlers

    private void PopupDelegate_DismissEvent( object? sender, UIPresentationController e )
    {
      if( this.VirtualView != null )
      {
        // Make sure the Popup.IsOpen property is closed, event when clicking outside of popup.
        this.VirtualView.IsOpen = false;
      }
    }

    #endregion
  }

  public class PopupPresentationDelegate : UIPopoverPresentationControllerDelegate
  {
    readonly WeakEventManager dismissEventManager = new WeakEventManager();

    public event EventHandler<UIPresentationController> DismissEvent
    {
      add => dismissEventManager.AddEventHandler( value );
      remove => dismissEventManager.RemoveEventHandler( value );
    }

    //public override UIModalPresentationStyle GetAdaptivePresentationStyle(UIPresentationController forPresentationController)
    //{
    //    return UIModalPresentationStyle.None;
    //}

    public override void DidDismiss( UIPresentationController presentationController )
    {
      dismissEventManager.HandleEvent( this, presentationController, nameof( DismissEvent ) );
    }
  }

  // For iOS !!
  class TransparentPopoverBackgroundView : UIPopoverBackgroundView
  {
    public TransparentPopoverBackgroundView( ObjCRuntime.NativeHandle handle ) : base( handle )
    {
      BackgroundColor = UIColor.Clear;// Colors.Transparent.ToPlatform();
      Alpha = 0.0f;
    }

    public override NFloat ArrowOffset { get; set; }

    public override UIPopoverArrowDirection ArrowDirection { get; set; }

    [Export( "arrowHeight" )]
    static new float GetArrowHeight()
    {
      return 0f;
    }

    [Export( "arrowBase" )]
    static new float GetArrowBase()
    {
      return 0f;
    }

    [Export( "contentViewInsets" )]
    static new UIEdgeInsets GetContentViewInsets()
    {
      return UIEdgeInsets.Zero;
    }
  }
}
