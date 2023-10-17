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
  internal class SeriesLine : AreaElement
  {
    #region Public Properties

    #region StrokeThickness

    public static readonly BindableProperty StrokeThicknessProperty = BindableProperty.Create( nameof( StrokeThickness), typeof( double ), typeof( SeriesLine ), 0d, propertyChanged: OnStrokeThicknessChanged );

    public double StrokeThickness
    {
      get => ( double )GetValue( StrokeThicknessProperty );
      set => SetValue( StrokeThicknessProperty, value );
    }

    private static void OnStrokeThicknessChanged( BindableObject bindable, object oldValue, object newValue )
    {
      var seriesLine = bindable as SeriesLine;
      if( seriesLine != null )
      {
        seriesLine.OnStrokeThicknessChanged( ( double )oldValue, ( double )newValue );
      }
    }

    protected internal virtual void OnStrokeThicknessChanged( double oldValue, double newValue )
    {
      this.SetStrokeThickness( newValue );
    }

    #endregion

    #endregion

    #region Internal Methods

    internal override void SetInfos()
    {
      this.Infos = new SeriesLineElementInfo();
    }

    internal override void SetLocation( Rect bounds, Point offset )
    {
      // LineSeries is located at (0, 0) in its Rect bounds(correctly arranged).
      this.Infos.Location = new Point( 0, 0 );
    }

    internal void SetPoints( List<DataPointMarker> dataPointMarkers )
    {
      if( dataPointMarkers == null )
        throw new ArgumentNullException( nameof( dataPointMarkers ) );

      var infos = this.Infos as SeriesLineElementInfo;

      infos.Points.Clear();

      var halfStrokeThickness = ( this.Infos as SeriesLineElementInfo ).StrokeThickness / 2;

      foreach( var marker in dataPointMarkers )
      {
        infos.Points.Add( new Point( marker.Infos.Location.X + ( marker.DesiredSize.Width / 2 ) - halfStrokeThickness,
                                     marker.Infos.Location.Y + ( marker.DesiredSize.Height / 2 ) - halfStrokeThickness ) );
      }
    }

    internal void SetStrokeThickness( double strokeThickness )
    {
      ( this.Infos as SeriesLineElementInfo ).StrokeThickness = strokeThickness;
    }

    #endregion
  }
}
