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
  public class Expander : ContentControl
  {
    #region Private Members

    private ToggleButton m_toggleButton;
    private const string PART_ToggleButton = "PART_ToggleButton";

    #endregion

    #region Public Properties

    #region Direction

    public static readonly BindableProperty DirectionProperty = BindableProperty.Create( nameof( Direction ), typeof( ExpandDirectionEnum ), typeof( Expander ), ExpandDirectionEnum.Down );

    public ExpandDirectionEnum Direction
    {
      get => ( ExpandDirectionEnum )GetValue( DirectionProperty );
      set => SetValue( DirectionProperty, value );
    }

    #endregion

    #region Header

    public static readonly BindableProperty HeaderProperty = BindableProperty.Create( nameof( Header ), typeof( object ), typeof( Expander ) );

    public object Header
    {
      get => ( object )GetValue( HeaderProperty );
      set => SetValue( HeaderProperty, value );
    }

    #endregion

    #region HeaderStyle

    public static readonly BindableProperty HeaderStyleProperty = BindableProperty.Create( nameof( HeaderStyle ), typeof( Style ), typeof( Expander ) );
    public Style HeaderStyle
    {
      get => ( Style )GetValue( HeaderStyleProperty );
      set => SetValue( HeaderStyleProperty, value );
    }

    #endregion

    #region IsExpanded

    public static readonly BindableProperty IsExpandedProperty = BindableProperty.Create( nameof( IsExpanded ), typeof( bool ), typeof( Expander ), false, propertyChanged: OnIsExpandedChanged );

    public bool IsExpanded
    {
      get => ( bool )GetValue( IsExpandedProperty );
      set => SetValue( IsExpandedProperty, value );
    }

    private static void OnIsExpandedChanged( BindableObject bindable, object oldValue, object newValue )
    {
      if( bindable is Expander toggle )
      {
        toggle.OnIsExpandedChanged( ( bool )oldValue, ( bool )newValue );
      }
    }

    protected virtual void OnIsExpandedChanged( bool oldValue, bool newValue )
    {
      this.RaiseExpandedChangedEvent( this, new ExpandedEventArgs( newValue ) );
    }

    #endregion

    #endregion

    #region Protected Methods
    protected override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      m_toggleButton = this.GetTemplateChild( PART_ToggleButton ) as ToggleButton;
    }
    #endregion

    #region Internal Methods
    internal void ToggleButton_Pressed( bool isChecked )
    {
      if( this.IsEnabled )
      {
        this.IsExpanded = isChecked;
      }
    }

    internal ToggleButton GetHeader()
    {
      return m_toggleButton;
    }
    #endregion

    #region Events

    public event EventHandler<ExpandedEventArgs> Expanded;

    private void RaiseExpandedChangedEvent( object sender, ExpandedEventArgs e )
    {
      if( this.IsEnabled )
      {
        this.Expanded?.Invoke( sender, e );
      }
    }

    #endregion
  }
}
