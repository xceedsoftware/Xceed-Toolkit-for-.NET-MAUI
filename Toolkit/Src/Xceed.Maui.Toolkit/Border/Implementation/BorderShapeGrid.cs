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


using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;
using System.Runtime.InteropServices;
using Point = Microsoft.Maui.Graphics.Point;

namespace Xceed.Maui.Toolkit
{
  // BorderShapeGrid is a Grid containing a Background RoundRectangle, an outline Path( or roundRectangle ) and the ContentPresenter.
  // The Path is useful to fill with a Gradient Brush and/or use real CornerRadius of 0. It also let us joins corners with independant BorderThickness.
  public partial class BorderShapeGrid : Layout
  {
    #region Internal Members

    internal static bool IsNet8AndUp = BorderShapeGrid.IsNet8Up();

    #endregion

    #region Private Members

    private const float K_RATIO = 0.551784777779014f; // ideal ratio of cubic Bezier points for a quarter circle
    private const string FlatRectanglePathOutline = "FlatRectanglePathOutline";
    private const string PathOutline = "PathOutline";
    private const string BackgroundRectangle = "BackgroundRectangle";
    private const string FrameworkRectangleOutline = "FrameworkRectangleOutline";

    #endregion

    #region Constructors

    public BorderShapeGrid() 
    {
      this.HandlerChanged += this.BorderShape_HandlerChanged;
      this.HandlerChanging += this.BorderShape_HandlerChanging;
    }

    #endregion

    #region Public Properties

    #region Background

    public static new readonly BindableProperty BackgroundProperty = BindableProperty.Create( nameof( Background ), typeof( Brush ), typeof( BorderShapeGrid ), null );

    public new Brush Background
    {
      get => (Brush)GetValue( BackgroundProperty );
      set => SetValue( BackgroundProperty, value );
    }

    #endregion

    #region BorderBrush

    public static readonly BindableProperty BorderBrushProperty = BindableProperty.Create( nameof( BorderBrush ), typeof( Brush ), typeof( BorderShapeGrid ), null, propertyChanged: OnBorderBrushPropertyChanged );

    public Brush BorderBrush
    {
      get => (Brush)GetValue( BorderBrushProperty );
      set => SetValue( BorderBrushProperty, value );
    }

    private static void OnBorderBrushPropertyChanged( BindableObject bindable, object oldValue, object newValue )
    {
      if( bindable is BorderShapeGrid borderShape )
      {
        borderShape.OnBorderBrushPropertyChanged( (Brush)oldValue, (Brush)newValue );
      }
    }

    protected virtual void OnBorderBrushPropertyChanged( Brush oldValue, Brush newValue )
    {
      if( !this.IsLoaded )
        return;

      if( this.IsFrameworkRoundRectangleOutline()
        && ( ( newValue == null ) || ( newValue is SolidColorBrush ) )
        && ( ( oldValue == null ) || ( oldValue is SolidColorBrush ) ) )
        return;

      if( this.IsPathFlatOutline()
        && ( ( newValue == null ) || ( newValue is SolidColorBrush ) )
        && ( ( oldValue == null ) || ( oldValue is SolidColorBrush ) ) )
        return;

      if( this.IsPathOutline() )
        return;

      this.UpdateShape();
    }

    #endregion

    #region BorderThickness

    public static readonly BindableProperty BorderThicknessProperty = BindableProperty.Create( nameof( BorderThickness ), typeof( Thickness ), typeof( BorderShapeGrid ), Thickness.Zero, propertyChanged: OnBorderThicknessPropertyChanged );

    public Thickness BorderThickness
    {
      get => (Thickness)GetValue( BorderThicknessProperty );
      set => SetValue( BorderThicknessProperty, value );
    }

    private static void OnBorderThicknessPropertyChanged( BindableObject bindable, object oldValue, object newValue )
    {
      if( bindable is BorderShapeGrid borderShape )
      {
        borderShape.OnBorderThicknessPropertyChanged( ( Thickness )oldValue, ( Thickness )newValue );
      }
    }

    protected virtual void OnBorderThicknessPropertyChanged( Thickness oldValue, Thickness newValue )
    {
      if( !this.IsLoaded )
        return;

      if( this.IsFrameworkRoundRectangleOutline() && this.IsUniformThicknessmOutlines() )
        return;

      this.UpdateShape();
    }

    #endregion

    #region CornerRadius

    public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create( nameof( CornerRadius ), typeof( CornerRadius ), typeof( BorderShapeGrid ), default( CornerRadius ), propertyChanged: OnCornerRadiusPropertyChanged );

