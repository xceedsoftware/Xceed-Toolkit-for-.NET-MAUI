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


using System.Globalization;

namespace Xceed.Maui.Toolkit
{
  public enum TimePeriod
  {
    AM,
    PM
  }

  public abstract partial class DateTimeUpDownBase<T> : UpDownBase<T>
  {
    #region Members

    internal T m_lastValidDate;
    internal List<DateTimeInfo> m_dateTimeInfoList = new List<DateTimeInfo>();
    internal DateTimeInfo m_selectedDateTimeInfo;
    internal bool m_fireSelectionChangedEvent = true;

    #endregion

    #region Constructors

    internal DateTimeUpDownBase()
    {
      this.InitializeDateTimeInfoList();
      this.Loaded += this.DateTimeUpDownBase_Loaded;

      this.HandlerChanged += this.DateTimeUpDownBase_HandlerChanged;
      this.HandlerChanging += this.DateTimeUpDownBase_HandlerChanging;
    }

    #endregion

    #region Public Properties

    #region CurrentDateTimePart

    public static readonly BindableProperty CurrentDateTimePartProperty = BindableProperty.Create( nameof( CurrentDateTimePart ), typeof( DateTimePart ), typeof( DateTimeUpDownBase<T> ), defaultValue: DateTimePart.Other, propertyChanged: OnCurrentDateTimePartChanged );

    public DateTimePart CurrentDateTimePart
    {
      get => (DateTimePart)GetValue( CurrentDateTimePartProperty );
      set => SetValue( CurrentDateTimePartProperty, value );
    }

    private static void OnCurrentDateTimePartChanged( BindableObject bindable, object oldValue, object newValue )
    {
      if( bindable is DateTimeUpDownBase<T> dateTimeUpDown )
      {
        dateTimeUpDown.OnCurrentDateTimePartChanged( (DateTimePart)oldValue, (DateTimePart)newValue );
      }
    }

    protected virtual void OnCurrentDateTimePartChanged( DateTimePart oldValue, DateTimePart newValue )
    {
      this.Select( this.GetDateTimeInfo( newValue ) );
    }

    #endregion

    #region CustomFormatString

    public static readonly BindableProperty CustomFormatStringProperty = BindableProperty.Create( nameof( CustomFormatString ), typeof( string ), typeof( DateTimeUpDownBase<T> ), defaultValue: null, propertyChanged: OnCustomFormatStringChanged, validateValue: OnValidateCustomFormatString );

    public string CustomFormatString
    {
      get => (string)GetValue( CustomFormatStringProperty );
      set => SetValue( CustomFormatStringProperty, value );
    }

    private static bool OnValidateCustomFormatString( BindableObject bindable, object value )
    {
      if( bindable is DateTimeUpDownBase<T> dateTimeUpDown )
      {
        return dateTimeUpDown.OnValidateCustomFormatString( (string)value );
      }
      return false;
    }

    protected virtual bool OnValidateCustomFormatString( string value )
    {
      return true;
    }

    private static void OnCustomFormatStringChanged( BindableObject bindable, object oldValue, object newValue )
    {
      if( bindable is DateTimeUpDownBase<T> dateTimeUpDown )
      {
        dateTimeUpDown.OnCustomFormatStringChanged( (string)oldValue, (string)newValue );
      }
    }

    protected virtual void OnCustomFormatStringChanged( string oldValue, string newValue )
    {
      this.UpdateFormatPattern();
    }

    #endregion

    #region Step

    public static readonly BindableProperty StepProperty = BindableProperty.Create( nameof( Step ), typeof( int ), typeof( DateTimeUpDownBase<T> ), defaultValue: 1 );

    public int Step
    {
      get => (int)GetValue( StepProperty );
      set => SetValue( StepProperty, value );
    }

    #endregion

    #endregion

    #region Partial Methods

    partial void InitializeForPlatform( object sender, EventArgs e );

    partial void UninitializeForPlatform( object sender, HandlerChangingEventArgs e );

    #endregion

    #region Protected Methods   

    protected abstract T GetValue( T newValue );

    protected abstract string GetStringValue( T newValue, string format, DateTimeFormatInfo dateTimeFormatInfo );

    protected abstract string GetFormattedValueString( T valueToFormat );

    protected abstract bool TryParseString( string text, out T result );

    protected abstract T GetTypeDefault();

    protected abstract T UpdateType( T value, int step );

