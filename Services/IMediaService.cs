using WindowsMediaController.Models;

namespace WindowsMediaController.Services;

public interface IMediaService
{
    Task PlayPauseAsync();
    Task StopAsync();
    Task NextAsync();
    Task PreviousAsync();
    Task<MediaStatus> GetStatusAsync();
}
