using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Yammerly.ViewModels;

using Xamarin.Forms;

namespace Yammerly.Views
{
    public partial class ProfilePage : ContentPage
    {
        public ProfilePage()
        {
            InitializeComponent();

            BindingContext = new ProfileViewModel();
        }
    }
}
