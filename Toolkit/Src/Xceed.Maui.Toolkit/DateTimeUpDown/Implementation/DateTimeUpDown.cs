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
  public class DateTimeUpDown: DateTimeUpDownBase<DateTime?>
  {
    #region Members

    private DateTime? _lastValidDate;

    #endregion

    #region Constructors

    public DateTimeUpDown()
    {
      this.Loaded += this.DateTimeUpDown_Loaded;
    }  

    #endregion

    #region Public Properties

    #region CustomFormatString

    public static readonly BindableProperty CustomFormatStringProperty = BindableProperty.Create( nameof( CustomFormatString ), typeof( string ), typeof( DateTimeUpDown ), defaultValue: null, propertyChanged: OnCustomFormatStringChanged, validateValue: OnValidateCustomFormatString );

    public string CustomFormatString
    {
      get => (string)GetValue( CustomFormatStringProperty );
      set => SetValue( CustomFormatStringProperty, value );
    }

    private static bool OnValidateCustomFormatString( BindableObject bindable, object value )
    {
      try
      {
        CultureInfo.CurrentCulture.DateTimeFormat.Calendar.MinSupportedDateTime.ToString( (string)value, CultureInfo.CurrentCulture );
      }
      catch
      {
        throw new InvalidDataException( "Unknown CustomFormatString value.");
      }

      return true;
    }

    private static void OnCustomFormatStringChanged( BindableObject bindable, object oldValue, object newValue )
    {
      if( bindable is DateTimeUpDown dateTimeUpDown )
      {
        dateTimeUpDown.OnCustomFormatStringChanged( (string)oldValue, (string)newValue );
      }
    }

    protected virtual void OnCustomFormatStringChanged( string oldValue, string newValue )
    {
      this.UpdateFormatPattern();
    }

    #endregion

    #region FormatPattern

    public static readonly BindableProperty FormatPatternProperty = BindableProperty.Create( nameof( FormatPattern ), typeof( DateTimeFormatPattern ), typeof( DateTimeUpDown ), defaultValue: DateTimeFormatPattern.FullDateTime, propertyChanged: OnFormatPatternChanged );

    public DateTimeFormatPattern FormatPattern
    {
      get => (DateTimeFormatPattern)GetValue( FormatPatternProperty );
      set => SetValue( FormatPatternProperty, value );
    }

    private static void OnFormatPatternChanged( BindableObject bindable, object oldValue, object newValue )
    {
      if( bindable is DateTimeUpDown dateTimeUpDown )
      {
        dateTimeUpDown.OnFormatPatternChanged( (DateTimeFormatPattern)oldValue, (DateTimeFormatPattern)newValue );
      }
    }

    protected virtual void OnFormatPatternChanged( DateTimeFormatPattern oldValue, DateTimeFormatPattern newValue )
    {
      this.UpdateFormatPattern();
    }

    #endregion

    #endregion

    #region Protected Methods

    protected override bool CommitInput()
    {
      bool isSyncValid = this.SyncTextAndValueProperties( true, this.Text );
      _lastValidDate = this.Value;
      return isSyncValid;
    }

    protected override void SetDefaultValues()
    {
      this.Minimum = CultureInfo.CurrentCulture.DateTimeFormat.Calendar.MinSupportedDateTime;
      this.Maximum = CultureInfo.CurrentCulture.DateTimeFormat.Calendar.MaxSupportedDateTime;
    }

    protected override void OnTextChanged( string oldValue, string newValue )
    {
      if( m_isTextChangedFromUI )
        return;

      base.OnTextChanged( oldValue, newValue );
    }

    protected override void OnValueChanged( DateTime? oldValue, DateTime? newValue )
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
        _lastValidDate = newValue;
      }
    }

    protected override string ConvertValueToText()
    {
      if( this.Value == null )
        return string.Empty;

      return this.Value.Value.ToString( this.GetFormatPatternString(), this.CultureInfo );
    }

#nullable enable
    protected override DateTime? ConvertTextToValue( string text )
    {
      if( string.IsNullOrEmpty( text ) )
        return null;

      DateTime result;
      this.TryParseDateTime( text, out result );

      if( this.ClipValueToMinMax )
      {
        return this.GetClippedMinMaxValue( result );
      }

      this.ValidateDefaultMinMax( result );

      return result;
    }

