using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Zaone_Incubator_Management_System.Database;
using Zaone_Incubator_Management_System.Model;
using Rg.Plugins.Popup.Services;

namespace Zaone_Incubator_Management_System.ViewModel
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IncubatorDashboard : ContentPage
    {
        DatabaseConnector databaseConnector = new DatabaseConnector();
        private bool emailSent = false;
        private Farmer _farmer;
        string email;

        public IncubatorDashboard()
        {
            InitializeComponent();
            LoadDataFromFirebaseAsync();
        }

        public void SetFarmerEmail(Farmer farmer)
        {
            _farmer = farmer;
            email = _farmer.Email;
        }

        private async void LoadDataFromFirebaseAsync()
        {
            try
            {
                loadingIndicator.IsRunning = true;
                loadingIndicator.IsVisible = true;
                loadingLabel.IsVisible = true;

                var eggBatchDataList = await databaseConnector.GetAllEggBatchDataAsync();
                var stackLayout = new StackLayout();

                var titleLabel = new Label
                {
                    Text = "ACTIVE INCUBATORS",
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

                    // Add edit and delete buttons
                    var editButton = new ImageButton
                    {
                        Source = "del3.png", // Replace with your edit button icon
                        WidthRequest = 30,
                        CornerRadius = 15,
                        VerticalOptions = LayoutOptions.CenterAndExpand,
                    };

                    var deleteButton = new ImageButton
                    {
                        Source = "edit3.png", // Replace with your delete button icon
                        WidthRequest = 30,
                        CornerRadius = 15,
                        VerticalOptions = LayoutOptions.CenterAndExpand,
                    };

                    editButton.Clicked += (sender, e) =>
                    {
                        // Handle edit button click here
                        // For example: await Navigation.PushAsync(new EditPage(IncubatorID, EggBatchID));
                    };

                    deleteButton.Clicked += async (sender, e) =>
                    {
                        // Handle delete button click here
                        var confirmDelete = await DisplayAlert("Confirm Delete", "Are you sure you want to delete this incubator batch?", "Yes", "No");

                        if (confirmDelete)
                        {
                            // Perform delete operation
                            // For example: await databaseConnector.DeleteEggBatchAsync(EggBatchID);
                            // Then reload the data: await LoadDataFromFirebaseAsync();
                        }
                    };

                    var buttonsLayout = new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        HorizontalOptions = LayoutOptions.CenterAndExpand,
                        Spacing = 10,
                    };

                    buttonsLayout.Children.Add(editButton);
                    buttonsLayout.Children.Add(deleteButton);

                    rowLayout.Children.Add(buttonsLayout);

                    // Send email if incubation is complete
                    if (progressPercentage >= 1.0 && !emailSent)
                    {
                        try
                        {
                            MailMessage mail = new MailMessage();
                            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                            mail.From = new MailAddress("mungoshiyacc4@gmail.com");

                            if (!string.IsNullOrEmpty(email))
                            {
                                mail.To.Add(email); // email for registered farmers 
                            }

                            mail.Subject = "Zaone Egg Incubator Management System";
                            mail.Body = $"Incubator : {IncubatorName} Set on : {StartDate} expected to hatch on : {HatchDate} is complete";

                            SmtpServer.Port = 587;
                            SmtpServer.Host = "smtp.gmail.com";
                            SmtpServer.EnableSsl = true;
                            SmtpServer.UseDefaultCredentials = false;
                            SmtpServer.Credentials = new NetworkCredential("mungoshiyacc4@gmail.com", "wsri cbru vkgm nhxq");

                            SmtpServer.Send(mail);

          
                        }
                        catch (Exception ex)
                        {
                            //Handle the exception, e.g., display an error message or log it.
                           //DisplayAlert("Failed", ex.Message, "OK");
                        }
                        emailSent = true;
                    }

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

                        var labelsLayout = new StackLayout
                        {
                            HorizontalOptions = LayoutOptions.CenterAndExpand,
                            VerticalOptions = LayoutOptions.Center,
                            Spacing = 5,
                        };

                        labelsLayout.Children.Add(incubationCompleteLabel);
                        rowLayout.Children.Add(labelsLayout);

                        var tapGesture = new TapGestureRecognizer();
                        tapGesture.Tapped += async (sender, e) =>
                        {
                            await PopupNavigation.PushAsync(new HatchBatchPopUp(IncubatorID, IncubatorName, EggBatchID));
                        };

                        curvedFrame.GestureRecognizers.Add(tapGesture);
                    }
                    else
                    {
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
                loadingIndicator.IsRunning = false;
                loadingIndicator.IsVisible = false;
                loadingLabel.IsVisible = false;
            }
        }
    }
}
