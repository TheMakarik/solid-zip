namespace SolidZip.Services;

public static class ExceptionHelper
{
    public static void ThrowIf<T>(bool condition, Func<T> exceptionCreator)  where T : Exception
    {
        if (condition)
            throw exceptionCreator();
    }
}