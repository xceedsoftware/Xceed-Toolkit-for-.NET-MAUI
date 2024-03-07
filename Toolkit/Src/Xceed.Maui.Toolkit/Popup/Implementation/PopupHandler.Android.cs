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


using Android.App;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Util;
using Android.Views;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;

#nullable enable

namespace Xceed.Maui.Toolkit
{
  public partial class PopupHandler : ViewHandler<IPopup, Android.Views.View>
  {
    #region Override Methods

    protected override Android.Views.View CreatePlatformView()
    {
      var mauiContext = this.MauiContext;
      if( mauiContext == null )
        throw new InvalidOperationException( "MauiContext is null" );

      var context = mauiContext.Context;
      if( context == null )
        throw new InvalidOperationException( "Android Context is null" );

      return new Android.Views.View( context )
      {
        Visibility = ViewStates.Invisible,
        Tag = new Dialog( context )
      };
    }

    protected override void ConnectHandler( Android.Views.View platformView )
    {
      if( platformView == null )
        throw new ArgumentNullException( "platformView" );

      if( ( this.VirtualView != null ) && ( this.VirtualView.Content != null ) && this.VirtualView.Handler is PopupHandler handler )
      {
        var mauiContext = this.MauiContext;
        if( mauiContext == null )
          throw new NullReferenceException( "mauiContext is null." );

        var platformContent = handler.VirtualView.Content?.ToPlatform( mauiContext );
        if( platformContent != null )
        {
          var dialog = platformView.Tag as Dialog;
          if( dialog != null )
          {
            dialog.SetContentView( platformContent );
            if( (dialog.Window != null) && this.VirtualView is Popup popup && !popup.Focusable )
            {
              dialog.Window.SetFlags( WindowManagerFlags.NotFocusable, WindowManagerFlags.NotFocusable );
            }
            dialog.DismissEvent += this.PlatformView_Closed;
          }
        }
      }

      base.ConnectHandler( platformView );
    }

    protected override void DisconnectHandler( Android.Views.View platformView )
    {
      if( platformView == null )
        throw new ArgumentNullException( "platformView" );

      var dialog = platformView.Tag as Dialog;
      if( dialog != null )
      {
        dialog.DismissEvent -= this.PlatformView_Closed;
      }

      var layoutviewGroup = platformView?.Parent as LayoutViewGroup;
      if( layoutviewGroup != null )
      {
        layoutviewGroup.RemoveView( platformView );
      }

      base.DisconnectHandler( platformView );

      platformView.Dispose();
    }

    #endregion

    #region Public Methods

    public static void MapAnchor( PopupHandler handler, IPopup popup )
    {
      // Done in MapOpen().
    }

    public static void MapIsModal( PopupHandler handler, IPopup popup )
    {
      if( popup == null )
        throw new ArgumentNullException( "popup" );

      var dialog = handler?.PlatformView?.Tag as Dialog;
      if( dialog != null )
      {
        dialog.SetCanceledOnTouchOutside( !popup.IsModal );
      }
    }

    public static void MapClose( PopupHandler handler, IPopup popup, object? result )
    {
      if( handler == null )
        throw new ArgumentNullException( "handler" );

      var platformView = handler.PlatformView;
      if( platformView == null )
        throw new ArgumentNullException( "platformView" );

      var dialog = platformView.Tag as Dialog;
      dialog?.Dismiss();

      // Disconnect handler only if Popup wasn't part of the original xaml and was added with something like: var myPopup = new Popup().
      if( !Popup.IsFromXaml( popup ) )
      {
        handler.DisconnectHandler( handler.PlatformView );
      }
    }

    public static void MapOpen( PopupHandler handler, IPopup popup, object? args )
    {
      if( handler == null )
        throw new ArgumentNullException( "handler" );

      var platformView = handler.PlatformView;
      if( platformView == null )
        throw new ArgumentNullException( "platformView" );

      var dialog = platformView.Tag as Dialog;

      dialog?.Window?.SetBackgroundDrawable( new ColorDrawable( Android.Graphics.Color.Transparent ) );
      PopupHandler.SetSize( handler, popup );
      PopupHandler.SetLayout( handler, popup );

      dialog?.Show();
    }

