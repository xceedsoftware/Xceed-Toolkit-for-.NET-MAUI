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


using System.Windows.Input;

namespace Xceed.Maui.Toolkit
{
  public class Calculator : ContentControl
  {
    #region Members

    private bool m_showNewNumber = true;
    private decimal m_previousValue;
    private Operation m_lastOperation = Operation.None;
    private TextBox m_displayTextControl;

    #endregion //Members

    #region Enumerations

    public enum CalculatorButtonType
    {
      Add,
      Back,
      Cancel,
      Clear,
      Decimal,
      Divide,
      Eight,
      Equal,
      Five,
      Four,
      Fraction,
      MAdd,
      MC,
      MR,
      MS,
      MSub,
      Multiply,
      Negate,
      Nine,
      None,
      One,
      Percent,
      Seven,
      Six,
      Sqrt,
      Subtract,
      Three,
      Two,
      Zero
    }

    public enum Operation
    {
      Add,
      Subtract,
      Divide,
      Multiply,
      Percent,
      Sqrt,
      Fraction,
      None,
      Clear,
      Negate
    }

    #endregion //Enumerations

    #region Constructors

    public Calculator()
    {
      this.ButtonCommand = new Command<string>(
      execute: ( string arg ) =>
      {
        if( this.IsEnabled )
        {
          this.Focus();
          if( Enum.TryParse( arg.ToString(), out CalculatorButtonType buttonType ) )
          {
            this.ProcessCalculatorButton( buttonType );
          }
          else
          {
            throw new ArgumentException( "Type parameter must be a CalculatorButtonType" );
          }
        }
      } );
    }

    #endregion //Constructors

    #region Properties

    #region ButtonCommand

    public static readonly BindableProperty ButtonCommandProperty = BindableProperty.Create( nameof( ButtonCommand ), typeof( ICommand ), typeof( Calculator ) );
    public ICommand ButtonCommand
    {
      get => (ICommand)GetValue( ButtonCommandProperty );
      internal set => SetValue( ButtonCommandProperty, value );
    }

    #endregion

    #region CalculatorButtonPanelTemplate

    public static readonly BindableProperty CalculatorButtonPanelTemplateProperty = BindableProperty.Create( nameof( CalculatorButtonPanelTemplate ), typeof( DataTemplate ), typeof( Calculator ) );
    public DataTemplate CalculatorButtonPanelTemplate
    {
      get
      {
        return ( DataTemplate )GetValue( CalculatorButtonPanelTemplateProperty );
      }
      set
      {
        SetValue( CalculatorButtonPanelTemplateProperty, value );
      }
    }

    #endregion //CalculatorButtonPanelTemplate

    #region ClearButtonStyle

    public static readonly BindableProperty ClearButtonStyleProperty = BindableProperty.Create( nameof( ClearButtonStyle ), typeof( Style ), typeof( Calculator ) );
    public Style ClearButtonStyle
    {
      get => (Style)GetValue( ClearButtonStyleProperty );
      set => SetValue( ClearButtonStyleProperty, value );
    }

    #endregion

    #region DigitButtonStyle

    public static readonly BindableProperty DigitButtonStyleProperty = BindableProperty.Create( nameof( DigitButtonStyle ), typeof( Style ), typeof( Calculator ) );
    public Style DigitButtonStyle
    {
      get => (Style)GetValue( DigitButtonStyleProperty );
      set => SetValue( DigitButtonStyleProperty, value );
    }

    #endregion

    #region DisplayText

    public static readonly BindableProperty DisplayTextProperty = BindableProperty.Create( nameof( DisplayText ), typeof( string ), typeof( Calculator ), defaultValue: "0" );

    public string DisplayText
    {
      get
      {
        return ( string )GetValue( DisplayTextProperty );
      }
      internal set
      {
        SetValue( DisplayTextProperty, value );
      }
    }

    #endregion //DisplayText

    #region DisplayTextStyle

    public static readonly BindableProperty DisplayTextStyleProperty = BindableProperty.Create( nameof( DisplayTextStyle ), typeof( Style ), typeof( Calculator ) );
    public Style DisplayTextStyle
    {
      get => (Style)GetValue( DisplayTextStyleProperty );
      set => SetValue( DisplayTextStyleProperty, value );
    }

    #endregion

    #region Memory

