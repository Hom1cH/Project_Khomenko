using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using HorizontalAlignment = System.Windows.HorizontalAlignment; 
using Brush = System.Windows.Media.Brush;                       
using MessageBox = System.Windows.MessageBox;
namespace WpfApp;

public partial class GamblingWindow : Window
{
    private readonly List<GamblingApp> _apps;
    private readonly Random _random = new();
    private MediaPlayer _spinSound = new();
    private double _offset = 0;
    private double _speed = 18;
    private bool _spinning = false;
    private GamblingApp? _winner;
    private DateTime _lastFrame = DateTime.Now;

    private const double ItemWidth = 100;
    private const double SlowdownRate = 0.994;

    public GamblingWindow(List<GamblingApp> apps)
    {
        InitializeComponent();
        LoadSpinSound();
        _apps = apps;
        BuildIconStrip();
    }

    private void LoadSpinSound()
    {
        try
        {
            string soundPath = Path.Combine(AppContext.BaseDirectory, "Assets", "CSGO Case Opening Sound Effect.mp3");
            if (!File.Exists(soundPath)) return;

            _spinSound.Open(new Uri(soundPath, UriKind.Absolute));
            _spinSound.Volume = 1.0;
        }
        catch { /* звук не критичний */ }
    }

    private void PlaySpinSound()
    {
        try
        {
            _spinSound.Position = TimeSpan.Zero;
            _spinSound.Play();
        }
        catch { }
    }

    private void StopSpinSound()
    {
        try { _spinSound.Stop(); }
        catch { }
    }

    private void StartRendering()
    {
        _lastFrame = DateTime.Now;
        CompositionTarget.Rendering += OnRendering;
    }

    private void StopRendering()
    {
        CompositionTarget.Rendering -= OnRendering;
    }

    private void OnRendering(object? sender, EventArgs e)
    {
        // Дельта часу для frame-rate незалежної анімації
        DateTime now = DateTime.Now;
        double delta = (now - _lastFrame).TotalMilliseconds;
        _lastFrame = now;

        // Обмежуємо delta щоб не було стрибків при втраті фокусу
        if (delta > 50) delta = 50;

        _offset += _speed * (delta / 16.0);

        Canvas.SetLeft(IconStrip, -_offset);

        // Безшовна прокрутка
        double totalWidth = _apps.Count * (ItemWidth + 8);
        if (_offset >= totalWidth * 3)
            _offset -= totalWidth * 3;

        // Гальмування
        _speed *= SlowdownRate;

        if (_speed < 0.3)
        {
            StopRendering();
            _spinning = false;
            SnapToCenter();
        }
    }

    private void SpinButton_Click(object? sender, RoutedEventArgs? e)
    {
        if (_apps.Count == 0) return;
        if (_spinning) return;

        _spinning = true;
        _winner = null;
        ResultText.Text = "";
        _speed = _random.Next(80, 160);

        StartRendering();
        PlaySpinSound();
    }

    // Відписатись при закритті вікна — інакше буде memory leak
    protected override void OnClosed(EventArgs e)
    {
        StopRendering();
        StopSpinSound();
        _spinSound.Close();
        base.OnClosed(e);
    }

    // Будуємо стрічку з іконок (з повторами для безшовної прокрутки)
    private async void BuildIconStrip()
    {
        IconStrip.Children.Clear();

        if (_apps.Count == 0) return;

        // Витягуємо іконки в background потоці щоб не блокувати UI
        List<(GamblingApp app, BitmapSource? icon)> items = await Task.Run(() =>
            _apps.Select(app => (app, GamblingIconExtractor.Extract(app.ExePath)))
                 .ToList()
        );

        // Повторюємо список для безшовної прокрутки
        List<(GamblingApp app, BitmapSource? icon)> repeated = new();
        for (int i = 0; i < 6; i++)
            repeated.AddRange(items);

        foreach (var (app, icon) in repeated)
        {
            StackPanel item = new()
            {
                Width = ItemWidth,
                Height = 100,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                Margin = new Thickness(4, 0, 4, 0)
            };

            if (icon != null)
            {
                item.Children.Add(new System.Windows.Controls.Image
                {
                    Source = icon,
                    Width = 48,
                    Height = 48,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                    Margin = new Thickness(0, 10, 0, 6)
                });
            }

            item.Children.Add(new TextBlock
            {
                Text = app.Name,
                FontSize = 11,
                TextTrimming = TextTrimming.CharacterEllipsis,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                Foreground = (System.Windows.Media.Brush)FindResource("PrimaryTextBrush"),
                MaxWidth = ItemWidth - 8,
                TextAlignment = TextAlignment.Center
            });

            IconStrip.Children.Add(item);
            SpinButton_Click(null, null);
        }
    }
    private void SnapToCenter()
    {
        StopSpinSound();

        double itemFullWidth = ItemWidth + 8;
        double centerX = CarouselCanvas.ActualWidth / 2;

        // Знаходимо який елемент зараз під стрілочкою
        double absoluteCenter = _offset + centerX;
        int rawIndex = (int)(absoluteCenter / itemFullWidth);
        int index = rawIndex % _apps.Count;
        if (index < 0) index += _apps.Count;

        _winner = _apps[index];
        ResultText.Text = $"🎯 {_winner.Name}";

        // Плавно доїхати до центру елемента (без телепортації)
        double snappedOffset = rawIndex * itemFullWidth - centerX + itemFullWidth / 2;
        Canvas.SetLeft(IconStrip, -snappedOffset);

        DispatcherTimer launchTimer = new() { Interval = TimeSpan.FromSeconds(2.5) };
        launchTimer.Tick += (_, _) => { launchTimer.Stop(); LaunchWinner(); };
        launchTimer.Start();
    }
    private void LaunchWinner()
    {
        if (_winner == null || !File.Exists(_winner.ExePath))
        {
            MessageBox.Show("Файл не знайдено.", "Помилка",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = _winner.ExePath,
                UseShellExecute = true
            });
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Помилка запуску:\n{ex.Message}", "Помилка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}