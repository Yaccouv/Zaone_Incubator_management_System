using Rg.Plugins.Popup.Services;
using System;
using System.Globalization;
using System.Xml.Linq;
using Xamarin.Forms; // for the main Xamarin.Forms Picker class

using Xamarin.Forms.Xaml;
using Zaone_Incubator_Management_System.Database;


namespace Zaone_Incubator_Management_System.ViewModel
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AddEggBatch : ContentPage
	{
        DatabaseConnector databaseConnector = new DatabaseConnector();
        public AddEggBatch ()
		{
			InitializeComponent ();
            LoadIncubators_Clicked();
            dataDropdown.SelectedIndexChanged += Dropdown_SelectionChanged;
            CalculateHatchingDate();

            
        }

        private async void LoadIncubators_Clicked()
        {
            try
            {
                var data = await databaseConnector.GetIncubatorNamesWithStatusAsync();

                dataDropdown.ItemsSource = data; // Populate the dropdown
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void Dropdown_SelectionChanged(object sender, EventArgs e)
        {
            if (sender is Picker dropdown)
            {
                string selectedValue = dropdown.SelectedItem as string;
                (string capacityValue, int incubatorId) = await databaseConnector.GetIncubatorDataByColumn1Async(selectedValue);

                if (capacityValue != null)
                {
                    capacity.Text =capacityValue;
                    lblIncubatorID.Text = incubatorId.ToString();
                }
            }

        }



        public async void btnSave_clicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(dataDropdown.SelectedItem as string) && !string.IsNullOrEmpty(txtNumber.Text))
            {
                if (int.TryParse(txtNumber.Text, out int enteredNumber) && int.TryParse(capacity.Text, out int capacityValue))
                {
                    if (enteredNumber <= capacityValue)
                    {
                        // Combine selected date and time for both the DatePicker and TimePicker
                        DateTime selectedDateTime = hatchingDatePicker.Date + hatchingTimePicker.Time;

                        // Provide the incubatorId as the fourth parameter
                        await databaseConnector.EggBatch(
                            enteredNumber,        // Use the parsed enteredNumber
                            DateTime.Now,        // Use DateTime.Now as the selected date
                            selectedDateTime,    // Use the selected date and time
                            Convert.ToInt32(lblIncubatorID.Text)
                        );

                        await DisplayAlert("Success", "Incubator has been set Successfully", "OK");
                        txtNumber.Text = string.Empty;
                        dataDropdown.SelectedItem = string.Empty;
                        await Navigation.PushAsync(new MasterPage());
                    }
                    else
                    {
                        await DisplayAlert("Denied", "Entered number exceeds capacity", "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Invalid Input", "Please enter valid numeric values for Capacity", "OK");
                }
            }
            else
            {
                await DisplayAlert("Denied", "Please fill in all fields", "OK");
            }
        }





        private void CalculateHatchingDate()
        {
            DateTime currentDateTime = DateTime.Now;

            // Add 21 days to the current date and time
            DateTime hatchingDateTime = currentDateTime.AddDays(21);

            // Set the date part to the hatchingDatePicker
            hatchingDatePicker.Date = hatchingDateTime.Date;

            // Set the time part to the hatchingTimePicker
            hatchingTimePicker.Time = new TimeSpan(hatchingDateTime.Hour, hatchingDateTime.Minute, hatchingDateTime.Second);

            // Set the time part to the startTimePicker
            startTimePicker.Time = new TimeSpan(hatchingDateTime.Hour, hatchingDateTime.Minute, hatchingDateTime.Second);
        }


        private async void Button_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.PushAsync(new AddIncubatorPopUp());
        }

    }
}