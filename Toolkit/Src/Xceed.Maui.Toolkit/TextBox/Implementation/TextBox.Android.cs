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
          var androidTextBox = m_entry.Handler?.PlatformView as AndroidX.AppCompat.Widget.AppCompatEditText;
          if( androidTextBox != null )
          {
            androidTextBox.SetPadding( 1, 1, 1, 1 );
            // Remove Underline.
            androidTextBox.SetBackgroundColor( Android.Graphics.Color.Transparent );
          }
        }
      }
    }

    #endregion

    #region Protected Methods

    protected override Size MeasureOverride( double widthConstraint, double heightConstraint )
    {
      var size = new Size( widthConstraint, heightConstraint );

      if( m_entry != null )
      {
        var parentBorder = this.GetParentBorder( m_entry );
        if( parentBorder != null )
        {
          // Set a height for PART_Entry so that VerticalTextAlignment can work.
          var parentBorderHeight = parentBorder.Measure( double.PositiveInfinity, double.PositiveInfinity ).Request.Height;
          m_entry.HeightRequest = Math.Max( 0, parentBorderHeight - parentBorder.BorderThickness.VerticalThickness - this.Padding.VerticalThickness );
        }
      }

      return base.MeasureOverride( size.Width, size.Height );
    }

    #endregion

    #region Private Methods

    private Border GetParentBorder( View view )
    {
      if( view == null )
        return null;

      var parent = view.Parent;
      while( parent != null ) 
      {
        if( parent is Border )
          return parent as Border;

        parent = parent.Parent;
      }

      return null;
    }

    #endregion
  }
}
