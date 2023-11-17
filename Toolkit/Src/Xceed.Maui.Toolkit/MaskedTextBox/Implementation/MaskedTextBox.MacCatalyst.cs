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
using Foundation;
using UIKit;

namespace Xceed.Maui.Toolkit
{
  public partial class MaskedTextBox
  {
    #region Partial Methods

    partial void Entry_Initialize( Entry entry )
    {
      if( entry != null )
      {
        var macTextBox = m_entry.Handler?.PlatformView as UITextField;
        if( macTextBox != null )
        {
          macTextBox.Delegate = new TextFieldValidator( this );
        }
      }
    }

    #endregion
  }

  public class TextFieldValidator : UITextFieldDelegate
  {
    private MaskedTextBox m_maskedTextBox;

    public TextFieldValidator( MaskedTextBox maskedTextBox )
    {
      m_maskedTextBox = maskedTextBox;
    }

    public override bool ShouldChangeCharacters( UITextField textField, NSRange range, string replacementString )
    {
      if( ( m_maskedTextBox == null ) || !m_maskedTextBox.IsMaskSet() )
        return true;

      if( replacementString.Length > 0 )
      {
        if( replacementString.Length == 1 )
        {
          if( m_maskedTextBox.TryInsertChar( replacementString[ 0 ], (int)range.Location, (int)range.Location + (int)range.Length, out int nextCursorPosition, out MaskedTextResultHint hint ) )
          {
            m_maskedTextBox.SetEntry( m_maskedTextBox.MaskedTextOutput, nextCursorPosition );
          }
        }
        else
        {
          replacementString = replacementString.Trim();
          var selectionLength = (int)range.Length - 1;
          m_maskedTextBox.TryReplaceString( replacementString, (int)range.Location, (int)range.Location + selectionLength, out int nextCursorPosition, out MaskedTextResultHint hint );

          m_maskedTextBox.SetEntry( m_maskedTextBox.MaskedTextOutput, nextCursorPosition );
        }
      }
      // We have a Cut action, a Backspace or a delete action.
      else
      {
        if( range.Length > 1 )
        {
          UIPasteboard.General.String = m_maskedTextBox.Text.Substring( (int)range.Location, (int)range.Length );
        }

        var selectionLength = Math.Max( 0, (int)range.Length - 1 );
        if( m_maskedTextBox.TryRemoveChar( (int)range.Location, (int)range.Location + selectionLength, out int nextCursorPosition, out MaskedTextResultHint resultHint ) )
        {
          if( resultHint == MaskedTextResultHint.NoEffect )
          {
            if( (int)range.Location == m_maskedTextBox.CursorPosition )
            {
              nextCursorPosition = m_maskedTextBox.GetMaskedTextProvider().FindEditPositionFrom( nextCursorPosition, true );
            }
          }

          m_maskedTextBox.SetEntry( m_maskedTextBox.MaskedTextOutput, nextCursorPosition );
        }
      }

      return false;
    }
  }

}
