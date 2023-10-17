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
  public enum Orientation
  {
    Horizontal,
    Vertical
  }

  internal class AxisPanel : Layout
  {
    #region Private Members

    private readonly Axis m_axis;
    private readonly List<Tick> m_ticks = new List<Tick>();
    private readonly List<GridLine> m_gridLines = new List<GridLine>();
    private IRange m_otherAxisDataPointRange;

    #endregion

    #region Constructors

    internal AxisPanel( Axis axis )
    {
      m_axis = axis ?? throw new ArgumentNullException( nameof( axis ) ); ;
    }

    #endregion

    #region Internal Properties

    #region Axis

    internal Axis Axis
    {
      get
      {
        return m_axis;
      }
    }

    #endregion

    #endregion

    #region Protected Methods

    protected override ILayoutManager CreateLayoutManager()
    {
      return new AxisPanelLayoutManager( this );
    }

    #endregion

    #region Internal Methods

    internal void CreateVisualChildren( IRange otherAxisDataPointRange )
    {
      if( otherAxisDataPointRange.End <= otherAxisDataPointRange.Start )
        throw new InvalidDataException( "Axis DataPointRange must have its End value greater than its Start value." );

      m_ticks.Clear();
      m_gridLines.Clear();
      this.Children.Clear();

      m_otherAxisDataPointRange = otherAxisDataPointRange;

      // The Axis Line.
      if( m_axis.ShowAxis )
      {
        m_axis.Content = m_axis.Infos;
        this.Children.Add( m_axis );
      }

      var tickCount = m_axis.GetInternalTickCount();
      for( int i = 0; i < tickCount; ++i )
      {
        if( m_axis.ShowTicks )
        {
          // The Tick marks.
          this.AddVisualTick();
        }

        if( m_axis.ShowGridLines )
        {
          // The Grid lines.
          this.AddVisualGridLine();
        }
      }
    }

    internal void SetupAxisPanel( Rect bounds )
    {
      this.SetAxis();

      this.SetAxisPoints( bounds );

      this.SetTicks( bounds );
    }

    internal double GetHalfTickLength()
    {
      return ( m_ticks.Count > 0 ) ? m_ticks[ 0 ].Length / 2 : 0d;
    }

    #endregion

    #region Private Methods   

    private void AddVisualTick()
    {
      var tickOrientation = ( m_axis.Orientation == Orientation.Horizontal ) ? Orientation.Vertical : Orientation.Horizontal;

      var tick = new Tick( tickOrientation );
      tick.SetBinding( Tick.ContentTemplateProperty, "TickTemplate" );
      tick.SetBinding( Tick.LengthProperty, "TickLength" );
      tick.Content = tick.Infos;
      tick.BindingContext = m_axis;

      m_ticks.Add( tick );
      this.Children.Add( tick );
    }

    private void RemoveVisualTick()
    {
      var tick = m_ticks.Last();
      tick.RemoveBinding( Tick.ContentTemplateProperty );
      tick.RemoveBinding( Tick.LengthProperty );
      tick.Content = null;
      tick.BindingContext = null;

      this.Children.Remove( tick );
      m_ticks.Remove( tick );
    }

    private void AddVisualGridLine()
    {
      var tickOrientation = ( m_axis.Orientation == Orientation.Horizontal ) ? Orientation.Vertical : Orientation.Horizontal;

      var gridLine = new GridLine( tickOrientation );
      gridLine.SetBinding( GridLine.ContentTemplateProperty, "GridLineTemplate" );
      gridLine.Content = gridLine.Infos;
      gridLine.ZIndex = -1;
      gridLine.BindingContext = m_axis;

      m_gridLines.Add( gridLine );
      this.Children.Add( gridLine );
    }

    private void RemoveVisualGridLine() 
    {
      var gridLine = m_gridLines.Last();
      gridLine.RemoveBinding( GridLine.ContentTemplateProperty );
      gridLine.Content = null;
      gridLine.BindingContext = null;

      this.Children.Remove( gridLine );
      m_gridLines.Remove( gridLine );
    }

    private void SetAxis()
    {
      var axis = this.Children.OfType<Axis>().FirstOrDefault();

      // Missing the Axis.
      if( m_axis.ShowAxis && ( axis == null ) )
      {
        m_axis.Content = m_axis.Infos;
        this.Children.Add( m_axis );
      }
      // Need to add the Axis.
      else if( !m_axis.ShowAxis && ( axis != null ) )
      {
        this.Children.Remove( axis );
      }
    }

    private void SetAxisPoints( Rect bounds )
    {
      if( !m_axis.ShowAxis )
        return;

      var areaPanel = this.Parent as AreaPanel;
      Debug.Assert( areaPanel != null, "AxisPanel can't find its areaPanel parent." );

      var axisPanelOffset = ( m_axis.Orientation == Orientation.Horizontal )
                            ? areaPanel.AxisOffset.Left
                            : areaPanel.AxisOffset.Bottom;
      var axisOffsetFromBottomLeft = this.GetOffsetFromBottomLeft( bounds );
      if( axisOffsetFromBottomLeft == 0d )
      {
        axisOffsetFromBottomLeft = this.GetHalfTickLength();
      }

      m_axis.SetPoints( bounds, axisPanelOffset );
      m_axis.SetLocation( bounds, new Point( axisPanelOffset, axisOffsetFromBottomLeft ) );
    }

    private void SetTicks( Rect bounds )
    {
      var tickCount = m_axis.GetInternalTickCount();

      if( m_axis.ShowTicks )
      {
        var diff = tickCount - m_ticks.Count;
        // Missing Ticks.
        if( diff > 0 )
        {
          for( int i = 0; i < diff; ++i )
          {
            this.AddVisualTick();
          }
        }
        // Too many Ticks.
        else if( diff < 0 )
        {
          for( int i = 0; i < -diff; ++i )
          {
            this.RemoveVisualTick();
          }
        }

        Debug.Assert( m_ticks.Count == tickCount, "Can't set Tick's because m_ticks.Count doesn't equals m_axis.TickCount." );
      }
      else 
      {
        var ticks = this.Children.OfType<Tick>().Count();
        for( int i = 0; i < ticks; ++i )
        {
          this.RemoveVisualTick();
        }
      }

      if( m_axis.ShowGridLines )
      {
        var diff = tickCount - m_gridLines.Count;
        // Missing GridLines.
        if( diff > 0 )
        {
          for( int i = 0; i < diff; ++i )
          {
            this.AddVisualGridLine();
          }
        }
        // Too many GridLines.
        else if( diff < 0 )
        {
          for( int i = 0; i < -diff; ++i )
          {
            this.RemoveVisualGridLine();
          }
        }

        Debug.Assert( m_gridLines.Count == tickCount, "Can't set GridLine's because m_gridLines.Count doesn't equals m_axis.TickCount." );
      }
      else
      {
        var gridLines = this.Children.OfType<GridLine>().Count();
        for( int i = 0; i < gridLines; ++i )
        {
          this.RemoveVisualGridLine();
        }
      }

      var areaPanel = this.Parent as AreaPanel;
      Debug.Assert( areaPanel != null, "AxisPanel can't find its areaPanel parent." );

      // A BarChart could have a SeriesBar overlapping left or right of Chart.
      var maxSeriesLeftOverlap = ( ( m_axis.Chart != null ) && ( m_axis.Orientation == Orientation.Horizontal ) )
                                 ? m_axis.Chart.Series.Max( series => series.GetLeftOverlap() )
                                 : 0d;
      var maxSeriesRightOverlap = ( ( m_axis.Chart != null ) && ( m_axis.Orientation == Orientation.Horizontal ) )
                                 ? m_axis.Chart.Series.Max( series => series.GetRightOverlap() )
                                 : 0d;

      var axisPanelOffset = ( m_axis.Orientation == Orientation.Horizontal )
                            ? areaPanel.AxisOffset.Left + maxSeriesLeftOverlap
                            : areaPanel.AxisOffset.Bottom;
      var offsetFromBottomLeft = this.GetOffsetFromBottomLeft( bounds );
      if( offsetFromBottomLeft != 0 )
      {
        offsetFromBottomLeft -= this.GetHalfTickLength();
      }

      var tickOffset = axisPanelOffset;
      var pixelStep = this.GetPixelAxisStep( bounds, axisPanelOffset, maxSeriesRightOverlap, tickCount );

      for( int i = 0; i < tickCount; ++i )
      {
        // Ticks
        if( m_axis.ShowTicks )
        {
          var tick = m_ticks[ i ];
          tick.SetPoints( bounds, 0 );
          tick.SetLocation( bounds, new Point( tickOffset, offsetFromBottomLeft ) );
        }

        if( m_axis.ShowGridLines )
        {
          // GridLines
          var gridLine = m_gridLines[ i ];
          gridLine.SetPoints( bounds, 0 );
          gridLine.SetLocation( bounds, new Point( tickOffset, 0 ) );
        }

        tickOffset += pixelStep;
      }
    }

    private double GetOffsetFromBottomLeft( Rect bounds )
    {
      var offset = 0d;

      // A BarChart could have a SeriesBar overlapping left of Chart.
      if( ( m_axis.Chart != null ) 
        && ( m_axis.Orientation == Orientation.Vertical )
        && !m_axis.Chart.HorizontalAxis.IsUsingTickLabel() )
      {
        offset += m_axis.Chart.Series.Max( series => series.GetLeftOverlap() );
      }

      if( m_otherAxisDataPointRange.Start >= 0 )
        return offset;
      if( m_otherAxisDataPointRange.End < 0 )
        return offset;

      var boundSize = ( m_axis.Orientation == Orientation.Horizontal ) ? bounds.Height : bounds.Width;
      return -m_otherAxisDataPointRange.Start / ( m_otherAxisDataPointRange.End - m_otherAxisDataPointRange.Start ) * boundSize;
    }   

    private double GetPixelAxisStep( Rect bounds, double offset, double rightMargin, uint tickCount )
    {
      if( tickCount < 2 )
        return 0d;

      var axisLength = ( m_axis.Orientation == Orientation.Horizontal )
                       ? bounds.Right - bounds.Left - offset - rightMargin
                       : bounds.Bottom - bounds.Top - offset;

      return ( axisLength / ( tickCount - 1 ) );
    }

    #endregion
  }

  #region Internal Class AxisPanelLayoutManager

  internal class AxisPanelLayoutManager : ILayoutManager
  {
    #region Private Members

    private readonly AxisPanel m_axisPanel;

    #endregion

    #region Constructors

    internal AxisPanelLayoutManager( AxisPanel axisPanel )
    {
      m_axisPanel = axisPanel ?? throw new ArgumentNullException( nameof( axisPanel ) );
    }

    #endregion

    #region Public Methods

    public Size ArrangeChildren( Rect bounds )
    {
      for( int i = 0; i < m_axisPanel.Count; ++i )
      {
        var child = m_axisPanel[ i ];

        var element = child as AreaElement;
        var destination = ( element != null )
                          ? new Rect( element.Infos.Location.X, element.Infos.Location.Y, child.DesiredSize.Width, child.DesiredSize.Height )
                          : new Rect( 0, 0, child.DesiredSize.Width, child.DesiredSize.Height );

        child.Arrange( destination );
      }

      return new Size( bounds.Width, bounds.Height );
    }

    public Size Measure( double widthConstraint, double heightConstraint )
    {
      if( widthConstraint == double.PositiveInfinity || heightConstraint == double.PositiveInfinity )
        throw new InvalidDataException( "Can't measure AxisPanel with Infinity.");

      m_axisPanel.SetupAxisPanel( new Rect( 0, 0, widthConstraint, heightConstraint ) );

      for( int i = 0; i < m_axisPanel.Count; ++i )
      {
        var child = m_axisPanel[ i ];

        child.Measure( widthConstraint, heightConstraint );
      }

      return new Size( widthConstraint, heightConstraint );
    }

    #endregion
  }

  #endregion
}
