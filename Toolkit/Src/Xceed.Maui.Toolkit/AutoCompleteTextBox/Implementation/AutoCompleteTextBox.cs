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


using System.Collections;
using System.ComponentModel;

namespace Xceed.Maui.Toolkit
{
  public enum StringFilterType
  {
    Contains,
    NotContain,
    StartsWith,
    EndsWith,
    Equals,
    NotEqual,
    Custom
  }

  [DefaultProperty( "ItemsSource" )]
  [ContentProperty( "ItemsSource" )]
  public partial class AutoCompleteTextBox : TextBox
  {
    #region Private Members

    private PopupContainer m_popupContainer;
    private TextBox m_textBox;
    private CollectionView m_collectionView;

    #endregion

    #region Constructors

    public AutoCompleteTextBox()
    {
      this.SetValue( AutoCompleteTextBox.FilteredItemsProperty, new List<object>() );

      this.TextChanged += this.OnTextChanged;
    }

    #endregion

    #region Public Properties

    #region CustomFilterAction

    public static readonly BindableProperty CustomFilterActionProperty = BindableProperty.Create( nameof( CustomFilterAction ), typeof( ICustomFilterAction ), typeof( AutoCompleteTextBox ), null );

    public ICustomFilterAction CustomFilterAction
    {
      get => (ICustomFilterAction)GetValue( CustomFilterActionProperty );
      set => SetValue( CustomFilterActionProperty, value );
    }

    #endregion

    #region DisplayMemberPath

    public static readonly BindableProperty DisplayMemberPathProperty = BindableProperty.Create( nameof( DisplayMemberPath ), typeof( string ), typeof( AutoCompleteTextBox ), null );

    public string DisplayMemberPath
    {
      get => (string)GetValue( DisplayMemberPathProperty );
      set => SetValue( DisplayMemberPathProperty, value );
    }

    #endregion

    #region FilteredItems

    public static readonly BindableProperty FilteredItemsProperty = BindableProperty.Create( nameof( FilteredItems ), typeof( IList ), typeof( AutoCompleteTextBox ), null, propertyChanged: OnFilteredItemsChanged );

    public IList FilteredItems
    {
      get => (IList)GetValue( FilteredItemsProperty );
      private set => SetValue( FilteredItemsProperty, value );
    }

    private static void OnFilteredItemsChanged( BindableObject bindable, object oldValue, object newValue )
    {
      if( bindable is AutoCompleteTextBox autoCompleteTextBox )
      {
        autoCompleteTextBox.OnFilteredItemsChanged( (IList)oldValue, (IList)newValue );
      }
    }

    protected virtual void OnFilteredItemsChanged( IList oldValue, IList newValue )
    {
      bool isNewList = true;
      if( ( newValue != null ) && ( oldValue != null ) && ( newValue.Count == oldValue.Count ) )
      {
        isNewList = !newValue.Cast<object>().SequenceEqual( oldValue.Cast<object>() );
      }

      if( isNewList )
      {
        this.RaiseFilteredItemsChangedEvent( this, new ValueChangedEventArgs<IList>( oldValue, newValue ) );
      }
    }

    #endregion

    #region FilterType

    public static readonly BindableProperty FilterTypeProperty = BindableProperty.Create( nameof( FilterType ), typeof( StringFilterType ), typeof( AutoCompleteTextBox ), StringFilterType.StartsWith );

    public StringFilterType FilterType
    {
      get => (StringFilterType)GetValue( FilterTypeProperty );
      set => SetValue( FilterTypeProperty, value );
    }

    #endregion

    #region IsDropDownOpen

    public bool IsDropDownOpen => ( m_popupContainer != null ) ? m_popupContainer.IsOpen : false;

    #endregion

    #region ItemsSource

    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create( nameof( ItemsSource ), typeof( IEnumerable ), typeof( AutoCompleteTextBox ), null );

    public IEnumerable ItemsSource
    {
      get => (IEnumerable)GetValue( ItemsSourceProperty );
      set => SetValue( ItemsSourceProperty, value );
    }

    #endregion

    #region ItemTemplate

    public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create( nameof( ItemTemplate ), typeof( DataTemplate ), typeof( AutoCompleteTextBox ), null );

    public DataTemplate ItemTemplate
    {
      get => (DataTemplate)GetValue( ItemTemplateProperty );
      set => SetValue( ItemTemplateProperty, value );
    }

    #endregion

    #region MaxDropDownHeight

