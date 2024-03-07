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
  static class CalculatorUtilities
  {
    public static string GetCalculatorButtonContent( Calculator.CalculatorButtonType type )
    {
      string content = string.Empty;
      switch( type )
      {
        case Calculator.CalculatorButtonType.Add:
          content = "+";
          break;
        case Calculator.CalculatorButtonType.Back:
          content = "Back";
          break;
        case Calculator.CalculatorButtonType.Cancel:
          content = "CE";
          break;
        case Calculator.CalculatorButtonType.Clear:
          content = "C";
          break;
        case Calculator.CalculatorButtonType.Decimal:
          content = CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator;
          break;
        case Calculator.CalculatorButtonType.Divide:
          content = "/";
          break;
        case Calculator.CalculatorButtonType.Eight:
          content = "8";
          break;
        case Calculator.CalculatorButtonType.Equal:
          content = "=";
          break;
        case Calculator.CalculatorButtonType.Five:
          content = "5";
          break;
        case Calculator.CalculatorButtonType.Four:
          content = "4";
          break;
        case Calculator.CalculatorButtonType.Fraction:
          content = "1/x";
          break;
        case Calculator.CalculatorButtonType.MAdd:
          content = "M+";
          break;
        case Calculator.CalculatorButtonType.MC:
          content = "MC";
          break;
        case Calculator.CalculatorButtonType.MR:
          content = "MR";
          break;
        case Calculator.CalculatorButtonType.MS:
          content = "MS";
          break;
        case Calculator.CalculatorButtonType.MSub:
          content = "M-";
          break;
        case Calculator.CalculatorButtonType.Multiply:
          content = "*";
          break;
        case Calculator.CalculatorButtonType.Nine:
          content = "9";
          break;
        case Calculator.CalculatorButtonType.None:
          break;
        case Calculator.CalculatorButtonType.One:
          content = "1";
          break;
        case Calculator.CalculatorButtonType.Percent:
          content = "%";
          break;
        case Calculator.CalculatorButtonType.Seven:
          content = "7";
          break;
        case Calculator.CalculatorButtonType.Negate:
          content = "+/-";
          break;
        case Calculator.CalculatorButtonType.Six:
          content = "6";
          break;
        case Calculator.CalculatorButtonType.Sqrt:
          content = "Sqrt";
          break;
        case Calculator.CalculatorButtonType.Subtract:
          content = "-";
          break;
        case Calculator.CalculatorButtonType.Three:
          content = "3";
          break;
        case Calculator.CalculatorButtonType.Two:
          content = "2";
          break;
       default:
          content = "0";
          break;
      }
      return content;
    }

    public static bool IsDigit( Calculator.CalculatorButtonType buttonType )
    {
      switch( buttonType )
      {
        case Calculator.CalculatorButtonType.Zero:
        case Calculator.CalculatorButtonType.One:
        case Calculator.CalculatorButtonType.Two:
        case Calculator.CalculatorButtonType.Three:
        case Calculator.CalculatorButtonType.Four:
        case Calculator.CalculatorButtonType.Five:
        case Calculator.CalculatorButtonType.Six:
        case Calculator.CalculatorButtonType.Seven:
        case Calculator.CalculatorButtonType.Eight:
        case Calculator.CalculatorButtonType.Nine:
        case Calculator.CalculatorButtonType.Decimal:
          return true;
        default:
          return false;
      }
    }

    public static bool IsMemory( Calculator.CalculatorButtonType buttonType )
    {
      switch( buttonType )
      {
        case Calculator.CalculatorButtonType.MAdd:
        case Calculator.CalculatorButtonType.MC:
        case Calculator.CalculatorButtonType.MR:
        case Calculator.CalculatorButtonType.MS:
        case Calculator.CalculatorButtonType.MSub:
          return true;
        default:
          return false;
      }
    }

    public static decimal ParseDecimal( string text )
    {
      decimal result;
      var success = Decimal.TryParse( text, NumberStyles.Any, CultureInfo.CurrentCulture, out result );
      return success ? result : decimal.Zero;
    }

    public static decimal Add( decimal firstNumber, decimal secondNumber )
    {
      return firstNumber + secondNumber;
    }

    public static decimal Subtract( decimal firstNumber, decimal secondNumber )
    {
      return firstNumber - secondNumber;
    }

    public static decimal Multiply( decimal firstNumber, decimal secondNumber )
    {
      return firstNumber * secondNumber;
    }

    public static decimal Divide( decimal firstNumber, decimal secondNumber )
    {
      return firstNumber / secondNumber;
    }

    public static decimal Percent( decimal firstNumber, decimal secondNumber )
    {
      return firstNumber * secondNumber / 100M;
    }

    public static decimal SquareRoot( decimal operand )
    {
      return Convert.ToDecimal( Math.Sqrt( Convert.ToDouble( operand ) ) );
    }

    public static decimal Fraction( decimal operand )
    {
      return 1 / operand;
    }

    public static decimal Negate( decimal operand )
    {
      return operand * -1M;
    }
  }
}