    public CornerRadius CornerRadius
    {
      get => (CornerRadius)GetValue( CornerRadiusProperty );
      set => SetValue( CornerRadiusProperty, value );
    }

    private static void OnCornerRadiusPropertyChanged( BindableObject bindable, object oldValue, object newValue )
    {
      if( bindable is BorderShapeGrid borderShape )
      {
        borderShape.OnCornerRadiusPropertyChanged( (CornerRadius)oldValue, (CornerRadius)newValue );
      }
    }

    protected virtual void OnCornerRadiusPropertyChanged( CornerRadius oldValue, CornerRadius newValue )
    {
      if( !this.IsLoaded )
        return;

      if( this.IsFrameworkRoundRectangleOutline() && ( newValue != default( CornerRadius ) ) )
        return;

      this.UpdateShape();
    }

    #endregion

    #endregion

    #region Partial Methods

    partial void InitializeForPlatform( object sender, EventArgs e );

    partial void UninitializeForPlatform( object sender, HandlerChangingEventArgs e );

    #endregion

    #region Protected Methods

    protected override ILayoutManager CreateLayoutManager()
    {
      return new BorderShapeGridLayoutManager( this );
    }

    protected override Size ArrangeOverride( Rect bounds )
    {
      // Update calculations for Path Outlines.
      this.UpdateShapeSize( bounds.Width, bounds.Height );

      return base.ArrangeOverride( bounds );
    }

    #endregion

    #region Internal Methods

    internal void UpdateShapeSize( double width, double height )
    {
      // Only outline Path need to be resized. RoundRectangle automatically fills content.
      if( ( this.Children.Count > 1 )
         && ( this.Children[ this.Children.Count - 2 ] is Microsoft.Maui.Controls.Shapes.Path path ) )
      {
        if( path.ClassId == BorderShapeGrid.PathOutline )
        {
          this.SetPathOutline( path, width, height );
        }
        else if( path.ClassId == BorderShapeGrid.FlatRectanglePathOutline )
        {
          this.SetFlatRectangleOutline( path, width, height );
        }
      }
    }

    internal void UpdateShape()
    {
      this.Children.ToList().ForEach( child =>
      {
        if( !( child is ContentPresenter ) )
        {
          this.Children.Remove( child );
        }
      } );

      var outlineShape = this.GetOutlineShape();
      if( outlineShape != null )
      {
        this.Children.Insert( 0, outlineShape );
      }

      var backgroundShape = this.GetBackgroundRoundedRectangle();
      if( backgroundShape != null )
      {
        this.Children.Insert( 0, backgroundShape );
      }

      this.UpdateShapeSize( this.Width, this.Height );
    }

    #endregion

    #region Private Methods

    private static bool IsNet8Up()
    {
      var version = RuntimeInformation.FrameworkDescription;
      if( !version.StartsWith( ".NET " ) )
        return false;

      var parts = version.Split( '.', ' ' );
      if( parts.Count() < 3 )
        return false;

      var versinNumber = int.Parse( parts[ 2 ] );

      return (versinNumber >= 8);
    }

    private static double ClampCornerRadius( double cornerRadius, double width, double height )
    {
      if( cornerRadius > ( height / 2 ) )
      {
        cornerRadius = height / 2;
      }

      if( cornerRadius > ( width / 2 ) )
      {
        cornerRadius = width / 2;
      }

      return cornerRadius;
    }

    private Shape GetOutlineShape()
    {
      if( this.IsUniformThicknessmOutlines() )
      {
        if( ( this.BorderThickness.Left == 0 ) || ( this.BorderBrush == null ) )
          return null;

        if( this.BorderBrush is SolidColorBrush solidColorBrush )
        {
          if( solidColorBrush.Color == Colors.Transparent )
            return null;


          // Create a Path with 0 CornerRadius. The framework's RoundRectangle(or Rectangle) always has visible rounded Corners, even with CornerRadius at 0.
          // This has been fixed in .NET 8+.
          if( !BorderShapeGrid.IsNet8AndUp )
          {
            if( this.CornerRadius == default( CornerRadius ) )
              return GetFlatRectanglePathOutline();
          }

          return this.GetFrameworkRoundedRectangleOutline();
        }
      }

      return this.GetPathOutline();
    }

    private bool IsUniformThicknessmOutlines()
    {
      return ( this.BorderThickness.Left == this.BorderThickness.Right )
          && ( this.BorderThickness.Left == this.BorderThickness.Top )
          && ( this.BorderThickness.Left == this.BorderThickness.Bottom );
    }

