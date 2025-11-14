namespace SolidZip.Core.Common;

public sealed class RetrySystem
{
    public async Task RetryWithDelayAsync<TException>(Task task, TimeSpan delay, int maxRetry, int currentRetry = 0)
        where TException : Exception
    {
        try
        {
            task.Start();
            await task;
        }
        catch (TException e)
        {
            if (maxRetry <= currentRetry)
                throw;

            currentRetry++;
            await Task.Delay(delay)
                .ContinueWith(res => RetryWithDelayAsync<TException>(task, delay, maxRetry, currentRetry));
        }
    }
    
    public async Task<TResult> RetryWithDelayAsync<TException, TResult>(Task<TResult>  task, TimeSpan delay, int maxRetry, int currentRetry = 0)
        where TException : Exception
    {
        try
        {
            task.Start();
            await task;
        }
        catch (TException e)
        {
            if (maxRetry <= currentRetry)
                throw;

            currentRetry++;
            await Task.Delay(delay)
                .ContinueWith(_ => RetryWithDelayAsync<TException>(task, delay, maxRetry, currentRetry));
        }

        return task.Result;
    }
}