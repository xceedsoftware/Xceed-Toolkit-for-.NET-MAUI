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


using System.Diagnostics;

namespace Xceed.Maui.Toolkit
{
  public class BarRenderer : RendererBase
  {
    #region Internal Members

    internal const string DefaultBarRendererTemplateName = "BarRendererTemplate";

    #endregion

    #region Private Members

    private readonly List<SeriesBar> m_seriesBars = new List<SeriesBar>();
    private readonly List<DataPointLabel> m_dataPointLabels = new List<DataPointLabel>();
    private readonly List<DataPointLabelLine> m_dataPointLabelLines = new List<DataPointLabelLine>();

    #endregion

    #region Public Properties

    #region DefaultTemplate

    public static DataTemplate DefaultTemplate
    {
      get;
      internal set;
    }

    #endregion

    #region Width

    public static readonly BindableProperty WidthProperty = BindableProperty.Create( nameof( Width ), typeof( double ), typeof( BarRenderer ), 0d, propertyChanged: OnWidthChanged );

    public double Width
    {
      get => ( double )GetValue( WidthProperty );
      set => SetValue( WidthProperty, value );
    }

    private static void OnWidthChanged( BindableObject bindable, object oldValue, object newValue )
    {
      var barRenderer = bindable as BarRenderer;
      if( barRenderer != null )
      {
        barRenderer.OnWidthChanged( (double)oldValue, (double)newValue );
      }
    }

    protected virtual void OnWidthChanged( double oldValue, double newValue )
    {
      if( newValue < 0 )
        throw new InvalidDataException( "BarRenderer's width must be greater than 0." );

      if( (m_seriesBars != null) && (newValue != 0d) )
      {
        m_seriesBars.ForEach( seriesBar => seriesBar.SetWidth( newValue ) );
      }
    }

    #endregion

    #endregion

    #region Public Methods

    public override IList<AreaElement> CreateVisualChildren( Series series )
    {
      m_seriesBars.Clear();
      m_dataPointLabels.Clear();
      m_dataPointLabelLines.Clear();

      var elements = new List<AreaElement>();

      series.DataPoints.ToList().ForEach( dataPoint => this.CreateDataPointVisual( series, elements ) );

      return elements;
    }

    public override void ApplyDefaultTemplate( Series series )
    {
      if( series == null )
        throw new ArgumentNullException( nameof( series ) );

      if( (series.Template == null) || series.IsUsingDefaultTemplate() )
      {
        series.Template = BarRenderer.DefaultTemplate;
      }
    }

    public override void InitializeSeries( Rect bounds, Series series )
    {
      if( series == null )
        throw new ArgumentNullException( nameof( series ) );

      Debug.Assert( m_seriesBars.Count == series.DataPoints.Count, "The SeriesBars should have the same number of points as the Series DataPoint's count." );

      var pixelDataPoints = BarRenderer.GetDataPointsInPixels( bounds, series, out double negativeOffsetYToZero, out double positiveOffsetYToZero );

      // Order the DataPoints.
      var orderedDataPoints = series.DataPoints.ToList();
      orderedDataPoints.Sort( new DataPointComparer() );

      if( series.ShowDataPointLabels )
      {
        Debug.Assert( series.DataPoints.Count == m_dataPointLabels.Count, "The SeriesBars should have the same number of points as the DataPoint's label count." );
        Debug.Assert( series.DataPoints.Count == m_dataPointLabelLines.Count, "The SeriesBars should have the same number of points as the DataPoint label's line count." );
      }

      for( int i = 0; i < pixelDataPoints.Count; ++i )
      {
        var isPositiveOrZeroValue = ( ( orderedDataPoints[ i ].InternalY >= 0 ) && ( series.Chart.DataPointRange[ 1 ].Start >= 0 ) )
                                    || ( ( orderedDataPoints[ i ].InternalY >= 0 ) && ( series.Chart.DataPointRange[ 1 ].End >= 0 ) )
                                    || ( ( orderedDataPoints[ i ].InternalY < 0 ) && ( series.Chart.DataPointRange[ 1 ].Start >= 0 ) );
        var pixelDataPoint = new Point( pixelDataPoints[ i ].X, pixelDataPoints[ i ].Y );

        if( series.ShowDataPointLabels )
        {
          m_dataPointLabels[ i ].SetDataPoint( orderedDataPoints[ i ] );
          var posY = isPositiveOrZeroValue ? pixelDataPoint.Y - m_dataPointLabelLines[ i ].Length : pixelDataPoint.Y + m_dataPointLabelLines[ i ].Length;
          var pixelDataPointLabel = new Point( pixelDataPoint.X, posY );
          m_dataPointLabels[ i ].SetLocation( pixelDataPointLabel );

          m_dataPointLabelLines[ i ].SetPoints( m_dataPointLabelLines[ i ].Length );
          posY = isPositiveOrZeroValue ? posY : pixelDataPoint.Y;
          var pixelDataPointLabelLine = new Point( pixelDataPoint.X, posY );
          m_dataPointLabelLines[ i ].SetLocation( pixelDataPointLabelLine );
        }

        if( orderedDataPoints[ i ].InternalY < 0 )
        {
          m_seriesBars[ i ].Rotation = 180;
        }

        if( this.Width != 0d )
        {
          m_seriesBars[ i ].SetWidth( this.Width );
        }

        var height = isPositiveOrZeroValue
                     ? bounds.Bottom - pixelDataPoints[ i ].Y - negativeOffsetYToZero
                     : pixelDataPoints[ i ].Y - bounds.Y - positiveOffsetYToZero;
        m_seriesBars[ i ].SetHeight( Math.Abs( height ) );

        var locationY = isPositiveOrZeroValue
                        ? pixelDataPoints[ i ].Y
                        : bounds.Y + positiveOffsetYToZero;
        m_seriesBars[ i ].SetLocation( new Point( pixelDataPoints[ i ].X, locationY ) );
      }
    }