#nullable disable

    protected override void InitializeDateTimeInfoList()
    {
      _dateTimeInfoList.Clear();
      _selectedDateTimeInfo = null;

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

        _dateTimeInfoList.Add( info );
        format = format.Substring( elementLength );
      }
    }

    protected override bool IsCurrentValueValid()
    {
      DateTime result;

      if( string.IsNullOrEmpty( this.TextBox?.Text ) )
        return true;

      return this.TryParseDateTime( this.TextBox.Text, out result );
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

    protected override bool IsLowerThan( DateTime? value1, DateTime? value2 )
    {
      if( value1 == null || value2 == null )
        return false;

      return ( value1.Value < value2.Value );
    }

    protected override bool IsGreaterThan( DateTime? value1, DateTime? value2 )
    {
      if( value1 == null || value2 == null )
        return false;

      return ( value1.Value > value2.Value );
    }

    protected override void OnCultureInfoChanged( CultureInfo oldValue, CultureInfo newValue )
    {
      this.UpdateFormatPattern();
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

    private string GetFormatPatternString()
    {
      switch( this.FormatPattern )
      {
        case DateTimeFormatPattern.ShortDate:
          return CultureInfo.DateTimeFormat.ShortDatePattern;
        case DateTimeFormatPattern.LongDate:
          return CultureInfo.DateTimeFormat.LongDatePattern;
        case DateTimeFormatPattern.ShortTime:
          return CultureInfo.DateTimeFormat.ShortTimePattern;
        case DateTimeFormatPattern.LongTime:
          return CultureInfo.DateTimeFormat.LongTimePattern;
        case DateTimeFormatPattern.FullDateTime:
          return CultureInfo.DateTimeFormat.FullDateTimePattern;
        case DateTimeFormatPattern.MonthDay:
          return CultureInfo.DateTimeFormat.MonthDayPattern;
        case DateTimeFormatPattern.RFC1123:
          return CultureInfo.DateTimeFormat.RFC1123Pattern;
        case DateTimeFormatPattern.SortableDateTime:
          return CultureInfo.DateTimeFormat.SortableDateTimePattern;
        case DateTimeFormatPattern.UniversalSortableDateTime:
          return CultureInfo.DateTimeFormat.UniversalSortableDateTimePattern;
        case DateTimeFormatPattern.YearMonth:
          return CultureInfo.DateTimeFormat.YearMonthPattern;
        case DateTimeFormatPattern.Custom:
          {
            switch( this.CustomFormatString )
            {
              case "d":
                return CultureInfo.DateTimeFormat.ShortDatePattern;
              case "t":
                return CultureInfo.DateTimeFormat.ShortTimePattern;
              case "T":
                return CultureInfo.DateTimeFormat.LongTimePattern;
              case "D":
                return CultureInfo.DateTimeFormat.LongDatePattern;
              case "f":
                return CultureInfo.DateTimeFormat.LongDatePattern + " " + CultureInfo.DateTimeFormat.ShortTimePattern;
              case "F":
                return CultureInfo.DateTimeFormat.FullDateTimePattern;
              case "g":
                return CultureInfo.DateTimeFormat.ShortDatePattern + " " + CultureInfo.DateTimeFormat.ShortTimePattern;
              case "G":
                return CultureInfo.DateTimeFormat.ShortDatePattern + " " + CultureInfo.DateTimeFormat.LongTimePattern;
              case "m":
                return CultureInfo.DateTimeFormat.MonthDayPattern;
              case "y":
                return CultureInfo.DateTimeFormat.YearMonthPattern;
              case "r":
                return CultureInfo.DateTimeFormat.RFC1123Pattern;
              case "s":
                return CultureInfo.DateTimeFormat.SortableDateTimePattern;
              case "u":
                return CultureInfo.DateTimeFormat.UniversalSortableDateTimePattern;
              default:
                return this.CustomFormatString;
            }
          }
        default:
          throw new InvalidDataException( "FormatPattern not supported for DatetimeUpDown" );
      }
    }

    private bool TryParseDateTime( string text, out DateTime result )
    {
      bool isValid = false;
      result = DateTime.Now;

      var current = DateTime.Now;
      try
      {
        // TempValue is used when Manipulating TextBox.Text while Value is not updated yet (used in DateTimePicker's TimePicker).
        current = /*this.TempValue.HasValue
                  ? this.TempValue.Value
                  : */this.Value.HasValue ? this.Value.Value : DateTime.Parse( current.ToString(), this.CultureInfo.DateTimeFormat );

        isValid = DateTimeParser.TryParse( text, this.GetFormatPatternString(), current, this.CultureInfo, out result );
      }
      catch( Exception )
      {
        isValid = false;
      }

      if( !isValid )
      {
        isValid = DateTime.TryParseExact( text, this.GetFormatPatternString(), this.CultureInfo, DateTimeStyles.None, out result );
      }

      if( !isValid )
      {
        result = ( _lastValidDate != null ) ? _lastValidDate.Value : current;
      }

      return isValid;
    }

    private void Increment( int step )
    {
      _fireSelectionChangedEvent = false;

      var currentValue = this.ConvertTextToValue( this.TextBox.Text );
      if( currentValue.HasValue )
      {
        var newValue = this.UpdateDateTime( currentValue, step );
        if( newValue == null )
          return;
        this.TextBox.Text = newValue.Value.ToString( this.GetFormatPatternString(), this.CultureInfo );
      }
      else
      {
        this.TextBox.Text = ( this.DefaultValue != null )
                            ? this.DefaultValue.Value.ToString( this.GetFormatPatternString(), this.CultureInfo )
                            : DateTime.Now.ToString( this.GetFormatPatternString(), this.CultureInfo );
      }

      if( this.TextBox != null )
      {
        var info = _selectedDateTimeInfo;
        //this only occurs when the user manually type in a value for the Value Property
        if( info == null )
        {
          info = ( this.CurrentDateTimePart != DateTimePart.Other ) ? this.GetDateTimeInfo( this.CurrentDateTimePart ) : _dateTimeInfoList[ 0 ];
        }
        if( info == null )
        {
          info = _dateTimeInfoList[ 0 ];
        }

        //whenever the value changes we need to parse out the value into out DateTimeInfo segments so we can keep track of the individual pieces
        this.ParseValueIntoDateTimeInfo( this.ConvertTextToValue( this.TextBox.Text ) );

        //we loose our selection when the Value is set so we need to reselect it without firing the selection changed event
        this.SetTextBoxSelection( info.StartPosition, info.Length );
      }
      _fireSelectionChangedEvent = true;

      this.SyncTextAndValueProperties( true, Text );
    }

    private DateTime? UpdateDateTime( DateTime? currentDateTime, int value )
    {
      var info = _selectedDateTimeInfo;

      //this only occurs when the user manually type in a value for the Value Property
      if( info == null )
      {
        info = ( this.CurrentDateTimePart != DateTimePart.Other ) ? this.GetDateTimeInfo( this.CurrentDateTimePart ) : _dateTimeInfoList[ 0 ];
      }
      if( info == null )
      {
        info = _dateTimeInfoList[ 0 ];
      }

      DateTime? result = null;

      try
      {
        switch( info.Type )
        {
          case DateTimePart.Year:
            {
              result = ( (DateTime)currentDateTime ).AddYears( value );              
            }
            break;
          case DateTimePart.Month:
          case DateTimePart.MonthName:
            {
              result = ( (DateTime)currentDateTime ).AddMonths( value );
            }
            break;
          case DateTimePart.Day:
          case DateTimePart.DayName:
            {
              result = ( (DateTime)currentDateTime ).AddDays( value );
            }
            break;
          case DateTimePart.Hour12:
          case DateTimePart.Hour24:
            {
              result = ( (DateTime)currentDateTime ).AddHours( value );
            }
            break;
          case DateTimePart.Minute:
            {
              result = ( (DateTime)currentDateTime ).AddMinutes( value );
            }
            break;
          case DateTimePart.Second:
            {
              result = ( (DateTime)currentDateTime ).AddSeconds( value );
            }
            break;
          case DateTimePart.Millisecond:
            {
              result = ( (DateTime)currentDateTime ).AddMilliseconds( value );
            }
            break;
          case DateTimePart.AmPmDesignator:
            {
              result = ( (DateTime)currentDateTime ).AddHours( value * 12 );
            }
            break;
          default:
            {
              throw new InvalidDataException( "Unknown DateTimePart." );
            }
        }
      }
      catch
      {
        //this can occur if the date/time = 1/1/0001 12:00:00 AM which is the smallest date allowed.
        //I could write code that would validate the date each and everytime but I think that it would be more
        //efficient if I just handle the edge case and allow an exeption to occur and swallow it instead.
      }

      return this.GetClippedMinMaxValue( result );
    }

    private void ParseValueIntoDateTimeInfo( DateTime? newDate )
    {
      var text = string.Empty;

      _dateTimeInfoList.ForEach( info =>
      {
        if( info.Format == null )
        {
          info.StartPosition = text.Length;
          info.Length = info.Content.Length;
          text += info.Content;
        }
        else if( newDate != null )
        {
          var date = newDate.Value;
          info.StartPosition = text.Length;
          info.Content = date.ToString( info.Format, CultureInfo.DateTimeFormat );
          info.Length = info.Content.Length;
          text += info.Content;
        }
      } );
    }

    private void UpdateFormatPattern()
    {
      this.InitializeDateTimeInfoList();

      if( this.Value != null )
      {
        this.ParseValueIntoDateTimeInfo( this.Value );
      }

      this.SyncTextAndValueProperties( false, null );
    }

    #endregion

    #region Event Handlers

    private void DateTimeUpDown_Loaded( object sender, EventArgs e )
    {
      if( ( this.FormatPattern == DateTimeFormatPattern.Custom ) && string.IsNullOrEmpty( this.CustomFormatString ) )
        throw new InvalidOperationException( "A CustomFormatString is necessary when FormatPattern is set to Custom." );
    }

    #endregion
  }
}
