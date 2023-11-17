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
using System.Globalization;

namespace Xceed.Maui.Toolkit
{
  public enum MaskedTextBoxValueFormat
  {
    ExcludePromptAndLiterals,
    IncludePrompt,
    IncludeLiterals,
    IncludePromptAndLiterals
  }

  public partial class MaskedTextBox : TextBox
  {
    #region Private Members

    private static string NullMaskString = "<>";

    private Entry m_entry;
    private MaskedTextProvider m_maskProvider = new MaskedTextProvider( MaskedTextBox.NullMaskString );
    private bool m_isUpdatingText;
    private bool m_isUpdatingValue;

    #endregion

    #region Public Properties

    #region CultureInfo

    public static readonly BindableProperty CultureInfoProperty = BindableProperty.Create( nameof( CultureInfo ), typeof( CultureInfo ), typeof( MaskedTextBox ), CultureInfo.CurrentCulture, propertyChanged: OnCultureInfoChanged );

    public CultureInfo CultureInfo
    {
      get => (CultureInfo)GetValue( CultureInfoProperty );
      set => SetValue( CultureInfoProperty, value );
    }

    private static void OnCultureInfoChanged( BindableObject bindable, object oldValue, object newValue )
    {
      if( bindable is MaskedTextBox maskedTextBox )
      {
        maskedTextBox.OnCultureInfoChanged( (CultureInfo)oldValue, (CultureInfo)newValue );
      }
    }

    protected virtual void OnCultureInfoChanged( CultureInfo oldValue, CultureInfo newValue )
    {
      this.SetMaskProvider();
    }

    #endregion

    #region IsMaskCompleted

    public static readonly BindableProperty IsMaskCompletedProperty = BindableProperty.Create( nameof( IsMaskCompleted ), typeof( bool ), typeof( MaskedTextBox ), false );

    public bool IsMaskCompleted
    {
      get => (bool)GetValue( IsMaskCompletedProperty );
      private set => SetValue( IsMaskCompletedProperty, value );
    }

    #endregion

    #region IsMaskFull

    public static readonly BindableProperty IsMaskFullProperty = BindableProperty.Create( nameof( IsMaskFull ), typeof( bool ), typeof( MaskedTextBox ), false );

    public bool IsMaskFull
    {
      get => (bool)GetValue( IsMaskFullProperty );
      private set => SetValue( IsMaskFullProperty, value );
    }

    #endregion

    #region Mask

    public static readonly BindableProperty MaskProperty = BindableProperty.Create( nameof( Mask ), typeof( string ), typeof( MaskedTextBox ), null, propertyChanged: OnMaskedChanged );

    public string Mask
    {
      get => (string)GetValue( MaskProperty );
      set => SetValue( MaskProperty, value );
    }

    private static void OnMaskedChanged( BindableObject bindable, object oldValue, object newValue )
    {
      if( bindable is MaskedTextBox maskedTextBox )
      {
        maskedTextBox.OnMaskChanged( (string)oldValue, (string)newValue );
      }
    }

    protected virtual void OnMaskChanged( string oldValue, string newValue )
    {
      this.SetMaskProvider();
    }

    #endregion

    #region PromptChar

    public static readonly BindableProperty PromptCharProperty = BindableProperty.Create( nameof( PromptChar ), typeof( char ), typeof( MaskedTextBox ), '_', propertyChanged: OnPromptCharChanged );

    public char PromptChar
    {
      get => (char)GetValue( PromptCharProperty );
      set => SetValue( PromptCharProperty, value );
    }

    private static void OnPromptCharChanged( BindableObject bindable, object oldValue, object newValue )
    {
      if( bindable is MaskedTextBox maskedTextBox )
      {
        maskedTextBox.OnPromptCharChanged( (char)oldValue, (char)newValue );
      }
    }

    protected virtual void OnPromptCharChanged( char oldValue, char newValue )
    {
      m_maskProvider.PromptChar = newValue;

      this.SetEntry( this.MaskedTextOutput, this.CursorPosition );
    }

    #endregion

    #region Value

    public static readonly BindableProperty ValueProperty = BindableProperty.Create( nameof( Value ), typeof( string ), typeof( MaskedTextBox ), null, propertyChanged: OnValueChanged );

    public string Value
    {
      get => (string)GetValue( ValueProperty );
      set => SetValue( ValueProperty, value );
    }

    private static void OnValueChanged( BindableObject bindable, object oldValue, object newValue )
    {
      if( bindable is MaskedTextBox maskedTextBox )
      {
        maskedTextBox.OnValueChanged( (string)oldValue, (string)newValue );
      }
    }

