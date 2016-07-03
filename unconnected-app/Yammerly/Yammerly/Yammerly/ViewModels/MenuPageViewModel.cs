using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Yammerly.ViewModels;

using Xamarin.Forms;

namespace Yammerly.ViewModels
{
    public class MenuPageViewModel : BaseViewModel
    {
        public string ProfilePhotoUrl { get; set; }

        public MenuPageViewModel()
        {
            Title = "Yammerly";
            Icon = "";
            ProfilePhotoUrl = "https://avatars3.githubusercontent.com/u/1091304?v=3&s=460";
        }
    }
}
