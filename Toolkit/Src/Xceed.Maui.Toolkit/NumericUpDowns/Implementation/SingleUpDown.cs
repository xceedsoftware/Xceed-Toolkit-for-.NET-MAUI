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
  public class SingleUpDown : NumericUpDown<float>
  {
    #region Properties

    #region AllowInputSpecialValues

    public static readonly BindableProperty AllowInputSpecialValuesProperty = BindableProperty.Create( nameof( AllowInputSpecialValues ), typeof( AllowedSpecialValues ), typeof( SingleUpDown ), defaultValue: AllowedSpecialValues.None );

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
      this.Increment = ( float )1;
      this.Minimum = float.MinValue;
      this.Maximum = float.MaxValue;
    }

    protected override float IncrementValue( float value, float increment )
    {
      return value + increment;
    }

    protected override float DecrementValue( float value, float increment )
    {
      return value - increment;
    }

    protected override bool IsLowerThan( float? value1, float? value2 )
    {
      if( ( value1 == null ) || ( value2 == null ) )
        return false;
      return value1 < value2;
    }

    protected override bool IsGreaterThan( float? value1, float? value2 )
    {
      if( ( value1 == null ) || ( value2 == null ) )
        return false;

      return value1 > value2;
    }

    protected override bool TryParseText( string text, out float result )
    {
      return Single.TryParse( text, this.CultureInfo, out result );
    }

    protected override float? OnCoerceIncrement( float? baseValue )
    {
      if( baseValue.HasValue && float.IsNaN( baseValue.Value ) )
      {
        throw new ArgumentException( "NaN is invalid for Increment." );
      }
      return base.OnCoerceIncrement( baseValue );
    }

    protected override float? OnCoerceMaximum( float? baseValue )
    {
      if( baseValue.HasValue && float.IsNaN( baseValue.Value ) )
      {
        throw new ArgumentException( "NaN is invalid for Maximum." );
      }
      return base.OnCoerceMaximum( baseValue );
    }

    protected override float? OnCoerceMinimum( float? baseValue )
    {
      if( baseValue.HasValue && float.IsNaN( baseValue.Value ) )
      {
        throw new ArgumentException( "NaN is invalid for Minimum." );
      }
      return base.OnCoerceMinimum( baseValue );
    }

    protected override void SetValidSpinDirection()
    {
      if( this.Value.HasValue && float.IsInfinity( this.Value.Value ) && ( this.Spinner != null ) )
      {
        this.Spinner.ValidSpinDirections = ValidSpinDirections.None;
      }
      else
      {
        base.SetValidSpinDirection();
      }
    }

    protected override float? ConvertTextToValue( string text )
    {
      float? result = base.ConvertTextToValue( text );

      if( result != null )
      {
        if( float.IsNaN( result.Value ) )
        {
          this.TestInputSpecialValue( this.AllowInputSpecialValues, AllowedSpecialValues.NaN );
        }
        else if( float.IsPositiveInfinity( result.Value ) )
        {
          this.TestInputSpecialValue( this.AllowInputSpecialValues, AllowedSpecialValues.PositiveInfinity );
        }
        else if( float.IsNegativeInfinity( result.Value ) )
        {
          this.TestInputSpecialValue( this.AllowInputSpecialValues, AllowedSpecialValues.NegativeInfinity );
        }
      }

      return result;
    }

    protected override float GetValueFromDecimal( decimal value )
    {
      return decimal.ToSingle( value );
    }

    #endregion
  }
}
