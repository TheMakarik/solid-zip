namespace SolidZip.ViewModels.Messages;

public sealed class UpdateProgressMessage(double value) : ValueChangedMessage<double>(value);