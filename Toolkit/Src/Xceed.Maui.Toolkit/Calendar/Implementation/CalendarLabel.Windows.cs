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


using Microsoft.UI.Xaml.Controls;

namespace Xceed.Maui.Toolkit
{
	public partial class CalendarLabel
	{
		#region Partial Methods

		partial void InitializeForPlatform( object sender, EventArgs e )
		{
			var label = sender as CalendarLabel;
			if( label != null )
			{
				var textBlock = label.Handler?.PlatformView as TextBlock;
				if( textBlock != null )
				{
					textBlock.PointerEntered += this.Button_PointerEntered;
					textBlock.PointerExited += this.Button_PointerExited;
					textBlock.PointerPressed += this.TextBlock_PointerPressed;
					textBlock.PointerReleased += this.TextBlock_PointerReleased;
				}
			}
		}

		partial void UninitializeForPlatform( object sender, HandlerChangingEventArgs e )
		{
			var label = sender as CalendarLabel;

			var textBlock = label.Handler?.PlatformView as TextBlock;
			if( textBlock != null )
			{
				textBlock.PointerEntered -= this.Button_PointerEntered;
				textBlock.PointerExited -= this.Button_PointerExited;
			}
		}

    #endregion

    #region Event Handlers

    private void Button_PointerEntered( object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e )
    {
      this.IsPointerOver = true;
    }

    private void Button_PointerExited( object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e )
    {
      this.IsPointerOver = false;
    }

    private void TextBlock_PointerPressed( object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e )
    {
			this.PointerDown();
    }

    private void TextBlock_PointerReleased( object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e )
    {
			this.PointerUp();
    }

    #endregion
  }
}
