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

public class MyTestsView : IEvolView
{
    private IStudentContext? _context;

    public new string Name => "Dolgozataim";
    public string Description => "A rögzített dolgozatok és az azokra kapott érdemjegyek listája, kártya nézetben.";

    public MyTestsView() { }

    public MyTestsView(IStudentContext context)
    {
        _context = context;
    }

    public Control CreateView()
    {
        var scrollViewer = new ScrollViewer();
        var wrapPanel = new WrapPanel
        {
            Margin = new Thickness(15),
            Orientation = Orientation.Horizontal,
        };

        if (_context == null)
        {
            wrapPanel.Children.Add(new TextBlock { Text = "Hiba: A nézet nincs megfelelően inicializálva." });
            return wrapPanel;
        }

        try
        {
            var myGrades = _context.GetMyGrades();
            var allTests = _context.QueryEntities("TestRecord");

            var testLookup = allTests
                .Where(t => t.Data.ContainsKey("Subject") && t.Data.ContainsKey("Date"))
                .Select(t =>
                {
                    DateTime.TryParse(t.Data["Date"], CultureInfo.InvariantCulture, DateTimeStyles.None, out var date);
                    return new { Test = t, Date = date.Date, Subject = t.Data["Subject"] };
                })
                .Where(t => t.Date != DateTime.MinValue)
                .GroupBy(t => (t.Subject, t.Date))
                .ToDictionary(g => g.Key, g => g.First().Test);

            var testGradesFound = 0;
            foreach (var grade in myGrades.Where(g => g.Subject != null).OrderByDescending(g => g.Date))
            {
                var key = (Subject: grade.Subject.Name, Date: grade.Date.Date);
                if (testLookup.TryGetValue(key, out var testRecord) && testRecord != null)
                {
                    string topic = testRecord.Data.TryGetValue("Topic", out var t) ? t : "Nincs megadva";
                    var card = CreateTestCard(grade.Subject.Name, topic, grade.Value, grade.Date);
                    wrapPanel.Children.Add(card);
                    testGradesFound++;
                }
            }
            
            if (testGradesFound == 0)
            {
                 wrapPanel.Children.Add(new TextBlock 
                 { 
                     Text = "Nincsenek rögzített dolgozatok.",
                     FontSize = 16,
                     HorizontalAlignment = HorizontalAlignment.Center,
                     VerticalAlignment = VerticalAlignment.Center,
                     Margin = new Thickness(20)
                 });
            }
        }
        catch (Exception ex)
        {
            wrapPanel.Children.Add(new TextBlock { Text = $"Hiba történt az adatok betöltése közben: {ex.Message}" });
        }

        scrollViewer.Content = wrapPanel;
        return scrollViewer;
    }

    private Border CreateTestCard(string subject, string topic, int gradeValue, DateTime date)
    {
        var gradeDisplay = new Border
        {
            Width = 50,
            Height = 50,
            CornerRadius = new CornerRadius(25),
            Background = GetGradeBrush(gradeValue),
            Child = new TextBlock
            {
                Text = gradeValue.ToString(),
                Foreground = Brushes.White,
                FontSize = 24,
                FontWeight = FontWeight.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }
        };

        var cardGrid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*,Auto")
        };

        var infoPanel = new StackPanel
        {
            Spacing = 4,
            VerticalAlignment = VerticalAlignment.Center
        };
        infoPanel.Children.Add(new TextBlock { Text = subject, FontSize = 18, FontWeight = FontWeight.Bold, TextTrimming = TextTrimming.CharacterEllipsis });
        infoPanel.Children.Add(new TextBlock { Text = topic, FontSize = 14, Foreground = Brushes.Gray, TextTrimming = TextTrimming.CharacterEllipsis });
        infoPanel.Children.Add(new TextBlock { Text = date.ToString("yyyy. MMMM dd.", new CultureInfo("hu-HU")), FontSize = 12, Foreground = Brushes.DarkGray });

        Grid.SetColumn(infoPanel, 0);
        cardGrid.Children.Add(infoPanel);

        var gradePanel = new StackPanel
        {
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(10, 0, 0, 0)
        };
        gradePanel.Children.Add(gradeDisplay);
        
        Grid.SetColumn(gradePanel, 1);
        cardGrid.Children.Add(gradePanel);

        var cardBorder = new Border
        {
            Background = Brushes.WhiteSmoke,
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(15),
            Margin = new Thickness(10),
            MinWidth = 300,
            MaxWidth = 450,
            BoxShadow = new BoxShadows(new BoxShadow { Blur = 10, OffsetX = 2, OffsetY = 2, Color = Color.FromArgb(40, 0, 0, 0) }),
            Child = cardGrid
        };

        return cardBorder;
    }

    private IBrush GetGradeBrush(int grade)
    {
        switch (grade)
        {
            case 5: return Brushes.Green;
            case 4: return Brushes.DodgerBlue;
            case 3: return Brushes.Orange;
            case 2: return Brushes.DarkOrange;
            case 1: return Brushes.Crimson;
            default: return Brushes.Gray;
        }
    }
}
