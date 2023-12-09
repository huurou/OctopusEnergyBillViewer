using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;

namespace OctopusEnergyBillViewer.Presentation.WPF.Behaviors;

public class PasswordBindingBehavior : Behavior<PasswordBox>
{
    public static readonly DependencyProperty PasswordProperty = DependencyProperty.Register(
        nameof(Password), typeof(string), typeof(PasswordBindingBehavior), new FrameworkPropertyMetadata(
            "", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, SourcePasswordChanged));

    /// <summary>
    ///     パスワードを取得、または設定します。
    /// </summary>
    public string Password
    {
        get { return (string)GetValue(PasswordProperty); }
        set { SetValue(PasswordProperty, value); }
    }

    private static void SourcePasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is PasswordBindingBehavior self &&
                    e.NewValue is string value &&
                    self.AssociatedObject is not null &&
                    self.AssociatedObject.Password != value)
        {
            self.AssociatedObject.Password = value ?? "";
        }
    }

    private void ControlPasswordChanged(object sender, RoutedEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(sender);
        if (sender is PasswordBox passwordBox &&
            Password != passwordBox.Password)
        {
            Password = passwordBox.Password;
        }
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        SourcePasswordChanged(this, new DependencyPropertyChangedEventArgs(PasswordProperty, null, Password));
        if (AssociatedObject != null)
        {
            AssociatedObject.PasswordChanged += ControlPasswordChanged;
        }
    }
}