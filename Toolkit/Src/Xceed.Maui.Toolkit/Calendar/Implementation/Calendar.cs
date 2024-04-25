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


// This file is inspired from Microsoft under the MIT License.
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;

namespace Xceed.Maui.Toolkit
{
  public enum CalendarMode
  {
    Month,
    Year,
    Decade
  }

  public partial class Calendar : Control
  {
    #region Private Members

    private const string PART_HeaderButton = "PART_HeaderButton";

    private const string PART_PreviousButton = "PART_PreviousButton";

    private const string PART_NextButton = "PART_NextButton";

    private const string PART_MonthView = "PART_MonthView";

    private const string PART_YearView = "PART_YearView";

    private const int COLS = 7;

    private const int ROWS = 7;

    private const int NUMBER_OF_DAYS_IN_WEEK = 7;

    private const int YEARS_PER_DECADE = 10;

    private static DateOnly TodayDateOnly = DateOnly.FromDateTime( DateTime.Today );

    private System.Globalization.Calendar m_calendar = new GregorianCalendar();

    private Button m_headerButton;

    private Grid m_monthView;

    private Button m_nextButton;

    private Button m_previousButton;

    private Grid m_yearView;

    private bool m_isMovingThroughMonths;

    private DateOnly? m_dateRangeStart;

    private DateOnly? m_dateRangeEnd;

    private DateOnly? m_currentDate;

    #endregion

    #region Constructors
    public Calendar()
    {
      this.BlackoutDates = new CalendarBlackoutDatesCollection( this );
      this.BlackoutDates.CollectionChanged += this.BlackoutDates_CollectionChanged;
      this.BlackoutDates.CalendarDateRangeChanged += this.BlackoutDates_CalendarDateRangeChanged;

      this.SelectedDates = new CalendarSelectedDatesCollection( this );
      this.SelectedDates.SelectedDatesChanged += this.SelectedDates_SelectedDatesChanged;
      this.SelectedDates.CollectionChanged += this.SelectedDates_CollectionChanged;
      this.SelectedDates.ItemsCleared += this.SelectedDates_ItemsCleared;

      this.DisplayedDate = Calendar.TodayDateOnly;

      this.Loaded += this.Calendar_Loaded;      
    }

    #endregion

    #region Public Properties

    #region BlackoutDates

    public static readonly BindableProperty BlackoutDatesProperty = BindableProperty.Create( nameof( BlackoutDates ), typeof( CalendarBlackoutDatesCollection ), typeof( Calendar ) );

    public CalendarBlackoutDatesCollection BlackoutDates
    {
      get
      {
        return ( CalendarBlackoutDatesCollection )GetValue( BlackoutDatesProperty );
      }
      private set
      {
        SetValue( BlackoutDatesProperty, value );
      }
    }

    #endregion

    #region CalendarButtonStyle

    public static readonly BindableProperty CalendarButtonStyleProperty = BindableProperty.Create( nameof( CalendarButtonStyle ), typeof( Style ), typeof( Calendar ) );

    public Style CalendarButtonStyle
    {
      get
      {
        return ( Style )GetValue( CalendarButtonStyleProperty );
      }
      set
      {
        SetValue( CalendarButtonStyleProperty, value );
      }
    }
    #endregion

    #region CalendarDayButtonStyle

    public static readonly BindableProperty CalendarDayButtonStyleProperty = BindableProperty.Create( nameof( CalendarDayButtonStyle ), typeof( Style ), typeof( Calendar ) );

    public Style CalendarDayButtonStyle
    {
      get
      {
        return ( Style )GetValue( CalendarDayButtonStyleProperty );
      }
      set
      {
        SetValue( CalendarDayButtonStyleProperty, value );
      }
    }

    #endregion

    #region DisplayedDate

    public static readonly BindableProperty DisplayedDateProperty = BindableProperty.Create( nameof( DisplayedDate ), typeof( DateOnly ), typeof( Calendar ), defaultValue: DateOnly.MinValue, propertyChanged: OnDisplayedDateChanged, coerceValue: CoerceDisplayedDate );

    [TypeConverter( typeof( DateOnlyTypeConverter ) )]
    public DateOnly DisplayedDate
    {
      get
      {
        return (DateOnly)GetValue( DisplayedDateProperty );
      }
      set
      {
        SetValue( DisplayedDateProperty, value );
      }
    }

    private static object CoerceDisplayedDate( BindableObject bindable, object value )
    {
      if( bindable is Calendar calendar )
      {
        var date = ( DateOnly )value;

        if( calendar.FirstVisibleDate.HasValue && ( date < calendar.FirstVisibleDate.Value ) )
          return calendar.FirstVisibleDate.Value;
        else if( calendar.LastVisibleDate.HasValue && ( date > calendar.LastVisibleDate.Value ) )
          return calendar.LastVisibleDate.Value;
      }

      return value;
    }

    private static void OnDisplayedDateChanged( BindableObject bindable, object oldValue, object newValue )
    {
      if( bindable is Calendar calendar )
      {
        calendar.OnDisplayedDateChanged( (DateOnly)oldValue, (DateOnly)newValue );
      }
    }

    protected virtual void OnDisplayedDateChanged( DateOnly oldValue, DateOnly newValue )
    {
      this.DisplayedDateInternal = new DateOnly( newValue.Year, newValue.Month, 1 );
      if( !m_isMovingThroughMonths )
      {
        this.UpdateViews();
      }

      this.RaiseDisplayedDateChanged( this, new ValueChangedEventArgs<DateOnly>( oldValue, newValue ) );
    }


    #endregion

    #region DisplayMode

    public static readonly BindableProperty DisplayModeProperty = BindableProperty.Create( nameof( DisplayMode ), typeof( CalendarMode ), typeof( Calendar ), defaultValue: CalendarMode.Month, propertyChanged: OnDisplayModePropertyChanged );

    public CalendarMode DisplayMode
    {
      get
      {
        return ( CalendarMode )GetValue( DisplayModeProperty );
      }
      set
      {
        SetValue( DisplayModeProperty, value );
      }
    }

