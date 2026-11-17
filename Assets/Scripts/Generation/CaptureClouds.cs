using System.Collections.Generic;
using UnityEngine;

public class CaptureClouds : Upgradable
{
    [NaughtyAttributes.ReadOnly]
    public int Captured;

    Transform _player;
    CloudManager _cManager;

    Dictionary<Vector3Int, CloudChunk> _chunks;
    float _voxelSize;
    [SerializeField]
    Stat _captureRadius;

    static int _shift = VoxelConstants.Shift;
    static int _mask = VoxelConstants.Mask;
 
    private void Start()
    {
        _cManager = CloudManager.Instance;
        _chunks = _cManager.Chunks;
        _voxelSize = _cManager.VoxelSize;

        //_player = GameManager.Instance.Player.Plane.transform;
    }
    //private void Update()
    //{
    //    Capture();
    //}

    public void Capture(Vector3 playerPosition)
    {
        Vector3 p = _cManager.transform.InverseTransformPoint(playerPosition) / _voxelSize;
        float r = _captureRadius.Value / _voxelSize;
        float rSqr = r * r;

        Vector3Int min = Vector3Int.FloorToInt(p - Vector3.one * r);
        Vector3Int max = Vector3Int.FloorToInt(p + Vector3.one * r);

        int grabbed = 0;

        // Only visit the (at most 8) chunks touching the capture box.
        // Missing chunk = empty space = skipped instantly.
        for (int cx = min.x >> _shift; cx <= max.x >> _shift; cx++)
            for (int cy = min.y >> _shift; cy <= max.y >> _shift; cy++)
                for (int cz = min.z >> _shift; cz <= max.z >> _shift; cz++)
                {
                    if (!_chunks.TryGetValue(new Vector3Int(cx, cy, cz), out var chunk)) 
                        continue;

                    grabbed += _cManager.CaptureInChunk(chunk, p, rSqr, min, max);
                }

        if (grabbed > 0)
        {
            Captured += grabbed;
            EventManager.Broadcast(Events.CaptureEvent);     // once per frame
        }
    }
    public void ApplySettings(Stat CaptureRadius)
    {
        _captureRadius = CaptureRadius;
    }
    protected override UpgradeData[] BuildUpgrades()
    {
        var captureRadius = UpgradeFactory.Create<CaptureClouds>("cloud_capture_radius", "Cloud Capture Radius", c => c._captureRadius.Bonus);
        return new UpgradeData[] { captureRadius }; 
    }
}
