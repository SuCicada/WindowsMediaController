using System.Runtime.InteropServices;
using Windows.Media.Control;
using WindowsMediaController.Models;

namespace WindowsMediaController.Services;

public class MediaService : IMediaService
{
    private GlobalSystemMediaTransportControlsSessionManager? _smtcManager;

    // keybd_event API for media control
    [DllImport("user32.dll")]
    private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

    private const byte VK_MEDIA_NEXT_TRACK = 0xB0;
    private const byte VK_MEDIA_PREV_TRACK = 0xB1;
    private const byte VK_MEDIA_STOP = 0xB2;
    private const byte VK_MEDIA_PLAY_PAUSE = 0xB3;
    private const uint KEYEVENTF_KEYUP = 0x0002;

    public MediaService()
    {
        // Initialize SMTC manager asynchronously
        Task.Run(async () =>
        {
            try
            {
                _smtcManager = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
            }
            catch
            {
                // Handle or log initialization failure
            }
        });
    }

    public Task PlayPauseAsync()
    {
        SendMediaKey(VK_MEDIA_PLAY_PAUSE);
        return Task.CompletedTask;
    }

    public Task StopAsync()
    {
        SendMediaKey(VK_MEDIA_STOP);
        return Task.CompletedTask;
    }

    public Task NextAsync()
    {
        SendMediaKey(VK_MEDIA_NEXT_TRACK);
        return Task.CompletedTask;
    }

    public Task PreviousAsync()
    {
        SendMediaKey(VK_MEDIA_PREV_TRACK);
        return Task.CompletedTask;
    }

    public async Task<MediaStatus> GetStatusAsync()
    {
        var status = new MediaStatus();

        if (_smtcManager == null)
        {
            _smtcManager = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
        }

        var session = _smtcManager.GetCurrentSession();
        if (session != null)
        {
            var playbackInfo = session.GetPlaybackInfo();
            status.IsPlaying = playbackInfo.PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing;

            var mediaProperties = await session.TryGetMediaPropertiesAsync();
            if (mediaProperties != null)
            {
                status.Title = mediaProperties.Title;
                status.Artist = mediaProperties.Artist;
                status.AlbumTitle = mediaProperties.AlbumTitle;
                status.AlbumArtist = mediaProperties.AlbumArtist;
                status.TrackNumber = mediaProperties.TrackNumber;

                if (mediaProperties.Thumbnail != null)
                {
                    try 
                    {
                        using var stream = await mediaProperties.Thumbnail.OpenReadAsync();
                        using var memoryStream = new MemoryStream();
                        await stream.AsStreamForRead().CopyToAsync(memoryStream);
                        status.Thumbnail = Convert.ToBase64String(memoryStream.ToArray());
                    }
                    catch
                    {
                        // Ignore thumbnail errors
                    }
                }
            }
        }

        return status;
    }

    private void SendMediaKey(byte key)
    {
        keybd_event(key, 0, 0, 0); // Key down
        keybd_event(key, 0, KEYEVENTF_KEYUP, 0); // Key up
    }
}
