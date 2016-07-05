using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using Yammerly.Models;
using Yammerly.Services;

using Xamarin.Forms;

namespace Yammerly.ViewModels
{
    public class EmployeesViewModel : BaseViewModel
    {
        public EmployeesViewModel()
        {
            Title = "Employees";

            ExecuteRefreshCommandAsync();
        }

        ObservableCollection<Employee> employees;
        public ObservableCollection<Employee> Employees
        {
            get { return employees; }
            set { employees = value; OnPropertyChanged("Employees"); }
        }

        Command refreshCommand;
        public Command RefreshCommand
        {
            get { return refreshCommand ?? (refreshCommand = new Command(async () => await ExecuteRefreshCommandAsync())); }
        }

        async Task ExecuteRefreshCommandAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                if (Employees == null)
                    Employees = new ObservableCollection<Employee>();

                var items = await DependencyService.Get<IDataService>().GetItems<Employee>();
                Employees.Clear();
                foreach (var item in items)
                {
                    Employees.Add(item);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}