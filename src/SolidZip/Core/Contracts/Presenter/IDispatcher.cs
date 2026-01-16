namespace SolidZip.Core.Contracts.Presenter;

public interface IDispatcher
{
    public void Configure(Action syncCall, Task asyncCall);
    public Task InvokeAsync(Action action);
    public void Invoke(Action action);
}