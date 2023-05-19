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
  public class SByteUpDown : NumericUpDown<sbyte>
  {
    #region Protected Methods

    protected override void SetDefaultValues()
    {
      this.Increment = ( sbyte )1;
      this.Minimum = sbyte.MinValue;
      this.Maximum = sbyte.MaxValue;
    }

    protected override sbyte IncrementValue( sbyte value, sbyte increment )
    {
      return ( sbyte )( value + increment );
    }

    protected override sbyte DecrementValue( sbyte value, sbyte increment )
    {
      return ( sbyte )( value - increment );
    }

    protected override bool IsLowerThan( sbyte? value1, sbyte? value2 )
    {
      if( ( value1 == null ) || ( value2 == null ) )
        return false;

      return value1 < value2;
    }

    protected override bool IsGreaterThan( sbyte? value1, sbyte? value2 )
    {
      if( ( value1 == null ) || ( value2 == null ) )
        return false;

      return value1 > value2;
    }

    protected override bool TryParseText( string text, out sbyte result )
    {
      return sbyte.TryParse( text, this.CultureInfo, out result );
    }

    protected override sbyte GetValueFromDecimal( decimal value )
    {
      return decimal.ToSByte( value );
    }

    #endregion
  }
}

