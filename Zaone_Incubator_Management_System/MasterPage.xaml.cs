using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Zaone_Incubator_Management_System.Model;
using Zaone_Incubator_Management_System.ViewModel;

namespace Zaone_Incubator_Management_System
{
    public partial class MasterPage : MasterDetailPage
    {
        private Farmer _farmer;
        List<MenuItems> menu;

        public MasterPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            menu = new List<MenuItems>();

            menu.Add(new MenuItems { OptionName = "Home", IconSource = ImageSource.FromFile("home1.jpeg") });
            menu.Add(new MenuItems { OptionName = "Add Egg Batches", IconSource = ImageSource.FromFile("inc3.jpeg") });
            menu.Add(new MenuItems { OptionName = "Hatched Batches", IconSource = ImageSource.FromFile("hat1.jpeg") });
            menu.Add(new MenuItems { OptionName = "Reports", IconSource = ImageSource.FromFile("report2.png") });
            menu.Add(new MenuItems { OptionName = "Settings", IconSource = ImageSource.FromFile("st2.png") });
            menu.Add(new MenuItems { OptionName = "Help", IconSource = ImageSource.FromFile("help1.png") });
            menu.Add(new MenuItems { OptionName = "Logout", IconSource = ImageSource.FromFile("log2.jpeg") });

            navigationList.ItemsSource = menu;
            Detail = new NavigationPage(new IncubatorDashboard());
        }

        public void SetFarmerUsername(Farmer farmer)
        {
            _farmer = farmer;
            lblUsername.Text = "Hello " + _farmer.Username;
        }

        private async void Item_Tapped(object sender, ItemTappedEventArgs e)
        {
            try
            {
                var item = e.Item as MenuItems;

                foreach (var menuItem in menu)
                {
                    menuItem.IsActive = (menuItem.OptionName == item.OptionName);
                }

                navigationList.ItemsSource = null;
                navigationList.ItemsSource = menu;

                foreach (var cell in navigationList.TemplatedItems)
                {
                    var viewCell = (ViewCell)cell;
                    var contentView = (ContentView)viewCell.View;

                    if (contentView.BindingContext == item)
                    {
                        contentView.BackgroundColor = Color.FromHex("#E5F0E5");
                    }
                    else
                    {
                        contentView.BackgroundColor = Color.White;
                    }
                }

                switch (item.OptionName)
                {
                    case "Home":
                        Detail = new NavigationPage(new IncubatorDashboard());
                        IsPresented = false;
                        break;
                    case "Add Egg Batches":
                        Detail = new NavigationPage(new AddEggBatch());
                        IsPresented = false;
                        break;
                    case "Hatched Batches":
                        Detail = new NavigationPage(new HatchedBatches());
                        IsPresented = false;
                        break;
                    case "Reports":
                        // Handle the "Reports" action here or remove this case if not needed.
                        IsPresented = false;
                        break;
                    case "Logout":
                        bool result = await DisplayAlert("Logout", "Are you sure you want to log out and exit?", "OK", "Cancel");

                        if (result)
                        {
                            await Detail.Navigation.PushAsync(new Login());
                            IsPresented = false;
                        }
                        break;
                    default:
                        // Handle other actions or defaults
                        IsPresented = false;
                        break;
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
            }
        }
    }
}
