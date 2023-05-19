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
  public class BorderDrawable : IDrawable
  {
    #region Public Properties

    #region Background

    public Brush Background 
    {
      get;
      set;
    }

    #endregion

    #region BorderBrush

    public Brush BorderBrush
    {
      get;
      set;
    }

    #endregion

    #region BorderThickness

    public Thickness BorderThickness
    {
      get;
      set;
    }

    #endregion

    #region CornerRadius

    public CornerRadius CornerRadius
    {
      get;
      set;
    }

    #endregion

    #endregion

    #region Public Methods

    public void Draw( ICanvas canvas, RectF dirtyRect )
    {
      this.DrawBackground( canvas, dirtyRect );

      if( this.IsUniformOutlines() )
      {
        this.DrawUniformOutline( canvas, dirtyRect );
      }
      else
      {
        this.DrawOutline( canvas, dirtyRect );
      }
    }

    #endregion

    #region Private Methods

    private bool IsUniformOutlines() 
    {
      return ( this.BorderThickness.Left == this.BorderThickness.Right )
          && ( this.BorderThickness.Left == this.BorderThickness.Top )
          && ( this.BorderThickness.Left == this.BorderThickness.Bottom );
    }

    private void DrawBackground( ICanvas canvas, RectF dirtyRect )
    {
      if( canvas == null )
        throw new ArgumentNullException( nameof( canvas ) );

      var backgroundColorBrush = this.Background as SolidColorBrush;
      if( backgroundColorBrush == null )
        return;

      var borderLeftSize = Convert.ToSingle( this.BorderThickness.Left );
      var borderRightSize = Convert.ToSingle( this.BorderThickness.Right );
      var borderTopSize = Convert.ToSingle( this.BorderThickness.Top );
      var borderBottomSize = Convert.ToSingle( this.BorderThickness.Bottom );

      canvas.FillColor = backgroundColorBrush.Color;

      canvas.FillRoundedRectangle( dirtyRect.X + ( borderLeftSize / 2 ), 
                                   dirtyRect.Y + ( borderTopSize / 2 ), 
                                   dirtyRect.Width - ( borderLeftSize / 2 ) - ( borderRightSize / 2 ), 
                                   dirtyRect.Height - ( borderTopSize / 2 ) - ( borderBottomSize / 2 ), 
                                   Convert.ToSingle( this.CornerRadius.TopLeft ), 
                                   Convert.ToSingle( this.CornerRadius.TopRight ), 
                                   Convert.ToSingle( this.CornerRadius.BottomLeft ), 
                                   Convert.ToSingle( this.CornerRadius.BottomRight ) );
    }

    private void DrawUniformOutline( ICanvas canvas, RectF dirtyRect )
    {
      if( canvas == null )
        throw new ArgumentNullException( nameof( canvas ) );

      var outlineColorBrush = this.BorderBrush as SolidColorBrush;
      if( outlineColorBrush == null )
        return;

      if( this.BorderThickness.IsEmpty )
        return;

      var borderSize = Convert.ToSingle( this.BorderThickness.Left );
      var halfBorderSize = borderSize / 2;

      canvas.StrokeColor = outlineColorBrush.Color;
      canvas.StrokeSize = borderSize;

      canvas.DrawRoundedRectangle( dirtyRect.X + halfBorderSize,
                                   dirtyRect.Y + halfBorderSize,
                                   dirtyRect.Width - borderSize,
                                   dirtyRect.Height - borderSize,
                                   Convert.ToSingle( this.CornerRadius.TopLeft ),
                                   Convert.ToSingle( this.CornerRadius.TopRight ),
                                   Convert.ToSingle( this.CornerRadius.BottomLeft ),
                                   Convert.ToSingle( this.CornerRadius.BottomRight ) );
    }

    private void DrawOutline( ICanvas canvas, RectF dirtyRect )
    {
      if( canvas == null )
        throw new ArgumentNullException( nameof( canvas ) );

      var outlineColorBrush = this.BorderBrush as SolidColorBrush;
      if( outlineColorBrush == null )
        return;

      canvas.StrokeColor = outlineColorBrush.Color;
      canvas.FillColor = outlineColorBrush.Color;

      var topLeftCornerRadius = Convert.ToSingle( this.CornerRadius.TopLeft );
      var topRightCornerRadius = Convert.ToSingle( this.CornerRadius.TopRight );
      var bottomLeftCornerRadius = Convert.ToSingle( this.CornerRadius.BottomLeft );
      var bottomRightCornerRadius = Convert.ToSingle( this.CornerRadius.BottomRight );
      var topBorderThickness = Convert.ToSingle( this.BorderThickness.Top );
      var bottomBorderThickness = Convert.ToSingle( this.BorderThickness.Bottom );
      var leftBorderThickness = Convert.ToSingle( this.BorderThickness.Left );
      var rightBorderThickness = Convert.ToSingle( this.BorderThickness.Right );

      // Top Line.
      if( topBorderThickness >= 1 )
      {
        var halfBorderThickness = topBorderThickness / 2;
        canvas.StrokeSize = topBorderThickness;
        canvas.DrawLine( dirtyRect.X + topLeftCornerRadius, dirtyRect.Y + halfBorderThickness, dirtyRect.X + dirtyRect.Width - topRightCornerRadius, dirtyRect.Y + halfBorderThickness );
      }

      // Right Line.
      if( rightBorderThickness >= 1 )
      {
        var halfBorderThickness = rightBorderThickness / 2;
        canvas.StrokeSize = rightBorderThickness;
        canvas.DrawLine( dirtyRect.X + dirtyRect.Width - halfBorderThickness, dirtyRect.Y + topRightCornerRadius, dirtyRect.X + dirtyRect.Width - halfBorderThickness, dirtyRect.Y + dirtyRect.Height - bottomRightCornerRadius );
      }

      // Bottom line.
      if( bottomBorderThickness >= 1 )
      {
        var halfBorderThickness = bottomBorderThickness / 2;
        canvas.StrokeSize = bottomBorderThickness;
        canvas.DrawLine( dirtyRect.X + dirtyRect.Width - bottomRightCornerRadius, dirtyRect.Y + dirtyRect.Height - halfBorderThickness, dirtyRect.X + bottomLeftCornerRadius, dirtyRect.Y + dirtyRect.Height - halfBorderThickness );
      }

      // Left Line.
      if( leftBorderThickness >= 1 )
      {
        var halfBorderThickness = leftBorderThickness / 2;
        canvas.StrokeSize = leftBorderThickness;
        canvas.DrawLine( dirtyRect.X + halfBorderThickness, dirtyRect.Y + dirtyRect.Height - bottomLeftCornerRadius, dirtyRect.X + halfBorderThickness, dirtyRect.Y + topLeftCornerRadius );
      }

      // Top-right Arc.
      if( ( rightBorderThickness >= 1 ) || ( topBorderThickness >= 1 ) )
      {
        canvas.StrokeSize = 1;
        var topRightCornerPath = new PathF();
        topRightCornerPath.AddArc( dirtyRect.X + dirtyRect.Width - ( topRightCornerRadius * 2 ), dirtyRect.Y, dirtyRect.X + dirtyRect.Width, dirtyRect.Y + ( topRightCornerRadius * 2 ), 90, 0, true );
        topRightCornerPath.LineTo( dirtyRect.X + dirtyRect.Width - rightBorderThickness, dirtyRect.Y + topRightCornerRadius );
        var pt1 = new Point( dirtyRect.X + dirtyRect.Width - ( topRightCornerRadius * 2 ), dirtyRect.Y + topBorderThickness );
        var pt2 = new Point( dirtyRect.X + dirtyRect.Width - rightBorderThickness, dirtyRect.Y + ( topRightCornerRadius * 2 ) );
        topRightCornerPath.AddArc( new Point( ( pt1.X <= pt2.X ) ? pt1.X : pt2.X, ( pt1.Y <= pt2.Y ) ? pt1.Y : pt2.Y ), new Point( ( pt1.X > pt2.X ) ? pt1.X : pt2.X, ( pt1.Y > pt2.Y ) ? pt1.Y : pt2.Y ), 0, 90, false );
        topRightCornerPath.Close();
        canvas.FillPath( topRightCornerPath );
      }

      // Bottom-Right Arc.
      if( ( rightBorderThickness >= 1 ) || ( bottomBorderThickness >= 1 ) )
      {
        var bottomRightCornerPath = new PathF();
        bottomRightCornerPath.AddArc( dirtyRect.X + dirtyRect.Width - ( bottomRightCornerRadius * 2 ), dirtyRect.Y + dirtyRect.Height - ( bottomRightCornerRadius * 2 ), dirtyRect.X + dirtyRect.Width, dirtyRect.Y + dirtyRect.Height, 0, 270, true );
        bottomRightCornerPath.LineTo( dirtyRect.X + dirtyRect.Width - bottomRightCornerRadius, dirtyRect.Y + dirtyRect.Height - bottomBorderThickness );
        var pt1 = new Point( dirtyRect.X + dirtyRect.Width - rightBorderThickness, dirtyRect.Y + dirtyRect.Height - bottomBorderThickness );
        var pt2 = new Point( dirtyRect.X + dirtyRect.Width - ( bottomRightCornerRadius * 2 ), dirtyRect.Y + dirtyRect.Height - ( bottomRightCornerRadius * 2 ) );
        bottomRightCornerPath.AddArc( new Point( ( pt1.X <= pt2.X ) ? pt1.X : pt2.X, ( pt1.Y <= pt2.Y ) ? pt1.Y : pt2.Y ), new Point( ( pt1.X > pt2.X ) ? pt1.X : pt2.X, ( pt1.Y > pt2.Y ) ? pt1.Y : pt2.Y ), 270, 0, false );
        bottomRightCornerPath.Close();
        canvas.FillPath( bottomRightCornerPath );
      }

      // Bottom-Left Arc.
      if( ( leftBorderThickness >= 1 ) || ( bottomBorderThickness >= 1 ) )
      {
        var bottomLeftCornerPath = new PathF();
        bottomLeftCornerPath.AddArc( dirtyRect.X, dirtyRect.Y + dirtyRect.Height - ( bottomLeftCornerRadius * 2 ), dirtyRect.X + ( bottomLeftCornerRadius * 2 ), dirtyRect.Y + dirtyRect.Height, 270, 180, true );
        bottomLeftCornerPath.LineTo( dirtyRect.X + leftBorderThickness, dirtyRect.Y + dirtyRect.Height - bottomLeftCornerRadius );
        var pt1 = new Point( dirtyRect.X + leftBorderThickness, dirtyRect.Y + dirtyRect.Height - ( bottomLeftCornerRadius * 2 ) );
        var pt2 = new Point( dirtyRect.X + ( bottomLeftCornerRadius * 2 ), dirtyRect.Y + dirtyRect.Height - bottomBorderThickness );
        bottomLeftCornerPath.AddArc( new Point( (pt1.X <= pt2.X) ? pt1.X : pt2.X, (pt1.Y <= pt2.Y) ? pt1.Y : pt2.Y), new Point( ( pt1.X > pt2.X ) ? pt1.X : pt2.X, ( pt1.Y > pt2.Y ) ? pt1.Y : pt2.Y ), 180, 270, false );
        bottomLeftCornerPath.Close();
        canvas.FillPath( bottomLeftCornerPath );
      }

      // Top-Left Arc.
      if( ( leftBorderThickness >= 1 ) || ( topBorderThickness >= 1 ) )
      {
        var topLeftCornerPath = new PathF();
        topLeftCornerPath.AddArc( dirtyRect.X, dirtyRect.Y, dirtyRect.X + ( topLeftCornerRadius * 2 ), dirtyRect.Y + ( topLeftCornerRadius * 2 ), 180, 90, true );
        topLeftCornerPath.LineTo( dirtyRect.X + topLeftCornerRadius, dirtyRect.Y + topBorderThickness );
        var pt1 = new Point( dirtyRect.X + leftBorderThickness, dirtyRect.Y + topBorderThickness );
        var pt2 = new Point( dirtyRect.X + ( topLeftCornerRadius * 2 ), dirtyRect.Y + ( topLeftCornerRadius * 2 ) );
        topLeftCornerPath.AddArc( new Point( ( pt1.X <= pt2.X ) ? pt1.X : pt2.X, ( pt1.Y <= pt2.Y ) ? pt1.Y : pt2.Y ), new Point( ( pt1.X > pt2.X ) ? pt1.X : pt2.X, ( pt1.Y > pt2.Y ) ? pt1.Y : pt2.Y ), 90, 180, false );
        topLeftCornerPath.Close();
        canvas.FillPath( topLeftCornerPath );
      }
    }

    #endregion
  }
}
