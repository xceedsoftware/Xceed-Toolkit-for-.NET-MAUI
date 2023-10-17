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
  public enum AxisScaleMode
  {
    Automatic,
    Manual
  }

  public enum AxisTickLabelType
  {
    Numeric,
    Text
  }

  // The Axis properties to set on Chart.Axis.
  public class Axis : LineBase
  {
    #region Private Members

    private Chart m_chart;

    #endregion

    #region Constructors

    public Axis()
      : base( Orientation.Horizontal )
    {
    }

    internal Axis( Chart chart )
      : base( Orientation.Horizontal )
    {
      this.Chart = chart;
    }

    #endregion

    #region Properties

    #region CustomRange

    public static readonly BindableProperty CustomRangeProperty = BindableProperty.Create( nameof( CustomRange ), typeof( IRange ), typeof( Axis ), null, propertyChanged: OnCustomRangeChanged );

    public IRange CustomRange
    {
      get => ( IRange )GetValue( CustomRangeProperty );
      set => SetValue( CustomRangeProperty, value );
    }

    private static void OnCustomRangeChanged( BindableObject bindable, object oldValue, object newValue )
    {
      var axis = bindable as Axis;
      if( axis != null )
      {
        axis.OnCustomRangeChanged( (IRange)oldValue, (IRange)newValue );
      }
    }

    protected virtual void OnCustomRangeChanged( IRange oldValue, IRange newValue )
    {
      if( newValue == null )
        throw new InvalidDataException( "CustomRange can't be null." );

      if( this.Chart != null )
      {
        this.Chart.SetDataPointRange();
        this.Chart.Refresh( false );
      }
    }

    #endregion

    #region GridLineTemplate

    public static readonly BindableProperty GridLineTemplateProperty = BindableProperty.Create( nameof( GridLineTemplate ), typeof( DataTemplate ), typeof( Axis ), null );

    public DataTemplate GridLineTemplate
    {
      get => ( DataTemplate )GetValue( GridLineTemplateProperty );
      set => SetValue( GridLineTemplateProperty, value );
    }

    #endregion

    #region ShowAxis

    public static readonly BindableProperty ShowAxisProperty = BindableProperty.Create( nameof( ShowAxis ), typeof( bool ), typeof( Axis ), true, propertyChanged: OnShowAxisChanged );

    public bool ShowAxis
    {
      get => ( bool )GetValue( ShowAxisProperty );
      set => SetValue( ShowAxisProperty, value );
    }

    private static void OnShowAxisChanged( BindableObject bindable, object oldValue, object newValue )
    {
      var axis = bindable as Axis;
      if( axis != null )
      {
        axis.OnShowAxisChanged( ( bool )oldValue, ( bool )newValue );
      }
    }

    protected virtual void OnShowAxisChanged( bool oldValue, bool newValue )
    {
      if( this.Chart != null )
      {
        this.Chart.Refresh( false );
      }
    }

    #endregion

    #region ShowGridLines

    public static readonly BindableProperty ShowGridLinesProperty = BindableProperty.Create( nameof( ShowGridLines ), typeof( bool ), typeof( Axis ), true, propertyChanged: OnShowGridLineChanged );

    public bool ShowGridLines
    {
      get => ( bool )GetValue( ShowGridLinesProperty );
      set => SetValue( ShowGridLinesProperty, value );
    }

    private static void OnShowGridLineChanged( BindableObject bindable, object oldValue, object newValue )
    {
      var axis = bindable as Axis;
      if( axis != null )
      {
        axis.OnShowGridLineChanged( ( bool )oldValue, ( bool )newValue );
      }
    }

    protected virtual void OnShowGridLineChanged( bool oldValue, bool newValue )
    {
      if( this.Chart != null )
      {
        this.Chart.Refresh( false );
      }
    }

    #endregion

    #region ShowTicks

    public static readonly BindableProperty ShowTicksProperty = BindableProperty.Create( nameof( ShowTicks ), typeof( bool ), typeof( Axis ), true, propertyChanged: OnShowTicksChanged );

    public bool ShowTicks
    {
      get => ( bool )GetValue( ShowTicksProperty );
      set => SetValue( ShowTicksProperty, value );
    }

    private static void OnShowTicksChanged( BindableObject bindable, object oldValue, object newValue )
    {
      var axis = bindable as Axis;
      if( axis != null )
      {
        axis.OnShowTicksChanged( ( bool )oldValue, ( bool )newValue );
      }
    }

    protected virtual void OnShowTicksChanged( bool oldValue, bool newValue )
    {
      if( this.Chart != null )
      {
        // Refresh everything because Axis Line/TickLine must be moved since tickLength is modified
        this.Chart.Refresh( true );
      }
    }

    #endregion

    #region ShowTickLabels

    public static readonly BindableProperty ShowTickLabelsProperty = BindableProperty.Create( nameof( ShowTickLabels ), typeof( bool ), typeof( Axis ), true, propertyChanged: OnShowTickLabelsChanged );

    public bool ShowTickLabels
    {
      get => ( bool )GetValue( ShowTickLabelsProperty );
      set => SetValue( ShowTickLabelsProperty, value );
    }

    private static void OnShowTickLabelsChanged( BindableObject bindable, object oldValue, object newValue )
    {
      var axis = bindable as Axis;
      if( axis != null )
      {
        axis.OnShowTickLabelsChanged( ( bool )oldValue, ( bool )newValue );
      }
    }

    protected virtual void OnShowTickLabelsChanged( bool oldValue, bool newValue )
    {
      if( this.Chart != null )
      {
        this.Chart.Refresh( false );
      }
    }

    #endregion

    #region ShowTitleLabel

    public static readonly BindableProperty ShowTitleLabelProperty = BindableProperty.Create( nameof( ShowTitleLabel ), typeof( bool ), typeof( Axis ), true, propertyChanged: OnShowTitleLabelChanged );

    public bool ShowTitleLabel
    {
      get => ( bool )GetValue( ShowTitleLabelProperty );
      set => SetValue( ShowTitleLabelProperty, value );
    }

    private static void OnShowTitleLabelChanged( BindableObject bindable, object oldValue, object newValue )
    {
      var axis = bindable as Axis;
      if( axis != null )
      {
        axis.OnShowTitleLabelChanged( ( bool )oldValue, ( bool )newValue );
      }
    }

    protected virtual void OnShowTitleLabelChanged( bool oldValue, bool newValue )
    {
      if( this.Chart != null )
      {
        this.Chart.Refresh( false );
      }
    }

    #endregion

    #region TickCount

    public static readonly BindableProperty TickCountProperty = BindableProperty.Create( nameof( TickCount ), typeof( uint ), typeof( Axis ), 10u, coerceValue: CoerceTickCount, propertyChanged: OnTickCountChanged );

    public uint TickCount
    {
      get => ( uint )GetValue( TickCountProperty );
      set => SetValue( TickCountProperty, value );
    }

    public static object CoerceTickCount( BindableObject bindable, object value )
    {
      if( ( uint )value < 2 )
        return 2u;

      return value;
    }

    private static void OnTickCountChanged( BindableObject bindable, object oldValue, object newValue )
    {
      var axis = bindable as Axis;
      if( axis != null ) 
      {
        axis.OnTickCountChanged( ( uint )oldValue, ( uint )newValue );
      }
    }

    protected virtual void OnTickCountChanged( uint oldValue, uint newValue )
    {
      if( this.Chart != null )
      {
        this.Chart.Refresh( false );
      }
    }

    #endregion

    #region TickLabelTemplate

    public static readonly BindableProperty TickLabelTemplateProperty = BindableProperty.Create( nameof( TickLabelTemplate ), typeof( DataTemplate ), typeof( Axis ), null );

    public DataTemplate TickLabelTemplate
    {
      get => ( DataTemplate )GetValue( TickLabelTemplateProperty );
      set => SetValue( TickLabelTemplateProperty, value );
    }

    #endregion

    #region TickLabelType

    public static readonly BindableProperty TickLabelTypeProperty = BindableProperty.Create( nameof( TickLabelType ), typeof( AxisTickLabelType ), typeof( Axis ), AxisTickLabelType.Numeric, propertyChanged: OnTickLabelTypeChanged );

    public AxisTickLabelType TickLabelType
    {
      get => ( AxisTickLabelType )GetValue( TickLabelTypeProperty );
      set => SetValue( TickLabelTypeProperty, value );
    }

    private static void OnTickLabelTypeChanged( BindableObject bindable, object oldValue, object newValue )
    {
      var axis = bindable as Axis;
      if( axis != null )
      {
        axis.OnTickLabelTypeChanged( ( AxisTickLabelType )oldValue, ( AxisTickLabelType )newValue );
      }
    }

    private void OnTickLabelTypeChanged( AxisTickLabelType oldValue, AxisTickLabelType newValue )
    {
      if( this.Chart == null )
        return;

      foreach( var series in this.Chart.Series )
      {
        series.ResetDataPoints();
      }

      foreach( var series in this.Chart.Series )
      {
        series.UpdateDataPointRange();
      }

      this.Chart.Refresh( false );
    }

    #endregion

    #region TickLength

    public static readonly BindableProperty TickLengthProperty = BindableProperty.Create( nameof( TickLength ), typeof( double ), typeof( Axis ), 8d, coerceValue: CoerceTickLength, propertyChanged: OnTickLengthChanged );

    public double TickLength
    {
      get => ( double )GetValue( TickLengthProperty );
      set => SetValue( TickLengthProperty, value );
    }

    public static object CoerceTickLength( BindableObject bindable, object value )
    {
      if( ( double )value < 2 )
        return 8d;

      return value;
    }

    private static void OnTickLengthChanged( BindableObject bindable, object oldValue, object newValue )
    {
      var axis = bindable as Axis;
      if( axis != null )
      {
        axis.OnTickLengthChanged( ( double )oldValue, ( double )newValue );
      }
    }

    protected virtual void OnTickLengthChanged( double oldValue, double newValue )
    {
      if( this.Chart != null )
      {
        this.Chart.Refresh( false );
      }
    }

    #endregion

    #region TickTemplate

    public static readonly BindableProperty TickTemplateProperty = BindableProperty.Create( nameof( TickTemplate ), typeof( DataTemplate ), typeof( Axis ), null );

    public DataTemplate TickTemplate
    {
      get => ( DataTemplate )GetValue( TickTemplateProperty );
      set => SetValue( TickTemplateProperty, value );
    }

    #endregion

    #region Title

    public static readonly BindableProperty TitleProperty = BindableProperty.Create( nameof( Title ), typeof( string ), typeof( Axis ), null, propertyChanged: OnTitleChanged );

    public string Title
    {
      get => ( string )GetValue( TitleProperty );
      set => SetValue( TitleProperty, value );
    }

    private static void OnTitleChanged( BindableObject bindable, object oldValue, object newValue )
    {
      var axis = bindable as Axis;
      if( axis != null )
      {
        axis.OnTitleChanged( ( string )oldValue, ( string )newValue );
      }
    }

    protected virtual void OnTitleChanged( string oldValue, string newValue )
    {
      if( this.Chart != null )
      {
        this.Chart.Refresh( false );
      }
    }

    #endregion

    #region TitleLabelTemplate

    public static readonly BindableProperty TitleLabelTemplateProperty = BindableProperty.Create( nameof( TitleLabelTemplate ), typeof( DataTemplate ), typeof( Axis ), null );

    public DataTemplate TitleLabelTemplate
    {
      get => ( DataTemplate )GetValue( TitleLabelTemplateProperty );
      set => SetValue( TitleLabelTemplateProperty, value );
    }

    #endregion

    #endregion

    #region Internal Properties

    #region Chart

    internal Chart Chart
    {
      get
      {
        return m_chart;
      }
      set
      {
        m_chart = value;
        Debug.Assert( m_chart != null );

        m_chart.SetDataPointRange();
      }
    }

    #endregion

    #endregion

    #region Internal Methods

    internal override void SetPoints( Rect bounds, double offset )
    {
      var infos = this.Infos as AreaElementInfos;

      infos.Points.Clear();

      // Use Points to measure the AxisLine.
      infos.Points.Add( new Point( bounds.Left, bounds.Top ) );

      if( this.Orientation == Orientation.Horizontal )
      {
        infos.Points.Add( new Point( bounds.Right - offset, bounds.Top ) );
      }
      else if( this.Orientation == Orientation.Vertical )
      {
        infos.Points.Add( new Point( bounds.Left, bounds.Bottom - offset ) );
      }
    }

    internal override void SetLocation( Rect bounds, Point offset )
    {
      // Use Location to arrange the AxisLine.
      this.Infos.Location = ( this.Orientation == Orientation.Horizontal )
                            ? new Point( bounds.Left + offset.X, bounds.Bottom - offset.Y )
                            : new Point( bounds.Left + offset.Y, bounds.Top );
    }

    internal void SetOrientation( Orientation orientation )
    {
      this.Orientation = orientation;
    }

    internal bool IsUsingTickLabel()
    {
      return (this.TickLabelType == AxisTickLabelType.Text);
    }

    internal uint GetInternalTickCount()
    {
      if( this.IsUsingTickLabel() )
        return (uint)this.GetAllDistinctDataPoints().Count;

      return this.TickCount;
    }

    internal List<DataPoint> GetAllDistinctDataPoints()
    {
      IEnumerable<DataPoint> allDataPoints = new List<DataPoint>();

      if( this.Chart != null )
      {
        allDataPoints = this.Chart.Series.Aggregate( allDataPoints, ( current, series ) => current.Concat( series.DataPoints ) );

        // Get distinct DataPoints relative to Text.
        allDataPoints = allDataPoints
                        .GroupBy( dataPoint => dataPoint.Text )
                        .Select( g => g.First() );
      }

      return allDataPoints.ToList();
    }

    internal bool IsUsingCustomRange()
    {
      return this.IsUsingCustomStartRange() || IsUsingCustomEndRange();
    }

    internal bool IsUsingCustomStartRange()
    {
      return ( this.CustomRange is AutomaticEndRange )
          || ( this.CustomRange is Range );
    }

    internal bool IsUsingCustomEndRange()
    {
      return ( this.CustomRange is AutomaticStartRange )
          || ( this.CustomRange is Range );
    }

    #endregion
  }
}
