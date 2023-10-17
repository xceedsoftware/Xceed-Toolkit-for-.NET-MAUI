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
  internal class Tick : LineBase
  {
    #region Constructors

    internal Tick( Orientation orientation )
      : base( orientation )
    {
    }

    #endregion

    #region Internal Properties

    #region Length

    internal static readonly BindableProperty LengthProperty = BindableProperty.Create( nameof( Length ), typeof( double ), typeof( Tick ) );

    internal double Length
    {
      get => (double)GetValue( LengthProperty );
      set => SetValue( LengthProperty, value );
    }

    #endregion

    #endregion

    #region Internal Methods

    internal override void SetPoints( Rect bounds, double offset )
    {
      var infos = this.Infos as AreaElementInfos;

      infos.Points.Clear();

      infos.Points.Add( new Point( bounds.Left, bounds.Top ) );

      if( this.Orientation == Orientation.Vertical )
      {
        infos.Points.Add( new Point( bounds.Left, bounds.Top + this.Length ) );
      }
      else if( this.Orientation == Orientation.Horizontal )
      {
        infos.Points.Add( new Point( bounds.Left + this.Length, bounds.Top ) );
      }
    }

    internal override void SetLocation( Rect bounds, Point offset )
    {
      var desiredSize = this.Measure( double.PositiveInfinity, double.PositiveInfinity );
      var posX = offset.X - ( desiredSize.Request.Width / 2 );
      var posY = offset.X + ( desiredSize.Request.Height / 2 );

      // Use Location to arrange the Tick.
      this.Infos.Location = ( this.Orientation == Orientation.Vertical )
                            ? new Point( bounds.Left + posX, bounds.Bottom - this.Length - offset.Y )
                            : new Point( bounds.Left + offset.Y, bounds.Bottom - posY );
    }

    #endregion
  }
}
