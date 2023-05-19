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
  public class RepeatButton : Button
  {
    #region Internal Members

    internal const int DefaultDelay = 750;
    internal const int DefaultInterval = 120;

    #endregion

    #region Private Members

    private CancellationTokenSource m_cancellationToken;

    #endregion

    #region Public Properties

    #region Delay

    public static readonly BindableProperty DelayProperty = BindableProperty.Create( nameof( Delay ), typeof( int ), typeof( RepeatButton ), RepeatButton.DefaultDelay, propertyChanging: OnDelayChanging );

    public int Delay
    {
      get => ( int )GetValue( DelayProperty );
      set => SetValue( DelayProperty, value );
    }

    private static void OnDelayChanging( BindableObject bindable, object oldValue, object newValue )
    {
      var repeatButton = bindable as RepeatButton;
      if( repeatButton != null )
      {
        repeatButton.OnDelayChanging( ( int )oldValue, ( int )newValue );
      }
    }

    protected virtual void OnDelayChanging( int oldValue, int newValue )
    {
      if( newValue < 0 )
        throw new InvalidDataException( "Delay property for RepeatButton must be greater or equal to 0." );
    }

    #endregion

    #region Interval

    public static readonly BindableProperty IntervalProperty = BindableProperty.Create( nameof( Interval ), typeof( int ), typeof( RepeatButton ), RepeatButton.DefaultInterval, propertyChanging: OnIntervalChanging );

    public int Interval
    {
      get => ( int )GetValue( IntervalProperty );
      set => SetValue( IntervalProperty, value );
    }

    private static void OnIntervalChanging( BindableObject bindable, object oldValue, object newValue )
    {
      var repeatButton = bindable as RepeatButton;
      if( repeatButton != null )
      {
        repeatButton.OnIsPressedChanged( ( int )oldValue, ( int )newValue );
      }
    }

    protected virtual void OnIsPressedChanged( int oldValue, int newValue )
    {
      if( newValue < 0 )
        throw new InvalidDataException( "Interval property for RepeatButton must be greater or equal to 0." );
    }

    #endregion

    #endregion

    #region Protected Methods   

    protected internal override void Button_PointerDown()
    {
      if( this.IsEnabled )
      {
        this.IsPressed = true;
      }
    }

    protected internal override void Button_PointerUp()
    {
      this.IsPressed = false;
    }

    protected override void OnIsPressedChanged( bool oldValue, bool newValue )
    {
      base.OnIsPressedChanged( oldValue, newValue );

      if( m_cancellationToken != null )
      {
        m_cancellationToken.Cancel();
      }

      // Newly Pressed.
      if( !oldValue && newValue )
      {
        this.RaiseClickEvent();

        m_cancellationToken = new CancellationTokenSource();

#pragma warning disable CS4014
         this.RaiseClickEventsAfterDelay( m_cancellationToken.Token );
#pragma warning restore CS4014
  }
    }

    #endregion

    #region Private Methods

    private static Task<bool> Wait( int delay, CancellationToken token )
    {
      return Task.Delay( delay, token ).ContinueWith( task => task.Exception == default );
    }

    private async Task RaiseClickEventsAfterDelay( CancellationToken token )
    {
      await RepeatButton.Wait( this.Delay, token );
      await this.RaiseClickEventsAtIntervals( token );
    }

    private async Task RaiseClickEventsAtIntervals( CancellationToken token )
    {
      if( this.IsPressed )
      {
        this.RaiseClickEvent();

        await RepeatButton.Wait( this.Interval, token );
        await this.RaiseClickEventsAtIntervals( token );
      }
    }

    #endregion
  }
}
