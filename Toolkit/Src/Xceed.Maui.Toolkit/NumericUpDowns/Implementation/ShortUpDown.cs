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
  public class ShortUpDown : NumericUpDown<short>
  {
    #region Protected Methods

    protected override void SetDefaultValues()
    {
      this.Increment = ( short )1;
      this.Minimum = short.MinValue;
      this.Maximum = short.MaxValue;
    }

    protected override short IncrementValue( short value, short increment )
    {
      return ( short )( value + increment );
    }

    protected override short DecrementValue( short value, short increment )
    {
      return ( short )( value - increment );
    }

    protected override bool IsLowerThan( short? value1, short? value2 )
    {
      if( ( value1 == null ) || ( value2 == null ) )
        return false;

      return value1 < value2;
    }

    protected override bool IsGreaterThan( short? value1, short? value2 )
    {
      if( ( value1 == null ) || ( value2 == null ) )
        return false;

      return value1 > value2;
    }

    protected override bool TryParseText( string text, out short result )
    {
      return Int16.TryParse( text, this.CultureInfo, out result );
    }

    protected override short GetValueFromDecimal( decimal value )
    {
      return decimal.ToInt16( value );
    }

    #endregion
  }
}
