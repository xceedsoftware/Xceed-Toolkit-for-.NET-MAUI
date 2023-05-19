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
  // All the code in this file is only included on Windows.
  public partial class Button
  {
    #region Priate Members

    private const string VisualState_PointerOver = "PointerOver";

    #endregion

    #region Public Properties

    #region IsPointerOver

    public static readonly BindableProperty IsPointerOverProperty = BindableProperty.Create( nameof( IsPointerOver ), typeof( bool ), typeof( Button ), false, propertyChanged: OnIsPointerOverChanged );

    public bool IsPointerOver
    {
      get => ( bool )GetValue( IsPointerOverProperty );
      private set => SetValue( IsPointerOverProperty, value );
    }

    private static void OnIsPointerOverChanged( BindableObject bindable, object oldValue, object newValue )
    {
      var button = bindable as Button;
      if( button != null )
      {
        button.OnIsPointerOverChanged( ( bool )oldValue, ( bool )newValue );
      }
    }

    protected virtual void OnIsPointerOverChanged( bool oldValue, bool newValue )
    {
      if( newValue )
      {
        this.RaisePointerEnterEvent( this, EventArgs.Empty );
      }
      else
      {
        this.RaisePointerLeaveEvent( this, EventArgs.Empty );
      }
    }

    #endregion

    #endregion

    #region Partial Methods

    partial void InitializeForPlatform( Border oldBorder, Border newBorder )
    {
      if( oldBorder != null )
      {
        oldBorder.PointerEnter -= this.Border_PointerEnter;
        oldBorder.PointerLeave -= this.Border_PointerLeave;
      }

      if( m_border != null )
      {
        m_border.PointerEnter += this.Border_PointerEnter;
        m_border.PointerLeave += this.Border_PointerLeave;
      }
    }

    partial void SetVisualStateAfterUnPressed()
    {
      VisualStateManager.GoToState( this, this.IsPointerOver ? VisualStateManager.CommonStates.PointerOver : VisualStateManager.CommonStates.Normal );
    }

    #endregion

    #region Event Handlers

    private void Border_PointerEnter( object sender, EventArgs e )
    {
      this.IsPointerOver = true;
    }

    private void Border_PointerLeave( object sender, EventArgs e )
    {
      this.IsPressed = false;
      this.IsPointerOver = false;
    }

    #endregion

    #region Events

    public event EventHandler PointerEnter;

    public void RaisePointerEnterEvent( object sender, EventArgs e )
    {
      this.PointerEnter?.Invoke( sender, e );
    }

    public event EventHandler PointerLeave;

    public void RaisePointerLeaveEvent( object sender, EventArgs e )
    {
      this.PointerLeave?.Invoke( sender, e );
    }

    #endregion
  }
}
