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
  public abstract class NumericUpDown<T> : UpDownBase<T?> where T : struct, IFormattable
  {
    #region Constructors

    protected NumericUpDown()
    {
      this.SetDefaultValues();
    }

    #endregion

    #region Properties

    #region FormatString

    public static readonly BindableProperty FormatStringProperty = BindableProperty.Create( "FormatString", typeof( string ), typeof( NumericUpDown<T> ), string.Empty, propertyChanged: OnFormatStringChanged );

    public string FormatString
    {
      get => ( string )GetValue( FormatStringProperty );
      set => SetValue( FormatStringProperty, value );
    }

    private static void OnFormatStringChanged( BindableObject bindable, object oldValue, object newValue )
    {
      if( bindable is NumericUpDown<T> numericUpDown )
      {
        numericUpDown.OnFormatStringChanged( ( string )oldValue, ( string )newValue );
      }
    }

    protected virtual void OnFormatStringChanged( string oldValue, string newValue )
    {
      this.SyncTextAndValueProperties( false, null );
    }

    #endregion

    #region Increment

    public static readonly BindableProperty IncrementProperty = BindableProperty.Create( nameof( Increment ), typeof( T ), typeof( NumericUpDown<T> ), default( T ), propertyChanged: OnIncrementChanged, coerceValue: OnCoerceIncrement );

    public T Increment
    {
      get => ( T )GetValue( IncrementProperty );
      set => SetValue( IncrementProperty, value );
    }

    private static void OnIncrementChanged( BindableObject bindable, object oldValue, object newValue )
    {
      if( bindable is NumericUpDown<T> numericUpDown )
      {
        numericUpDown.OnIncrementChanged( ( T )oldValue, ( T )newValue );
      }
    }

    protected virtual void OnIncrementChanged( T oldValue, T newValue )
    {
      if( this.IsLoaded )
      {
        SetValidSpinDirection();
      }
    }

    private static object OnCoerceIncrement( BindableObject bindable, object value )
    {
      if( bindable is NumericUpDown<T> numericUpDown )
      {
        return numericUpDown.OnCoerceIncrement( ( T )value );
      }
      return value;
    }

    protected virtual T? OnCoerceIncrement( T? baseValue )
    {
      return baseValue;
    }

    #endregion

    #endregion

    #region Protected Methods

    protected abstract T GetValueFromDecimal( decimal value );

    protected abstract void SetDefaultValues();

    protected abstract T IncrementValue( T value, T increment );

    protected abstract T DecrementValue( T value, T increment );

    protected abstract bool IsLowerThan( T? value1, T? value2 );

    protected abstract bool IsGreaterThan( T? value1, T? value2 );

    protected abstract bool TryParseText( string text, out T result );

    protected override string ConvertValueToText()
    {
      if( this.Value == null )
        return string.Empty;

      //Manage FormatString of type "{}{0:N2} °" (in xaml) or "{0:N2} °" in code-behind.
      if( ( this.FormatString != null ) && this.FormatString.Contains( "{0" ) )
        return string.Format( this.CultureInfo, this.FormatString, this.Value.Value );

      return this.Value.Value.ToString( this.FormatString, this.CultureInfo );
    }

    protected override T? ConvertTextToValue( string text )
    {
      T? result = null;

      if( String.IsNullOrEmpty( text ) )
        return result;

      // Since the conversion from Value to text using a FormartString may not be parsable,
      // we verify that the already existing text is not the exact same value.
      string currentValueText = this.ConvertValueToText();
      if( object.Equals( currentValueText, text ) )
      {
        return this.Value;
      }

      result = this.ConvertTextToValueCore( currentValueText, text );

      if( this.ClipValueToMinMax )
      {
        return this.CoerceValueMinMax( ( T )result );
      }

      this.ValidateDefaultMinMax( result );

      return result;
    }

    protected override void OnIncrement()
    {
      if( !this.HandleNullSpin() )
      {
        var result = this.IncrementValue( this.Value.Value, this.Increment );
        this.Value = this.CoerceValueMinMax( result );
      }
    }

    protected override void OnDecrement()
    {
      if( !this.HandleNullSpin() )
      {
        var result = this.DecrementValue( this.Value.Value, this.Increment );
        this.Value = this.CoerceValueMinMax( result );
      }
    }

    protected override void SetValidSpinDirection()
    {
      var validDirections = ValidSpinDirections.None;

      if( this.IsLowerThan( this.Value, this.Maximum ) || !this.Value.HasValue || !this.Maximum.HasValue )
      {
        validDirections |= ValidSpinDirections.Increase;
      }

      if( this.IsGreaterThan( this.Value, this.Minimum ) || !this.Value.HasValue || !this.Minimum.HasValue )
      {
        validDirections |= ValidSpinDirections.Decrease;
      }

      this.SetValidSpinDirection( validDirections );
    }

    protected static decimal ParsePercent( string text, IFormatProvider cultureInfo )
    {
      NumberFormatInfo info = NumberFormatInfo.GetInstance( cultureInfo );

      text = text.Replace( info.PercentSymbol, null );

      decimal result = Decimal.Parse( text, NumberStyles.Any, info );
      result /= 100;

      return result;
    }

    protected void TestInputSpecialValue( AllowedSpecialValues allowedValues, AllowedSpecialValues valueToCompare )
    {
      if( ( allowedValues & valueToCompare ) != valueToCompare )
      {
        switch( valueToCompare )
        {
          case AllowedSpecialValues.NaN:
            throw new InvalidDataException( "Value to parse shouldn't be NaN." );
          case AllowedSpecialValues.PositiveInfinity:
            throw new InvalidDataException( "Value to parse shouldn't be Positive Infinity." );
          case AllowedSpecialValues.NegativeInfinity:
            throw new InvalidDataException( "Value to parse shouldn't be Negative Infinity." );
        }
      }
    }

    #endregion

    #region Internal Methods

    internal bool CultureContainsCharacter( char c )
    {
      var info = NumberFormatInfo.GetInstance( this.CultureInfo );
      var charString = c.ToString();

      // Only accept Separators and Negative Sign.
      return info.GetType().GetProperties()
                 .Where( p => p.PropertyType == typeof( string ) && ( p.Name.Contains( "Separator" ) || p.Name.Contains( "NegativeSign" ) ) )
                 .Select( p => ( string )p.GetValue( info, null ) )
                 .Any( value => !string.IsNullOrEmpty( value ) && value == charString );
    }

    internal bool IsBetweenMinMax( T? value )
    {
      return !this.IsLowerThan( value, this.Minimum ) && !this.IsGreaterThan( value, this.Maximum );
    }

    internal bool IsPercent( string stringToTest )
    {
      int PIndex = stringToTest.IndexOf( "P" );
      if( PIndex >= 0 )
      {
        //stringToTest contains a "P" between 2 "'", it's considered as text, not percent
        bool isText = ( stringToTest.Substring( 0, PIndex ).Contains( '\'' )
                      && stringToTest.Substring( PIndex, FormatString.Length - PIndex ).Contains( '\'' ) );

        return !isText;
      }
      return false;
    }

    #endregion

    #region Private Methods

    private T? CoerceValueMinMax( T value )
    {
      if( this.IsLowerThan( value, this.Minimum ) )
        return this.Minimum;
      else if( this.IsGreaterThan( value, this.Maximum ) )
        return this.Maximum;
      else
        return value;
    }

    private bool HandleNullSpin()
    {
      if( !this.Value.HasValue )
      {
        var forcedValue = this.DefaultValue.HasValue ? this.DefaultValue.Value : default;
        this.Value = this.CoerceValueMinMax( forcedValue );

        return true;
      }

      return false;
    }

    private T? ConvertTextToValueCore( string currentValueText, string text )
    {
      T? result;

      if( this.IsPercent( this.FormatString ) )
      {
        result = this.GetValueFromDecimal( ParsePercent( text, this.CultureInfo ) );
      }
      else
      {
        T outputValue = new T();
        // Problem while converting new text
        if( !this.TryParseText( text, out outputValue ) )
        {
          bool shouldThrow = true;

          // case 164198: Throw when replacing only the digit part of 99° through UI.
          // Check if CurrentValueText is also failing => it also contains special characters. ex : 90°
          T currentValueTextOutputValue;
          if( !TryParseText( currentValueText, out currentValueTextOutputValue ) )
          {
            // extract non-digit characters
            var currentValueTextSpecialCharacters = currentValueText.Where( c => !Char.IsDigit( c ) );
            if( currentValueTextSpecialCharacters.Any() )
            {
              var textSpecialCharacters = text.Where( c => !Char.IsDigit( c ) );
              // same non-digit characters on currentValueText and new text => remove them on new Text to parse it again.
              if( currentValueTextSpecialCharacters.Except( textSpecialCharacters ).ToList().Count == 0 )
              {
                var numericValue = new string( text.Where( c => char.IsDigit( c ) || this.CultureContainsCharacter( c ) ).ToArray() );
                decimal number;
                if( Decimal.TryParse( numericValue, NumberStyles.Any, this.CultureInfo, out number ) )
                {
                  foreach( var character in textSpecialCharacters )
                  {
                    if( !this.CultureContainsCharacter( character ) )
                    {
                      text = text.Replace( character.ToString(), string.Empty );
                    }
                  }
                }
                else
                {
                  foreach( var character in textSpecialCharacters )
                  {
                    text = text.Replace( character.ToString(), string.Empty );
                  }
                }

                // if without the special characters, parsing is good, do not throw
                if( this.TryParseText( text, out outputValue ) )
                {
                  shouldThrow = false;
                }
              }
            }
          }

          if( shouldThrow )
          {
            throw new InvalidDataException( "Input string was not in a correct format." );
          }
        }
        result = outputValue;
      }
      return result;
    }

    private void ValidateDefaultMinMax( T? value )
    {
      // DefaultValue is always accepted.
      if( object.Equals( value, this.DefaultValue ) )
        return;

      if( this.IsLowerThan( value, this.Minimum ) )
        throw new ArgumentOutOfRangeException( "Minimum", String.Format( "Value must be greater than MinValue of {0}", this.Minimum ) );
      else if( this.IsGreaterThan( value, this.Maximum ) )
        throw new ArgumentOutOfRangeException( "Maximum", String.Format( "Value must be less than MaxValue of {0}", this.Maximum ) );
    }


    #endregion
  }
}
