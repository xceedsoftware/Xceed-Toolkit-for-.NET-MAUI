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


using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;

namespace Xceed.Maui.Toolkit
{
  public class Chart : Control
  {
    #region Constants

    internal const string PART_HorizontalLabelPanel = "PART_HorizontalLabelPanel";
    private const string PART_VerticalLabelPanel = "PART_VerticalLabelPanel";
    private const string PART_AreaPanel = "PART_AreaPanel";

    #endregion

    #region Private Members

    private AreaPanel m_areaPanel;
    private LabelPanel m_horizontalLabelPanel;
    private LabelPanel m_verticalLabelPanel;
    private Axis m_horizontalAxis;
    private Axis m_verticalAxis;

    #endregion

    #region Constructors

    public Chart()
    {
      this.DataPointRange = new List<Range>();
      this.Series = new ObservableCollection<Series>();
      this.Series.CollectionChanged += this.Series_CollectionChanged;
      this.HorizontalAxis = new Axis( this );
      this.VerticalAxis = new Axis( this );
      this.Legend = new Legend();

      this.Loaded += this.Chart_Loaded;
    }

    #endregion

    #region Public Properties

    #region AreaBackground

    public static readonly BindableProperty AreaBackgroundProperty = BindableProperty.Create( nameof( AreaBackground ), typeof( Brush ), typeof( Chart ), null );

    public Brush AreaBackground
    {
      get => ( Brush )GetValue( AreaBackgroundProperty );
      set => SetValue( AreaBackgroundProperty, value );
    }

    #endregion

    #region HorizontalAxis

    public Axis HorizontalAxis
    {
      get
      {
        return m_horizontalAxis;
      }
      set 
      {
        m_horizontalAxis = value ?? throw new InvalidDataException( "The Chart must have a valid Horizontal Axis." );
        if( m_horizontalAxis != null )
        {
          m_horizontalAxis.SetOrientation( Orientation.Horizontal );
          m_horizontalAxis.Chart = this;

          // Slow but shouldn't happen too often. Need to redo a specific AxisPanel.
          this.Refresh();
        }
      }
    }

    #endregion

    #region Legend

    public static readonly BindableProperty LegendProperty = BindableProperty.Create( nameof( Legend ), typeof( Legend ), typeof( Chart ), null, propertyChanged: OnLegendChanged );

    public Legend Legend
    {
      get => ( Legend )GetValue( LegendProperty );
      set => SetValue( LegendProperty, value );
    }

    private static void OnLegendChanged( BindableObject bindable, object oldValue, object newValue )
    {
      if( bindable is Chart chart )
      {
        chart.OnLegendChanged( ( Legend )oldValue, ( Legend )newValue );
      }
    }

    protected virtual void OnLegendChanged( Legend oldValue, Legend newValue )
    {
      this.UpdateLegend();
    }

    #endregion

    #region Series

    public ObservableCollection<Series> Series
    {
      get;
    }

    #endregion

    #region ShowLegend

    public static readonly BindableProperty ShowLegendProperty = BindableProperty.Create( nameof( ShowLegend ), typeof( bool ), typeof( Chart ), false );

    public bool ShowLegend
    {
      get => ( bool )GetValue( ShowLegendProperty );
      set => SetValue( ShowLegendProperty, value );
    }

    #endregion

    #region VerticalAxis

    public Axis VerticalAxis
    {
      get
      {
        return m_verticalAxis;
      }
      set
      {
        m_verticalAxis = value ?? throw new InvalidDataException( "The Chart must have a valid Vertical Axis." ); 
        if( m_verticalAxis != null )
        {
          m_verticalAxis.SetOrientation( Orientation.Vertical );
          m_verticalAxis.Chart = this;

          // Slow but shouldn't happen too often. Need to redo a specific AxisPanel.
          this.Refresh();
        }
      }
    }

    #endregion

    #endregion

    #region Internal Properties

    #region DataPointRange

    internal IList<Range> DataPointRange
    {
      get;
    }

    #endregion

    #region DefaultSeriesBackgroundColor

    internal static Color[] DefaultSeriesBackgroundColor
    {
      get;
      private set;
    }


    #endregion

    #endregion

    #region Protected Methods

