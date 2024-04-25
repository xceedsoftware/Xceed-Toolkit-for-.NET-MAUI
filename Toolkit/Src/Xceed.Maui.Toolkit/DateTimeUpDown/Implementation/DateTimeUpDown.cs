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
    #region Constructors

    public DateTimeUpDown()
    {
      this.Loaded += this.DateTimeUpDown_Loaded;
    }  

    #endregion

    #region Public Properties

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

    protected override bool OnValidateCustomFormatString( string value )
    {
      try
      {
        CultureInfo.CurrentCulture.DateTimeFormat.Calendar.MinSupportedDateTime.ToString( value, CultureInfo.CurrentCulture );
      }
      catch
      {
        throw new InvalidDataException( "Unknown CustomFormatString value." );
      }

      return true;
    }

    protected override void SetDefaultValues()
    {
      this.Minimum = CultureInfo.CurrentCulture.DateTimeFormat.Calendar.MinSupportedDateTime;
      this.Maximum = CultureInfo.CurrentCulture.DateTimeFormat.Calendar.MaxSupportedDateTime;
    }      

    protected override string GetFormattedValueString( DateTime? valueToFormat )
    {
      if( valueToFormat == null )
        return string.Empty;

      return valueToFormat.Value.ToString( this.GetFormatPatternString(), this.CultureInfo );
    }

#nullable enable
    protected override bool TryParseString( string text, out DateTime? result )
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

        isValid = DateTimeParser.TryParse( text, this.GetFormatPatternString(), current, this.CultureInfo, out DateTime dateTimeResult );
        result = (DateTime?)dateTimeResult;
      }
      catch( Exception )
      {
        isValid = false;
      }

      if( !isValid )
      {
        isValid = DateTime.TryParseExact( text, this.GetFormatPatternString(), this.CultureInfo, DateTimeStyles.None, out DateTime dateTimeResult );
        result = (DateTime?)dateTimeResult;
      }

      if( !isValid )
      {
        result = ( m_lastValidDate != null ) ? m_lastValidDate.Value : current;
      }

      return isValid;
    }

#nullable disable

    protected override DateTime? GetTypeDefault()
    {
      return DateTime.Now;
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

    protected override string GetFormatPatternString()
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

    protected override DateTime? UpdateType( DateTime? value, int step )
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

      DateTime? result = null;
      if( value.HasValue )
      {
        try
        {
          switch( info.Type )
          {
            case DateTimePart.Year:
              {
                result = value.Value.AddYears( step );
              }
              break;
            case DateTimePart.Month:
            case DateTimePart.MonthName:
              {
                result = value.Value.AddMonths( step );
              }
              break;
            case DateTimePart.Day:
            case DateTimePart.DayName:
              {
                result = value.Value.AddDays( step );
              }
              break;
            case DateTimePart.Hour12:
            case DateTimePart.Hour24:
              {
                result = value.Value.AddHours( step );
              }
              break;
            case DateTimePart.Minute:
              {
                result = value.Value.AddMinutes( step );
              }
              break;
            case DateTimePart.Second:
              {
                result = value.Value.AddSeconds( step );
              }
              break;
            case DateTimePart.Millisecond:
              {
                result = value.Value.AddMilliseconds( step );
              }
              break;
            case DateTimePart.AmPmDesignator:
              {
                result = value.Value.AddHours( step * 12 );
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
      }

      return this.GetClippedMinMaxValue( result );
    }

    protected override DateTime? GetValue( DateTime? newValue )
    {
      return newValue.Value;
    }

    protected override string GetStringValue( DateTime? newValue, string format, DateTimeFormatInfo dateTimeFormatInfo )
    {
      return newValue.Value.ToString( format, dateTimeFormatInfo );
    }

    protected override void InitSelection()
    {
      if( m_selectedDateTimeInfo == null )
      {
        this.Select( ( this.CurrentDateTimePart != DateTimePart.Other ) ? this.GetDateTimeInfo( this.CurrentDateTimePart ) : this.GetDateTimeInfo( DateTimePart.Day ) );
      }
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
