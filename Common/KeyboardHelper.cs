using System.Runtime.InteropServices;

namespace WindowsMediaController.Services;

/// <summary>
/// 键盘输入模拟帮助类
/// 提供模拟按键按下和释放的功能
/// </summary>
public static class KeyboardHelper
{
    private const uint KEYEVENTF_KEYUP = 0x0002;

    [DllImport("user32.dll")]
    private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

   
    /// <summary>
    /// 模拟按键按下和释放
    /// </summary>
    /// <param name="virtualKeyCode">虚拟键码</param>
    public static void SimulateKeyPress(byte virtualKeyCode)
    {
        // 按下键
        keybd_event(virtualKeyCode, 0, 0, 0);
        // 释放键
        keybd_event(virtualKeyCode, 0, KEYEVENTF_KEYUP, 0);
    }
}
