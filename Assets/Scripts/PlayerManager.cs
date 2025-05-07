using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

[Obsolete]
public class PlayerManager : BaseManager
{
    private readonly Dictionary<int, Player> _mPlayerDict;
    
    public ReadOnlyDictionary<int, Player> PlayerDict => new(_mPlayerDict);
    
    public event Action<Player> OnAddPlayer; 

    public PlayerManager()
    {
        _mPlayerDict = new Dictionary<int, Player>();
    }

    public void AddPlayer(int playerId, Player player)
    {
        _mPlayerDict[playerId] = player;
    }

    public void RemovePlayer(int playerId)
    {
        _mPlayerDict.Remove(playerId);
    }
}