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


using System.Windows.Input;

namespace Xceed.Maui.Toolkit
{
  public partial class TextBox : Control
  {
    #region Private Members

    private Entry m_entry;
    private Button m_clearButton;
    private ContentControl m_watermark;

    #endregion

    #region Constructors

    public TextBox()
    {
      this.HandlerChanged += this.TextBox_HandlerChanged;
      this.HandlerChanging += this.TextBox_HandlerChanging;
    }

    #endregion

    #region Public Properties

    #region CharacterSpacing

    public static readonly BindableProperty CharacterSpacingProperty = BindableProperty.Create( nameof( CharacterSpacing ), typeof( double ), typeof( TextBox ), 0d );

    public double CharacterSpacing
    {
      get => (double)GetValue( CharacterSpacingProperty );
      set => SetValue( CharacterSpacingProperty, value );
    }

    #endregion

    #region ClearButtonStyle

    public static readonly BindableProperty ClearButtonStyleProperty = BindableProperty.Create( nameof( ClearButtonStyle ), typeof( Style ), typeof( TextBox ), null );

    public Style ClearButtonStyle
    {
      get => (Style)GetValue( ClearButtonStyleProperty );
      set => SetValue( ClearButtonStyleProperty, value );
    }

    #endregion

    #region ClearButtonVisibility

    public static readonly BindableProperty ClearButtonVisibilityProperty = BindableProperty.Create( nameof( ClearButtonVisibility ), typeof( ClearButtonVisibility ), typeof( TextBox ), ClearButtonVisibility.Never );

    public ClearButtonVisibility ClearButtonVisibility
    {
      get => (ClearButtonVisibility)GetValue( ClearButtonVisibilityProperty );
      set => SetValue( ClearButtonVisibilityProperty, value );
    }

    #endregion

    #region CursorPosition

    public static readonly BindableProperty CursorPositionProperty = BindableProperty.Create( nameof( CursorPosition ), typeof( int ), typeof( TextBox ), 0, coerceValue: OnCoerceCursorPosition, propertyChanged: OnCursorPositionChanged );

    public int CursorPosition
    {
      get => (int)GetValue( CursorPositionProperty );
      set => SetValue( CursorPositionProperty, value );
    }

    private static object OnCoerceCursorPosition( BindableObject bindable, object value )
    {
      if( (int)value < 0 )
        throw new InvalidDataException( "CursorPosition must be greater or equal to 0." );

      return value;
    }

    private static void OnCursorPositionChanged( BindableObject bindable, object oldValue, object newValue )
    {
      if( bindable is TextBox textBox )
      {
        textBox.OnCursorPositionChanged( (int)oldValue, (int)newValue );
      }
    }

    protected virtual void OnCursorPositionChanged( int oldValue, int newValue )
    {
      this.RaiseSelectionChangedEvent( this, EventArgs.Empty );
    }

    #endregion

    #region IsFocusUnderlineVisible

    public static readonly BindableProperty IsFocusUnderlineVisibleProperty = BindableProperty.Create( nameof( IsFocusUnderlineVisible ), typeof( bool ), typeof( TextBox ), false );

    public bool IsFocusUnderlineVisible
    {
      get => (bool)GetValue( IsFocusUnderlineVisibleProperty );
      set => SetValue( IsFocusUnderlineVisibleProperty, value );
    }

    #endregion

    #region IsReadOnly

    public static readonly BindableProperty IsReadOnlyProperty = BindableProperty.Create( nameof( IsReadOnly ), typeof( bool ), typeof( TextBox ), false );

    public bool IsReadOnly
    {
      get => (bool)GetValue( IsReadOnlyProperty );
      set => SetValue( IsReadOnlyProperty, value );
    }

    #endregion

    #region IsFocused

    public static new readonly BindableProperty IsFocusedProperty = BindableProperty.Create( nameof( IsFocused ), typeof( bool ), typeof( TextBox ) );

    public new bool IsFocused
    {
      get => (bool)GetValue( IsFocusedProperty );
      private set => SetValue( IsFocusedProperty, value );
    }

    #endregion

    #region IsTextPredictionEnabled

    public static readonly BindableProperty IsTextPredictionEnabledProperty = BindableProperty.Create( nameof( IsTextPredictionEnabled ), typeof( bool ), typeof( TextBox ), true );

    public bool IsTextPredictionEnabled
    {
      get => (bool)GetValue( IsTextPredictionEnabledProperty );
      set => SetValue( IsTextPredictionEnabledProperty, value );
    }

    #endregion

    #region Keyboard

