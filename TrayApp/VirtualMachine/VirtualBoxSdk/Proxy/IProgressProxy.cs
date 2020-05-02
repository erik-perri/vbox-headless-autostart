namespace TrayApp.VirtualMachine.VirtualBoxSdk.Proxy
{
    public interface IProgressProxy
    {
        void WaitForCompletion(int timeout);

        int ResultCode { get; }

        public bool CheckForSuccess();
    }
}