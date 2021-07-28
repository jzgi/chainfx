namespace SkyChain.Db
{
    public interface IRecord
    {
        short PeerId { get; set; }

        string Account { get; set; }

        string Name { get; set; }

        decimal Amount { get; set; }
    }
}