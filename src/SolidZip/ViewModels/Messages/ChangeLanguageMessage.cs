
namespace SolidZip.ViewModels.Messages;

public class ChangeLanguageMessage(CultureInfo value) : ValueChangedMessage<CultureInfo>(value);