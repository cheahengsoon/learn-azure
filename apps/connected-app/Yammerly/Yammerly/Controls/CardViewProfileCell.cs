using System;

using Xamarin.Forms;

namespace Yammerly.Controls
{
    public class CardViewProfileCell : ViewCell
    {
        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(CardViewProfileCell), null, BindingMode.OneWay, propertyChanged: (BindableObject bindable, object oldValue, object newValue) =>
            {
                var ctrl = (CardViewProfileCell)bindable;
                ctrl.Text = (string)newValue;
            });

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); textLabel.Text = value; }
        }

        public static readonly BindableProperty ImageSourceProperty =
            BindableProperty.Create(nameof(ImageSource), typeof(ImageSource), typeof(CardViewProfileCell), null, BindingMode.OneWay, propertyChanged: (BindableObject bindable, object oldValue, object newValue) =>
            {
                var ctrl = (CardViewProfileCell)bindable;
                ctrl.ImageSource = (ImageSource)newValue;
            });

        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); image.Source = value; }
        }


        StackLayout layout;
        Image image;
        Label textLabel;

        public CardViewProfileCell()
        {
            image = new Image
            {
                Aspect = Device.OnPlatform<Aspect>(Aspect.AspectFill, Aspect.AspectFill, Aspect.AspectFit),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.StartAndExpand,
                WidthRequest = 220,
                HeightRequest = 200,
                Source = ImageSource.FromUri(new Uri("https://avatars3.githubusercontent.com/u/1091304?v=3&s=460"))
            };

            textLabel = new Label
            {
                FontSize = Device.OnPlatform<double>(15, 15, 15),
                TextColor = Device.OnPlatform<Color>(Color.FromHex("030303"), Color.FromHex("030303"), Color.FromHex("030303")),
                HorizontalOptions = LayoutOptions.Start,
                HorizontalTextAlignment = TextAlignment.Start,
                Margin = Device.OnPlatform<Thickness>(new Thickness(12, 10, 12, 12), new Thickness(20, 10, 20, 20), new Thickness(20, 10, 20, 20)),
                LineBreakMode = LineBreakMode.WordWrap,
                VerticalOptions = LayoutOptions.End,
                Text = "Pierce Boggan"
            };

            layout = new StackLayout
            {
                BackgroundColor = Color.White,
                Spacing = 0,
                Children = { image, textLabel }
            };

            View = layout;
        }
    }
}