    private static void OnDisplayModePropertyChanged( BindableObject bindable, object oldValue, object newValue )
    {
      if( bindable is Calendar calendar )
      {
        calendar.OnDisplayModePropertyChanged( ( CalendarMode )oldValue, ( CalendarMode )newValue );
      }
    }

    protected virtual void OnDisplayModePropertyChanged( CalendarMode oldValue, CalendarMode newValue )
    {
      if( m_headerButton != null )
      {
        m_headerButton.IsEnabled = false;
      }
      if( m_monthView != null )
      {
        m_monthView.IsVisible = false;
      }
      if( m_yearView != null )
      {
        m_yearView.IsVisible = false;
      }

      switch( newValue )
      {
        case CalendarMode.Month:
          {
            if( m_headerButton != null )
            {
              m_headerButton.IsEnabled = true;
            }
            if( m_monthView != null )
            {
              m_monthView.IsVisible = true;
            }
            if( oldValue == CalendarMode.Year || oldValue == CalendarMode.Decade )
            {
              this.DateRangeStart = ( this.DateRangeEnd = null );
              this.CurrentDateInternal = this.DisplayedDate;
            }
          }
          break;
        case CalendarMode.Year:
          {
            if( m_headerButton != null )
            {
              m_headerButton.IsEnabled = true;
            }
            if( m_yearView != null )
            {
              m_yearView.IsVisible = true;
            }
          }
          break;
        case CalendarMode.Decade:
          {
            if( m_yearView != null )
            {
              m_yearView.IsVisible = true;
            }
          }
          break;
      }

      this.UpdateViews();

      this.RaiseDisplayModeChanged( this, new ValueChangedEventArgs<CalendarMode>( oldValue, newValue ) );
    }

    #endregion

    #region FirstDayOfWeek

    public static readonly BindableProperty FirstDayOfWeekProperty = BindableProperty.Create( nameof( FirstDayOfWeek ), typeof( DayOfWeek ), typeof( Calendar ), defaultValue: DateTimeHelper.GetDateFormat( CultureInfo.CurrentCulture ).FirstDayOfWeek, propertyChanged: OnFirstDayOfWeekChanged );

    public DayOfWeek FirstDayOfWeek
    {
      get
      {
        return ( DayOfWeek )GetValue( FirstDayOfWeekProperty );
      }
      set
      {
        SetValue( FirstDayOfWeekProperty, value );
      }
    }

    private static void OnFirstDayOfWeekChanged( BindableObject bindable, object oldValue, object newValue )
    {
      if( bindable is Calendar calendar )
      {
        calendar.OnFirstDayOfWeekChanged( ( DayOfWeek )oldValue, ( DayOfWeek )newValue );
      }
    }

    protected virtual void OnFirstDayOfWeekChanged( DayOfWeek oldValue, DayOfWeek newValue )
    {
      this.UpdateViews();
    }

    #endregion

    #region FirstVisibleDate

    public static readonly BindableProperty FirstVisibleDateProperty = BindableProperty.Create( nameof( FirstVisibleDate ), typeof( DateOnly? ), typeof( Calendar ), defaultBindingMode: BindingMode.TwoWay, defaultValue: null, propertyChanged: OnFirstVisibleDateChanged, coerceValue: CoerceFirstVisibleDate );

    [TypeConverter( typeof( DateOnlyTypeConverter ) )]
    public DateOnly? FirstVisibleDate
    {
      get
      {
        return (DateOnly?)GetValue( FirstVisibleDateProperty );
      }
      set
      {
        SetValue( FirstVisibleDateProperty, value );
      }
    }

    private static object CoerceFirstVisibleDate( BindableObject bindable, object value )
    {
      if( bindable is Calendar calendar )
      {
        var minDate = (DateOnly?)value;
        if( minDate.HasValue && calendar.LastVisibleDate.HasValue && ( minDate.Value > calendar.LastVisibleDate.Value ) )
          return calendar.LastVisibleDate;
      }

      return value;
    }

    private static void OnFirstVisibleDateChanged( BindableObject bindable, object oldValue, object newValue )
    {
      if( bindable is Calendar calendar )
      {
        calendar.OnFirstVisibleDateChanged( (DateOnly?)oldValue, (DateOnly?)newValue );
      }
    }

    protected virtual void OnFirstVisibleDateChanged( DateOnly? oldValue, DateOnly? newValue )
    {
      this.CoerceValue( Calendar.LastVisibleDateProperty );
      this.CoerceValue( Calendar.DisplayedDateProperty );
      this.CoerceValue( SelectedDatesProperty );
      this.UpdateViews();
    }

    #endregion

    #region IsTodayHighlighted

    public static readonly BindableProperty IsTodayHighlightedProperty = BindableProperty.Create( nameof( IsTodayHighlighted ), typeof( bool ), typeof( Calendar ), defaultValue: true, propertyChanged: OnIsTodayHighlightedChanged );

    public bool IsTodayHighlighted
    {
      get
      {
        return ( bool )GetValue( IsTodayHighlightedProperty );
      }
      set
      {
        SetValue( IsTodayHighlightedProperty, value );
      }
    }

    private static void OnIsTodayHighlightedChanged( BindableObject bindable, object oldValue, object newValue )
    {
      if( bindable is Calendar calendar )
      {
        calendar.OnIsTodayHighlightedChanged( ( bool )oldValue, ( bool )newValue );
      }
    }

    protected virtual void OnIsTodayHighlightedChanged( bool oldValue, bool newValue )
    {
      var num = DateTimeHelper.CompareYearAndMonth( this.DisplayedDateInternal, Calendar.TodayDateOnly );
      if( ( num > -2 ) && ( num < 2 ) )
      {
        this.UpdateViews();
      }
    }

    #endregion

    #region LastVisibleDate 

