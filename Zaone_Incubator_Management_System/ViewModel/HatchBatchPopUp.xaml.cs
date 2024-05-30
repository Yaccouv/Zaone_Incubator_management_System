using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Zaone_Incubator_Management_System.Database;

namespace Zaone_Incubator_Management_System.ViewModel
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HatchBatchPopUp : PopupPage
    {
        DatabaseConnector connector = new DatabaseConnector();
        private Label numberOfEggsLabel;
        private int IncubatorID;
        private int EggBatchID;

        public HatchBatchPopUp(int incubatorID, string incubatorName, int eggBatchID)
        {
            InitializeComponent();

            incubatorname.Text = "Incubator Name: " + incubatorName;
            IncubatorID = incubatorID;
            EggBatchID = eggBatchID;

            // Call the method to retrieve the number of eggs asynchronously
            LoadNumberOfEggsAsync(incubatorID);
        }

        private async void LoadNumberOfEggsAsync(int incubatorID)
        {
            try
            {
                // Call the async method to retrieve the number of eggs
                var numberOfEggs = await connector.GetNumberOfEggsAsync(incubatorID);

                // Display the number of eggs on a label
                EggNumber.Text = "Number of Eggs: " + numberOfEggs.ToString();
            }
            catch (Exception ex)
            {
                // Handle exceptions here
                await DisplayAlert("Error", "An error occurred: " + ex.Message, "OK");
            }
        }

        private async void btnCancle_OnClicked(object sender, EventArgs e)
        {
            await PopupNavigation.PopAsync(true);
        }

        private async void btnFinish_OnClicked(object sender, EventArgs e)
        {
            try
            {
                if (int.TryParse(txtEggNumber.Text, out int eggsToRemove))
                {
                    // Ensure that eggsToRemove is a valid integer

                    // Subtract eggsToRemove from the current egg count displayed in the label
                    int currentEggCount = int.Parse(EggNumber.Text.Replace("Number of Eggs: ", ""));

                    // Swap the variables for hatched and unhatched eggs
                    int unhatchedEggs = currentEggCount - eggsToRemove;
                    int hatchedEggs = currentEggCount - unhatchedEggs;

                    // Update the EGG batch and Incubator status with the new egg count and unhatched eggs
                    await connector.UpdateEGGbatchAndIncubatorStatus(IncubatorID, EggBatchID, hatchedEggs, unhatchedEggs);

                    // Show a success message
                    await DisplayAlert("Incubation Finished", "Incubation process has been successfully completed.", "OK");

                    // Close the popup
                    var masterPage = new MasterPage();
                    await Navigation.PushAsync(masterPage);
                    await PopupNavigation.PopAsync(true);
                }
                else
                {
                    // Handle the case where txtEggNumber is not a valid integer
                    await DisplayAlert("Error", "Please enter a valid number of eggs to remove.", "OK");
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions here
                await DisplayAlert("Error", "An error occurred: " + ex.Message, "OK");
            }
        }




    }
}

