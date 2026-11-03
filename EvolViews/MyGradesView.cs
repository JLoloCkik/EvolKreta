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

public class MyGradesView : IEvolView
{
    private IStudentContext? _context;

    public new string Name => "Jegyeim";
    public string Description => "A rögzített jegyeid, tantárgyi és teljes átlagod megjelenítése.";

    public MyGradesView() { }

    public MyGradesView(IStudentContext context)
    {
        _context = context;
    }

    public Control CreateView()
    {
        var mainPanel = new StackPanel { Margin = new Thickness(20) };

        if (_context == null)
        {
            mainPanel.Children.Add(new TextBlock { Text = "Hiba: A nézet nincs megfelelően konfigurálva.", Foreground = Brushes.Red });
            return mainPanel;
        }

        var grades = _context.GetMyGrades();

        if (grades == null || !grades.Any())
        {
            mainPanel.Children.Add(new TextBlock { Text = "Nincsenek rögzített jegyeid.", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center });
            return mainPanel;
        }

        // --- Fő átlag --- 
        var overallAverage = grades.Average(g => g.Value);
        var overallAverageCard = new Border
        {
            Background = Brushes.LightGray,
            CornerRadius = new CornerRadius(10),
            Padding = new Thickness(15),
            Margin = new Thickness(0, 0, 0, 20),
            Child = new StackPanel
            {
                Spacing = 5,
                Children =
                {
                    new TextBlock { Text = "Teljes átlag", FontSize = 16, FontWeight = FontWeight.Bold, HorizontalAlignment = HorizontalAlignment.Center },
                    new TextBlock
                    {
                        Text = overallAverage.ToString("F2", CultureInfo.InvariantCulture),
                        FontSize = 32,
                        FontWeight = FontWeight.ExtraBold,
                        HorizontalAlignment = HorizontalAlignment.Center
                    }
                }
            }
        };
        mainPanel.Children.Add(overallAverageCard);

        // --- Tantárgyi átlagok --- 
        mainPanel.Children.Add(new TextBlock { Text = "Tantárgyi átlagok", FontSize = 18, FontWeight = FontWeight.Bold, Margin = new Thickness(0, 0, 0, 10) });
        var subjectAverages = grades
            .GroupBy(g => g.Subject?.Name ?? "Ismeretlen tantárgy")
            .Select(group => new { Subject = group.Key, Average = group.Average(g => g.Value) })
            .OrderBy(a => a.Subject);

        var subjectAveragesPanel = new WrapPanel { Orientation = Orientation.Horizontal };

        foreach (var subjectAvg in subjectAverages)
        {
            var subjectCard = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(240, 240, 240)),
                CornerRadius = new CornerRadius(5),
                Padding = new Thickness(10),
                Margin = new Thickness(0, 0, 10, 10),
                Child = new StackPanel
                {
                    Children =
                    {
                        new TextBlock { Text = subjectAvg.Subject, FontWeight = FontWeight.Bold },
                        new TextBlock { Text = subjectAvg.Average.ToString("F2", CultureInfo.InvariantCulture), Foreground = Brushes.DarkSlateGray }
                    }
                }
            };
            subjectAveragesPanel.Children.Add(subjectCard);
        }
        mainPanel.Children.Add(subjectAveragesPanel);

        // --- Jegyek listája ---
        mainPanel.Children.Add(new TextBlock { Text = "Rögzített jegyek", FontSize = 18, FontWeight = FontWeight.Bold, Margin = new Thickness(0, 20, 0, 10) });
        var gradesScrollViewer = new ScrollViewer();
        var gradesPanel = new StackPanel { Spacing = 10 };

        foreach (var grade in grades.OrderByDescending(g => g.Date))
        {
            var gradeCard = new Border
            {
                Padding = new Thickness(15),
                CornerRadius = new CornerRadius(8),
                Background = Brushes.White,
                BoxShadow = new BoxShadows(new BoxShadow { Blur = 5, OffsetX = 2, OffsetY = 2, Color = Colors.LightGray })
            };

            var grid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitions("Auto, *, Auto")
            };

            var gradeCircle = new Border
            {
                Width = 40, Height = 40,
                CornerRadius = new CornerRadius(20),
                Background = GetGradeBrush(grade.Value),
                Child = new TextBlock
                {
                    Text = grade.Value.ToString(),
                    Foreground = Brushes.White,
                    FontSize = 18,
                    FontWeight = FontWeight.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                }
            };

            var subjectText = new TextBlock
            {
                Text = grade.Subject?.Name ?? "N/A",
                FontSize = 16,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(15, 0)
            };

            var dateText = new TextBlock
            {
                Text = grade.Date.ToString("yyyy.MM.dd."),
                Foreground = Brushes.Gray,
                VerticalAlignment = VerticalAlignment.Center
            };

            Grid.SetColumn(gradeCircle, 0);
            Grid.SetColumn(subjectText, 1);
            Grid.SetColumn(dateText, 2);

            grid.Children.Add(gradeCircle);
            grid.Children.Add(subjectText);
            grid.Children.Add(dateText);

            gradeCard.Child = grid;
            gradesPanel.Children.Add(gradeCard);
        }

        gradesScrollViewer.Content = gradesPanel;
        mainPanel.Children.Add(gradesScrollViewer);

        return mainPanel;
    }

    private IBrush GetGradeBrush(int grade)
    {
        return grade switch
        {
            5 => new SolidColorBrush(Color.FromRgb(76, 175, 80)),  // Green
            4 => new SolidColorBrush(Color.FromRgb(139, 195, 74)), // Light Green
            3 => new SolidColorBrush(Color.FromRgb(255, 235, 59)), // Yellow
            2 => new SolidColorBrush(Color.FromRgb(255, 152, 0)),  // Orange
            1 => new SolidColorBrush(Color.FromRgb(244, 67, 54)),   // Red
            _ => Brushes.Gray
        };
    }
}
