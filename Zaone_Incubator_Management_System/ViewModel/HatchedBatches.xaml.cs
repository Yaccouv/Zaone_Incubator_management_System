using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Zaone_Incubator_Management_System.Database;
using Rg.Plugins.Popup.Services;
using Xamarin.Essentials;

namespace Zaone_Incubator_Management_System.ViewModel
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HatchedBatches : ContentPage
    {
        DatabaseConnector databaseConnector = new DatabaseConnector();
        public HatchedBatches()
        {
            InitializeComponent();
            LoadDataFromFirebaseAsync();
        }

        private async void LoadDataFromFirebaseAsync()
        {
            try
            {
                loadingIndicator.IsRunning = true;
                loadingIndicator.IsVisible = true;
                loadingLabel.IsVisible = true;

                var eggBatchDataList = await databaseConnector.GetAllEggBatchDataSatus1();
                var stackLayout = new StackLayout();

                var titleLabel = new Label
                {
                    Text = "HATCHED BATCHES" + Environment.NewLine + "Click to check Reports",
                    FontSize = 24,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Color.FromHex("#4CD964"),
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(0, 10, 0, 10),
                };


                var titleFrame = new Frame
                {
                    HasShadow = false,
                    BackgroundColor = Color.FromHex("#E5F0E5"),
                    CornerRadius = 10,
                    Padding = new Thickness(10),
                    Content = titleLabel
                };

                stackLayout.Children.Add(titleFrame);

                foreach (var (EggBatchID, IncubatorID, StartDate, HatchDate, IncubatorName) in eggBatchDataList)
                {
                    var currentDate = DateTime.Now;
                    int remainingDays;
                    double progressPercentage;

                    if (currentDate < HatchDate)
                    {
                        remainingDays = (int)(HatchDate - currentDate).TotalDays;
                        progressPercentage = 1 - (double)remainingDays / (double)(HatchDate - StartDate).TotalDays;
                    }
                    else
                    {
                        // Incubation is complete, set progress to 100%
                        remainingDays = 0;
                        progressPercentage = 1.0;
                    }

                    var progressBar = new CircularProgressBar
                    {
                        Progress = progressPercentage,
                        WidthRequest = 100,
                        HeightRequest = 100,
                        Margin = new Thickness(10),
                    };

                    var incubatorNameLabel = new Label
                    {
                        Text = $"Incubator Name: {IncubatorName}",
                        FontSize = 20,
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Color.FromHex("#4CD964"),
                        HorizontalOptions = LayoutOptions.CenterAndExpand,
                        VerticalOptions = LayoutOptions.Center,
                        Margin = new Thickness(0, 3, 0, 10),
                    };

                    var startDateLabel = new Label
                    {
                        Text = $"Start Date: {StartDate}",
                        FontSize = 16,
                        TextColor = Color.FromHex("#333333"),
                        HorizontalOptions = LayoutOptions.CenterAndExpand,
                        VerticalOptions = LayoutOptions.Center,
                    };

                    var hatchDateLabel = new Label
                    {
                        Text = $"Hatch Date: {HatchDate}",
                        FontSize = 16,
                        TextColor = Color.FromHex("#333333"),
                        HorizontalOptions = LayoutOptions.CenterAndExpand,
                        VerticalOptions = LayoutOptions.Center,
                    };

                    var remainingDaysLabel = new Label
                    {
                        Text = $"Remaining Days: {remainingDays} of 21 Days",
                        FontSize = 16,
                        TextColor = Color.FromHex("#333333"),
                        HorizontalOptions = LayoutOptions.CenterAndExpand,
                        VerticalOptions = LayoutOptions.Center,
                    };

                    var rowLayout = new StackLayout
                    {
                        Spacing = 10,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                    };

                    rowLayout.Children.Add(progressBar);
                    rowLayout.Children.Add(incubatorNameLabel);
                    rowLayout.Children.Add(startDateLabel);
                    rowLayout.Children.Add(hatchDateLabel);
                    rowLayout.Children.Add(remainingDaysLabel);

                    var curvedFrame = new Frame
                    {
                        HasShadow = false,
                        CornerRadius = 10,
                        Padding = new Thickness(10),
                        Margin = new Thickness(10),
                        Content = rowLayout
                    };

                    if (progressPercentage >= 1.0)
                    {
                        curvedFrame.BackgroundColor = Color.FromRgb(250, 186, 107);

                        var incubationCompleteLabel = new Label
                        {
                            Text = "Incubation Complete",
                            FontSize = 16,
                            TextColor = Color.White,
                            HorizontalOptions = LayoutOptions.CenterAndExpand,
                            VerticalOptions = LayoutOptions.Center,
                        };

                        // Create a new StackLayout to hold the labels inside the curved frame
                        var labelsLayout = new StackLayout
                        {
                            HorizontalOptions = LayoutOptions.CenterAndExpand,
                            VerticalOptions = LayoutOptions.Center,
                            Spacing = 5,
                        };

                        // Add the "Incubation Complete" label to the labelsLayout
                        labelsLayout.Children.Add(incubationCompleteLabel);

                        // Add the labelsLayout to the rowLayout
                        rowLayout.Children.Add(labelsLayout);

                        var tapGesture = new TapGestureRecognizer();
                        tapGesture.Tapped += async (sender, e) =>
                        {
                            // Navigate to another page for completed incubators with 100% progress
                            await Navigation.PushAsync(new ChartPage(IncubatorID, IncubatorName, EggBatchID));
                        };

                        curvedFrame.GestureRecognizers.Add(tapGesture);
                    }
                    else
                    {
                        // For incubators with less than 100% progress, navigate to IncubatorMonitor page
                        var tapGesture = new TapGestureRecognizer();
                        tapGesture.Tapped += async (sender, e) =>
                        {
                            await Navigation.PushAsync(new IncubatorMonitor(IncubatorID, StartDate, HatchDate, IncubatorName));
                        };

                        curvedFrame.GestureRecognizers.Add(tapGesture);
                    }
                    Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                    {
                        if (progressPercentage < 1.0)
                        {
                            curvedFrame.BackgroundColor = Color.FromHex("#E5F0E5");
                        }
                        else
                        {
                            curvedFrame.BackgroundColor = (curvedFrame.BackgroundColor == Color.FromRgb(250, 186, 107)) ? Color.FromHex("#E5F0E5") : Color.FromRgb(250, 186, 107);
                        }
                        return true;
                    });

                    stackLayout.Children.Add(curvedFrame);
                }

                var scrollView = new ScrollView
                {
                    Content = stackLayout
                };

                Content = scrollView;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error loading data: {ex.Message}", "OK");
            }
            finally
            {
                // Hide the loader after data retrieval is complete
                loadingIndicator.IsRunning = false;
                loadingIndicator.IsVisible = false;
                loadingLabel.IsVisible = false;
            }
        }
    }
}