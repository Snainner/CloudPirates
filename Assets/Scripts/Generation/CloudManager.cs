using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Image;

public class CloudManager : MonoBehaviour
{
    public static CloudManager Instance { get; private set; }
    //Transform _player;

    public Material Material;
    [SerializeField]
    int _cloudAmount = 100;
    public float VoxelSize = 1f;
    [SerializeField]
    Vector3Int _minCloudSize = new Vector3Int(20, 8, 20);
    [SerializeField]
    Vector3Int _maxCloudSize = new Vector3Int(40, 16, 40);
    [SerializeField]
    float _minDistBtwnClouds = 20f;
    public float CaptureRadius = 3f;
    [SerializeField]
    Vector3 _spawnArea = new Vector3(1000, 100, 1000);

    //[NaughtyAttributes.ReadOnly]
    //[SerializeField]
    // int Captured = 0;
    
    CloudChunkPool _objectPool;
    public int RebuildsPerFrame = 4;
    bool _isFlushing;
    readonly Dictionary<Vector3Int, CloudChunk> _chunks = new Dictionary<Vector3Int, CloudChunk>();
    public Dictionary<Vector3Int, CloudChunk> Chunks { get { return _chunks; } }
    CancellationTokenSource Cts;


    public void SetChunks(Dictionary<Vector3Int, CloudChunk> value)
    { SetChunks(_chunks); }
    readonly Queue<CloudChunk> _dirtyChunks = new Queue<CloudChunk>();

