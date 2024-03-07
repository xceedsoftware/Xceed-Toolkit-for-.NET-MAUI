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


using Microsoft.Maui.Platform;

#nullable enable

namespace Xceed.Maui.Toolkit
{
  [ContentProperty( nameof( Content ) )]
  public class Popup : View, IPopup
  {
    #region Private Members

    private bool m_isShown;
    private bool m_focusable = true;

    #endregion

    #region Constructors

    public Popup()
    {
      // When Popup is not in a PopupContainer, Popup's parent must be set in constructor for Content's control to load correctly (loaded event callback being called).
      this.Parent = Application.Current?.MainPage;
    }

    internal Popup( PopupContainer popupContainer )
    {
      this.Parent = popupContainer;
    }

    #endregion

    #region Public Properties

    #region Anchor

    public static readonly BindableProperty AnchorProperty = BindableProperty.Create( nameof( Anchor ), typeof( View ), typeof( Popup ) );

    public View? Anchor
    {
      get => (View?)GetValue( AnchorProperty );
      set => SetValue( AnchorProperty, value );
    }

    #endregion

    #region Content

    public static readonly BindableProperty ContentProperty = BindableProperty.Create( nameof( Content ), typeof( View ), typeof( Popup ), propertyChanged: OnContentChanged );

    public virtual View? Content
    {
      get => (View?)GetValue( ContentProperty );
      set => SetValue( ContentProperty, value );
    }

    private static void OnContentChanged( BindableObject bindable, object oldValue, object newValue )
    {
      if( bindable is Popup popup )
      {
        popup.OnContentChanged( (View?)oldValue, (View?)newValue );
      }
    }

    protected virtual void OnContentChanged( View? oldValue, View? newValue )
    {
      this.OnBindingContextChanged();
    }

    #endregion

    #region IsModal

    public static readonly BindableProperty IsModalProperty = BindableProperty.Create( nameof( IsModal ), typeof( bool ), typeof( Popup ), false );

    public bool IsModal
    {
      get => (bool)GetValue( IsModalProperty );
      set => SetValue( IsModalProperty, value );
    }

    #endregion

    #region IsOpen

    public static readonly BindableProperty IsOpenProperty = BindableProperty.Create( nameof( IsOpen ), typeof( bool ), typeof( Popup ), defaultBindingMode: BindingMode.TwoWay, propertyChanged: OnIsOpenChanged );

    public bool IsOpen
    {
      get => (bool)GetValue( IsOpenProperty );
      set => SetValue( IsOpenProperty, value );
    }

    private static void OnIsOpenChanged( BindableObject bindable, object oldValue, object newValue )
    {
      if( bindable is Popup popup )
      {
        popup.OnIsOpenChanged( (bool)oldValue, (bool)newValue );
      }
    }

    protected virtual void OnIsOpenChanged( bool oldValue, bool newValue )
    {
      if( newValue )
      {
        this.ShowPopup();
      }
      else
      {
        this.ClosePopup();
      }
    }

    #endregion

    #endregion

    #region Internal Properties

    internal bool Focusable
    {
      get
      {
        return m_focusable;
      }
      set
      {
        m_focusable = value;
      }
    }

    #endregion

    #region Overrides Methods

    protected override void OnBindingContextChanged()
    {
      base.OnBindingContextChanged();

      if( this.Content is not null )
      {
        BindableObject.SetInheritedBindingContext( this.Content, this.BindingContext );
        this.Content.Parent = this;
      }
    }

    #endregion

    #region Public Methods

    public void Close()
    {
      this.IsOpen = false;
    }

    public void Open()
    {
      this.IsOpen = true;
    }

    public void UpdateSize()
    {
      this.Handler?.Invoke( nameof( IPopup.UpdateSize ) );
    }


    #endregion

    #region Internal Methods

    // if Popup was part of the original xaml and was not added with something like: var myPopup = new Popup().
    internal static bool IsFromXaml( IPopup popup )
    {
      if( popup == null )
        return false;

      var parent = PopupHandler.GetParent( popup );

      return !( parent is Page );
    }

    internal static Page GetParentPage( View popup )
    {
      Element? parent = popup;
      while( parent != null )
      {
        if( parent is Page )
          return (Page)parent;

        parent = parent.Parent;
      }

      if( parent == null )
      {
        parent = Application.Current?.MainPage;
      }
      if( parent == null )
        throw new InvalidDataException( "Can't find a parent page for the Popup to pop in." );

      return (Page)parent;
    }

    internal void ShowPopup()
    {
      if( m_isShown )
        return;
      if( this.Content == null )
        return;

      var parentPage = Popup.GetParentPage( this );
      var mauiContext = Popup.GetMauiContext( parentPage );
      if( mauiContext != null )
      {
        var platformHandler = this.ToHandler( mauiContext );
        platformHandler?.Invoke( nameof( IPopup.Open ) );
      }

      this.RaiseOpenedEvent( this, EventArgs.Empty );

      m_isShown = true;
    }

    internal void UpdateParent( PopupContainer popupContainer )
    {
      // When popup is in PopupContainer, Popup's parent must be reset for Content's control to load correctly( loaded event callback being called).
      this.Parent = null;
      this.Parent = popupContainer;

      if( this.Content != null )
      {
        // When popup is in PopupContainer, Popup Content's Parent must be reset for Content's control to load correctly( loaded event callback being called).
        this.Content.Parent = null;
        this.Content.Parent = this;
      }
    }

    #endregion

    #region Private Methods

    private static IMauiContext? GetMauiContext( Page page )
    {
      return page?.Handler?.MauiContext;
    }

    private void ClosePopup()
    {
      this.Handler?.Invoke( nameof( IPopup.Close ) );

      this.RaiseClosedEvent( this, EventArgs.Empty );

      m_isShown = false;
    }

    #endregion

    #region Events

    public event EventHandler? Opened;

    public void RaiseOpenedEvent( object sender, EventArgs e )
    {
      if( this.IsEnabled )
      {
        this.Opened?.Invoke( sender, e );
      }
    }

    public event EventHandler? Closed;

    public void RaiseClosedEvent( object sender, EventArgs e )
    {
      if( this.IsEnabled )
      {
        this.Closed?.Invoke( sender, e );
      }
    }

    #endregion //Events

    #region IPopup Interface

    IView? IPopup.Anchor => this.Anchor;

    IView? IPopup.Content => this.Content;

    bool IPopup.IsModal => this.IsModal;

    bool IPopup.IsOpen
    {
      get => this.IsOpen;
      set => this.IsOpen = value;
    }

    Window IPopup.Window => this.Window;

    void IPopup.Close() => this.Close();

    void IPopup.Open() => this.Open();

    #endregion
  }
}
