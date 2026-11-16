using Unity.Mathematics;
using UnityEngine;

public class VoxelCloud2 : MonoBehaviour
{
    public Mesh Mesh;
    public Material Material;
    public float VoxelSize;
    public float CaptureRadius;
    public int Captured;

    const int _batchSize = 1023;
    static readonly Vector3Int[] Dirs =
    {
        new(1, 0, 0), new(-1, 0, 0), new(0,1,0), new(0,-1,0),new(0,0,1),new(0,0,-1)
    };

    Transform _player;
    Matrix4x4 _localToWorld;

    Vector3Int _size;
    bool[] _cells; //Flat voxel grid
    int[] _slotOf;
    int[] _cellAtSlot;
    //batches are divided up into arrays as DrawMeshInstanced has a max of 1023 matrices per call
    //The first array acts as an index for the second array 
    Matrix4x4[][] _batches;
    int _instanceCount;

    //Create a flat index
    int CellIndex(int x, int y, int z) => x + _size.x * (y + _size.y * z);
    private void Awake()
    {
        _player = GameManager.Instance.Player.transform;
    }
    private void Update()
    {
        if (_cells == null)
            return;
        if (PlayerInBounds())
            Capture();

        for (int b = 0; b < _batches.Length; b++)
        {
            int count = Mathf.Min(_batchSize, _instanceCount - b * _batchSize);
            if (count <= 0)
                break;

            Graphics.DrawMeshInstanced(Mesh, 0, Material, _batches[b], count);
        }

    }
    public void BuildCloud(Vector3Int size)
    {
        _size = size;
        _localToWorld = transform.localToWorldMatrix;
        int total = size.x * size.y * size.z;

        //Initialize arrays
        _cells = new bool[total];
        _slotOf = new int[total];
        _cellAtSlot = new int[total];
        System.Array.Fill(_slotOf, -1);

        _batches = new Matrix4x4[Mathf.CeilToInt(total/ (float)_batchSize)][];
        for (int i = 0; i < _batches.Length; i++)
        {
            _batches[i] = new Matrix4x4[_batchSize];
        }

        _instanceCount = 0;
        Vector3 c = (Vector3)_size / 2f;
        float seed = UnityEngine.Random.value * 1000f;

        for (int x = 0; x < _size.x; x++)
            for(int y = 0; y < _size.y; y++)
                for(int z = 0; z < _size.z; z++)
                {
                    var d = new Vector3((x - c.x) / c.x, (y-c.y)/c.y,(z - c.z)/c.z);
                    float falloff = 1f - d.magnitude;
                    float n = Mathf.PerlinNoise(x * .15f + seed, z * .15f + seed) * .5f + Mathf.PerlinNoise(y * .2f + 50f + seed, x * .2f +seed) * .5f;
                    _cells[CellIndex(x, y, z)] = falloff + (n - .5f) * .8f > .2f;
                }

        //One time full surface pass
        for(int x = 0; x < _size.x;x++)
            for(int y = 0; y < _size.y;y++)
                for(int z =0; z < _size.z; z++)
                {
                    if (_cells[CellIndex(x,y,z)] && IsSurface(x, y, z))
                            AddInstance(x,y,z);
                }
    }
    void AddInstance(int x, int y, int z)
    {
        int index = CellIndex(x,y,z);
        int slot = _instanceCount++;

        _cellAtSlot[slot] = index;
        _slotOf[index] = slot;

        Vector3 localPos = new Vector3(x + 0.5f, y + 0.5f, z + 0.5f) * VoxelSize;
        _batches[slot / _batchSize][slot % _batchSize] = _localToWorld * Matrix4x4.TRS(localPos, Quaternion.identity, Vector3.one * VoxelSize);
    }

    void RemoveInstance(int cellIndex)
    {
        int slot = _slotOf[cellIndex];
        if (slot == -1)
            return;

        int last = --_instanceCount;
        if (slot != last)
        {
            int movedCell = _cellAtSlot[last];
            _batches[slot / _batchSize][slot % _batchSize] = _batches[last/ _batchSize][last%_batchSize];
            _cellAtSlot[slot] = movedCell;
            _slotOf[movedCell] = slot;
        }
        _slotOf[cellIndex] = -1;
    }

    void Capture()
    {
        Vector3 local = transform.InverseTransformPoint(_player.position) / VoxelSize;
        int radius = Mathf.CeilToInt(CaptureRadius / VoxelSize);
        float rCells = CaptureRadius / VoxelSize;
       
        //To prevent having to calculate a the sqr root each to get the distance, the sqr gets calculated once and used each time
        float rSqrt = rCells * rCells; 
        
        int cx = Mathf.FloorToInt(local.x), cy = Mathf.FloorToInt(local.y), cz = Mathf.FloorToInt(local.z);
        int captured = 0;

        for(int x = Mathf.Max(0, cx - radius); x <= Mathf.Min(_size.x -1, cx + radius);  x++)
            for(int y = Mathf.Max(0, cy - radius); y <= Mathf.Min(_size.y -1, cy + radius); y++)
                for(int z = Mathf.Max(0, cz - radius); z <= Mathf.Min(_size.z -1, cz + radius); z++)
                {
                    int i = CellIndex(x, y, z);
                    if (!_cells[i])
                        continue;

                    var center = new Vector3(x + .5f, y + .5f, z + .5f);
                    if ((center - local).sqrMagnitude > rSqrt)
                        continue;

                    _cells[i] = false; 
                    RemoveInstance(i);
                    captured++;

                    //Check if neighbouring cells are now visible
                    foreach(var d in Dirs)
                    {
                        int nx = x + d.x, ny = y +d.y, nz = z + d.z;
                        if(Solid(nx, ny,nz) && _slotOf[CellIndex(nx,ny,nz)] == -1)
                            AddInstance(nx,ny, nz); 
                    }
                }

        if(captured > 0)
        {
            Captured += captured;
            EventManager.Broadcast(Events.CaptureEvent);
        }
    }

    bool PlayerInBounds()
    {
        Vector3 local = transform.InverseTransformPoint(_player.position) / VoxelSize;
        float m = CaptureRadius / VoxelSize;
        return local.x > -m && local.y > -m && local.z > -m &&local.x < _size.x +m && local.y < _size.y + m && local.z < _size.z +m;
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
        return _cells[CellIndex(x, y, z)];
    }
}
