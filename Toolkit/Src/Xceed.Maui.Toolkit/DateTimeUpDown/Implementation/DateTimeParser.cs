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
  internal static class DateTimeParser
  {
    internal static bool TryParse( string value, string formatPattern, DateTime currentDate, CultureInfo cultureInfo, out DateTime result )
    {
      var success = false;
      result = currentDate;

      if( string.IsNullOrEmpty( value ) || string.IsNullOrEmpty( formatPattern ) )
        return false;

      DateTimeParser.UpdateValueFormatForQuotes( ref value, ref formatPattern );

      var dateTimeString = ComputeDateTimeString( value, formatPattern, currentDate, cultureInfo ).Trim();

      if( !String.IsNullOrEmpty( dateTimeString ) )
      {
        success = DateTime.TryParse( dateTimeString, cultureInfo.DateTimeFormat, DateTimeStyles.None, out result );
      }

      if( !success )
      {
        result = currentDate;
      }

      return success;
    }

    private static void UpdateValueFormatForQuotes( ref string value, ref string formatPattern )
    {
      var quoteStart = formatPattern.IndexOf( "'" );
      if( quoteStart > -1 )
      {
        var quoteEnd = formatPattern.IndexOf( "'", quoteStart + 1 );
        if( quoteEnd > -1 )
        {
          var quoteContent = formatPattern.Substring( quoteStart + 1, quoteEnd - quoteStart - 1 );
          value = value.Replace( quoteContent, "" );
          formatPattern = formatPattern.Remove( quoteStart, quoteEnd - quoteStart + 1 );

          // Use recursive calls for many quote text. 
          DateTimeParser.UpdateValueFormatForQuotes( ref value, ref formatPattern );
        }
      }
    }

    private static string ComputeDateTimeString( string dateTime, string formatPattern, DateTime currentDate, CultureInfo cultureInfo )
    {
      var dateParts = GetDateParts( currentDate, cultureInfo );
      var timeParts = new string[ 3 ] { currentDate.Hour.ToString(), currentDate.Minute.ToString(), currentDate.Second.ToString() };
      var millisecondsPart = currentDate.Millisecond.ToString();
      var designator = "";
      var dateTimeSeparators = new string[] { ",", " ", "-", ".", "/", cultureInfo.DateTimeFormat.DateSeparator, cultureInfo.DateTimeFormat.TimeSeparator };
      var forcePMDesignator = false;

      DateTimeParser.UpdateSortableDateTimeString( ref dateTime, ref formatPattern, cultureInfo );

      var dateTimeParts = new List<string>();
      var formats = new List<string>();
      var isContainingDateTimeSeparators = dateTimeSeparators.Any( s => dateTime.Contains( s ) );
      if( isContainingDateTimeSeparators )
      {
        dateTimeParts = dateTime.Split( dateTimeSeparators, StringSplitOptions.RemoveEmptyEntries ).ToList();
        formats = formatPattern.Split( dateTimeSeparators, StringSplitOptions.RemoveEmptyEntries ).ToList();
      }
      else
      {
        var currentformat = "";
        var currentString = "";
        var formatArray = formatPattern.ToCharArray();
        for( int i = 0; i < formatArray.Count(); ++i )
        {
          var c = formatArray[ i ];
          if( !currentformat.Contains( c ) )
          {
            if( !string.IsNullOrEmpty( currentformat ) )
            {
              formats.Add( currentformat );
              dateTimeParts.Add( currentString );
            }
            currentformat = c.ToString();
            currentString = ( i < dateTime.Length ) ? dateTime[ i ].ToString() : "";
          }
          else
          {
            currentformat = string.Concat( currentformat, c );
            currentString = string.Concat( currentString, ( i < dateTime.Length ) ? dateTime[ i ] : '\0' );
          }
        }
        if( !string.IsNullOrEmpty( currentformat ) )
        {
          formats.Add( currentformat );
          dateTimeParts.Add( currentString );
        }
      }

      //Auto-complete missing date parts
      if( dateTimeParts.Count < formats.Count )
      {
        while( dateTimeParts.Count != formats.Count )
        {
          dateTimeParts.Add( "0" );
        }
      }

      //something went wrong
      if( dateTimeParts.Count != formats.Count )
        return string.Empty;

      for( int i = 0; i < formats.Count; i++ )
      {
        var f = formats[ i ];
        if( !f.Contains( "ddd" ) && !f.Contains( "GMT" ) )
        {
          if( f.Contains( "M" ) )
          {
            dateParts[ "Month" ] = dateTimeParts[ i ];
          }
          else if( f.Contains( "d" ) )
          {
            dateParts[ "Day" ] = dateTimeParts[ i ];
          }
          else if( f.Contains( "y" ) )
          {
            dateParts[ "Year" ] = dateTimeParts[ i ] != "0" ? dateTimeParts[ i ] : "0000";
            if( dateParts[ "Year" ].Length == 2 )
            {
              var yearDigits = int.Parse( dateParts[ "Year" ] );
              var twoDigitYearMax = cultureInfo.Calendar.TwoDigitYearMax;
              var hundredDigits = ( yearDigits <= twoDigitYearMax % 100 ) ? twoDigitYearMax / 100 : ( twoDigitYearMax / 100 ) - 1;

              dateParts[ "Year" ] = string.Format( "{0}{1}", hundredDigits, dateParts[ "Year" ] );
            }
          }
          else if( f.Contains( "hh" ) || f.Contains( "HH" ) )
          {
            timeParts[ 0 ] = dateTimeParts[ i ];
          }
          else if( f.Contains( "h" ) || f.Contains( "H" ) )
          {
            timeParts[ 0 ] = dateTimeParts[ i ];
          }
          else if( f.Contains( "m" ) )
          {
            timeParts[ 1 ] = dateTimeParts[ i ];
          }
          else if( f.Contains( "s" ) )
          {
            timeParts[ 2 ] = dateTimeParts[ i ];
          }
          else if( f.Contains( "f" ) )
          {
            millisecondsPart = dateTimeParts[ i ];
          }
          else if( f.Contains( "t" ) )
          {
            designator = forcePMDesignator ? "PM" : dateTimeParts[ i ];
          }
        }
      }

      var date = string.Join( cultureInfo.DateTimeFormat.DateSeparator, dateParts.Select( x => x.Value ).ToArray() );
      var time = string.Join( cultureInfo.DateTimeFormat.TimeSeparator, timeParts );
      time += "." + millisecondsPart; 

      return String.Format( "{0} {1} {2}", date, time, designator );
    }

    private static Dictionary<string, string> GetDateParts( DateTime currentDate, CultureInfo cultureInfo )
    {
      var dateParts = new Dictionary<string, string>();
      var dateTimeSeparators = new[] { ",", " ", "-", ".", "/", cultureInfo.DateTimeFormat.DateSeparator, cultureInfo.DateTimeFormat.TimeSeparator };
      var dateFormatParts = cultureInfo.DateTimeFormat.ShortDatePattern.Split( dateTimeSeparators, StringSplitOptions.RemoveEmptyEntries ).ToList();
      dateFormatParts.ForEach( item =>
      {
        var key = string.Empty;
        var value = string.Empty;

        if( item.Contains( "M" ) )
        {
          key = "Month";
          value = currentDate.Month.ToString();
        }
        else if( item.Contains( "d" ) )
        {
          key = "Day";
          value = currentDate.Day.ToString();
        }
        else if( item.Contains( "y" ) )
        {
          key = "Year";
          value = currentDate.Year.ToString( "D4" );
        }
        if( !dateParts.ContainsKey( key ) )
        {
          dateParts.Add( key, value );
        }
      } );
      return dateParts;
    }

    private static void UpdateSortableDateTimeString( ref string dateTime, ref string formatPattern, CultureInfo cultureInfo )
    {
      if( formatPattern == cultureInfo.DateTimeFormat.SortableDateTimePattern )
      {
        formatPattern = formatPattern.Replace( "'", "" ).Replace( "T", " " );
        dateTime = dateTime.Replace( "'", "" ).Replace( "T", " " );
      }
      else if( formatPattern == cultureInfo.DateTimeFormat.UniversalSortableDateTimePattern )
      {
        formatPattern = formatPattern.Replace( "'", "" ).Replace( "Z", "" );
        dateTime = dateTime.Replace( "'", "" ).Replace( "Z", "" );
      }
    }
  }
}
