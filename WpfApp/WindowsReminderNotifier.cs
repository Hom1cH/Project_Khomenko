using System.Drawing;
using DotNet_Lab01_Core;
using Forms = System.Windows.Forms;

namespace WpfApp;

public sealed class WindowsReminderNotifier : IReminderNotifier, IDisposable
{
    private readonly Forms.NotifyIcon _notifyIcon;
    private bool _disposed;

    public WindowsReminderNotifier()
    {
        _notifyIcon = new Forms.NotifyIcon
        {
            Icon = SystemIcons.Information,
            Visible = true,
            Text = "Education Dashboard"
        };
    }

    public void Show(string title, string message)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(WindowsReminderNotifier));

        _notifyIcon.ShowBalloonTip(7000, title, message, Forms.ToolTipIcon.Warning);
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _notifyIcon.Visible = false;
        _notifyIcon.Dispose();
        _disposed = true;
    }
}
