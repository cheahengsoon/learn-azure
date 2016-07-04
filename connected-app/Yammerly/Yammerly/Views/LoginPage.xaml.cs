using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Yammerly.Helpers;
using Yammerly.ViewModels;

using Xamarin.Forms;

namespace Yammerly.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();

            BindingContext = new LoginViewModel();
        }
    }
}
