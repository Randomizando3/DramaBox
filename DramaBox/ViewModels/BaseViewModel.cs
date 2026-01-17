using CommunityToolkit.Mvvm.ComponentModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DramaBox.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private string title = "";
    [ObservableProperty] private string error = "";

    protected void SetError(Exception ex) => Error = ex.Message;
    protected void ClearError() => Error = "";
}
