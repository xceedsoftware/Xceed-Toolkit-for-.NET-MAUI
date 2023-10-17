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
  public class DoubleAnimation : TriggerAction<VisualElement>
  {
    #region Properties

    public int Delay { get; set; } = 0;

    public uint Duration { get; set; } = 1000;

    public Easing Easing { get; set; } = Easing.Linear;

    public double From { get; set; }

    public BindableProperty TargetProperty { get; set; } = default( BindableProperty );

    public double To { get; set; }  

    #endregion

    #region Protected Methods

    protected override async void Invoke( VisualElement sender )
    {
      if( this.TargetProperty == null )
        throw new NullReferenceException( "Null Target Property." );

      if( this.Delay > 0 )
        await Task.Delay( this.Delay );

      this.SetDefaultValueFrom( ( double )sender.GetValue( this.TargetProperty ) );

      sender.Animate( $"DoubleAnimation{this.TargetProperty.PropertyName}", new Animation( ( progress ) =>
      {
        sender.SetValue( this.TargetProperty, GetValue( this.From, this.To, progress ) );
      } ),
      length: this.Duration,
      easing: this.Easing );
    }

    protected void SetDefaultValueFrom( double property )
    {
      this.From = ( this.From == 0d ) ? property : this.From;
    }

    #endregion

    #region Internal Methods
    internal static double GetValue( double from, double to, double progress )
    {
      return from + ( to - from ) * progress;
    }

    #endregion
  }
}