    public static readonly BindableProperty LastVisibleDateProperty = BindableProperty.Create( nameof( LastVisibleDate ), typeof( DateOnly? ), typeof( Calendar ), defaultBindingMode: BindingMode.TwoWay, defaultValue: null, propertyChanged: OnLastVisibleDateChanged, coerceValue: CoerceLastVisibleDate );

    [TypeConverter( typeof( DateOnlyTypeConverter ) )]
    public DateOnly? LastVisibleDate
    {
      get
      {
        return (DateOnly?)GetValue( LastVisibleDateProperty );
      }
      set
      {
        SetValue( LastVisibleDateProperty, value );
      }
    }

    private static object CoerceLastVisibleDate( BindableObject bindable, object value )
    {
      if( bindable is Calendar calendar )
      {
        var maxDate = (DateOnly?)value;
        if( maxDate.HasValue && calendar.FirstVisibleDate.HasValue && ( maxDate.Value < calendar.FirstVisibleDate.Value ) )
          return calendar.FirstVisibleDate;
      }
      return value;
    }

    private static void OnLastVisibleDateChanged( BindableObject bindable, object oldValue, object newValue )
    {
      if( bindable is Calendar calendar )
      {
        calendar.OnLastVisibleDateChanged( (DateOnly?)oldValue, (DateOnly?)newValue );
      }
    }

    protected virtual void OnLastVisibleDateChanged( DateOnly? oldValue, DateOnly? newValue )
    {
      this.CoerceValue( Calendar.FirstVisibleDateProperty );
      this.CoerceValue( Calendar.DisplayedDateProperty );
      this.CoerceValue( Calendar.SelectedDatesProperty );
      this.UpdateViews();
    }

    #endregion

    #region SelectedDate

    public static readonly BindableProperty SelectedDateProperty = BindableProperty.Create( nameof( SelectedDate ), typeof( DateOnly? ), typeof( Calendar ), defaultValue: null, defaultBindingMode: BindingMode.TwoWay, propertyChanged: OnSelectedDateChanged );

    [TypeConverter( typeof( DateOnlyTypeConverter ) )]
    public DateOnly? SelectedDate
    {
      get
      {
        return (DateOnly?)GetValue( SelectedDateProperty );
      }
      set
      {
        SetValue( SelectedDateProperty, value );
      }
    }

    private static void OnSelectedDateChanged( BindableObject bindable, object oldValue, object newValue )
    {
      if( bindable is Calendar calendar )
      {
        calendar.OnSelectedDateChanged( (DateOnly?)oldValue, (DateOnly?)newValue );
      }
    }

    protected virtual void OnSelectedDateChanged( DateOnly? oldValue, DateOnly? newValue )
    {
      if( ( this.SelectionMode != CalendarSelectionMode.None ) || ( newValue == null ) )
      {
        if( Calendar.IsValidDateSelection( this, newValue ) )
        {
          if( this.SelectionMode == CalendarSelectionMode.Single )
          {
            if( !newValue.HasValue )
            {
              this.SelectedDates.ClearInternal();
              if( this.SelectedDate.HasValue )
              {
                this.SelectedDate = null;
              }

              if( this.SelectedDates.OldItems.Count > 0 )
              {
                Collection<DateOnly> addedItems = [];
                this.OnSelectedDatesCollectionChanged( new ValueChangedEventArgs<Collection<DateOnly>>( this.SelectedDates.OldItems, addedItems ) );

                this.SelectedDates.OldItems.Clear();
              }
              this.UpdateViews();
            }
            else if( newValue.HasValue && ( this.SelectedDates.Count <= 0 || !( this.SelectedDates[ 0 ] == newValue.Value ) ) )
            {
              this.SelectedDates.ClearInternal();
              this.SelectedDates.Add( newValue.Value );
            }
            if( newValue.HasValue )
            {
              this.CurrentDateInternal = newValue.Value;
            }
            this.UpdateViews();
          }
          return;
        }
        throw new ArgumentOutOfRangeException( $"The value {newValue} is invalid as SelectedDate property." );
      }
      throw new InvalidOperationException( "The SelectedDate property cannot be set when the selection mode is None." );
    }

    #endregion

    #region SelectedDates

    public static readonly BindableProperty SelectedDatesProperty = BindableProperty.Create( nameof( SelectedDates ), typeof( CalendarSelectedDatesCollection ), typeof( Calendar ), coerceValue: CoerceSelectedDates );

    public CalendarSelectedDatesCollection SelectedDates
    {
      get
      {
        return ( CalendarSelectedDatesCollection )GetValue( SelectedDatesProperty );
      }
      private set
      {
        SetValue( SelectedDatesProperty, value );
      }
    }

    private static object CoerceSelectedDates( BindableObject bindable, object value )
    {
      if( bindable is Calendar calendar )
      {
        foreach( var date in ( CalendarSelectedDatesCollection )value )
        {
          if( calendar.FirstVisibleDate.HasValue )
          {
            if( calendar.FirstVisibleDate.Value > date )
              throw new InvalidDataException( $"The value {date} is invalid as SelectedDate property." );
          }
          if( calendar.LastVisibleDate.HasValue )
          {
            if( calendar.LastVisibleDate.Value < date )
              throw new InvalidDataException( $"The value {date} is invalid as SelectedDate property." );
          }
        }
      }
      return value;
    }

    #endregion

    #region SelectionMode

    public static readonly BindableProperty SelectionModeProperty = BindableProperty.Create( nameof( SelectionMode ), typeof( CalendarSelectionMode ), typeof( Calendar ), defaultValue: CalendarSelectionMode.Single, propertyChanged: OnSelectionModeChanged );

    public CalendarSelectionMode SelectionMode
    {
      get
      {
        return ( CalendarSelectionMode )GetValue( SelectionModeProperty );
      }
      set
      {
        SetValue( SelectionModeProperty, value );
      }
    }

    private static void OnSelectionModeChanged( BindableObject bindable, object oldValue, object newValue )
    {
      if( bindable is Calendar calendar )
      {
        calendar.OnSelectionModeChanged( ( CalendarSelectionMode )oldValue, ( CalendarSelectionMode )newValue );
      }
    }

