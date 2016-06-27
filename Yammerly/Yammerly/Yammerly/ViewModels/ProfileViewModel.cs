using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Yammerly.Models;

using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace Yammerly.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        public Employee Employee { get; set; }
        ObservableCollection<TimelineItem> timelineItems;
        public ObservableCollection<TimelineItem> TimelineItems
        {
            get { return timelineItems; }
            set { timelineItems = value; OnPropertyChanged("TimelineItems"); }
        }

        public ProfileViewModel()
        {
            Title = "Profile";

            Employee = new Employee { FirstName = "Pierce", LastName = "Boggan", Title = "Software Engineer", PhotoUrl = "https://avatars3.githubusercontent.com/u/1091304?v=3&s=460" };
            TimelineItems = new ObservableCollection<TimelineItem>
            {
                new TimelineItem { Author = Employee, Text = "Thought I showed up to see Miguel de Icaza?", PhotoUrl = "https://pbs.twimg.com/media/ChKQOQRWgAEB7hu.jpg" },
                new TimelineItem { Author = Employee, Text = "If you're not in Mike James' session and are building apps with Azure, you need to be!", PhotoUrl = "https://sec.ch9.ms/ch9/2d33/ee514b78-c024-49bf-8341-87fd2a492d33/XE16AzureXamarin_Custom.jpg" },
            };
        }
    }
}
