using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Yammerly.Models;
using Yammerly.Services;

using Xamarin.Forms;

namespace Yammerly.ViewModels
{
    public class TimelineViewModel : BaseViewModel
    {
        public TimelineViewModel()
        {
            Title = "Timeline";

            ExecuteRefreshCommandAsync();
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
                if (TimelineItems == null)
                    TimelineItems = new ObservableCollection<TimelineItem>();

                var items = await DependencyService.Get<IDataService>().GetItems<TimelineItem>();
                TimelineItems.Clear();
                foreach (var item in items)
                {
                    TimelineItems.Add(item);
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
