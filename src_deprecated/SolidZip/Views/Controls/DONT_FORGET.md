## If you use TextBoxWithWatermark you need to configure template before using
## By default you are able to use SzTextBoxWithWatermarkEnds style from Styles/
## This is example of template's implementation
```xml
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                    xmlns:controls="clr-namespace:SolidZip.Views.Controls">
    <Style TargetType="{x:Type controls:TextBoxWithWatermarkEnds}">
        <Setter Property="TextBoxName" Value="InputTextBox" />
        <Setter Property="WatermarkTextBlockName" Value="WatermarkTextBlock" />
        <Setter Property="IconBorderName" Value="IconBorder" />
        <Setter Property="TextBoxBorderName" Value="TextBoxBorder" />
        <Setter Property="Template">
            <Setter.Value>
                <ControlTemplate TargetType="{x:Type controls:TextBoxWithWatermarkEnds}">
                    <Grid>
                        <Grid.ColumnDefinitions>
                            <ColumnDefinition Width="Auto" />
                            <ColumnDefinition Width="*" />
                        </Grid.ColumnDefinitions>

                        <Border
                            BorderThickness="3,3,0,3"
                            Grid.Column="0"
                            Width="{Binding Height, RelativeSource={RelativeSource Self}}"
                            x:Name="IconBorder">
                            <ContentPresenter x:Name="IconContent" />
                        </Border>

                        <Border
                            BorderThickness="0,3,3,3"
                            Grid.Column="1"
                            x:Name="TextBoxBorder">
                            <Grid>
                                <TextBox
                                    Background="Transparent"
                                    BorderThickness="0"
                                    FontSize="15"
                                    Foreground="{TemplateBinding Foreground}"
                                    HorizontalAlignment="Stretch"
                                    TextAlignment="Left"
                                    VerticalAlignment="Center"
                                    x:Name="InputTextBox" />
                                <TextBlock
                                    Background="Transparent"
                                    FontSize="15"
                                    Foreground="{TemplateBinding WatermarkForeground}"
                                    HorizontalAlignment="Stretch"
                                    IsHitTestVisible="False"
                                    Text="{TemplateBinding Watermark}"
                                    VerticalAlignment="Center"
                                    Visibility="Collapsed"
                                    x:Name="WatermarkTextBlock" />
                            </Grid>
                        </Border>
                    </Grid>
                </ControlTemplate>
            </Setter.Value>
        </Setter>
    </Style>
</ResourceDictionary>
```