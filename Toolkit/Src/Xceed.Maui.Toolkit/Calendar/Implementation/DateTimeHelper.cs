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


// This file is inspired from Microsoft under the MIT License.
using System.Globalization;

namespace Xceed.Maui.Toolkit
{
  internal static class DateTimeHelper
  {
    #region Internal Methods
    internal static DateOnly? AddDays( DateOnly date, int days )
    {
      try
      {
        return date.AddDays( days );
      }
      catch( ArgumentException )
      {
        return null;
      }
    }

    internal static DateOnly? AddMonths( DateOnly date, int months )
    {
      try
      {
        return date.AddMonths( months );
      }
      catch( ArgumentException )
      {
        return null;
      }
    }

    internal static DateOnly? AddYears( DateOnly date, int years )
    {
      try
      {
        return date.AddYears( years );
      }
      catch( ArgumentException )
      {
        return null;
      }
    }

    internal static DateOnly? SetYearMonth( DateOnly date, DateOnly yearMonth )
    {
      var result = DateTimeHelper.AddYears( date, yearMonth.Year - date.Year );
      if( result.HasValue )
      {
        result = DateTimeHelper.AddMonths( result.Value, yearMonth.Month - date.Month );
      }

      return result;
    }

    internal static int CompareYearAndMonth( DateOnly dt1, DateOnly dt2 )
    {
      return ( dt1.Year - dt2.Year ) * 12 + ( dt1.Month - dt2.Month );
    }

    internal static string ToDecadeRangeString( int decade, VisualElement visualElement )
    {
      string result = string.Empty;
      var dateFormat = DateTimeHelper.GetDateFormat( CultureInfo.CurrentCulture );
      if( dateFormat != null )
      {
        var flag = visualElement.FlowDirection == Microsoft.Maui.FlowDirection.RightToLeft;
        result = string.Concat( str2: ( flag ? decade : ( decade + 9 ) ).ToString( dateFormat ), str0: ( flag ? ( decade + 9 ) : decade ).ToString( dateFormat ), str1: "-" );
      }

      return result;
    }

    internal static string ToYearMonthPatternString( DateOnly? date, CultureInfo culture )
    {
      var result = string.Empty;
      var dateFormat = DateTimeHelper.GetDateFormat( culture );
      if( date.HasValue && dateFormat != null )
      {
        result = date.Value.ToString( dateFormat.YearMonthPattern, dateFormat );
      }

      return result;
    }

    internal static string ToYearString( DateOnly? date, CultureInfo culture )
    {
      var result = string.Empty;
      var dateFormat = DateTimeHelper.GetDateFormat( culture );
      if( date.HasValue && dateFormat != null )
        return date.Value.Year.ToString( dateFormat );

      return result;
    }

    internal static DateTimeFormatInfo GetDateFormat( CultureInfo culture )
    {
      if( culture.Calendar is GregorianCalendar )
        return culture.DateTimeFormat;

      GregorianCalendar gregorianCalendar = null;
      DateTimeFormatInfo dateTimeFormatInfo = null;
      var optionalCalendars = culture.OptionalCalendars;
      foreach( System.Globalization.Calendar calendar in optionalCalendars )
      {
        if( calendar is GregorianCalendar gCalendar )
        {
          if( gregorianCalendar == null )
          {
            gregorianCalendar = calendar as GregorianCalendar;
          }

          if( gCalendar.CalendarType == GregorianCalendarTypes.Localized )
          {
            gregorianCalendar = gCalendar;
            break;
          }
        }
      }

      if( gregorianCalendar == null )
      {
        dateTimeFormatInfo = ( ( CultureInfo )CultureInfo.InvariantCulture.Clone() ).DateTimeFormat;
        dateTimeFormatInfo.Calendar = new GregorianCalendar();
      }
      else
      {
        dateTimeFormatInfo = ( ( CultureInfo )culture.Clone() ).DateTimeFormat;
        dateTimeFormatInfo.Calendar = gregorianCalendar;
      }

      return dateTimeFormatInfo;
    }

    #endregion
  }
}
