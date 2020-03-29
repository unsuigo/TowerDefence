using UnityEngine;

public class LaserTower : Tower 
{
    [SerializeField, Range(1f, 100f)]
    float damagePerSecond = 10f;

    [SerializeField] private Transform towerHead = default;
    [SerializeField] private Transform laserBeam = default;

    Target target;

    Vector3 laserBeamScale;

    void Awake () 
    {
        laserBeamScale = laserBeam.localScale;
    }

    public override TowerType TowerType => TowerType.Laser;

    public override void GameUpdate () 
    {
        if (TrackTarget(ref target) || GetTargetFromBuffer(out target)) 
        {
            Shoot();
        }
        else {
            laserBeam.localScale = Vector3.zero;
        }
    }

    void Shoot () 
    {
        Vector3 point = target.Position;
        towerHead.LookAt(point);
        laserBeam.localRotation = towerHead.localRotation;

        float d = Vector3.Distance(towerHead.position, point);
        laserBeamScale.z = d;
        laserBeam.localScale = laserBeamScale;
        laserBeam.localPosition =
            towerHead.localPosition + 0.5f * d * laserBeam.forward;

        target.Enemy.ApplyDamage(damagePerSecond * Time.deltaTime);
    }
}