    public static readonly BindableProperty KeyboardProperty = BindableProperty.Create( nameof( Keyboard ), typeof( Keyboard ), typeof( TextBox ) );

    public Keyboard Keyboard
    {
      get => (Keyboard)GetValue( KeyboardProperty );
      set => SetValue( KeyboardProperty, value );
    }

    #endregion

    #region MaxLength

    public static readonly BindableProperty MaxLengthProperty = BindableProperty.Create( nameof( MaxLength ), typeof( int ), typeof( TextBox ), int.MaxValue, coerceValue: OnCoerceMaxLength );

    public int MaxLength
    {
      get => (int)GetValue( MaxLengthProperty );
      set => SetValue( MaxLengthProperty, value );
    }

    private static object OnCoerceMaxLength( BindableObject bindable, object value )
    {
      if( (int)value < 0 )
        throw new InvalidDataException( "MaxLength must be greater or equal to 0." );

      return value;
    }

    #endregion

    #region ReturnCommand

    public static readonly BindableProperty ReturnCommandProperty = BindableProperty.Create( nameof( ReturnCommand ), typeof( ICommand ), typeof( TextBox ) );

    public ICommand ReturnCommand
    {
      get => (ICommand)GetValue( ReturnCommandProperty );
      set => SetValue( ReturnCommandProperty, value );
    }

    #endregion

    #region ReturnCommandParameter

    public static readonly BindableProperty ReturnCommandParameterProperty = BindableProperty.Create( nameof( ReturnCommandParameter ), typeof( object ), typeof( TextBox ) );

    public object ReturnCommandParameter
    {
      get => (object)GetValue( ReturnCommandParameterProperty );
      set => SetValue( ReturnCommandParameterProperty, value );
    }

    #endregion

    #region SelectionLength

    public static readonly BindableProperty SelectionLengthProperty = BindableProperty.Create( nameof( SelectionLength ), typeof( int ), typeof( TextBox ), 0, coerceValue: OnCoerceSelectionLength, propertyChanged: OnSelectionLengthChanged );

    public int SelectionLength
    {
      get => (int)GetValue( SelectionLengthProperty );
      set => SetValue( SelectionLengthProperty, value );
    }

    private static object OnCoerceSelectionLength( BindableObject bindable, object value )
    {
      if( (int)value < 0 )
        throw new InvalidDataException( "SelectionLength must be greater or equal to 0." );

      return value;
    }

    private static void OnSelectionLengthChanged( BindableObject bindable, object oldValue, object newValue )
    {
      if( bindable is TextBox textBox )
      {
        textBox.OnSelectionLengthChanged( (int)oldValue, (int)newValue );
      }
    }

    protected virtual void OnSelectionLengthChanged( int oldValue, int newValue )
    {
      this.RaiseSelectionChangedEvent( this, EventArgs.Empty );
    }

    #endregion

    #region Text

    public static readonly BindableProperty TextProperty = BindableProperty.Create( nameof( Text ), typeof( string ), typeof( TextBox ), null, coerceValue: OnCoerceText, propertyChanged: OnTextChanged );

    public string Text
    {
      get => (string)GetValue( TextProperty );
      set => SetValue( TextProperty, value );
    }

    private static object OnCoerceText( BindableObject bindable, object value )
    {
      if( bindable is TextBox textBox )
        return textBox.OnCoerceText( (string)value );

      return value;
    }

    protected virtual object OnCoerceText( string value )
    {
      return value;
    }

    private static void OnTextChanged( BindableObject bindable, object oldValue, object newValue )
    {
      if( bindable is TextBox textBox )
      {
        textBox.OnTextChanged( (string)oldValue, (string)newValue );
      }
    }

    protected virtual void OnTextChanged( string oldValue, string newValue )
    {
      if( m_entry != null )
      {
        if( m_entry.Text != newValue )
        {
          m_entry.Text = newValue;
        }
      }
      this.RaiseTextChangedEvent( this, new TextChangedEventArgs( oldValue, newValue ) );
    }

    #endregion

    #region Watermark

    public static readonly BindableProperty WatermarkProperty = BindableProperty.Create( nameof( Watermark ), typeof( object ), typeof( TextBox ), null );

    public object Watermark
    {
      get => (object)GetValue( WatermarkProperty );
      set => SetValue( WatermarkProperty, value );
    }

    #endregion

    #endregion

    #region Partial Methods

    partial void InitializeForPlatform( object sender, EventArgs e );

    partial void UninitializeForPlatform( object sender, HandlerChangingEventArgs e );

    #endregion

    #region Protected Methods

