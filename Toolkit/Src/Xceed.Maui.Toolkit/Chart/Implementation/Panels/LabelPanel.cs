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
  public class LabelPanel : Layout
  {
    #region Private Members

    private readonly IList<TickLabel> m_tickLabels = new List<TickLabel>();
    private TitleLabel m_titleLabel;

    #endregion

    #region Internal Properties

    #region Axis

    internal Axis Axis
    {
      get;
      private set;
    }

    #endregion

    #endregion

    #region Protected Methods

    protected override ILayoutManager CreateLayoutManager()
    {
      return new LabelPanelLayoutManager( this );
    }

    #endregion

    #region Internal Methods

    internal void CreateVisualChildren( Axis axis )
    {
      if( axis == null )
        throw new ArgumentNullException( nameof( axis ) );

      var tickCount = axis.GetInternalTickCount();
      if( tickCount <= 1 )
        throw new InvalidDataException( "axis.TickCount must contains at least 2 Ticks." );

      this.Axis = axis;

      m_tickLabels.Clear();
      this.Children.Clear();

      if( this.Axis.ShowTickLabels )
      {
        for( int i = 0; i < tickCount; ++i )
        {
          // The Tick Labels.
          this.AddVisualTickLabel();
        }
      }

      if( this.Axis.ShowTitleLabel && ( this.Axis.Title != null ) )
      {
        // The axis title label.
        this.AddVisualTitleLabel();
      }
    }

    internal void SetupTickLabels()
    {
      if( this.Axis == null )
        return;

      if( !this.Axis.ShowTickLabels )
      {
        var tickLabels = this.Children.OfType<TickLabel>().Count();
        for( int i = 0; i < tickLabels; ++i )
        {
          this.RemoveVisualTickLabel();
        }
        return;
      }

      var tickCount = this.Axis.GetInternalTickCount();

      var diff = tickCount - m_tickLabels.Count;
      // Missing TickLabels.
      if( diff > 0 )
      {
        for( int i = 0; i < diff; ++i )
        {
          this.AddVisualTickLabel();
        }
      }
      // Too many TickLabels.
      else if( diff < 0 )
      {
        for( int i = 0; i < -diff; ++i )
        {
          this.RemoveVisualTickLabel();
        }
      }

      if( this.Axis.IsUsingTickLabel() )
      {
        var allDataPoints = this.Axis.GetAllDistinctDataPoints();

        for( var i = 0; i < allDataPoints.Count; ++i )
        {
          var tickLabel = m_tickLabels[ i ];
          ( tickLabel.Infos as LabelElementInfos ).Text = allDataPoints[ i ].Text;
        }
      }
      else
      {
        var axisDataPointRange = this.GetAxisDataPointRange();

        var dataPointStep = ( axisDataPointRange.End - axisDataPointRange.Start ) / ( tickCount - 1 );
        var startValue = axisDataPointRange.Start;

        for( var i = 0; i < tickCount; ++i )
        {
          var tickLabel = m_tickLabels[ i ];
          ( tickLabel.Infos as LabelElementInfos ).Text = Math.Round( startValue + i * dataPointStep, 2 ).ToString();
        }
      }
    }

    internal void SetupTitleLabel()
    {
      if( this.Axis == null )
        return;

      if( this.Axis.ShowTitleLabel && ( m_titleLabel == null ) && !string.IsNullOrEmpty( this.Axis.Title ) )
      {
        // The axis title label.
        this.AddVisualTitleLabel();
      }
      else if( !this.Axis.ShowTitleLabel && ( m_titleLabel != null ) )
      {
        this.RemoveVisualTitleLabel();
        return;
      }

      if( m_titleLabel != null )
      {
        ( m_titleLabel.Infos as LabelElementInfos ).Text = this.Axis.Title;
      }
    }

    #endregion

    #region Private Methods

    private void AddVisualTickLabel()
    {
      Debug.Assert( this.Axis != null, "Axis should n't be null when adding TickLabel." );

      var tickLabel = new TickLabel( this.Axis.Orientation );
      tickLabel.SetBinding( TickLabel.ContentTemplateProperty, "TickLabelTemplate" );
      tickLabel.Content = tickLabel.Infos;
      tickLabel.BindingContext = this.Axis;

      m_tickLabels.Add( tickLabel );
      this.Children.Add( tickLabel );
    }

    private void RemoveVisualTickLabel()
    {
      var tickLabel = m_tickLabels.Last();
      tickLabel.RemoveBinding( TickLabel.ContentTemplateProperty );
      tickLabel.Content = null;
      tickLabel.BindingContext = null;

      this.Children.Remove( tickLabel );
      m_tickLabels.Remove( tickLabel );
    }

    private void AddVisualTitleLabel()
    {
      var titleLabel = new TitleLabel( this.Axis.Orientation );
      titleLabel.SetBinding( TitleLabel.ContentTemplateProperty, "TitleLabelTemplate" );
      titleLabel.Content = titleLabel.Infos;
      titleLabel.AnchorX = 0;
      titleLabel.AnchorY = 1;
      titleLabel.Rotation = ( this.Axis.Orientation == Orientation.Vertical ) ? 90 : 0;
      titleLabel.BindingContext = this.Axis;

      m_titleLabel = titleLabel;
      this.Children.Add( titleLabel );
    }

    private void RemoveVisualTitleLabel()
    {
      if( m_titleLabel != null )
      {
        m_titleLabel.RemoveBinding( TickLabel.ContentTemplateProperty );
        m_titleLabel.Content = null;
        m_titleLabel.BindingContext = null;

        this.Children.Remove( m_titleLabel );
        m_titleLabel = null;
      }
    }

    private IRange GetAxisDataPointRange()
    {
      if( ( this.Axis == null ) || ( this.Axis.Chart == null ) )
        return Range.Zero;

      return this.Axis.IsUsingCustomRange()
              ? this.Axis.CustomRange
              : ( this.Axis.Orientation == Orientation.Horizontal ) ? this.Axis.Chart.DataPointRange[ 0 ] : this.Axis.Chart.DataPointRange[ 1 ];
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

    #region Internal Class LabelPanelLayoutManager

    internal class LabelPanelLayoutManager : ILayoutManager
    {
      #region Private Members

      private readonly LabelPanel m_labelPanel;

      #endregion

      #region Constructors

      internal LabelPanelLayoutManager( LabelPanel labelPanel )
      {
        m_labelPanel = labelPanel ?? throw new ArgumentNullException( nameof( labelPanel ) );
      }

      #endregion

      #region Public Methods

      public Size ArrangeChildren( Rect bounds )
      {
        if( (m_labelPanel.Axis != null) && (m_labelPanel.Axis.Chart.Series.Count() > 0) )
        {
          var maxSeriesLeftOverlap = ( ( m_labelPanel.Axis.Chart != null ) && ( m_labelPanel.Axis.Orientation == Orientation.Horizontal ) )
                                    ? m_labelPanel.Axis.Chart.Series.Max( series => series.GetLeftOverlap() )
                                    : 0d;
          var maxSeriesRightOverlap = ( ( m_labelPanel.Axis.Chart != null ) && ( m_labelPanel.Axis.Orientation == Orientation.Horizontal ) )
                                      ? m_labelPanel.Axis.Chart.Series.Max( series => series.GetRightOverlap() )
                                      : 0d;

          var offset = maxSeriesLeftOverlap;

          var axisLength = ( m_labelPanel.Axis.Orientation == Orientation.Horizontal )
                            ? bounds.Right - bounds.Left - maxSeriesLeftOverlap - maxSeriesRightOverlap
                            : bounds.Bottom - bounds.Top;
          var pixelStep = axisLength / ( m_labelPanel.Axis.GetInternalTickCount() - 1 );

          foreach( var child in m_labelPanel )
          {
            // Arrange the Tick labels and the Title label.
            var element = child as AreaElement;
            if( element != null )
            {
              element.SetLocation( bounds, ( child is TitleLabel ) ? Point.Zero : new Point( offset, 0 ) );
            }

            var destination = ( element != null )
                              ? new Rect( element.Infos.Location.X, element.Infos.Location.Y, element.DesiredSize.Width, element.DesiredSize.Height )
                              : new Rect( 0, 0, child.DesiredSize.Width, child.DesiredSize.Height );

            child.Arrange( destination );

            if( !( child is TitleLabel ) )
            {
              offset += pixelStep;
            }
          }
        }

        return new Size( bounds.Width, bounds.Height );
      }

      public Size Measure( double widthConstraint, double heightConstraint )
      {
        if( m_labelPanel.Count == 0 )
          return new Size( 0, 0 );

        m_labelPanel.SetupTickLabels();
        m_labelPanel.SetupTitleLabel();

#if WINDOWS
        Chart parentChart = null;
        if( double.IsInfinity( widthConstraint ) )
        {
          parentChart = m_labelPanel.GetParentChart();
          widthConstraint = ( parentChart != null ) ? parentChart.Width : widthConstraint;
        }
        if( double.IsInfinity( heightConstraint ) )
        {
          parentChart = (parentChart == null) ? m_labelPanel.GetParentChart() : parentChart;
          heightConstraint = ( parentChart != null ) ? parentChart.Height : heightConstraint;
        }
#endif

        var maxValue = 0d;
        for( int i = 0; i < m_labelPanel.Count; ++i )
        {
          var child = m_labelPanel[ i ];
          var isTitleLabel = child is TitleLabel;

          var size = child.Measure( isTitleLabel ? double.PositiveInfinity : widthConstraint, heightConstraint );

          if( isTitleLabel )
          {
            maxValue += size.Height;
          }
          else
          {
            maxValue = Math.Max( maxValue, ( m_labelPanel.Axis.Orientation == Orientation.Horizontal ) ? size.Height : size.Width );
          }
        }

        return ( m_labelPanel.Axis.Orientation == Orientation.Horizontal )
                ? new Size( widthConstraint, maxValue )
                : new Size( maxValue, heightConstraint );
      }

      #endregion
    }

    #endregion
  }
}
