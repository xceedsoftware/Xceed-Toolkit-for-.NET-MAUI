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
  public partial class BorderGraphicsView : GraphicsView
  {
    #region Constructors

    public BorderGraphicsView()
    {
      this.Drawable = new BorderDrawable();

      this.HandlerChanged += this.BorderGraphicsView_HandlerChanged;
      this.HandlerChanging += this.Border_HandlerChanging;
    }

    #endregion

    #region Partial Methods

    partial void InitializeForPlatform( object sender, EventArgs e );

    partial void UninitializeForPlatform( object sender, HandlerChangingEventArgs e );

    #endregion

    #region Public Properties

    #region Background

    public static new readonly BindableProperty BackgroundProperty = BindableProperty.Create( nameof( Background ), typeof( Brush ), typeof( BorderGraphicsView ), null, propertyChanged: OnBackgroundChanged );

    public new Brush Background
    {
      get => ( Brush )GetValue( BackgroundProperty );
      set => SetValue( BackgroundProperty, value );
    }

    private static void OnBackgroundChanged( BindableObject bindable, object oldValue, object newValue )
    {
      var view = bindable as BorderGraphicsView;
      if( view != null )
      {
        view.OnBackgroundChanged( oldValue as Brush, newValue as Brush );
      }
    }

    internal virtual void OnBackgroundChanged( Brush oldValue, Brush newValue )
    {
      var drawable = this.Drawable as BorderDrawable;
      if( drawable != null )
      {
        drawable.Background = newValue;
        this.Invalidate();
      }
    }

    #endregion

    #region BorderBrush

    public static readonly BindableProperty BorderBrushProperty = BindableProperty.Create( nameof( BorderBrush ), typeof( Brush ), typeof( BorderGraphicsView ), null, propertyChanged: OnBorderBrushChanged );

    public Brush BorderBrush
    {
      get => ( Brush )GetValue( BorderBrushProperty );
      set => SetValue( BorderBrushProperty, value );
    }

    private static void OnBorderBrushChanged( BindableObject bindable, object oldValue, object newValue )
    {
      var view = bindable as BorderGraphicsView;
      if( view != null )
      {
        view.OnBorderBrushChanged( oldValue as Brush, newValue as Brush );
      }
    }

    internal virtual void OnBorderBrushChanged( Brush oldValue, Brush newValue )
    {
      var drawable = this.Drawable as BorderDrawable;
      if( drawable != null )
      {
        drawable.BorderBrush = newValue;
        this.Invalidate();
      }
    }

    #endregion

    #region BorderThickness

    public static readonly BindableProperty BorderThicknessProperty = BindableProperty.Create( nameof( BorderThickness ), typeof( Thickness ), typeof( BorderGraphicsView ), Thickness.Zero, propertyChanged: OnBorderThicknessChanged );

    public Thickness BorderThickness
    {
      get => ( Thickness )GetValue( BorderThicknessProperty );
      set => SetValue( BorderThicknessProperty, value );
    }

    private static void OnBorderThicknessChanged( BindableObject bindable, object oldValue, object newValue )
    {
      var view = bindable as BorderGraphicsView;
      if( view != null )
      {
        view.OnBorderThicknessChanged( (Thickness)oldValue, (Thickness)newValue );
      }
    }

    internal virtual void OnBorderThicknessChanged( Thickness oldValue, Thickness newValue )
    {
      var drawable = this.Drawable as BorderDrawable;
      if( drawable != null )
      {
        drawable.BorderThickness = newValue;
        this.Invalidate();
      }
    }

    #endregion

    #region CornerRadius

    public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create( nameof( CornerRadius ), typeof( CornerRadius ), typeof( BorderGraphicsView ), default( CornerRadius ), propertyChanged: OnCornerRadiusChanged );

    public CornerRadius CornerRadius
    {
      get => ( CornerRadius )GetValue( CornerRadiusProperty );
      set => SetValue( CornerRadiusProperty, value );
    }

    private static void OnCornerRadiusChanged( BindableObject bindable, object oldValue, object newValue )
    {
      var view = bindable as BorderGraphicsView;
      if( view != null )
      {
        view.OnCornerRadiusChanged( ( CornerRadius )oldValue, ( CornerRadius )newValue );
      }
    }

    internal virtual void OnCornerRadiusChanged( CornerRadius oldValue, CornerRadius newValue )
    {
      var drawable = this.Drawable as BorderDrawable;
      if( drawable != null )
      {
        drawable.CornerRadius = newValue;
        this.Invalidate();
      }
    }

    #endregion

    #endregion

    #region Event Handlers

    private void BorderGraphicsView_HandlerChanged( object sender, EventArgs e )
    {
      this.InitializeForPlatform( sender, e );
    }

    private void Border_HandlerChanging( object sender, HandlerChangingEventArgs e )
    {
      this.UninitializeForPlatform( sender, e );
    }

    #endregion
  }
}
