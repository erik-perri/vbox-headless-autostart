namespace CommonLib.Processes
{
    public interface IProcessOutputFactory
    {
        IProcessOutput CreateProcess(string fileName, string arguments);
    }
}