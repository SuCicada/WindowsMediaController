namespace WindowsMediaController.Services;

public interface IVolumeService
{
    float GetVolume();
    void SetVolume(float level);
    void VolumeUp();
    void VolumeDown();
    void Mute();
    void Unmute();
    bool IsMuted();
}
