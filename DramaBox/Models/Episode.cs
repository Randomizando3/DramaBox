namespace DramaBox.Models;

public class Episode
{
    public string Id { get; set; } = "";

    public int Number { get; set; } = 0;
    public string Title { get; set; } = "";

    public int DurationSec { get; set; } = 0;

    public string ThumbUrl { get; set; } = "";
    public string VideoUrl { get; set; } = "";

    public bool Active { get; set; } = true;
    public int Order { get; set; } = 0;
}