    private bool IsPathOutline()
    {
      return ( this.Children.Count > 1 )
          && ( this.Children[ this.Children.Count - 2 ] is Microsoft.Maui.Controls.Shapes.Path path )
          && ( path.ClassId == BorderShapeGrid.PathOutline );
    }

    private bool IsFrameworkRoundRectangleOutline()
    {
      return ( this.Children.Count > 1 )
          && ( this.Children[ this.Children.Count - 2 ] is RoundRectangle );
    }

    private bool IsPathFlatOutline()
    {
      return ( this.Children.Count > 1 )
          && ( this.Children[ this.Children.Count - 2 ] is Microsoft.Maui.Controls.Shapes.Path path )
          && ( path.ClassId == BorderShapeGrid.FlatRectanglePathOutline );
    }

    private Shape GetFlatRectanglePathOutline()
    {
      var path = new Microsoft.Maui.Controls.Shapes.Path();
      path.StrokeThickness = 0;

      path.SetBinding( Microsoft.Maui.Controls.Shapes.Path.FillProperty, "BorderBrush" );
      path.BindingContext = this;

      path.ClassId = BorderShapeGrid.FlatRectanglePathOutline;

      return path;
    }

    private Shape SetFlatRectangleOutline( Microsoft.Maui.Controls.Shapes.Path path, double width, double height )
    {
      if( path == null )
        return null;

      var thickness = this.BorderThickness.Left;
      var pathFigureCollection = new PathFigureCollection();

      // Top lines
      var pathFigure = new PathFigure() { StartPoint = new Point( 0, 0 ) };
      pathFigure.Segments.Add( new LineSegment( new Point( width, 0 ) ) );
      pathFigure.Segments.Add( new LineSegment( new Point( width, thickness ) ) );
      pathFigure.Segments.Add( new LineSegment( new Point( 0, thickness ) ) );
      pathFigureCollection.Add( pathFigure );
      // Right lines
      pathFigure = new PathFigure() { StartPoint = new Point( width, 0 ) };
      pathFigure.Segments.Add( new LineSegment( new Point( width, height ) ) );
      pathFigure.Segments.Add( new LineSegment( new Point( width - thickness, height ) ) );
      pathFigure.Segments.Add( new LineSegment( new Point( width - thickness, 0 ) ) );
      pathFigureCollection.Add( pathFigure );
      // Bottom lines
      pathFigure = new PathFigure() { StartPoint = new Point( width, height ) };
      pathFigure.Segments.Add( new LineSegment( new Point( 0, height ) ) );
      pathFigure.Segments.Add( new LineSegment( new Point( 0, height - thickness ) ) );
      pathFigure.Segments.Add( new LineSegment( new Point( width, height - thickness ) ) );
      pathFigureCollection.Add( pathFigure );
      // Left lines
      pathFigure = new PathFigure() { StartPoint = new Point( 0, height ) };
      pathFigure.Segments.Add( new LineSegment( new Point( 0, 0 ) ) );
      pathFigure.Segments.Add( new LineSegment( new Point( thickness, 0 ) ) );
      pathFigure.Segments.Add( new LineSegment( new Point( thickness, height ) ) );
      pathFigureCollection.Add( pathFigure );

      path.Data = new PathGeometry( pathFigureCollection );

      return path;
    }

    private Shape GetPathOutline()
    {
      var path = new Microsoft.Maui.Controls.Shapes.Path();
      path.StrokeThickness = 0;

      path.SetBinding( Microsoft.Maui.Controls.Shapes.Path.FillProperty, "BorderBrush" );
      path.BindingContext = this;

      path.ClassId = BorderShapeGrid.PathOutline;

      return path;
    }