    protected override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      if( this.TextBox != null )
      {
        this.TextBox.SelectionChanged += this.TextBox_SelectionChanged;
      }
    }

    protected override string ConvertValueToText()
    {
      return this.GetFormattedValueString( this.Value );
    }

    protected override T ConvertTextToValue( string text )
    {
      if( string.IsNullOrEmpty( text ) )
        return default(T);

      T result;
      this.TryParseString( text, out result );

      if( this.ClipValueToMinMax )
      {
        return this.GetClippedMinMaxValue( result );
      }

      this.ValidateDefaultMinMax( result );

      return result;
    }

    protected override bool CommitInput()
    {
      bool isSyncValid = this.SyncTextAndValueProperties( true, this.Text );
      m_lastValidDate = this.Value;
      return isSyncValid;
    }

    protected override void OnIncrement()
    {
      if( this.IsCurrentValueValid() )
      {
        this.Increment( this.Step );
      }
    }

    protected override void OnDecrement()
    {
      if( this.IsCurrentValueValid() )
      {
        this.Increment( -this.Step );
      }
    }

    protected override void SetValidSpinDirection()
    {
      var validDirections = ValidSpinDirections.None;

      if( this.IsLowerThan( this.Value, this.Maximum ) || (this.Value == null) || (this.Maximum == null) )
      {
        validDirections |= ValidSpinDirections.Increase;
      }

      if( this.IsGreaterThan( this.Value, this.Minimum ) || ( this.Value == null ) || ( this.Minimum == null ) )
      {
        validDirections |= ValidSpinDirections.Decrease;
      }

      this.SetValidSpinDirection( validDirections );
    }

    protected override void OnTextChanged( string oldValue, string newValue )
    {
      if( m_isTextChangedFromUI )
        return;

      base.OnTextChanged( oldValue, newValue );
    }

    protected override void OnValueChanged( T oldValue, T newValue )
    {
      //whenever the value changes we need to parse out the value into out DateTimeInfo segments so we can keep track of the individual pieces
      //but only if it is not null
      if( newValue != null )
      {
        this.ParseValueIntoDateTimeInfo( this.Value );
      }

      base.OnValueChanged( oldValue, newValue );

      if( !m_isTextChangedFromUI )
      {
        m_lastValidDate = newValue;
      }
    }

    protected override object OnCoerceValue( object value )
    {
      if( this.ClipValueToMinMax )
        return this.GetClippedMinMaxValue( (T)value );

      this.ValidateDefaultMinMax( (T)value );

      return value;
    }

    protected internal virtual void PerformKeyboardSelection( int nextSelectionStart )
    {
      this.TextBox.Focus();

      this.CommitInput();

      int selectedDateStartPosition = ( m_selectedDateTimeInfo != null ) ? m_selectedDateTimeInfo.StartPosition : 0;
      int direction = nextSelectionStart - selectedDateStartPosition;
      if( direction > 0 )
      {
        this.Select( this.GetNextDateTimeInfo( nextSelectionStart ) );
      }
      else
      {
        this.Select( this.GetPreviousDateTimeInfo( nextSelectionStart - 1 ) );
      }
    }

    protected virtual void PerformMouseSelection()
    {
      var dateTimeInfo = this.GetDateTimeInfo( this.TextBox.CursorPosition );

      this.Select( dateTimeInfo, false );
    }

