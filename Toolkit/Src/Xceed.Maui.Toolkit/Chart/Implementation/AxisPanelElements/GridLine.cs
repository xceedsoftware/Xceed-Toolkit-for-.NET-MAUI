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
  internal class GridLine : LineBase
  {
    #region Constructors

    internal GridLine( Orientation orientation )
      : base( orientation )
    {
    }

    #endregion

    #region Internal Methods

    internal override void SetPoints( Rect bounds, double offset )
    {
      var infos = this.Infos as AreaElementInfos;

      infos.Points.Clear();

      // Use Points to measure the GridLine.
      infos.Points.Add( new Point( bounds.Left, bounds.Top ) );

      if( this.Orientation == Orientation.Vertical )
      {
        infos.Points.Add( new Point( bounds.Left, bounds.Bottom ) );
      }
      else if( this.Orientation == Orientation.Horizontal )
      {
        infos.Points.Add( new Point( bounds.Right, bounds.Top ) );
      }
    }

    internal override void SetLocation( Rect bounds, Point offset )
    {
      // Use Location to arrange the GridLine.
      this.Infos.Location = ( this.Orientation == Orientation.Vertical )
                            ? new Point( bounds.Left + offset.X, bounds.Top )
                            : new Point( bounds.Left, bounds.Bottom - offset.X );
    }

    #endregion
  }
}
