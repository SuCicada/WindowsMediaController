using NAudio.CoreAudioApi;
using System.Runtime.InteropServices;
using Serilog;

namespace WindowsMediaController.Services;

public class VolumeService : IVolumeService, IDisposable
{
    private readonly MMDeviceEnumerator _deviceEnumerator;
    private readonly MMDevice _defaultDevice;

    // Windows API 常量
    private const int APPCOMMAND_VOLUME_UP = 0x0A;
    private const int APPCOMMAND_VOLUME_DOWN = 0x09;
    private const int APPCOMMAND_VOLUME_MUTE = 0x08;
    private const int WM_APPCOMMAND = 0x0319;
    private static readonly IntPtr HWND_BROADCAST = new IntPtr(0xFFFF);

  // Windows API 常量
    private const byte VK_VOLUME_MUTE = 0xAD;
    private const byte VK_VOLUME_DOWN = 0xAE;
    private const byte VK_VOLUME_UP = 0xAF;

    public VolumeService()
    {
        _deviceEnumerator = new MMDeviceEnumerator();
        _defaultDevice = _deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
    }

    public float GetVolume()
    {
        return _defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar;
    }

    public void SetVolume(float level)
    {
        float targetVolume = Math.Clamp(level, 0.0f, 1.0f);
        float currentVolume = GetVolume();
        Log.Information("SetVolume: targetVolume: {TargetVolume}, currentVolume: {CurrentVolume}", targetVolume, currentVolume);

        // 如果目标音量与当前音量相同，直接返回
        // if (Math.Abs(targetVolume - currentVolume) < 0.001f)
        // {
        //     Log.Information("Volume is already at the target level");
        //     return;
        // }

        // 先用 COM API 精确设置音量值
        _defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar = targetVolume  ;
        
        // 再发送一个 WM_APPCOMMAND 消息来触发系统音量 UI 显示
        // 根据音量变化方向选择合适的命令
        // int command = (targetVolume > currentVolume) ? APPCOMMAND_VOLUME_UP : APPCOMMAND_VOLUME_DOWN;
        // TriggerVolumeOSD(command);
        KeyboardHelper.SimulateKeyPress(VK_VOLUME_UP);
        KeyboardHelper.SimulateKeyPress(VK_VOLUME_DOWN);
    }

    public void VolumeUp()
    {
        // float current = GetVolume();
        // SetVolume(current + 0.02f);
        KeyboardHelper.SimulateKeyPress(VK_VOLUME_UP);
    }

    public void VolumeDown()
    {
        // float current = GetVolume();
        // SetVolume(current - 0.02f);
        KeyboardHelper.SimulateKeyPress(VK_VOLUME_DOWN);
    }

    public void Mute()
    {
        // _defaultDevice.AudioEndpointVolume.Mute = true;
          if (!IsMuted())
        {
            KeyboardHelper.SimulateKeyPress(VK_VOLUME_MUTE);
        }
    }

    public void Unmute()
    {
        // _defaultDevice.AudioEndpointVolume.Mute = false;
        if (IsMuted())
        {
            KeyboardHelper.SimulateKeyPress(VK_VOLUME_MUTE);
        }
    }

    public bool IsMuted()
    {
        return _defaultDevice.AudioEndpointVolume.Mute;
    }

    /// <summary>
    /// 触发 Windows 系统音量 OSD (On-Screen Display) 显示
    /// 通过发送 WM_APPCOMMAND 消息，但实际音量已通过 COM API 设置
    /// </summary>
    private void TriggerVolumeOSD(int appCommand)
    {
        // 构造 lParam: 高位字 = appCommand, 低位字 = 设备标识符
        IntPtr lParam = new IntPtr(appCommand << 16);

        // 向所有顶级窗口广播消息，触发系统音量 UI
        SendMessageTimeout(
            HWND_BROADCAST,
            WM_APPCOMMAND,
            IntPtr.Zero,
            lParam,
            SendMessageTimeoutFlags.SMTO_ABORTIFHUNG,
            100,
            out _
        );
    }

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern IntPtr SendMessageTimeout(
        IntPtr hWnd,
        uint Msg,
        IntPtr wParam,
        IntPtr lParam,
        SendMessageTimeoutFlags fuFlags,
        uint uTimeout,
        out IntPtr lpdwResult
    );

    [Flags]
    private enum SendMessageTimeoutFlags : uint
    {
        SMTO_NORMAL = 0x0,
        SMTO_BLOCK = 0x1,
        SMTO_ABORTIFHUNG = 0x2,
        SMTO_NOTIMEOUTIFNOTHUNG = 0x8
    }


    public void Dispose()
    {
        _deviceEnumerator?.Dispose();
        _defaultDevice?.Dispose();
    }
}
