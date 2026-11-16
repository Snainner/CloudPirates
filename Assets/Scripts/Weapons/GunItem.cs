using NaughtyAttributes;
using System.Collections;
using UnityEngine;

public class GunItem : Upgradable
{
    [Header("Gun Parameters")]
    //public Vector3 GunOffset;
    [SerializeField]
    Transform _muzzleLoc;
    public int RayDist;
    public LayerMask DamagableMask;

    DamageData _damageData;

    //public int Level;
    //public float _levelModifier = 1;
    public GunData _gunData;
    //public float _fireRateMod;

    public Stat Damage;
    public Stat FireRate;
    //float _fireRate => _gunData.FireRate * _fireRateMod;

    public event System.Action OnFired;
    public event System.Action<RaycastHit> OnHit;
    public event System.Action OnMiss;

    float _lastFired;

    [Header("Bullet")]
    [SerializeField]
    bool _addBulletSpread = true;
    [SerializeField]
    Vector3 _bulletSpreadVariance = new Vector3(0.1f, .1f, .1f);
    [SerializeField]
    ParticleSystem _impactParticle;
    [SerializeField]
    TrailRenderer _bulletTrail;
    [SerializeField]
    float _tracerSpeed = 200f;
    [SerializeField]
    float _maxTracerDist = 400f;
    [SerializeField]
    bool _hasAimAssist = false;
    [SerializeField]
    [Range(0.05f, 0.2f)]
    float _aimAssistRadius = .1f;
    private void Awake()
    {
        //Set base values for the stats
        Damage = new Stat(_gunData.Damage);
        FireRate = new Stat(_gunData.FireRate);
        RefreshDamageData();
    }
    [Button]
    void Shoot()
    {
        Vector3 rayOrigin = _muzzleLoc.position;
        Vector3 rayDir = transform.forward;

        bool rayHit = Physics.Raycast(rayOrigin, rayDir, out RaycastHit hit, RayDist, DamagableMask);
        Vector3 endPoint = rayOrigin + (rayDir * _maxTracerDist); //Tracer endpoint if it didn't hit anything
        //Debug.DrawLine(rayOrigin, endPoint, rayHit ? Color.green : Color.red, .4f);

        OnFired?.Invoke();
        TrailRenderer trail = Instantiate(_bulletTrail, rayOrigin, Quaternion.identity);
        trail.Clear();

        if (rayHit)
        {
            HitDamage(hit, trail);
        }
        else
        {
            if(_hasAimAssist)
            {
                rayHit = Physics.SphereCast(rayOrigin, _aimAssistRadius, rayDir, out hit, RayDist, DamagableMask);
                if (rayHit)
                {
                    HitDamage(hit, trail);
                }
            }

            StartCoroutine(SpawnTrail(trail, endPoint, Vector3.zero, false));
        }
    }
    void HitDamage(RaycastHit hit, TrailRenderer trail)
    {
        var player = hit.collider.gameObject;
        OnHit?.Invoke(hit);
        StartCoroutine(SpawnTrail(trail, hit.point, hit.normal, true));
        //StartCoroutine(SpawnTrail(_bulletTrail, hit));

        if (hit.collider.TryGetComponent(out IDamagable damagable))
        {
            damagable.TakeDamage(_damageData);
        }
    }
    public void AutomaticFire(bool isFiring)
    {
        if(isFiring)
        {
            if(Time.time - _lastFired > 1 / FireRate.Value)
            {
                _lastFired = Time.time;
                Shoot();
                
            }
        }
    }

    IEnumerator SpawnTrail(TrailRenderer trail, Vector3 hitPoint, Vector3 hitNormal, bool madeImpact)
    {
        Vector3 startPos = trail.transform.position;
        Vector3 dir = (hitPoint - trail.transform.position).normalized;
        //Debug.Log($"muzzle world: {_muzzleLoc.position}, gun: {transform.position}", _muzzleLoc);

        float dist = Vector3.Distance(startPos, hitPoint);
        float travelled = 0;

        while(travelled < dist)
        {
            if(trail == null)
                yield break; 


            travelled += Time.deltaTime * _tracerSpeed;
            trail.transform.position = Vector3.Lerp(startPos, hitPoint, travelled / dist);

            yield return null;
        }
        trail.transform.position = hitPoint;

        if(madeImpact)
        {
            //instantiate impact system with the rotation as Quaternion.LookRotation(hitNormal)
        }

        Destroy(trail.gameObject, trail.time);
    }

    void RefreshDamageData()
    {
        _damageData = new DamageData(Mathf.RoundToInt(Damage.Value), this);
    }
    protected override UpgradeData[] BuildUpgrades()
    {
        var damage = UpgradeFactory.Create<GunItem>("damage", "Damage", c => c.Damage.Bonus);
        var speed = UpgradeFactory.Create<GunItem>("fire_rate", "Fire Rate", c => FireRate.Bonus);

        return new[] { damage, speed };
    }

}
