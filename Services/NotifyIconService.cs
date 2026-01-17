using System.Diagnostics;
using System.Reflection;
using System.Drawing;
using System.IO;

namespace WindowsMediaController.Services;

public class NotifyIconService : IDisposable
{
    private NotifyIcon? _notifyIcon;
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly ILogger<NotifyIconService> _logger;
    private ApplicationContext? _applicationContext;

    public NotifyIconService(IHostApplicationLifetime applicationLifetime, ILogger<NotifyIconService> logger)
    {
        _applicationLifetime = applicationLifetime;
        _logger = logger;
    }

    public void Initialize()
    {
        try
        {
            // 确保 Application 已初始化
            if (Application.MessageLoop == false)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
            }

            // 加载图标
            Icon? icon = null;
            try
            {
                icon = LoadTrayIcon();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "加载托盘图标失败，使用系统默认图标作为后备。");
                System.Diagnostics.Debug.WriteLine($"加载图标失败: {ex.Message}");
                // 使用系统默认图标作为后备
                icon = SystemIcons.Application;
            }

            _notifyIcon = new NotifyIcon
            {
                Icon = icon,
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

            // 使用 ApplicationContext 来运行消息循环，这样可以更好地控制退出
            _applicationContext = new ApplicationContext();
            Application.Run(_applicationContext);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"初始化系统托盘失败: {ex.Message}");
        }
    }

    private Icon LoadTrayIcon()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var baseName = assembly.GetName().Name;
        var resourceName = $"{baseName}.Resources.tray.ico";

        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream != null)
        {
            _logger.LogInformation("托盘图标来自嵌入资源: {ResourceName}", resourceName);
            return new Icon(stream);
        }

        var resourcePath = Path.Combine(AppContext.BaseDirectory, "Resources", "tray.ico");
        if (File.Exists(resourcePath))
        {
            _logger.LogInformation("托盘图标来自文件: {Path}", resourcePath);
            return new Icon(resourcePath);
        }

        var rootPath = Path.Combine(AppContext.BaseDirectory, "tray.ico");
        if (File.Exists(rootPath))
        {
            _logger.LogInformation("托盘图标来自文件: {Path}", rootPath);
            return new Icon(rootPath);
        }

        _logger.LogWarning("托盘图标未找到。嵌入资源名: {ResourceName}; 程序集资源: {Resources}",
            resourceName,
            string.Join(", ", assembly.GetManifestResourceNames()));

        throw new FileNotFoundException("托盘图标未找到", resourceName);
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
        try
        {
            _notifyIcon?.Dispose();
            
            // 退出消息循环，确保线程能够正常结束
            _applicationContext?.ExitThread();
            
            // 如果上面的方法不起作用，强制退出
            Application.Exit();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "释放托盘图标服务时出错");
        }
    }
}
