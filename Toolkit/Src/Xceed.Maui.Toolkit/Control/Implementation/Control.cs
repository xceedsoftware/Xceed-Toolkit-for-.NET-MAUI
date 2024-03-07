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


using System.Runtime.CompilerServices;

namespace Xceed.Maui.Toolkit
{
  public abstract class Control : TemplatedView
  {
    #region Internal Members

    internal const double DefaultFontSize = 14d;
    internal const FontAttributes DefaultFontAttributes = FontAttributes.None;
    internal readonly static Color DefaultLightTextColor = Colors.Black;
    internal readonly static Color DefaultDarkTextColor = Colors.White;

    #endregion

    #region Private Members

    private static bool IsFirstLoad = true;

    private Color m_oldTextColor;
    private FontAttributes m_oldFontAttributes;
    private string m_oldFontFamily;
    private double m_oldFontSize;

    #endregion

    #region Constructors

    public Control()
    {
      if( Control.IsFirstLoad )
      {
        // Loaded callback not called when located in a popup.
        this.Loaded += this.Control_Loaded;
      }
    }

    #endregion

    #region Public Properties

    #region Background

    public static new readonly BindableProperty BackgroundProperty = BindableProperty.Create( nameof( Background ), typeof( Brush ), typeof( Control ), null );

    public new Brush Background
    {
      get => (Brush)GetValue( BackgroundProperty );
      set => SetValue( BackgroundProperty, value );
    }

    #endregion

    #region BorderBrush

    public static readonly BindableProperty BorderBrushProperty = BindableProperty.Create( nameof( BorderBrush ), typeof( Brush ), typeof( Control ), null );

    public Brush BorderBrush
    {
      get => (Brush)GetValue( BorderBrushProperty );
      set => SetValue( BorderBrushProperty, value );
    }

    #endregion

    #region BorderThickness

    public static readonly BindableProperty BorderThicknessProperty = BindableProperty.Create( nameof( BorderThickness ), typeof( Thickness ), typeof( Control ), Thickness.Zero );

    public Thickness BorderThickness
    {
      get => (Thickness)GetValue( BorderThicknessProperty );
      set => SetValue( BorderThicknessProperty, value );
    }

    #endregion

    #region CornerRadius

    public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create( nameof( CornerRadius ), typeof( CornerRadius ), typeof( Control ), default( CornerRadius ) );

    public CornerRadius CornerRadius
    {
      get => (CornerRadius)GetValue( CornerRadiusProperty );
      set => SetValue( CornerRadiusProperty, value );
    }

    #endregion

    #region FontAttributes

    public static readonly BindableProperty FontAttributesProperty = BindableProperty.Create( nameof( FontAttributes ), typeof( FontAttributes ), typeof( Control ), FontAttributes.None, propertyChanged: OnFontAttributesChanged );

    public FontAttributes FontAttributes
    {
      get => (FontAttributes)GetValue( FontAttributesProperty );
      set => SetValue( FontAttributesProperty, value );
    }

    private static void OnFontAttributesChanged( BindableObject bindable, object oldValue, object newValue )
    {
      var control = bindable as Control;
      if( control != null )
      {
        control.OnFontAttributesChanged( (FontAttributes)oldValue, (FontAttributes)newValue );
      }
    }

    protected virtual void OnFontAttributesChanged( FontAttributes oldValue, FontAttributes newValue )
    {
      m_oldFontAttributes = oldValue;
    }

    #endregion

    #region FontFamily

    public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create( nameof( FontFamily ), typeof( string ), typeof( Control ), null, propertyChanged: OnFontFamilyChanged );

    public string FontFamily
    {
      get => (string)GetValue( FontFamilyProperty );
      set => SetValue( FontFamilyProperty, value );
    }

    private static void OnFontFamilyChanged( BindableObject bindable, object oldValue, object newValue )
    {
      var control = bindable as Control;
      if( control != null )
      {
        control.OnFontFamilyChanged( (string)oldValue, (string)newValue );
      }
    }

    protected virtual void OnFontFamilyChanged( string oldValue, string newValue )
    {
      m_oldFontFamily = oldValue;
    }

    #endregion

    #region FontSize

    public static readonly BindableProperty FontSizeProperty = BindableProperty.Create( nameof( FontSize ), typeof( double ), typeof( Control ), Control.DefaultFontSize, propertyChanged: OnFontSizeChanged );

    public double FontSize
    {
      get => (double)GetValue( FontSizeProperty );
      set => SetValue( FontSizeProperty, value );
    }

    private static void OnFontSizeChanged( BindableObject bindable, object oldValue, object newValue )
    {
      var control = bindable as Control;
      if( control != null )
      {
        if( (double)newValue > 0 )
        {
          control.OnFontSizeChanged( (double)oldValue, (double)newValue );
        }
        else
        {
          throw new InvalidDataException( "FontSize must be bigger than 0" );
        }
      }
    }

    protected virtual void OnFontSizeChanged( double oldValue, double newValue )
    {
      m_oldFontSize = oldValue;
    }

