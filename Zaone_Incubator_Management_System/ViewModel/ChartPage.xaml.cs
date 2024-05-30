using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zaone_Incubator_Management_System.Model;
using Firebase.Database;
using Firebase.Database.Query;

namespace Zaone_Incubator_Management_System.ViewModel
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChartPage : ContentPage
    {
        private FirebaseClient firebase;
        private List<ChartData> chartDataList;
        private int hatchedEggs;
        private int unhatchedEggs;
        private int EggBatchID;

        public ChartPage(int IncubatorID, string IncubatorName, int EggBatchID)
        {
            InitializeComponent();
            firebase = new FirebaseClient("https://xamarinfirebase-4fbd5-default-rtdb.firebaseio.com/");
            chartDataList = new List<ChartData>();
            this.EggBatchID = EggBatchID; // Store EggBatchID in a class-level variable
            LoadChartData();

            incubatorN.Text = "Incubator Name: " + IncubatorName;
        }

        private async void LoadChartData()
        {
            try
            {
                loadingIndicator.IsRunning = true;
                loadingIndicator.IsVisible = true;
                loadingLabel.IsVisible = true;
                lineChartFrame.IsVisible = false;
                pieChartFrame.IsVisible = false;
                hatchedeggsFrame.IsVisible = false;
                unhatchedeggsFrame.IsVisible = false;
                totaleggsFrame.IsVisible = false;
                incubatorN.IsVisible = false;
                incubatorNFrame.IsVisible = false;

                // Fetch temperature and humidity data from Firebase (adjust the paths as needed)
                var temperatureData = await firebase
                    .Child("TemperatureData")
                    .OnceAsync<ChartData>();

                var humidityData = await firebase
                    .Child("HumidityData")
                    .OnceAsync<ChartData>();

                // Calculate temperature averages
                var groupedTemperatureData = temperatureData
                    .GroupBy(entry => entry.Object.Date.Date)
                    .Select(group => new
                    {
                        Date = group.Key,
                        Temperature = group.Average(entry => entry.Object.Temperature)
                    });

                // Calculate humidity averages
                var groupedHumidityData = humidityData
                    .GroupBy(entry => entry.Object.Date.Date)
                    .Select(group => new
                    {
                        Date = group.Key,
                        Humidity = group.Average(entry => entry.Object.Humidity)
                    });

                // Merge temperature and humidity data by date
                var mergedData = groupedTemperatureData.Join(
                    groupedHumidityData,
                    temp => temp.Date.Date,
                    humidity => humidity.Date.Date,
                    (temp, humidity) => new ChartData
                    {
                        Date = temp.Date,
                        Temperature = temp.Temperature,
                        Humidity = humidity.Humidity
                    })
                    .OrderBy(entry => entry.Date)
                    .ToList();

                // Filter the data for the last 21 days
                var last21DaysData = mergedData.Skip(Math.Max(0, mergedData.Count() - 21)).ToList();

                // Load the chart data
                chartDataList = last21DaysData;

                // Display the chart
                await DisplayChart();

                // Load egg data after displaying the chart
                await LoadEggData();

                // Hide loader and frames
                loadingIndicator.IsRunning = false;
                loadingIndicator.IsVisible = false;
                loadingLabel.IsVisible = false;
                lineChartFrame.IsVisible = true;
                pieChartFrame.IsVisible = true;
                hatchedeggsFrame.IsVisible = true;
                unhatchedeggsFrame.IsVisible = true;
                totaleggsFrame.IsVisible = true;
                incubatorN.IsVisible = true;
                incubatorNFrame.IsVisible = true;
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Console.WriteLine("Exception: " + ex.Message);

                // Hide loader and frames
                loadingIndicator.IsRunning = false;
                loadingIndicator.IsVisible = false;
                loadingLabel.IsVisible = false;
                lineChartFrame.IsVisible = false;
                pieChartFrame.IsVisible = false;
                hatchedeggsFrame.IsVisible = false;
                unhatchedeggsFrame.IsVisible = false;
                totaleggsFrame.IsVisible = false;
                incubatorN.IsVisible = false;
                incubatorNFrame.IsVisible = false;
            }
        }

        private async Task DisplayChart()
        {
            if (chartDataList != null && chartDataList.Count > 0)
            {
                // Create the plot model for the line chart
                var plotModel = new PlotModel
                {
                    Title = "",
                };
                // Create a category axis for the x-axis (days)
                var dayAxis = new CategoryAxis
                {
                    Position = AxisPosition.Bottom,
                    Title = "Days",
                    Minimum = -0.5, 
                    Maximum = chartDataList.Count - 0.5, 
                    AbsoluteMinimum = 0, 
                    IntervalLength = 2 
                };
                // Populate the day axis with labels 
                for (int i = 0; i < chartDataList.Count; i++)
                {
                    dayAxis.Labels.Add("Day " + (i + 1));
                }
                plotModel.Axes.Add(dayAxis);
                // Create a linear axis for the y-axis (temperature and humidity)
                var yAxis = new LinearAxis
                {
                    Position = AxisPosition.Left, // Position on the left
                    Title = "Temperature / Humidity", // Combined title
                    IsZoomEnabled = true, // Enable zooming
                    IsPanEnabled = true // Enable panning
                };

                plotModel.Axes.Add(yAxis);

                // Create a line series for temperature
                var temperatureSeries = new LineSeries
                {
                    Title = "Temperature", // Legend title for temperature
                    MarkerType = MarkerType.Circle,
                    MarkerSize = 4,
                    MarkerStroke = OxyColor.Parse("#00CED1")
                };

                // Create a line series for humidity
                var humiditySeries = new LineSeries
                {
                    Title = "Humidity", // Legend title for humidity
                    MarkerType = MarkerType.Circle,
                    MarkerSize = 4,
                    MarkerStroke = OxyColor.Parse("#FFA500")
                };

                // Add temperature and humidity data points to their respective series
                for (int i = 0; i < chartDataList.Count; i++)
                {
                    temperatureSeries.Points.Add(new DataPoint(i, chartDataList[i].Temperature));
                    humiditySeries.Points.Add(new DataPoint(i, chartDataList[i].Humidity));
                }

                // Add both temperature and humidity series to the plot model
                plotModel.Series.Add(temperatureSeries);
                plotModel.Series.Add(humiditySeries);

                // Create the legend for the line chart
                var legend = new Legend
                {
                    LegendPosition = LegendPosition.BottomLeft,
                    LegendPlacement = LegendPlacement.Outside,
                    LegendItemSpacing = 10,
                    LegendPadding = 10,
                };

                plotModel.Legends.Add(legend);

                // Set the plot model to the OxyPlotView for the line chart
                oxyPlotViewLineChart.Model = plotModel;
            }
        }

        private async Task LoadEggData()
        {
            try
            {
                // Fetch hatchery data from Firebase where EggBatchID matches
                var hatcheryData = await firebase
                    .Child("Hatchery")
                    .OrderBy("EggBatchID")
                    .EqualTo(EggBatchID)
                    .OnceAsync<Hatchery>();

                if (hatcheryData != null && hatcheryData.Count > 0)
                {
                    // Assuming you expect only one Hatchery object, you can access it by indexing.
                    var firstHatchery = hatcheryData.SingleOrDefault();

                    hatchedEggs = firstHatchery.Object.HatchedEggs;
                    unhatchedEggs = firstHatchery.Object.UnhatchedEggs;

                    // Calculate the total number of eggs
                    int totalEggs = hatchedEggs + unhatchedEggs;

                    // Calculate the percentages
                    double hatchedPercentage = (double)hatchedEggs / totalEggs * 100;
                    double unhatchedPercentage = (double)unhatchedEggs / totalEggs * 100;

                    // Display the data in a pie chart
                    await DisplayPieChart(hatchedPercentage, unhatchedPercentage);

                    // Hide pie chart and labels
                    pieChartFrame.IsVisible = true;
                    hatchedeggsFrame.IsVisible = true;
                    unhatchedeggsFrame.IsVisible = true;
                    totaleggsFrame.IsVisible = true;
                    incubatorN.IsVisible = true;
                    lineChartFrame.IsVisible = true;
                    incubatorNFrame.IsVisible = true;

                }
                else
                {
                    Console.WriteLine($"No Hatchery data found for EggBatchID: {EggBatchID}");
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Console.WriteLine("Exception in LoadEggData: " + ex.Message);

                // Hide pie chart and labels
                pieChartFrame.IsVisible = false;
                hatchedeggsFrame.IsVisible = false;
                unhatchedeggsFrame.IsVisible = false;
                totaleggsFrame.IsVisible = false;
                lineChartFrame.IsVisible = false;
                incubatorN.IsVisible = false;
                incubatorNFrame.IsVisible = false;
            }
        }

        private async Task DisplayPieChart(double hatchedPercentage, double unhatchedPercentage)
        {
            if (hatchedEggs >= 0 && unhatchedEggs >= 0)
            {
                var pieModel = new PlotModel
                {
                    Title = "Egg Hatch Rate",
                };

                var pieSeries = new PieSeries
                {
                    StrokeThickness = 2.0,
                    InsideLabelPosition = 0.5,
                    AngleSpan = 360,
                };

                pieSeries.Slices.Add(new PieSlice("Hatched", hatchedPercentage)
                {
                    IsExploded = true,
                    Fill = OxyColor.Parse("#00CED1")
                });

                pieSeries.Slices.Add(new PieSlice("Unhatched", unhatchedPercentage)
                {
                    IsExploded = true,
                    Fill = OxyColor.Parse("#FFA500")
                });

                pieModel.Series.Add(pieSeries);

                // Set the plot model to the OxyPlotView for the pie chart
                oxyPlotViewPieChart.Model = pieModel;

                int Totaleggs = hatchedEggs + unhatchedEggs;
                hatchedeggslabel.Text = "Total Number of Hatched Eggs: " + hatchedEggs.ToString();
                unhatchedeggslabel.Text = "Total Number of Unhatched Eggs: " + unhatchedEggs.ToString();
                totaleggs.Text = "Total Number of Eggs: " + Totaleggs.ToString();
            }
        }
    }
}
