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
  public abstract partial class DateTimeUpDownBase<T> : UpDownBase<T>
  {
    #region Members

    internal List<DateTimeInfo> _dateTimeInfoList = new List<DateTimeInfo>();
    internal DateTimeInfo _selectedDateTimeInfo;
    internal bool _fireSelectionChangedEvent = true;

    #endregion

    #region Constructors

    internal DateTimeUpDownBase()
    {
      this.InitializeDateTimeInfoList();
      this.Loaded += this.DateTimeUpDownBase_Loaded;

      this.HandlerChanged += this.DateTimeUpDownBase_HandlerChanged;
      this.HandlerChanging += this.DateTimeUpDownBase_HandlerChanging;
    }

    #endregion

    #region Public Properties

    #region CurrentDateTimePart

    public static readonly BindableProperty CurrentDateTimePartProperty = BindableProperty.Create( nameof( CurrentDateTimePart ), typeof( DateTimePart ), typeof( DateTimeUpDownBase<T> ), defaultValue: DateTimePart.Other, propertyChanged: OnCurrentDateTimePartChanged );

    public DateTimePart CurrentDateTimePart
    {
      get => (DateTimePart)GetValue( CurrentDateTimePartProperty );
      set => SetValue( CurrentDateTimePartProperty, value );
    }

    private static void OnCurrentDateTimePartChanged( BindableObject bindable, object oldValue, object newValue )
    {
      if( bindable is DateTimeUpDownBase<T> dateTimeUpDown )
      {
        dateTimeUpDown.OnCurrentDateTimePartChanged( (DateTimePart)oldValue, (DateTimePart)newValue );
      }
    }

    protected virtual void OnCurrentDateTimePartChanged( DateTimePart oldValue, DateTimePart newValue )
    {
      this.Select( this.GetDateTimeInfo( newValue ) );
    }

    #endregion

    #region Step

    public static readonly BindableProperty StepProperty = BindableProperty.Create( nameof( Step ), typeof( int ), typeof( DateTimeUpDownBase<T> ), defaultValue: 1 );

    public int Step
    {
      get => (int)GetValue( StepProperty );
      set => SetValue( StepProperty, value );
    }

    #endregion

    #endregion

    #region Partial Methods

    partial void InitializeForPlatform( object sender, EventArgs e );

    partial void UninitializeForPlatform( object sender, HandlerChangingEventArgs e );

    #endregion    

    #region Protected Methods   

    protected override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      if( this.TextBox != null )
      {
        this.TextBox.SelectionChanged += this.TextBox_SelectionChanged;
      }
    }

    protected override object OnCoerceValue( object value )
    {
      if( this.ClipValueToMinMax )
        return this.GetClippedMinMaxValue( (T)value );

      this.ValidateDefaultMinMax( (T)value );

      return value;
    }

    protected override void SetValidSpinDirection()
    {
    }

    protected virtual void InitializeDateTimeInfoList()
    {
    }

    protected virtual bool IsCurrentValueValid()
    {
      return true;
    }

    protected internal virtual void PerformKeyboardSelection( int nextSelectionStart )
    {
      this.TextBox.Focus();

      this.CommitInput();

      int selectedDateStartPosition = ( _selectedDateTimeInfo != null ) ? _selectedDateTimeInfo.StartPosition : 0;
      int direction = nextSelectionStart - selectedDateStartPosition;
      if( direction > 0 )
      {
        this.Select( this.GetNextDateTimeInfo( nextSelectionStart ) );
      }
      else
      {
        this.Select( this.GetPreviousDateTimeInfo( nextSelectionStart - 1 ) );
      }
    }

    protected virtual void PerformMouseSelection()
    {
      var dateTimeInfo = this.GetDateTimeInfo( this.TextBox.CursorPosition );

      this.Select( dateTimeInfo, false );
    }

    protected override bool IsGreaterThan( T value1, T value2 )
    {
      return false;
    }

    protected override bool IsLowerThan( T value1, T value2 )
    {
      return false;
    }

    #endregion

    #region Internal Methods

    internal T GetClippedMinMaxValue( T value )
    {
      if( this.IsGreaterThan( value, this.Maximum ) )
        return this.Maximum;
      else if( this.IsLowerThan( value, this.Minimum ) )
        return this.Minimum;
      return value;
    }

    internal void ValidateDefaultMinMax( T value )
    {
      // DefaultValue is always accepted.
      if( object.Equals( value, this.DefaultValue ) )
        return;

      if( this.IsLowerThan( value, this.Minimum ) )
        throw new ArgumentOutOfRangeException( "Minimum", String.Format( "Value must be greater than MinValue of {0}", this.Minimum ) );
      else if( this.IsGreaterThan( value, this.Maximum ) )
        throw new ArgumentOutOfRangeException( "Maximum", String.Format( "Value must be less than MaxValue of {0}", this.Maximum ) );
    }

    internal DateTimeInfo GetDateTimeInfo( DateTimePart part )
    {
      return _dateTimeInfoList.FirstOrDefault( ( info ) => info.Type == part );
    }

    internal DateTimeInfo GetDateTimeInfo( int selectionStart )
    {
      return _dateTimeInfoList.FirstOrDefault( ( info ) =>
                              ( info.StartPosition <= selectionStart ) && ( selectionStart < ( info.StartPosition + info.Length ) ) );
    }

    internal virtual void Select( DateTimeInfo info, bool updateTextBoxSelection = true )
    {
      if( ( info != null ) && !info.Equals( _selectedDateTimeInfo ) && ( info.Type != DateTimePart.Other ) && ( this.TextBox != null ) && !string.IsNullOrEmpty( this.TextBox.Text ) )
      {
        if( updateTextBoxSelection )
        {
          this.SetTextBoxSelection( info.StartPosition, info.Length );
        }
        _selectedDateTimeInfo = info;

        this.SetValue( DateTimeUpDownBase<T>.CurrentDateTimePartProperty, info.Type );
      }
    }

    internal void SetTextBoxSelection( int start, int length )
    {
      if( start < 0 )
        throw new InvalidDataException( "TextBox SelectionStart must be greater than or equal to 0." );
      if( length < 0 )
        throw new InvalidDataException( "TextBox SelectionLength must be greater than or equal to 0." );

      _fireSelectionChangedEvent = false;

      this.TextBox.CursorPosition = ( this.TextBox.CursorPosition != 0 ) ? 0 : this.TextBox.Text.Length;
      this.TextBox.SelectionLength = 0;

      this.TextBox.CursorPosition = start;
      this.TextBox.SelectionLength = length;

      _fireSelectionChangedEvent = true;
    }

    internal TextBox GetTextBox()
    {
      return this.TextBox;
    }

    #endregion

    #region Private Methods

    private void InitSelection()
    {
      if( _selectedDateTimeInfo == null )
      {
        this.Select( ( this.CurrentDateTimePart != DateTimePart.Other ) ? this.GetDateTimeInfo( this.CurrentDateTimePart ) : this.GetDateTimeInfo( DateTimePart.Day ) );
      }
    }

    private DateTimeInfo GetNextDateTimeInfo( int nextSelectionStart )
    {
      var nextDateTimeInfo = this.GetDateTimeInfo( nextSelectionStart );
      if( nextDateTimeInfo == null )
      {
        nextDateTimeInfo = _dateTimeInfoList.First();
      }

      var initialDateTimeInfo = nextDateTimeInfo;

      while( nextDateTimeInfo.Type == DateTimePart.Other )
      {
        nextDateTimeInfo = this.GetDateTimeInfo( nextDateTimeInfo.StartPosition + nextDateTimeInfo.Length );
        if( nextDateTimeInfo == null )
        {
          nextDateTimeInfo = _dateTimeInfoList.First();
        }
        if( object.Equals( nextDateTimeInfo, initialDateTimeInfo ) )
          throw new InvalidOperationException( "Couldn't find a valid DateTimeInfo." );
      }

      return nextDateTimeInfo;
    }

    private DateTimeInfo GetPreviousDateTimeInfo( int previousSelectionStart )
    {
      var previousDateTimeInfo = this.GetDateTimeInfo( previousSelectionStart );
      if( previousDateTimeInfo == null )
      {
        if( _dateTimeInfoList.Count > 0 )
        {
          previousDateTimeInfo = _dateTimeInfoList.Last();
        }
      }

      var initialDateTimeInfo = previousDateTimeInfo;

      while( ( previousDateTimeInfo != null ) && ( previousDateTimeInfo.Type == DateTimePart.Other ) )
      {
        previousDateTimeInfo = this.GetDateTimeInfo( previousDateTimeInfo.StartPosition - 1 );
        if( previousDateTimeInfo == null )
        {
          previousDateTimeInfo = _dateTimeInfoList.Last();
        }
        if( object.Equals( previousDateTimeInfo, initialDateTimeInfo ) )
          throw new InvalidOperationException( "Couldn't find a valid DateTimeInfo." );
      }

      return previousDateTimeInfo;
    }

    #endregion

    #region Event Handlers

    private void DateTimeUpDownBase_Loaded( object sender, EventArgs e )
    {
      this.InitSelection();
    }

    private void DateTimeUpDownBase_HandlerChanged( object sender, EventArgs e )
    {
      this.InitializeForPlatform( sender, e );
    }

    private void DateTimeUpDownBase_HandlerChanging( object sender, HandlerChangingEventArgs e )
    {
      this.UninitializeForPlatform( sender, e );
    }

    private void TextBox_SelectionChanged( object sender, EventArgs e )
    {
      if( _fireSelectionChangedEvent )
      {
        this.PerformMouseSelection();
      }
    }

    #endregion
  }
}
