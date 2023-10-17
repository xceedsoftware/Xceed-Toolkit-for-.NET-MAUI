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
  internal class DataPointLabelLine : AreaElement
  {
    #region Internal Properties

    #region Length

    internal static readonly BindableProperty LengthProperty = BindableProperty.Create( nameof( Length ), typeof( double ), typeof( DataPointLabelLine ) );

    internal double Length
    {
      get => ( double )GetValue( LengthProperty );
      set => SetValue( LengthProperty, value );
    }

    #endregion

    #endregion

    #region Internal Methods

    internal override void SetInfos()
    {
      this.Infos = new SeriesLineElementInfo();
    }

    internal override void SetLocation( Point offset )
    {
      var desiredSize = this.Measure( double.PositiveInfinity, double.PositiveInfinity );
      var posX = offset.X - ( desiredSize.Request.Width / 2 );

      this.Infos.Location = new Point( posX, offset.Y );
    }

    internal void SetPoints( double lineLength )
    {
      var infos = this.Infos as SeriesLineElementInfo;

      infos.Points.Clear();

      infos.Points.Add( new Point( 0, 0 ) );
      infos.Points.Add( new Point( 0, lineLength ) );
    }

    #endregion
  }
}
