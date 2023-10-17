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


using System.Runtime.CompilerServices;

namespace Xceed.Maui.Toolkit
{
  public class AreaPanel : Grid
  {
    #region public Properties

    #region AxisMargin

    public static readonly BindableProperty AxisMarginProperty = BindableProperty.Create( nameof( AxisMargin ), typeof( Thickness ), typeof( AreaPanel ), Thickness.Zero );

    // Axis Margin based on DataPointLabels size (so the highest/lowest one do not goes out of View).
    public Thickness AxisMargin
    {
      get => ( Thickness )GetValue( AxisMarginProperty );
      private set => SetValue( AxisMarginProperty, value );
    }

    #endregion

    #region AxisOffset

    public static readonly BindableProperty AxisOffsetProperty = BindableProperty.Create( nameof( AxisOffset ), typeof( Thickness ), typeof( AreaPanel ), Thickness.Zero );

    // Axis Offset from Bottom/Left based on half-Tick size of Axis.
    public Thickness AxisOffset
    {
      get => ( Thickness )GetValue( AxisOffsetProperty );
      private set => SetValue( AxisOffsetProperty, value );
    }

    #endregion

    #endregion

    #region Internal Methods

    protected override void OnPropertyChanged( [CallerMemberName] string propertyName = null )
    {
      base.OnPropertyChanged( propertyName );

      if( ( propertyName == "Height" ) || ( propertyName == "Width" ) )
      {
        this.UpdatePanelsMargin();
      }
    }

    internal void CreateVisualChildren( Chart chart )
    {
      if( chart == null )
        throw new ArgumentNullException( nameof( chart ) );

      foreach( var children in this.Children )
      {
        var childrenLayout = children as Layout;
        if( childrenLayout != null )
        {
          childrenLayout.Clear();
        }
      }
      this.Children.Clear();

      if( chart.DataPointRange.Count == 0 )
        return;

      if( chart.VerticalAxis == null )
        return;
      if( chart.HorizontalAxis == null )
        return;

      var verticalAxisRange = chart.VerticalAxis.IsUsingCustomRange() ? chart.VerticalAxis.CustomRange : chart.DataPointRange[ 1 ];
      if( verticalAxisRange.Start == verticalAxisRange.End )
        return; 

      var horizontalAxisRange = chart.HorizontalAxis.IsUsingCustomRange() ? chart.HorizontalAxis.CustomRange : chart.DataPointRange[ 0 ];
      if( horizontalAxisRange.Start == horizontalAxisRange.End )
        return;

      // Create HorizontalAxisPanel and its children.
      var horizontalAxisPanel = new AxisPanel( chart.HorizontalAxis );
      horizontalAxisPanel.CreateVisualChildren( verticalAxisRange );
      this.Children.Add( horizontalAxisPanel );

      // Create VerticalAxisPanel and its children.
      var verticalAxisPanel = new AxisPanel( chart.VerticalAxis );
      verticalAxisPanel.CreateVisualChildren( horizontalAxisRange );
      this.Children.Add( verticalAxisPanel );

      // Create SeriesPanels and their children.
      this.CreateSeriesVisualChildren( chart.Series );

      this.AxisOffset = Thickness.Zero;
      this.AxisMargin = Thickness.Zero;
    }

    internal void CreateSeriesVisualChildren( IList<Series> seriesList )
    {
      if( seriesList == null || seriesList.Count == 0 )
        return;

      for( int i = 0; i < seriesList.Count; ++i )
      {
        var series = seriesList[ i ];
        series.Id = i;
        var seriesPanel = new SeriesPanel( series );
        seriesPanel.CreateVisualChildren();

        this.Children.Add( seriesPanel );
      }
    }

    internal void AddDataPointVisual( Series series )
    {
      var existingSeriesPanel = this.GetSeriesPanel( series );
      if( existingSeriesPanel != null )
      {
        existingSeriesPanel.AddDataPointVisual();
      }
    }

    internal void RemoveDataPointVisual( Series series, int index )
    {
      var existingSeriesPanel = this.GetSeriesPanel( series );
      if( existingSeriesPanel != null )
      {
        existingSeriesPanel.RemoveDataPointVisual( index );
      }
    }

    internal void ClearDataPointVisuals( Series series )
    {
      var existingSeriesPanel = this.GetSeriesPanel( series );
      if( existingSeriesPanel != null )
      {
        existingSeriesPanel.ClearDataPointVisuals();
      }
    }