    public override void ClearDataPointVisuals( Series series )
    {
      if( series == null )
        throw new ArgumentNullException( nameof( series ) );

      m_seriesBars.Clear();

      if( series.ShowDataPointLabels )
      {
        m_dataPointLabelLines.Clear();

        m_dataPointLabels.Clear();
      }
    }

    #endregion

    #region Internal Methods

    protected internal override double GetSeriesHeight( IRange seriesDataPointRangeY, IRange chartRangeY, double heightConstraint )
    {
      double minSeriesDataPointY;
      double maxSeriesDataPointY;

      // Series is over 0.
      if( seriesDataPointRangeY.Start >= 0 )
      {
        minSeriesDataPointY = Math.Min( seriesDataPointRangeY.Start, Math.Max( 0, chartRangeY.Start ) );
        maxSeriesDataPointY = Math.Min( seriesDataPointRangeY.End, chartRangeY.End );
      }
      // Series is under 0.
      else if( seriesDataPointRangeY.End <= 0 )
      {
        minSeriesDataPointY = Math.Max( seriesDataPointRangeY.Start, chartRangeY.Start );
        maxSeriesDataPointY = Math.Max( seriesDataPointRangeY.End, Math.Min( 0, chartRangeY.End ) );
      }
      else
      {
        minSeriesDataPointY = Math.Max( seriesDataPointRangeY.Start, chartRangeY.Start );
        maxSeriesDataPointY = Math.Min( seriesDataPointRangeY.End, chartRangeY.End );
      }

      var numerator = ( maxSeriesDataPointY - minSeriesDataPointY );

      return Math.Abs( numerator / ( chartRangeY.End - chartRangeY.Start ) ) * heightConstraint;
    }

    protected internal override double GetSeriesY( IRange seriesDataPointRangeY, IRange chartRangeY, double heightConstraint )
    {
      var seriesY = Math.Max( 0, Math.Min( seriesDataPointRangeY.End, chartRangeY.End ) );

      return Math.Max( 0d, ( chartRangeY.End - seriesY ) / ( chartRangeY.End - chartRangeY.Start ) * heightConstraint );
    }

