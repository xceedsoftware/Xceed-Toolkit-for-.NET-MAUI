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


using Microsoft.Maui.Controls.Shapes;

namespace Xceed.Maui.Toolkit
{
  public partial class RadioButton : ToggleButton
  {
    #region Private Members

    private const string VisualState_Checked = "Checked";
    private const string VisualState_PressedChecked = "PressedChecked";
#if ANDROID || IOS
    private const double AnimatedEllipse_PressedPercent = 0.5d;
#else
    private const double AnimatedEllipse_PressedPercent = 0.38d;
#endif
    private const double AnimatedEllipse_CheckedPercent = 0.48d;

    private Border m_mainBorder;
    private Ellipse m_animatedEllipse;

    #endregion

    #region Constructors

    public RadioButton()
    {
      this.Loaded += this.RadioButton_Loaded;
    }

    #endregion

    #region Public Properties

    #region BoxSizeRequest

    public static readonly BindableProperty BoxSizeRequestProperty = BindableProperty.Create( nameof( BoxSizeRequest ), typeof( double ), typeof( RadioButton ), 20d, coerceValue: OnCoerceBoxSizeRequest );

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

    #region GroupName

    public static readonly BindableProperty GroupNameProperty = BindableProperty.Create( nameof( GroupName ), typeof( string ), typeof( RadioButton ) );

    public string GroupName
    {
      get => (string)GetValue( GroupNameProperty );
      set => SetValue( GroupNameProperty, value );
    }

    #endregion

    #region IsAnimated

    public static readonly BindableProperty IsAnimatedProperty = BindableProperty.Create( nameof( IsAnimated ), typeof( bool ), typeof( RadioButton ), true );

    public bool IsAnimated
    {
      get => (bool)GetValue( IsAnimatedProperty );
      set => SetValue( IsAnimatedProperty, value );
    }

    #endregion

    #endregion

    #region Partial Methods

    partial void ApplyTemplateForPlatform( Border oldMainBorder, Border newMainBorder );

    partial void SetPlatformSpecificVisualState();

    #endregion

    #region Overrides Methods

    protected override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      var old_mainButton = m_mainBorder;

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

      m_animatedEllipse = this.GetTemplateChild( "PART_AnimatedEllipse" ) as Ellipse;

      this.ApplyTemplateForPlatform( old_mainButton, m_mainBorder );      
    }

    protected override void OnIsPressedChanged( bool oldValue, bool newValue )
    {
      if( this.IsEnabled )
      {
        if( newValue )
        {
          this.RaisePointerDownEvent( this, EventArgs.Empty );

          this.SetAnimatedEllipseVisibility( true );

          if( this.IsChecked.HasValue && this.IsChecked.Value )
          {
            VisualStateManager.GoToState( this, RadioButton.VisualState_PressedChecked );
            this.AnimateEllipseStrokeThickness( "ToPressedChecked", RadioButton.AnimatedEllipse_PressedPercent );
          }
          else
          {
            VisualStateManager.GoToState( this, Button.VisualState_Pressed );
            this.AnimateEllipseStrokeThickness( "ToPressed", RadioButton.AnimatedEllipse_PressedPercent );
          }
        }
        else
        {
          this.RaisePointerUpEvent( this, EventArgs.Empty );

          if( !this.IsChecked.HasValue
            || ( this.IsChecked.HasValue && !this.IsChecked.Value ) )
          {
            this.IsChecked = true;
          }
          else
          {
            this.SetPlatformSpecificVisualState();
          }
        }
      }
    }

