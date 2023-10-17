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


using Microsoft.Maui.Layouts;
using System.Diagnostics;

namespace Xceed.Maui.Toolkit
{
  internal class SeriesPanel : Layout
  {
    #region Constructors

    internal SeriesPanel( Series series )
    {
      this.Series = series;
    }

    #endregion

    #region Internal Properties

    #region Series

    internal Series Series 
    {
      get;
      private set;
    }

    #endregion

    #endregion

    #region Protected Methods

    protected override ILayoutManager CreateLayoutManager()
    {
      return new SeriesPanelLayoutManager( this );
    }

    #endregion

    #region Internal Methods

    internal void CreateVisualChildren()
    {
      this.Children.Clear();

      var elements = this.Series.Renderer.CreateVisualChildren( this.Series );
      foreach ( var element in elements ) 
      {
        this.Children.Add( element );
      }
    }

    internal void AddDataPointVisual()
    {
      var elements = this.Series.Renderer.AddDataPointVisual( this.Series );
      if( elements == null )
        return;

      foreach( var element in elements )
      {
        this.Children.Add( element );
      }
    }

    internal void RemoveDataPointVisual( int index )
    {
      var elements = this.Series.Renderer.RemoveDataPointVisual( this.Series, index );
      if( elements == null )
        return;

      foreach( var element in elements )
      {
        this.Children.Remove( element );
      }
    }

    internal void ClearDataPointVisuals()
    {
      this.Series.Renderer.ClearDataPointVisuals( this.Series );

      this.Children.Clear();
    }

    internal void InitializeSeries( Rect bounds )
    {
      this.Series.Renderer.InitializeSeries( bounds, this.Series );
    }

    internal Rect GetSeriesBounds( double widthConstraint, double heightConstraint )
    {
      Debug.Assert( this.Series.Chart != null, "Series.Chart shouldn't be null when calculating series dataPoint range." );

      var x = 0d;
      var y = 0d;
      var width = 0d;
      var height = 0d;
      if( ( this.Series.DataPointRange.Count == 2 ) && ( this.Series.Chart.DataPointRange.Count == 2 ) )
      {
        var seriesDataPointRangeX = this.Series.DataPointRange[ 0 ];
        var seriesDataPointRangeY = this.Series.DataPointRange[ 1 ];

        if( ( seriesDataPointRangeX.Start == seriesDataPointRangeX.End )
          || ( seriesDataPointRangeY.Start == seriesDataPointRangeY.End ) )
          return new Rect( x, y, width, height );

        var chartRangeX = this.Series.Chart.HorizontalAxis.IsUsingCustomRange()
                          ? this.Series.Chart.HorizontalAxis.CustomRange
                          : this.Series.Chart.DataPointRange[ 0 ];
        var chartRangeY = this.Series.Chart.VerticalAxis.IsUsingCustomRange()
                          ? this.Series.Chart.VerticalAxis.CustomRange
                          : this.Series.Chart.DataPointRange[ 1 ];

        if( seriesDataPointRangeX.End <= seriesDataPointRangeX.Start )
          throw new InvalidDataException( "seriesDataPointRange in X must have its End value greater than its Start value." );
        if( seriesDataPointRangeY.End <= seriesDataPointRangeY.Start )
          throw new InvalidDataException( "seriesDataPointRange in Y must have its End value greater than its Start value." );
        if( chartRangeX.End <= chartRangeX.Start )
          throw new InvalidDataException( "chartDataPointRange in X must have its End value greater than its Start value." );
        if( chartRangeY.End <= chartRangeY.Start )
          throw new InvalidDataException( "chartDataPointRange in Y must have its End value greater than its Start value." );        

        x = this.Series.GetX( seriesDataPointRangeX, chartRangeX, widthConstraint );
        y = this.Series.GetY( seriesDataPointRangeY, chartRangeY, heightConstraint );

        width = this.Series.GetWidth( seriesDataPointRangeX, chartRangeX, widthConstraint );
        height = this.Series.GetHeight( seriesDataPointRangeY, chartRangeY, heightConstraint );
      }

      return new Rect( x, y, width, height );
    }

    #endregion    
  }

  #region Internal Class SeriesLayoutManager

  internal class SeriesPanelLayoutManager : ILayoutManager
  {
    #region Private Members

    private readonly SeriesPanel m_seriesPanel;

    #endregion

    #region Constructors

    internal SeriesPanelLayoutManager( SeriesPanel seriesPanel )
    {
      m_seriesPanel = seriesPanel ?? throw new ArgumentNullException( nameof( seriesPanel ) );
    }

    #endregion

    #region Public Methods

    public Size ArrangeChildren( Rect bounds )
    {
      for( int i = 0; i < m_seriesPanel.Count; ++i )
      {
        var child = m_seriesPanel[ i ];

        var destination = child is AreaElement element
                          ? new Rect( element.Infos.Location.X, element.Infos.Location.Y, child.DesiredSize.Width, child.DesiredSize.Height )
                          : new Rect( 0, 0, child.DesiredSize.Width, child.DesiredSize.Height );

        child.Arrange( destination );
      }

      return new Size( bounds.Width, bounds.Height );
    }

    public Size Measure( double widthConstraint, double heightConstraint )
    {
      m_seriesPanel.InitializeSeries( m_seriesPanel.GetSeriesBounds( widthConstraint, heightConstraint ) );

      for( int i = 0; i < m_seriesPanel.Count; ++i )
      {
        var child = m_seriesPanel[ i ];

        child.Measure( widthConstraint, heightConstraint );
      }

      return new Size( widthConstraint, heightConstraint );
    }

    #endregion
  }

  #endregion
}
