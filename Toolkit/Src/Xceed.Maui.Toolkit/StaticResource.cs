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
  [ContentProperty( "KeyOrType" )]
  public class StaticResource : IMarkupExtension
  {
    #region Private Members

    private readonly StaticResourceExtension m_defaultStaticResource = new StaticResourceExtension();

    #endregion

    #region Public Properties

    #region KeyOrType

    public object KeyOrType 
    { 
      get; 
      set; 
    }

    #endregion

    #endregion

    #region IMarkupExtension Methods

    public object ProvideValue( IServiceProvider serviceProvider )
    {
      // When a type is set, the m_defaultStaticResource's key will be the full name of the Type.
      // When a key is set, use it like normally.
      m_defaultStaticResource.Key = this.KeyOrType is Type
                                    ? (( Type )this.KeyOrType).FullName 
                                    : this.KeyOrType is string ? (string)this.KeyOrType : null;    

      return m_defaultStaticResource.ProvideValue( serviceProvider );
    }

    #endregion
  }
}
