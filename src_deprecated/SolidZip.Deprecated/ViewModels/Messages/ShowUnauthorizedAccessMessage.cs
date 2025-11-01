namespace SolidZip.Deprecated.ViewModels.Messages;

public  sealed class ShowUnauthorizedAccessMessage(string path) : ValueChangedMessage<string>(path);