    public static readonly BindableProperty MaxDropDownHeightProperty = BindableProperty.Create( nameof( MaxDropDownHeight ), typeof( double ), typeof( AutoCompleteTextBox ), 300d, coerceValue: OnCoerceMaxDropDownHeight );

    public double MaxDropDownHeight
    {
      get => (double)GetValue( MaxDropDownHeightProperty );
      set => SetValue( MaxDropDownHeightProperty, value );
    }

    private static object OnCoerceMaxDropDownHeight( BindableObject bindable, object value )
    {
      if( (double)value < 0d )
        throw new InvalidDataException( "MaxDropDownHeight must be greater or equal to 0." );

      return value;
    }

    #endregion

    #endregion

    #region Partial Methods

    partial void InitializeCollectionViewForPlatform( object sender, EventArgs e );

    partial void ForceFocus();

    #endregion

    #region Overrides

    protected override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      if( m_popupContainer != null )
      {
        m_popupContainer.Focused -= this.PopupContainerOnFocused;
        m_popupContainer.Opened -= this.PopupContainerOnFocused;   //For Mac, since Focused is never caught.
      }

      if( m_collectionView != null )
      {
        m_collectionView.HandlerChanged -= this.CollectionViewOnHandlerChanged;
        m_collectionView.SelectionChanged -= this.CollectionViewOnSelectionChanged;
        m_collectionView.Scrolled -= this.CollectionView_Scrolled;
      }

      m_textBox = this.GetTemplateChild( "PART_TextBox" ) as TextBox;
      m_popupContainer = this.GetTemplateChild( "PART_PopupContainer" ) as PopupContainer;
      m_collectionView = this.GetTemplateChild( "PART_CollectionView" ) as CollectionView;

      if( m_popupContainer != null )
      {
        m_popupContainer.Focused += this.PopupContainerOnFocused;
        m_popupContainer.Opened += this.PopupContainerOnFocused;   //For Mac, since Focused is never caught.
      }

      if( m_collectionView != null )
      {
        m_collectionView.HandlerChanged += this.CollectionViewOnHandlerChanged;
        m_collectionView.SelectionChanged += this.CollectionViewOnSelectionChanged;
        m_collectionView.Scrolled += this.CollectionView_Scrolled;
      }
    }

    #endregion

    #region Internal Methods

    internal TextBox GetInternalTextBox() 
    {   
      return m_textBox; 
    }

    internal CollectionView GetInternalCollectionView()
    {
      return m_collectionView;
    }

    #endregion

    #region Private Methods

    private IEnumerable GetFilteredItems( string searchContent )
    {
      var filteredItems = new List<object>();

      if( m_collectionView == null )
        return filteredItems;
      if( this.ItemsSource == null )
        return filteredItems;
      if( string.IsNullOrEmpty( searchContent ) )
        return this.ItemsSource;

      foreach( var item in this.ItemsSource )
      {
        if( this.IsFiltered( item, searchContent ) )
        {
          filteredItems.Add( item );
        }
      }

      return filteredItems;
    }

    private bool IsFiltered( object item, string searchContent )
    {
      if( item == null )
        return false;
      if( string.IsNullOrEmpty( searchContent ) )
        return true;

      var itemValue = this.GetItemValue( item );

      switch( this.FilterType )
      {
        case StringFilterType.StartsWith: return itemValue.StartsWith( searchContent, StringComparison.CurrentCultureIgnoreCase );
        case StringFilterType.EndsWith: return itemValue.EndsWith( searchContent, StringComparison.CurrentCultureIgnoreCase );
        case StringFilterType.Contains: return itemValue.Contains( searchContent, StringComparison.CurrentCultureIgnoreCase );
        case StringFilterType.NotContain: return !itemValue.Contains( searchContent, StringComparison.CurrentCultureIgnoreCase );
        case StringFilterType.Equals: return itemValue.Equals( searchContent, StringComparison.CurrentCultureIgnoreCase );
        case StringFilterType.NotEqual: return !itemValue.Equals( searchContent, StringComparison.CurrentCultureIgnoreCase );
        case StringFilterType.Custom:
          {
            if( this.CustomFilterAction == null )
              throw new InvalidDataException( "The CustomFilterAction property must be set when the FilterType is set to Custom in order to use a custom filter." );

            return this.CustomFilterAction.IsItemPassFilter( item, searchContent );
          }
        default: throw new InvalidDataException( "Can't filter for this Unknown FilterType." );
      }
    }

