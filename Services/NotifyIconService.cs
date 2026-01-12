using System.Diagnostics;
using System.Reflection;

namespace WindowsMediaController.Services;

public class NotifyIconService : IDisposable
{
    private NotifyIcon? _notifyIcon;
    private readonly IHostApplicationLifetime _applicationLifetime;

    public NotifyIconService(IHostApplicationLifetime applicationLifetime)
    {
        _applicationLifetime = applicationLifetime;
    }

    public void Initialize()
    {
        _notifyIcon = new NotifyIcon
        {
            Icon = SystemIcons.Application, // Replace with your own icon later
            Visible = true,
            Text = "Windows Media Controller"
        };

        var contextMenu = new ContextMenuStrip();
        
        var openConfigItem = new ToolStripMenuItem("打开设置文件目录");
        openConfigItem.Click += (s, e) => OpenConfigDirectory();
        contextMenu.Items.Add(openConfigItem);

        contextMenu.Items.Add(new ToolStripSeparator());

        var aboutItem = new ToolStripMenuItem("About");
        aboutItem.Click += (s, e) => ShowAbout();
        contextMenu.Items.Add(aboutItem);

        var exitItem = new ToolStripMenuItem("退出");
        exitItem.Click += (s, e) => Exit();
        contextMenu.Items.Add(exitItem);

        _notifyIcon.ContextMenuStrip = contextMenu;

        Application.Run();
    }

    private void OpenConfigDirectory()
    {
        string path = AppDomain.CurrentDomain.BaseDirectory;
        Process.Start("explorer.exe", path);
    }

    private void ShowAbout()
    {
        MessageBox.Show($"Windows Media Controller\nVersion: {Assembly.GetEntryAssembly()?.GetName().Version}", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void Exit()
    {
        _notifyIcon?.Dispose();
        _applicationLifetime.StopApplication();
        Application.Exit();
    }

    public void Dispose()
    {
        _notifyIcon?.Dispose();
    }
}