    #endregion

    #region HorizontalContentOptions

    public static readonly BindableProperty HorizontalContentOptionsProperty = BindableProperty.Create( nameof( HorizontalContentOptions ), typeof( LayoutOptions ), typeof( Control ), LayoutOptions.Center );

    public LayoutOptions HorizontalContentOptions
    {
      get => (LayoutOptions)GetValue( HorizontalContentOptionsProperty );
      set => SetValue( HorizontalContentOptionsProperty, value );
    }

    #endregion

    #region OverrideDefaultVisualStates

    public static readonly BindableProperty OverrideDefaultVisualStatesProperty = BindableProperty.Create( nameof( OverrideDefaultVisualStates ), typeof( bool? ), typeof( Control ), null );

    public bool? OverrideDefaultVisualStates
    {
      get => (bool?)GetValue( OverrideDefaultVisualStatesProperty );
      set => SetValue( OverrideDefaultVisualStatesProperty, value );
    }

    #endregion

    #region Padding

    public static new readonly BindableProperty PaddingProperty = BindableProperty.Create( nameof( Padding ), typeof( Thickness ), typeof( Control ), null );

    public new Thickness Padding
    {
      get => (Thickness)GetValue( PaddingProperty );
      set => SetValue( PaddingProperty, value );
    }

    #endregion

    #region TextColor

    public static readonly BindableProperty TextColorProperty = BindableProperty.Create( nameof( TextColor ), typeof( Color ), typeof( Control ), propertyChanged: OnTextColorChanged );

    public Color TextColor
    {
      get => (Color)GetValue( TextColorProperty );
      set => SetValue( TextColorProperty, value );
    }

    private static void OnTextColorChanged( BindableObject bindable, object oldValue, object newValue )
    {
      var control = bindable as Control;
      if( control != null )
      {
        control.OnTextColorChanged( (Color)oldValue, (Color)newValue );
      }
    }

    protected virtual void OnTextColorChanged( Color oldValue, Color newValue )
    {
      m_oldTextColor = oldValue;
    }

    #endregion

    #region VerticalContentOptions

    public static readonly BindableProperty VerticalContentOptionsProperty = BindableProperty.Create( nameof( VerticalContentOptions ), typeof( LayoutOptions ), typeof( Control ), LayoutOptions.Center );

    public LayoutOptions VerticalContentOptions
    {
      get => (LayoutOptions)GetValue( VerticalContentOptionsProperty );
      set => SetValue( VerticalContentOptionsProperty, value );
    }

    #endregion

    #endregion

    #region Overrides Methods

    protected override void OnPropertyChanged( [CallerMemberName] string propertyName = null )
    {
      base.OnPropertyChanged( propertyName );

      if( propertyName == "Style" )
      {
        Control.UpdateFontProperties( this, this.Style );
      }
    }

    #endregion

    #region Internal Methods

    internal static void UpdateFontProperties( View newValue, Control parentControl )
    {
      if( parentControl != null )
      {
        Control.UpdateFontTextColor( newValue, parentControl.m_oldTextColor, parentControl.TextColor );
        Control.UpdateFontSize( newValue, parentControl.m_oldFontSize, parentControl.FontSize );
        Control.UpdateFontFamily( newValue, parentControl.m_oldFontFamily, parentControl.FontFamily );
        Control.UpdateFontAttributes( newValue, parentControl.m_oldFontAttributes, parentControl.FontAttributes );
      }
    }

    internal static void UpdateFontProperties( Control control, Style newStyle )
    {
      if( newStyle == null )
        return;

      var textColorSetter = newStyle.Setters.FirstOrDefault( setter => setter.Property.PropertyName == "TextColor" );
      if( (textColorSetter != null) && (textColorSetter.Value is Color))
      {
        Control.UpdateFontTextColor( control, control.TextColor, (Color)textColorSetter.Value );
      }

      var fontSizeSetter = newStyle.Setters.FirstOrDefault( setter => setter.Property.PropertyName == "FontSize" );
      if( ( fontSizeSetter != null ) && ( fontSizeSetter.Value is double ) )
      {
        Control.UpdateFontSize( control, control.FontSize, (double)fontSizeSetter.Value );
      }

      var fontAttributesSetter = newStyle.Setters.FirstOrDefault( setter => setter.Property.PropertyName == "FontAttributes" );
      if( ( fontAttributesSetter != null ) && ( fontAttributesSetter.Value is FontAttributes ) )
      {
        Control.UpdateFontAttributes( control, control.FontAttributes, (FontAttributes)fontAttributesSetter.Value );
      }

      var fontFamilySetter = newStyle.Setters.FirstOrDefault( setter => setter.Property.PropertyName == "FontFamily" );
      if( ( fontFamilySetter != null ) && ( fontFamilySetter.Value is string ) )
      {
        Control.UpdateFontFamily( control, control.FontFamily, (string)fontFamilySetter.Value );
      }
    }