    protected override void OnTextColorChanged( Color oldValue, Color newValue )
    {
      base.OnTextColorChanged( oldValue, newValue );

      Control.UpdateFontTextColor( m_entry, oldValue, newValue );
    }

    protected override void OnFontAttributesChanged( FontAttributes oldValue, FontAttributes newValue )
    {
      base.OnFontAttributesChanged( oldValue, newValue );

      Control.UpdateFontAttributes( m_entry, oldValue, newValue );
    }

    protected override void OnFontFamilyChanged( string oldValue, string newValue )
    {
      base.OnFontFamilyChanged( oldValue, newValue );

      Control.UpdateFontFamily( m_entry, oldValue, newValue );
    }

    protected override void OnFontSizeChanged( double oldValue, double newValue )
    {
      base.OnFontSizeChanged( oldValue, newValue );

      Control.UpdateFontSize( m_entry, oldValue, newValue );
    }

    protected override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      if( m_entry != null )
      {
        m_entry.TextChanged -= this.Entry_TextChanged;
        m_entry.Completed -= this.Entry_Completed;
      }
      m_entry = this.GetTemplateChild( "PART_Entry" ) as Entry;
      if( m_entry != null )
      {
        m_entry.TextChanged += this.Entry_TextChanged;
        m_entry.Completed += this.Entry_Completed;
      }

      if( m_clearButton != null )
      {
        m_clearButton.Clicked -= this.ClearButton_Clicked;
      }
      m_clearButton = this.GetTemplateChild( "PART_ClearButton" ) as Button;
      if( m_clearButton != null )
      {
        m_clearButton.Clicked += this.ClearButton_Clicked;
      }

      m_watermark = this.GetTemplateChild( "PART_Watermark" ) as ContentControl;
    }

    protected internal virtual void ClearButtonAction()
    {
      if( m_entry != null )
      {
        m_entry.Text = string.Empty;
        m_entry.Focus();  //Not working !
      }
    }

    #endregion

    #region Internal Methods

    internal Button GetClearButton()
    {
      return m_clearButton;
    }

    internal ContentControl GetWatermarkContentControl()
    {
      return m_watermark;
    }

    internal Entry GetEntry()
    {
      return m_entry;
    }

    internal void SetFocus( bool isFocused )
    {
      if( isFocused )
      {
        this.Focus();
        this.IsFocused = true;
        this.RaiseFocusedEvent( this, EventArgs.Empty );
      }
      else
      {
        this.Unfocus();
        this.IsFocused = false;
        this.RaiseUnFocusedEvent( this, EventArgs.Empty );
      }
    }

    #endregion

    #region Events

    public event EventHandler Completed;

    private void RaiseCompletedEvent( object sender, EventArgs e )
    {
      if( this.IsEnabled )
      {
        this.Completed?.Invoke( sender, e );
      }
    }

    public event EventHandler<TextChangedEventArgs> TextChanged;

    internal void RaiseTextChangedEvent( object sender, TextChangedEventArgs e )
    {
      if( this.IsEnabled )
      {
        this.TextChanged?.Invoke( sender, e );
      }
    }

    public new event EventHandler Focused;

    internal void RaiseFocusedEvent( object sender, EventArgs e )
    {
      if( this.IsEnabled )
      {
        this.Focused?.Invoke( sender, e );
      }
    }

    public new event EventHandler Unfocused;

    internal void RaiseUnFocusedEvent( object sender, EventArgs e )
    {
      if( this.IsEnabled )
      {
        this.Unfocused?.Invoke( sender, e );
      }
    }

    public event EventHandler SelectionChanged;

    internal void RaiseSelectionChangedEvent( object sender, EventArgs e )
    {
      if( this.IsEnabled )
      {
        this.SelectionChanged?.Invoke( sender, e );
      }
    }

    #endregion

    #region Event Handlers

    protected virtual void Entry_TextChanged( object sender, TextChangedEventArgs e )
    {
      if( this.Text != e.NewTextValue )
      {
        this.Text = e.NewTextValue;
      }
    }

    private void Entry_Completed( object sender, EventArgs e )
    {
      this.RaiseCompletedEvent( this, e );
    }

    private void TextBox_HandlerChanged( object sender, EventArgs e )
    {
      this.InitializeForPlatform( sender, e );
    }

    private void TextBox_HandlerChanging( object sender, HandlerChangingEventArgs e )
    {
      this.UninitializeForPlatform( sender, e );
    }

    private void ClearButton_Clicked( object sender, EventArgs e )
    {
      this.ClearButtonAction();
    }

    #endregion
  }
}