    protected virtual void OnSelectionModeChanged( CalendarSelectionMode oldValue, CalendarSelectionMode newValue )
    {
      this.DateRangeStart = null;
      this.DateRangeEnd = null;
      this.SelectedDates.ClearInternal();
      if( this.SelectedDate.HasValue )
      {
        this.SelectedDate = null;
      }

      if( this.SelectedDates.OldItems.Count > 0 )
      {
        Collection<DateOnly> addedItems = [];
        this.OnSelectedDatesCollectionChanged( new ValueChangedEventArgs<Collection<DateOnly>>( this.SelectedDates.OldItems, addedItems ) );
        this.SelectedDates.OldItems.Clear();
      }
      this.UpdateViews();
      this.RaiseSelectionModeChanged( this, EventArgs.Empty );
    }

    #endregion

    #endregion

    #region Internal Properties

    internal DateOnly DisplayedDateInternal { get; private set; }

    internal DateOnly LastVisibleDateInternal => this.LastVisibleDate.GetValueOrDefault( DateOnly.MaxValue );

    internal DateOnly FirstVisibleDateInternal => this.FirstVisibleDate.GetValueOrDefault( DateOnly.MinValue );

    internal DateOnly CurrentDateInternal
    {
      get
      {
        return m_currentDate.GetValueOrDefault( this.DisplayedDateInternal );
      }
      set
      {
        m_currentDate = value;
      }
    }

    internal DateOnly? DateRangeStart
    {
      get
      {
        if( this.SelectionMode != CalendarSelectionMode.None )
          return m_dateRangeStart;

        return null;
      }
      set
      {
        m_dateRangeStart = value;
      }
    }

    internal DateOnly? DateRangeEnd
    {
      get
      {
        if( this.SelectionMode != CalendarSelectionMode.None )
          return m_dateRangeEnd;

        return null;
      }
      set
      {
        m_dateRangeEnd = value;
      }
    }

    #endregion    

    #region Protected Methods

    protected override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      if( m_monthView != null )
      {
        foreach( var calendarDayButton in m_monthView.Children.OfType<CalendarDayButton>() )
        {
          calendarDayButton.Clicked -= this.CalendarDayButton_Clicked;
        }
      }
      m_monthView = this.GetTemplateChild( PART_MonthView ) as Grid;
      if( m_monthView != null )
      {
        foreach( var calendarDayButton in m_monthView.Children.OfType<CalendarDayButton>() )
        {
          calendarDayButton.Clicked += this.CalendarDayButton_Clicked;
        }
      }

      if( m_yearView != null )
      {
        foreach( var calendarButton in m_yearView.Children.OfType<CalendarButton>() )
        {
          calendarButton.Clicked -= this.CalendarButton_Clicked;
        }
      }
      m_yearView = this.GetTemplateChild( PART_YearView ) as Grid;
      if( m_yearView != null )
      {
        foreach( var calendarButton in m_yearView.Children.OfType<CalendarButton>() )
        {
          calendarButton.Clicked += this.CalendarButton_Clicked;
        }
      }

      if( m_headerButton != null )
      {
        m_headerButton.Clicked -= this.HeaderButton_Click;
      }
      m_headerButton = this.GetTemplateChild( PART_HeaderButton ) as Button;
      if( m_headerButton != null )
      {
        m_headerButton.Clicked += this.HeaderButton_Click;
      }

      if( m_previousButton != null )
      {
        m_previousButton.Clicked -= this.PreviousButton_Click;
      }
      m_previousButton = this.GetTemplateChild( PART_PreviousButton ) as Button;
      if( m_previousButton != null )
      {
        m_previousButton.Clicked += this.PreviousButton_Click;
      }

      if( m_nextButton != null )
      {
        m_nextButton.Clicked -= this.NextButton_Click;
      }
      m_nextButton = this.GetTemplateChild( PART_NextButton ) as Button;
      if( m_nextButton != null )
      {
        m_nextButton.Clicked += this.NextButton_Click;
      }

      this.CurrentDateInternal = this.DisplayedDate;
    }

    #endregion

    #region Internal Methods

    internal void RefreshDecadeView()
    {
      var decadeForDecadeMode = new DateOnly( this.DisplayedDate.Year, 1, 1 ).Year - new DateOnly( this.DisplayedDate.Year, 1, 1 ).Year % 10;
      var num = decadeForDecadeMode + 9;
      this.DecadeHeaderButton( decadeForDecadeMode );
      this.DecadePreviousButton( decadeForDecadeMode );
      this.DecadeNextButton( num );
      if( m_yearView != null )
      {
        this.RefreshDecadeViewCalendarButtons( decadeForDecadeMode, num );
      }
    }

    internal void RefreshMonthView()
    {
      this.RefreshMonthViewHeaderButton();
      this.RefreshMonthViewPreviousButton();
      this.RefreshMonthViewNextButton();
      this.RefreshMonthViewDayTitles();
      if( m_monthView != null )
      {
        this.RefreshMonthViewCalendarDayButtons();
      }
    }

    internal void RefreshYearView()
    {
      this.RefreshYearViewPreviousButton();
      this.RefreshYearViewNextButton();
      this.RefreshYearViewHeaderButton();
      if( m_yearView != null )
      {
        this.RefreshYearViewCalendarButtons();
      }
    }

    internal static bool IsValidDateSelection( Calendar cal, DateOnly? value )
    {
      if( value.HasValue )
      {
        var flag = true;
        if( cal.BlackoutDates.Contains( value.Value )
          || ( cal.FirstVisibleDate != null &&  ( value.Value < cal.FirstVisibleDate.Value ) )
          || ( cal.LastVisibleDate != null && ( value.Value > cal.LastVisibleDate.Value ) ) )
        {
          flag = false;
        }
        return flag;
      }

      return true;
    }