    protected virtual void InitializeDateTimeInfoList()
    {
      m_dateTimeInfoList.Clear();
      m_selectedDateTimeInfo = null;

      var format = this.GetFormatPatternString();
      if( string.IsNullOrEmpty( format ) )
        return;

      while( format.Length > 0 )
      {
        var elementLength = DateTimeUpDown.GetElementLengthByFormatType( format );
        DateTimeInfo info = null;

        switch( format[ 0 ] )
        {
          case '"':
          case '\'':
            {
              var closingQuotePosition = format.IndexOf( format[ 0 ], 1 );
              info = new DateTimeInfo
              {
                IsReadOnly = true,
                Type = DateTimePart.Other,
                Length = 1,
                Content = format.Substring( 1, Math.Max( 1, closingQuotePosition - 1 ) )
              };
              elementLength = Math.Max( 1, closingQuotePosition + 1 );
            }
            break;
          case 'D':
          case 'd':
            {
              var d = format.Substring( 0, elementLength );
              if( elementLength == 1 )
              {
                d = "%" + d;
              }

              if( elementLength > 2 )
              {
                info = new DateTimeInfo
                {
                  IsReadOnly = true,
                  Type = DateTimePart.DayName,
                  Length = elementLength,
                  Format = d
                };
              }
              else
                info = new DateTimeInfo
                {
                  IsReadOnly = false,
                  Type = DateTimePart.Day,
                  Length = elementLength,
                  Format = d
                };
            }
            break;
          case 'F':
          case 'f':
            {
              var f = format.Substring( 0, elementLength );
              if( elementLength == 1 )
              {
                f = "%" + f;
              }

              info = new DateTimeInfo
              {
                IsReadOnly = false,
                Type = DateTimePart.Millisecond,
                Length = elementLength,
                Format = f
              };
            }
            break;
          case 'h':
            {
              var h = format.Substring( 0, elementLength );
              if( elementLength == 1 )
              {
                h = "%" + h;
              }

              info = new DateTimeInfo
              {
                IsReadOnly = false,
                Type = DateTimePart.Hour12,
                Length = elementLength,
                Format = h
              };
            }
            break;
          case 'H':
            {
              var H = format.Substring( 0, elementLength );
              if( elementLength == 1 )
              {
                H = "%" + H;
              }

              info = new DateTimeInfo
              {
                IsReadOnly = false,
                Type = DateTimePart.Hour24,
                Length = elementLength,
                Format = H
              };
            }
            break;
          case 'M':
            {
              var M = format.Substring( 0, elementLength );
              if( elementLength == 1 )
              {
                M = "%" + M;
              }

              if( elementLength >= 3 )
              {
                info = new DateTimeInfo
                {
                  IsReadOnly = false,
                  Type = DateTimePart.MonthName,
                  Length = elementLength,
                  Format = M
                };
              }
              else
                info = new DateTimeInfo
                {
                  IsReadOnly = false,
                  Type = DateTimePart.Month,
                  Length = elementLength,
                  Format = M
                };
            }
            break;
          case 'S':
          case 's':
            {
              var s = format.Substring( 0, elementLength );
              if( elementLength == 1 )
              {
                s = "%" + s;
              }

              info = new DateTimeInfo
              {
                IsReadOnly = false,
                Type = DateTimePart.Second,
                Length = elementLength,
                Format = s
              };
            }
            break;
          case 'T':
          case 't':
            {
              var t = format.Substring( 0, elementLength );
              if( elementLength == 1 )
              {
                t = "%" + t;
              }

              info = new DateTimeInfo
              {
                IsReadOnly = false,
                Type = DateTimePart.AmPmDesignator,
                Length = elementLength,
                Format = t
              };
            }
            break;
          case 'Y':
          case 'y':
            {
              var y = format.Substring( 0, elementLength );
              if( elementLength == 1 )
              {
                y = "%" + y;
              }

              info = new DateTimeInfo
              {
                IsReadOnly = false,
                Type = DateTimePart.Year,
                Length = elementLength,
                Format = y
              };
            }
            break;
          case '\\':
            {
              if( format.Length >= 2 )
              {
                info = new DateTimeInfo
                {
                  IsReadOnly = true,
                  Content = format.Substring( 1, 1 ),
                  Length = 1,
                  Type = DateTimePart.Other
                };
                elementLength = 2;
              }
            }
            break;
          case 'g':
            {
              var g = format.Substring( 0, elementLength );
              if( elementLength == 1 )
              {
                g = "%" + g;
              }

              info = new DateTimeInfo
              {
                IsReadOnly = true,
                Type = DateTimePart.Period,
                Length = elementLength,
                Format = format.Substring( 0, elementLength )
              };
            }
            break;
          case 'm':
            {
              var m = format.Substring( 0, elementLength );
              if( elementLength == 1 )
              {
                m = "%" + m;
              }

              info = new DateTimeInfo
              {
                IsReadOnly = false,
                Type = DateTimePart.Minute,
                Length = elementLength,
                Format = m
              };
            }
            break;
          case 'z':
            {
              var z = format.Substring( 0, elementLength );
              if( elementLength == 1 )
              {
                z = "%" + z;
              }

              info = new DateTimeInfo
              {
                IsReadOnly = true,
                Type = DateTimePart.TimeZone,
                Length = elementLength,
                Format = z
              };
            }
            break;
          default:
            {
              elementLength = 1;
              info = new DateTimeInfo
              {
                IsReadOnly = true,
                Length = 1,
                Content = format[ 0 ].ToString(),
                Type = DateTimePart.Other
              };
            }
            break;
        }

        m_dateTimeInfoList.Add( info );
        format = format.Substring( elementLength );
      }
    }

