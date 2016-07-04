using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Yammerly.Models;

using Xamarin.Forms;
using Yammerly.Helpers;
using Yammerly.Services;

namespace Yammerly.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        public string Name { get; set; }
        public string Position { get; set; }
        public string PhotoUrl { get; set; }

        ObservableCollection<TimelineItem> timelineItems;
        public ObservableCollection<TimelineItem> TimelineItems
        {
            get { return timelineItems; }
            set { timelineItems = value; OnPropertyChanged("TimelineItems"); }
        }

        public ProfileViewModel()
        {
            Title = "Profile";

            Name = $"{Settings.FirstName} {Settings.LastName}";
            Position = "Software Engineer";
            PhotoUrl = Settings.PhotoUrl;

            TimelineItems = new ObservableCollection<TimelineItem>();
            ExecuteRefreshCommandAsync();
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
                var client = DependencyService.Get<IDataService>() as AzureService;
                var table = client.MobileService.GetSyncTable<TimelineItem>();
                var items = await table.Where((item) => item.Id == Settings.UserId).ToListAsync();
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
