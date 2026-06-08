using System.ComponentModel;
using System.Windows;

namespace WpfApp;

public partial class TagFilterWindow : Window
{
    private readonly List<TagFilterItem> _items;

    public TagFilterWindow(IEnumerable<string> tags)
    {
        InitializeComponent();

        _items = tags
            .Select(tag => new TagFilterItem(tag))
            .ToList();

        TagsList.ItemsSource = _items;
    }

    public IEnumerable<string>? SelectedTags { get; private set; }

    private void ApplyButton_Click(object sender, RoutedEventArgs e)
    {
        SelectedTags = _items
            .Where(item => item.IsSelected)
            .Select(item => item.Text)
            .ToList();

        DialogResult = true;
    }

    private void ClearButton_Click(object sender, RoutedEventArgs e)
    {
        SelectedTags = Array.Empty<string>();
        DialogResult = true;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}

public sealed class TagFilterItem : INotifyPropertyChanged
{
    private bool _isSelected;

    public TagFilterItem(string text)
    {
        Text = text;
    }

    public string Text { get; }

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected == value)
                return;

            _isSelected = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
