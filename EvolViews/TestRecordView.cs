using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Kreta.Core;
using Kreta.Contexts;
using System.Globalization;

public class TestRecordView : IEvolView
{
    private ITeacherContext? _context;
    private ComboBox? _classComboBox;
    private ComboBox? _subjectComboBox;
    private TextBox? _topicTextBox;
    private DatePicker? _datePicker;
    private TextBlock? _statusTextBlock;

    public new string Name => "Dolgozat rögzítése";
    public string Description => "Új dolgozat adatainak rögzítése egy osztály számára.";

    public TestRecordView() { }

    public TestRecordView(ITeacherContext context)
    {
        _context = context;
    }

    private Grid CreateInputRow(string iconPath, string labelText, Control inputControl)
    {
        var grid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("Auto, *"),
            Margin = new Thickness(0, 0, 0, 15),
            VerticalAlignment = VerticalAlignment.Center
        };

        var icon = new PathIcon
        {
            Data = StreamGeometry.Parse(iconPath),
            Width = 22,
            Height = 22,
            Foreground = Brush.Parse("#4A90E2")
        };

        var label = new TextBlock
        {
            Text = labelText,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(10, 0, 15, 0),
            FontWeight = FontWeight.Medium
        };

        var labelPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            VerticalAlignment = VerticalAlignment.Center
        };
        labelPanel.Children.Add(icon);
        labelPanel.Children.Add(label);

        Grid.SetColumn(labelPanel, 0);
        grid.Children.Add(labelPanel);

        inputControl.VerticalAlignment = VerticalAlignment.Center;
        Grid.SetColumn(inputControl, 1);
        grid.Children.Add(inputControl);

        return grid;
    }

    public Control CreateView()
    {
        _classComboBox = new ComboBox
        {
            PlaceholderText = "Válasszon osztályt...",
            ItemsSource = _context?.GetAllClasses() ?? new List<string>(),
            HorizontalAlignment = HorizontalAlignment.Stretch
        };

        _subjectComboBox = new ComboBox
        {
            PlaceholderText = "Válasszon tantárgyat...",
            ItemsSource = _context?.GetAllSubjects().Select(s => s.Name).ToList() ?? new List<string>(),
            HorizontalAlignment = HorizontalAlignment.Stretch
        };

        _topicTextBox = new TextBox
        {
            PlaceholderText = "A dolgozat témája",
            HorizontalAlignment = HorizontalAlignment.Stretch
        };

        _datePicker = new DatePicker
        {
            SelectedDate = DateTime.Today,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };

        var mainStackPanel = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Spacing = 5,
            Width = 450
        };

        var title = new TextBlock
        {
            Text = "Új Dolgozat Rögzítése",
            FontSize = 22,
            FontWeight = FontWeight.Bold,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 0, 0, 25),
            Foreground = Brush.Parse("#333333")
        };
        mainStackPanel.Children.Add(title);

        mainStackPanel.Children.Add(CreateInputRow("M12,5.5A3.5,3.5 0 0,1 15.5,9A3.5,3.5 0 0,1 12,12.5A3.5,3.5 0 0,1 8.5,9A3.5,3.5 0 0,1 12,5.5M5,8C5.5,8 6,8.5 6,9C6,9.5 5.5,10 5,10C4.5,10 4,9.5 4,9C4,8.5 4.5,8 5,8M19,8C19.5,8 20,8.5 20,9C20,9.5 19.5,10 19,10C18.5,10 18,9.5 18,9C18,8.5 18.5,8 19,8M12,15.5C14.67,15.5 20,17.17 20,19.5V21H4V19.5C4,17.17 9.33,15.5 12,15.5Z", "Osztály:", _classComboBox));
        mainStackPanel.Children.Add(CreateInputRow("M6,2A2,2 0 0,0 4,4V20A2,2 0 0,0 6,22H18A2,2 0 0,0 20,20V4A2,2 0 0,0 18,2H6M6,4H18V20H6V4M8,6V8H16V6H8M8,10V12H13V10H8Z", "Tantárgy:", _subjectComboBox));
        mainStackPanel.Children.Add(CreateInputRow("M20.71,7.04C21.1,6.65 21.1,6 20.71,5.63L18.37,3.29C18,2.9 17.35,2.9 16.96,3.29L15.12,5.12L18.87,8.87M3,17.25V21H6.75L17.81,9.93L14.06,6.18L3,17.25Z", "Téma:", _topicTextBox));
        mainStackPanel.Children.Add(CreateInputRow("M19,19H5V8H19M16,1V3H8V1H6V3H5C3.89,3 3,3.89 3,5V19A2,2 0 0,0 5,21H19A2,2 0 0,0 21,19V5C21,3.89 20.1,3 19,3H18V1M17,10H12V15H17V10Z", "Dátum:", _datePicker));

        var saveButtonIcon = new PathIcon
        {
            Data = StreamGeometry.Parse("M15,9H5V5H15V9M12,19C10.34,19 9,17.66 9,16C9,14.34 10.34,13 12,13C13.66,13 15,14.34 15,16C15,17.66 13.66,19 12,19M17,3H5A2,2 0 0,0 3,5V19A2,2 0 0,0 5,21H19A2,2 0 0,0 21,19V7L17,3Z"),
            Width = 16, Height = 16
        };
        var saveButtonContent = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center };
        saveButtonContent.Children.Add(saveButtonIcon);
        saveButtonContent.Children.Add(new TextBlock { Text = "Mentés", VerticalAlignment = VerticalAlignment.Center });

        var saveButton = new Button
        {
            Content = saveButtonContent,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Margin = new Thickness(0, 20, 0, 10),
            Padding = new Thickness(15, 10),
            Background = Brush.Parse("#4A90E2"),
            Foreground = Brushes.White,
            CornerRadius = new CornerRadius(5)
        };
        saveButton.Click += SaveButton_Click;
        mainStackPanel.Children.Add(saveButton);

        _statusTextBlock = new TextBlock
        {
            Text = "",
            HorizontalAlignment = HorizontalAlignment.Center
        };
        mainStackPanel.Children.Add(_statusTextBlock);

        var card = new Border
        {
            Child = mainStackPanel,
            Padding = new Thickness(30),
            Background = Brushes.White,
            CornerRadius = new CornerRadius(10),
            BoxShadow = new BoxShadows(new BoxShadow { Blur = 20, OffsetX = 0, OffsetY = 5, Color = Color.Parse("#1A000000") })
        };

        return new Panel
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Children = { card }
        };
    }

    private void SaveButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (_context == null || _statusTextBlock == null || _classComboBox == null || _subjectComboBox == null || _topicTextBox == null || _datePicker == null)
        {
            if (_statusTextBlock != null)
            {
                _statusTextBlock.Text = "Hiba: A modul nincs megfelelően beállítva.";
                _statusTextBlock.Foreground = Brushes.Red;
            }
            return;
        }

        var selectedClass = _classComboBox.SelectedItem as string;
        var selectedSubject = _subjectComboBox.SelectedItem as string;
        var topic = _topicTextBox.Text;
        var selectedDate = _datePicker.SelectedDate;

        if (string.IsNullOrWhiteSpace(selectedClass) || 
            string.IsNullOrWhiteSpace(selectedSubject) || 
            string.IsNullOrWhiteSpace(topic) || 
            !selectedDate.HasValue)
        {
            _statusTextBlock.Text = "Hiba: Minden mező kitöltése kötelező!";
            _statusTextBlock.Foreground = Brushes.Red;
            return;
        }

        var dataToSave = new Dictionary<string, string>
        {
            { "Osztaly", selectedClass },
            { "Tantargy", selectedSubject },
            { "Tema", topic },
            { "Datum", selectedDate.Value.ToString("o", CultureInfo.InvariantCulture) }
        };

        try
        {
            _context.SaveEntity("TestRecord", dataToSave);
            _statusTextBlock.Text = "A dolgozat sikeresen rögzítve!";
            _statusTextBlock.Foreground = Brushes.Green;

            _classComboBox.SelectedIndex = -1;
            _subjectComboBox.SelectedIndex = -1;
            _topicTextBox.Text = "";
            _datePicker.SelectedDate = DateTime.Today;
        }
        catch (Exception ex)
        {
            _statusTextBlock.Text = $"Hiba történt a mentés során: {ex.Message}";
            _statusTextBlock.Foreground = Brushes.Red;
        }
    }
}