    internal static void UpdateFontTextColor( View view, Color oldColor, Color newColor )
    {
      if( view != null )
      {
        var children = view.GetVisualTreeDescendants();
        foreach( var element in children )
        {
          if( element is Label label )
          {
            if( Control.CanSetFontProperty<Color>( label, Label.TextColorProperty, null, oldColor ) )
            {
              label.TextColor = newColor;
            }
          }
          else if( element is Entry entry )
          {
            if( Control.CanSetFontProperty<Color>( entry, Entry.TextColorProperty, null, oldColor ) )
            {
              entry.TextColor = newColor;
            }
          }
          else if( element is Control control )
          {
            if( Control.CanSetFontProperty<Color>( control, Control.TextColorProperty, null, oldColor ) )
            {
              control.TextColor = newColor;
            }
          }
        }
      }
    }

    internal static void UpdateFontSize( View view, double oldSize, double newSize )
    {
      if( view != null )
      {
        var children = view.GetVisualTreeDescendants();
        foreach( var element in children )
        {
          if( element is Label label )
          {
            if( Control.CanSetFontProperty<double>( label, Label.FontSizeProperty, Control.DefaultFontSize, oldSize ) )
            {
              label.FontSize = newSize;
            }
          }
          else if( element is Entry entry )
          {
            if( Control.CanSetFontProperty<double>( entry, Entry.FontSizeProperty, Control.DefaultFontSize, oldSize ) )
            {
              entry.FontSize = newSize;
            }
          }
          else if( element is Control control )
          {
            if( Control.CanSetFontProperty<double>( control, Control.FontSizeProperty, Control.DefaultFontSize, oldSize ) )
            {
              control.FontSize = newSize;
            }
          }
        }
      }
    }

    internal static void UpdateFontFamily( View view, string oldFontFamily, string newFontFamily )
    {
      if( view != null )
      {
        var children = view.GetVisualTreeDescendants();
        foreach( var element in children )
        {
          if( element is Label label )
          {
            if( Control.CanSetFontProperty<string>( label, Label.FontFamilyProperty, null, oldFontFamily ) )
            {
              label.FontFamily = newFontFamily;
            }
          }
          else if( element is Entry entry )
          {
            if( Control.CanSetFontProperty<string>( entry, Entry.FontFamilyProperty, null, oldFontFamily ) )
            {
              entry.FontFamily = newFontFamily;
            }
          }
          else if( element is Control control )
          {
            if( Control.CanSetFontProperty<string>( control, Control.FontFamilyProperty, null, oldFontFamily ) )
            {
              control.FontFamily = newFontFamily;
            }
          }
        }
      }
    }

    internal static void UpdateFontAttributes( View view, FontAttributes oldFontAttributes, FontAttributes newFontAttributes )
    {
      if( view != null )
      {
        var children = view.GetVisualTreeDescendants();
        foreach( var element in children )
        {
          if( element is Label label )
          {
            if( Control.CanSetFontProperty<FontAttributes>( label, Label.FontAttributesProperty, Control.DefaultFontAttributes, oldFontAttributes ) )
            {
              label.FontAttributes = newFontAttributes;
            }
          }
          else if( element is Entry entry )
          {
            if( Control.CanSetFontProperty<FontAttributes>( entry, Entry.FontAttributesProperty, Control.DefaultFontAttributes, oldFontAttributes ) )
            {
              entry.FontAttributes = newFontAttributes;
            }
          }
          else if( element is Control control )
          {
            if( Control.CanSetFontProperty<FontAttributes>( control, Control.FontAttributesProperty, Control.DefaultFontAttributes, oldFontAttributes ) )
            {
              control.FontAttributes = newFontAttributes;
            }
          }
        }
      }
    }

    internal static void ResetIsFirstLoad()
    {
      Control.IsFirstLoad = true;
    }

    internal void SetInitialOverrideDefaultVisualStates()
    {
      if( this.OverrideDefaultVisualStates == null )
      {
        this.OverrideDefaultVisualStates = false;
      }
    }

    #endregion

    #region Private Methods

    private static bool CanSetFontProperty<T>( View view, BindableProperty property, T defaultValue, T oldValue )
    {
      var fontPropertyValue = BindableObjectExtensions.GetPropertyIfSet( view, property, defaultValue );

      return ( fontPropertyValue == null ) || fontPropertyValue.Equals( oldValue ) || fontPropertyValue.Equals( defaultValue );
    }

    #endregion

    #region Event Handlers

    private void Control_Loaded( object sender, EventArgs e )
    {
      if( !AppBuilderExtensions.IsFluentDesignLoaded( this ) )
        throw new InvalidOperationException( "You are using an Xceed Fluent Design control but you aren't referencing it. Please add " +
                                              "'.UseXceedMauiToolkit()' in your MauiProgram.CreateMauiApp() or the '<xctk:FluentDesign/>' +" +
                                              " ResourceDictionary in your ContentPage or App.xaml. See documentation for more details." );

      this.SetInitialOverrideDefaultVisualStates();
    }

    #endregion
  }
}
