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
  public partial class PopupHandler
  {
    public static IPropertyMapper<IPopup, PopupHandler> PopUpMapper = new PropertyMapper<IPopup, PopupHandler>( ViewHandler.ElementMapper )
    {
      [ nameof( IPopup.IsModal ) ] = PopupHandler.MapIsModal,
      [ nameof( IPopup.Anchor ) ] = PopupHandler.MapAnchor
    };

    public static CommandMapper<IPopup, PopupHandler> PopUpCommandMapper = new( ViewHandler.ElementCommandMapper )
    {
      [ nameof( IPopup.Close ) ] = PopupHandler.MapClose,
      [ nameof( IPopup.Open ) ] = PopupHandler.MapOpen,
      [ nameof( IPopup.UpdateSize ) ] = PopupHandler.MapUpdateSize
    };

    public PopupHandler( IPropertyMapper? mapper, CommandMapper? commandMapper )
      : base( mapper ?? PopupHandler.PopUpMapper, commandMapper ?? PopupHandler.PopUpCommandMapper )
    {
    }

    public PopupHandler()
      : base( PopupHandler.PopUpMapper, PopupHandler.PopUpCommandMapper )
    {
    }

    internal static IElement GetParent( IPopup popup )
    {
      var parent = ( popup?.Parent is PopupContainer ) ? popup?.Parent?.Parent : popup?.Parent;
      if( parent == null )
      {
        parent = Application.Current?.MainPage;
      }

      if( parent == null )
        throw new InvalidDataException( "Popup's Parent can't be found." );

      return parent;
    }
  }
}