    public static void MapUpdateSize( PopupHandler handler, IPopup popup, object? result )
    {
      PopupHandler.SetSize( handler, popup );
    }

    #endregion

    #region Private Methods

    private static int GetAndroidNavigationBarHeight()
    {
      var androidContext = Android.App.Application.Context;
      var androidResources = androidContext?.Resources;
      var resourceId = androidResources?.GetIdentifier( "design_bottom_navigation_height", "dimen", androidContext?.PackageName );
      return ( ( androidResources != null ) && ( resourceId > 0 ) ) ? androidResources.GetDimensionPixelSize( (int)resourceId ) : 0;
    }

    private static int GetAndroidStatusBarHeight( Android.Views.Window window )
    {
      var defaultDisplay = window?.WindowManager?.DefaultDisplay;
      if( defaultDisplay == null )
        throw new InvalidDataException( "defaultDisplay must not be null." );

      var displayMetrics = new DisplayMetrics();
      defaultDisplay.GetMetrics( displayMetrics );
      var realDisplayMetrics = new DisplayMetrics();
      defaultDisplay.GetRealMetrics( realDisplayMetrics );
      return realDisplayMetrics.HeightPixels - displayMetrics.HeightPixels;
    }

    private static void SetSize( PopupHandler handler, IPopup popup )
    {
      if( handler == null )
        throw new ArgumentNullException( "handler" );
      if( popup == null )
        throw new ArgumentNullException( "popup" );

      if( popup.Content == null )
        return;

      var platformView = handler.PlatformView;
      if( platformView == null )
        throw new ArgumentNullException( "platformView" );

      var dialog = platformView.Tag as Dialog;
      if( dialog == null )
        return;

      var window = dialog.Window;
      if( window == null )
        return;

      var wantedWidth = -1;
      var wantedHeight = -1;

      if( !double.IsNaN( popup.Content.Width ) )
      {
        wantedWidth = (int)dialog.Context.ToPixels( popup.Content.Width );
      }
      if( !double.IsNaN( popup.Content.Height ) )
      {
        wantedHeight = (int)dialog.Context.ToPixels( popup.Content.Height );
      }

      if( ( wantedWidth == -1 ) && !double.IsNaN( popup.Width ) )
      {
        wantedWidth = (int)dialog.Context.ToPixels( popup.Width );
      }
      if( ( wantedHeight == -1 ) && !double.IsNaN( popup.Height ) )
      {
        wantedHeight = (int)dialog.Context.ToPixels( popup.Height );
      }

      if( ( wantedWidth == -1 ) || ( wantedHeight == -1 ) )
      {
        var popupContentMeasuredSize = popup.Content.Measure( double.PositiveInfinity, double.PositiveInfinity );
        wantedWidth = ( wantedWidth == -1 ) ? (int)dialog.Context.ToPixels( popupContentMeasuredSize.Width ) : wantedWidth;
        wantedHeight = ( wantedHeight == -1 ) ? (int)dialog.Context.ToPixels( popupContentMeasuredSize.Height ) : wantedHeight;
      }

      if( double.IsFinite( popup.MaximumWidth ) )
      {
        wantedWidth = Math.Min( wantedWidth, (int)dialog.Context.ToPixels( popup.MaximumWidth ) );
      }
      if( double.IsFinite( popup.MaximumHeight ) )
      {
        wantedHeight = Math.Min( wantedHeight, (int)dialog.Context.ToPixels( popup.MaximumHeight ) );
      }

      var parentWidth = double.NaN;
      var parentHeight = double.NaN;
      var parent = PopupHandler.GetParent( popup );
      if( parent is Android.Views.View popupParent )
      {
        parentWidth = (int)dialog.Context.ToPixels( popupParent.Width );
        parentHeight = (int)dialog.Context.ToPixels( popupParent.Height );
      }
      else
      {
        if( OperatingSystem.IsAndroidVersionAtLeast( 30 ) )
        {
          var windowMetrics = window.WindowManager?.CurrentWindowMetrics;
          if( windowMetrics != null )
          {
            parentWidth = windowMetrics.Bounds.Width();
            parentHeight = windowMetrics.Bounds.Height();
          }
        }
      }

      if( !double.IsNaN( parentWidth ) )
      {
        wantedWidth = (int)Math.Min( wantedWidth, parentWidth );
      }
      if( !double.IsNaN( parentHeight ) )
      {
        wantedHeight = (int)Math.Min( wantedHeight, parentHeight );
      }

      window.SetLayout( wantedWidth, wantedHeight );
    }

