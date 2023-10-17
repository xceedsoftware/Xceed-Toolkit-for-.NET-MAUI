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


namespace Xceed.Maui.Toolkit
{
  public class DataPoint : BindableObject
  {
    #region Constructors

    public DataPoint() 
    {
    }

    public DataPoint( double x, double y )
      : this()
    {
      this.X = x;
      this.Y = y;
    }

    public DataPoint( double x, double y, string text )
      : this( x, y )
    {
      this.Text = text;
    }

    #endregion

    #region Public Properties

    #region Text

    public static readonly BindableProperty TextProperty = BindableProperty.Create( nameof( Text ), typeof( string ), typeof( DataPoint ), null );

    public string Text
    {
      get => ( string )GetValue( TextProperty );
      set => SetValue( TextProperty, value );
    }

    #endregion

    #region X

    public static readonly BindableProperty XProperty = BindableProperty.Create( nameof( X ), typeof( double ), typeof( DataPoint ), 0d, propertyChanged: OnXChanged );

    public double X
    {
      get => ( double )GetValue( XProperty );
      set => SetValue( XProperty, value );
    }

    private static void OnXChanged( BindableObject bindable, object oldValue, object newValue )
    {
      var dataPoint = bindable as DataPoint;
      if( dataPoint != null )
      {
        dataPoint.OnXChanged( ( double )oldValue, ( double )newValue );
      }
    }

    protected virtual void OnXChanged( double oldValue, double newValue )
    {
      this.InternalX = newValue;
    }

    #endregion

    #region Y

    public static readonly BindableProperty YProperty = BindableProperty.Create( nameof( Y ), typeof( double ), typeof( DataPoint ), 0d, propertyChanged: OnYChanged );

    public double Y
    {
      get => ( double )GetValue( YProperty );
      set => SetValue( YProperty, value );
    }

    private static void OnYChanged( BindableObject bindable, object oldValue, object newValue )
    {
      var dataPoint = bindable as DataPoint;
      if( dataPoint != null )
      {
        dataPoint.OnYChanged( ( double )oldValue, ( double )newValue );
      }
    }

    protected virtual void OnYChanged( double oldValue, double newValue )
    {
      this.InternalY = newValue;
    }

    #endregion

    #endregion

    #region Internal Properties

    // We do not want to modify the original DataPointX (when Axis.TickLabelType is Text, this value goes from 1 to DataPoints.Count).
    #region InternalX

    internal double InternalX
    {
      get;
      set;
    }

    #endregion

    // We do not want to modify the original DataPointY (when Axis.TickLabelType is Text, this value goes from 1 to DataPoints.Count).
    #region InternalY

    internal double InternalY
    {
      get;
      set;
    }

    #endregion

    #endregion

    #region Internal Methods

    internal void Reset() 
    {
      this.InternalX = this.X;
      this.InternalY = this.Y;
    }

    #endregion
  }
}
