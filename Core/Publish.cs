namespace Greatbone.Core
{
    public class Publish : IMember
    {
        string topic;

        int priority;

        int duration;

        public string Key => topic;
    }
}