    protected virtual void ParseValueIntoDateTimeInfo( T newDate )
    {
      var text = string.Empty;

      m_dateTimeInfoList.ForEach( info =>
      {
        if( info.Format == null )
        {
          info.StartPosition = text.Length;
          info.Length = info.Content.Length;
          text += info.Content;
        }
        else if( newDate != null )
        {
          var date = this.GetValue( newDate );
          info.StartPosition = text.Length;
          info.Content = this.GetStringValue( date, info.Format, CultureInfo.DateTimeFormat );
          info.Length = info.Content.Length;
          text += info.Content;
        }
      } );
    }

    protected virtual string GetFormatPatternString()
    {
      return string.Empty;
    }

    protected virtual void InitSelection()
    {
    }

    protected override bool IsGreaterThan( T value1, T value2 )
    {
      return false;
    }

    protected override bool IsLowerThan( T value1, T value2 )
    {
      return false;
    }

    protected override void OnCultureInfoChanged( CultureInfo oldValue, CultureInfo newValue )
    {
      this.UpdateFormatPattern();
    }

    #endregion

    #region Internal Methods

    internal bool IsCurrentValueValid()
    {
      if( string.IsNullOrEmpty( this.TextBox?.Text ) )
        return true;

      return this.TryParseString( this.TextBox.Text, out T result );
    }

    internal T GetClippedMinMaxValue( T value )
    {
      if( this.IsGreaterThan( value, this.Maximum ) )
        return this.Maximum;
      else if( this.IsLowerThan( value, this.Minimum ) )
        return this.Minimum;
      return value;
    }

    internal void ValidateDefaultMinMax( T value )
    {
      // DefaultValue is always accepted.
      if( object.Equals( value, this.DefaultValue ) )
        return;

      if( this.IsLowerThan( value, this.Minimum ) )
        throw new ArgumentOutOfRangeException( "Minimum", String.Format( "Value must be greater than MinValue of {0}", this.Minimum ) );
      else if( this.IsGreaterThan( value, this.Maximum ) )
        throw new ArgumentOutOfRangeException( "Maximum", String.Format( "Value must be less than MaxValue of {0}", this.Maximum ) );
    }

    internal DateTimeInfo GetDateTimeInfo( DateTimePart part )
    {
      return m_dateTimeInfoList.FirstOrDefault( ( info ) => info.Type == part );
    }

    internal DateTimeInfo GetDateTimeInfo( int selectionStart )
    {
      return m_dateTimeInfoList.FirstOrDefault( ( info ) =>
                              ( info.StartPosition <= selectionStart ) && ( selectionStart < ( info.StartPosition + info.Length ) ) );
    }

    internal virtual void Select( DateTimeInfo info, bool updateTextBoxSelection = true )
    {
      if( ( info != null ) && !info.Equals( m_selectedDateTimeInfo ) && ( info.Type != DateTimePart.Other ) && ( this.TextBox != null ) && !string.IsNullOrEmpty( this.TextBox.Text ) )
      {
        if( updateTextBoxSelection )
        {
          this.SetTextBoxSelection( info.StartPosition, info.Length );
        }
        m_selectedDateTimeInfo = info;

        this.SetValue( DateTimeUpDownBase<T>.CurrentDateTimePartProperty, info.Type );
      }
    }

    internal void SetTextBoxSelection( int start, int length )
    {
      if( start < 0 )
        throw new InvalidDataException( "TextBox SelectionStart must be greater than or equal to 0." );
      if( length < 0 )
        throw new InvalidDataException( "TextBox SelectionLength must be greater than or equal to 0." );

      m_fireSelectionChangedEvent = false;

      this.TextBox.CursorPosition = ( this.TextBox.CursorPosition != 0 ) ? 0 : this.TextBox.Text.Length;
      this.TextBox.SelectionLength = 0;

      this.TextBox.CursorPosition = start;
      this.TextBox.SelectionLength = length;

      m_fireSelectionChangedEvent = true;
    }

    internal TextBox GetTextBox()
    {
      return this.TextBox;
    }

    internal void UpdateFormatPattern()
    {
      this.InitializeDateTimeInfoList();

      if( this.Value != null )
      {
        this.ParseValueIntoDateTimeInfo( this.Value );
      }

      this.SyncTextAndValueProperties( false, null );
    }

    #endregion

    #region Private Methods    

