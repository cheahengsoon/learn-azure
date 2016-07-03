using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Yammerly.Models;

using Xamarin.Forms;

namespace Yammerly.ViewModels
{
    public class TimelineViewModel : BaseViewModel
    {
        public TimelineViewModel()
        {
            Title = "Timeline";

            var employees = new ObservableCollection<Employee>
            {
                new Employee { FirstName = "Nat", LastName = "Friedman", Title = "CEO", PhotoUrl = "http://static4.businessinsider.com/image/559d359decad04574c42a3c4-480/xamarin-nat-friedman.jpg" },
                new Employee { FirstName = "Miguel", LastName = "de Icaza", Title = "CTO", PhotoUrl = "http://images.techhive.com/images/idge/imported/article/nww/2011/03/031111-deicaza-100272676-orig.jpg" },
                new Employee { FirstName = "Joseph", LastName = "Hill", Title = "VP of Developer Relations", PhotoUrl = "https://www.gravatar.com/avatar/f763ec6935726b7f7715808828e52223.jpg?s=256" },
                new Employee { FirstName = "James", LastName = "Montemagno", Title = "Developer Evangelist", PhotoUrl = "http://www.gravatar.com/avatar/7d1f32b86a6076963e7beab73dddf7ca?s=256" },
                new Employee { FirstName = "Pierce", LastName = "Boggan", Title = "Software Engineer", PhotoUrl = "https://avatars3.githubusercontent.com/u/1091304?v=3&s=460" },
            };

            TimelineItems = new ObservableCollection<TimelineItem>
            {
                new TimelineItem { Author = employees[0], Text = "Had a great time talking about the Mobile DevOps lifecycle today!", PhotoUrl="https://lh3.googleusercontent.com/proxy/cvqTGBR-t3YY9dNwF0IN00xs_B7WDGF2h-klJThcBL40sdoI_piW8c33SsU3J3uGbM9go0_5ZWjAtr_MKqJFHRp6i8HdSWMBN_Iud9DtGw7EblrTSXiVgk06K2YpVjW8C95yptzDI3zwEPnXGw=w506-h284" },
                new TimelineItem { Author = employees[2], Text = "First full day of the conference and it's packed! Over 2,000 people here.", PhotoUrl = "http://img.youtube.com/vi/jgXCB51e4ak/hqdefault.jpg" },
                new TimelineItem { Author = employees[3], Text = "Learning all about building connected apps with Azure thanks to Mike James!", PhotoUrl = "https://sec.ch9.ms/ch9/2d33/ee514b78-c024-49bf-8341-87fd2a492d33/XE16AzureXamarin_Custom.jpg" },
                new TimelineItem { Author = employees[4], Text = "Thought I showed up to see Miguel de Icaza?", PhotoUrl = "https://pbs.twimg.com/media/ChKQOQRWgAEB7hu.jpg" },
                new TimelineItem { Author = employees[1], Text= "We finally did it! .NET native, open source and on all platforms.", PhotoUrl = "https://encrypted-tbn3.gstatic.com/images?q=tbn:ANd9GcTgTyHJtGi8odJpb1chJC2e8oCKCBevfft-PJueaCcO4V7O3DFv" }
            };
        }

        ObservableCollection<TimelineItem> timelineItems;
        public ObservableCollection<TimelineItem> TimelineItems
        {
            get { return timelineItems; }
            set { timelineItems = value; OnPropertyChanged("TimelineItems"); }
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