    protected override void OnApplyTemplate()
    {
      m_areaPanel = this.GetTemplateChild( Chart.PART_AreaPanel ) as AreaPanel;
      m_horizontalLabelPanel = this.GetTemplateChild( Chart.PART_HorizontalLabelPanel ) as LabelPanel;
      m_verticalLabelPanel = this.GetTemplateChild( Chart.PART_VerticalLabelPanel ) as LabelPanel;

      base.OnApplyTemplate();
    }

    #endregion

    #region Internal Methods

    internal void SetDataPointRange()
    {
      this.DataPointRange.Clear();

      var minX = ( this.Series.Count == 0 ) ? 0 : this.Series.Min( series => ( series.DataPoints.Count == 0 ) ? 0 : series.DataPoints.Min( dataPoint => dataPoint.InternalX ) );
      var maxX = ( this.Series.Count == 0 ) ? 0 : this.Series.Max( series => ( series.DataPoints.Count == 0 ) ? 0 : series.DataPoints.Max( dataPoint => dataPoint.InternalX ) );
      var minY = ( this.Series.Count == 0 ) ? 0 : this.Series.Min( series => ( series.DataPoints.Count == 0 ) ? 0 : series.DataPoints.Min( dataPoint => dataPoint.InternalY ) );
      var maxY = ( this.Series.Count == 0 ) ? 0 : this.Series.Max( series => ( series.DataPoints.Count == 0 ) ? 0 : series.DataPoints.Max( dataPoint => dataPoint.InternalY ) );

      if( ( this.HorizontalAxis != null ) && this.HorizontalAxis.IsUsingCustomRange() )
      {
        if( this.HorizontalAxis.IsUsingCustomStartRange() )
        {
          minX = Math.Max( minX, this.HorizontalAxis.CustomRange.Start );          
        }
        if( this.HorizontalAxis.IsUsingCustomEndRange() )
        {
          maxX = Math.Min( maxX, this.HorizontalAxis.CustomRange.End );
        }

        // Set automatic Range value.
        if( this.HorizontalAxis.CustomRange is AutomaticEndRange )
        {
          ( this.HorizontalAxis.CustomRange as AutomaticEndRange ).SetEnd( maxX );
        }
        else if( this.HorizontalAxis.CustomRange is AutomaticStartRange )
        {
          ( this.HorizontalAxis.CustomRange as AutomaticStartRange ).SetStart( minX );
        }
      }
      if( ( this.VerticalAxis != null ) && this.VerticalAxis.IsUsingCustomRange() )
      {
        if( this.VerticalAxis.IsUsingCustomStartRange() )
        {
          minY = Math.Max( minY, this.VerticalAxis.CustomRange.Start );
        }
        if( this.VerticalAxis.IsUsingCustomEndRange() )
        {
          maxY = Math.Min( maxY, this.VerticalAxis.CustomRange.End );
        }

        // Set automatic Range value.
        if( this.VerticalAxis.CustomRange is AutomaticEndRange )
        {
          ( this.VerticalAxis.CustomRange as AutomaticEndRange ).SetEnd( maxY );
        }
        else if( this.VerticalAxis.CustomRange is AutomaticStartRange )
        {
          ( this.VerticalAxis.CustomRange as AutomaticStartRange ).SetStart( minY );
        }
      }

      this.DataPointRange.Add( new Range( minX, maxX ) );
      this.DataPointRange.Add( new Range( minY, maxY ) );
    }

    internal void AddDataPointVisual( Series series )
    {
      if( m_areaPanel != null )
      {
        m_areaPanel.AddDataPointVisual( series );
      }
    }

    internal void RemoveDataPointVisual( Series series, int index )
    {
      if( m_areaPanel != null )
      {
        m_areaPanel.RemoveDataPointVisual( series, index );
      }
    }

    internal void ClearDataPointVisuals( Series series )
    {
      if( m_areaPanel != null )
      {
        m_areaPanel.ClearDataPointVisuals( series );
      }
    }

    internal AreaPanel GetAreaPanel()
    {
      return m_areaPanel;
    }

    internal LabelPanel GetHorizontalLabelPanel()
    {
      return m_horizontalLabelPanel;
    }

    internal void UpdateLegend()
    {
      if( !this.IsLoaded )
        return;

      if( ( this.Legend != null ) && ( this.Series != null ) )
      {
        var legendItems = new List<LegendItem>();

        foreach( var series in this.Series )
        {
          legendItems.Add( new LegendItem( series.Title, RendererBase.GetSeriesBackground( series ) ) );
        }

        this.Legend.ItemsSource = legendItems;
      }
    }

