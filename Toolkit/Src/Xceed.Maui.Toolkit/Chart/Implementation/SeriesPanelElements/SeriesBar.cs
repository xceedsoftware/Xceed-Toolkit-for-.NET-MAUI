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
  internal class SeriesBar : AreaElement
  {
    #region Internal Properties

    #region Width

    public static new readonly BindableProperty WidthProperty = BindableProperty.Create( nameof( Width ), typeof( double ), typeof( SeriesBar ), 0d, propertyChanged: OnWidthChanged );

    public new double Width
    {
      get => ( double )GetValue( WidthProperty );
      set => SetValue( WidthProperty, value );
    }

    private static void OnWidthChanged( BindableObject bindable, object oldValue, object newValue )
    {
      var seriesBar = bindable as SeriesBar;
      if( seriesBar != null )
      {
        seriesBar.OnWidthChanged( ( double )oldValue, ( double )newValue );
      }
    }

    protected internal virtual void OnWidthChanged( double oldValue, double newValue )
    {
      this.SetWidth( newValue );
    }

    #endregion

    #endregion

    #region Internal Methods

    internal override void SetInfos()
    {
      this.Infos = new SeriesBarElementInfo();
    }

    internal override void SetLocation( Point offset )
    {
      var desiredSize = this.Measure( double.PositiveInfinity, double.PositiveInfinity );
      var posX = offset.X - ( desiredSize.Request.Width / 2 );
      var posY = offset.Y;

      this.Infos.Location = new Point( posX, posY );
    }

    internal void SetHeight( double height )
    {
      (this.Infos as SeriesBarElementInfo).Height = height;
    }

    internal void SetWidth( double width )
    {
      ( this.Infos as SeriesBarElementInfo ).Width = width;
    }

    #endregion
  }
}
