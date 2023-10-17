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
  public class AreaElementInfosBase : INotifyPropertyChanged
  {
    #region Private Members

    private Brush m_background;
    private Point m_location;

    #endregion

    #region Constructors

    public AreaElementInfosBase()
    {
      this.Location = Point.Zero;
    }

    #endregion

    #region Public Properties

    #region Background

    public Brush Background
    {
      get
      {
        return m_background;
      }
      internal set
      {
        m_background = value;
        this.OnPropertyChanged( new PropertyChangedEventArgs( nameof( Background ) ) );
      }
    }

    #endregion

    #region Location

    public Point Location
    {
      get
      {
        return m_location;
      }
      internal set
      {
        m_location = value;
        this.OnPropertyChanged( new PropertyChangedEventArgs( nameof( Location ) ) );
      }
    }

    #endregion

    #endregion


    #region INotifyPropertyChanged Interface

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged( PropertyChangedEventArgs e )
    {
      this.PropertyChanged?.Invoke( this, e );
    }

    #endregion
  }
}