    private static void SetLayout( PopupHandler handler, IPopup popup )
    {
      if( popup == null )
        throw new ArgumentNullException( "popup" );

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

    // This method will position the popup around an anchor defined in XAML, based on Alignment.
    private static void SetLayoutFromAnchor( PopupHandler handler, IPopup popup )
    {
      if( handler == null )
        throw new ArgumentNullException( "handler" );
      if( popup == null )
        throw new ArgumentNullException( "popup" );

      var visualAnchor = popup?.Anchor as Microsoft.Maui.Controls.VisualElement;
      if( visualAnchor == null )
        throw new InvalidDataException( "Anchor must be a VisualElement" );

      var dialog = handler.PlatformView?.Tag as Dialog;
      if( dialog == null )
        return;

      var window = dialog.Window;
      if( window == null )
        return;

      window.SetGravity( GravityFlags.Top | GravityFlags.Left );

      var windowAttributes = window.Attributes;
      if( windowAttributes == null )
        throw new InvalidDataException( "dialog.Window.Attributes must not be null." );

      var defaultDisplay = window.WindowManager?.DefaultDisplay;
      if( defaultDisplay == null )
        throw new InvalidDataException( "defaultDisplay must not be null." );

      var anchorPosX = 0;
      var anchorPosY = 0;
      // if Popup was part of the original xaml and was not added with something like: var myPopup = new Popup().
      if( Popup.IsFromXaml( popup ) )
      {
        var locationOnScreen = new int[ 2 ];
        visualAnchor.ToPlatform( handler.MauiContext ).GetLocationOnScreen( locationOnScreen );

        anchorPosX = locationOnScreen[ 0 ];
        anchorPosY = locationOnScreen[ 1 ] - PopupHandler.GetAndroidStatusBarHeight( window );
      }
      else
      {
        anchorPosX += (int)visualAnchor.Bounds.X;
        anchorPosY += (int)visualAnchor.Bounds.Y;
        var parent = visualAnchor as Element;
        while( parent.Parent != null )
        {
          if( parent.Parent is Microsoft.Maui.Controls.VisualElement parentVisualElement )
          {
            anchorPosX += (int)parentVisualElement.Bounds.X;
            anchorPosY += (int)parentVisualElement.Bounds.Y;
          }
          parent = parent.Parent;
        }
        anchorPosX = (int)dialog.Context.ToPixels( anchorPosX );
        anchorPosY = (int)dialog.Context.ToPixels( anchorPosY ) + PopupHandler.GetAndroidNavigationBarHeight();
      }

      var visualAnchorPosX = anchorPosX;
      var visualAnchorPosY = anchorPosY;
      var visualAnchorWidth = (int)dialog.Context.ToPixels( visualAnchor.Bounds.Width );
      var visualAnchorHeight = (int)dialog.Context.ToPixels( visualAnchor.Bounds.Height );

      var offsetX = 0;
      var offsetY = 0;
      switch(popup.HorizontalLayoutAlignment, popup.VerticalLayoutAlignment)
      {
        case (Microsoft.Maui.Primitives.LayoutAlignment.Start, Microsoft.Maui.Primitives.LayoutAlignment.Start ):
        case (Microsoft.Maui.Primitives.LayoutAlignment.Start, Microsoft.Maui.Primitives.LayoutAlignment.Fill ):
        case (Microsoft.Maui.Primitives.LayoutAlignment.Fill, Microsoft.Maui.Primitives.LayoutAlignment.Start ):
        case (Microsoft.Maui.Primitives.LayoutAlignment.Fill, Microsoft.Maui.Primitives.LayoutAlignment.Fill ):
          {
            offsetY = -windowAttributes.Height;
          }
          break;
        case (Microsoft.Maui.Primitives.LayoutAlignment.Start, Microsoft.Maui.Primitives.LayoutAlignment.Center ):
        case (Microsoft.Maui.Primitives.LayoutAlignment.Fill, Microsoft.Maui.Primitives.LayoutAlignment.Center ):
          {
            offsetX = -windowAttributes.Width;
            offsetY = ( visualAnchorHeight / 2 ) - ( windowAttributes.Height / 2 );
          }
          break;
        case (Microsoft.Maui.Primitives.LayoutAlignment.Start, Microsoft.Maui.Primitives.LayoutAlignment.End ):
        case (Microsoft.Maui.Primitives.LayoutAlignment.Fill, Microsoft.Maui.Primitives.LayoutAlignment.End ):
          {
            offsetY = visualAnchorHeight;
          }
          break;
        case (Microsoft.Maui.Primitives.LayoutAlignment.Center, Microsoft.Maui.Primitives.LayoutAlignment.Start ):
        case (Microsoft.Maui.Primitives.LayoutAlignment.Center, Microsoft.Maui.Primitives.LayoutAlignment.Fill ):
          {
            offsetX = ( visualAnchorWidth / 2 ) - ( windowAttributes.Width / 2 );
            offsetY = -windowAttributes.Height;
          }
          break;
        case (Microsoft.Maui.Primitives.LayoutAlignment.Center, Microsoft.Maui.Primitives.LayoutAlignment.End ):
          {
            offsetX = ( visualAnchorWidth / 2 ) - ( windowAttributes.Width / 2 );
            offsetY = visualAnchorHeight;
          }
          break;
        case (Microsoft.Maui.Primitives.LayoutAlignment.End, Microsoft.Maui.Primitives.LayoutAlignment.Start ):
        case (Microsoft.Maui.Primitives.LayoutAlignment.End, Microsoft.Maui.Primitives.LayoutAlignment.Fill ):
          {
            offsetX = visualAnchorWidth - windowAttributes.Width;
            offsetY = -windowAttributes.Height;
          }
          break;
        case (Microsoft.Maui.Primitives.LayoutAlignment.End, Microsoft.Maui.Primitives.LayoutAlignment.Center ):
          {
            offsetX = visualAnchorWidth;
            offsetY = ( visualAnchorHeight / 2 ) - ( windowAttributes.Height / 2 );
          }
          break;
        case (Microsoft.Maui.Primitives.LayoutAlignment.End, Microsoft.Maui.Primitives.LayoutAlignment.End ):
          {
            offsetX = visualAnchorWidth - windowAttributes.Width;
            offsetY = visualAnchorHeight;
          }
          break;
        default:
          {
            offsetX = ( visualAnchorWidth / 2 ) - ( windowAttributes.Width / 2 );
            offsetY = visualAnchorHeight;
          }
          break;
      }

      windowAttributes.X = visualAnchorPosX + offsetX;
      windowAttributes.Y = visualAnchorPosY + offsetY;
    }

    // This method will position the popup inside the Application, based on Alignment.
    private static void SetLayoutWithoutAnchorFromWindow( PopupHandler handler, IPopup popup )
    {
      if( handler == null )
        throw new ArgumentNullException( "handler" );
      if( popup == null )
        throw new ArgumentNullException( "popup" );

      var dialog = handler.PlatformView?.Tag as Dialog;
      if( dialog == null )
        return;

      var window = dialog.Window;
      if( window == null )
        return;

      var gravityFlags = GravityFlags.Left | GravityFlags.Top;

      switch(popup.HorizontalLayoutAlignment, popup.VerticalLayoutAlignment)
      {
        case (Microsoft.Maui.Primitives.LayoutAlignment.Center, Microsoft.Maui.Primitives.LayoutAlignment.Start ):
        case (Microsoft.Maui.Primitives.LayoutAlignment.Center, Microsoft.Maui.Primitives.LayoutAlignment.Fill ):
          gravityFlags = GravityFlags.CenterHorizontal | GravityFlags.Top;
          break;
        case (Microsoft.Maui.Primitives.LayoutAlignment.End, Microsoft.Maui.Primitives.LayoutAlignment.Start ):
        case (Microsoft.Maui.Primitives.LayoutAlignment.End, Microsoft.Maui.Primitives.LayoutAlignment.Fill ):
          gravityFlags = GravityFlags.Right | GravityFlags.Top;
          break;
        case (Microsoft.Maui.Primitives.LayoutAlignment.Start, Microsoft.Maui.Primitives.LayoutAlignment.Center ):
        case (Microsoft.Maui.Primitives.LayoutAlignment.Fill, Microsoft.Maui.Primitives.LayoutAlignment.Center ):
          gravityFlags = GravityFlags.Left | GravityFlags.CenterVertical;
          break;
        case (Microsoft.Maui.Primitives.LayoutAlignment.Center, Microsoft.Maui.Primitives.LayoutAlignment.Center ):
          gravityFlags = GravityFlags.CenterHorizontal | GravityFlags.CenterVertical;
          break;
        case (Microsoft.Maui.Primitives.LayoutAlignment.End, Microsoft.Maui.Primitives.LayoutAlignment.Center ):
          gravityFlags = GravityFlags.Right | GravityFlags.CenterVertical;
          break;
        case (Microsoft.Maui.Primitives.LayoutAlignment.Start, Microsoft.Maui.Primitives.LayoutAlignment.End ):
        case (Microsoft.Maui.Primitives.LayoutAlignment.Fill, Microsoft.Maui.Primitives.LayoutAlignment.End ):
          gravityFlags = GravityFlags.Left | GravityFlags.Bottom;
          break;
        case (Microsoft.Maui.Primitives.LayoutAlignment.Center, Microsoft.Maui.Primitives.LayoutAlignment.End ):
          gravityFlags = GravityFlags.CenterHorizontal | GravityFlags.Bottom;
          break;
        case (Microsoft.Maui.Primitives.LayoutAlignment.End, Microsoft.Maui.Primitives.LayoutAlignment.End ):
          gravityFlags = GravityFlags.Right | GravityFlags.Bottom;
          break;
        default:
          gravityFlags = GravityFlags.Left | GravityFlags.Top;
          break;
      }

      window.SetGravity( gravityFlags );
    }

    // This method will position the popup inside the popup's parent (defined in XAML), based on Alignment.
    private static void SetLayoutWithoutAnchorFromParentXaml( PopupHandler handler, IPopup popup )
    {
      if( handler == null )
        throw new ArgumentNullException( "handler" );
      if( popup == null )
        throw new ArgumentNullException( "popup" );

      var dialog = handler.PlatformView?.Tag as Dialog;
      if( dialog == null )
        return;

      var window = dialog.Window;
      if( window == null )
        return;

      window.SetGravity( GravityFlags.Top | GravityFlags.Left );

      var parent = PopupHandler.GetParent( popup ) as Microsoft.Maui.Controls.VisualElement;
      if( parent == null )
        throw new InvalidDataException( "parent must be a VisualElement" );

      var popupVisualElement = popup as Microsoft.Maui.Controls.VisualElement;
      if( popupVisualElement == null )
        throw new InvalidDataException( "popup must be a VisualElement" );

      var locationOnScreen = new int[ 2 ];
      parent.ToPlatform( handler.MauiContext ).GetLocationOnScreen( locationOnScreen );

      var androidNavigationBarHeight = PopupHandler.GetAndroidStatusBarHeight( window );

      var parentPosX = locationOnScreen[ 0 ];
      var parentPosY = locationOnScreen[ 1 ];
      var parentWidth = (int)dialog.Context.ToPixels( parent.Bounds.Width );
      var parentHeight = (int)dialog.Context.ToPixels( parent.Bounds.Height );
      var offsetX = (int)dialog.Context.ToPixels( popupVisualElement.Bounds.X );
      var offsetY = (int)dialog.Context.ToPixels( popupVisualElement.Bounds.Y );

      var windowAttributes = window.Attributes;
      if( windowAttributes == null )
        throw new InvalidDataException( "dialog.Window.Attributes must not be null." );

      switch(popup.HorizontalLayoutAlignment, popup.VerticalLayoutAlignment)
      {
        case (Microsoft.Maui.Primitives.LayoutAlignment.Center, Microsoft.Maui.Primitives.LayoutAlignment.Start ):
        case (Microsoft.Maui.Primitives.LayoutAlignment.Center, Microsoft.Maui.Primitives.LayoutAlignment.Fill ):
          {
            offsetX = ( parentWidth / 2 ) - ( windowAttributes.Width / 2 );
          }
          break;
        case (Microsoft.Maui.Primitives.LayoutAlignment.End, Microsoft.Maui.Primitives.LayoutAlignment.Start ):
        case (Microsoft.Maui.Primitives.LayoutAlignment.End, Microsoft.Maui.Primitives.LayoutAlignment.Fill ):
          {
            offsetX = parentWidth - windowAttributes.Width;
          }
          break;
        case (Microsoft.Maui.Primitives.LayoutAlignment.Start, Microsoft.Maui.Primitives.LayoutAlignment.Center ):
        case (Microsoft.Maui.Primitives.LayoutAlignment.Fill, Microsoft.Maui.Primitives.LayoutAlignment.Center ):
          {
            offsetY = ( parentHeight / 2 ) - ( windowAttributes.Height / 2 );
          }
          break;
        case (Microsoft.Maui.Primitives.LayoutAlignment.Center, Microsoft.Maui.Primitives.LayoutAlignment.Center ):
          {
            offsetX = ( parentWidth / 2 ) - ( windowAttributes.Width / 2 );
            offsetY = ( parentHeight / 2 ) - ( windowAttributes.Height / 2 );
          }
          break;
        case (Microsoft.Maui.Primitives.LayoutAlignment.End, Microsoft.Maui.Primitives.LayoutAlignment.Center ):
          {
            offsetX = parentWidth - windowAttributes.Width;
            offsetY = ( parentHeight / 2 ) - ( windowAttributes.Height / 2 );
          }
          break;
        case (Microsoft.Maui.Primitives.LayoutAlignment.Start, Microsoft.Maui.Primitives.LayoutAlignment.End ):
        case (Microsoft.Maui.Primitives.LayoutAlignment.Fill, Microsoft.Maui.Primitives.LayoutAlignment.End ):
          {
            offsetY = parentHeight - windowAttributes.Height;
          }
          break;
        case (Microsoft.Maui.Primitives.LayoutAlignment.Center, Microsoft.Maui.Primitives.LayoutAlignment.End ):
          {
            offsetX = ( parentWidth / 2 ) - ( windowAttributes.Width / 2 );
            offsetY = parentHeight - windowAttributes.Height;
          }
          break;
        case (Microsoft.Maui.Primitives.LayoutAlignment.End, Microsoft.Maui.Primitives.LayoutAlignment.End ):
          {
            offsetX = parentWidth - windowAttributes.Width;
            offsetY = parentHeight - windowAttributes.Height;
          }
          break;
        default:
          break;
      }

      windowAttributes.X = parentPosX + offsetX;
      windowAttributes.Y = parentPosY - androidNavigationBarHeight + offsetY;
    }

    #endregion

    #region Event Handlers

    private void PlatformView_Closed( object sender, object e )
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
