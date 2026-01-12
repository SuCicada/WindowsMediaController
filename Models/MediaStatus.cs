namespace WindowsMediaController.Models;

public class MediaStatus
{
    public string? Artist { get; set; }
    public string? Title { get; set; }
    public string? AlbumArtist { get; set; }
    public string? AlbumTitle { get; set; }
    public int TrackNumber { get; set; }
    public string? Thumbnail { get; set; } // Base64 or path
    public bool IsPlaying { get; set; }
}
