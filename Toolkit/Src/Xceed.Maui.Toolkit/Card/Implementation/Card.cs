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
  public class Card : ContentControl
  {
    #region Public Properties

    #region ShadowBrush

    public static readonly BindableProperty ShadowBrushProperty = BindableProperty.Create( nameof( ShadowBrush ), typeof( Brush ), typeof( Card ) );

    public Brush ShadowBrush
    {
      get => ( Brush )GetValue( ShadowBrushProperty );
      set => SetValue( ShadowBrushProperty, value );
    }
    #endregion    

    #region ShadowOffset

    public static readonly BindableProperty ShadowOffsetProperty = BindableProperty.Create( nameof( ShadowOffset ), typeof( Point ), typeof( Card ) );

    public Point ShadowOffset
    {
      get => ( Point )GetValue( ShadowOffsetProperty );
      set => SetValue( ShadowOffsetProperty, value );
    }
    #endregion

    #region ShadowOpacity

    public static readonly BindableProperty ShadowOpacityProperty = BindableProperty.Create( nameof( ShadowOpacity ), typeof( float ), typeof( Card ), propertyChanged: OnShadowOpacityChanged );

    public float ShadowOpacity
    {
      get => ( float )GetValue( ShadowOpacityProperty );
      set => SetValue( ShadowOpacityProperty, value );
    }

    private static void OnShadowOpacityChanged( BindableObject bindable, object oldValue, object newValue )
    {
      var card = bindable as Card;
      if( card != null )
      {
        if( ( float )newValue >= 0 && ( float )newValue <= 1 )
        {
          card.OnShadowOpacityChanged( ( float )oldValue, ( float )newValue );
        }
        else
        {
          throw new InvalidDataException( "ShadowOpacity value must be between 0 and 1" );
        }
      }
    }

    protected virtual void OnShadowOpacityChanged( float oldValue, float newValue )
    {
    }


    #endregion

    #region ShadowRadius

    public static readonly BindableProperty ShadowRadiusProperty = BindableProperty.Create( nameof( ShadowRadius ), typeof( float ), typeof( Card ), propertyChanged: OnShadowRadiusChanged);

    public float ShadowRadius
    {
      get => ( float )GetValue( ShadowRadiusProperty );
      set => SetValue( ShadowRadiusProperty, value );
    }

    private static void OnShadowRadiusChanged( BindableObject bindable, object oldValue, object newValue )
    {
      var card = bindable as Card;
      if( card != null )
      {
        if( ( float )newValue >= 0 )
        {
          card.OnShadowRadiusChanged( ( float )oldValue, ( float )newValue );
        }
        else
        {
          throw new InvalidDataException( "ShadowRadius value cannot be negative" );
        }
      }      
    }

    protected virtual void OnShadowRadiusChanged( float oldValue, float newValue )
    {
    }

    #endregion

    #region Title

    public static readonly BindableProperty TitleProperty = BindableProperty.Create( nameof( Title ), typeof( object ), typeof( Card ) );

    public object Title
    {
      get => ( object )GetValue( TitleProperty );
      set => SetValue( TitleProperty, value );
    }
    #endregion

    #endregion
  }
}