    internal void Refresh( bool refreshAll = true )
    {
      if( !this.IsLoaded )
        return;

      if( refreshAll )
      {
        if( m_areaPanel != null )
        {
          m_areaPanel.CreateVisualChildren( this );

          m_areaPanel.UpdatePanelsMargin();
        }

        this.UpdateLegend();

        if( this.DataPointRange.Count == 0 )
          return;

        Debug.Assert( this.DataPointRange.Count == 2, "Chart.DataPointRange must contains 2 Points." );

        if( ( this.DataPointRange[ 0 ].Start == this.DataPointRange[ 0 ].End )
          || ( this.DataPointRange[ 1 ].Start == this.DataPointRange[ 1 ].End ) )
          return;

        if(m_horizontalLabelPanel != null)
        {
          m_horizontalLabelPanel.CreateVisualChildren( this.HorizontalAxis );
        }

        if(m_verticalLabelPanel != null)
        {
          m_verticalLabelPanel.CreateVisualChildren( this.VerticalAxis );
        }
      }
      else
      {
        // force a refresh of the Axis panels and the Series panels. 
        if( m_areaPanel != null )
        {
          m_areaPanel.UpdatePanelsMargin();

          // Width and Height must not be infinity values because we need them to position visual elements.
          // Use + 1 to force a new Measure pass. Could not work if width/height are the same.
          m_areaPanel.Measure( m_areaPanel.Width + 1, m_areaPanel.Height + 1 );
        }

        if( this.DataPointRange.Count == 0 )
          return;

        Debug.Assert( this.DataPointRange.Count == 2, "Chart.DataPointRange must contains 2 Points." );

        if( ( this.DataPointRange[ 0 ].Start == this.DataPointRange[ 0 ].End )
          || ( this.DataPointRange[ 1 ].Start == this.DataPointRange[ 1 ].End ) )
          return;

        // force a refresh of the label panels.
        if( m_horizontalLabelPanel != null )
        {
          m_horizontalLabelPanel.Measure( m_horizontalLabelPanel.Width, m_horizontalLabelPanel.Height );
        }
        if( m_verticalLabelPanel != null )
        {
          m_verticalLabelPanel.Measure( m_verticalLabelPanel.Width, m_verticalLabelPanel.Height );
        }
      }
    }

    #endregion

    #region Private Methods

    #endregion

    #region Event Handlers

    private void Series_CollectionChanged( object sender, NotifyCollectionChangedEventArgs e )
    {
      if( e.Action == NotifyCollectionChangedAction.Add )
      {
        foreach( Series series in e.NewItems )
        {
          series.Chart = this;
          series.UpdateDataPointRange();
        }
      }

      this.SetDataPointRange();
      this.Refresh();
    }

    private void Chart_Loaded( object sender, EventArgs e )
    {
      this.LoadDefaultResources();

      foreach( var series in this.Series )
      {
        series.UpdateDataPointRange();
      }

      this.Refresh();
    }

    private void LoadDefaultResources()
    {
      Chart.DefaultSeriesBackgroundColor = this.LoadResource( "ChartSeriesDefaultBackgroundColor" ) as Color[];
      Debug.Assert( Chart.DefaultSeriesBackgroundColor != null, "Chart.ChartSeriesDefaultBackgroundColor should never be null." );

      // These DataTemplates will be used when we'll add Series to SeriesPanel.
      LineRenderer.DefaultTemplate = this.LoadResource( LineRenderer.DefaultLineRendererTemplateName ) as DataTemplate;
      LineRenderer.DefaultDataPoinMarkerTemplate = this.LoadResource( LineRenderer.DefaultLineRendererDataPointMarkerTemplateName ) as DataTemplate;
      BarRenderer.DefaultTemplate = this.LoadResource( BarRenderer.DefaultBarRendererTemplateName ) as DataTemplate;

      foreach( var series in this.Series )
      {
        series.Renderer.ApplyDefaultTemplate( series );
      }
    }

    private object LoadResource( string templateName )
    {
      var data = this.Parent as VisualElement;
      object result = null;
      while( ( data != null ) && ( result == null ) )
      {
        data.Resources.TryGetValue( templateName, out result );

        data = data.Parent as VisualElement;
      }

      if( result == null )
      {
        Application.Current?.Resources.TryGetValue( templateName, out result );
      }

      return result;
    }

    #endregion
  }
}
