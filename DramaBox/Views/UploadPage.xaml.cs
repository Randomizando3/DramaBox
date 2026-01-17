namespace DramaBox.Views;

public partial class UploadPage : ContentPage
{
    public UploadPage() { InitializeComponent(); }

    private async void OnGoSub(object sender, EventArgs e)
        => await Navigation.PushAsync(new SubscriptionPage());
}
