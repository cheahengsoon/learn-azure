using System;
using System.Collections.Generic;

using Yammerly.Models;
using Yammerly.ViewModels;

using Xamarin.Forms;
using Yammerly.Helpers;

namespace Yammerly.Views
{
    public partial class MenuPage : ContentPage
    {
        RootPage root;
        List<HomeMenuItem> menuItems;
        public MenuPage(RootPage root)
        {
            Title = "Yammerly";

            this.root = root;
            InitializeComponent();

            BackgroundColor = Color.FromHex("#03A9F4");
            ListViewMenu.BackgroundColor = Color.FromHex("#F5F5F5");

            ListViewMenu.ItemsSource = menuItems = new List<HomeMenuItem>
            {       new HomeMenuItem { Title = "Timeline", MenuType = MenuType.Timeline, Icon ="about.png" },
                    new HomeMenuItem { Title = "Employees", MenuType = MenuType.Employees, Icon = "blog.png" },
                    new HomeMenuItem { Title = "Profile", MenuType = MenuType.Profile, Icon = "twitternav.png" }
            };

            ListViewMenu.SelectedItem = menuItems[0];

            ListViewMenu.ItemSelected += async (sender, e) =>
            {
                if (ListViewMenu.SelectedItem == null)
                    return;
                
                await this.root.NavigateAsync(((HomeMenuItem)e.SelectedItem).MenuType);

                ListViewMenu.SelectedItem = null;
            };
            
            nameLabel.Text = $"{Settings.FirstName} {Settings.LastName}";
            profilePhoto.Source = Settings.PhotoUrl;
        }
    }
}
