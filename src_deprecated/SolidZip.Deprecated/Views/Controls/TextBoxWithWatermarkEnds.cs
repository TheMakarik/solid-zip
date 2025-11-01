namespace SolidZip.Deprecated.Views.Controls;

public class TextBoxWithWatermarkEnds : Control
{
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(TextBoxWithWatermarkEnds),
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTextChanged));

    public static readonly DependencyProperty WatermarkProperty =
        DependencyProperty.Register(nameof(Watermark), typeof(string), typeof(TextBoxWithWatermarkEnds), 
        new PropertyMetadata(string.Empty, OnWatermarkChanged));

    public static readonly DependencyProperty WatermarkForegroundProperty =
        DependencyProperty.Register(nameof(WatermarkForeground), typeof(Brush), typeof(TextBoxWithWatermarkEnds), 
        new PropertyMetadata(Brushes.Gray));

    public static readonly DependencyProperty UpdateWatermarkCommandProperty =
        DependencyProperty.Register(nameof(UpdateWatermarkCommand), typeof(ICommand), typeof(TextBoxWithWatermarkEnds));

    public static readonly DependencyProperty UpdateWatermarkKeyProperty =
        DependencyProperty.Register(nameof(UpdateWatermarkKey), typeof(Key), typeof(TextBoxWithWatermarkEnds),
            new PropertyMetadata(Key.Tab));

    public static readonly DependencyProperty SetWatermarkCommandProperty =
        DependencyProperty.Register(nameof(SetWatermarkCommand), typeof(ICommand), typeof(TextBoxWithWatermarkEnds));

    public static readonly DependencyProperty SetWatermarkKeyProperty =
        DependencyProperty.Register(nameof(SetWatermarkKey), typeof(Key), typeof(TextBoxWithWatermarkEnds),
            new PropertyMetadata(Key.Enter));

    public static readonly DependencyProperty TextBoxNameProperty =
        DependencyProperty.Register(nameof(TextBoxName), typeof(string), typeof(TextBoxWithWatermarkEnds),
            new PropertyMetadata("InputTextBox"));

    public static readonly DependencyProperty WatermarkTextBlockNameProperty =
        DependencyProperty.Register(nameof(WatermarkTextBlockName), typeof(string), typeof(TextBoxWithWatermarkEnds),
            new PropertyMetadata("WatermarkTextBlock"));

    public static readonly DependencyProperty IconBorderNameProperty =
        DependencyProperty.Register(nameof(IconBorderName), typeof(string), typeof(TextBoxWithWatermarkEnds),
            new PropertyMetadata("IconBorder"));

    public static readonly DependencyProperty TextBoxBorderNameProperty =
        DependencyProperty.Register(nameof(TextBoxBorderName), typeof(string), typeof(TextBoxWithWatermarkEnds),
            new PropertyMetadata("TextBoxBorder"));

    private TextBox? _innerTextBox;
    private TextBlock? _watermarkTextBlock;
    private Border? _iconBorder;
    private Border? _textBoxBorder;

    static TextBoxWithWatermarkEnds() =>
        DefaultStyleKeyProperty.OverrideMetadata(typeof(TextBoxWithWatermarkEnds), 
            new FrameworkPropertyMetadata(typeof(TextBoxWithWatermarkEnds)));

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public string Watermark
    {
        get => (string)GetValue(WatermarkProperty);
        set => SetValue(WatermarkProperty, value);
    }

    public Brush? WatermarkForeground
    {
        get => (Brush?)GetValue(WatermarkForegroundProperty);
        set => SetValue(WatermarkForegroundProperty, value);
    }

    public ICommand? UpdateWatermarkCommand
    {
        get => (ICommand?)GetValue(UpdateWatermarkCommandProperty);
        set => SetValue(UpdateWatermarkCommandProperty, value);
    }

    public Key UpdateWatermarkKey
    {
        get => (Key)GetValue(UpdateWatermarkKeyProperty);
        set => SetValue(UpdateWatermarkKeyProperty, value);
    }

    public ICommand? SetWatermarkCommand
    {
        get => (ICommand?)GetValue(SetWatermarkCommandProperty);
        set => SetValue(SetWatermarkCommandProperty, value);
    }

    public Key SetWatermarkKey
    {
        get => (Key)GetValue(SetWatermarkKeyProperty);
        set => SetValue(SetWatermarkKeyProperty, value);
    }

    public string TextBoxName
    {
        get => (string)GetValue(TextBoxNameProperty);
        set => SetValue(TextBoxNameProperty, value);
    }

    public string WatermarkTextBlockName
    {
        get => (string)GetValue(WatermarkTextBlockNameProperty);
        set => SetValue(WatermarkTextBlockNameProperty, value);
    }

    public string IconBorderName
    {
        get => (string)GetValue(IconBorderNameProperty);
        set => SetValue(IconBorderNameProperty, value);
    }

    public string TextBoxBorderName
    {
        get => (string)GetValue(TextBoxBorderNameProperty);
        set => SetValue(TextBoxBorderNameProperty, value);
    }

    public override void OnApplyTemplate()
    {
        if (_innerTextBox is not null)
        {
            _innerTextBox.TextChanged -= OnInnerTextChanged;
            _innerTextBox.KeyDown -= OnInnerKeyDown;
        }

        base.OnApplyTemplate();

        _innerTextBox = GetTemplateChild(TextBoxName) as TextBox;
        _watermarkTextBlock = GetTemplateChild(WatermarkTextBlockName) as TextBlock;
        _iconBorder = GetTemplateChild(IconBorderName) as Border;
        _textBoxBorder = GetTemplateChild(TextBoxBorderName) as Border;

        if (_innerTextBox is not null)
        {
            _innerTextBox.TextChanged += OnInnerTextChanged;
            _innerTextBox.KeyDown += OnInnerKeyDown;
            _innerTextBox.Text = Text;
        }

        UpdateWatermarkVisuals();
    }

    private void OnInnerTextChanged(object sender, TextChangedEventArgs e)
    {
        if (_innerTextBox is not null)
            Text = _innerTextBox.Text;

        UpdateWatermarkVisuals();
    }

    private void OnInnerKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == UpdateWatermarkKey && UpdateWatermarkCommand is not null && UpdateWatermarkCommand.CanExecute(null))
        {
            UpdateWatermarkCommand.Execute(null);
            e.Handled = true;
        }
        else if (e.Key == SetWatermarkKey && SetWatermarkCommand is not null && SetWatermarkCommand.CanExecute(null))
        {
            SetWatermarkCommand.Execute(null);
            e.Handled = true;
        }
    }

    private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (TextBoxWithWatermarkEnds)d;
        if (control._innerTextBox is not null && control._innerTextBox.Text != (string)e.NewValue)
            control._innerTextBox.Text = (string)e.NewValue;
        
        control.UpdateWatermarkVisuals();
    }

    private static void OnWatermarkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (TextBoxWithWatermarkEnds)d;
        control.UpdateWatermarkVisuals();
    }

    private void UpdateWatermarkVisuals()
    {
        if (_watermarkTextBlock is null || _innerTextBox is null) return;

        var currentText = Text ?? string.Empty;
        var watermark = Watermark ?? string.Empty;

        _watermarkTextBlock.Text = watermark;
        _watermarkTextBlock.Foreground = WatermarkForeground;
        
        if (string.IsNullOrEmpty(currentText))
        {
            _watermarkTextBlock.Visibility = Visibility.Visible;
            _watermarkTextBlock.Margin = new Thickness(0, 0, 0, 0);
        }
        else if (watermark.StartsWith(currentText))
        {
            _watermarkTextBlock.Visibility = Visibility.Visible;
            
            var formattedText = new FormattedText(
                currentText,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(_innerTextBox.FontFamily, _innerTextBox.FontStyle, _innerTextBox.FontWeight, _innerTextBox.FontStretch),
                _innerTextBox.FontSize,
                Brushes.Black,
                VisualTreeHelper.GetDpi(this).PixelsPerDip);

            double textWidth = formattedText.Width;
            _watermarkTextBlock.Margin = new Thickness(textWidth, 0, 0, 0);
            _watermarkTextBlock.Text = watermark.Substring(currentText.Length);
        }
        else
            _watermarkTextBlock.Visibility = Visibility.Collapsed;
    }
}