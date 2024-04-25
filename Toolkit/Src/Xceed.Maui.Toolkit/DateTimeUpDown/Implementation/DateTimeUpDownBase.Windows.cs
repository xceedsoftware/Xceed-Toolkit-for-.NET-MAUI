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
using Windows.System;

namespace Xceed.Maui.Toolkit
{
  public partial class DateTimeUpDownBase<T>
  {
    #region Partial Methods

    partial void InitializeForPlatform( object sender, EventArgs e )
    {
      var dateTimeUpDown = sender as DateTimeUpDownBase<T>;
      if( dateTimeUpDown != null )
      {
        var contentPanel = dateTimeUpDown.Handler?.PlatformView as ContentPanel;
        if( contentPanel != null )
        {
          contentPanel.PreviewKeyDown += this.OnPreviewKeyDown;
        }
      }
    }

    partial void UninitializeForPlatform( object sender, HandlerChangingEventArgs e )
    {
      var dateTimeUpDown = sender as DateTimeUpDownBase<T>;
      if( dateTimeUpDown != null )
      {
        var contentPanel = dateTimeUpDown.Handler?.PlatformView as ContentPanel;
        if( contentPanel != null )
        {
          contentPanel.PreviewKeyDown -= this.OnPreviewKeyDown;
        }
      }
    }

    #endregion

    #region Event Handlers

    private void OnPreviewKeyDown( object sender, KeyRoutedEventArgs e )
    {
      var selectionStart = ( m_selectedDateTimeInfo != null ) ? m_selectedDateTimeInfo.StartPosition : 0;
      var selectionLength = ( m_selectedDateTimeInfo != null ) ? m_selectedDateTimeInfo.Length : 0;

      switch( e.Key )
      {
        case VirtualKey.Right:
          {
            if( this.IsCurrentValueValid() )
            {
              this.PerformKeyboardSelection( selectionStart + selectionLength );
              e.Handled = true;
            }
          }
          break;
        case VirtualKey.Left:
          {
            if( this.IsCurrentValueValid() )
            {
              this.PerformKeyboardSelection( selectionStart > 0 ? selectionStart - 1 : 0 );
              e.Handled = true;
            }
          }
          break;
      }
    }

    #endregion
  }
}
