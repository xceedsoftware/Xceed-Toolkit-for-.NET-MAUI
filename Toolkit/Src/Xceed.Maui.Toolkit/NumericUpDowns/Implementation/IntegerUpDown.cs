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
  public class IntegerUpDown : NumericUpDown<int>
  {
    #region Protected Methods

    protected override void SetDefaultValues()
    {
      this.Increment = 1;
      this.Minimum = int.MinValue;
      this.Maximum = int.MaxValue;
    }

    protected override int IncrementValue( int value, int increment )
    {
      return value + increment;
    }

    protected override int DecrementValue( int value, int increment )
    {
      return value - increment;
    }

    protected override bool IsLowerThan( int? value1, int? value2 )
    {
      if( ( value1 == null ) || ( value2 == null ) )
        return false;

      return value1 < value2;
    }

    protected override bool IsGreaterThan( int? value1, int? value2 )
    {
      if( ( value1 == null ) || ( value2 == null ) )
        return false;

      return value1 > value2;
    }

    protected override bool TryParseText( string text, out int result )
    {
      return Int32.TryParse( text, this.CultureInfo, out result );
    }

    protected override int GetValueFromDecimal( decimal value )
    {
      return decimal.ToInt32( value );
    }

    #endregion
  }
}
