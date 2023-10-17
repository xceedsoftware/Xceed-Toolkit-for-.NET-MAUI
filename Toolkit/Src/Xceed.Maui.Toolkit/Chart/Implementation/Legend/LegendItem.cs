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
  internal class LegendItem : INotifyPropertyChanged
  {
    #region Private Members

    private string m_text;
    private Brush m_background;

    #endregion

    #region Constructors

    internal LegendItem( string text, Brush background )
    {
      this.Text = text;
      this.Background = background;
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
      set
      {
        if( m_background != value )
        {
          m_background = value;

          this.OnPropertyChanged( "Background" );
        }
      }
    }

    #endregion

    #region Text

    public string Text
    {
      get
      {
        return m_text;
      }
      set
      {
        if( m_text != value )
        {
          m_text = value;

          this.OnPropertyChanged( "Text" );
        }
      }
    }

    #endregion

    #endregion

    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged( string name )
    {
      this.PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( name ) );
    }

    #endregion
  }
}
