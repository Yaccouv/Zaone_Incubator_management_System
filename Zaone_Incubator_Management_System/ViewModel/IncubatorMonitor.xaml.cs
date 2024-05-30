using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;
using Zaone_Incubator_Management_System.Database;
using Zaone_Incubator_Management_System.Model;

namespace Zaone_Incubator_Management_System.ViewModel
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IncubatorMonitor : ContentPage
    {
        private FirebaseClient firebase;
        private DateTime currentDateWithoutTime;
        private double temperatureData; // Declare a class-level variable to store temperatureData
        private double humidityData;
        int seconds = 0;
        string timeq = "";

        private readonly TemperatureViewModel viewModel;

        // Add a private field to store the IncubatorID
        private readonly int incubatorID;

        public IncubatorMonitor(int IncubatorID, DateTime StartDate, DateTime HatchDate, string IncubatorNamee)
        {
            InitializeComponent();

            viewModel = new TemperatureViewModel();
            BindingContext = viewModel;

            // Store the IncubatorID in the private field
            incubatorID = IncubatorID;

            lblname.Text = "Incubator Name:  " + IncubatorNamee;

            firebase = new FirebaseClient("https://xamarinfirebase-4fbd5-default-rtdb.firebaseio.com/");

            // Initialize currentDateWithoutTime to the current date with the time set to midnight
            currentDateWithoutTime = DateTime.Now.Date;

            // Start a timer to fetch data every 2 seconds
            Device.StartTimer(TimeSpan.FromSeconds(2), () =>
            {
                FetchDataFromFirebase();
                return true; // Repeat the timer
            });

            // Start a timer to insert data every 1 minute
            Device.StartTimer(TimeSpan.FromMinutes(2), () =>
            {
                InsertTemperatureAndHumidityData();
                return true; // Repeat the timer
            });


        }

        private async void OnSaveSensorDataClicked(object sender, EventArgs e)
        {
            // You can use the incubatorID field here
            int savedIncubatorID = incubatorID;

            // Pass savedIncubatorID to SaveSensorDataAsync method
            await viewModel.SaveTemparatureHumidityRotation(savedIncubatorID, seconds);

            await DisplayAlert("Success", "Incubator is set Successfully", "OK");
        }

        private async Task FetchDataFromFirebase()
        {
            try
            {
                temperatureData = await firebase
                    .Child("temperature")
                    .OnceSingleAsync<double>();

                humidityData = await firebase
                    .Child("humidity")
                    .OnceSingleAsync<double>();

                // Retrieve the saved temperature and humidity data from Firebase
                var savedTemperatureData = await firebase
                    .Child("Sensor")
                    .Child("yaccouv")
                    .Child("TemperatureValue")
                    .OnceSingleAsync<double>();

                var savedHumidityData = await firebase
                    .Child("Sensor")
                    .Child("yaccouv")
                    .Child("HumidityValue")
                    .OnceSingleAsync<double>();

                var savedTimerData = await firebase
                    .Child("Sensor")
                    .Child("yaccouv")
                    .Child("EggRotationTimer")
                    .OnceSingleAsync<int>();

                // Update the labels with the fetched data
                TemperatureLabel.Text = "Temp: " + temperatureData + "°C".ToString();
                HumidityLabel.Text = "Hum: " + humidityData + "%".ToString();

                //Update the labels with the fetched data from saved data
                SavedTemperatureLabel.Text = savedTemperatureData + "°C".ToString();
                SavedHumidityLabel.Text = savedHumidityData + "%".ToString();
                
                if (savedTimerData == 30000)
                {
                    timeq = "30 Seconds";
                }
                else if (savedTimerData == 60000)
                {
                    timeq = "1 Minute";
                }
                else if (savedTimerData == 120000)
                {
                    timeq = "2 Minutes";
                }

                SavedRotationTimeLabel.Text = timeq.ToString();
   

                // Compare the retrieved temperature data with the saved temperature
                if (temperatureData < savedTemperatureData)
                {
                    HeatingElementLabel.Text = "On";
                    HeatingElementLabel.TextColor = Color.White;
                    HeatingElementFrame.BackgroundColor = Color.FromRgb(250, 186, 107);
                }
                else
                {
                    HeatingElementLabel.Text = "Off";
                    HeatingElementFrame.BackgroundColor = Color.FromRgb(229, 240, 229);
                }

                // Compare the retrieved humidity data with the saved humidity
                if (humidityData > savedHumidityData)
                {
                    BlowerLabel.Text = "On";
                    BlowerLabel.TextColor = Color.White;
                    BlowerFrame.BackgroundColor = Color.FromRgb(250, 186, 107);
                }
                else
                {
                    BlowerLabel.Text = "Off";
                    BlowerFrame.BackgroundColor = Color.FromRgb(229, 240, 229);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions here
                Console.WriteLine("Exception: " + ex.Message);
            }
        }

        private async void InsertTemperatureAndHumidityData()
        {
            try
            {
                // Get the current temperature and humidity labels' text
                string temperatureText = temperatureData.ToString();
                string humidityText = humidityData.ToString();


                
                var temperatureDataEntry = new TemperatureData
                {
                    Date = currentDateWithoutTime,
                    Temperature = temperatureText
                };

                var humidityDataEntry = new HumidityData
                {
                    Date = currentDateWithoutTime,
                    Humidity = humidityText
                };

                // Push the data entries to Firebase under their respective tables
                await firebase.Child("TemperatureData").PostAsync(temperatureDataEntry);
                await firebase.Child("HumidityData").PostAsync(humidityDataEntry);

                // Increment currentDateWithoutTime by 1 minute
                currentDateWithoutTime = currentDateWithoutTime.AddMinutes(1);
            }
            catch (Exception ex)
            {
                // Handle exceptions here
                Console.WriteLine("Exception: " + ex.Message);
            }
        }


        private async void OnTimerPickerSelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedText = TimerPicker.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(selectedText))
                return;

            // Update the timer interval based on the selected option


            if (selectedText == "30 seconds")
            {
                seconds = 30000;
            }
            else if (selectedText == "1 minute")
            {
                seconds = 60000;
            }
            else if (selectedText == "2 minutes")
            {
                seconds = 120000;
            }
        }
    }
}