    public static readonly BindableProperty MemoryProperty = BindableProperty.Create( nameof( Memory ), typeof( decimal ), typeof( Calculator ) );
    public decimal Memory
    {
      get
      {
        return ( decimal )GetValue( MemoryProperty );
      }
      set
      {
        SetValue( MemoryProperty, value );
      }
    }

    #endregion //Memory

    #region OperationButtonStyle

    public static readonly BindableProperty OperationButtonStyleProperty = BindableProperty.Create( nameof( OperationButtonStyle ), typeof( Style ), typeof( Calculator ) );
    public Style OperationButtonStyle
    {
      get => (Style)GetValue( OperationButtonStyleProperty );
      set => SetValue( OperationButtonStyleProperty, value );
    }

    #endregion

    #region Precision

    public static readonly BindableProperty PrecisionProperty = BindableProperty.Create( nameof( Precision ), typeof( uint ), typeof( Calculator ), defaultValue: ( uint )6 );
    public uint Precision
    {
      get
      {
        return ( uint )GetValue( PrecisionProperty );
      }
      set
      {
        SetValue( PrecisionProperty, value );
      }
    }

    #endregion //Precision

    #region Value

    public static readonly BindableProperty ValueProperty = BindableProperty.Create( nameof( Value ), typeof( decimal? ), typeof( Calculator ), defaultValue: default( decimal ), defaultBindingMode: BindingMode.TwoWay, propertyChanged: OnValueChanged );
    public decimal? Value
    {
      get
      {
        return ( decimal? )GetValue( ValueProperty );
      }
      set
      {
        SetValue( ValueProperty, value );
      }
    }

    private static void OnValueChanged( BindableObject bindable, object oldValue, object newValue )
    {
      if( bindable is Calculator calculator )
      {
        calculator.OnValueChanged( ( decimal? )oldValue, ( decimal? )newValue );
      }
    }

    protected virtual void OnValueChanged( decimal? oldValue, decimal? newValue )
    {
      this.SetDisplayText( newValue );
      this.RaiseValueChangedEvent( this, new ValueChangedEventArgs<decimal?>( oldValue, newValue ) );
    }

    #endregion //Value

    #endregion //Properties

    #region Protected Methods

    protected override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      m_displayTextControl = this.GetTemplateChild( "PART_DisplayTextControl" ) as TextBox;
    }

    #endregion

    #region Internal Methods

    internal TextBox GetDisplayTextControl()
    {
      return m_displayTextControl;
    }

    internal void ProcessCalculatorButton( CalculatorButtonType buttonType )
    {
      if( this.IsEnabled )
      {
        if( CalculatorUtilities.IsDigit( buttonType ) )
        {
          this.ProcessDigitKey( buttonType );
        }
        else
        {
          if( CalculatorUtilities.IsMemory( buttonType ) )
          {
            this.ProcessMemoryKey( buttonType );
          }
          else
          {
            this.ProcessOperationKey( buttonType );
          }
        }
      }
    }

    #endregion

    #region Private Methods

    private void Calculate()
    {
      if( m_lastOperation == Operation.None )
        return;
      try
      {
        this.Value = Decimal.Round( this.CalculateValue( m_lastOperation ), Convert.ToInt32( this.Precision ) );
        this.SetDisplayText( this.Value ); //Set DisplayText even when Value doesn't change
      }
      catch
      {
        this.Value = null;
        this.DisplayText = "ERROR";
      }
    }

    private void SetDisplayText( decimal? newValue )
    {
      if( newValue.HasValue && ( newValue.Value != 0 ) )
      {
        this.DisplayText = newValue.ToString();
      }
      else
      {
        this.DisplayText = "0";
      }
    }

    private void Calculate( Operation newOperation )
    {
      if( !m_showNewNumber )
      {
        this.Calculate();
      }
      m_lastOperation = newOperation;
    }

    private void Calculate( Operation currentOperation, Operation newOperation )
    {
      m_lastOperation = currentOperation;
      this.Calculate();
      m_lastOperation = newOperation;
    }

    private decimal CalculateValue( Operation operation )
    {
      decimal newValue = decimal.Zero;
      decimal currentValue = CalculatorUtilities.ParseDecimal( this.DisplayText );
      switch( operation )
      {
        case Operation.Add:
          newValue = CalculatorUtilities.Add( m_previousValue, currentValue );
          break;
        case Operation.Subtract:
          newValue = CalculatorUtilities.Subtract( m_previousValue, currentValue );
          break;
        case Operation.Multiply:
          newValue = CalculatorUtilities.Multiply( m_previousValue, currentValue );
          break;
        case Operation.Divide:
          newValue = CalculatorUtilities.Divide( m_previousValue, currentValue );
          break;
        case Operation.Sqrt:
          newValue = CalculatorUtilities.SquareRoot( currentValue );
          break;
        case Operation.Fraction:
          newValue = CalculatorUtilities.Fraction( currentValue );
          break;
        case Operation.Negate:
          newValue = CalculatorUtilities.Negate( currentValue );
          break;
        default:
          newValue = decimal.Zero;
          break;
      }

      return newValue;
    }

