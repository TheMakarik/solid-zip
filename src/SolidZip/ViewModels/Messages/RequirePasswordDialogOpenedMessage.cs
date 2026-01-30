namespace SolidZip.ViewModels.Messages;

public sealed class RequirePasswordDialogOpenedMessage(RequestPasswordMessage requestMessage)
{
    public RequestPasswordMessage RequestMessage => requestMessage;
}
