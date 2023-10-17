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
  public class SeriesLineElementInfo : AreaElementInfos
  {
    #region Private Members

    private double m_strokeThickness;

    #endregion

    #region Public Properties

    #region StrokeThickness

    public double StrokeThickness
    {
      get
      {
        return m_strokeThickness;
      }
      internal set
      {
        m_strokeThickness = value;
        this.OnPropertyChanged( new PropertyChangedEventArgs( nameof( StrokeThickness ) ) );
      }
    }

    #endregion

    #endregion
  }
}
