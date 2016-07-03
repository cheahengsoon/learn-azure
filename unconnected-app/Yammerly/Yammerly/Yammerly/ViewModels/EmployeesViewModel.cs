using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using Yammerly.Models;

using Xamarin.Forms;

namespace Yammerly.ViewModels
{
    public class EmployeesViewModel : BaseViewModel
    {
        public EmployeesViewModel()
        {
            Title = "Employees";

            Employees = new ObservableCollection<Employee>
            {
                new Employee { FirstName = "Nat", LastName = "Friedman", Title = "CEO", PhotoUrl = "http://static4.businessinsider.com/image/559d359decad04574c42a3c4-480/xamarin-nat-friedman.jpg" },
                new Employee { FirstName = "Miguel", LastName = "de Icaza", Title = "CTO", PhotoUrl = "http://images.techhive.com/images/idge/imported/article/nww/2011/03/031111-deicaza-100272676-orig.jpg" },
                new Employee { FirstName = "Joseph", LastName = "Hill", Title = "VP of Developer Relations", PhotoUrl = "https://www.gravatar.com/avatar/f763ec6935726b7f7715808828e52223.jpg?s=256" },
                new Employee { FirstName = "James", LastName = "Montemagno", Title = "Developer Evangelist", PhotoUrl = "http://www.gravatar.com/avatar/7d1f32b86a6076963e7beab73dddf7ca?s=256" },
                new Employee { FirstName = "Pierce", LastName = "Boggan", Title = "Software Engineer", PhotoUrl = "https://avatars3.githubusercontent.com/u/1091304?v=3&s=460" },
            };
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
                // Refresh employees here
                
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