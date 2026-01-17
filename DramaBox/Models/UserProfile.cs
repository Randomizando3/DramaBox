namespace DramaBox.Models;

public class UserProfile
{
    public string Uid { get; set; } = "";
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string PhotoUrl { get; set; } = "";
    public string Plan { get; set; } = "free"; // free / plus / premium
}