    private Shape SetPathOutline( Microsoft.Maui.Controls.Shapes.Path path, double width, double height )
    {
      if( path == null )
        return null;

      var pathFigureCollection = new PathFigureCollection();

      var topLeftCornerRadius = BorderShapeGrid.ClampCornerRadius( this.CornerRadius.TopLeft, width, height );
      var topRightCornerRadius = BorderShapeGrid.ClampCornerRadius( this.CornerRadius.TopRight, width, height );
      var bottomLeftCornerRadius = BorderShapeGrid.ClampCornerRadius( this.CornerRadius.BottomLeft, width, height );
      var bottomRightCornerRadius = BorderShapeGrid.ClampCornerRadius( this.CornerRadius.BottomRight, width, height );

      var topLeftCornerOffset = topLeftCornerRadius - ( topLeftCornerRadius * BorderShapeGrid.K_RATIO );
      var topRightCornerOffset = topRightCornerRadius - ( topRightCornerRadius * BorderShapeGrid.K_RATIO );
      var bottomLeftCornerOffset = bottomLeftCornerRadius - ( bottomLeftCornerRadius * BorderShapeGrid.K_RATIO );
      var bottomRightCornerOffset = bottomRightCornerRadius - ( bottomRightCornerRadius * BorderShapeGrid.K_RATIO );

      var innerTopLeftLeftCornerOffset = topLeftCornerOffset + ( this.BorderThickness.Left * BorderShapeGrid.K_RATIO );
      var innerTopLeftTopCornerOffset = topLeftCornerOffset + ( this.BorderThickness.Top * BorderShapeGrid.K_RATIO );
      var innerTopRightRightCornerOffset = topRightCornerOffset + ( this.BorderThickness.Right * BorderShapeGrid.K_RATIO );
      var innerTopRightTopCornerOffset = topRightCornerOffset + ( this.BorderThickness.Top * BorderShapeGrid.K_RATIO );
      var innerBottomRightRightCornerOffset = bottomRightCornerOffset + ( this.BorderThickness.Right * BorderShapeGrid.K_RATIO );
      var innerBottomRightBottomCornerOffset = bottomRightCornerOffset + ( this.BorderThickness.Bottom * BorderShapeGrid.K_RATIO );
      var innerBottomLeftLeftCornerOffset = bottomLeftCornerOffset + ( this.BorderThickness.Left * BorderShapeGrid.K_RATIO );
      var innerBottomLeftBottomCornerOffset = bottomLeftCornerOffset + ( this.BorderThickness.Bottom * BorderShapeGrid.K_RATIO );

      // TopLeft Arcs
      var pathFigure = new PathFigure() { StartPoint = new Point( 0, topLeftCornerRadius ) };
      var bezierCurve = new BezierSegment( new Point( 0, topLeftCornerOffset ), new Point( topLeftCornerOffset, 0 ), new Point( topLeftCornerRadius, 0 ) );
      pathFigure.Segments.Add( bezierCurve );
      if( topLeftCornerRadius <= this.BorderThickness.Left )
      {
        pathFigure.Segments.Add( new LineSegment( new Point( this.BorderThickness.Left, 0 ) ) );
        if( topLeftCornerRadius <= this.BorderThickness.Top )
        {
          pathFigure.Segments.Add( new LineSegment( new Point( this.BorderThickness.Left, this.BorderThickness.Top ) ) );
          pathFigure.Segments.Add( new LineSegment( new Point( 0, this.BorderThickness.Top ) ) );
        }
        else
        {
          pathFigure.Segments.Add( new LineSegment( new Point( this.BorderThickness.Left, topLeftCornerRadius ) ) );
        }
      }
      else
      {
        pathFigure.Segments.Add( new LineSegment( new Point( topLeftCornerRadius, this.BorderThickness.Top ) ) );
        if( topLeftCornerRadius <= this.BorderThickness.Top )
        {
          pathFigure.Segments.Add( new LineSegment( new Point( 0, this.BorderThickness.Top ) ) );
        }
        else
        {
          bezierCurve = new BezierSegment( new Point( innerTopLeftLeftCornerOffset, this.BorderThickness.Top ), new Point( this.BorderThickness.Left, innerTopLeftTopCornerOffset ), new Point( this.BorderThickness.Left, topLeftCornerRadius ) );
          pathFigure.Segments.Add( bezierCurve );
        }
      }
      pathFigureCollection.Add( pathFigure );

      // Top lines
      pathFigure = new PathFigure() { StartPoint = new Point( topLeftCornerRadius, 0 ) };
      pathFigure.Segments.Add( new LineSegment( new Point( width - topRightCornerRadius, 0 ) ) );
      pathFigure.Segments.Add( new LineSegment( new Point( width - topRightCornerRadius, this.BorderThickness.Top ) ) );
      pathFigure.Segments.Add( new LineSegment( new Point( topLeftCornerRadius, this.BorderThickness.Top ) ) );
      pathFigureCollection.Add( pathFigure );

      // TopRight Arcs
      pathFigure = new PathFigure() { StartPoint = new Point( width - topRightCornerRadius, 0 ) };
      bezierCurve = new BezierSegment( new Point( width - topRightCornerOffset, 0 ), new Point( width, topRightCornerOffset ), new Point( width, topRightCornerRadius ) );
      pathFigure.Segments.Add( bezierCurve );
      if( topRightCornerRadius <= this.BorderThickness.Top )
      {
        pathFigure.Segments.Add( new LineSegment( new Point( width, this.BorderThickness.Top ) ) );
        if( topRightCornerRadius <= this.BorderThickness.Right )
        {
          pathFigure.Segments.Add( new LineSegment( new Point( width - this.BorderThickness.Right, this.BorderThickness.Top ) ) );
          pathFigure.Segments.Add( new LineSegment( new Point( width - this.BorderThickness.Right, 0 ) ) );
        }
        else
        {
          pathFigure.Segments.Add( new LineSegment( new Point( width - topRightCornerRadius, this.BorderThickness.Top ) ) );
        }
      }
      else
      {
        pathFigure.Segments.Add( new LineSegment( new Point( width - this.BorderThickness.Right, topRightCornerRadius ) ) );
        if( topRightCornerRadius <= this.BorderThickness.Right )
        {
          pathFigure.Segments.Add( new LineSegment( new Point( width - this.BorderThickness.Right, 0 ) ) );
        }
        else
        {
          bezierCurve = new BezierSegment( new Point( width - this.BorderThickness.Right, innerTopRightTopCornerOffset ), new Point( width - innerTopRightRightCornerOffset, this.BorderThickness.Top ), new Point( width - topRightCornerRadius, this.BorderThickness.Top ) );
          pathFigure.Segments.Add( bezierCurve );
        }
      }
      pathFigureCollection.Add( pathFigure );

      // Right lines
      pathFigure = new PathFigure() { StartPoint = new Point( width, topRightCornerRadius ) };
      pathFigure.Segments.Add( new LineSegment( new Point( width, height - bottomRightCornerRadius ) ) );
      pathFigure.Segments.Add( new LineSegment( new Point( width - this.BorderThickness.Right, height - bottomRightCornerRadius ) ) );
      pathFigure.Segments.Add( new LineSegment( new Point( width - this.BorderThickness.Right, topRightCornerRadius ) ) );
      pathFigureCollection.Add( pathFigure );

      // BottomRight Arcs
      pathFigure = new PathFigure() { StartPoint = new Point( width, height - bottomRightCornerRadius ) };
      bezierCurve = new BezierSegment( new Point( width, height - bottomRightCornerOffset ), new Point( width - bottomRightCornerOffset, height ), new Point( width - bottomRightCornerRadius, height ) );
      pathFigure.Segments.Add( bezierCurve );
      if( bottomRightCornerRadius <= this.BorderThickness.Right )
      {
        pathFigure.Segments.Add( new LineSegment( new Point( width - this.BorderThickness.Right, height ) ) );
        if( bottomRightCornerRadius <= this.BorderThickness.Bottom )
        {
          pathFigure.Segments.Add( new LineSegment( new Point( width - this.BorderThickness.Right, height - this.BorderThickness.Bottom ) ) );
          pathFigure.Segments.Add( new LineSegment( new Point( width, height - this.BorderThickness.Bottom ) ) );
        }
        else
        {
          pathFigure.Segments.Add( new LineSegment( new Point( width - this.BorderThickness.Right, height - bottomRightCornerRadius ) ) );
        }
      }
      else
      {
        pathFigure.Segments.Add( new LineSegment( new Point( width - bottomRightCornerRadius, height - this.BorderThickness.Bottom ) ) );
        if( bottomRightCornerRadius <= this.BorderThickness.Bottom )
        {
          pathFigure.Segments.Add( new LineSegment( new Point( width, height - this.BorderThickness.Bottom ) ) );
        }
        else
        {
          bezierCurve = new BezierSegment( new Point( width - innerBottomRightRightCornerOffset, height - this.BorderThickness.Bottom ), new Point( width - this.BorderThickness.Right, height - innerBottomRightBottomCornerOffset ), new Point( width - this.BorderThickness.Right, height - bottomRightCornerRadius ) );
          pathFigure.Segments.Add( bezierCurve );
        }
      }
      pathFigureCollection.Add( pathFigure );

      // Bottom lines
      pathFigure = new PathFigure() { StartPoint = new Point( width - bottomRightCornerRadius, height ) };
      pathFigure.Segments.Add( new LineSegment( new Point( bottomLeftCornerRadius, height ) ) );
      pathFigure.Segments.Add( new LineSegment( new Point( bottomLeftCornerRadius, height - this.BorderThickness.Bottom ) ) );
      pathFigure.Segments.Add( new LineSegment( new Point( width - bottomRightCornerRadius, height - this.BorderThickness.Bottom ) ) );
      pathFigureCollection.Add( pathFigure );

      // BottomLeft Arcs
      pathFigure = new PathFigure() { StartPoint = new Point( bottomLeftCornerRadius, height ) };
      bezierCurve = new BezierSegment( new Point( bottomLeftCornerOffset, height ), new Point( 0, height - bottomLeftCornerOffset ), new Point( 0, height - bottomLeftCornerRadius ) );
      pathFigure.Segments.Add( bezierCurve );
      if( bottomLeftCornerRadius <= this.BorderThickness.Bottom )
      {
        pathFigure.Segments.Add( new LineSegment( new Point( 0, height - this.BorderThickness.Bottom ) ) );
        if( bottomLeftCornerRadius <= this.BorderThickness.Left )
        {
          pathFigure.Segments.Add( new LineSegment( new Point( this.BorderThickness.Left, height - this.BorderThickness.Bottom ) ) );
          pathFigure.Segments.Add( new LineSegment( new Point( this.BorderThickness.Left, height ) ) );
        }
        else
        {
          pathFigure.Segments.Add( new LineSegment( new Point( bottomLeftCornerRadius, height - this.BorderThickness.Bottom ) ) );
        }
      }
      else
      {
        pathFigure.Segments.Add( new LineSegment( new Point( this.BorderThickness.Left, height - bottomLeftCornerRadius ) ) );
        if( bottomLeftCornerRadius <= this.BorderThickness.Left )
        {
          pathFigure.Segments.Add( new LineSegment( new Point( this.BorderThickness.Left, height ) ) );
        }
        else
        {
          bezierCurve = new BezierSegment( new Point( this.BorderThickness.Left, height - innerBottomLeftBottomCornerOffset ), new Point( innerBottomLeftLeftCornerOffset, height - this.BorderThickness.Bottom ), new Point( bottomLeftCornerRadius, height - this.BorderThickness.Bottom ) );
          pathFigure.Segments.Add( bezierCurve );
        }
      }
      pathFigureCollection.Add( pathFigure );

      // Left lines
      pathFigure = new PathFigure() { StartPoint = new Point( 0, height - bottomLeftCornerRadius ) };
      pathFigure.Segments.Add( new LineSegment( new Point( 0, topLeftCornerRadius ) ) );
      pathFigure.Segments.Add( new LineSegment( new Point( this.BorderThickness.Left, topLeftCornerRadius ) ) );
      pathFigure.Segments.Add( new LineSegment( new Point( this.BorderThickness.Left, height - bottomLeftCornerRadius ) ) );
      pathFigureCollection.Add( pathFigure );

      path.Data = new PathGeometry( pathFigureCollection );

      return path;
    }

