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
  public abstract class UpDownBase<T> : InputBase
  {
    #region Private Members

    private readonly WeakEventManager weakEventManager = new WeakEventManager();
    private bool m_isSyncingTextAndValue;
    private bool m_internalValueSet;
    internal bool m_isTextChangedFromUI;

    #endregion

    #region Constructors

    internal UpDownBase()
    {
      this.Unfocused += this.OnLostFocus;
    }

    #endregion

    #region Public Properties

    #region AllowSpin

    public static readonly BindableProperty AllowSpinProperty = BindableProperty.Create( "AllowSpin", typeof( bool ), typeof( UpDownBase<T> ), true );

    public bool AllowSpin
    {
      get => ( bool )GetValue( AllowSpinProperty );
      set => SetValue( AllowSpinProperty, value );
    }

    #endregion   

    #region ClipValueToMinMax

    public static readonly BindableProperty ClipValueToMinMaxProperty = BindableProperty.Create( "ClipValueToMinMax", typeof( bool ), typeof( UpDownBase<T> ), defaultValue: false );
    public bool ClipValueToMinMax
    {
      get
      {
        return ( bool )GetValue( ClipValueToMinMaxProperty );
      }
      set
      {
        SetValue( ClipValueToMinMaxProperty, value );
      }
    }

    #endregion //ClipValueToMinMax

    #region DefaultValue

    public static readonly BindableProperty DefaultValueProperty = BindableProperty.Create( "DefaultValue", typeof( T ), typeof( UpDownBase<T> ), default( T ), propertyChanged: OnDefaultValueChanged );

    public T DefaultValue
    {
      get => ( T )GetValue( DefaultValueProperty );
      set => SetValue( DefaultValueProperty, value );
    }

    private static void OnDefaultValueChanged( BindableObject bindable, object oldValue, object newValue )
    {
      var upDown = bindable as UpDownBase<T>;
      if( upDown != null )
      {
        upDown.OnDefaultValueChanged();
      }
    }

    private void OnDefaultValueChanged()
    {
      if( string.IsNullOrEmpty( this.Text ) )
      {
        this.SyncTextAndValueProperties( true, this.Text );
      }
    }

    #endregion

    #region DisplayDefaultValueOnEmptyText

    public static readonly BindableProperty DisplayDefaultValueOnEmptyTextProperty = BindableProperty.Create( "DisplayDefaultValueOnEmptyText", typeof( bool ), typeof( UpDownBase<T> ), defaultValue: false, propertyChanged: OnDisplayDefaultValueOnEmptyTextChanged );

    public bool DisplayDefaultValueOnEmptyText
    {
      get
      {
        return ( bool )GetValue( DisplayDefaultValueOnEmptyTextProperty );
      }
      set
      {
        SetValue( DisplayDefaultValueOnEmptyTextProperty, value );
      }
    }

    private static void OnDisplayDefaultValueOnEmptyTextChanged( BindableObject bindable, object oldValue, object newValue )
    {
      var upDown = bindable as UpDownBase<T>;
      if( upDown != null )
      {
        upDown.OnDisplayDefaultValueOnEmptyTextChanged( ( bool )oldValue, ( bool )newValue );
      }
    }

    private void OnDisplayDefaultValueOnEmptyTextChanged( bool oldValue, bool newValue )
    {
      if( this.IsLoaded && string.IsNullOrEmpty( this.Text ) )
      {
        this.SyncTextAndValueProperties( false, this.Text );
      }
    }

    #endregion //DisplayDefaultValueOnEmptyText

    #region Maximum

    public static readonly BindableProperty MaximumProperty = BindableProperty.Create( "Maximum", typeof( T ), typeof( UpDownBase<T> ), default( T ), propertyChanged: OnMaximumChanged, coerceValue: OnCoerceMaximum );

    public T Maximum
    {
      get => ( T )GetValue( MaximumProperty );
      set => SetValue( MaximumProperty, value );
    }

    private static void OnMaximumChanged( BindableObject bindable, object oldValue, object newValue )
    {
      var upDown = bindable as UpDownBase<T>;
      if( upDown != null )
      {
        upDown.OnMaximumChanged( ( T )oldValue, ( T )newValue );
      }
    }

    protected virtual void OnMaximumChanged( T oldValue, T newValue )
    {
      if( this.IsLoaded )
      {
        this.SetValidSpinDirection();
      }
    }

    private static object OnCoerceMaximum( BindableObject bindable, object value )
    {
      if( bindable is UpDownBase<T> upDown )
      {
        return upDown.OnCoerceMaximum( ( T )value );
      }
      return value;
    }

    protected virtual T OnCoerceMaximum( T baseValue )
    {
      return baseValue;
    }

    #endregion

    #region Minimum

    public static readonly BindableProperty MinimumProperty = BindableProperty.Create( "Minimum", typeof( T ), typeof( UpDownBase<T> ), default( T ), propertyChanged: OnMinimumChanged, coerceValue: OnCoerceMinimum );

    public T Minimum
    {
      get => ( T )GetValue( MinimumProperty );
      set => SetValue( MinimumProperty, value );
    }

    private static void OnMinimumChanged( BindableObject bindable, object oldValue, object newValue )
    {
      var upDown = bindable as UpDownBase<T>;
      if( upDown != null )
      {
        upDown.OnMinimumChanged( ( T )oldValue, ( T )newValue );
      }
    }

    protected virtual void OnMinimumChanged( T oldValue, T newValue )
    {
      if( this.IsLoaded )
      {
        this.SetValidSpinDirection();
      }
    }

    private static object OnCoerceMinimum( BindableObject bindable, object value )
    {
      if( bindable is UpDownBase<T> upDown )
      {
        return upDown.OnCoerceMinimum( ( T )value );
      }
      return value;
    }

    protected virtual T OnCoerceMinimum( T baseValue )
    {
      return baseValue;
    }

    #endregion

    #region SpinnerDownContentTemplate

    public static readonly BindableProperty SpinnerDownContentTemplateProperty = BindableProperty.Create( "SpinnerDownContentTemplate", typeof( DataTemplate ), typeof( UpDownBase<T> ), null );

    public DataTemplate SpinnerDownContentTemplate
    {
      get => ( DataTemplate )GetValue( SpinnerDownContentTemplateProperty );
      set => SetValue( SpinnerDownContentTemplateProperty, value );
    }

    #endregion   

    #region SpinnerLocation

    public static readonly BindableProperty SpinnerLocationProperty = BindableProperty.Create( "SpinnerLocation", typeof( SpinnerLocation ), typeof( UpDownBase<T> ), SpinnerLocation.Right );

    public SpinnerLocation SpinnerLocation
    {
      get => ( SpinnerLocation )GetValue( SpinnerLocationProperty );
      set => SetValue( SpinnerLocationProperty, value );
    }

    #endregion   

    #region SpinnerUpContentTemplate

    public static readonly BindableProperty SpinnerUpContentTemplateProperty = BindableProperty.Create( "SpinnerUpContentTemplate", typeof( DataTemplate ), typeof( UpDownBase<T> ), null );

    public DataTemplate SpinnerUpContentTemplate
    {
      get => ( DataTemplate )GetValue( SpinnerUpContentTemplateProperty );
      set => SetValue( SpinnerUpContentTemplateProperty, value );
    }

    #endregion

    #region Value

    public static readonly BindableProperty ValueProperty = BindableProperty.Create( "Value", typeof( T ), typeof( UpDownBase<T> ), default( T ), propertyChanged: OnValueChanged );

    public T Value
    {
      get => ( T )GetValue( ValueProperty );
      set => SetValue( ValueProperty, value );
    }

    private static void OnValueChanged( BindableObject bindable, object oldValue, object newValue )
    {
      var upDown = bindable as UpDownBase<T>;
      if( upDown != null )
      {
        upDown.OnValueChanged( ( T )oldValue, ( T )newValue );
      }
    }

    protected virtual void OnValueChanged( T oldValue, T newValue )
    {
      if( !m_internalValueSet )
      {
        this.SyncTextAndValueProperties( false, null, true );
      }

      this.SetValidSpinDirection();

      // Raise ValueChanged event.
      weakEventManager.HandleEvent( this, new ValueChangedEventArgs<T>( oldValue, newValue ), nameof( this.ValueChanged ) );
    }


    #endregion

    #endregion

    #region Protected Properties
    protected ButtonSpinner Spinner
    {
      get;
      private set;
    }

    protected Entry Entry
    {
      get;
      private set;
    }

    #endregion

    #region Protected Methods

    protected abstract string ConvertValueToText();

#nullable enable
    protected abstract T? ConvertTextToValue( string text );
#nullable disable

    protected abstract void OnIncrement();

    protected abstract void OnDecrement();

    protected abstract void SetValidSpinDirection();

    protected override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      if( this.Entry != null )
      {
        this.Entry.TextChanged -= this.EntryTextChanged;
        this.Entry.Completed -= this.Entry_Completed;
      }
      this.Entry = this.GetTemplateChild( "PART_Entry" ) as Entry;
      if( this.Entry != null )
      {
        this.Entry.TextChanged += this.EntryTextChanged;
        // When Enter key is pressed in windows and Check is pressed in Android.
        this.Entry.Completed += this.Entry_Completed;
      }

      if( this.Spinner != null )
      {
        this.Spinner.Spinned -= this.OnSpinnerSpin;
      }
      this.Spinner = this.GetTemplateChild( "PART_Spinner" ) as ButtonSpinner;
      if( this.Spinner != null )
      {
        this.Spinner.Spinned += this.OnSpinnerSpin;
        this.Spinner.SetSpinnerDirections();
      }

      // This is necessary in MacOS to set the Entry.Text value at startup.
      this.CommitInput();
    }

    protected override void OnTextChanged( string oldValue, string newValue )
    {
      this.SyncTextAndValueProperties( true, this.Text );
    }

    protected override void OnCultureInfoChanged( CultureInfo oldValue, CultureInfo newValue )
    {
      this.SyncTextAndValueProperties( false, null );
    }

    protected bool SyncTextAndValueProperties( bool updateValueFromText, string text )
    {
      return this.SyncTextAndValueProperties( updateValueFromText, text, false );
    }

    protected virtual bool CommitInput()
    {
      return this.SyncTextAndValueProperties( true, this.Text );
    }

    #endregion

    #region Internal Methods

    internal void SetValidSpinDirection( ValidSpinDirections validSpinDirections )
    {
      if( this.Spinner != null )
      {
        this.Spinner.ValidSpinDirections = validSpinDirections;
      }
    }

    internal void ClickUpButton()
    {
      this.OnSpinnerSpin( this, new SpinEventArgs( SpinDirection.Increase ) );
    }

    internal void ClickDownButton()
    {
      this.OnSpinnerSpin( this, new SpinEventArgs( SpinDirection.Decrease ) );
    }

    #endregion

    #region Private Methods

    private void SetValueInternal( T value )
    {
      m_internalValueSet = true;
      try
      {
        this.Value = value;
      }
      finally
      {
        m_internalValueSet = false;
      }
    }

    private bool SyncTextAndValueProperties( bool updateValueFromText, string text, bool forceTextUpdate )
    {
      if( m_isSyncingTextAndValue )
        return true;

      m_isSyncingTextAndValue = true;
      bool parsedTextIsValid = true;
      try
      {
        if( updateValueFromText )
        {
          if( string.IsNullOrEmpty( text ) )
          {
            // An empty input sets the value to the default value.
            this.SetValueInternal( this.DefaultValue );
          }
          else
          {
            try
            {
              T newValue = this.ConvertTextToValue( text );
              if( !object.Equals( newValue, this.Value ) )
              {
                this.SetValueInternal( newValue );
              }
            }
            catch( Exception )
            {
              parsedTextIsValid = false;

              // Just allow any input.
            }
          }
        }

        // Do not touch the ongoing text input from user.
        if( !m_isTextChangedFromUI )
        {
          // Don't replace the empty Text with the non-empty representation of DefaultValue.
          bool shouldKeepEmpty = !forceTextUpdate && string.IsNullOrEmpty( this.Text ) && object.Equals( this.Value, this.DefaultValue ) && !this.DisplayDefaultValueOnEmptyText;
          if( !shouldKeepEmpty )
          {
            string newText = this.ConvertValueToText();
            if( !object.Equals( this.Text, newText ) )
            {
              this.Text = newText;
            }
          }

          // Sync Text and textBox
          if( this.Entry != null )
            this.Entry.Text = Text;
        }

        if( m_isTextChangedFromUI && !parsedTextIsValid )
        {
          // Text input was made from the user and the text
          // repesents an invalid value. Disable the spinner
          // in this case.
          if( this.Spinner != null )
          {
            this.Spinner.ValidSpinDirections = ValidSpinDirections.None;
          }
        }
        else
        {
          this.SetValidSpinDirection();
        }
      }
      finally
      {
        m_isSyncingTextAndValue = false;
      }
      return parsedTextIsValid;
    }    

    #endregion

    #region Events

    public event EventHandler<SpinEventArgs> Spinned
    {
      add => weakEventManager.AddEventHandler( value );
      remove => weakEventManager.RemoveEventHandler( value );
    }

    public event EventHandler<ValueChangedEventArgs<T>> ValueChanged
    {
      add => weakEventManager.AddEventHandler( value );
      remove => weakEventManager.RemoveEventHandler( value );
    }

    #endregion

    #region Event Handlers

    private void OnSpinnerSpin( object sender, SpinEventArgs e )
    {
      if( this.AllowSpin )
      {
        // Raise Spinned event.
        weakEventManager.HandleEvent( this, e, nameof( this.Spinned ) );

        if( e.Direction == SpinDirection.Increase )
        {
          this.OnIncrement();
        }
        else
        {
          this.OnDecrement();
        }
      }
    }

    private void EntryTextChanged( object sender, TextChangedEventArgs e )
    {
      try
      {
        m_isTextChangedFromUI = true;
        var entry = sender as Entry;
        if( entry != null )
        {
          this.Text = entry.Text;
        }
      }
      finally
      {
        m_isTextChangedFromUI = false;
      }

    }

    private void Entry_Completed( object sender, EventArgs e )
    {
      this.CommitInput();
    }

    private void OnLostFocus( object sender, FocusEventArgs e )
    {
      if( !e.IsFocused )
      {
        this.CommitInput();
      }
    }

    #endregion
  }
}