    internal void OnDayClick( DateOnly selectedDate )
    {
      if( this.IsEnabled )
      {
        switch( this.SelectionMode )
        {
          case CalendarSelectionMode.None:
            {
              this.CurrentDateInternal = selectedDate;
              return;
            }
          case CalendarSelectionMode.Single:
          case CalendarSelectionMode.Multiple:
            {
              if( Calendar.IsValidDateSelection( this, selectedDate ) )
              {
                if( this.SelectionMode == CalendarSelectionMode.Single )
                {
                  if( !this.SelectedDate.HasValue || ( this.SelectedDate.Value != selectedDate ) )
                  {
                    this.SelectedDate = selectedDate;
                  }
                  else
                  {
                    this.SelectedDate = null;
                  }
                }
                else if( this.SelectionMode == CalendarSelectionMode.Multiple )
                {
                  if( !this.SelectedDates.Remove( selectedDate ) )
                  {
                    this.SelectedDate = selectedDate;
                    this.SelectedDates.Add( selectedDate );
                  }
                  else
                  {
                    this.SelectedDate = this.SelectedDates.LastOrDefault();
                  }
                }
              }
              this.DateRangeStart = null;
              this.DateRangeEnd = null;
            }
            break;
          case CalendarSelectionMode.Range:
            {
              if( this.DateRangeStart == null && this.DateRangeEnd == null )
              {
                this.SelectedDates.ClearInternal();
                this.SelectedDate = selectedDate;
                this.DateRangeStart = selectedDate;
                this.SelectedDates.Add( selectedDate );
              }
              else
              {
                if( this.DateRangeStart != null && this.DateRangeEnd == null )
                {
                  var rangeStart = this.DateRangeStart.Value;
                  this.DateRangeEnd = selectedDate;
                  this.SelectedDates.IsAddingRange = true;
                  if( this.SelectedDates.Count > 0 )
                  {
                    this.SelectedDates.ClearInternal();
                  }

                  var currentDate = rangeStart;
                  this.SelectedDates.NewItems = [];
                  if( rangeStart == this.DateRangeEnd.Value )
                  {
                    this.SelectedDates.OldItems = [];
                  }

                  foreach( var item in CalendarSelectedDatesCollection.GetDaysInRange( rangeStart, this.DateRangeEnd.Value ) )
                  {
                    if( Calendar.IsValidDateSelection( this, item ) )
                    {
                      if( !this.SelectedDates.Contains( item ) )
                      {
                        this.SelectedDates.Add( item );
                        this.SelectedDates.NewItems.Add( item );
                      }
                      currentDate = item;
                    }
                  }
                  this.SelectedDates.EndAddRange();
                  this.DateRangeStart = null;
                  this.DateRangeEnd = null;
                }
              }
            }
            break;
          case CalendarSelectionMode.MultiRange:
            {
              if( !this.DateRangeStart.HasValue )
              {
                this.DateRangeStart = selectedDate;
                if( this.SelectedDate == null )
                {
                  this.SelectedDate = selectedDate;
                }
                if( !this.SelectedDates.Remove( this.DateRangeStart.Value ) )
                {
                  this.SelectedDates.Add( this.DateRangeStart.Value );
                }
              }
              else
              {
                this.CurrentDateInternal = selectedDate;
                if( this.DateRangeStart.HasValue )
                {
                  this.SelectedDates.IsAddingRange = true;
                  var currentDate = this.DateRangeStart.Value;
                  this.SelectedDates.NewItems = new Collection<DateOnly>( this.SelectedDates.ToList() );
                  this.SelectedDates.OldItems = new Collection<DateOnly>( this.SelectedDates.ToList() );
                  this.CurrentDateInternal = selectedDate;

                  foreach( var item in CalendarSelectedDatesCollection.GetDaysInRange( this.DateRangeStart.Value, selectedDate ) )
                  {
                    if( item != this.DateRangeStart.Value )
                    {
                      if( Calendar.IsValidDateSelection( this, item ) )
                      {
                        if( !this.SelectedDates.Remove( item ) )
                        {
                          this.SelectedDates.Add( item );
                          this.SelectedDates.NewItems.Add( item );
                          currentDate = item;
                        }
                        else
                        {
                          this.SelectedDates.NewItems.Remove( item );
                        }
                      }
                      else
                      {
                        this.CurrentDateInternal = currentDate;
                      }
                    }
                  }
                  this.SelectedDates.EndAddRange();
                  this.DateRangeStart = null;
                  this.DateRangeEnd = null;
                }
              }
            }
            break;
        }

        if( DateTimeHelper.CompareYearAndMonth( selectedDate, this.DisplayedDateInternal ) != 0 )
        {
          this.MoveViewToDate( selectedDate );
          return;
        }

        this.UpdateViews();
      }
    }

    internal void OnCalendarButtonPressed( CalendarButton b, bool switchDisplayMode )
    {
      if( b.BindingContext is DateOnly yearMonth )
      {
        DateOnly? date = null;
        var calendarMode = CalendarMode.Month;

        switch( this.DisplayMode )
        {
          case CalendarMode.Year:
            {
              date = DateTimeHelper.SetYearMonth( this.DisplayedDate, yearMonth );
              calendarMode = CalendarMode.Month;
            }
            break;
          case CalendarMode.Decade:
            {
              date = DateTimeHelper.AddYears( this.DisplayedDate, yearMonth.Year - this.DisplayedDate.Year );
              calendarMode = CalendarMode.Year;
            }
            break;
        }

        if( date.HasValue )
        {
          this.DisplayedDate = date.Value;
          if( switchDisplayMode )
          {
            this.DisplayMode = calendarMode;
          }
        }
      }
    }

    internal void OnNextClick()
    {
      if( this.IsEnabled )
      {
        m_isMovingThroughMonths = true;

        var dateOffset = this.GetDateOffset( this.DisplayedDate, 1, this.DisplayMode );
        if( dateOffset.HasValue )
        {
          this.MoveViewToDate( new DateOnly( dateOffset.Value.Year, dateOffset.Value.Month, 1 ) );
        }

        m_isMovingThroughMonths = false;
      }
    }