    protected internal override void CreateDataPointVisual( Series series, List<AreaElement> elements )
    {
      if( series == null )
        throw new ArgumentNullException( nameof( series ) );

      if( elements == null )
        throw new ArgumentNullException( nameof( elements ) );

      var seriesBackground = RendererBase.GetSeriesBackground( series );

      if( series.ShowDataPointLabels )
      {
        // Add the DataPoint label's lines.
        var dataPointLabelLine = new DataPointLabelLine();
        dataPointLabelLine.SetBinding( DataPointLabelLine.ContentTemplateProperty, "DataPointLabelLineTemplate" );
        dataPointLabelLine.SetBinding( DataPointLabelLine.LengthProperty, "DataPointLabelLineLength" );
        dataPointLabelLine.Content = dataPointLabelLine.Infos;
        dataPointLabelLine.BindingContext = series;

        m_dataPointLabelLines.Add( dataPointLabelLine );
        elements.Add( dataPointLabelLine );

        // Add the DataPoint's labels.
        var dataPointLabel = new DataPointLabel();
        dataPointLabel.SetBinding( DataPointLabel.ContentTemplateProperty, "DataPointLabelTemplate" );
        dataPointLabel.Content = dataPointLabel.Infos;
        dataPointLabel.BindingContext = series;

        m_dataPointLabels.Add( dataPointLabel );
        elements.Add( dataPointLabel );
      }

      // Add the DataPoint's bars.
      var seriesBar = new SeriesBar();
      seriesBar.SetBinding( SeriesBar.ContentTemplateProperty, "Template" );
      seriesBar.Infos.Background = seriesBackground;
      seriesBar.Content = seriesBar.Infos;
      seriesBar.BindingContext = series;

      m_seriesBars.Add( seriesBar );
      elements.Add( seriesBar );
    }

    protected internal override void RemoveDataPointVisual( Series series, int index, List<AreaElement> elements )
    {
      if( series == null )
        throw new ArgumentNullException( nameof( series ) );

      if( index < 0 )
        throw new InvalidDataException( "Can't remove DataPoint at index " + index );

      if( elements == null )
        throw new ArgumentNullException( nameof( elements ) );

      var orderedDataPoints = series.DataPoints.ToList();
      orderedDataPoints.Sort( new DataPointComparer() );

      if( index >= m_seriesBars.Count )
        throw new InvalidDataException( "Can't remove element at position " + index + " for SeriesBars." );

      elements.Add( m_seriesBars[ index ] );
      m_seriesBars.RemoveAt( index );

      if( series.ShowDataPointLabels )
      {
        elements.Add( m_dataPointLabelLines[ index ] );
        m_dataPointLabelLines.RemoveAt( index );

        elements.Add( m_dataPointLabels[ index ] );
        m_dataPointLabels.RemoveAt( index );
      }
    }

    protected internal override double GetLeftOverlap( Series series )
    {
      if( (series.DataPointRange.Count <= 0) || (series.Chart == null) )
        return 0d;

      var seriesDataPointRangeX = series.DataPointRange[ 0 ];
      var chartRangeX = series.Chart.HorizontalAxis.IsUsingCustomRange()
                         ? series.Chart.HorizontalAxis.CustomRange
                         : series.Chart.DataPointRange[ 0 ];

      if( seriesDataPointRangeX.Start == chartRangeX.Start )
      {
          var barWidth = (m_seriesBars.Count > 0) ? m_seriesBars[ 0 ].Width : 0d;
          return (barWidth / 2);
      }

      return 0;
    }

    protected internal override double GetRightOverlap( Series series )
    {
      if( ( series.DataPointRange.Count <= 0 ) || ( series.Chart == null ) )
        return 0d;

      var seriesDataPointRangeX = series.DataPointRange[ 0 ];
      var chartRangeX = series.Chart.HorizontalAxis.IsUsingCustomRange()
                         ? series.Chart.HorizontalAxis.CustomRange
                         : series.Chart.DataPointRange[ 0 ];

      if( seriesDataPointRangeX.End == chartRangeX.End )
      {
        var barWidth = ( m_seriesBars.Count > 0 ) ? m_seriesBars.Last().Width : 0d;
        return ( barWidth / 2 );
      }

      return 0;
    }

    #endregion

    #region Private Methods

