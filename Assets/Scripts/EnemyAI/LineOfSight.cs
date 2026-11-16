using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class LineOfSight : MonoBehaviour
{
    GameObject _target;
    [SerializeField]
    float _detectionDelay = 0.5f;
    [SerializeField]
    float _viewAngle = 70f; //Target angle is max angle from middle to either left or right meaning the total viewing angle is times 2
    [SerializeField]
    string _coverMaskName = "Cloud";
    
    SphereCollider _detectionCollider;
    Collider _playerCollider;
    Bounds _playerBounds;
    Coroutine _detectPlayer;

    PlayerSightedEvent _sightTrue = new PlayerSightedEvent { PlayerSighted = true };
    PlayerSightedEvent _sightFalse = new PlayerSightedEvent { PlayerSighted = false };
    public bool PlayerInSight {  get; private set; }


    private void Awake()
    {
        _detectionCollider = GetComponent<SphereCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            _target = other.gameObject;
            _detectPlayer = StartCoroutine(DetectPlayer());
            _playerCollider = other;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            _target = null;
            StopCoroutine(DetectPlayer());
        }
    }
    IEnumerator DetectPlayer()
    {
        while(true)
        {
            yield return new WaitForSeconds(_detectionDelay);

            Vector3[] points = GetBoundingPoints(_playerCollider.bounds);
            int pointsHidden = 0;

            foreach (Vector3 point in points)
            {
                Vector3 targetDir = point - transform.position;
                float targetDist = Vector3.Distance(transform.position, point);
                float targetAngle = Vector3.Angle(targetDir, transform.forward);

                if(IsPointCovered(targetDir, targetDist) || targetAngle > _viewAngle)
                {
                    ++pointsHidden;
                }
            }

            if(pointsHidden >= points.Length)
            {
                PlayerInSight = true;
                EventManager.Broadcast(_sightTrue);
                Debug.Log("Player Sighted");
            }
            else
            {
                EventManager.Broadcast(_sightFalse);
            }
        }
    }
    bool IsPointCovered(Vector3 targetDir, float targetDist)
    {
        RaycastHit[] hits = Physics.RaycastAll(this.transform.position, targetDir, _detectionCollider.radius);
        foreach (RaycastHit hit in hits)
        {
            if(hit.transform.gameObject.layer == LayerMask.NameToLayer("Cloud"))
            {
                float coverDist = Vector3.Distance(transform.position, hit.point);
                if(coverDist< targetDist)
                    return true;
            }
        }
        return false;
    }
    Vector3[] GetBoundingPoints(Bounds bounds)
    {
        Vector3[] boundingPoints =
        {
            bounds.min,
            bounds.max,
            new Vector3(bounds.min.x, bounds.min.y, bounds.max.z),
            new Vector3(bounds.min.x, bounds.max.y, bounds.min.z),
            new Vector3(bounds.max.x, bounds.min.y, bounds.min.z),
            new Vector3(bounds.min.x, bounds.max.y, bounds.max.z),
            new Vector3(bounds.max.x, bounds.min.y, bounds.max.z),
            new Vector3(bounds.max.x, bounds.max.y, bounds.min.z)
        };

        return boundingPoints;
    }
}
