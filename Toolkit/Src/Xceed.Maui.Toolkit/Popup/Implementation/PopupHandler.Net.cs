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


using Microsoft.Maui.Handlers;

#nullable enable

namespace Xceed.Maui.Toolkit
{
  public partial class PopupHandler : ElementHandler<IPopup, object>
  {
    protected override object CreatePlatformElement() => throw new NotSupportedException();

    public static void MapAnchor( PopupHandler handler, IPopup view ) => throw new NotImplementedException();

    public static void MapIsModal( PopupHandler handler, IPopup view ) => throw new NotImplementedException();

    public static void MapClose( PopupHandler handler, IPopup popup, object? args ) => throw new NotImplementedException();

    public static void MapOpen( PopupHandler handler, IPopup popup, object? args ) => throw new NotImplementedException();

    public static void MapUpdateSize( PopupHandler handler, IPopup popup, object? result ) => throw new NotImplementedException();
  }
}
