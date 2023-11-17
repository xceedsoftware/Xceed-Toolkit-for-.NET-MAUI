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


using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Xceed.Maui.Toolkit
{
  public partial class FilePicker : Control
  {
    #region Private Members

    private bool m_updateLocal;
    private TextBox m_textBox;

    private ToggleButton m_browseButton;

    #endregion

    #region Constructors

    public FilePicker()
    {
      this.SetValue( FilePicker.SelectedFilesProperty, new ObservableCollection<string>() );
      this.HandlerChanged += this.FilePicker_HandlerChanged;
      this.HandlerChanging += this.FilePicker_HandlerChanging;
    }

    #endregion

    #region Properties

    #region BrowseButtonStyle

    public static readonly BindableProperty BrowseButtonStyleProperty = BindableProperty.Create( nameof( BrowseButtonStyle ), typeof( Style ), typeof( FilePicker ) );
    public Style BrowseButtonStyle
    {
      get => ( Style )GetValue( BrowseButtonStyleProperty );
      set => SetValue( BrowseButtonStyleProperty, value );
    }

    #endregion //BrowseButtonStyle

    #region BrowseContent

    public static readonly BindableProperty BrowseContentProperty = BindableProperty.Create( nameof( BrowseContent ), typeof( object ), typeof( FilePicker ), defaultValue: "..." );
    public object BrowseContent
    {
      get => ( object )GetValue( BrowseContentProperty );
      set => SetValue( BrowseContentProperty, value );
    }

    #endregion    

    #region Filter

    public static readonly BindableProperty FilterProperty = BindableProperty.Create( nameof( Filter ), typeof( FilePickerFileType ), typeof( FilePicker ) );

    public FilePickerFileType Filter
    {
      get => ( FilePickerFileType )GetValue( FilterProperty );
      set => SetValue( FilterProperty, value );
    }

    #endregion

    #region IsFullPath

    public static readonly BindableProperty IsFullPathProperty = BindableProperty.Create( nameof( IsFullPath ), typeof( bool ), typeof( FilePicker ), defaultValue: true );
    public bool IsFullPath
    {
      get => ( bool )GetValue( IsFullPathProperty );
      set => SetValue( IsFullPathProperty, value );
    }

    #endregion

    #region IsFocusUnderlineVisible 

    public static readonly BindableProperty IsFocusUnderlineVisibleProperty = BindableProperty.Create( nameof( IsFocusUnderlineVisible ), typeof( bool ), typeof( FilePicker ), defaultValue: true, propertyChanged: OnIsFocusUnderlineVisibleChanged );

    public bool IsFocusUnderlineVisible
    {
      get => ( bool )GetValue( IsFocusUnderlineVisibleProperty );
      set => SetValue( IsFocusUnderlineVisibleProperty, value );
    }

    private static void OnIsFocusUnderlineVisibleChanged( BindableObject bindable, object oldValue, object newValue )
    {
      if( bindable is FilePicker filePicker )
      {
        filePicker.OnIsFocusUnderlineVisibleChanged( ( bool )oldValue, ( bool )newValue );
      }
    }

    private void OnIsFocusUnderlineVisibleChanged( bool oldValue, bool newValue )
    {
      if( !newValue )
      {
        if( m_browseButton != null )
        {
          m_browseButton.Margin = new Thickness( 0 );
        }
      }
    }

    #endregion

    #region IsMultiSelect

    public static readonly BindableProperty IsMultiSelectProperty = BindableProperty.Create( nameof( IsMultiSelect ), typeof( bool ), typeof( FilePicker ), defaultValue: false );

    public bool IsMultiSelect
    {
      get => ( bool )GetValue( IsMultiSelectProperty );
      set => SetValue( IsMultiSelectProperty, value );
    }

    #endregion

    #region IsOpen

    public static readonly BindableProperty IsOpenProperty = BindableProperty.Create( nameof( IsOpen ), typeof( bool ), typeof( FilePicker ), defaultValue: false, propertyChanged: OnIsOpenChanged );

    public bool IsOpen
    {
      get => ( bool )GetValue( IsOpenProperty );
      set => SetValue( IsOpenProperty, value );
    }

    private static void OnIsOpenChanged( BindableObject bindable, object oldValue, object newValue )
    {
      var filePicker = ( FilePicker )bindable;
      filePicker?.OnIsOpenChanged( ( bool )oldValue, ( bool )newValue );
    }

    protected virtual async void OnIsOpenChanged( bool oldValue, bool newValue )
    {
      if( newValue )
      {
        //For android is displayed on the initial prompt to the user, but not in the picker dialog itself.
        //For macOS is displayed to the user.
        //For Windows and iOS isn't displayed to the user.
        var pickOptions = new PickOptions() { PickerTitle = this.Title, FileTypes = this.Filter };
        if( this.IsMultiSelect )
        {
          var results = await Microsoft.Maui.Storage.FilePicker.PickMultipleAsync( pickOptions );
          if( results != null && results.Any() )
          {
            var list = new List<string>();
            foreach( var result in results )
            {
              list.Add( this.IsFullPath ? result.FullPath.ToString() : result.FileName.ToString() );
            }
            this.SelectedFiles = new ObservableCollection<string>( list );
          }
        }
        else
        {
          var result = await Microsoft.Maui.Storage.FilePicker.PickAsync( pickOptions );
          if( result != null )
          {
            this.SelectedFile = this.IsFullPath ? result.FullPath.ToString() : result.FileName.ToString();
          }
        }
        this.IsOpen = false;
      }
    }

    #endregion //IsOpen

    #region SelectedFile

    public static readonly BindableProperty SelectedFileProperty = BindableProperty.Create( nameof( SelectedFile ), typeof( string ), typeof( FilePicker ), defaultValue: null, propertyChanged: OnSelectedFileChanged );

    public string SelectedFile
    {
      get => ( string )GetValue( SelectedFileProperty );
      set => SetValue( SelectedFileProperty, value );
    }

    private static void OnSelectedFileChanged( BindableObject bindable, object oldValue, object newValue )
    {
      var filePicker = ( FilePicker )bindable;
      filePicker?.OnSelectedFileChanged( ( string )oldValue, ( string )newValue );
    }

    protected virtual void OnSelectedFileChanged( string oldValue, string newValue )
    {
      if( !this.IsFullPath && ( newValue != null ) && ( newValue.IndexOfAny( new char[] { '/', '\\' } ) != -1 ) )
        throw new InvalidOperationException( "SelectedFile shouldn't contain \"/\" or \"\\\" when IsFullPath is false." );

      this.UpdateSelectedValue();
      this.RaiseSelectedFileChangedEvent( this, EventArgs.Empty );
    }

    #endregion

    #region SelectedFiles

    public static readonly BindableProperty SelectedFilesProperty = BindableProperty.Create( nameof( SelectedFiles ), typeof( ObservableCollection<string> ), typeof( FilePicker ), propertyChanged: OnSelectedFilesChanged );

    public ObservableCollection<string> SelectedFiles
    {
      get => ( ObservableCollection<string> )GetValue( SelectedFilesProperty );
      set => SetValue( SelectedFilesProperty, value );
    }

    private static void OnSelectedFilesChanged( BindableObject bindable, object oldValue, object newValue )
    {
      var filePicker = ( FilePicker )bindable;
      filePicker?.OnSelectedFilesChanged( ( ObservableCollection<string> )oldValue, ( ObservableCollection<string> )newValue );
    }

    protected virtual void OnSelectedFilesChanged( ObservableCollection<string> oldValue, ObservableCollection<string> newValue )
    {
      if( !this.IsFullPath && ( newValue != null ) && ( newValue.FirstOrDefault( x => x.IndexOfAny( new char[] { '/', '\\' } ) != -1 ) != null ) )
        throw new InvalidOperationException( "SelectedFiles shouldn't contain a string with \"/\" or \"\\\" when IsFullPath is false." );

      var oldCollection = oldValue as INotifyCollectionChanged;
      var newCollection = newValue as INotifyCollectionChanged;
      if( oldCollection != null )
      {
        oldCollection.CollectionChanged -= new NotifyCollectionChangedEventHandler( this.SelectedFiles_CollectionChanged );
      }
      this.SelectedFile = null;

      if( newCollection != null )
      {
        newCollection.CollectionChanged += new NotifyCollectionChangedEventHandler( this.SelectedFiles_CollectionChanged );
        this.SelectedFile = newValue.FirstOrDefault();
      }
      this.UpdateSelectedValue();
      this.RaiseSelectedFilesChangedEvent( this, EventArgs.Empty );
    }

    #endregion

    #region SelectedValue

    public static readonly BindableProperty SelectedValueProperty = BindableProperty.Create( nameof( SelectedValue ), typeof( string ), typeof( FilePicker ), defaultValue: null, propertyChanged: OnSelectedValueChanged );
    public string SelectedValue
    {
      get => ( string )GetValue( SelectedValueProperty );
      private set => SetValue( SelectedValueProperty, value );
    }

    private static void OnSelectedValueChanged( BindableObject bindable, object oldValue, object newValue )
    {
      if( bindable is FilePicker filePicker )
      {
        filePicker.OnSelectedValueChanged( ( string )oldValue, ( string )newValue );
      }
    }

    protected virtual void OnSelectedValueChanged( string oldValue, string newValue )
    {
      if( !m_updateLocal )
      {
        this.SetSelectedValueText();
      }
    }

    #endregion //SelectedValue

    #region Title

    public static readonly BindableProperty TitleProperty = BindableProperty.Create( nameof( Title ), typeof( string ), typeof( FilePicker ), defaultValue: "FilePicker" );
    public string Title
    {
      get => ( string )GetValue( TitleProperty );
      set => SetValue( TitleProperty, value );
    }

    #endregion

    #region Watermark

    public static readonly BindableProperty WatermarkProperty = BindableProperty.Create( nameof( Watermark ), typeof( object ), typeof( FilePicker ), defaultValue: "" );
    public object Watermark
    {
      get => ( object )GetValue( WatermarkProperty );
      set => SetValue( WatermarkProperty, value );
    }

    #endregion //Watermark

    #endregion //Properties

    #region Partial Methods

    partial void InitializeForPlatform( object sender, EventArgs e );

    partial void UninitializeForPlatform( object sender, HandlerChangingEventArgs e );

    #endregion

    #region Protected Methods

    protected override void OnTextColorChanged( Color oldValue, Color newValue )
    {
      base.OnTextColorChanged( oldValue, newValue );
      Control.UpdateFontTextColor( m_textBox, oldValue, newValue );
    }

    protected override void OnFontAttributesChanged( FontAttributes oldValue, FontAttributes newValue )
    {
      base.OnFontAttributesChanged( oldValue, newValue );

      Control.UpdateFontAttributes( m_textBox, oldValue, newValue );
    }

    protected override void OnFontFamilyChanged( string oldValue, string newValue )
    {
      base.OnFontFamilyChanged( oldValue, newValue );

      Control.UpdateFontFamily( m_textBox, oldValue, newValue );
    }

    protected override void OnFontSizeChanged( double oldValue, double newValue )
    {
      base.OnFontSizeChanged( oldValue, newValue );

      Control.UpdateFontSize( m_textBox, oldValue, newValue );
    }

    protected override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      if( m_textBox != null )
      {
        m_textBox.Unfocused -= this.TextBox_Unfocused;
        m_textBox.Focused -= this.TextBox_Focused;
      }
      m_textBox = this.GetTemplateChild( "PART_TextBox" ) as TextBox;
      if( m_textBox != null )
      {
        m_textBox.Unfocused += this.TextBox_Unfocused;
        m_textBox.Focused += this.TextBox_Focused;
      }

      m_browseButton = this.GetTemplateChild( "PART_BrowseButton" ) as ToggleButton;
    }


    #endregion

    #region Internal Methods

    //For unit Test purpose (Test BrowseButton Style)
    internal ToggleButton GetBrowseButton()
    {
      return m_browseButton;
    }

    #endregion

    #region Private Methods

    private void SetSelectedValueText()
    {
      if( m_textBox != null )
      {
        if( this.IsMultiSelect )
        {
          string[] files = null;
          if( !string.IsNullOrEmpty( m_textBox.Text ) )
          {
            var stringsWithoutQuotes = m_textBox.Text.Replace( "\"", "" );
            files = stringsWithoutQuotes.Split( new char[] { ';' } );
          }
          else
          {
            this.SelectedFiles = new ObservableCollection<string>();
          }

          if( files != null )
          {
            this.SelectedFiles = new ObservableCollection<string>( files );
          }
        }
        else
        {
          this.SelectedFile = m_textBox.Text;
        }
      }
    }

    private void UpdateSelectedValue()
    {
      m_updateLocal = true;
      this.SelectedValue = this.IsMultiSelect ? this.CreateSelectedValueFromStrings() : this.SelectedFile;
      if( m_textBox != null )
      {
        m_textBox.Text = this.SelectedValue;
      }
      m_updateLocal = false;
    }

    private string CreateSelectedValueFromStrings()
    {
      string files = "";
      if( this.SelectedFiles != null )
      {
        if( this.SelectedFiles.Count == 0 && this.SelectedFile != string.Empty )
          return this.SelectedFile;
        if( this.SelectedFiles.Count == 1 )
          return this.SelectedFiles[ 0 ];

        foreach( var file in this.SelectedFiles )
        {
          files += "\"" + file + "\";";
        }
      }

      return ( files.Length > 0 ) ? files.Substring( 0, files.Length - 1 ) : files;
    }

    #endregion

    #region EventHandlers
    private void TextBox_Focused( object sender, EventArgs e )
    {
      if( m_browseButton != null )
      {
        if( this.IsFocusUnderlineVisible )
        {
          m_browseButton.Margin = new Thickness( 0, 0, 0, 3 );
        }
      }
    }
    private void TextBox_Unfocused( object sender, EventArgs e )
    {
      if( m_browseButton != null )
      {
        if( this.IsFocusUnderlineVisible )
        {
          m_browseButton.Margin = new Thickness( 0, 0, 0, 0 );
        }
      }
      this.SetSelectedValueText();
    }

    private void SelectedFiles_CollectionChanged( object sender, NotifyCollectionChangedEventArgs e )
    {
      if( !this.IsFullPath && ( e.NewItems != null ) && ( e.NewItems.Count > 0 )
          && ( e.NewItems[ 0 ] is string ) && ( ( ( string )e.NewItems[ 0 ] ).IndexOfAny( new char[] { '/', '\\' } ) != -1 ) )
      {
        throw new InvalidOperationException( "SelectedFiles shouldn't contain a string with \"/\" or \"\\\" when IsFullPath is false." );
      }

      this.SelectedFile = ( ( e.NewItems != null ) && ( e.NewItems.Count > 0 ) && ( e.NewItems[ 0 ] is string ) ) ? ( string )e.NewItems[ 0 ] : null;

      this.UpdateSelectedValue();
    }

    private void FilePicker_HandlerChanged( object sender, EventArgs e )
    {
      this.InitializeForPlatform( sender, e );
    }

    private void FilePicker_HandlerChanging( object sender, HandlerChangingEventArgs e )
    {
      this.UninitializeForPlatform( sender, e );
    }

    #endregion //EventHandlers

    #region Events

    #region SelectedFileChanged Event

    public event EventHandler<EventArgs> SelectedFileChanged;

    public void RaiseSelectedFileChangedEvent( object sender, EventArgs e )
    {
      if( this.IsEnabled )
      {
        this.SelectedFileChanged?.Invoke( sender, e );
      }
    }

    #endregion //SelectedFilesChangedEvent Event

    #region SelectedFilesChangedEvent Event

    public event EventHandler<EventArgs> SelectedFilesChanged;

    public void RaiseSelectedFilesChangedEvent( object sender, EventArgs e )
    {
      if( this.IsEnabled )
      {
        this.SelectedFilesChanged?.Invoke( sender, e );
      }
    }

    #endregion //SelectedFilesChangedEvent Event

    #endregion  //Events
  }
}
