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
  public class UIntegerUpDown : NumericUpDown<uint>
  {
    #region Protected Methods

    protected override void SetDefaultValues()
    {
      this.Increment = ( uint )1;
      this.Minimum = uint.MinValue;
      this.Maximum = uint.MaxValue;
    }

    protected override uint IncrementValue( uint value, uint increment )
    {
      return value + increment;
    }

    protected override uint DecrementValue( uint value, uint increment )
    {
      return value - increment;
    }

    protected override bool IsLowerThan( uint? value1, uint? value2 )
    {
      if( ( value1 == null ) || ( value2 == null ) )
        return false;

      return value1 < value2;
    }

    protected override bool IsGreaterThan( uint? value1, uint? value2 )
    {
      if( ( value1 == null ) || ( value2 == null ) )
        return false;

      return value1 > value2;
    }

    protected override bool TryParseText( string text, out uint result )
    {
      return uint.TryParse( text, this.CultureInfo, out result );
    }

    protected override uint GetValueFromDecimal( decimal value )
    {
      return decimal.ToUInt32( value );
    }

    #endregion
  }
}

