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


#nullable enable

namespace Xceed.Maui.Toolkit
{
  [ContentProperty( nameof( Content ) )]
  public class PopupContainer : TemplatedView, IDisposable
  {
    #region Private Members

    private Popup m_popup;

    #endregion

    #region Constructors

    public PopupContainer()
    {
      m_popup = new Popup( this );
      m_popup.Focused += this.PopupOnFocused;
      m_popup.Opened += this.PopupOnOpened;
      m_popup.Closed += this.PopupOnClosed;

      this.Loaded += this.PopupContainer_Loaded;

      PopupContainer.BindProperty( m_popup, Popup.AnchorProperty, this, PopupContainer.AnchorProperty );
      PopupContainer.BindProperty( m_popup, Popup.ContentProperty, this, PopupContainer.ContentProperty );
      PopupContainer.BindProperty( m_popup, Popup.IsModalProperty, this, PopupContainer.IsModalProperty );
      PopupContainer.BindProperty( m_popup, Popup.IsOpenProperty, this, PopupContainer.IsOpenProperty );
      PopupContainer.BindProperty( m_popup, Popup.HorizontalOptionsProperty, this, PopupContainer.HorizontalOptionsProperty );
      PopupContainer.BindProperty( m_popup, Popup.VerticalOptionsProperty, this, PopupContainer.VerticalOptionsProperty );
      PopupContainer.BindProperty( m_popup, Popup.WidthRequestProperty, this, PopupContainer.WidthRequestProperty );
      PopupContainer.BindProperty( m_popup, Popup.HeightRequestProperty, this, PopupContainer.HeightRequestProperty );
      PopupContainer.BindProperty( m_popup, Popup.MaximumWidthRequestProperty, this, PopupContainer.MaximumWidthRequestProperty );
      PopupContainer.BindProperty( m_popup, Popup.MaximumHeightRequestProperty, this, PopupContainer.MaximumHeightRequestProperty );
    }

    ~PopupContainer()
    {
      this.Dispose();
    }

    #endregion

    #region Public Properties

    #region Anchor

    public static readonly BindableProperty AnchorProperty = BindableProperty.Create( nameof( Anchor ), typeof( View ), typeof( PopupContainer ) );

    public View? Anchor
    {
      get => (View?)GetValue( AnchorProperty );
      set => SetValue( AnchorProperty, value );
    }

    #endregion

    #region Content

    public static readonly BindableProperty ContentProperty = BindableProperty.Create( nameof( Content ), typeof( View ), typeof( PopupContainer ), propertyChanged: OnContentChanged );

    public View? Content
    {
      get => (View?)GetValue( ContentProperty );
      set => SetValue( ContentProperty, value );
    }

    private static void OnContentChanged( BindableObject bindable, object oldValue, object newValue )
    {
      if( bindable is PopupContainer popupContainer )
      {
        popupContainer.OnContentChanged( (View?)oldValue, (View?)newValue );
      }
    }

    protected virtual void OnContentChanged( View? oldValue, View? newValue )
    {
    }

    #endregion

    #region IsModal

    public static readonly BindableProperty IsModalProperty = BindableProperty.Create( nameof( IsModal ), typeof( bool ), typeof( PopupContainer ) );

    public bool IsModal
    {
      get => (bool)GetValue( IsModalProperty );
      set => SetValue( IsModalProperty, value );
    }

    #endregion

    #region IsOpen

    public static readonly BindableProperty IsOpenProperty = BindableProperty.Create( nameof( IsOpen ), typeof( bool ), typeof( PopupContainer ), defaultBindingMode: BindingMode.TwoWay );

    public bool IsOpen
    {
      get => (bool)GetValue( IsOpenProperty );
      set => SetValue( IsOpenProperty, value );
    }

    #endregion

    #endregion

    #region Public Methods

    public void UpdateSize()
    {
      m_popup.UpdateSize();
    }

    #endregion

    #region Overrides

    protected override Size MeasureOverride( double widthConstraint, double heightConstraint )
    {
      return Size.Zero;
    }

    protected override Size ArrangeOverride( Rect bounds )
    {
      return bounds.Size;
    }

    #endregion

    #region Internal Methods

    internal void SetFocusable( bool isFocusable )
    {
      m_popup.Focusable = isFocusable;
    }

    #endregion

    #region Private Methods

    private static void BindProperty( Popup targetElement, BindableProperty targetProperty, PopupContainer sourceElement, BindableProperty sourceProperty )
    {
      var binding = new Binding( sourceProperty.PropertyName, source: sourceElement );

      targetElement.SetBinding( targetProperty, binding );
    }

    #endregion

    #region Events 

    public new event EventHandler<EventArgs> Focused;

    private void RaiseFocusedEvent( object sender, EventArgs e )
    {
      if( this.IsEnabled )
      {
        this.Focused?.Invoke( sender, e );
      }
    }

    public event EventHandler<EventArgs> Opened;

    private void RaiseOpenedEvent( object sender, EventArgs e )
    {
      if( this.IsEnabled )
      {
        this.Opened?.Invoke( sender, e );
      }
    }

    public event EventHandler<EventArgs> Closed;

    private void RaiseClosedEvent( object sender, EventArgs e )
    {
      if( this.IsEnabled )
      {
        this.Closed?.Invoke( sender, e );
      }
    }

    #endregion

    #region Event Handlers

    private void PopupOnFocused( object? sender, FocusEventArgs e )
    {
      this.RaiseFocusedEvent( this, EventArgs.Empty );
    }

    private void PopupOnOpened( object? sender, EventArgs e )
    {
      this.RaiseOpenedEvent( this, EventArgs.Empty );
    }

    private void PopupOnClosed( object? sender, EventArgs e )
    {
      this.RaiseClosedEvent( this, EventArgs.Empty );
    }

    private void PopupContainer_Loaded( object? sender, EventArgs e )
    {
      m_popup.UpdateParent( this );

      this.Dispatcher.Dispatch( () =>
      {
        if( m_popup.IsOpen )
        {
          m_popup.ShowPopup();
        }
      } );
    }

    #endregion

    #region IDisposable Interface

    public void Dispose()
    {
      m_popup.Focused -= this.PopupOnFocused;
      m_popup.Opened -= this.PopupOnOpened;
      m_popup.Closed -= this.PopupOnClosed;
      m_popup = null;
    }

    #endregion
  }
}
