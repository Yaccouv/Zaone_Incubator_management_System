using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Zaone_Incubator_Management_System.Database;

namespace Zaone_Incubator_Management_System.ViewModel
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddIncubatorPopUp : PopupPage
    {
        DatabaseConnector databaseConnector = new DatabaseConnector();
        public AddIncubatorPopUp()
        {
            InitializeComponent();
            
        }

        private async void btnCancle_OnClicked(object sender, EventArgs e)
        {
            await PopupNavigation.PopAsync(true);
        }

        private async void btnSaveIncubator_OnClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtName.Text) &&
           !string.IsNullOrWhiteSpace(txtCapacity.Text))
            {
                if (Regex.IsMatch(txtName.Text, "^[a-zA-Z]+$"))
                {
                    // Check if the capacity contains only digits
                    if (Regex.IsMatch(txtCapacity.Text, "^[0-9]+$"))
                    {
                        // Check if the name already exists
                        bool nameExists = await databaseConnector.AddIncubator(txtName.Text, txtCapacity.Text);

                        if (!nameExists)
                        {
                            txtName.Text = string.Empty;
                            txtCapacity.Text = string.Empty;
                            await DisplayAlert("Success", "Incubator Added Successfully", "OK");
                            await PopupNavigation.PopAsync(true);
                        }
                        else
                        {
                            await DisplayAlert("Denied", "Incubator already exists", "OK");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Denied", "Capacity must contain only Numbers", "OK");
                    }
                }

                else
                {
                    await DisplayAlert("Denied", "Name must contain only letters", "OK");
                }
            }

            else
            {
                await DisplayAlert("Denied", "Please fill in all fields", "OK");
            }

        }


    }
}