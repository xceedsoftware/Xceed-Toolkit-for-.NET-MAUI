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


using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Xceed.Maui.Toolkit
{
  // The Series properties to set on Chart.Series.
  public class Series : Control
  {
    #region Private Members

    private static int m_id = -1;

    private bool m_preventChartRefresh;

    #endregion

    #region Constructors
    public Series() 
    {
      this.DataPoints = new ObservableCollection<DataPoint>();
      this.DataPoints.CollectionChanged += this.DataPoints_CollectionChanged;
      this.DataPointsSourceBindingInfos = new ObservableCollection<BindingInfos>();
      this.DataPointsSourceBindingInfos.CollectionChanged += this.DataPointsSourceBindingInfos_CollectionChanged;
      this.DataPointRange = new List<Range>();
      this.Renderer = new LineRenderer();
      this.Title = "Series " + ++m_id;
    }

    #endregion

    #region Public Properties

    #region DataPoints

    public ObservableCollection<DataPoint> DataPoints
    {
      get;
    }

    #endregion

    #region DataPointsSource

    public static readonly BindableProperty DataPointsSourceProperty = BindableProperty.Create( nameof( DataPointsSource ), typeof( IEnumerable ), typeof( Series ), null, propertyChanged: OnDataPointsSourceChanged  );

    public IEnumerable DataPointsSource
    {
      get => ( IEnumerable )GetValue( DataPointsSourceProperty );
      set => SetValue( DataPointsSourceProperty, value );
    }

    private static void OnDataPointsSourceChanged( BindableObject bindable, object oldValue, object newValue )
    {
      var series = bindable as Series;
      if( series != null )
      {
        series.OnDataPointsSourceChanged( ( IEnumerable )oldValue, ( IEnumerable )newValue );
      }
    }

    protected virtual void OnDataPointsSourceChanged( IEnumerable oldValue, IEnumerable newValue )
    {
      var notifyCollection = oldValue as INotifyCollectionChanged;
      if( notifyCollection != null )
      {
        notifyCollection.CollectionChanged -= this.DataPointsSourceCollectionChanged;
      }

      notifyCollection = newValue as INotifyCollectionChanged;
      if( notifyCollection != null )
      {
        // Be notified when a DataPoint is added/removed.
        notifyCollection.CollectionChanged += this.DataPointsSourceCollectionChanged;
      }

      this.UpdateDataPointsFromSource();
    }

    #endregion

    #region DataPointsSourceBindingInfos

    public static readonly BindableProperty DataPointsSourceBindingInfosProperty = BindableProperty.Create( nameof( DataPointsSourceBindingInfos ), typeof( ObservableCollection<BindingInfos> ), typeof( Series ), null, propertyChanged: OnDataPointsSourceBindingInfosChanged );

    public ObservableCollection<BindingInfos> DataPointsSourceBindingInfos
    {
      get => (ObservableCollection<BindingInfos>)GetValue( DataPointsSourceBindingInfosProperty );
      set => SetValue( DataPointsSourceBindingInfosProperty, value );
    }

    private static void OnDataPointsSourceBindingInfosChanged( BindableObject bindable, object oldValue, object newValue )
    {
      var series = bindable as Series;
      if( series != null )
      {
        series.OnDataPointsSourceBindingInfosChanged( (ObservableCollection<BindingInfos>)oldValue, (ObservableCollection<BindingInfos>)newValue );
      }
    }

    protected virtual void OnDataPointsSourceBindingInfosChanged( ObservableCollection<BindingInfos> oldValue, ObservableCollection<BindingInfos> newValue )
    {
      var notifyCollection = oldValue as INotifyCollectionChanged;
      if( notifyCollection != null )
      {
        notifyCollection.CollectionChanged -= this.DataPointsSourceBindingInfosCollectionChanged;
      }

      notifyCollection = newValue as INotifyCollectionChanged;
      if( notifyCollection != null )
      {
        // Be notified when a DataPointSourceBindingInfos is added/removed.
        notifyCollection.CollectionChanged += this.DataPointsSourceBindingInfosCollectionChanged;
      }

      this.UpdateDataPointsFromSource();
    }

    #endregion

    #region DataPointMarkerTemplate

    public static readonly BindableProperty DataPointMarkerTemplateProperty = BindableProperty.Create( nameof( DataPointMarkerTemplate ), typeof( DataTemplate ), typeof( Series ), null );

    public DataTemplate DataPointMarkerTemplate
    {
      get => ( DataTemplate )GetValue( DataPointMarkerTemplateProperty );
      set => SetValue( DataPointMarkerTemplateProperty, value );
    }

    #endregion

    #region DataPointLabelTemplate

    public static readonly BindableProperty DataPointLabelTemplateProperty = BindableProperty.Create( nameof( DataPointLabelTemplate ), typeof( DataTemplate ), typeof( Series ), null );

    public DataTemplate DataPointLabelTemplate
    {
      get => ( DataTemplate )GetValue( DataPointLabelTemplateProperty );
      set => SetValue( DataPointLabelTemplateProperty, value );
    }

    #endregion

    #region DataPointLabelLineLength

    public static readonly BindableProperty DataPointLabelLineLengthProperty = BindableProperty.Create( nameof( DataPointLabelLineLength ), typeof( double ), typeof( Series ), propertyChanged: OnDataPointLabelLineLengthChanged );

    public double DataPointLabelLineLength
    {
      get => ( double )GetValue( DataPointLabelLineLengthProperty );
      set => SetValue( DataPointLabelLineLengthProperty, value );
    }

    private static void OnDataPointLabelLineLengthChanged( BindableObject bindable, object oldValue, object newValue )
    {
      var series = bindable as Series;
      if( series != null )
      {
        series.OnDataPointLabelLineLengthChanged( ( double )oldValue, ( double )newValue );
      }
    }

    protected virtual void OnDataPointLabelLineLengthChanged( double oldValue, double newValue )
    {
      if( newValue < 0d )
        throw new InvalidDataException( "DataPointLabelLineLength must be greater than or equal to 0." );

      this.RefreshChart( false );
    }

    #endregion

    #region DataPointLabelLineTemplate

    public static readonly BindableProperty DataPointLabelLineTemplateProperty = BindableProperty.Create( nameof( DataPointLabelLineTemplate ), typeof( DataTemplate ), typeof( Series ), null );

    public DataTemplate DataPointLabelLineTemplate
    {
      get => ( DataTemplate )GetValue( DataPointLabelLineTemplateProperty );
      set => SetValue( DataPointLabelLineTemplateProperty, value );
    }

    #endregion

    #region Renderer

    public static readonly BindableProperty RendererProperty = BindableProperty.Create( nameof( Renderer ), typeof( RendererBase ), typeof( Series ), null, propertyChanged: OnRendererChanged, coerceValue: OnCoerceRenderer );

    public RendererBase Renderer
    {
      get => ( RendererBase )GetValue( RendererProperty );
      set => SetValue( RendererProperty, value );
    }

    private static object OnCoerceRenderer( BindableObject bindable, object value )
    {
      if( value == null )
        throw new InvalidDataException( "Renderer property must be set to a valid Renderer." );

      return value;
    }

    public static void OnRendererChanged( BindableObject bindable, object oldValue, object newValue )
    {
      var series = bindable as Series;
      if( series != null )
      {
        series.OnRendererChanged( ( RendererBase )oldValue, ( RendererBase )newValue );
      }
    }

    protected virtual void OnRendererChanged( RendererBase oldValue, RendererBase newValue )
    {
      if( newValue != null )
      {
        newValue.ApplyDefaultTemplate( this );
      }

      var seriesPanel = this.GetSeriesPanel();
      if( seriesPanel != null )
      {
        oldValue.ClearDataPointVisuals( this );
        seriesPanel.ClearDataPointVisuals();

        seriesPanel.CreateVisualChildren();
      }
    }

    #endregion

    #region ShowDataPointLabels

    public static readonly BindableProperty ShowDataPointLabelsProperty = BindableProperty.Create( nameof( ShowDataPointLabels ), typeof( bool ), typeof( Series ), false, propertyChanged: OnShowDataPointLabelsChanged );

    public bool ShowDataPointLabels
    {
      get => ( bool )GetValue( ShowDataPointLabelsProperty );
      set => SetValue( ShowDataPointLabelsProperty, value );
    }

    public static void OnShowDataPointLabelsChanged( BindableObject bindable, object oldValue, object newValue )
    {
      var series = bindable as Series;
      if( series != null )
      {
        series.OnShowDataPointLabelsChanged( (bool)oldValue, (bool)newValue );
      }
    }

    protected virtual void OnShowDataPointLabelsChanged( bool oldValue, bool newValue )
    {
      this.RefreshChart();
    }

    #endregion

    #region Template

    public static readonly BindableProperty TemplateProperty = BindableProperty.Create( nameof( Template ), typeof( DataTemplate ), typeof( Series ), null );

    public DataTemplate Template
    {
      get => ( DataTemplate )GetValue( TemplateProperty );
      set => SetValue( TemplateProperty, value );
    }

    #endregion

    #region Title

    public static readonly BindableProperty TitleProperty = BindableProperty.Create( nameof( Title ), typeof( string ), typeof( Series ), null, propertyChanged: OnTitleChanged );

    public string Title
    {
      get => (string)GetValue( TitleProperty );
      set => SetValue( TitleProperty, value );
    }

    private static void OnTitleChanged( BindableObject bindable, object oldValue, object newValue )
    {
      var series = bindable as Series;
      if( series != null ) 
      {
        series.OnTitleChanged( (string)oldValue, (string)newValue );
      }
    }

    private void OnTitleChanged( string oldValue, string newValue )
    {
      if( this.Chart != null )
      {
        this.Chart.UpdateLegend();
      }
    }

    #endregion

    #endregion

    #region Public Methods

    public bool IsUsingDefaultTemplate()
    {
      return ( ( this.Template == LineRenderer.DefaultTemplate )
            || ( this.Template == BarRenderer.DefaultTemplate ) );
    }

    #endregion

    #region Internal Properties

    #region Chart

    internal Chart Chart
    {
      get;
      set;
    }

    #endregion

    #region DataPointRange

    internal IList<Range> DataPointRange
    {
      get;
    }

    #endregion

    #region Id

    internal new int Id
    {
      get;
      set;
    }

    #endregion

    #endregion

    #region Internal Methods

    internal double GetWidth( IRange seriesDataPointRangeX, IRange chartRangeX, double widthConstraint )
    {
      return this.Renderer.GetSeriesWidth( seriesDataPointRangeX, chartRangeX, widthConstraint );
    }

    internal double GetHeight( IRange seriesDataPointRangeY, IRange chartRangeY, double heightConstraint )
    {
      return this.Renderer.GetSeriesHeight( seriesDataPointRangeY, chartRangeY, heightConstraint );
    }

    internal double GetX( IRange seriesDataPointRangeX, IRange chartRangeX, double widthConstraint )
    {
      return this.Renderer.GetSeriesX( seriesDataPointRangeX, chartRangeX, widthConstraint );
    }

    internal double GetY( IRange seriesDataPointRangeY, IRange chartRangeY, double heightConstraint )
    {
      return this.Renderer.GetSeriesY( seriesDataPointRangeY, chartRangeY, heightConstraint );
    }

    internal double GetLeftOverlap()
    {
      return this.Renderer.GetLeftOverlap( this );
    }

    internal double GetRightOverlap()
    {
      return this.Renderer.GetRightOverlap( this );
    }

    internal double GetTopOverlap( double dataPointLabelHeight, double heightConstraint )
    {
      if( !this.ShowDataPointLabels )
        return 0d;
      if( ( this.Chart == null ) || ( this.Chart.DataPointRange.Count < 2 ) || ( this.DataPointRange.Count < 2 ) )
        return 0d;
      if( this.DataPointRange[ 1 ].End < 0 )
        return 0d;

      var availableHeight = ( this.Chart.DataPointRange[ 1 ].End - this.DataPointRange[ 1 ].End ) / ( this.Chart.DataPointRange[ 1 ].End - this.Chart.DataPointRange[ 1 ].Start ) * heightConstraint;
      var requiredHeight = ( dataPointLabelHeight + this.DataPointLabelLineLength );

      return Math.Max( 0, requiredHeight - availableHeight );
    }

    internal double GetBottomOverlap( double dataPointLabelHeight, double heightConstraint )
    {
      if( !this.ShowDataPointLabels )
        return 0d;
      if( ( this.Chart == null ) || ( this.Chart.DataPointRange.Count < 2 ) || ( this.DataPointRange.Count < 2 ) )
        return 0d;
      if( this.DataPointRange[ 1 ].Start >= 0 )
        return 0d;

      var availableHeight = ( Math.Abs( this.DataPointRange[ 1 ].Start - this.Chart.DataPointRange[ 1 ].Start ) ) / ( this.Chart.DataPointRange[ 1 ].End - this.Chart.DataPointRange[ 1 ].Start ) * heightConstraint;
      var requiredHeight = ( dataPointLabelHeight + this.DataPointLabelLineLength );

      return Math.Max( 0, requiredHeight - availableHeight );
    }

    internal void UpdateDataPointRange()
    {
      var minX = 0d;
      var maxX = 0d;
      var minY = 0d;
      var maxY = 0d;

      this.DataPointRange.Clear();      

      if( this.DataPoints.Count > 0 )
      {
        if( this.Chart != null )
        {
          if( this.Chart.HorizontalAxis.IsUsingTickLabel() )
          {
            var allDataPoints = this.Chart.HorizontalAxis.GetAllDistinctDataPoints();

            for( int i = 0; i < this.DataPoints.Count; ++i )
            {
              var index = allDataPoints.FindIndex( dataPoint => dataPoint.Text == this.DataPoints[ i ].Text );              
              if( index >= 0 )
              {
                this.DataPoints[ i ].InternalX = index + 1;
              }
            }
          }
          if( this.Chart.VerticalAxis.IsUsingTickLabel() )
          {
            var allDataPoints = this.Chart.VerticalAxis.GetAllDistinctDataPoints();

            for( int i = 0; i < this.DataPoints.Count; ++i )
            {
              var index = allDataPoints.FindIndex( dataPoint => dataPoint.Text == this.DataPoints[ i ].Text );
              if( index >= 0 )
              {
                this.DataPoints[ i ].InternalY = index + 1;
              }
            }
          }
        }

        minX = this.DataPoints.Min( dataPoint => dataPoint.InternalX );
        maxX = this.DataPoints.Max( dataPoint => dataPoint.InternalX );
        minY = this.DataPoints.Min( dataPoint => dataPoint.InternalY );
        maxY = this.DataPoints.Max( dataPoint => dataPoint.InternalY );
      }      

      this.DataPointRange.Add( new Range( minX, maxX ) );
      this.DataPointRange.Add( new Range( minY, maxY ) );

      if( this.Chart != null )
      {
        this.Chart.SetDataPointRange();
      }
    }

    internal void ResetDataPoints()
    {
      foreach( var dataPoint in this.DataPoints )
      {
        dataPoint.Reset();
      }
    }

    #endregion

    #region Private Methods

    private void UpdateDataPointsFromSource()
    {
      m_preventChartRefresh = true;

      foreach( var point in this.DataPoints.ToList() )
      {
        this.RemoveDataPointWithBinding( point );
      }

      if( (this.DataPointsSource == null) || ( this.DataPointsSourceBindingInfos == null ) || ( this.DataPointsSourceBindingInfos.Count < 2 ) )
      {
        m_preventChartRefresh = false;
        return;
      }

      foreach( var item in this.DataPointsSource )
      {
        this.AddDataPointWithBinding( item );
      }

      m_preventChartRefresh = false;

      // Do only 1 Refresh at the end.
      this.RefreshChart( false );
    }

    private void AddDataPointWithBinding( object item )
    {
      if( ( item == null ) || ( this.DataPointsSourceBindingInfos == null ) )
        return;

      var point = new DataPoint();

      foreach( var bindingInfos in this.DataPointsSourceBindingInfos )
      {
        switch( bindingInfos.DataPointPropertyName )
        {
          case DataPointPropertyName.X:
            point.SetBinding( DataPoint.XProperty, bindingInfos.UserObjectPropertyName );
            break;
          case DataPointPropertyName.Y:
            point.SetBinding( DataPoint.YProperty, bindingInfos.UserObjectPropertyName );
            break;
          case DataPointPropertyName.Text:
            point.SetBinding( DataPoint.TextProperty, bindingInfos.UserObjectPropertyName );
            break;
          default: throw new InvalidDataException( "Unknown DataPointPropertyName. Must be X, Y or Text." );
        }
      }

      point.BindingContext = item;
      // Be notified when a DataPoints is modified.
      point.PropertyChanged += this.DataPointWithBinding_PropertyChanged;

      this.DataPoints.Add( point );
    }

    private void RemoveDataPointWithBinding( DataPoint dataPoint )
    {
      if( dataPoint == null )
        return;

      dataPoint.RemoveBinding( DataPoint.XProperty );
      dataPoint.RemoveBinding( DataPoint.YProperty );
      dataPoint.RemoveBinding( DataPoint.TextProperty );

      dataPoint.PropertyChanged -= this.DataPointWithBinding_PropertyChanged;

      this.DataPoints.Remove( dataPoint );
    }

    private void RefreshChart( bool refreshAll = true )
    {
      if( this.Chart != null )
      {
        this.Chart.Refresh( refreshAll );
      }
    }

    private SeriesPanel GetSeriesPanel()
    {
      if( this.Chart != null )
      {
        var areaPanel = this.Chart.GetAreaPanel();
        if( areaPanel != null )
        {
          return areaPanel.GetSeriesPanel( this );
        }
      }

      return null;
    }

    #endregion

    #region Event Handlers

    // A DataPoint is added/removed in this.DataPoints Collection.
    private void DataPoints_CollectionChanged( object sender, NotifyCollectionChangedEventArgs e )
    {
      this.UpdateDataPointRange();

      if( e.Action == NotifyCollectionChangedAction.Add )
      {
        if( this.Chart != null )
        {
          foreach( var _ in e.NewItems )
          {
            this.Chart.AddDataPointVisual( this );
          }
        }
      }
      else if( e.Action == NotifyCollectionChangedAction.Remove )
      {
        if( this.Chart != null )
        {
          foreach( var _ in e.OldItems )
          {
            this.Chart.RemoveDataPointVisual( this, e.OldStartingIndex );
          }
        }
      }
      else if( e.Action == NotifyCollectionChangedAction.Reset )
      {
        if( this.Chart != null )
        {
          this.Chart.ClearDataPointVisuals( this );
        }
      }

      if( !m_preventChartRefresh )
      {
        this.RefreshChart( false );
      }
    }

    // A BindingInfos was modified.
    private void DataPointsSourceBindingInfos_CollectionChanged( object sender, NotifyCollectionChangedEventArgs e )
    {
      this.UpdateDataPointsFromSource();
    }

    // A DataPoint with binding gets modified.
    private void DataPointWithBinding_PropertyChanged( object sender, PropertyChangedEventArgs e )
    {
      if( !m_preventChartRefresh )
      {
        this.UpdateDataPointRange();
        this.RefreshChart( false );
      }
    }

    // A DataPoint is added/removed in the this.DataPointsSource Collection.
    private void DataPointsSourceCollectionChanged( object sender, NotifyCollectionChangedEventArgs e )
    {
      if( e.Action == NotifyCollectionChangedAction.Add )
      {
        foreach( var item in e.NewItems )
        {
          this.AddDataPointWithBinding( item );
        }
      }
      else if( e.Action == NotifyCollectionChangedAction.Remove )
      {
        foreach( var item in e.OldItems )
        {
          this.RemoveDataPointWithBinding( this.DataPoints.FirstOrDefault( dataPoint => dataPoint.BindingContext == item ) );
        }
      }
      else if( e.Action == NotifyCollectionChangedAction.Reset )
      {
        this.DataPoints.ToList().ForEach( this.RemoveDataPointWithBinding );
      }
    }

    private void DataPointsSourceBindingInfosCollectionChanged( object sender, NotifyCollectionChangedEventArgs e )
    {
      this.UpdateDataPointsFromSource();
    }

    #endregion
  }
}