    private static void GetMinMaxDataPoints( Series series, out double minDataPointX, out double maxDataPointX, out double minDataPointY, out double maxDataPointY )
    {
      if( series == null )
        throw new ArgumentNullException( nameof( series ) );

      var dataPoints = series.DataPoints;

      minDataPointX = ( dataPoints.Count > 0 ) ? dataPoints.Min( dataPoint => dataPoint.InternalX ) : 0d;
      maxDataPointX = ( dataPoints.Count > 0 ) ? dataPoints.Max( dataPoint => dataPoint.InternalX ) : 0d;
      minDataPointY = ( dataPoints.Count > 0 ) ? dataPoints.Min( dataPoint => dataPoint.InternalY ) : 0d;
      maxDataPointY = ( dataPoints.Count > 0 ) ? dataPoints.Max( dataPoint => dataPoint.InternalY ) : 0d;

      if( series.Chart.HorizontalAxis.IsUsingCustomRange() )
      {
        if( minDataPointX < series.Chart.HorizontalAxis.CustomRange.Start )
        {
          minDataPointX = ( minDataPointX > 0 ) ? series.Chart.HorizontalAxis.CustomRange.Start : Math.Min( series.Chart.HorizontalAxis.CustomRange.Start, minDataPointX );
        }
        if( maxDataPointX > series.Chart.HorizontalAxis.CustomRange.End )
        {
          maxDataPointX = ( maxDataPointX < 0 ) ? series.Chart.HorizontalAxis.CustomRange.End : Math.Max( series.Chart.HorizontalAxis.CustomRange.End, maxDataPointX );
        }
      }
      // Clip too big values when scaling is manual.
      if( series.Chart.VerticalAxis.IsUsingCustomRange() )
      {
        minDataPointY = ( minDataPointY >= 0 ) ? Math.Max( 0, series.Chart.VerticalAxis.CustomRange.Start ) : Math.Max( series.Chart.VerticalAxis.CustomRange.Start, minDataPointY );
        maxDataPointY = ( maxDataPointY < 0 ) ? Math.Min( 0, series.Chart.VerticalAxis.CustomRange.End ) : Math.Min( series.Chart.VerticalAxis.CustomRange.End, maxDataPointY );
      }
    }

    private static List<Point> GetDataPointsInPixels( Rect bounds, Series series, out double negativeOffsetYToZero, out double positiveOffsetYToZero )
    {
      if( series == null )
        throw new ArgumentNullException( nameof( series ) );

      var dataPoints = series.DataPoints.ToList();
      dataPoints.Sort( new DataPointComparer() );

      var XAxisLength = bounds.Width;
      var YAxisLength = bounds.Height;
      BarRenderer.GetMinMaxDataPoints( series, out double minDataPointX, out double maxDataPointX, out double minDataPointY, out double maxDataPointY );
      negativeOffsetYToZero = (( maxDataPointY > 0 ) && ( minDataPointY < 0 ))
                              ? -minDataPointY / Math.Max( 1, ( maxDataPointY - minDataPointY ) ) * YAxisLength
                              : 0d;
      positiveOffsetYToZero = ( maxDataPointY > 0 ) && ( minDataPointY < 0 )
                              ? maxDataPointY / Math.Max( 1, ( maxDataPointY - minDataPointY ) ) * YAxisLength
                              : 0d;      

      var pixelsDataPoints = new List<Point>();

      for( int i = 0; i < dataPoints.Count; ++i )
      {
        var point = new Point( dataPoints[ i ].InternalX, dataPoints[ i ].InternalY );

        // Clip dataPoint to DataPointRange values.
        point.Y = Math.Max( point.Y, series.Chart.DataPointRange[ 1 ].Start );
        point.Y = Math.Min( point.Y, series.Chart.DataPointRange[ 1 ].End );

        var pointPercentX = ( ( point.X - minDataPointX ) / Math.Max( 1, ( maxDataPointX - minDataPointX ) ) );
        var numerator = ( point.Y >= 0 ) 
                        ? ( minDataPointY > 0 ) ? point.Y - minDataPointY : point.Y
                        : ( maxDataPointY < 0 ) ? maxDataPointY - point.Y : -point.Y;
        var pointPercentY = ( numerator / Math.Max( 1, ( maxDataPointY - minDataPointY ) ) );

        var pointInPixelsX = pointPercentX * XAxisLength;
        var pointInPixelsY = pointPercentY * YAxisLength;

        var posY = ( point.Y >= 0 )
                   ? bounds.Bottom - negativeOffsetYToZero - pointInPixelsY
                   : bounds.Y + positiveOffsetYToZero + pointInPixelsY;
        pixelsDataPoints.Add( new Point( bounds.Left + pointInPixelsX, posY ) );
      }

      return pixelsDataPoints;
    }

    #endregion
  }
}
