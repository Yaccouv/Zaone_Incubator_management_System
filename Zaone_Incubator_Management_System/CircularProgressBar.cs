using Xamarin.Forms;


namespace Zaone_Incubator_Management_System
{
    public class CircularProgressBar : ContentView
    {
        private readonly BoxView circle;
        private readonly Label progressLabel;

        public double Progress
        {
            get => (double)GetValue(ProgressProperty);
            set => SetValue(ProgressProperty, value);
        }

        public static readonly BindableProperty ProgressProperty =
            BindableProperty.Create(nameof(Progress), typeof(double), typeof(CircularProgressBar), 0.0, propertyChanged: OnProgressChanged);

        public CircularProgressBar()
        {
            circle = new BoxView
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = 100, // Adjust as needed
                HeightRequest = 100, // Adjust as needed
                CornerRadius = (float)(100 / 2),
                BackgroundColor = Color.Transparent,
                //BorderColor = Color.Blue, // Change the color to your preference
                //BorderWidth = 10, // Adjust as needed
            };

            progressLabel = new Label
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                FontSize = 18, // Adjust as needed
                TextColor = Color.Black // Change the color to your preference
            };

            Content = new Grid
            {
                Children =
            {
                circle,
                progressLabel
            }
            };
        }

        private static void OnProgressChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is CircularProgressBar circularProgressBar)
            {
                double progress = (double)newValue;
                double angle = 360 * progress;

                circularProgressBar.circle.Rotation = angle;

                // Update the progress label with the percentage
                circularProgressBar.progressLabel.Text = $"{(int)(progress * 100)}%";
            }
        }
    }
}
