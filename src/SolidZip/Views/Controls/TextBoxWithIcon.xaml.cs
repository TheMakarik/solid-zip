namespace SolidZip.Views.Controls;

public partial class TextBoxWithIcon : UserControl
{
    public TextBoxWithIcon()
    {
        InitializeComponent();
        MainTextBorder.MouseDown += MainTextBorder_MouseDown;
    }

    private void MainTextBorder_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        InputTextBox.Focus();
    }
    
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(TextBoxWithIcon),
            new PropertyMetadata(string.Empty, OnTextChanged));

    private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (TextBoxWithIcon)d;
        control.InputTextBox.Text = e.NewValue?.ToString()!;
    }

    public ImageSource Icon
    {
        get => (ImageSource)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register(nameof(Icon), typeof(ImageSource), typeof(TextBoxWithIcon),
            new PropertyMetadata(null, OnIconChanged));

    private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (TextBoxWithIcon)d;
        control.IconImage.Source = e.NewValue as ImageSource;
    }
}