    internal void UpdatePanelsMargin()
    {
      //if( this.AxisOffset != Thickness.Zero )
      //  return;

      var chart = this.GetParentChart();
      if( chart == null )
        return;

      var axisPanels = this.Children.OfType<AxisPanel>();
      if( !axisPanels.Any() )
        return;

      var maxSeriesTopOverlap = 0d;
      var maxSeriesBottomOverlap = 0d;
      var seriesWithShowDataPointLabels = chart.Series.Where( series => series.ShowDataPointLabels );
      if( seriesWithShowDataPointLabels.Any() )
      {
        maxSeriesTopOverlap = seriesWithShowDataPointLabels.Max( series => series.GetTopOverlap( this.GetDataPointLabelHeight( series ), this.Height ) );
        maxSeriesBottomOverlap = seriesWithShowDataPointLabels.Max( series => series.GetBottomOverlap( this.GetDataPointLabelHeight( series ), this.Height ) );
      }

      foreach( var axisPanel in axisPanels )
      {
        // Set Top/Bottom Margins for AxisPanels.
        // Useful when Series.ShowDataPointLabels is true to view the highest/Lowest DataPointLabel.
        axisPanel.Margin = new Thickness( 0, maxSeriesTopOverlap, 0, maxSeriesBottomOverlap );
      }

      var horizontalAxisPanel = this.GetHorizontalAxisPanel();
      var verticalAxisPanel = this.GetVerticalAxisPanel();
      if( ( horizontalAxisPanel == null ) || ( verticalAxisPanel == null ) )
        return;

      var verticalAxisRange = verticalAxisPanel.Axis.IsUsingCustomRange() ? verticalAxisPanel.Axis.CustomRange : chart.DataPointRange[ 1 ];
      if( verticalAxisRange.Start == verticalAxisRange.End )
        return;

      var horizontalAxisRange = horizontalAxisPanel.Axis.IsUsingCustomRange() ? horizontalAxisPanel.Axis.CustomRange : chart.DataPointRange[ 0 ];
      if( horizontalAxisRange.Start == horizontalAxisRange.End )
        return;

      var maxSeriesLeftOverlap = chart.Series.Max( series => series.GetLeftOverlap() );
      var maxSeriesRightOverlap = chart.Series.Max( series => series.GetRightOverlap() );

      // Offset for LabelPanels and SeriesPanel.
      var leftAxisOffset = ( ( horizontalAxisRange.Start < 0 )
                             || ( ( maxSeriesLeftOverlap > 0 ) && !horizontalAxisPanel.Axis.IsUsingTickLabel() ) )
                             ? 0d
                             : verticalAxisPanel.GetHalfTickLength();
      this.AxisOffset = new Thickness( leftAxisOffset,
                                     0d,
                                     0d,
                                     ( verticalAxisRange.Start < 0d ) ? 0d : horizontalAxisPanel.GetHalfTickLength() );

      this.AxisMargin = new Thickness( 0, maxSeriesTopOverlap, 0d, maxSeriesBottomOverlap );

      var areaPanel = chart.GetAreaPanel();
      horizontalAxisPanel.Measure( areaPanel.Width, areaPanel.Height );
      verticalAxisPanel.Measure( areaPanel.Width, areaPanel.Height );

      var seriesPanelList = new List<SeriesPanel>();
      chart.Series.ToList().ForEach( series =>
      {
        var seriesPanel = this.GetSeriesPanel( series );
        if( seriesPanel != null )
        {
          seriesPanelList.Add( seriesPanel );
        }
      } );

      foreach( var seriesPanel in seriesPanelList )
      {
        // SeriesPanel will use a margin based on the xAxis / yAxis offsets and the Max Series overlaps on each sides.
        seriesPanel.Margin = new Thickness( horizontalAxisPanel.Margin.Left + this.AxisOffset.Left + maxSeriesLeftOverlap,
                                            verticalAxisPanel.Margin.Top,
                                            horizontalAxisPanel.Margin.Right + maxSeriesRightOverlap,
                                            verticalAxisPanel.Margin.Bottom + this.AxisOffset.Bottom );
      }
    }

    internal SeriesPanel GetSeriesPanel( Series series )
    {
      if( series == null )
        return null;

      return this.Children.OfType<SeriesPanel>().FirstOrDefault( seriesPanel => ( seriesPanel.Series.Id == series.Id ) );
    }

    internal AxisPanel GetHorizontalAxisPanel()
    {
      return this.Children.OfType<AxisPanel>().FirstOrDefault( axisPanel => axisPanel.Axis.Orientation == Orientation.Horizontal );
    }

    internal AxisPanel GetVerticalAxisPanel()
    {
      return this.Children.OfType<AxisPanel>().FirstOrDefault( axisPanel => axisPanel.Axis.Orientation == Orientation.Vertical );
    }

    #endregion

    #region Private Methods   

    private double GetDataPointLabelHeight( Series series )
    {
      if( series == null )
        throw new ArgumentNullException( nameof( series ) );

      var dataPointLabel = new DataPointLabel()
      {
        ContentTemplate = series.DataPointLabelTemplate,
        Content = new DataPointLabelElementInfos() { DataPoint = new DataPoint() { X = 0, Y = 0 } }
      };

      this.Children.Add( dataPointLabel );

      var size = dataPointLabel.Measure( double.PositiveInfinity, double.PositiveInfinity );

      this.Children.Remove( dataPointLabel );

      return size.Request.Height;
    }

    private Chart GetParentChart()
    {
      var parent = this.Parent;
      while( parent != null )
      {
        if( parent is Chart )
          return parent as Chart;

        parent = parent.Parent;
      }

      return null;
    }

    #endregion
  }
}
