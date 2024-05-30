using System;
using Xamarin.Forms;
using System.Net.Http;
using System.Threading.Tasks;

namespace Zaone_Incubator_Management_System
{
    public partial class MainPage : ContentPage
    {
        private const string Esp32IpAddress = "192.168.161.121"; // Replace with your ESP32's IP address
        private const int Esp32Port = 80; // Replace with the port your ESP32 is listening on
        private const string RotateCommand = "rotate"; // The path to trigger motor rotation

        public MainPage()
        {
            InitializeComponent();
        }

        private async void RotateMotorButton_Clicked(object sender, EventArgs e)
        {
            // Send a GET request to ESP32 to trigger motor rotation
            var httpClient = new HttpClient();
            var url = $"http://{Esp32IpAddress}:{Esp32Port}/{RotateCommand}";

            try
            {
                var response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    // Motor rotation request was successful
                    StatusLabel.Text = "Motor rotated 360 degrees.";
                }
                else
                {
                    // Failed to rotate the motor
                    StatusLabel.Text = "Failed to rotate motor.";
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions here
                StatusLabel.Text = $"An error occurred: {ex.Message}";
            }
        }
    }
}
