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


namespace Xceed.Maui.Toolkit.LiveExplorer
{
  public partial class MainPage : ContentPage
  {
    private readonly List<Color> m_fluentColors = new List<Color>
    {
         Color.FromArgb("#0078D4"),
         Color.FromArgb("#2D7D9A"),
         Color.FromArgb("#744DA9"),
         Color.FromArgb("#018574"),
         Color.FromArgb("#CA5010"),
         Color.FromArgb("#6B69D6"),
         Color.FromArgb("#847545"),
         Color.FromArgb("#FF8C00"),
         Color.FromArgb("#107C10"),
         Color.FromArgb("#038387"),
         Color.FromArgb("#00CC6A"),
         Color.FromArgb("#498205"),
         Color.FromArgb("#10893E"),
         Color.FromArgb("#0099BC"),
         Color.FromArgb("#FFB900"),
         Color.FromArgb("#8764B8"),
         Color.FromArgb("#00B294"),
         Color.FromArgb("#F7630C"),
         Color.FromArgb("#C239B3"),
         Color.FromArgb("#EF6950"),
         Color.FromArgb("#E3008C"),
         Color.FromArgb("#8E8CD8"),
         Color.FromArgb("#E74856"),
         Color.FromArgb("#B146C2"),
         Color.FromArgb("#9A0089"),
         Color.FromArgb("#DA3B01"),
         Color.FromArgb("#C30052"),
         Color.FromArgb("#BF0077"),
         Color.FromArgb("#E81123"),
         Color.FromArgb("#D13438"),
         Color.FromArgb("#FF4343"),
         Color.FromArgb("#0063B1"),
         Color.FromArgb("#00B7C3"),
         Color.FromArgb("#881798"),
         Color.FromArgb("#EA005E")
    };

    public MainPage()
    {
      InitializeComponent();
      this.AddBordersForColors();
    }

    private void AddBordersForColors()
    {
      foreach( var color in m_fluentColors )
      {
        Xceed.Maui.Toolkit.Border border = new Xceed.Maui.Toolkit.Border
        {
          HeightRequest = 20,
          WidthRequest = 20,
          CornerRadius = 4,
          Margin = new Thickness( 2 ),
          Background = color
        };

        borderContainer.Children.Add( border );
      }
    }

    private void PlusRepeatButton_Clicked( System.Object sender, System.EventArgs e )
    {
      var value = int.Parse( repeatLabel.Text );
      value++;
      repeatLabel.Text = value.ToString();
    }

    private void MinusRepeatButton_Clicked( System.Object sender, System.EventArgs e )
    {
      var value = int.Parse( repeatLabel.Text );
      value--;
      repeatLabel.Text = value.ToString();
    }
  }
}
