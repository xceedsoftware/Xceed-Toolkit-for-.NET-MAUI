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
  public class LineRenderer : RendererBase
  {
    #region Internal Members

    internal const string DefaultLineRendererTemplateName = "LineRendererTemplate";
    internal const string DefaultLineRendererDataPointMarkerTemplateName = "DataPointMarkerTemplate";

    #endregion

    #region Private Members

    private SeriesLine m_seriesLine;
    private readonly List<DataPointMarker> m_dataPointMarkers = new List<DataPointMarker>();
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

    #region DefaultDataPointMarkerTemplate

    public static DataTemplate DefaultDataPoinMarkerTemplate
    {
      get;
      internal set;
    }

    #endregion

    #region StrokeThickness

    public static readonly BindableProperty StrokeThicknessProperty = BindableProperty.Create( nameof( StrokeThickness ), typeof( double ), typeof( BarRenderer ), 0d, propertyChanged: OnStrokeThicknessChanged );

    public double StrokeThickness
    {
      get => ( double )GetValue( StrokeThicknessProperty );
      set => SetValue( StrokeThicknessProperty, value );
    }

    private static void OnStrokeThicknessChanged( BindableObject bindable, object oldValue, object newValue )
    {
      var lineRenderer = bindable as LineRenderer;
      if( lineRenderer != null )
      {
        lineRenderer.OnStrokeThicknessChanged( (double)oldValue, (double)newValue );
      }
    }

    protected virtual void OnStrokeThicknessChanged( double oldValue, double newValue )
    {
      if( newValue < 0 )
        throw new InvalidDataException( "lineRenderer's strokeThickness must be greater than 0." );

      if( (m_seriesLine != null) && (newValue != 0d) )
      {
        m_seriesLine.SetStrokeThickness( newValue );

        this.Refresh();
      }
    }

    #endregion

    #endregion

    #region Public Methods

    public override IList<AreaElement> CreateVisualChildren( Series series )
    {
      if( series == null )
        throw new ArgumentNullException( nameof( series ) );

      m_dataPointMarkers.Clear();
      m_dataPointLabels.Clear();
      m_dataPointLabelLines.Clear();
      m_seriesLine = null;

      var elements = new List<AreaElement>();

      // Add the line, which will connect dataPoints.
      m_seriesLine = new SeriesLine();
      m_seriesLine.SetBinding( SeriesLine.ContentTemplateProperty, "Template" );
      m_seriesLine.Infos.Background = RendererBase.GetSeriesBackground( series );
      m_seriesLine.Content = m_seriesLine.Infos;
      m_seriesLine.BindingContext = series;

      elements.Add( m_seriesLine );

      for( int i = 0; i < series.DataPoints.Count; ++i )
      {
        this.CreateDataPointVisual( series, elements );
      }

      return elements;
    }

    public override void ApplyDefaultTemplate( Series series )
    {
      if( series == null )
        throw new ArgumentNullException( nameof( series ) );

      if( ( series.Template == null ) || series.IsUsingDefaultTemplate() )
      {
        series.Template = LineRenderer.DefaultTemplate;
      }
      if( series.DataPointMarkerTemplate == null )
      {
        series.DataPointMarkerTemplate = LineRenderer.DefaultDataPoinMarkerTemplate;
      }
    }

    public override void ClearDataPointVisuals( Series series )
    {
      if( series == null )
        throw new ArgumentNullException( nameof( series ) );

      m_dataPointMarkers.Clear();

      if( series.ShowDataPointLabels )
      {
        m_dataPointLabelLines.Clear();

        m_dataPointLabels.Clear();
      }
    }

    public override void InitializeSeries( Rect bounds, Series series )
    {
      if( series == null )
        throw new ArgumentNullException( nameof( series ) );

      Debug.Assert( series.DataPoints.Count == m_dataPointMarkers.Count, "The SeriesLine should have the same number of points as the DataPoint's marker count." );

      var pixelDataPoints = LineRenderer.GetDataPointsInPixels( bounds, series );

      // Order the DataPoints.
      var orderedDataPoints = series.DataPoints.ToList();
      orderedDataPoints.Sort( new DataPointComparer() );

      if( series.ShowDataPointLabels )
      {
        Debug.Assert( series.DataPoints.Count == m_dataPointLabels.Count, "The SeriesLine should have the same number of points as the DataPoint's label count." );
        Debug.Assert( series.DataPoints.Count == m_dataPointLabelLines.Count, "The SeriesLine should have the same number of points as the DataPoint label's line count." );
      }

      for( int i = 0; i < pixelDataPoints.Count; ++i )
      {
        var isPositiveOrZeroValue = ( ( orderedDataPoints[ i ].InternalY >= 0 ) && ( series.Chart.DataPointRange[ 1 ].Start >= 0 ) )
                                    || ( ( orderedDataPoints[ i ].InternalY >= 0 ) && ( series.Chart.DataPointRange[ 1 ].End >= 0 ) )
                                    || ( ( orderedDataPoints[ i ].InternalY < 0 ) && ( series.Chart.DataPointRange[ 1 ].Start >= 0 ) );
        var pixelDataPoint = new Point( pixelDataPoints[ i ].X, pixelDataPoints[ i ].Y );

        m_dataPointMarkers[ i ].SetLocation( pixelDataPoint );

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
      }

      Debug.Assert( m_seriesLine != null, "Can't set Line's Points because m_seriesLine is not defined." );

      if( this.StrokeThickness != 0d )
      {
        m_seriesLine.SetStrokeThickness( this.StrokeThickness );
      }
      m_seriesLine.SetPoints( m_dataPointMarkers );
      m_seriesLine.SetLocation( bounds, Point.Zero );
    }

    #endregion

    #region Internal Methods

    protected internal override void CreateDataPointVisual( Series series, List<AreaElement> elements )
    {
      if( series == null )
        throw new ArgumentNullException( nameof( series ) );

      if( elements == null )
        throw new ArgumentNullException( nameof( elements ) );

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

      // Add the DataPoint's markers.
      var dataPointMarker = new DataPointMarker();
      dataPointMarker.SetBinding( DataPointMarker.ContentTemplateProperty, "DataPointMarkerTemplate" );
      dataPointMarker.Content = dataPointMarker.Infos;
      dataPointMarker.BindingContext = series;

      m_dataPointMarkers.Add( dataPointMarker );
      elements.Add( dataPointMarker );
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

      if( index >= m_dataPointMarkers.Count )
        throw new InvalidDataException( "Can't remove element at position " + index + " for DataPointMarker." );

      elements.Add( m_dataPointMarkers[ index ] );
      m_dataPointMarkers.RemoveAt( index );

      if( series.ShowDataPointLabels )
      {
        elements.Add( m_dataPointLabelLines[ index ] );
        m_dataPointLabelLines.RemoveAt( index );

        elements.Add( m_dataPointLabels[ index ] );
        m_dataPointLabels.RemoveAt( index );
      }
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
      if( series.Chart.VerticalAxis.IsUsingCustomRange() )
      {
        if( minDataPointY < series.Chart.VerticalAxis.CustomRange.Start )
        {
          minDataPointY = ( minDataPointY > 0 ) ? series.Chart.VerticalAxis.CustomRange.Start : Math.Min( series.Chart.VerticalAxis.CustomRange.Start, minDataPointY );
        }
        if( maxDataPointY > series.Chart.VerticalAxis.CustomRange.End )
        {
          maxDataPointY = ( maxDataPointY < 0 ) ? series.Chart.VerticalAxis.CustomRange.End : Math.Max( series.Chart.VerticalAxis.CustomRange.End, maxDataPointY );
        }
      }
    }

    private static List<Point> GetDataPointsInPixels( Rect bounds, Series series )
    {
      if( series == null )
        throw new ArgumentNullException( nameof( series ) );

      var dataPoints = series.DataPoints.ToList();
      var XAxisLength = bounds.Width;
      var YAxisLength = bounds.Height;
      LineRenderer.GetMinMaxDataPoints( series, out double minDataPointX, out double maxDataPointX, out double minDataPointY, out double maxDataPointY );
      dataPoints.Sort( new DataPointComparer() );

      var pixelsDataPoints = new List<Point>();

      for( int i = 0; i < dataPoints.Count; ++i )
      {
        var point = new Point( dataPoints[ i ].InternalX, dataPoints[ i ].InternalY );

        var pointPercentX = ( ( point.X - minDataPointX ) / Math.Max( 1, ( maxDataPointX - minDataPointX ) ) );
        var pointPercentY = ( ( point.Y - minDataPointY ) / Math.Max( 1, ( maxDataPointY - minDataPointY ) ) );

        var pointInPixelsX = pointPercentX * XAxisLength;
        var pointInPixelsY = pointPercentY * YAxisLength;

        pixelsDataPoints.Add( new Point( bounds.Left + pointInPixelsX, bounds.Bottom - pointInPixelsY ) );
      }

      return pixelsDataPoints;
    }

    private void Refresh()
    {
      var current = m_seriesLine.Parent;
      while( current != null )
      {
        if( current is Chart chart )
        {
          chart.Refresh( false );
          break;
        }

        current = current.Parent;
      }
    }

    #endregion
  }
}
