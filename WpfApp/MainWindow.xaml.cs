using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using DotNet_Lab01_Core;
using Button = System.Windows.Controls.Button;
using ContextMenu = System.Windows.Controls.ContextMenu;
using MenuItem = System.Windows.Controls.MenuItem;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace WpfApp;

public partial class MainWindow : Window
{
    private readonly Reminder _reminder;
    private readonly System.Windows.Threading.DispatcherTimer _deadlineTimer;
    private readonly MainViewModel _viewModel;
    private System.Windows.Forms.NotifyIcon? _trayIcon;

    public MainWindow()
    {
        InitializeComponent();
        _viewModel = (MainViewModel)DataContext;
        _viewModel.AddObjectRequested += AddObject;
        _viewModel.GamblingRequested += OpenGambling;
        _viewModel.ImportRequested += ImportZip;
        _viewModel.ExportRequested += ExportZip;
        _viewModel.CoursesSaved += ShowSaveMessage;
        _viewModel.BackToCoursesRequested += ShowCoursesPanel;
        _viewModel.CurrentViewChanged += ShowCurrentPanel;
        _viewModel.ConfirmDeleteCourse = ConfirmDeleteCourse;
        _viewModel.ConfirmDeleteTask = ConfirmDeleteTask;
        _viewModel.SelectTagFilters = SelectTagFilters;

        _reminder = new Reminder(new WindowsReminderNotifier());
        _deadlineTimer = new System.Windows.Threading.DispatcherTimer
        {
            Interval = TimeSpan.FromMinutes(30)
        };
        _deadlineTimer.Tick += (_, _) => CheckDeadlineReminders();

        _viewModel.LoadData();
        ThemeManager.ApplyTheme(_viewModel.Theme);
        LoadApplicationLogo();
        LoadTrayIcon();
        ShowCourses();
        CheckDeadlineReminders();
        _deadlineTimer.Start();
    }

    protected override void OnClosed(EventArgs e)
    {
        _deadlineTimer.Stop();
        _viewModel.AddObjectRequested -= AddObject;
        _viewModel.GamblingRequested -= OpenGambling;
        _viewModel.ExportRequested -= ExportZip;
        _viewModel.ImportRequested -= ImportZip;
        _viewModel.CoursesSaved -= ShowSaveMessage;
        _viewModel.BackToCoursesRequested -= ShowCoursesPanel;
        _viewModel.CurrentViewChanged -= ShowCurrentPanel;
        _viewModel.ConfirmDeleteCourse = null;
        _viewModel.ConfirmDeleteTask = null;
        _viewModel.SelectTagFilters = null;
        _trayIcon?.Dispose();
        _reminder.Dispose();
        _viewModel.Dispose();
        base.OnClosed(e);
    }

    private void ShowCourses()
    {
        _viewModel.ShowCourses();
        CoursesView.Visibility = Visibility.Visible;
        TasksView.Visibility = Visibility.Collapsed;
        TaskDetailView.Visibility = Visibility.Collapsed;
        ShowSummarySidebar();
    }

    private void ShowTasks(Course course)
    {
        _viewModel.ShowTasks(course);
        CoursesView.Visibility = Visibility.Collapsed;
        TasksView.Visibility = Visibility.Visible;
        TaskDetailView.Visibility = Visibility.Collapsed;
        ShowCourseSidebar(course);
    }

    private void ShowSummarySidebar()
    {
        SummaryPanel.Visibility = Visibility.Visible;
        SelectedCoursePanel.Visibility = Visibility.Collapsed;
        SelectedTaskPanel.Visibility = Visibility.Collapsed;
    }

    private void ShowCourseSidebar(Course course)
    {
        SummaryPanel.Visibility = Visibility.Collapsed;
        SelectedCoursePanel.Visibility = Visibility.Visible;
        SelectedTaskPanel.Visibility = Visibility.Collapsed;

        _viewModel.ShowSelectedCourse(course);
    }

