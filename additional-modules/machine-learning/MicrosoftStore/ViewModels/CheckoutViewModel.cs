using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;

using Recommendations;

namespace MicrosoftStore
{
	public class CheckoutViewModel : BaseViewModel
	{
		public ObservableCollection<Inventory> Cart { get; set; }	
		public ObservableCollection<Inventory> Recommendations { get; set; }

		public CheckoutViewModel()
		{
			Title = "Checkout";

			var app = (App)Application.Current;
			Cart = app.Cart;

			Recommendations = new ObservableCollection<Inventory>();

			CheckoutRecommendations();
		}

		async Task CheckoutRecommendations()
		{
			Task.Run(() =>
			{
				var client = new RecommendationsApi(Constants.AccountKey, Constants.BaseUri);
				var recommendations = client.GetRecommendations(Constants.ModelId, Constants.BuildId, Cart[0].ItemId, 3);

				foreach (var rec in recommendations.RecommendedItemSetInfo)
				{
					foreach (var item in rec.Items)
					{
						Recommendations.Add(new Inventory { ItemId = item.Id, Name = item.Name, Description = rec.Rating.ToString() });
					}
				}
			});
		}
	}
}

