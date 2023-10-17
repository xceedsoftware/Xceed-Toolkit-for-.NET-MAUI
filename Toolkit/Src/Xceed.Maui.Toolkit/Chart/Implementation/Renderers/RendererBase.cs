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
  public abstract class RendererBase : BindableObject
  {
    #region Public Methods

    public abstract IList<AreaElement> CreateVisualChildren( Series series );

    public abstract void ApplyDefaultTemplate( Series series );

    public abstract void InitializeSeries( Rect bounds, Series series );

    public abstract void ClearDataPointVisuals( Series series );

    #endregion

    #region Internal Methods

    protected internal static Brush GetSeriesBackground( Series series )
    {
      if( series == null )
        throw new ArgumentNullException( nameof( series ) );

      var background = series.Background;
      if( background != null )
        return background;

      if( Chart.DefaultSeriesBackgroundColor == null )
        return background;

      Debug.Assert( series.Id >= 0, "series.Id must be greater or equal to 0." );
      Debug.Assert( Chart.DefaultSeriesBackgroundColor != null, "Chart.DefaultSeriesBackgroundColor should never be null." );

      var colors = Chart.DefaultSeriesBackgroundColor;

      return new SolidColorBrush( colors.ElementAt( series.Id % colors.Length ) );
    }

    protected internal virtual double GetSeriesWidth( IRange seriesDataPointRangeX, IRange chartRangeX, double widthConstraint )
    {
      return ( seriesDataPointRangeX.End - seriesDataPointRangeX.Start ) / ( chartRangeX.End - chartRangeX.Start ) * widthConstraint;
    }

    protected internal virtual double GetSeriesHeight( IRange seriesDataPointRangeY, IRange chartRangeY, double heightConstraint )
    {
      return Math.Abs( ( seriesDataPointRangeY.End - seriesDataPointRangeY.Start ) / ( chartRangeY.End - chartRangeY.Start ) ) * heightConstraint;
    }

    protected internal virtual double GetSeriesX( IRange seriesDataPointRangeX, IRange chartRangeX, double widthConstraint )
    {
      return ( seriesDataPointRangeX.Start - chartRangeX.Start ) / ( chartRangeX.End - chartRangeX.Start ) * widthConstraint;
    }

    protected internal virtual double GetSeriesY( IRange seriesDataPointRangeY, IRange chartRangeY, double heightConstraint )
    {
      return ( chartRangeY.End - seriesDataPointRangeY.End ) / ( chartRangeY.End - chartRangeY.Start ) * heightConstraint;
    }

    protected internal virtual void CreateDataPointVisual( Series series, List<AreaElement> elements )
    {
    }

    protected internal virtual void RemoveDataPointVisual( Series series, int index, List<AreaElement> elements )
    { 
    }

    protected internal virtual double GetLeftOverlap( Series series )
    {
      return 0;
    }

    protected internal virtual double GetRightOverlap( Series series )
    {
      return 0;
    }

    internal IList<AreaElement> AddDataPointVisual( Series series )
    {
      if( series == null )
        return null;

      var elements = new List<AreaElement>();

      this.CreateDataPointVisual( series, elements );

      return elements;
    }

    internal IList<AreaElement> RemoveDataPointVisual( Series series, int index )
    {
      if( series == null )
        return null;

      var elements = new List<AreaElement>();

      this.RemoveDataPointVisual( series, index, elements );

      return elements;
    }

    internal void ClearDataPointVisual( Series series )
    {
      if( series == null )
        return;

      this.ClearDataPointVisuals( series );
    }

    #endregion
  }
}
