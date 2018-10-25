namespace Greatbone
{
    public interface IEventContext
    {
        bool FirstTime { get; }

        string RemoteSvc { get; }

        void SetParam(string name, string v);

        string GetAsync();
    }
}