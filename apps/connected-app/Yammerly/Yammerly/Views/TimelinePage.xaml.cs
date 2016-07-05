using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Yammerly.ViewModels;

using Xamarin.Forms;

namespace Yammerly.Views
{
    public partial class TimelinePage : ContentPage
    {
        public TimelinePage()
        {
            InitializeComponent();

            BindingContext = new TimelineViewModel();
            if (Device.OS == TargetPlatform.Windows)
            {
                var toolbarItem = new ToolbarItem
                {
                    Icon = "refresh.png",
                    Command = ViewModel.RefreshCommand
                };

                ToolbarItems.Add(toolbarItem);
            }
        }

        private TimelineViewModel ViewModel
        {
            get { return BindingContext as TimelineViewModel; }
        }
    }
}