    internal void OnPreviousClick()
    {
      if( this.IsEnabled )
      {
        m_isMovingThroughMonths = true;

        var dateOffset = this.GetDateOffset( this.DisplayedDate, -1, this.DisplayMode );
        if( dateOffset.HasValue )
        {
          this.MoveViewToDate( new DateOnly( dateOffset.Value.Year, dateOffset.Value.Month, 1 ) );
        }

        m_isMovingThroughMonths = false;
      }
    }

    internal void OnHeaderClick()
    {
      if( this.IsEnabled )
      {
        this.DisplayMode = this.DisplayMode == CalendarMode.Month ? CalendarMode.Year : CalendarMode.Decade;
      }
    }

    internal Button GetHeaderButton()
    {
      return m_headerButton;
    }

    internal void UpdateViews()
    {
      if( !this.IsLoaded )
        return;

      switch( this.DisplayMode )
      {
        case CalendarMode.Month:
          this.RefreshMonthView();
          break;
        case CalendarMode.Year:
          this.RefreshYearView();
          break;
        case CalendarMode.Decade:
          this.RefreshDecadeView();
          break;
      }
    }

    internal void OnSelectedDatesCollectionChanged( ValueChangedEventArgs<Collection<DateOnly>> e )
    {
      if( Calendar.IsSelectionChanged( e ) )
      {
        this.CoerceFromSelection();
        this.RaiseSelectedDatesChanged( this, e );
      }
    }

    internal IEnumerable<CalendarDayButton> GetCalendarDayButtons()
    {
      if( m_monthView == null )
        yield break;

      var dayButtonsHost = m_monthView.Children;
      for( var childIndex = NUMBER_OF_DAYS_IN_WEEK; childIndex < ROWS * COLS; childIndex++ )
      {
        var calendarDayButton = dayButtonsHost[ childIndex ] as CalendarDayButton;
        if( calendarDayButton != null )
          yield return calendarDayButton;
      }
    }

    internal IEnumerable<CalendarButton> GetCalendarButtons()
    {
      if( m_yearView == null )
        return null;

      return m_yearView.Children.Cast<CalendarButton>();
    }

    #endregion

    #region Private Methods

    #region Month View
    private void RefreshMonthViewHeaderButton()
    {
      if( m_headerButton != null )
      {
        m_headerButton.Content = DateTimeHelper.ToYearMonthPatternString( this.DisplayedDate, CultureInfo.CurrentCulture );
      }
    }

    private void RefreshMonthViewPreviousButton()
    {
      if( m_previousButton != null )
      {
        var dt = new DateOnly( this.DisplayedDate.Year, this.DisplayedDate.Month, 1 );
        m_previousButton.IsEnabled = ( this.FirstVisibleDateInternal < dt );
      }
    }

    private void RefreshMonthViewNextButton()
    {
      if( m_nextButton != null )
      {
        var date = new DateOnly( this.DisplayedDate.Year, this.DisplayedDate.Month, 1 );
        if( DateTimeHelper.CompareYearAndMonth( date, DateOnly.MaxValue ) == 0 )
        {
          m_nextButton.IsEnabled = false;
          return;
        }

        var dt = date.AddMonths( 1 );
        m_nextButton.IsEnabled = (this.LastVisibleDateInternal >= dt);
      }
    }

    private void RefreshMonthViewDayTitles()
    {
      if( (m_monthView == null) || (m_monthView.Children.Count == 0) )
        return;
      if( (m_monthView.Children[ 0 ] is Label label) && (label.BindingContext != null) )
        return;

      var shortestDayNames = DateTimeHelper.GetDateFormat( CultureInfo.CurrentCulture ).ShortestDayNames;
      for( var i = 0; i < NUMBER_OF_DAYS_IN_WEEK; i++ )
      {
        var dayLabel = (Label)m_monthView.Children[ i ];
        dayLabel.Text = shortestDayNames[ (int)( i + this.FirstDayOfWeek ) % shortestDayNames.Length ];
      }
    }

    internal void RefreshMonthViewCalendarDayButtons()
    {
      if( m_monthView.Children.Count < ( COLS * ROWS ) )
        return;

      var date = new DateOnly( this.DisplayedDate.Year, this.DisplayedDate.Month, 1 );
      var numberOfVisibleDaysFromPreviousMonth = this.GetNumberOfVisibleDaysFromPreviousMonth( date );
      var flag = DateTimeHelper.CompareYearAndMonth( date, DateOnly.MinValue ) <= 0;
      var flag2 = DateTimeHelper.CompareYearAndMonth( date, DateOnly.MaxValue ) >= 0;
      var daysInMonth =  m_calendar.GetDaysInMonth( date.Year, date.Month );
      for( var i = NUMBER_OF_DAYS_IN_WEEK; i < COLS * ROWS; i++ )
      {
        var calendarDayButton = m_monthView.Children[ i ] as CalendarDayButton;
        var num2 = i - numberOfVisibleDaysFromPreviousMonth - NUMBER_OF_DAYS_IN_WEEK;        
        if( ( !flag || num2 >= 0 ) && ( !flag2 || num2 < daysInMonth ) )
        {
          var date2 = date.AddDays( num2 );
          this.RefreshMonthDayButtonState( calendarDayButton, date2 );
        }
        else
        {
          this.RefreshMonthDayButtonState( calendarDayButton, null );
        }
      }
    }

    private int GetNumberOfVisibleDaysFromPreviousMonth( DateOnly firstOfMonth )
    {
      var dayOfWeek = firstOfMonth.DayOfWeek;
      int num = ( dayOfWeek - this.FirstDayOfWeek + NUMBER_OF_DAYS_IN_WEEK ) % NUMBER_OF_DAYS_IN_WEEK;
      if( num == 0 )
        return NUMBER_OF_DAYS_IN_WEEK;

      return num;
    }