    protected override void OnIsCheckedChanged( bool? oldValue, bool? newValue )
    {
      if( newValue.HasValue && newValue.Value )
      {
        this.SetAnimatedEllipseVisibility( true );

        this.ResetMultualExclusiveRadioButtons();

        this.SetPlatformSpecificVisualState();

        this.RaiseCheckedEvent( this, EventArgs.Empty );
      }
      else 
      {
        this.SetAnimatedEllipseVisibility( false );

        VisualStateManager.GoToState( this, VisualStateManager.CommonStates.Normal );

        this.AnimateEllipseStrokeThickness( "ToNormal", 0d );

        this.RaiseUnCheckedEvent( this, EventArgs.Empty );
      }
    }

    #endregion

    #region Private Methods

    private static VisualElement GetParentType<T>( Element element ) where T : VisualElement
    {
      if( element == null )
        return null;

      var parent = element.Parent;
      while( parent != null )
      {
        if( parent is T parentType )
          return parentType;

        parent = parent.Parent;
      }

      return null;
    }

    private void ResetMultualExclusiveRadioButtons()
    {
      if( !string.IsNullOrEmpty( this.GroupName ) )
      {
        var parentPage = RadioButton.GetParentType<ContentPage>( this );
        if( parentPage != null )
        {
          var pageAffectedRadioButtons = parentPage.GetVisualTreeDescendants().Where( child => (child is RadioButton childRadioButton) 
                                                                                              && (childRadioButton.GroupName == this.GroupName) 
                                                                                              && (childRadioButton != this) );

          foreach( RadioButton childRadioButton in pageAffectedRadioButtons )
          {
            childRadioButton.IsChecked = false;
          }
        }
      }
      else
      {
        if( this.Parent is Layout parentLayout )
        {
          foreach( var child in parentLayout.Children )
          {
            if( child is RadioButton childRadioButton
              && ( childRadioButton != this ) )
            {
              childRadioButton.IsChecked = false;
            }
          }
        }
      }
    }

    private void SetAnimatedEllipseVisibility( bool isVisible )
    {
      if( m_animatedEllipse != null )
      {
        m_animatedEllipse.IsVisible = isVisible;
      }
    }

    private void AnimateEllipseStrokeThickness( string animationName, double wantedPercent )
    {
      if( m_animatedEllipse != null )
      {
        var ellipseParent = RadioButton.GetParentType<VisualElement>( m_animatedEllipse );
        if( ellipseParent != null )
        {
          var size = ellipseParent.Width;
          if( size > 0d )
          {
            if( this.IsAnimated )
            {
              var initialStrokeThickness = ( m_animatedEllipse.StrokeThickness != 0 ) ? m_animatedEllipse.StrokeThickness : size;
              var animation = new Animation( value => m_animatedEllipse.StrokeThickness = value, initialStrokeThickness, size * wantedPercent );
              animation.Commit( m_animatedEllipse, animationName, 16, 100 );
            }
            else if( m_animatedEllipse.StrokeThickness == 0 )
            {
              m_animatedEllipse.StrokeThickness = size * RadioButton.AnimatedEllipse_CheckedPercent;
            }
          }
        }
      }
    }

    #endregion

    #region Event Handlers

    private void RadioButton_Loaded( object sender, EventArgs e )
    {
      if( this.IsChecked.HasValue && this.IsChecked.Value )
      {
        this.SetAnimatedEllipseVisibility( true );
        this.Dispatcher.Dispatch( () =>
        {
          this.AnimateEllipseStrokeThickness( "ToChecked", RadioButton.AnimatedEllipse_CheckedPercent );
        } );
      }
    }

    private void Border_PointerDown( object sender, EventArgs e )
    {
      if( this.IsEnabled )
      {
        if( ( this.ClickMode == ClickMode.Press ) && !this.IsPressed )
        {
          this.RaiseClickEvent();
        }

        this.IsPressed = true;
      }
    }

    private void Border_PointerUp( object sender, EventArgs e )
    {
      if( this.IsEnabled )
      {
        if( ( this.ClickMode == ClickMode.Release ) && this.IsPressed )
        {
          this.RaiseClickEvent();
        }
      }

      this.IsPressed = false;
    }

    #endregion
  }
}