    protected virtual void OnValueChanged( string oldValue, string newValue )
    {
      if( m_isUpdatingValue )
        return;

      if( this.IsMaskSet() )
      {
        this.SetMaskProviderValue( newValue );

        this.SetEntry( this.MaskedTextOutput, this.CursorPosition );
      }
    }
    #endregion

    #region ValueFormat

    public static readonly BindableProperty ValueFormatProperty = BindableProperty.Create( nameof( ValueFormat ), typeof( MaskedTextBoxValueFormat ), typeof( MaskedTextBox ), MaskedTextBoxValueFormat.ExcludePromptAndLiterals, propertyChanged: OnValueFormatChanged );

    public MaskedTextBoxValueFormat ValueFormat
    {
      get => (MaskedTextBoxValueFormat)GetValue( ValueFormatProperty );
      set => SetValue( ValueFormatProperty, value );
    }

    private static void OnValueFormatChanged( BindableObject bindable, object oldValue, object newValue )
    {
      if( bindable is MaskedTextBox maskedTextBox )
      {
        maskedTextBox.OnValueFormatChanged( (MaskedTextBoxValueFormat)oldValue, (MaskedTextBoxValueFormat)newValue );
      }
    }

    protected virtual void OnValueFormatChanged( MaskedTextBoxValueFormat oldValue, MaskedTextBoxValueFormat newValue )
    {
      this.SetValue();
    }

    #endregion

    #endregion

    #region Internal Properties

    internal string MaskedTextOutput
    {
      get
      {
        if( this.IsMaskSet() )
          return m_maskProvider.ToString( false, true, true, 0, m_maskProvider.Length );

        return this.Value;
      }
    }

    #endregion

    #region Partial Methods

    partial void Entry_Initialize( Entry entry );

    partial void Entry_Destroy( Entry entry );

    #endregion

    #region Override Methods

    protected override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      if( m_entry != null )
      {
        m_entry.HandlerChanged -= this.Entry_HandlerChanged;
        m_entry.HandlerChanging -= this.Entry_HandlerChanging;
      }
      m_entry = this.GetTemplateChild( "PART_Entry" ) as Entry;
      if( m_entry != null )
      {
        m_entry.HandlerChanged += this.Entry_HandlerChanged;
        m_entry.HandlerChanging += this.Entry_HandlerChanging;
      }
    }

    protected override object OnCoerceText( string value )
    {
      if( m_isUpdatingText )
        return value;

      this.SetMaskProviderValue( value );

      return this.MaskedTextOutput;
    }

    protected override void OnTextChanged( string oldValue, string newValue )
    {
      this.RaiseTextChangedEvent( this, new TextChangedEventArgs( oldValue, newValue ) );

      if( m_isUpdatingText )
        return;

      this.SetEntry( newValue, this.CursorPosition );
    }

    protected override void Entry_TextChanged( object sender, TextChangedEventArgs e )
    {
      if( m_isUpdatingText && ( this.Text != e.NewTextValue ) )
      {
        this.Text = e.NewTextValue;
      }
    }

    protected internal override void ClearButtonAction()
    {
      m_maskProvider.RemoveAt( 0, Math.Max( 0, m_maskProvider.Length - 1 ), out int nextCursorPosition, out MaskedTextResultHint unused );

      this.SetEntry( this.MaskedTextOutput, nextCursorPosition );
    }

    #endregion

    #region Internal Methods   

    internal void SetEntry( string newText, int cursorPosition )
    {
      if( m_entry != null )
      {
        m_isUpdatingText = true;
        m_entry.Text = newText;
        m_isUpdatingText = false;

        m_entry.CursorPosition = Math.Max( 0, cursorPosition );
      }

      this.IsMaskFull = m_maskProvider.MaskFull;
      this.IsMaskCompleted = m_maskProvider.MaskCompleted;
      this.SetValue();
    }

    internal bool TryInsertChar( char newChar, int start, int end, out int nextPosition, out MaskedTextResultHint resultHint )
    {
      resultHint = MaskedTextResultHint.Unknown;
      nextPosition = start;

      if( start < 0 )
      {
        start = 0;
      }

      if( end < start )
        return false;

      if( m_maskProvider == null )
      {
        nextPosition = start++;
        return true;
      }

      if( end >= m_maskProvider.Length )
      {
        end = m_maskProvider.Length - 1;
      }

      var result = false;

      if( end == start )
      {
        result = m_maskProvider.InsertAt( newChar, start, out nextPosition, out resultHint );
      }
      else
      {
        result = m_maskProvider.Replace( newChar, start, end, out nextPosition, out resultHint );
      }

      nextPosition = result ? nextPosition + 1 : this.CursorPosition;

      return result;
    }

