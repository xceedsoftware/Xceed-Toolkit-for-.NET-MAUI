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
using System.ComponentModel;

namespace Xceed.Maui.Toolkit
{
  public partial class MaskedTextBox
  {
    #region Private Members

    private int m_nextCursorPosition;
    private bool m_isPastingText;

    #endregion

    #region Partial Methods

    partial void Entry_Initialize( Entry entry )
    {
      if( entry != null )
      {
        var textBox = entry.Handler?.PlatformView as MauiPasswordTextBox;
        if( textBox != null )
        {
          textBox.BeforeTextChanging += this.TextBox_BeforeTextChanging;
          textBox.TextChanging += this.TextBox_TextChanging;
          textBox.PreviewKeyDown += this.TextBox_PreviewKeyDown;
          textBox.Paste += this.TextBox_Paste;
        }
      }
    }

    partial void Entry_Destroy( Entry entry )
    {
      if( entry != null )
      {
        var textBox = entry.Handler?.PlatformView as MauiPasswordTextBox;
        if( textBox != null )
        {
          textBox.BeforeTextChanging -= this.TextBox_BeforeTextChanging;
          textBox.TextChanging -= this.TextBox_TextChanging;
          textBox.PreviewKeyDown -= this.TextBox_PreviewKeyDown;
          textBox.Paste -= this.TextBox_Paste;
        }
      }
    }

    #endregion

    #region Event Handlers

    private void TextBox_BeforeTextChanging( Microsoft.UI.Xaml.Controls.TextBox sender, Microsoft.UI.Xaml.Controls.TextBoxBeforeTextChangingEventArgs args )
    {
      if( m_isUpdatingText || m_isPastingText || !this.IsMaskSet() )
        return;

      var selectionLength = ( m_entry != null ) ? Math.Max( 0, m_entry.SelectionLength - 1 ) : 0;
      if( !this.TryInsertChar( args.NewText[ this.CursorPosition ], this.CursorPosition, this.CursorPosition + selectionLength, out int nextCursorPosition, out MaskedTextResultHint hint ) )
      {
        args.Cancel = true;
      }

      m_nextCursorPosition = nextCursorPosition;
    }

    private void TextBox_TextChanging( Microsoft.UI.Xaml.Controls.TextBox sender, Microsoft.UI.Xaml.Controls.TextBoxTextChangingEventArgs args )
    {
      if( m_isUpdatingText || !this.IsMaskSet() )
        return;

      if( m_isPastingText )
      {
        m_isPastingText = false;
        return;
      }

      this.SetEntry( this.MaskedTextOutput, m_nextCursorPosition );
    }

    private void TextBox_PreviewKeyDown( object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e )
    {
      if( !this.IsMaskSet() )
        return;

      if( e.Key == Windows.System.VirtualKey.Back )
      {
        var selectionLength = ( m_entry != null ) ? Math.Max( 0, m_entry.SelectionLength - 1 ) : 0;
        var startIndex = (selectionLength > 0) ? this.CursorPosition : this.CursorPosition - 1;
        this.TryRemoveChar( startIndex, startIndex + selectionLength, out int nextCursorPosition, out MaskedTextResultHint unused );

        this.SetEntry( this.MaskedTextOutput, nextCursorPosition );

        e.Handled = true;
      }
      else if( e.Key == Windows.System.VirtualKey.Delete )
      {
        var selectionLength = (m_entry != null) ? Math.Max( 0, m_entry.SelectionLength - 1 ) : 0;
        this.TryRemoveChar( this.CursorPosition, this.CursorPosition + selectionLength, out int nextCursorPosition, out MaskedTextResultHint resultHint );

        if( resultHint == MaskedTextResultHint.NoEffect )
        {
          nextCursorPosition = m_maskProvider.FindEditPositionFrom( nextCursorPosition, true );
        }

        this.SetEntry( this.MaskedTextOutput, nextCursorPosition );

        e.Handled = true;
      }
    }

    private void TextBox_Paste( object sender, Microsoft.UI.Xaml.Controls.TextControlPasteEventArgs e )
    {
      if( !this.IsMaskSet() )
        return;

      var clipboardContent = Clipboard.GetTextAsync().Result;

      if( !string.IsNullOrEmpty( clipboardContent ) )
      {
        var selectionLength = ( m_entry != null ) ? m_entry.SelectionLength - 1 : -1;
        this.TryReplaceString( clipboardContent, this.CursorPosition, this.CursorPosition + selectionLength, out int nextCursorPosition, out MaskedTextResultHint hint );

        this.SetEntry( this.MaskedTextOutput, nextCursorPosition );

        if( m_entry != null )
        {
          m_entry.SelectionLength = 0;
        }
      }

      m_isPastingText = true;

      e.Handled = true;
    }

    #endregion
  }
}