    private void RefreshMonthDayButtonState( CalendarDayButton childButton, DateOnly? dateToAdd )
    {
      if( dateToAdd.HasValue )
      {
        var visibilityFlag = true;
        var enabledFlag = this.IsEnabled;
        if( (dateToAdd.Value < this.FirstVisibleDateInternal)
          || (dateToAdd.Value > this.LastVisibleDateInternal) )
        {
          visibilityFlag = false;
          enabledFlag = false;
        }
        var selectionFlag = this.SelectedDates.Contains( dateToAdd.Value );

        childButton.ParentCalendar = this;
        childButton.IsBlackedOut = this.BlackoutDates.Contains( dateToAdd.Value );
        childButton.IsInactive = DateTimeHelper.CompareYearAndMonth( dateToAdd.Value, this.DisplayedDateInternal ) != 0;
        childButton.IsToday = (dateToAdd.Value == Calendar.TodayDateOnly);
        childButton.IsEnabled = enabledFlag;
        childButton.IsVisible = visibilityFlag;
        childButton.IsSelected = selectionFlag;
        childButton.Text = dateToAdd.Value.ToString( "%d" );
        childButton.BindingContext = dateToAdd.Value;

        if( childButton.IsLoaded )
        {
          childButton.NotifyNeedsVisualStateUpdate();
        }
      }
    }

    #endregion

    #region Year View
    private void RefreshYearViewNextButton()
    {
      if( m_nextButton != null )
      {
        m_nextButton.IsEnabled = this.LastVisibleDateInternal.Year != this.DisplayedDate.Year;
      }
    }

    private void RefreshYearViewPreviousButton()
    {
      if( m_previousButton != null )
      {
        m_previousButton.IsEnabled = this.FirstVisibleDateInternal.Year != this.DisplayedDate.Year;
      }
    }

    private void RefreshYearViewHeaderButton()
    {
      if( m_headerButton != null )
      {
        m_headerButton.Content = DateTimeHelper.ToYearString( this.DisplayedDate, CultureInfo.CurrentCulture );
      }
    }

    private void RefreshYearViewCalendarButtons()
    {
      var num = 0;
      foreach( var calendarButton in m_yearView.Children.OfType<CalendarButton>() )
      {
        var date = new DateOnly( this.DisplayedDate.Year, num + 1, 1 );
        calendarButton.BindingContext = date;
        calendarButton.Text = date.ToString( "MMM" );
        calendarButton.HasSelectedDays = DateTimeHelper.CompareYearAndMonth( date, this.DisplayedDateInternal ) == 0;
        if( DateTimeHelper.CompareYearAndMonth( date, this.FirstVisibleDateInternal ) < 0 || DateTimeHelper.CompareYearAndMonth( date, this.LastVisibleDateInternal ) > 0 )
        {
          calendarButton.IsPointerOver = false;
          calendarButton.IsEnabled = false;
          calendarButton.IsVisible = false;
        }
        else
        {
          calendarButton.IsVisible = true;
          calendarButton.IsEnabled = true;
        }
        calendarButton.IsInactive = false;

        calendarButton.RequestUpdateVisualState();
        num++;
      }
    }

    #endregion

    #region Decade View

    private void DecadeNextButton( int decadeEnd )
    {
      if( m_nextButton != null )
      {
        m_nextButton.IsEnabled = this.LastVisibleDateInternal.Year > decadeEnd;
      }
    }

    private void DecadePreviousButton( int decade )
    {
      if( m_previousButton != null )
      {
        m_previousButton.IsEnabled = decade > this.FirstVisibleDateInternal.Year;
      }
    }

    private void DecadeHeaderButton( int decade )
    {
      if( m_headerButton != null )
      {
        m_headerButton.Content = DateTimeHelper.ToDecadeRangeString( decade, this );
      }
    }

    private void RefreshDecadeViewCalendarButtons( int decade, int decadeEnd )
    {
      var num = -1;
      foreach( var calendarButton in m_yearView.Children.OfType<CalendarButton>() )
      {
        int num2 = decade + num;
        if( ( num2 <= DateOnly.MaxValue.Year ) && ( num2 >= DateOnly.MinValue.Year ) )
        {
          var date = new DateOnly( num2, 1, 1 );
          calendarButton.Text = date.ToString( "yyyy" );
          calendarButton.BindingContext = date;
          calendarButton.HasSelectedDays = this.DisplayedDate.Year == num2;
          if( ( num2 < this.FirstVisibleDateInternal.Year ) || ( num2 > this.LastVisibleDateInternal.Year ) )
          {
            calendarButton.IsPointerOver = false;
            calendarButton.IsEnabled = false;
            calendarButton.IsVisible = false;
          }
          else
          {
            calendarButton.IsVisible = true;
            calendarButton.IsEnabled = true;
          }
          calendarButton.IsInactive = num2 < decade || num2 > decadeEnd;
          calendarButton.RequestUpdateVisualState();
        }
        else
        {
          calendarButton.BindingContext = null;
          calendarButton.IsEnabled = false;
          calendarButton.IsVisible = false;
        }

        num++;
      }
    }

    #endregion

    private static bool IsSelectionChanged( ValueChangedEventArgs<System.Collections.ObjectModel.Collection<DateOnly>> e )
    {
      if( e.NewValue.Count != e.OldValue.Count )
        return true;

      foreach( DateOnly addedItem in e.NewValue )
      {
        if( !e.OldValue.Contains( addedItem ) )
          return true;
      }

      return false;
    }

    private DateOnly? GetDateOffset( DateOnly date, int offset, CalendarMode displayMode )
    {
      DateOnly? dateTime = null;
      switch( displayMode )
      {
        case CalendarMode.Month: return DateTimeHelper.AddMonths( date, offset );
        case CalendarMode.Year: return DateTimeHelper.AddYears( date, offset );
        case CalendarMode.Decade: return DateTimeHelper.AddYears( this.DisplayedDate, offset * YEARS_PER_DECADE );
        default: return dateTime;
      }
    }

