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


using UIKit;

namespace Xceed.Maui.Toolkit
{
  public partial class TextBox
  {
    #region Partial Methods

    partial void InitializeForPlatform( object sender, EventArgs e )
    {
      var textBox = sender as TextBox;
      if( textBox != null )
      {
        if( m_entry == null )
        {
          m_entry = textBox.GetTemplateChild( "PART_Entry" ) as Entry;
        }

        if( m_entry != null )
        {
          var iosTextBox = m_entry.Handler?.PlatformView as UITextField;
          if( iosTextBox != null )
          {
            iosTextBox.BorderStyle = UITextBorderStyle.None;
            iosTextBox.EditingDidBegin += this.IosTextBox_EditingDidBegin;
            iosTextBox.EditingDidEnd += this.IosTextBox_EditingDidEnd;
          }
        }
      }
    }

    partial void UninitializeForPlatform( object sender, HandlerChangingEventArgs e )
    {
      var textBox = sender as TextBox;
      if( textBox != null )
      {
        if( m_entry == null )
        {
          m_entry = textBox.GetTemplateChild( "PART_Entry" ) as Entry;
        }

        if( m_entry != null )
        {
          var iosTextBox = m_entry.Handler?.PlatformView as UITextField;
          if( iosTextBox != null )
          {
            iosTextBox.EditingDidBegin -= this.IosTextBox_EditingDidBegin;
            iosTextBox.EditingDidEnd -= this.IosTextBox_EditingDidEnd;
          }
        }
      }
    }

    #endregion

    #region Event Handlers

    private void IosTextBox_EditingDidEnd( object sender, EventArgs e )
    {
      this.SetFocus( false );
    }

    private void IosTextBox_EditingDidBegin( object sender, EventArgs e )
    {
      this.SetFocus( true );
    }
    #endregion
  }
}
