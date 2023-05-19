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
  internal class OnPlatform : IMarkupExtension
  {
    #region Public Properties

    #region Android

    public string Android
    {
      get;
      set;
    }

    #endregion

    #region Default

    public string Default
    {
      get;
      set;
    }

    #endregion

    #region IOS

    public string IOS
    {
      get;
      set;
    }

    #endregion

    #region Mac

    public string Mac
    {
      get;
      set;
    }

    #endregion

    #region WinUI

    public string WinUI
    {
      get;
      set;
    }

    #endregion

    #endregion

    #region IMarkupExtension Methods

    public object ProvideValue( IServiceProvider serviceProvider )
    {
      if( ( DeviceInfo.Platform == DevicePlatform.WinUI ) && ( this.WinUI != null ) )
        return this.WinUI;
      if( ( DeviceInfo.Platform == DevicePlatform.Android ) && ( this.Android != null ) )
        return this.Android;
      if( ( DeviceInfo.Platform == DevicePlatform.MacCatalyst ) && ( this.Mac != null ) )
        return this.Mac;
      if( ( DeviceInfo.Platform == DevicePlatform.iOS ) && ( this.IOS != null ) )
        return this.IOS;

      return this.Default;
    }

    #endregion
  }
}
