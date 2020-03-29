using UnityEngine;

[CreateAssetMenu]
public class TileContentFactory : GameObjectFactory {

    [SerializeField]
    TileContent enemyDestinationPrefab = default;

    [SerializeField]
    TileContent emptyPrefab = default;

    [SerializeField]
    TileContent wallPrefab = default;

    [SerializeField]
    TileContent spawnPointPrefab = default;

    [SerializeField]
    Tower[] towerPrefabs = default;

    public TileContent Get (TileContentType type) {
        switch (type) {
            case TileContentType.Empty: return Get(emptyPrefab);
            case TileContentType.Destination: return Get(enemyDestinationPrefab);
            case TileContentType.Wall: return Get(wallPrefab);
            case TileContentType.SpawnPoint: return Get(spawnPointPrefab);
        }
        return null;
    }

    public Tower Get (TowerType type) {
        Tower prefab = towerPrefabs[(int)type];
        return Get(prefab);
    }

    public void Reclaim (TileContent content) {
        Destroy(content.gameObject);
    }

    T Get<T> (T prefab) where T : TileContent {
        T instance = CreateGameObjectInstance(prefab);
        instance.Factory = this;
        return instance;
    }
}