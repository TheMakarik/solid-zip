namespace SolidZip.ViewModels.Messages;

public sealed class UpdateStartupProgressMessage(double value) : ValueChangedMessage<double>(value);