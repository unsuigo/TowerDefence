using UnityEngine;

public class Target : MonoBehaviour 
{
    const int enemyLayerMask = 1 << 9;

    static Collider[] buffer = new Collider[100];

    public static int BufferedCount { get; private set; }

    public static Target RandomBuffered =>
        GetBuffered(Random.Range(0, BufferedCount));

    public static bool FillBuffer (Vector3 position, float range) 
    {
        Vector3 top = position;
        top.y += 3f;
        BufferedCount = Physics.OverlapCapsuleNonAlloc(
            position, top, range, buffer, enemyLayerMask
        );
        return BufferedCount > 0;
    }

    public static Target GetBuffered (int index) 
    {
        var target = buffer[index].GetComponent<Target>();
        return target;
    }

    public Enemy Enemy { get; private set; }

    public Vector3 Position => transform.position;

    void Awake () 
    {
        Enemy = transform.root.GetComponent<Enemy>();
    }
}