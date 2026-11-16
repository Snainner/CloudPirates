using System.Collections.Generic;
using UnityEngine;

public class VoxelCloud : MonoBehaviour
{
    public Mesh mesh;
    public Material material;          // GPU instancing enabled
    Transform player;
    Bounds bounds;
    public Vector3Int _size;// new Vector3Int(40, 16, 40)
    public float voxelSize = 1f;
    public float captureRadius = 3f;
    public int captured;
    Vector3 WorldCenter => transform.TransformPoint((Vector3)_size * voxelSize * 0.5f);

    bool[,,] cells;
    readonly List<Matrix4x4[]> batches = new List<Matrix4x4[]>();
    bool dirty = true;

    private void Awake()
    {
        player = GameManager.Instance.Player.transform;
    }
    void Update()
    {
        if (PlayerInBounds())
        {
            Capture();
        }

        if (dirty)
            Rebuild();

        if (batches.Count > 0)
            foreach (var b in batches)
                Graphics.DrawMeshInstanced(mesh, 0, material, b);
    }
    public void BuildCloud(Vector3Int size)
    {
        _size = size;
        bounds = CreateBounds();
        cells = new bool[size.x, size.y, size.z];
        Vector3 c = (Vector3)size / 2f;
        float seed = Random.value * 1000f;

        for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
                for (int z = 0; z < size.z; z++)
                {
                    Vector3 d = new Vector3(x - c.x, y - c.y, z - c.z);
                    d = new Vector3(d.x / c.x, d.y / c.y, d.z / c.z);
                    float falloff = 1f - d.magnitude;
                    float n = Mathf.PerlinNoise(x * .15f + seed, z * .15f + seed) * .5f
                            + Mathf.PerlinNoise(y * .2f + 50f + seed, x * .2f + seed) * .5f;
                    cells[x, y, z] = falloff + (n - .5f) * .8f > .2f;
                }
        Rebuild();
    }

    void Capture()
    {
        Vector3 local = transform.InverseTransformPoint(player.position) / voxelSize;
        int r = Mathf.CeilToInt(captureRadius / voxelSize);
        int cx = Mathf.FloorToInt(local.x), cy = Mathf.FloorToInt(local.y), cz = Mathf.FloorToInt(local.z);

        for (int x = Mathf.Max(0, cx - r); x <= Mathf.Min(_size.x - 1, cx + r); x++)
            for (int y = Mathf.Max(0, cy - r); y <= Mathf.Min(_size.y - 1, cy + r); y++)
                for (int z = Mathf.Max(0, cz - r); z <= Mathf.Min(_size.z - 1, cz + r); z++)
                {
                    if (!cells[x, y, z]) continue;
                    Vector3 center = new Vector3(x + .5f, y + .5f, z + .5f);
                    if ((center - local).magnitude * voxelSize > captureRadius) continue;
                    cells[x, y, z] = false;
                    captured++;
                    EventManager.Broadcast(Events.CaptureEvent);
                    dirty = true;
                }

    }

    void Rebuild()
    {
        //Debug.Log("Rebuilding Cloud");
        dirty = false;
        batches.Clear();
        var list = new List<Matrix4x4>(1023);
        for (int x = 0; x < _size.x; x++)
            for (int y = 0; y < _size.y; y++)
                for (int z = 0; z < _size.z; z++)
                {
                    if (!cells[x, y, z] || !IsSurface(x,y,z)) 
                        continue;

                    //Vector3 half = (Vector3)_size / 2f;
                    Vector3 pos = transform.TransformPoint(new Vector3(x + .5f, y + .5f, z + .5f) * voxelSize);
                    
                    list.Add(Matrix4x4.TRS(pos, transform.rotation, Vector3.one * voxelSize));
                    
                    if (list.Count == 1023)
                    {
                        batches.Add(list.ToArray()); 
                        list.Clear(); 
                    }
                }
        if (list.Count > 0) 
            batches.Add(list.ToArray());
    }

    bool PlayerInBounds()
    {
        return bounds.Contains(player.position);
    }
    Bounds CreateBounds()
    {
        Bounds b = new Bounds(WorldCenter, (Vector3)_size * voxelSize);
        //Bounds b = new Bounds(WorldCenter + (Vector3)_size * voxelSize * .5f, (Vector3 )_size * voxelSize);
        b.Expand(captureRadius * 2f);
        return b;
    }
    bool IsSurface(int x, int y, int z)
    {
        return !Solid(x + 1, y, z) || !Solid(x - 1, y, z) ||
               !Solid(x, y + 1, z) || !Solid(x, y - 1, z) ||
               !Solid(x, y, z + 1) || !Solid(x, y, z - 1);
    }

    bool Solid(int x, int y, int z)
    {
        if (x < 0 || y < 0 || z < 0 || x >= _size.x || y >= _size.y || z >= _size.z) return false;
        return cells[x, y, z];
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

            Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
}