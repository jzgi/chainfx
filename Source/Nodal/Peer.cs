namespace ChainFX.Nodal;

/// <summary>
/// Represents a peering node in peer-to-peer workwork.
/// </summary>
public class Peer : Entity
{
    public short peerid;

    public string secret;

    public string certpasswd;

    public short logging;

    public override void Read(ISource s, short msk = 255)
    {
        base.Read(s, msk);

        s.Get(nameof(peerid), ref peerid);
        s.Get(nameof(secret), ref secret);
        s.Get(nameof(certpasswd), ref certpasswd);
        s.Get(nameof(logging), ref logging);
    }

    public override void Write(ISink s, short msk = 255)
    {
        base.Write(s, msk);

        s.Put(nameof(peerid), peerid);
        s.Put(nameof(secret), secret);
        s.Put(nameof(certpasswd), certpasswd);
        s.Put(nameof(logging), logging);
    }

    public short PeerId => peerid;

    public string Secret => secret;

    public string CertPasswd => certpasswd;

    public short Logging => logging;
}