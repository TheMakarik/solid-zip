namespace SolidZip.Core.Contracts.Presenter;

public interface IMessageBox
{
    MessageBoxResultEnum Show(string messageBoxText, string caption, MessageBoxButtonEnum button, MessageBoxImageEnum icon);
}
