using UnityEngine;

[CreateAssetMenu]
public class EnemyFactory : GameObjectFactory {
	
    [SerializeField] private  Enemy prefab = default;
    [SerializeField] private float scale = 0.2f;
    [SerializeField] private float speed = 0.2f;
    [SerializeField] private float pathOffset = 0;
    public Enemy Get () {
        Enemy instance = CreateGameObjectInstance(prefab);
        instance.Factory = this;
        instance.Initialize(scale, speed, pathOffset);
        return instance;
    }
   
    //For Pool after refactoring
    public void Reclaim (Enemy enemy) {
        Debug.Assert(enemy.Factory == this, "Wrong factory reclaimed!");
        Destroy(enemy.gameObject);
    }
}