    private void CourseCard_Click(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { Tag: WpfApp.CourseCardViewModel viewModel })
            ShowTasks(viewModel.Course);
    }

    private void TaskCard_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: WpfApp.TaskCardViewModel viewModel })
            ShowTaskDetails(viewModel.Task);
    }

    private void TaskCard_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is Border { Tag: WpfApp.TaskCardViewModel viewModel })
        {
            ShowTaskDetails(viewModel.Task);
            e.Handled = true;
        }
    }

    private void BackToTasks_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel.SelectedCourse != null)
            ShowTasks(_viewModel.SelectedCourse);
        else
            ShowCourses();
    }

    private void AddTaskImage_Click(object? sender, RoutedEventArgs? e)
    {
        if (_viewModel.SelectedTask == null)
            return;

        OpenFileDialog dialog = new OpenFileDialog
        {
            Title = "Select image",
            Filter = "Image files (*.png;*.jpg;*.jpeg;*.bmp;*.gif)|*.png;*.jpg;*.jpeg;*.bmp;*.gif|All files (*.*)|*.*",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)
        };

        if (dialog.ShowDialog(this) != true)
            return;

        try
        {
            string fileName = Path.GetFileName(dialog.FileName);
            string taskImagesDir = Path.Combine(_viewModel.DataDirectory, "task_images");
            Directory.CreateDirectory(taskImagesDir);

            string destPath = GetUniqueDestinationPath(taskImagesDir, fileName);
            File.Copy(dialog.FileName, destPath);

            _viewModel.AddImageToSelectedTask(destPath);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error adding image: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void AttachTaskFile_Click(object? sender, RoutedEventArgs? e)
    {
        if (_viewModel.SelectedTask == null)
            return;

        OpenFileDialog dialog = new OpenFileDialog
        {
            Title = "Select file to attach",
            Filter = "All files (*.*)|*.*",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
        };

        if (dialog.ShowDialog(this) != true)
            return;

        try
        {
            string fileName = Path.GetFileName(dialog.FileName);
            string taskFilesDir = Path.Combine(_viewModel.DataDirectory, "task_files");
            Directory.CreateDirectory(taskFilesDir);

            string destPath = GetUniqueDestinationPath(taskFilesDir, fileName);
            File.Copy(dialog.FileName, destPath);

            _viewModel.AddAttachmentToSelectedTask(destPath);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error attaching file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ShowTaskDetails(ParacTask task)
    {
        _viewModel.ShowTaskDetails(task);
        CoursesView.Visibility = Visibility.Collapsed;
        TasksView.Visibility = Visibility.Collapsed;
        TaskDetailView.Visibility = Visibility.Visible;

        SummaryPanel.Visibility = Visibility.Collapsed;
        SelectedCoursePanel.Visibility = Visibility.Collapsed;
        SelectedTaskPanel.Visibility = Visibility.Visible;
    }
    private void OpenGambling()
    {
        if (_viewModel.GamblingApps.Count == 0)
        {
            MessageBox.Show("Якщо ви не заєте з чого почати?  Хай це зробить рандом!\n\nДодайте .exe файли програм в Settings → Gambling Apps,\nі хай вам допоможе Фортуна.",
                "Gambling", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        GamblingWindow window = new GamblingWindow(_viewModel.GamblingApps)
        {
            Owner = this
        };
        window.ShowDialog();
    }
    private void ExportZip()
    {
        var dialog = new Microsoft.Win32.SaveFileDialog
        {
            Title = "Export to ZIP",
            Filter = "ZIP files (*.zip)|*.zip",
            FileName = $"education_backup_{DateTime.Now:yyyy-MM-dd}"
        };

        if (dialog.ShowDialog(this) != true) return;

        try
        {
            ZipExportImport.Export(
                _viewModel.DataDirectory,
                _viewModel.ExportDirectory, // <- додали
                dialog.FileName);

            MessageBox.Show($"Exported to:\n{dialog.FileName}", "Export",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Export error:\n{ex.Message}", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ImportZip()
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Title = "Import from ZIP",
            Filter = "ZIP files (*.zip)|*.zip"
        };

        if (dialog.ShowDialog(this) != true) return;

        var confirm = MessageBox.Show(
            "Импорт заменит все текущие данные. Продолжить?",
            "Импорт",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (confirm != MessageBoxResult.Yes) return;

        try
        {
            ZipExportImport.Import(
                dialog.FileName,
                _viewModel.DataDirectory,
                _viewModel.ExportDirectory);

            string jsonPath = Path.Combine(_viewModel.ExportDirectory, "courses.json");
            _viewModel.ImportCoursesAndRefresh(jsonPath);

            ShowCoursesPanel();
            MessageBox.Show("Import successful!", "Import",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Import error:\n{ex.Message}", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void SettingsButton_Click(object sender, RoutedEventArgs e)
    {
        SettingsWindow dialog = new SettingsWindow(
            _viewModel.ExportDirectory,
            _viewModel.Theme,
            _viewModel.ReminderEnabled,
            _viewModel.GamblingApps)   // <- додати
        {
            Owner = this
        };
        dialog.ApplyRequested += ApplySettings;
        dialog.ShowDialog();
    }

    private void AddObject()
    {
        if (TasksView.Visibility == Visibility.Visible && _viewModel.SelectedCourse != null)
        {
            AddTaskToSelectedCourse();
            return;
        }

        AddCourse();
    }

    private void AddCourse()
    {
        CourseDialog dialog = new CourseDialog
        {
            Owner = this
        };

        if (dialog.ShowDialog() != true)
            return;

        _viewModel.CreateCourse(
            dialog.CourseTitle,
            dialog.CourseDescription,
            dialog.Credits,
            dialog.Difficulty,
            dialog.Deadline,
            dialog.Progress,
            dialog.Tags);

        ShowCourses();
    }

    private void AddTaskToSelectedCourse()
    {
        if (_viewModel.SelectedCourse == null)
            return;

        TaskDialog dialog = new TaskDialog
        {
            Owner = this
        };

        if (dialog.ShowDialog() != true)
            return;

        _viewModel.CreateTaskForSelectedCourse(
            dialog.TaskTitle,
            dialog.Deadline,
            dialog.Difficulty,
            dialog.Credits,
            dialog.TaskDescription,
            dialog.Progress,
            dialog.Tags);

        ShowCourseViewIfSelected();
        CheckDeadlineReminders();
    }

    private void CourseCardSettings_Click(object sender, RoutedEventArgs e)
    {
        e.Handled = true;

        if (sender is Button button && button.ContextMenu != null)
        {
            button.ContextMenu.PlacementTarget = button;
            button.ContextMenu.IsOpen = true;
        }
    }

    private void TaskCardSettings_Click(object sender, RoutedEventArgs e)
    {
        e.Handled = true;

        if (sender is Button button && button.ContextMenu != null)
        {
            button.ContextMenu.PlacementTarget = button;
            button.ContextMenu.IsOpen = true;
        }
    }

    private void TaskDetailIMGSettings_Click(object sender, RoutedEventArgs e) 
    {
        e.Handled = true;
        if (sender is Button button && button.ContextMenu != null)
        {
            button.ContextMenu.PlacementTarget = button;
            button.ContextMenu.IsOpen = true;
        }
    }
    private void TaskDetailFileSettings_Click(object sender, RoutedEventArgs e)
    {
        e.Handled = true;
        if (sender is Button button && button.ContextMenu != null)
        {
            button.ContextMenu.PlacementTarget = button;
            button.ContextMenu.IsOpen = true;
        }
    }

    // ─── Image context menu ───────────────────────────────────────────

    private void OpenImageLocation_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem { Tag: string path }) return;
        OpenFileLocation(path);
    }

    private void OpenImageWith_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem { Tag: string path }) return;
        OpenWithDialog(path);
    }

    private void DeleteImageFromDevice_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem { Tag: string path }) return;

        var result = MessageBox.Show(
            $"Удалить файл с устройства?\n\n{path}",
            "Удалить с устройства",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (result != MessageBoxResult.Yes) return;

        _viewModel.RemoveImageFromSelectedTask(path);
        TryDeleteFileFromDisk(path);
    }

    // ─── File context menu ────────────────────────────────────────────

    private void OpenFileLocation_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem { Tag: string path }) return;
        OpenFileLocation(path);
    }

    private void OpenFileWith_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem { Tag: string path }) return;
        OpenWithDialog(path);
    }

    private void DeleteFileFromDevice_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem { Tag: string path }) return;

        var result = MessageBox.Show(
            $"Удалить файл с устройства?\n\n{path}",
            "Удалить с устройства",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (result != MessageBoxResult.Yes) return;

        _viewModel.RemoveAttachmentFromSelectedTask(path);
        TryDeleteFileFromDisk(path);
    }

    // ─── Helpers ──────────────────────────────────────────────────────

    private static void OpenFileLocation(string path)
    {
        try
        {
            if (!File.Exists(path))
            {
                MessageBox.Show("Файл не найден на диске.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Открываем проводник с выделением файла
            System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{path}\"");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private static void OpenWithDialog(string path)
    {
        try
        {
            if (!File.Exists(path))
            {
                MessageBox.Show("Файл не найден на диске.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Стандартное диалоговое окно Windows "Открыть с помощью"
            OpenWithWindowsDialog(path);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private static void OpenWithWindowsDialog(string path)
    {
        try
        {
            string openWithPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.System),
                "OpenWith.exe");

            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = openWithPath,
                Arguments = $"\"{path}\"",
                UseShellExecute = false
            });
        }
        catch
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "rundll32.exe",
                Arguments = $"shell32.dll,OpenAs_RunDLL \"{path}\"",
                UseShellExecute = false
            });
        }
    }

    private static string GetUniqueDestinationPath(string directory, string fileName)
    {
        string safeFileName = Path.GetFileName(fileName);
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(safeFileName);
        string extension = Path.GetExtension(safeFileName);
        string destinationPath = Path.Combine(directory, safeFileName);
        int copyIndex = 1;

        while (File.Exists(destinationPath))
        {
            destinationPath = Path.Combine(directory, $"{fileNameWithoutExtension} ({copyIndex}){extension}");
            copyIndex++;
        }

        return destinationPath;
    }

    private static void TryDeleteFileFromDisk(string path)
    {
        try
        {
            if (File.Exists(path))
                File.Delete(path);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Не удалось удалить файл с диска:\n{ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void EditCourseMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem { Parent: ContextMenu { PlacementTarget: Button { Tag: WpfApp.CourseCardViewModel viewModel } } })
            return;

        CourseDialog dialog = new CourseDialog(viewModel.Course)
        {
            Owner = this
        };

        if (dialog.ShowDialog() != true)
            return;

        _viewModel.UpdateCourse(
            viewModel.Course,
            dialog.CourseTitle,
            dialog.CourseDescription,
            dialog.Credits,
            dialog.Difficulty,
            dialog.Progress,
            dialog.Deadline,
            dialog.Tags);

        ShowCourses();
        CheckDeadlineReminders();
    }

    private void EditTaskMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem { Parent: ContextMenu { PlacementTarget: Button { Tag: WpfApp.TaskCardViewModel viewModel } } })
            return;

        TaskDialog dialog = new TaskDialog(viewModel.Task)
        {
            Owner = this
        };

        if (dialog.ShowDialog() != true)
            return;

        Course? course = _viewModel.UpdateTask(
            viewModel.Task,
            dialog.TaskTitle,
            dialog.TaskDescription,
            dialog.Credits,
            dialog.Difficulty,
            dialog.Progress,
            dialog.Deadline,
            dialog.Tags);

        ShowCurrentView(course);

        CheckDeadlineReminders();
    }

    private bool ConfirmDeleteCourse(CourseCardViewModel viewModel)
    {
        MessageBoxResult result = MessageBox.Show(
            $"Delete course \"{viewModel.Title}\"?",
            "Delete",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        return result == MessageBoxResult.Yes;
    }

    private bool ConfirmDeleteTask(TaskCardViewModel viewModel)
    {
        MessageBoxResult result = MessageBox.Show(
            $"Delete task \"{viewModel.Title}\"?",
            "Delete",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        return result == MessageBoxResult.Yes;
    }

    private void CheckDeadlineReminders()
    {
        if (!_viewModel.ReminderEnabled)
            return;

        _reminder.CheckDeadlines(_viewModel.CourseManager.GetCourses());
    }

    private void SortButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.ContextMenu != null)
        {
            button.ContextMenu.PlacementTarget = button;
            button.ContextMenu.IsOpen = true;
        }
    }

    private void Status_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        e.Handled = true;

        if (e.ClickCount != 2)
            return;

        if (sender is TextBlock { ContextMenu: not null } statusText)
        {
            statusText.ContextMenu.PlacementTarget = statusText;
            statusText.ContextMenu.IsOpen = true;
        }
    }

    private void ShowCurrentView(Course? course)
    {
        if (course != null)
            ShowTasks(course);
        else
            ShowCourses();
    }

    private void ShowCurrentPanel()
    {
        if (TaskDetailView.Visibility == Visibility.Visible && _viewModel.SelectedTask != null)
            ShowTaskDetailsPanel(_viewModel.SelectedTask);
        else if (_viewModel.SelectedCourse != null)
            ShowTasksPanel(_viewModel.SelectedCourse);
        else
            ShowCoursesPanel();

        CheckDeadlineReminders();
    }

    private void ShowCoursesPanel()
    {
        CoursesView.Visibility = Visibility.Visible;
        TasksView.Visibility = Visibility.Collapsed;
        TaskDetailView.Visibility = Visibility.Collapsed;
        ShowSummarySidebar();
        CheckDeadlineReminders();
    }

    private void ShowTasksPanel(Course course)
    {
        CoursesView.Visibility = Visibility.Collapsed;
        TasksView.Visibility = Visibility.Visible;
        TaskDetailView.Visibility = Visibility.Collapsed;
        ShowCourseSidebar(course);
    }

    private void ShowTaskDetailsPanel(ParacTask task)
    {
        CoursesView.Visibility = Visibility.Collapsed;
        TasksView.Visibility = Visibility.Collapsed;
        TaskDetailView.Visibility = Visibility.Visible;
        SummaryPanel.Visibility = Visibility.Collapsed;
        SelectedCoursePanel.Visibility = Visibility.Collapsed;
        SelectedTaskPanel.Visibility = Visibility.Visible;
        _viewModel.ShowTaskDetails(task);
    }

    private void ShowSaveMessage()
    {
        MessageBox.Show(
            $"Courses saved.\nJSON file:\n{_viewModel.JsonFilePath}\n\nXML file:\n{_viewModel.XmlFilePath}",
            "Save",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    private void ApplySettings(SettingsWindow dialog)
    {
        bool wasTaskDetailsOpen = TaskDetailView.Visibility == Visibility.Visible;
        ParacTask? selectedTask = _viewModel.SelectedTask;

        _viewModel.UpdateSettings(dialog.ExportDirectory, dialog.SelectedTheme, dialog.ReminderEnabled, dialog.GamblingApps);
        ThemeManager.ApplyTheme(dialog.SelectedTheme);

        if (wasTaskDetailsOpen && selectedTask != null)
            ShowTaskDetailsPanel(selectedTask);
        else
        {
            _viewModel.RefreshCurrentView();
            ShowCurrentPanel();
        }
    }

    private IEnumerable<string>? SelectTagFilters(IEnumerable<string> tags)
    {
        TagFilterWindow dialog = new TagFilterWindow(tags)
        {
            Owner = this
        };

        if (dialog.ShowDialog() != true)
            return null;

        return dialog.SelectedTags;
    }

    private void LoadApplicationLogo()
    {
        string iconPath = Path.Combine(AppContext.BaseDirectory, "Assets", "AppIcon.png");

        if (!File.Exists(iconPath))
            return;

        BitmapImage icon = new BitmapImage();
        icon.BeginInit();
        icon.CacheOption = BitmapCacheOption.OnLoad;
        icon.UriSource = new Uri(iconPath, UriKind.Absolute);
        icon.EndInit();
        icon.Freeze();

        Icon = icon;
    }

    private void LoadTrayIcon()
    {
        string iconPath = Path.Combine(AppContext.BaseDirectory, "Assets", "AppIcon.ico");

        if (!File.Exists(iconPath))
            return;

        _trayIcon = new System.Windows.Forms.NotifyIcon
        {
            Icon = new System.Drawing.Icon(iconPath),
            Text = "Education Dashboard",
            Visible = true
        };

        _trayIcon.ShowBalloonTip(
            3000,
            "Education Dashboard",
            "Application is running.",
            System.Windows.Forms.ToolTipIcon.Info);
    }

    private void ShowCourseViewIfSelected()
    {
        if (_viewModel.SelectedCourse != null)
            ShowTasks(_viewModel.SelectedCourse);
        else
            ShowCourses();
    }

    private void EditDescription_Click(object sender, RoutedEventArgs e)
    {
        var editPanel = FindName("DescriptionEditPanel") as StackPanel;
        var viewBorder = FindName("DescriptionViewBorder") as Border;
        var editBox = FindName("DescriptionEditBox") as System.Windows.Controls.TextBox;

        if (editPanel != null && viewBorder != null && editBox != null)
        {
            viewBorder.Visibility = Visibility.Collapsed;
            editPanel.Visibility = Visibility.Visible;
            editBox.Focus();
            editBox.SelectAll();
        }
    }
    private void GamblingButton_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel.GamblingApps.Count == 0)
        {
            MessageBox.Show("Якщо ви не заєте з чого почати, хай це зробить рандом.Додайте .exe файли програм в Settings → Gambling Apps, і хай вам допоможе Фортуна.",
                "Gambling", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        GamblingWindow window = new GamblingWindow(_viewModel.GamblingApps)
        {
            Owner = this
        };
        window.ShowDialog();
    }
    private void SaveDescription_Click(object sender, RoutedEventArgs e)
    {
        var editBox = FindName("DescriptionEditBox") as System.Windows.Controls.TextBox;
        var editPanel = FindName("DescriptionEditPanel") as StackPanel;
        var viewBorder = FindName("DescriptionViewBorder") as Border;

        if (_viewModel.SelectedTask != null && editBox != null)
        {
            _viewModel.SelectedTask.TaskDescription = editBox.Text;
            _viewModel.CourseManager.SaveChanges();
        }

        if (editPanel != null && viewBorder != null)
        {
            editPanel.Visibility = Visibility.Collapsed;
            viewBorder.Visibility = Visibility.Visible;
        }
    }

    private void CancelDescription_Click(object sender, RoutedEventArgs e)
    {
        var editBox = FindName("DescriptionEditBox") as System.Windows.Controls.TextBox;
        var editPanel = FindName("DescriptionEditPanel") as StackPanel;
        var viewBorder = FindName("DescriptionViewBorder") as Border;

        if (editBox != null)
            editBox.Text = _viewModel.SelectedTaskDescription;

        if (editPanel != null && viewBorder != null)
        {
            editPanel.Visibility = Visibility.Collapsed;
            viewBorder.Visibility = Visibility.Visible;
        }
    }

    private void DescriptionViewText_DoubleClick(object sender, MouseButtonEventArgs e)
    {
        var editPanel = FindName("DescriptionEditPanel") as StackPanel;
        var viewBorder = FindName("DescriptionViewBorder") as Border;
        var editBox = FindName("DescriptionEditBox") as System.Windows.Controls.TextBox;

        if (editPanel != null && viewBorder != null && editBox != null)
        {
            viewBorder.Visibility = Visibility.Collapsed;
            editPanel.Visibility = Visibility.Visible;
            editBox.Focus();
            editBox.SelectAll();
        }
    }

    private void DescriptionViewBorder_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
        {
            var editPanel = FindName("DescriptionEditPanel") as StackPanel;
            var viewBorder = FindName("DescriptionViewBorder") as Border;
            var editBox = FindName("DescriptionEditBox") as System.Windows.Controls.TextBox;

            if (editPanel != null && viewBorder != null && editBox != null)
            {
                viewBorder.Visibility = Visibility.Collapsed;
                editPanel.Visibility = Visibility.Visible;
                editBox.Focus();
                editBox.SelectAll();
            }
            e.Handled = true;
        }
    }

    private void OpenTaskImage_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: string imagePath } && !string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = imagePath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening image: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void OpenTaskFile_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: string filePath } && !string.IsNullOrEmpty(filePath) && File.Exists(filePath))
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void FloatingAddButton_Click(object sender, RoutedEventArgs e)
    {
        // If we're on task detail view, show menu for adding images/files
        if (TaskDetailView.Visibility == Visibility.Visible)
        {
            ContextMenu menu = new ContextMenu();

            MenuItem addImageItem = new MenuItem { Header = "Add image" };
            addImageItem.Click += (_, _) => AddTaskImage_Click(null, null);
            menu.Items.Add(addImageItem);

            MenuItem addFileItem = new MenuItem { Header = "Attach file" };
            addFileItem.Click += (_, _) => AttachTaskFile_Click(null, null);
            menu.Items.Add(addFileItem);

            menu.PlacementTarget = FloatingAddButton;
            menu.IsOpen = true;
        }
        else
        {
            // Otherwise, trigger the regular add object command
            AddObject();
        }
    }
}
