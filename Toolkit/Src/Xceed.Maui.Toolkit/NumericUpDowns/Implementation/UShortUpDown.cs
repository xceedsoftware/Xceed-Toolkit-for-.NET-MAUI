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
  public class UShortUpDown : NumericUpDown<ushort>
  {
    #region Protected Methods

    protected override void SetDefaultValues()
    {
      this.Increment = ( ushort )1;
      this.Minimum = ushort.MinValue;
      this.Maximum = ushort.MaxValue;
    }

    protected override ushort IncrementValue( ushort value, ushort increment )
    {
      return ( ushort )( value + increment );
    }

    protected override ushort DecrementValue( ushort value, ushort increment )
    {
      return ( ushort )( value - increment );
    }

    protected override bool IsLowerThan( ushort? value1, ushort? value2 )
    {
      if( ( value1 == null ) || ( value2 == null ) )
        return false;

      return value1 < value2;
    }

    protected override bool IsGreaterThan( ushort? value1, ushort? value2 )
    {
      if( ( value1 == null ) || ( value2 == null ) )
        return false;

      return value1 > value2;
    }

    protected override bool TryParseText( string text, out ushort result )
    {
      return ushort.TryParse( text, this.CultureInfo, out result );
    }

    protected override ushort GetValueFromDecimal( decimal value )
    {
      return decimal.ToUInt16( value );
    }

    #endregion
  }
}
