using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Yammerly.Helpers;

using Xamarin.Forms;

namespace Yammerly.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        void On_Clicked(object sender, EventArgs e)
        {
            Settings.LoggedIn = true;

            App.Current.MainPage = new RootPage();
        }
    }
}