    private string GetItemValue( object item )
    {
      if( item == null )
        return string.Empty;

      if( this.DisplayMemberPath != null )
      {
        var propertyInfo = item.GetType().GetProperty( this.DisplayMemberPath );
        if( propertyInfo != null )
        {
          return propertyInfo.GetValue( item, null )?.ToString();
        }
      }

      return item.ToString();
    }

    private void ActivatePopup( bool isOpen )
    {
      if( m_popupContainer != null )
      {
        m_popupContainer.SetFocusable( false );

        if( !m_popupContainer.IsOpen && isOpen )
        {
          m_popupContainer.IsOpen = true;
          this.RaiseDropDownOpenedEvent( this, EventArgs.Empty );
        }
        else if( m_popupContainer.IsOpen && !isOpen )
        {
          m_popupContainer.IsOpen = false;
          this.RaiseDropDownClosedEvent( this, EventArgs.Empty );
        }
      }
    }

    #endregion

    #region Events

    public event EventHandler<SelectionChangedEventArgs> SelectionChanged;

    private void RaiseSelectionChangedEvent( object sender, SelectionChangedEventArgs e )
    {
      if( this.IsEnabled )
      {
        this.SelectionChanged?.Invoke( sender, e );
      }
    }

    public event EventHandler<EventArgs> DropDownOpened;

    private void RaiseDropDownOpenedEvent( object sender, EventArgs e )
    {
      if( this.IsEnabled )
      {
        this.DropDownOpened?.Invoke( sender, e );
      }
    }

    public event EventHandler<EventArgs> DropDownClosed;

    private void RaiseDropDownClosedEvent( object sender, EventArgs e )
    {
      if( this.IsEnabled )
      {
        this.DropDownClosed?.Invoke( sender, e );
      }
    }

    public event EventHandler<ValueChangedEventArgs<IList>> FilteredItemsChanged;

    private void RaiseFilteredItemsChangedEvent( object sender, ValueChangedEventArgs<IList> e )
    {
      if( this.IsEnabled )
      {
        this.FilteredItemsChanged?.Invoke( sender, e );
      }
    }

    #endregion

    #region Event Handlers

    private void OnTextChanged( object sender, TextChangedEventArgs e )
    {
      if( m_popupContainer != null )
      {
        if( ( m_collectionView != null ) && ( m_textBox != null ) && m_textBox.IsFocused && ( this.Text?.Length > 0 ) )
        {
          this.FilteredItems = this.GetFilteredItems( this.Text ).Cast<object>().ToList();
          if( this.FilteredItems.Count == 0 )
          {
            this.ActivatePopup( false );
            return;
          }

          // Scroll to Top to prevent a CollectionView Height of 0.
          m_collectionView.ScrollTo( 0 );
          m_collectionView.ItemsSource = this.FilteredItems;

          this.ActivatePopup( true );
          // Update Size after popupContainer is Opened to ensure size will fit.
          m_popupContainer.UpdateSize();
        }
        else
        {
          this.ActivatePopup( false );
          this.FilteredItems.Clear();
        }
      }
    }

    private void PopupContainerOnFocused( object sender, EventArgs e )
    {
      if( m_textBox != null )
      {
        this.Dispatcher.Dispatch( () =>
        {
          m_textBox.Focus();

          this.ForceFocus();  //For Mac after popup opens.  // Need to simulate a mouseLeftButton down in TextBox after popup opens because calling m_textBox.Focus() returns false !!!
        } );
      }
    }

    private void CollectionViewOnSelectionChanged( object sender, SelectionChangedEventArgs e )
    {
      if( sender is CollectionView collectionView )
      {
        var newText = collectionView.SelectedItem?.ToString();
        if( this.DisplayMemberPath != null )
        {
          var propertyInfo = collectionView.SelectedItem?.GetType().GetProperty( this.DisplayMemberPath );
          if( propertyInfo != null )
          {
            newText = propertyInfo.GetValue( collectionView.SelectedItem, null )?.ToString();
          }
        }

        this.Text = newText;

        this.ActivatePopup( false );

        this.RaiseSelectionChangedEvent( sender, e );
      }
    }

    private void CollectionView_Scrolled( object sender, ItemsViewScrolledEventArgs e )
    {
      if( ( m_popupContainer != null ) && m_popupContainer.IsOpen )
      {
        m_popupContainer.UpdateSize();
      }
    }

    private void CollectionViewOnHandlerChanged( object sender, EventArgs e )
    {
      this.InitializeCollectionViewForPlatform( sender, e );
    }

    #endregion
  }
}
