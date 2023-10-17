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
  public partial class AreaElement : ContentControl
  {
    #region Constructors

    public AreaElement()
    {
      if( this.Infos == null )
      {
        this.SetInfos();
      }

      this.Loaded += this.AreaElement_Loaded;
    }

    #endregion

    #region Public Properties

    #region Infos

    public AreaElementInfosBase Infos
    {
      get;
      internal set;
    }

    #endregion

    #endregion

    #region Partial Methods

    partial void SetShapeSize();

    #endregion

    #region Internal Methods

    protected override void OnApplyTemplate()
    {
      if( this.Infos == null )
      {
        this.SetInfos();
      }

      base.OnApplyTemplate();
    }

    internal virtual void SetInfos()
    {
      this.Infos = new AreaElementInfos();
    }

    internal virtual void SetLocation( Rect bounds, Point offset )
    { 
    }

    internal virtual void SetLocation( Point offset )
    {
    }

    #endregion

    #region Event Handlers

    private void AreaElement_Loaded( object sender, EventArgs e )
    {
      this.SetShapeSize();
    }

    #endregion
  }
}
