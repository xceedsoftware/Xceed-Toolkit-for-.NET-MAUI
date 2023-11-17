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


using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Xceed.Maui.Toolkit
{
  public partial class MaskedTextBox
  {
    #region Partial Methods

    partial void Entry_Initialize( Entry entry )
    {
      if( entry != null )
      {
        var androidTextBox = m_entry.Handler?.PlatformView as AndroidX.AppCompat.Widget.AppCompatEditText;
        if( androidTextBox != null )
        {
          androidTextBox.AfterTextChanged += this.AndroidTextBox_AfterTextChanged;
        }
      }
    }

    partial void Entry_Destroy( Entry entry )
    {
      if( entry != null )
      {
        var androidTextBox = m_entry.Handler?.PlatformView as AndroidX.AppCompat.Widget.AppCompatEditText;
        if( androidTextBox != null )
        {
          androidTextBox.AfterTextChanged -= this.AndroidTextBox_AfterTextChanged;
        }
      }
    }

    #endregion

    #region Event Handlers

    private void AndroidTextBox_AfterTextChanged( object sender, Android.Text.AfterTextChangedEventArgs e )
    {
      if( m_isUpdatingText || !this.IsMaskSet() )
        return;

      var androidTextBox = m_entry.Handler?.PlatformView as AndroidX.AppCompat.Widget.AppCompatEditText;
      if( androidTextBox != null )
      {
        int nextCursorPosition = this.CursorPosition;

        var newText = androidTextBox.Text ?? string.Empty;
        var cloneMaskProvider = m_maskProvider.Clone() as MaskedTextProvider;
        var rgx = new Regex( "[^a-zA-Z0-9]" );
        newText = rgx.Replace( newText, "" );

        if( newText.Length > m_maskProvider.EditPositionCount )
        {
          newText = newText.Substring( 0, m_maskProvider.EditPositionCount );
        }

        if( newText == this.Value )
        {
          nextCursorPosition = androidTextBox.SelectionStart;
        }
        else if( cloneMaskProvider.Set( newText ) )
        {
          m_maskProvider.Set( newText );

          if( androidTextBox.SelectionStart >= this.CursorPosition )
          {
            var diff = androidTextBox.SelectionStart - this.CursorPosition;
            nextCursorPosition = m_maskProvider.FindEditPositionFrom( this.CursorPosition, true );
            nextCursorPosition = m_maskProvider.FindEditPositionFrom( nextCursorPosition + diff, true );
            if( nextCursorPosition == -1 )
            {
              nextCursorPosition = m_maskProvider.Length;
            }
          }
          else 
          {
            nextCursorPosition = m_maskProvider.FindEditPositionFrom( androidTextBox.SelectionStart, false );
            if( nextCursorPosition == -1 )
            {
              nextCursorPosition = m_maskProvider.FindEditPositionFrom( androidTextBox.SelectionStart, true ) + 1;
            }
          }
        }

        this.SetEntry( this.MaskedTextOutput, nextCursorPosition );
      }
    }

    #endregion
  }
}
