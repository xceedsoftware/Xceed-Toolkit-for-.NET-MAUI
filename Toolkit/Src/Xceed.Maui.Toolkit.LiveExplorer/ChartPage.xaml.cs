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


namespace Xceed.Maui.Toolkit.LiveExplorer;

public partial class ChartPage : ContentPage
{
	public ChartPage()
	{
		InitializeComponent();
	}

    Random _random = new Random();
    private void AddPointButton_Clicked(object sender, EventArgs e)
    {
        var randomSeries = _random.Next(0, MyChart.Series.Count);

        // Create a new DataPoint object
        DataPoint myDataPoint = new DataPoint()
        {
            Y = _random.Next(30, 70),
            Text = MyChart.Series[randomSeries].DataPoints.Count.ToString()
        };

        MyChart.Series[randomSeries].DataPoints.Add(myDataPoint);
    }

    private void ChangeChartButton_Clicked(object sender, EventArgs e)
    {
        FirstSeries.Renderer = new BarRenderer();

        //foreach (Series series in MyChart.Series)
        //{
        //    series.Renderer = new BarRenderer();
        //}        
    }

}
