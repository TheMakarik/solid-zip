namespace SolidZip.Views.Controls;

public sealed class ClippingBorder : Border
{
    private readonly RectangleGeometry _clipRect = new();
    private object? _oldClip;

    protected override void OnRender(DrawingContext dc)
    {
        OnApplyChildClip();
        base.OnRender(dc);
    }

    public override UIElement Child
    {
        get => base.Child;

        set
        {
            if (Child == value) 
                return;
            
            Child?.SetValue(ClipProperty, _oldClip);
            _oldClip = ReadLocalValue(ClipProperty);
            base.Child = value;
        }
    }

    protected void OnApplyChildClip()
    {
        UIElement child = Child;
        _clipRect.RadiusX = _clipRect.RadiusY = Math.Max(0.0, CornerRadius.TopLeft - BorderThickness.Left * 0.5);
        _clipRect.Rect = new Rect(Child.RenderSize);
        child.Clip = _clipRect;
    }
}