    internal bool TryRemoveChar( int start, int end, out int nextPosition, out MaskedTextResultHint resultHint )
    {
      resultHint = MaskedTextResultHint.Unknown;
      nextPosition = start;

      if( start < 0 )
      {
        start = 0;
      }

      if( end < start )
        return false;

      if( m_maskProvider == null )
      {
        nextPosition = Math.Max( 0, start-- );        
        return true;
      }

      if( end >= m_maskProvider.Length )
      {
        end = m_maskProvider.Length - 1;
      }

      var result = m_maskProvider.RemoveAt( start, end, out nextPosition, out resultHint );

      return result;
    }

    internal bool TryReplaceString( string newString, int start, int end, out int nextPosition, out MaskedTextResultHint resultHint )
    {
      resultHint = MaskedTextResultHint.Unknown;
      nextPosition = start;

      if( start < 0 )
      {
        start = 0;
      }

      if( m_maskProvider == null )
      {
        nextPosition = start++;
        return true;
      }

      if( end >= m_maskProvider.Length )
      {
        end = m_maskProvider.Length - 1;
      }

      var initialMaskProvider = m_maskProvider.Clone();

      if( end >= start )
      {
        this.TryRemoveChar( start, end, out int unusedNextPosition, out MaskedTextResultHint unused );
      }

      start = m_maskProvider.FindEditPositionFrom( start, true );
      if( start == MaskedTextProvider.InvalidIndex )
        return false;

      foreach( char ch in newString )
      {
        if( !this.TryInsertChar( ch, start, start, out nextPosition, out MaskedTextResultHint hint ) )
        {
          // replace all char that fits.
          if( hint == MaskedTextResultHint.PositionOutOfRange )
          {
            nextPosition = start;
            break;
          }

          // replacement can't be done : revert to initial text.
          m_maskProvider.Set( initialMaskProvider.ToString() );

          return false;
        }

        start = nextPosition;
      }

      return true;
    }

    internal MaskedTextProvider GetMaskedTextProvider()
    {
      return m_maskProvider;
    }

    internal bool IsMaskSet()
    {
      return ( m_maskProvider.Mask != MaskedTextBox.NullMaskString );
    }

    #endregion

    #region Private Methods

    private void SetMaskProvider()
    {
      m_maskProvider = new MaskedTextProvider( this.Mask ?? MaskedTextBox.NullMaskString, this.CultureInfo )
      {
        PromptChar = this.PromptChar
      };

      this.SetMaskProviderValue( this.Value );

      this.SetEntry( this.MaskedTextOutput, this.CursorPosition );
    }

    private void SetMaskProviderValue( string newValue )
    {
      if( !this.IsMaskSet() )
        return;

      if( newValue == null )
      {
        newValue = string.Empty;
      }

      if( !m_maskProvider.VerifyString( newValue ) )
        throw new InvalidDataException( "The value representation '" + newValue + "' does not match the mask." );

      m_maskProvider.Set( newValue );
    }

    private void SetValue()
    {
      m_isUpdatingValue = true;
      this.Value = this.IsMaskSet() ? m_maskProvider.ToString( this.IsValueIncludingPrompt(), this.IsValueIncludingLiterals() ) : null;
      m_isUpdatingValue = false;
    }

    private bool IsValueIncludingPrompt()
    {
      return ( this.ValueFormat == MaskedTextBoxValueFormat.IncludePrompt ) 
           || ( this.ValueFormat == MaskedTextBoxValueFormat.IncludePromptAndLiterals );
    }

    private bool IsValueIncludingLiterals()
    {
      return ( this.ValueFormat == MaskedTextBoxValueFormat.IncludeLiterals )
           || ( this.ValueFormat == MaskedTextBoxValueFormat.IncludePromptAndLiterals );
    }

    #endregion

    #region Event Handlers

    private void Entry_HandlerChanged( object sender, EventArgs e )
    {
      var entry = sender as Entry;
      if( entry != null )
      {
        this.Entry_Initialize( entry );
      }
    }

    private void Entry_HandlerChanging( object sender, HandlerChangingEventArgs e )
    {
      var entry = sender as Entry;
      if( entry != null )
      {
        this.Entry_Destroy( entry );
      }
    }

    #endregion
  }
}
