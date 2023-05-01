using System.Collections.Concurrent;

namespace ChainFx.Nodal;

public class TwinCollection
{
    // index for all entities
    private ConcurrentDictionary<int, ITwin> all;
    
    // index for top
    private ConcurrentDictionary<int, ITwin> top;




    public void Load()
    {
        
    }
    
    public void Save()
    {
        
    }

}