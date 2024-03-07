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


using System.ComponentModel;

namespace Xceed.Maui.Toolkit
{
  internal class MonthViewDayModel : INotifyPropertyChanged
  {
    #region Private Members

    private DateTime m_date;
    private bool m_isBlackedOut;
    private bool m_isToday;
    private bool m_isInactive;
    private bool m_isSelected;
    private bool m_isElementVisible;
    private bool m_isElementEnabled;
    private string m_dateOfMonth;

    #endregion

    #region Contructors

    public MonthViewDayModel( bool isBlackOut, bool isToday, bool isInactive, bool isSelected, bool isVisible, bool isEnabled )
    {
      m_isBlackedOut = isBlackOut;
      m_isToday = isToday;
      m_isInactive = isInactive;
      m_isSelected = isSelected;
      m_isElementVisible = isVisible;
      m_isElementEnabled = isEnabled;
    }

    #endregion

    #region Public Properties

    #region Date

    public DateTime Date
    {
      get
      {
        return m_date;
      }
      set
      {
        m_date = value;
        this.DayOfMonth = m_date.ToString( "%d" );
        this.OnPropertyChanged( new PropertyChangedEventArgs( nameof( Date ) ) );
      }
    }

    #endregion

    #region DayOfMonth

    public string DayOfMonth
    {
      get
      {
        return m_dateOfMonth;
      }
      private set
      {
        m_dateOfMonth = value;
        this.OnPropertyChanged( new PropertyChangedEventArgs( nameof( DayOfMonth ) ) );
      }
    }

    #endregion    

    #region IsBlackedOut

    public bool IsBlackedOut
    {
      get
      {
        return m_isBlackedOut;
      }
      set
      {
        m_isBlackedOut = value;
        this.OnPropertyChanged( new PropertyChangedEventArgs( nameof( IsBlackedOut ) ) );
      }
    }

    #endregion

    #region IsElementEnabled

    public bool IsElementEnabled
    {
      get
      {
        return m_isElementEnabled;
      }
      set
      {
        m_isElementEnabled = value;
        this.OnPropertyChanged( new PropertyChangedEventArgs( nameof( IsElementEnabled ) ) );
      }
    }

    #endregion

    #region IsElementVisible

    public bool IsElementVisible
    {
      get
      {
        return m_isElementVisible;
      }
      set
      {
        m_isElementVisible = value;
        this.OnPropertyChanged( new PropertyChangedEventArgs( nameof( IsElementVisible ) ) );
      }
    }

    #endregion

    #region IsInactive

    public bool IsInactive
    {
      get
      {
        return m_isInactive;
      }
      set
      {
        m_isInactive = value;
        this.OnPropertyChanged( new PropertyChangedEventArgs( nameof( IsInactive ) ) );
      }
    }

    #endregion

    #region IsSelected

    public bool IsSelected
    {
      get
      {
        return m_isSelected;
      }
      set
      {
        m_isSelected = value;
        this.OnPropertyChanged( new PropertyChangedEventArgs( nameof( IsSelected ) ) );
      }
    }

    #endregion

    #region IsToday

    public bool IsToday
    {
      get
      {
        return m_isToday;
      }
      set
      {
        m_isToday = value;
        this.OnPropertyChanged( new PropertyChangedEventArgs( nameof( IsToday ) ) );
      }
    }

    #endregion   

    #endregion

    #region Events

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion

    #region Events Handlers

    private void OnPropertyChanged( PropertyChangedEventArgs e )
    {
      this.PropertyChanged?.Invoke( this, e );
    }

    #endregion
  }
}
