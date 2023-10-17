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


using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Xceed.Maui.Toolkit
{
  [DefaultProperty( "Content" )]
  [ContentProperty( "Content" )]
  public partial class Border : Control
  {
    #region Construtors

    public Border()
    {
      this.HandlerChanged += this.Border_HandlerChanged;
      this.HandlerChanging += this.Border_HandlerChanging;
    }

    #endregion

    #region Partial Methods

    partial void InitializeForPlatform( object sender, EventArgs e );

    partial void UninitializeForPlatform( object sender, HandlerChangingEventArgs e );

    #endregion    

    #region Public Properties

    #region Content

    public static readonly BindableProperty ContentProperty = BindableProperty.Create( nameof( Content ), typeof( View ), typeof( Border ), propertyChanged: OnContentChanged  );

    public View Content
    {
      get => ( View )GetValue( ContentProperty );
      set => SetValue( ContentProperty, value );
    }

    private static void OnContentChanged( BindableObject bindable, object oldValue, object newValue )
    {
      var border = bindable as Border;
      if( border != null )
      {
        border.OnContentChanged( ( View )oldValue, ( View )newValue );
      }
    }

    protected virtual void OnContentChanged( View oldValue, View newValue )
    {
      // Update this Border's Children label's and Entry's Font properties, when not already set.
      Control.UpdateFontProperties( newValue, this );
    }

    #endregion

    #endregion

    #region Protected Methods

    protected override void OnPropertyChanging( [CallerMemberName] string propertyName = null )
    {
      if( propertyName == "BackgroundColor" )
        throw new InvalidDataException( "BackgroundColor is not available in Border. Use the Background property instead." );

      base.OnPropertyChanging( propertyName );
    }

    protected override void OnTextColorChanged( Color oldValue, Color newValue )
    {
      base.OnTextColorChanged( oldValue, newValue );

      // Update this Border's Children label's and Entry's Font properties, when not already set.
      Control.UpdateFontTextColor( this.Content, oldValue, newValue );
    }

    protected override void OnFontAttributesChanged( FontAttributes oldValue, FontAttributes newValue )
    {
      base.OnFontAttributesChanged( oldValue, newValue );

      // Update this Border's Children label's and Entry's Font properties, when not already set.
      Control.UpdateFontAttributes( this.Content, oldValue, newValue );
    }

    protected override void OnFontFamilyChanged( string oldValue, string newValue )
    {
      base.OnFontFamilyChanged( oldValue, newValue );

      // Update this Border's Children label's and Entry's Font properties, when not already set.
      Control.UpdateFontFamily( this.Content, oldValue, newValue );
    }

    protected override void OnFontSizeChanged( double oldValue, double newValue )
    {
      base.OnFontSizeChanged( oldValue, newValue );

      // Update this Border's Children label's and Entry's Font properties, when not already set.
      Control.UpdateFontSize( this.Content, oldValue, newValue );
    }

    #endregion

    #region Event Handlers

    private void Border_HandlerChanged( object sender, EventArgs e )
    {
      this.InitializeForPlatform( sender, e );
    }

    private void Border_HandlerChanging( object sender, HandlerChangingEventArgs e )
    {
      this.UninitializeForPlatform( sender, e );
    }

    #endregion

    #region Events

    public event EventHandler PointerEnter;   //Windows and Mac only

    internal void RaisePointerEnterEvent( object sender, EventArgs e )
    {
      if( this.IsEnabled )
      {
        this.PointerEnter?.Invoke( sender, e );
      }
    }

    public event EventHandler PointerLeave;

    internal void RaisePointerLeaveEvent( object sender, EventArgs e )
    {
      if( this.IsEnabled )
      {
        this.PointerLeave?.Invoke( sender, e );
      }
    }

    public event EventHandler PointerDown;   //Windows and Mac only

    internal void RaisePointerDownEvent( object sender, EventArgs e )
    {
      if( this.IsEnabled )
      {
        this.PointerDown?.Invoke( sender, e );
      }
    }

    public event EventHandler PointerUp;

    internal void RaisePointerUpEvent( object sender, EventArgs e )
    {
      if( this.IsEnabled )
      {
        this.PointerUp?.Invoke( sender, e );
      }
    }

    #endregion
  }
}
