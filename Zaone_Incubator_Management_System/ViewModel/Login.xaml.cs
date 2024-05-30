using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Zaone_Incubator_Management_System.Database;
using Zaone_Incubator_Management_System.Model;

namespace Zaone_Incubator_Management_System.ViewModel
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Login : ContentPage
    {
        DatabaseConnector databaseConnector = new DatabaseConnector();
       
        public Login()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private void btnLogin_Tapped(object sender, EventArgs e)
        {
            LoginCode();
        }
        private void imgLogin_Tapped(object sender, EventArgs e)
        {
            LoginCode();
        }


        public async void LoginCode()
        {
            if (!string.IsNullOrWhiteSpace(txtPassword.Text) &&
                 !string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                if (txtPassword.Text.Length == 5) // Check for a 5-digit password
                {
                    // Check if the username contains only letters
                    if (Regex.IsMatch(txtUsername.Text, "^[a-zA-Z]+$"))
                    {
                        var farmer = await databaseConnector.Login(txtUsername.Text, txtPassword.Text);
                        if (farmer != null)
                        {
                            await DisplayAlert("Success", "Login Successfully", "OK");
                            txtUsername.Text = string.Empty;
                            txtPassword.Text = string.Empty;

                            // Create a new MasterPage instance
                            var masterPage = new MasterPage();
                            var incubator = new IncubatorDashboard();

                            // Set the Farmer data on the MasterPage
                            masterPage.SetFarmerUsername(farmer);
                            incubator.SetFarmerEmail(farmer);

                            // Navigate to the MasterPage
                            await Navigation.PushAsync(masterPage);
                        }
                        else
                        {
                            await DisplayAlert("Denied", "Login denied", "OK");
                            txtUsername.Text = string.Empty;
                            txtPassword.Text = string.Empty;
                        }
                    }
                    else
                    {
                        await DisplayAlert("Denied", "Username must contain only letters", "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Denied", "Password must be 5 digits", "OK");
                }
            }
            else
            {
                await DisplayAlert("Denied", "Please fill in all fields", "OK");
            }
        }





        private void Signup_Tapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Register());
        }


        private void HidePass_Clicked(object sender, EventArgs e)
        {
            var imageButton = sender as ImageButton;
            if (txtPassword.IsPassword)
            {
                imageButton.Source = ImageSource.FromFile("eyeopen.png");
                txtPassword.IsPassword = false;
            }
            else
            {
                imageButton.Source = ImageSource.FromFile("eyeclose.png");
                txtPassword.IsPassword = true;
            }
        }

    }
}