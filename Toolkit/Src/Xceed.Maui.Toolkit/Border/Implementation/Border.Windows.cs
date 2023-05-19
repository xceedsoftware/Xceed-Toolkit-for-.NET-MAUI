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
using Microsoft.UI.Xaml.Input;

namespace Xceed.Maui.Toolkit
{
  public partial class Border
  {
    #region Partial Methods

    partial void InitializeForPlatform( object sender, EventArgs e )
    {
      var border = sender as Border;
      if( border != null )
      {
        var contentPanel = border.Handler?.PlatformView as ContentPanel;
        if( contentPanel != null )
        {
          contentPanel.PointerPressed += this.PointerPressed;
          contentPanel.PointerReleased += this.PointerReleased;
          contentPanel.PointerEntered += this.PointerEntered;
          contentPanel.PointerExited += this.PointerExited;
        }
      }
    }

    partial void UninitializeForPlatform( object sender, HandlerChangingEventArgs e )
    {
      var border = sender as Border;
      if( border != null )
      {
        var contentPanel = border.Handler?.PlatformView as ContentPanel;
        if( contentPanel != null )
        {
          contentPanel.PointerPressed -= this.PointerPressed;
          contentPanel.PointerReleased -= this.PointerReleased;
          contentPanel.PointerEntered -= this.PointerEntered;
          contentPanel.PointerExited -= this.PointerExited;
        }
      }
    }

    #endregion

    #region Event Handlers

    private void PointerPressed( object sender, PointerRoutedEventArgs e )
    {
      this.RaisePointerDownEvent( this, EventArgs.Empty );
    }

    private void PointerReleased( object sender, PointerRoutedEventArgs e )
    {
      this.RaisePointerUpEvent( this, EventArgs.Empty );
    }

    private void PointerEntered( object sender, PointerRoutedEventArgs e )
    {
      this.RaisePointerEnterEvent( this, EventArgs.Empty );
    }

    private void PointerExited( object sender, PointerRoutedEventArgs e )
    {
      this.RaisePointerLeaveEvent( this, EventArgs.Empty );
    }

    #endregion

    #region Events

    public event EventHandler PointerEnter;

    public void RaisePointerEnterEvent( object sender, EventArgs e )
    {
      this.PointerEnter?.Invoke( sender, e );
    }

    public event EventHandler PointerLeave;

    public void RaisePointerLeaveEvent( object sender, EventArgs e )
    {
      this.PointerLeave?.Invoke( sender, e );
    }

    #endregion
  }
}
