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
  public class LongUpDown : NumericUpDown<long>
  {
    #region Protected Methods

    protected override void SetDefaultValues()
    {
      this.Increment = ( long )1;
      this.Minimum = long.MinValue;
      this.Maximum = long.MaxValue;
    }

    protected override long IncrementValue( long value, long increment )
    {
      return value + increment;
    }

    protected override long DecrementValue( long value, long increment )
    {
      return value - increment;
    }

    protected override bool IsLowerThan( long? value1, long? value2 )
    {
      if( ( value1 == null ) || ( value2 == null ) )
        return false;

      return value1 < value2;
    }

    protected override bool IsGreaterThan( long? value1, long? value2 )
    {
      if( ( value1 == null ) || ( value2 == null ) )
        return false;

      return value1 > value2;
    }

    protected override bool TryParseText( string text, out long result )
    {
      return Int64.TryParse( text, this.CultureInfo, out result );
    }

    protected override long GetValueFromDecimal( decimal value )
    {
      return decimal.ToInt64( value );
    }

    #endregion
  }
}
