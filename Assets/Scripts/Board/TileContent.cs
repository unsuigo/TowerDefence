using UnityEngine;

public class TileContent : MonoBehaviour
{
    [SerializeField] TileContentType tileType = default;
    TileContentFactory _factory;
    public TileContentType TileType => tileType;
    public bool BlocksPath =>  TileType == TileContentType.Wall || TileType == TileContentType.Tower;
    
    public TileContentFactory Factory 
    {
        get => _factory;
        set {
            _factory = value;
        }
    }
    
    public virtual void GameUpdate () {}
    public void Recycle () 
    {
        _factory.Reclaim(this);
    }
}
