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
    public string Description => "Megszerzett érdemjegyek megtekintése és átlagszámítás.";

    public MyGradesView() { }

    public MyGradesView(IStudentContext context)
    {
        _context = context;
    }

    public Control CreateView()
    {
        var panel = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Spacing = 15,
            Margin = new Thickness(20)
        };

        var titleBlock = new TextBlock
        {
            Text = "Érdemjegyeim",
            FontSize = 28,
            FontWeight = FontWeight.Bold,
            Margin = new Thickness(0, 0, 0, 10)
        };
        panel.Children.Add(titleBlock);

        if (_context == null)
        {
            panel.Children.Add(new TextBlock { Text = "Hiba: A nézet kontextusa nem elérhető." });
            return panel;
        }

        var grades = _context.GetMyGrades();

        if (grades == null || !grades.Any())
        {
            panel.Children.Add(new TextBlock { Text = "Nincsenek rögzített jegyeid.", HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(0, 50) });
            return panel;
        }
        
        var gradesListPanel = new StackPanel
        {
            Spacing = 10
        };

        foreach (var grade in grades.OrderByDescending(g => g.Date))
        {
            var gradeCard = new Border
            {
                Background = Brushes.White,
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(15),
                BoxShadow = new BoxShadows(new BoxShadow { Blur = 8, OffsetX = 0, OffsetY = 2, Color = Color.Parse("#20000000") })
            };

            var cardGrid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitions("*,Auto")
            };

            var subjectDetailsPanel = new StackPanel { Spacing = 2, VerticalAlignment = VerticalAlignment.Center };
            subjectDetailsPanel.Children.Add(new TextBlock { Text = grade.Subject?.Name ?? "Ismeretlen tantárgy", FontWeight = FontWeight.SemiBold, FontSize = 16 });
            subjectDetailsPanel.Children.Add(new TextBlock { Text = grade.Date.ToString("yyyy. MMMM dd."), FontSize = 12, Foreground = Brushes.Gray });
            Grid.SetColumn(subjectDetailsPanel, 0);

            var gradeCircle = new Border
            {
                Width = 48,
                Height = 48,
                CornerRadius = new CornerRadius(24),
                Background = GetGradeBrush(grade.Value),
                Child = new TextBlock
                {
                    Text = grade.Value.ToString(),
                    Foreground = Brushes.White,
                    FontSize = 22,
                    FontWeight = FontWeight.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                }
            };
            Grid.SetColumn(gradeCircle, 1);

            cardGrid.Children.Add(subjectDetailsPanel);
            cardGrid.Children.Add(gradeCircle);
            gradeCard.Child = cardGrid;

            gradesListPanel.Children.Add(gradeCard);
        }
        
        panel.Children.Add(gradesListPanel);

        double average = grades.Average(g => g.Value);
        var averageCard = new Border
        {
            Background = new SolidColorBrush(Color.Parse("#F5F5F5")),
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(15),
            Margin = new Thickness(0, 15, 0, 0)
        };
        
        var averageGrid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*,Auto"),
            VerticalAlignment = VerticalAlignment.Center
        };
        
        averageGrid.Children.Add(new TextBlock
        {
            Text = "Tanulmányi átlag",
            FontWeight = FontWeight.Bold,
            FontSize = 18,
            VerticalAlignment = VerticalAlignment.Center
        });

        var averageValueText = new TextBlock
        {
            Text = average.ToString("F2", CultureInfo.InvariantCulture),
            FontWeight = FontWeight.Bold,
            FontSize = 26,
            Foreground = GetAverageBrush(average),
            VerticalAlignment = VerticalAlignment.Center
        };
        Grid.SetColumn(averageValueText, 1);
        averageGrid.Children.Add(averageValueText);
        
        averageCard.Child = averageGrid;
        panel.Children.Add(averageCard);

        return new ScrollViewer { Content = panel };
    }

    private IBrush GetGradeBrush(int value)
    {
        switch (value)
        {
            case 5: return new SolidColorBrush(Color.Parse("#4CAF50"));
            case 4: return new SolidColorBrush(Color.Parse("#8BC34A"));
            case 3: return new SolidColorBrush(Color.Parse("#FFC107"));
            case 2: return new SolidColorBrush(Color.Parse("#FF9800"));
            case 1: return new SolidColorBrush(Color.Parse("#F44336"));
            default: return Brushes.Gray;
        }
    }
    
    private IBrush GetAverageBrush(double value)
    {
        if (value >= 4.5) return new SolidColorBrush(Color.Parse("#4CAF50"));
        if (value >= 3.5) return new SolidColorBrush(Color.Parse("#8BC34A"));
        if (value >= 2.5) return new SolidColorBrush(Color.Parse("#FFC107"));
        if (value >= 1.5) return new SolidColorBrush(Color.Parse("#FF9800"));
        return new SolidColorBrush(Color.Parse("#F44336"));
    }
}