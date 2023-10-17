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
  public partial class CheckBox : ToggleButton
  {
    #region Private Members

    private Border m_animateBorder;
    private Border m_mainBorder;

    #endregion

    #region Public Properties

    #region BoxSizeRequest

    public static readonly BindableProperty BoxSizeRequestProperty = BindableProperty.Create( nameof( BoxSizeRequest ), typeof( double ), typeof( CheckBox ), 20d, coerceValue: OnCoerceBoxSizeRequest );

    public double BoxSizeRequest
    {
      get => (double)GetValue( BoxSizeRequestProperty );
      set => SetValue( BoxSizeRequestProperty, value );
    }

    private static object OnCoerceBoxSizeRequest( BindableObject bindable, object value )
    {
      if( (double)value < 0d )
        throw new InvalidDataException( "BoxSizeRequest must be greater or equal to 0." );

      return value;
    }

    #endregion

    #region CheckedSymbol

    // Content can't be shared with multi CheckBoxes, so we must set ContentTemplate instead of Content on ContentControl in XAML.(https://github.com/dotnet/maui/issues/9747)
    public static readonly BindableProperty CheckedSymbolProperty = BindableProperty.Create( nameof( CheckedSymbol ), typeof( DataTemplate ), typeof( CheckBox ), null );

    public DataTemplate CheckedSymbol
    {
      get => (DataTemplate)GetValue( CheckedSymbolProperty );
      set => SetValue( CheckedSymbolProperty, value );
    }

    #endregion

    #region IndeterminateSymbol

    public static readonly BindableProperty IndeterminateSymbolProperty = BindableProperty.Create( nameof( IndeterminateSymbol ), typeof( DataTemplate ), typeof( CheckBox ), null );

    public DataTemplate IndeterminateSymbol
    {
      get => (DataTemplate)GetValue( IndeterminateSymbolProperty );
      set => SetValue( IndeterminateSymbolProperty, value );
    }

    #endregion

    #region IsAnimated

    public static readonly BindableProperty IsAnimatedProperty = BindableProperty.Create( nameof( IsAnimated ), typeof( bool ), typeof( CheckBox ), true );

    public bool IsAnimated
    {
      get => (bool)GetValue( IsAnimatedProperty );
      set => SetValue( IsAnimatedProperty, value );
    }

    #endregion

    #region UncheckedSymbol

    public static readonly BindableProperty UncheckedSymbolProperty = BindableProperty.Create( nameof( UncheckedSymbol ), typeof( DataTemplate ), typeof( CheckBox ), null );

    public DataTemplate UncheckedSymbol
    {
      get => (DataTemplate)GetValue( UncheckedSymbolProperty );
      set => SetValue( UncheckedSymbolProperty, value );
    }

    #endregion

    #endregion

    #region Partial Methods

    partial void ApplyTemplateForPlatform( Border oldMainBorder, Border newMainBorder );

    #endregion

    #region Overrides Methods

    protected override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      m_animateBorder = this.GetTemplateChild( "PART_AnimateBorder" ) as Border;

      var old_mainBorder = m_mainBorder;

      if( m_mainBorder != null )
      {
        m_mainBorder.PointerDown -= this.Border_PointerDown;
        m_mainBorder.PointerUp -= this.Border_PointerUp;
      }
      m_mainBorder = this.GetTemplateChild( "PART_MainBorder" ) as Border;
      if( m_mainBorder != null )
      {
        m_mainBorder.PointerDown += this.Border_PointerDown;
        m_mainBorder.PointerUp += this.Border_PointerUp;
      }

      this.ApplyTemplateForPlatform( old_mainBorder, m_mainBorder );
    }

    protected override void OnIsCheckedChanged( bool? oldValue, bool? newValue )
    {
      base.OnIsCheckedChanged( oldValue, newValue );

      if( this.IsAnimated && ( m_animateBorder != null ) )
      {
        if( newValue.HasValue && newValue.Value )
        {
          m_animateBorder.TranslateTo( this.BoxSizeRequest, 0d, 250 );
        }
        else 
        {
          m_animateBorder.TranslateTo( 0d, 0d, 0 );
        }
      }
    }

    protected internal override void Initialize()
    {
      base.Initialize();

      if( this.IsAnimated && ( m_animateBorder != null ) )
      {
        if( this.IsChecked.HasValue && this.IsChecked.Value )
        {
          m_animateBorder.TranslateTo( this.BoxSizeRequest, 0d, 0 );
        }
      }
    }

    #endregion

    #region Event Handlers

    private void Border_PointerUp( object sender, EventArgs e )
    {
      base.Button_PointerUp();
    }

    private void Border_PointerDown( object sender, EventArgs e )
    {
      base.Button_PointerDown();
    }

    #endregion
  }
}
