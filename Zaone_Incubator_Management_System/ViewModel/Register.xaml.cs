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
    public partial class Register : ContentPage
    {
        DatabaseConnector databaseConnector = new DatabaseConnector();
        public Register()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

       

        private void btnRegister_Tapped(object sender, EventArgs e)
        {
            RegisterFarmer();
        }

        private void imgRegister_Tapped(object sender, EventArgs e)
        {
            RegisterFarmer();
        }

        private void LoginBtn_Tapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Login());
        }

        public async Task RegisterFarmer()
        {
            if (!string.IsNullOrWhiteSpace(txtPassword.Text) &&
                !string.IsNullOrWhiteSpace(txtUsernamee.Text) &&
                !string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                if (txtPassword.Text.Length == 5) // Check for a 5-digit password
                {
                    // Check if the username contains only letters
                    if (Regex.IsMatch(txtUsernamee.Text, "^[a-zA-Z]+$"))
                    {
                        // Check if the email is valid
                        if (IsValidEmail(txtEmail.Text))
                        {
                            if (txtPassword.Text == txtConfirm.Text)
                            {
                                // Check if the email already exists
                                bool emailExists = await databaseConnector.AddFarmer(txtUsernamee.Text, txtPassword.Text, txtEmail.Text);

                                if (!emailExists)
                                {
                                    txtUsernamee.Text = string.Empty;
                                    txtPassword.Text = string.Empty;
                                    txtEmail.Text = string.Empty;
                                    txtConfirm.Text = string.Empty;
                                    await DisplayAlert("Success", "Farmer Added Successfully", "OK");
                                }
                                else
                                {
                                    await DisplayAlert("Denied", "Email already exists", "OK");
                                }
                            }
                            else
                            {
                                await DisplayAlert("Denied", "Password do not match", "OK");
                            }
                        }
                        else
                        {
                            await DisplayAlert("Denied", "Invalid email format", "OK");
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

        public bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
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

        private void HidePass1_Clicked(object sender, EventArgs e)
        {
            var imageButton = sender as ImageButton;
            if (txtConfirm.IsPassword)
            {
                imageButton.Source = ImageSource.FromFile("eyeopen.png");
                txtConfirm.IsPassword = false;
            }
            else
            {
                imageButton.Source = ImageSource.FromFile("eyeclose.png");
                txtConfirm.IsPassword = true;
            }
        }




    }
}