    private static int GetElementLengthByFormatType( string format )
    {
      for( int i = 1; i < format.Length; i++ )
      {
        if( string.Compare( format[ i ].ToString(), format[ 0 ].ToString(), false ) != 0 )
          return i;
      }
      return format.Length;
    }

    private void Increment( int step )
    {
      m_fireSelectionChangedEvent = false;

      var currentValue = this.ConvertTextToValue( this.TextBox.Text );
      if( currentValue != null )
      {
        var newValue = this.UpdateType( currentValue, step );
        if( newValue == null )
          return;
        this.TextBox.Text = this.GetFormattedValueString( newValue );
      }
      else
      {
        this.TextBox.Text = ( this.DefaultValue != null )
                            ? this.GetFormattedValueString( this.DefaultValue )
                            : this.GetFormattedValueString( this.GetTypeDefault() );
      }

      if( this.TextBox != null )
      {
        var info = m_selectedDateTimeInfo;
        //this only occurs when the user manually type in a value for the Value Property
        if( info == null )
        {
          info = ( this.CurrentDateTimePart != DateTimePart.Other ) ? this.GetDateTimeInfo( this.CurrentDateTimePart ) : m_dateTimeInfoList[ 0 ];
        }
        if( info == null )
        {
          info = m_dateTimeInfoList[ 0 ];
        }

        //whenever the value changes we need to parse out the value into out DateTimeInfo segments so we can keep track of the individual pieces
        this.ParseValueIntoDateTimeInfo( this.ConvertTextToValue( this.TextBox.Text ) );

        //we loose our selection when the Value is set so we need to reselect it without firing the selection changed event
        this.SetTextBoxSelection( info.StartPosition, info.Length );
      }
      m_fireSelectionChangedEvent = true;

      this.SyncTextAndValueProperties( true, Text );
    }

    private DateTimeInfo GetNextDateTimeInfo( int nextSelectionStart )
    {
      var nextDateTimeInfo = this.GetDateTimeInfo( nextSelectionStart );
      if( nextDateTimeInfo == null )
      {
        nextDateTimeInfo = m_dateTimeInfoList.First();
      }

      var initialDateTimeInfo = nextDateTimeInfo;

      while( nextDateTimeInfo.Type == DateTimePart.Other )
      {
        nextDateTimeInfo = this.GetDateTimeInfo( nextDateTimeInfo.StartPosition + nextDateTimeInfo.Length );
        if( nextDateTimeInfo == null )
        {
          nextDateTimeInfo = m_dateTimeInfoList.First();
        }
        if( object.Equals( nextDateTimeInfo, initialDateTimeInfo ) )
          throw new InvalidOperationException( "Couldn't find a valid DateTimeInfo." );
      }

      return nextDateTimeInfo;
    }

    private DateTimeInfo GetPreviousDateTimeInfo( int previousSelectionStart )
    {
      var previousDateTimeInfo = this.GetDateTimeInfo( previousSelectionStart );
      if( previousDateTimeInfo == null )
      {
        if( m_dateTimeInfoList.Count > 0 )
        {
          previousDateTimeInfo = m_dateTimeInfoList.Last();
        }
      }

      var initialDateTimeInfo = previousDateTimeInfo;

      while( ( previousDateTimeInfo != null ) && ( previousDateTimeInfo.Type == DateTimePart.Other ) )
      {
        previousDateTimeInfo = this.GetDateTimeInfo( previousDateTimeInfo.StartPosition - 1 );
        if( previousDateTimeInfo == null )
        {
          previousDateTimeInfo = m_dateTimeInfoList.Last();
        }
        if( object.Equals( previousDateTimeInfo, initialDateTimeInfo ) )
          throw new InvalidOperationException( "Couldn't find a valid DateTimeInfo." );
      }

      return previousDateTimeInfo;
    }

    #endregion

    #region Event Handlers

    private void DateTimeUpDownBase_Loaded( object sender, EventArgs e )
    {
      this.InitSelection();
    }

    private void DateTimeUpDownBase_HandlerChanged( object sender, EventArgs e )
    {
      this.InitializeForPlatform( sender, e );
    }

    private void DateTimeUpDownBase_HandlerChanging( object sender, HandlerChangingEventArgs e )
    {
      this.UninitializeForPlatform( sender, e );
    }

    private void TextBox_SelectionChanged( object sender, EventArgs e )
    {
      if( m_fireSelectionChangedEvent )
      {
        this.PerformMouseSelection();
      }
    }

    #endregion
  }
}