    private Shape GetFrameworkRoundedRectangleOutline()
    {
      var rectangle = new RoundRectangle();

      rectangle.SetBinding( RoundRectangle.StrokeProperty, "BorderBrush" );
      rectangle.SetBinding( RoundRectangle.StrokeThicknessProperty, "BorderThickness.Left" );
      rectangle.SetBinding( RoundRectangle.CornerRadiusProperty, "CornerRadius" );
      rectangle.BindingContext = this;

      rectangle.ClassId = BorderShapeGrid.FrameworkRectangleOutline;

      return rectangle;
    }

    private Shape GetBackgroundRoundedRectangle()
    {
      if( this.Background == null )
        return null;

      var rectangle = new RoundRectangle();

      rectangle.SetBinding( RoundRectangle.StrokeThicknessProperty, "BorderThickness.Left" );
      rectangle.SetBinding( RoundRectangle.FillProperty, "Background" );
      rectangle.SetBinding( RoundRectangle.CornerRadiusProperty, "CornerRadius" );
      rectangle.BindingContext = this;

      rectangle.ClassId = BorderShapeGrid.BackgroundRectangle;

      return rectangle;
    }

    #endregion

    #region Event Handlers

    private void BorderShape_HandlerChanged( object sender, EventArgs e )
    {
      this.InitializeForPlatform( sender, e );

      this.UpdateShape();
    }

    private void BorderShape_HandlerChanging( object sender, HandlerChangingEventArgs e )
    {
      this.UninitializeForPlatform( sender, e );
    }

    #endregion
  }
}