    private void ProcessBackKey()
    {
      string displayText;
      if( this.DisplayText.Length > 1 && !( this.DisplayText.Length == 2 && this.DisplayText[ 0 ] == '-' ) )
      {
        displayText = this.DisplayText.Remove( this.DisplayText.Length - 1, 1 );
      }
      else
      {
        displayText = "0";
        m_showNewNumber = true;
      }
      this.DisplayText = displayText;
    }

    private void ProcessDigitKey( CalculatorButtonType buttonType )
    {
      if( m_showNewNumber )
      {
        this.DisplayText = CalculatorUtilities.GetCalculatorButtonContent( buttonType );
      }
      else
      {
        this.DisplayText += CalculatorUtilities.GetCalculatorButtonContent( buttonType );
      }
      m_showNewNumber = false;
    }

    private void ProcessMemoryKey( Calculator.CalculatorButtonType buttonType )
    {
      decimal currentValue = CalculatorUtilities.ParseDecimal( this.DisplayText );
      m_showNewNumber = true;
      switch( buttonType )
      {
        case Calculator.CalculatorButtonType.MAdd:
          this.Memory += currentValue;
          break;
        case Calculator.CalculatorButtonType.MC:
          this.Memory = decimal.Zero;
          break;
        case Calculator.CalculatorButtonType.MR:
          this.DisplayText = this.Memory.ToString();
          m_showNewNumber = false;
          break;
        case Calculator.CalculatorButtonType.MS:
          this.Memory = currentValue;
          break;
        case Calculator.CalculatorButtonType.MSub:
          this.Memory -= currentValue;
          break;
        default:
          break;
      }
    }

    private void ProcessOperationKey( CalculatorButtonType buttonType )
    {
      switch( buttonType )
      {
        case CalculatorButtonType.Add:
          this.Calculate( Operation.Add );
          break;
        case CalculatorButtonType.Subtract:
          this.Calculate( Operation.Subtract );
          break;
        case CalculatorButtonType.Multiply:
          this.Calculate( Operation.Multiply );
          break;
        case CalculatorButtonType.Divide:
          this.Calculate( Operation.Divide );
          break;
        case CalculatorButtonType.Percent:
          if( m_lastOperation != Operation.None )
          {
            decimal currentValue = CalculatorUtilities.ParseDecimal( this.DisplayText );
            decimal newValue = CalculatorUtilities.Percent( m_previousValue, currentValue );
            this.DisplayText = newValue.ToString();
          }
          else
          {
            this.DisplayText = "0";
            m_showNewNumber = true;
          }
          return;
        case CalculatorButtonType.Sqrt:
          this.Calculate( Operation.Sqrt, Operation.None );
          break;
        case CalculatorButtonType.Fraction:
          this.Calculate( Operation.Fraction, Operation.None );
          break;
        case CalculatorButtonType.Negate:
          this.Calculate( Operation.Negate, Operation.None );
          break;
        case CalculatorButtonType.Equal:
          this.Calculate( Operation.None );
          break;
        case CalculatorButtonType.Clear:
          this.Calculate( Operation.Clear, Operation.None );
          break;
        case CalculatorButtonType.Cancel:
          this.DisplayText = m_previousValue.ToString();
          m_lastOperation = Operation.None;
          m_showNewNumber = true;
          return;
        case CalculatorButtonType.Back:
          this.ProcessBackKey();
          return;
        default:
          break;
      }
      decimal.TryParse( this.DisplayText, out m_previousValue );
      m_showNewNumber = true;
    }

    #endregion //Methods

    #region Events

    public event EventHandler<ValueChangedEventArgs<decimal?>> ValueChanged;
    public void RaiseValueChangedEvent( object sender, Xceed.Maui.Toolkit.ValueChangedEventArgs<decimal?> e )
    {
      if( this.IsEnabled )
      {
        this.ValueChanged?.Invoke( sender, e );
      }
    }

    #endregion //Events
  }
}
