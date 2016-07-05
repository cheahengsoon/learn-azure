using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Yammerly.ViewModels;

using Xamarin.Forms;

namespace Yammerly.Views
{
    public partial class EmployeesPage : ContentPage
    {
        public EmployeesPage()
        {
            InitializeComponent();

            BindingContext = new EmployeesViewModel();

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

        private EmployeesViewModel ViewModel
        {
            get { return BindingContext as EmployeesViewModel; }
        }
    }
}