    static int _shift = VoxelConstants.Shift;
    static int _mask = VoxelConstants.Mask;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(Instance);
            return;
        }

        if(_objectPool == null)
            _objectPool = new CloudChunkPool();
    }
    private async void Start()
    {
        //_player = GameManager.Instance.Player.transform;
        SpawnClouds();
        FlushQueueRoutine(1000);
    }
    private void Update()
    {
        //Capture();
        if(!_isFlushing)
            ProcessQueue(RebuildsPerFrame);
    }
    private void OnDestroy()
    {
        foreach(var chunk in _chunks.Values)
        {
            if(chunk.Mesh)
                Destroy(chunk.Mesh);
        }
        //foreach(var chunk in _objectPool)
        //{
        //    if(chunk.Mesh)
        //        { Destroy(chunk.Mesh); }
        //}
        if(Instance == this)
            Instance = null;
    }

    async void SpawnClouds()
    {
        List<Vector3> spawnPoints = CloudSpawner.GenerateSpawnPoints(transform.position, _cloudAmount,_spawnArea, _minDistBtwnClouds);
        foreach (Vector3 p in spawnPoints)
            await AddCloudAsync(p, CloudSpawner.RandomCloudSize(_minCloudSize, _maxCloudSize));
    }

    public int CaptureInChunk(CloudChunk chunk, Vector3 p, float rSqr, Vector3Int min, Vector3Int max)
    {
        int ox = chunk.Location.x << _shift, oy = chunk.Location.y << _shift, oz = chunk.Location.z << _shift;

        // capture box clamped to this chunk, in local coords
        int x0 = Mathf.Max(min.x, ox) - ox, x1 = Mathf.Min(max.x, ox + _mask) - ox;
        int y0 = Mathf.Max(min.y, oy) - oy, y1 = Mathf.Min(max.y, oy + _mask) - oy;
        int z0 = Mathf.Max(min.z, oz) - oz, z1 = Mathf.Min(max.z, oz + _mask) - oz;

        int removed = 0;

        for (int lx = x0; lx <= x1; lx++)
            for (int ly = y0; ly <= y1; ly++)
                for (int lz = z0; lz <= z1; lz++)
                {
                    int i = VoxelConstants.LocalPosIndex(lx, ly, lz);
                    if (!chunk.Cells[i]) continue;

                    var center = new Vector3(ox + lx + .5f, oy + ly + .5f, oz + lz + .5f);
                    if ((center - p).sqrMagnitude > rSqr) continue;

                    chunk.Cells[i] = false;
                    chunk.SolidCount--;
                    removed++;
                    MarkDirty(chunk);

                    // On a chunk edge? The neighbouring chunk just got a newly exposed face.
                    if (lx == 0) MarkNeighbourDirty(chunk.Location + new Vector3Int(-1, 0, 0), _chunks);
                    if (lx == _mask) MarkNeighbourDirty(chunk.Location + new Vector3Int(1, 0, 0), _chunks);
                    if (ly == 0) MarkNeighbourDirty(chunk.Location + new Vector3Int(0, -1, 0), _chunks);
                    if (ly == _mask) MarkNeighbourDirty(chunk.Location + new Vector3Int(0, 1, 0), _chunks);
                    if (lz == 0) MarkNeighbourDirty(chunk.Location + new Vector3Int(0, 0, -1), _chunks);
                    if (lz == _mask) MarkNeighbourDirty(chunk.Location + new Vector3Int(0, 0, 1), _chunks);
                }
        
        return removed;
    }
    public async Awaitable<List<Vector3Int>> CalculateCloudAsync(Vector3Int size)
    {
        float seed = Random.value * 1000f; //Unity random needs to be on the main thread
        await Awaitable.BackgroundThreadAsync();

        var cells = new List<Vector3Int>();
        Vector3 c = (Vector3)size / 2f;

        for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
                for (int z = 0; z < size.z; z++)
                {
                    var d = new Vector3((x - c.x) / c.x, (y - c.y) / c.y, (z - c.z) / c.z);
                    float falloff = 1f - d.magnitude;
                    float n = Mathf.PerlinNoise(x * .15f + seed, z * .15f + seed) * .5f
                            + Mathf.PerlinNoise(y * .2f + 50f + seed, x * .2f + seed) * .5f;

                    if (falloff + (n - .5f) * .8f > .2f)
                        cells.Add(new Vector3Int(x, y, z));
                }

        await Awaitable.MainThreadAsync(); //Need to switch back to the main thread to make the data accessible 
        return cells;
    }
    public async Task AddCloudAsync(Vector3 worldCenter, Vector3Int size)
    {
        Vector3 localCenter = transform.InverseTransformPoint(worldCenter) / VoxelSize;
        Vector3Int origin = Vector3Int.RoundToInt(localCenter - (Vector3)size * 0.5f);

        var cells = await CalculateCloudAsync(size);
        foreach (var cell in cells)
            AddCell(origin + cell);
    }
    void AddCell(Vector3Int cell)
    {
        var coord = VoxelConstants.ChunkCoordOf(cell);
        if (!_chunks.TryGetValue(coord, out var chunk))
        {
            chunk = _objectPool.AcquireChunk(coord, Material, transform, VoxelSize);// chunks are created lazily
            _chunks.Add(coord, chunk);
        }

        int mask = VoxelConstants.Mask;

        int i = VoxelConstants.LocalPosIndex(cell.x & mask, cell.y & mask, cell.z & mask);
        
        if (chunk.Cells[i])
            return;  // overlapping clouds merge
        
        chunk.Cells[i] = true;
        chunk.SolidCount++;
        MarkDirty(chunk);
    }

    //Dirty Queue
    void MarkNeighbourDirty(Vector3Int coord, Dictionary<Vector3Int, CloudChunk> chunks)
    {
        if(chunks.TryGetValue(coord, out var c))
            MarkDirty(c);
    }
    void MarkDirty(CloudChunk chunk)
    {
        if (chunk.Dirty)
            return;
        chunk.Dirty = true;
        _dirtyChunks.Enqueue(chunk);
    }
    void ProcessQueue(int budget)
    {
        while(budget-- > 0 && _dirtyChunks.Count > 0)
        {
            var chunk = _dirtyChunks.Dequeue();
            chunk.Dirty = false;
            if(chunk.SolidCount == 0)
            {
                _chunks.Remove(chunk.Location);
                _objectPool.ReleaseChunk(chunk);
                continue;
            }

            MeshBuilder.BuildChunkMesh(chunk, VoxelSize, NeighbourSolid);
        }
    }

    public IEnumerator FlushQueueRoutine(int ChunksPerFrame = 16)
    {
        _isFlushing = true;
        while (_dirtyChunks.Count > 0)
        {
            ProcessQueue(ChunksPerFrame);
            yield return null;
        }
        _isFlushing = false;
    }
    /// <summary>
    /// Call after spawning a big batch to build all the pending chunks
    /// </summary>
    public void FlushQueue()
    {
        ProcessQueue(int.MaxValue);
    }
    // Inside this chunk: read the array directly (fast).
    // Outside it: ask the world (dictionary lookup).
    bool NeighbourSolid(CloudChunk chunk, int lx, int ly, int lz)
    {
        int chunkSize = VoxelConstants.ChunkSize;
        if ((uint)lx < chunkSize && (uint)ly < chunkSize && (uint)lz < chunkSize)
            return chunk.Cells[VoxelConstants.LocalPosIndex(lx, ly, lz)];

        var cell = (chunk.Location * chunkSize) + new Vector3Int(lx, ly, lz);
        
        return VoxelConstants.IsSolid(cell, _chunks);
    }
    //Chunk Pool

    #region Depreciated
    //public void AddCloud(Vector3 worldCenter, Vector3Int size)
    //{
    //    Vector3 localCenter = transform.InverseTransformPoint(worldCenter) / VoxelSize;
    //    Vector3Int origin = Vector3Int.RoundToInt(localCenter - (Vector3)size * 0.5f);

    //    Vector3 c = (Vector3)size / 2f;
    //    float seed = Random.value * 1000f;

    //    for (int x = 0; x < size.x; x++)
    //        for (int y = 0; y < size.y; y++)
    //            for (int z = 0; z < size.z; z++)
    //            {
    //                var d = new Vector3((x - c.x) / c.x, (y - c.y) / c.y, (z - c.z) / c.z);
    //                float falloff = 1f - d.magnitude;
    //                float n = Mathf.PerlinNoise(x * .15f + seed, z * .15f + seed) * .5f
    //                        + Mathf.PerlinNoise(y * .2f + 50f + seed, x * .2f + seed) * .5f;

    //                if (falloff + (n - .5f) * .8f > .2f)
    //                    AddCell(origin + new Vector3Int(x, y, z));
    //            }
    //}
    //void Capture()
    //{
    //    Vector3 p = transform.InverseTransformPoint(_player.position) / VoxelSize;
    //    float r = CaptureRadius / VoxelSize;
    //    float rSqr = r * r;

    //    Vector3Int min = Vector3Int.FloorToInt(p - Vector3.one * r);
    //    Vector3Int max = Vector3Int.FloorToInt(p + Vector3.one * r);

    //    int grabbed = 0;

    //    // Only visit the (at most 8) chunks touching the capture box.
    //    // Missing chunk = empty space = skipped instantly.
    //    for (int cx = min.x >> _shift; cx <= max.x >> _shift; cx++)
    //        for (int cy = min.y >> _shift; cy <= max.y >> _shift; cy++)
    //            for (int cz = min.z >> _shift; cz <= max.z >> _shift; cz++)
    //            {
    //                if (!_chunks.TryGetValue(new Vector3Int(cx, cy, cz), out var chunk)) continue;
    //                grabbed += CaptureInChunk(chunk, p, rSqr, min, max);
    //            }

    //    if (grabbed > 0)
    //    {
    //        Captured += grabbed;
    //        EventManager.Broadcast(Events.CaptureEvent);     // once per frame
    //    }
    //}
    #endregion
}