    private void MoveViewToDate( DateOnly? date )
    {
      if( date.HasValue )
      {
        var date2 = date.Value;
        switch( this.DisplayMode )
        {
          case CalendarMode.Month:
            {
              this.DisplayedDate = new DateOnly( date2.Year, date2.Month, 1 );
              this.CurrentDateInternal = date2;
              this.UpdateViews();
            }
            break;
          case CalendarMode.Year:
          case CalendarMode.Decade:
            {
              this.DisplayedDate = date2;
              this.UpdateViews();
            }
            break;
        }
      }
    }

    private void CoerceFromSelection()
    {
      this.CoerceValue( Calendar.FirstVisibleDateProperty );
      this.CoerceValue( Calendar.LastVisibleDateProperty );
      this.CoerceValue( Calendar.DisplayedDateProperty );
    }

    #endregion

    #region Events

    public event EventHandler<ValueChangedEventArgs<Collection<DateOnly>>> SelectedDatesChanged;

    internal void RaiseSelectedDatesChanged( object sender, ValueChangedEventArgs<Collection<DateOnly>> e )
    {
      if( this.IsEnabled )
      {
        this.SelectedDatesChanged?.Invoke( sender, e );
      }
    }

    public event EventHandler<ValueChangedEventArgs<DateOnly>> DisplayedDateChanged;

    internal void RaiseDisplayedDateChanged( object sender, ValueChangedEventArgs<DateOnly> e )
    {
      if( this.IsEnabled )
      {
        this.DisplayedDateChanged?.Invoke( sender, e );
      }
    }

    public event EventHandler<ValueChangedEventArgs<CalendarMode>> DisplayModeChanged;

    internal void RaiseDisplayModeChanged( object sender, ValueChangedEventArgs<CalendarMode> e )
    {
      if( this.IsEnabled )
      {
        this.DisplayModeChanged?.Invoke( sender, e );
      }
    }

    public event EventHandler<EventArgs> SelectionModeChanged;

    internal void RaiseSelectionModeChanged( object sender, EventArgs e )
    {
      if( this.IsEnabled )
      {
        this.SelectionModeChanged?.Invoke( sender, e );
      }
    }
    #endregion

    #region Events Handlers

    private void BlackoutDates_CollectionChanged( object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e )
    {
      if( (m_monthView != null) && (m_monthView.Children.Count == ( COLS * ROWS ) ) )
      {
        this.UpdateViews();
      }
    }

    private void BlackoutDates_CalendarDateRangeChanged( object sender, EventArgs e )
    {
      if( ( m_monthView != null ) && ( m_monthView.Children.Count == ( COLS * ROWS ) ) )
      {
        this.UpdateViews();
      }
    }

    private void SelectedDates_SelectedDatesChanged( object sender, ValueChangedEventArgs<Collection<DateOnly>> e )
    {
      this.OnSelectedDatesCollectionChanged( e );
    }

    private void SelectedDates_CollectionChanged( object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e )
    {
      if( e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add )
      {
        if( e.NewItems[ 0 ] is DateOnly )
        {
          if( ( e.NewStartingIndex == 0 )
          && ( !this.SelectedDate.HasValue || ( this.SelectedDate.Value != (DateOnly)e.NewItems[ 0 ] ) ) )
          {
            this.SelectedDate = (DateOnly)e.NewItems[ 0 ];
          }

          if( !this.SelectedDates.IsAddingRange )
          {
            var num = DateTimeHelper.CompareYearAndMonth( (DateOnly)e.NewItems[ 0 ], this.DisplayedDateInternal );
            if( ( num < 2 ) && ( num > -2 ) )
            {
              this.UpdateViews();
            }
          }
        }
      }
      else if( e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove )
      {
        if( e.OldStartingIndex == 0 )
        {
          this.SelectedDate = ( this.SelectedDates.Count > 0 ) ? this.SelectedDates[ 0 ] : null;
        }

        if( e.OldItems[ 0 ] is DateOnly )
        {
          var num = DateTimeHelper.CompareYearAndMonth( (DateOnly)e.OldItems[ 0 ], this.DisplayedDateInternal );
          if( ( num < 2 ) && ( num > -2 ) )
          {
            this.UpdateViews();
          }
        }
      }
    }

    private void SelectedDates_ItemsCleared( object sender, EventArgs e )
    {
      if( this.SelectedDate.HasValue )
      {
        this.SelectedDate = null;
      }
      this.DateRangeStart = null;
      this.UpdateViews();
    }

    private void Calendar_Loaded( object sender, EventArgs e )
    {
      m_isMovingThroughMonths = true;

      var dateOffset = this.GetDateOffset( this.DisplayedDate, 0, this.DisplayMode );
      if( dateOffset.HasValue )
      {
        switch( this.DisplayMode )
        {
          case CalendarMode.Month:
            {
              this.CurrentDateInternal = dateOffset.Value;
              this.UpdateViews();
            }
            break;
          case CalendarMode.Year:
          case CalendarMode.Decade:
            {
              this.DisplayedDate = dateOffset.Value;
              this.UpdateViews();
            }
            break;
        }
      }

      m_isMovingThroughMonths = false;
    }

    private void PreviousButton_Click( object sender, EventArgs e )
    {
      this.OnPreviousClick();
    }

    private void HeaderButton_Click( object sender, EventArgs e )
    {
      this.OnHeaderClick();
    }

    private void NextButton_Click( object sender, EventArgs e )
    {
      this.OnNextClick();
    }

    private void CalendarDayButton_Clicked( object sender, EventArgs e )
    {
      if( this.IsEnabled )
      {
        var btn = ( CalendarDayButton )sender;
        if( btn.IsBlackedOut )
          return;

        this.OnDayClick( ( (DateOnly)btn.BindingContext ) );
      }
    }

    private void CalendarButton_Clicked( object sender, EventArgs e )
    {
      if( this.IsEnabled )
      {
        var calendarButton = ( CalendarButton )sender;

        if( calendarButton != null )
        {
          this.OnCalendarButtonPressed( calendarButton, switchDisplayMode: true );
        }
      }
    }

    #endregion
  }
}
