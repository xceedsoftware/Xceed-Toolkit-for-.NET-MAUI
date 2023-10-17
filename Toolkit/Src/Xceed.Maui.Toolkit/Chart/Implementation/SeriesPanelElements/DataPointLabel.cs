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
  internal class DataPointLabel : AreaElement
  {
    #region Internal Methods

    internal override void SetInfos()
    {
      this.Infos = new DataPointLabelElementInfos();
    }

    internal override void SetLocation( Point offset )
    {
      var isPositiveValue = ( this.Infos as DataPointLabelElementInfos ).DataPoint.InternalY >= 0d;
      var desiredSize = this.Measure( double.PositiveInfinity, double.PositiveInfinity );

      var posX = offset.X - desiredSize.Request.Width / 2;
      var posY = isPositiveValue
                 ? offset.Y - desiredSize.Request.Height
                 : offset.Y;

      this.Infos.Location = new Point( posX, posY );
    }

    internal void SetDataPoint( DataPoint dataPoint )
    {
      ( this.Infos as DataPointLabelElementInfos ).DataPoint = dataPoint;
    }

    #endregion
  }
}
