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
  internal class TitleLabel : AreaElement
  {
    #region Private Members

    private readonly Orientation m_axisOrientation;

    #endregion

    #region Constructors

    internal TitleLabel( Orientation axisOrientation )
    {
      m_axisOrientation = axisOrientation;
    }

    #endregion

    #region Internal Properties

    internal override void SetInfos()
    {
      this.Infos = new LabelElementInfos();
    }

    internal override void SetLocation( Rect bounds, Point offset )
    {
      this.Infos.Location = ( m_axisOrientation == Orientation.Horizontal )
                            ? new Point( bounds.Width / 2 - this.DesiredSize.Width / 2, bounds.Height - this.DesiredSize.Height )
                            : new Point( 0, bounds.Height / 2 - this.DesiredSize.Width / 2 - this.DesiredSize.Height );  // Vertical Title label is rotated.
    }

    #endregion
  }
}
