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


namespace Xceed.Maui.Toolkit
{
  public class DoubleUpDown : NumericUpDown<double>
  {
    #region Properties

    #region AllowInputSpecialValues

    public static readonly BindableProperty AllowInputSpecialValuesProperty = BindableProperty.Create( nameof( AllowInputSpecialValues ), typeof( AllowedSpecialValues ), typeof( DoubleUpDown ), defaultValue: AllowedSpecialValues.None );

    public AllowedSpecialValues AllowInputSpecialValues
    {
      get
      {
        return ( AllowedSpecialValues )GetValue( AllowInputSpecialValuesProperty );
      }
      set
      {
        SetValue( AllowInputSpecialValuesProperty, value );
      }
    }

    #endregion //AllowInputSpecialValues

    #endregion

    #region Protected Methods

    protected override void SetDefaultValues()
    {
      this.Increment = ( double )1;
      this.Minimum = double.MinValue;
      this.Maximum = double.MaxValue;
    }

    protected override double IncrementValue( double value, double increment )
    {
      return value + increment;
    }

    protected override double DecrementValue( double value, double increment )
    {
      return value - increment;
    }

    protected override bool IsLowerThan( double? value1, double? value2 )
    {
      if( ( value1 == null ) || ( value2 == null ) )
        return false;

      return value1 < value2;
    }

    protected override bool IsGreaterThan( double? value1, double? value2 )
    {
      if( ( value1 == null ) || ( value2 == null ) )
        return false;

      return value1 > value2;
    }

    protected override bool TryParseText( string text, out double result )
    {
      return double.TryParse( text, this.CultureInfo, out result );
    }

    protected override double? OnCoerceIncrement( double? baseValue )
    {
      if( baseValue.HasValue && double.IsNaN( baseValue.Value ) )
      {
        throw new ArgumentException( "NaN is invalid for Increment." );
      }
      return base.OnCoerceIncrement( baseValue );
    }

    protected override double? OnCoerceMaximum( double? baseValue )
    {
      if( baseValue.HasValue && double.IsNaN( baseValue.Value ) )
      {
        throw new ArgumentException( "NaN is invalid for Maximum." );
      }
      return base.OnCoerceMaximum( baseValue );
    }

    protected override double? OnCoerceMinimum( double? baseValue )
    {
      if( baseValue.HasValue && double.IsNaN( baseValue.Value ) )
      {
        throw new ArgumentException( "NaN is invalid for Minimum." );
      }
      return base.OnCoerceMinimum( baseValue );
    }

    protected override double? ConvertTextToValue( string text )
    {
      double? result = base.ConvertTextToValue( text );
      if( result != null )
      {
        if( double.IsNaN( result.Value ) )
        {
          this.TestInputSpecialValue( this.AllowInputSpecialValues, AllowedSpecialValues.NaN );
        }
        else if( double.IsPositiveInfinity( result.Value ) )
        {
          this.TestInputSpecialValue( this.AllowInputSpecialValues, AllowedSpecialValues.PositiveInfinity );
        }
        else if( double.IsNegativeInfinity( result.Value ) )
        {
          this.TestInputSpecialValue( this.AllowInputSpecialValues, AllowedSpecialValues.NegativeInfinity );
        }
      }
      return result;
    }

    protected override void SetValidSpinDirection()
    {
      if( this.Value.HasValue && double.IsInfinity( this.Value.Value ) && ( this.Spinner != null ) )
      {
        this.Spinner.ValidSpinDirections = ValidSpinDirections.None;
      }
      else
      {
        base.SetValidSpinDirection();
      }
    }

    protected override double GetValueFromDecimal( decimal value )
    {
      return decimal.ToDouble( value );
    }

    #endregion
  }
}
