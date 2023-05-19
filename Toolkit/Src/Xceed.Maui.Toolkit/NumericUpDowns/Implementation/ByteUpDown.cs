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
  public class ByteUpDown : NumericUpDown<byte>
  {
    #region Protected Methods

    protected override void SetDefaultValues()
    {
      this.Increment = ( byte )1;
      this.Minimum = byte.MinValue;
      this.Maximum = byte.MaxValue;
    }

    protected override byte IncrementValue( byte value, byte increment )
    {
      return ( byte )( value + increment );
    }

    protected override byte DecrementValue( byte value, byte increment )
    {
      return ( byte )( value - increment );
    }

    protected override bool IsLowerThan( byte? value1, byte? value2 )
    {
      if( ( value1 == null ) || ( value2 == null ) )
        return false;

      return value1 < value2;
    }

    protected override bool IsGreaterThan( byte? value1, byte? value2 )
    {
      if( ( value1 == null ) || ( value2 == null ) )
        return false;

      return value1 > value2;
    }

    protected override bool TryParseText( string text, out byte result )
    {
      return Byte.TryParse( text, this.CultureInfo, out result );
    }

    protected override byte GetValueFromDecimal( decimal value )
    {
      return decimal.ToByte( value );
    }
    #endregion
  }
}
