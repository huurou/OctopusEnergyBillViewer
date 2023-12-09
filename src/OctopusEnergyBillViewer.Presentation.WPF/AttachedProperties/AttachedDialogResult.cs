using System.Windows;

namespace OctopusEnergyBillViewer.Presentation.WPF.AttachedProperties;

public static class AttachedDialogResult
{
    public static bool? GetDialogResult(DependencyObject obj)
    {
        return (bool?)obj.GetValue(DialogResultProperty);
    }

    public static void SetDialogResult(Window target, bool? value)
    {
        target.SetValue(DialogResultProperty, value);
    }

    public static readonly DependencyProperty DialogResultProperty = DependencyProperty.RegisterAttached(
        "DialogResult", typeof(bool?), typeof(AttachedDialogResult), new PropertyMetadata(default(bool?),
            (d, e) =>
            {
                if (d is Window window)
                {
                    window.DialogResult = e.NewValue as bool?;
                }
            }));
}