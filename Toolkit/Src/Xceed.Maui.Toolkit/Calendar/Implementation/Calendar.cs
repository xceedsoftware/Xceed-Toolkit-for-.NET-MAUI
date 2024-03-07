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

    private System.Globalization.Calendar m_calendar = new GregorianCalendar();

    private Button m_headerButton;

    private Grid m_monthView;

    private Button m_nextButton;

    private Button m_previousButton;

    private Grid m_yearView;

    private bool m_isMovingThroughMonths;

    private DateTime? m_dateRangeStart;

    private DateTime? m_dateRangeEnd;

    private DateTime? m_currentDate;

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

      this.DisplayedDate = DateTime.Today;

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

    public static readonly BindableProperty DisplayedDateProperty = BindableProperty.Create( nameof( DisplayedDate ), typeof( DateTime ), typeof( Calendar ), defaultValue: DateTime.MinValue, propertyChanged: OnDisplayedDateChanged, coerceValue: CoerceDisplayedDate );

    public DateTime DisplayedDate
    {
      get
      {
        return ( DateTime )GetValue( DisplayedDateProperty );
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
        var dateTime = ( DateTime )value;

        if( calendar.FirstVisibleDate.HasValue && ( dateTime < calendar.FirstVisibleDate.Value ) )
          return calendar.FirstVisibleDate.Value;
        else if( calendar.LastVisibleDate.HasValue && ( dateTime > calendar.LastVisibleDate.Value ) )
          return calendar.LastVisibleDate.Value;
      }

      return value;
    }

    private static void OnDisplayedDateChanged( BindableObject bindable, object oldValue, object newValue )
    {
      if( bindable is Calendar calendar )
      {
        calendar.OnDisplayedDateChanged( ( DateTime )oldValue, ( DateTime )newValue );
      }
    }

    protected virtual void OnDisplayedDateChanged( DateTime oldValue, DateTime newValue )
    {
      this.DisplayedDateInternal = new DateTime( newValue.Year, newValue.Month, 1, 0, 0, 0 );
      if( !m_isMovingThroughMonths )
      {
        this.UpdateViews();
      }

      this.RaiseDisplayedDateChanged( this, new ValueChangedEventArgs<DateTime>( oldValue, newValue ) );
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

    public static readonly BindableProperty FirstVisibleDateProperty = BindableProperty.Create( nameof( FirstVisibleDate ), typeof( DateTime? ), typeof( Calendar ), defaultBindingMode: BindingMode.TwoWay, defaultValue: null, propertyChanged: OnFirstVisibleDateChanged, coerceValue: CoerceFirstVisibleDate );

    public DateTime? FirstVisibleDate
    {
      get
      {
        return ( DateTime? )GetValue( FirstVisibleDateProperty );
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
        var minDate = ( DateTime? )value;
        if( minDate.HasValue && calendar.LastVisibleDate.HasValue && ( minDate.Value > calendar.LastVisibleDate.Value ) )
          return calendar.LastVisibleDate;
      }

      return value;
    }

    private static void OnFirstVisibleDateChanged( BindableObject bindable, object oldValue, object newValue )
    {
      if( bindable is Calendar calendar )
      {
        calendar.OnFirstVisibleDateChanged( ( DateTime? )oldValue, ( DateTime? )newValue );
      }
    }

    protected virtual void OnFirstVisibleDateChanged( DateTime? oldValue, DateTime? newValue )
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
      var num = DateTimeHelper.CompareYearAndMonth( this.DisplayedDateInternal, DateTime.Today );
      if( ( num > -2 ) && ( num < 2 ) )
      {
        this.UpdateViews();
      }
    }

    #endregion

    #region LastVisibleDate 

    public static readonly BindableProperty LastVisibleDateProperty = BindableProperty.Create( nameof( LastVisibleDate ), typeof( DateTime? ), typeof( Calendar ), defaultBindingMode: BindingMode.TwoWay, defaultValue: null, propertyChanged: OnLastVisibleDateChanged, coerceValue: CoerceLastVisibleDate );

    public DateTime? LastVisibleDate
    {
      get
      {
        return ( DateTime? )GetValue( LastVisibleDateProperty );
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
        var maxDate = ( DateTime? )value;
        if( maxDate.HasValue && calendar.FirstVisibleDate.HasValue && ( maxDate.Value < calendar.FirstVisibleDate.Value ) )
          return calendar.FirstVisibleDate;
      }
      return value;
    }

    private static void OnLastVisibleDateChanged( BindableObject bindable, object oldValue, object newValue )
    {
      if( bindable is Calendar calendar )
      {
        calendar.OnLastVisibleDateChanged( ( DateTime? )oldValue, ( DateTime? )newValue );
      }
    }

    protected virtual void OnLastVisibleDateChanged( DateTime? oldValue, DateTime? newValue )
    {
      this.CoerceValue( Calendar.FirstVisibleDateProperty );
      this.CoerceValue( Calendar.DisplayedDateProperty );
      this.CoerceValue( Calendar.SelectedDatesProperty );
      this.UpdateViews();
    }

    #endregion

    #region SelectedDate

    public static readonly BindableProperty SelectedDateProperty = BindableProperty.Create( nameof( SelectedDate ), typeof( DateTime? ), typeof( Calendar ), defaultValue: null, defaultBindingMode: BindingMode.TwoWay, propertyChanged: OnSelectedDateChanged );

    public DateTime? SelectedDate
    {
      get
      {
        return ( DateTime? )GetValue( SelectedDateProperty );
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
        calendar.OnSelectedDateChanged( ( DateTime? )oldValue, ( DateTime? )newValue );
      }
    }

    protected virtual void OnSelectedDateChanged( DateTime? oldValue, DateTime? newValue )
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
                Collection<DateTime> addedItems = [];
                this.OnSelectedDatesCollectionChanged( new ValueChangedEventArgs<Collection<DateTime>>( this.SelectedDates.OldItems, addedItems ) );

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
            if( DateTime.Compare( calendar.FirstVisibleDate.Value, date ) > 0 )
              throw new InvalidDataException( $"The value {date.Date} is invalid as SelectedDate property." );
          }
          if( calendar.LastVisibleDate.HasValue )
          {
            if( DateTime.Compare( calendar.LastVisibleDate.Value, date ) < 0 )
              throw new InvalidDataException( $"The value {date.Date} is invalid as SelectedDate property." );
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
        Collection<DateTime> addedItems = [];
        this.OnSelectedDatesCollectionChanged( new ValueChangedEventArgs<Collection<DateTime>>( this.SelectedDates.OldItems, addedItems ) );
        this.SelectedDates.OldItems.Clear();
      }
      this.UpdateViews();
      this.RaiseSelectionModeChanged( this, EventArgs.Empty );
    }

    #endregion

    #endregion

    #region Internal Properties

    internal DateTime DisplayedDateInternal { get; private set; }

    internal DateTime LastVisibleDateInternal => this.LastVisibleDate.GetValueOrDefault( DateTime.MaxValue );

    internal DateTime FirstVisibleDateInternal => this.FirstVisibleDate.GetValueOrDefault( DateTime.MinValue );

    internal DateTime CurrentDateInternal
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

    internal DateTime? DateRangeStart
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

    internal DateTime? DateRangeEnd
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
      var decadeForDecadeMode = new DateTime( this.DisplayedDate.Year, 1, 1 ).Year - new DateTime( this.DisplayedDate.Year, 1, 1 ).Year % 10;
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

    internal static bool IsValidDateSelection( Calendar cal, DateTime? value )
    {
      if( value.HasValue )
      {
        var flag = true;
        if( cal.BlackoutDates.Contains( value.Value )
          || ( cal.FirstVisibleDate != null && DateTime.Compare( value.Value, cal.FirstVisibleDate.Value ) < 0 )
          || ( cal.LastVisibleDate != null && DateTime.Compare( value.Value, cal.LastVisibleDate.Value ) > 0 ) )
        {
          flag = false;
        }
        return flag;
      }

      return true;
    }

    internal void OnDayClick( DateTime selectedDate )
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
                  if( !this.SelectedDate.HasValue
                      || DateTime.Compare( this.SelectedDate.Value.Date, selectedDate.Date ) != 0 )
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
                  this.SelectedDates.NewItems = new Collection<DateTime>( this.SelectedDates.ToList() );
                  this.SelectedDates.OldItems = new Collection<DateTime>( this.SelectedDates.ToList() );
                  this.CurrentDateInternal = selectedDate;

                  foreach( var item in CalendarSelectedDatesCollection.GetDaysInRange( this.DateRangeStart.Value, selectedDate ) )
                  {
                    if( DateTime.Compare( item, this.DateRangeStart.Value ) != 0 )
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
      if( b.Content is DateTime yearMonth )
      {
        DateTime? dateTime = null;
        var calendarMode = CalendarMode.Month;

        switch( this.DisplayMode )
        {
          case CalendarMode.Year:
            {
              dateTime = DateTimeHelper.SetYearMonth( this.DisplayedDate, yearMonth );
              calendarMode = CalendarMode.Month;
            }
            break;
          case CalendarMode.Decade:
            {
              dateTime = DateTimeHelper.AddYears( this.DisplayedDate, yearMonth.Year - this.DisplayedDate.Year );
              calendarMode = CalendarMode.Year;
            }
            break;
        }

        if( dateTime.HasValue )
        {
          this.DisplayedDate = dateTime.Value;
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
          this.MoveViewToDate( new DateTime( dateOffset.Value.Year, dateOffset.Value.Month, 1, 0, 0, 0 ) );
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
          this.MoveViewToDate( new DateTime( dateOffset.Value.Year, dateOffset.Value.Month, 1, 0, 0, 0 ) );
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

    internal void OnSelectedDatesCollectionChanged( ValueChangedEventArgs<Collection<DateTime>> e )
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
        var dt = new DateTime( this.DisplayedDate.Year, this.DisplayedDate.Month, 1, 0, 0, 0 );
        m_previousButton.IsEnabled = DateTime.Compare( this.FirstVisibleDateInternal.Date, dt.Date ) < 0;
      }
    }

    private void RefreshMonthViewNextButton()
    {
      if( m_nextButton != null )
      {
        var dateTime = new DateTime( this.DisplayedDate.Year, this.DisplayedDate.Month, 1, 0, 0, 0 );
        if( DateTimeHelper.CompareYearAndMonth( dateTime, DateTime.MaxValue ) == 0 )
        {
          m_nextButton.IsEnabled = false;
          return;
        }

        var dt = m_calendar.AddMonths( dateTime, 1 );
        m_nextButton.IsEnabled = DateTime.Compare( this.LastVisibleDateInternal.Date, dt.Date ) > -1;
      }
    }

    private void RefreshMonthViewDayTitles()
    {
      if( m_monthView == null )
        return;

      var shortestDayNames = DateTimeHelper.GetDateFormat( CultureInfo.CurrentCulture ).ShortestDayNames;
      for( var i = 0; i < NUMBER_OF_DAYS_IN_WEEK; i++ )
      {
        var dayLabel = ( Label )m_monthView.Children[ i ];
        dayLabel.BindingContext = shortestDayNames[ ( int )( i + this.FirstDayOfWeek ) % shortestDayNames.Length ];
      }
    }

    internal void RefreshMonthViewCalendarDayButtons()
    {
      var dateTime = new DateTime( this.DisplayedDate.Year, this.DisplayedDate.Month, 1, 0, 0, 0 );
      var numberOfVisibleDaysFromPreviousMonth = this.GetNumberOfVisibleDaysFromPreviousMonth( dateTime );
      var flag = DateTimeHelper.CompareYearAndMonth( dateTime, DateTime.MinValue ) <= 0;
      var flag2 = DateTimeHelper.CompareYearAndMonth( dateTime, DateTime.MaxValue ) >= 0;
      var daysInMonth = m_calendar.GetDaysInMonth( dateTime.Year, dateTime.Month );
      for( var i = NUMBER_OF_DAYS_IN_WEEK; i < COLS * ROWS; i++ )
      {
        var calendarDayButton = m_monthView.Children[ i ] as CalendarDayButton;
        var num2 = i - numberOfVisibleDaysFromPreviousMonth - NUMBER_OF_DAYS_IN_WEEK;
        if( ( !flag || num2 >= 0 ) && ( !flag2 || num2 < daysInMonth ) )
        {
          var dateTime2 = m_calendar.AddDays( dateTime, num2 );
          this.RefreshMonthDayButtonState( calendarDayButton, dateTime2 );
        }
        else
        {
          this.RefreshMonthDayButtonState( calendarDayButton, null );
        }
      }
    }

    private int GetNumberOfVisibleDaysFromPreviousMonth( DateTime firstOfMonth )
    {
      var dayOfWeek = m_calendar.GetDayOfWeek( firstOfMonth );
      int num = ( dayOfWeek - this.FirstDayOfWeek + NUMBER_OF_DAYS_IN_WEEK ) % NUMBER_OF_DAYS_IN_WEEK;
      if( num == 0 )
        return NUMBER_OF_DAYS_IN_WEEK;

      return num;
    }

    private void RefreshMonthDayButtonState( CalendarDayButton childButton, DateTime? dateToAdd )
    {
      if( dateToAdd.HasValue )
      {
        var visibilityFlag = true;
        var enabledFlag = this.IsEnabled;
        if( DateTime.Compare( dateToAdd.Value.Date, this.FirstVisibleDateInternal.Date ) < 0
          || DateTime.Compare( dateToAdd.Value.Date, this.LastVisibleDateInternal.Date ) > 0 )
        {
          visibilityFlag = false;
          enabledFlag = false;
        }
        var selectionFlag = false;
        foreach( var selectedDate in this.SelectedDates )
        {
          selectionFlag |= DateTime.Compare( dateToAdd.Value.Date, selectedDate.Date ) == 0;
        }

        if( childButton.BindingContext == null )
        {
          MonthViewDayModel monthViewModel = new( isBlackOut: this.BlackoutDates.Contains( dateToAdd.Value ),
                                                  isToday: DateTime.Compare( dateToAdd.Value.Date, DateTime.Today.Date ) == 0,
                                                  isInactive: DateTimeHelper.CompareYearAndMonth( dateToAdd.Value, this.DisplayedDateInternal ) != 0,
                                                  isSelected: selectionFlag,
                                                  isVisible: visibilityFlag,
                                                  isEnabled: enabledFlag )
          { Date = dateToAdd.Value };
          childButton.BindingContext = monthViewModel;
        }
        else
        {
          var monthViewModel = childButton.BindingContext as MonthViewDayModel;
          monthViewModel.IsBlackedOut = this.BlackoutDates.Contains( dateToAdd.Value );
          monthViewModel.IsToday = DateTime.Compare( dateToAdd.Value.Date, DateTime.Today.Date ) == 0;
          monthViewModel.IsInactive = DateTimeHelper.CompareYearAndMonth( dateToAdd.Value, this.DisplayedDateInternal ) != 0;
          monthViewModel.IsSelected = selectionFlag;
          monthViewModel.IsElementVisible = visibilityFlag;
          monthViewModel.IsElementEnabled = enabledFlag;
          monthViewModel.Date = dateToAdd.Value;
        }
        childButton.NotifyNeedsVisualStateUpdate();
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
        var dateTime = new DateTime( this.DisplayedDate.Year, num + 1, 1 );
        calendarButton.Content = dateTime;
        calendarButton.HasSelectedDays = DateTimeHelper.CompareYearAndMonth( dateTime, this.DisplayedDateInternal ) == 0;
        if( DateTimeHelper.CompareYearAndMonth( dateTime, this.FirstVisibleDateInternal ) < 0 || DateTimeHelper.CompareYearAndMonth( dateTime, this.LastVisibleDateInternal ) > 0 )
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
        if( ( num2 <= DateTime.MaxValue.Year ) && ( num2 >= DateTime.MinValue.Year ) )
        {
          calendarButton.Content = new DateTime( num2, 1, 1 );
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

    private static bool IsSelectionChanged( ValueChangedEventArgs<System.Collections.ObjectModel.Collection<DateTime>> e )
    {
      if( e.NewValue.Count != e.OldValue.Count )
        return true;

      foreach( DateTime addedItem in e.NewValue )
      {
        if( !e.OldValue.Contains( addedItem ) )
          return true;
      }

      return false;
    }

    private DateTime? GetDateOffset( DateTime date, int offset, CalendarMode displayMode )
    {
      DateTime? dateTime = null;
      switch( displayMode )
      {
        case CalendarMode.Month: return DateTimeHelper.AddMonths( date, offset );
        case CalendarMode.Year: return DateTimeHelper.AddYears( date, offset );
        case CalendarMode.Decade: return DateTimeHelper.AddYears( this.DisplayedDate, offset * YEARS_PER_DECADE );
        default: return dateTime;
      }
    }

    private void MoveViewToDate( DateTime? date )
    {
      if( date.HasValue )
      {
        var date2 = date.Value.Date;
        switch( this.DisplayMode )
        {
          case CalendarMode.Month:
            {
              this.DisplayedDate = new DateTime( date2.Year, date2.Month, 1, 0, 0, 0 );
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

    public event EventHandler<ValueChangedEventArgs<Collection<DateTime>>> SelectedDatesChanged;

    internal void RaiseSelectedDatesChanged( object sender, ValueChangedEventArgs<Collection<DateTime>> e )
    {
      if( this.IsEnabled )
      {
        this.SelectedDatesChanged?.Invoke( sender, e );
      }
    }

    public event EventHandler<ValueChangedEventArgs<DateTime>> DisplayedDateChanged;

    internal void RaiseDisplayedDateChanged( object sender, ValueChangedEventArgs<DateTime> e )
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
      this.UpdateViews();
    }

    private void BlackoutDates_CalendarDateRangeChanged( object sender, EventArgs e )
    {
      this.UpdateViews();
    }

    private void SelectedDates_SelectedDatesChanged( object sender, ValueChangedEventArgs<Collection<DateTime>> e )
    {
      this.OnSelectedDatesCollectionChanged( e );
    }

    private void SelectedDates_CollectionChanged( object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e )
    {
      if( e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add )
      {
        if( e.NewItems[ 0 ] is DateTime )
        {
          if( ( e.NewStartingIndex == 0 )
          && ( !this.SelectedDate.HasValue || DateTime.Compare( this.SelectedDate.Value, (DateTime)e.NewItems[ 0 ] ) != 0 ) )
          {
            this.SelectedDate = (DateTime)e.NewItems[ 0 ];
          }

          if( !this.SelectedDates.IsAddingRange )
          {
            var num = DateTimeHelper.CompareYearAndMonth( (DateTime)e.NewItems[ 0 ], this.DisplayedDateInternal );
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

        if( e.OldItems[ 0 ] is DateTime )
        {
          var num = DateTimeHelper.CompareYearAndMonth( (DateTime)e.OldItems[ 0 ], this.DisplayedDateInternal );
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
              DisplayedDate = dateOffset.Value;
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

        this.OnDayClick( ( ( MonthViewDayModel )btn.BindingContext ).Date );
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
