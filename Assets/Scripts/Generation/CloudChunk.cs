using UnityEngine;

public class CloudChunk
{
    public Vector3Int Location;
    public bool[] Cells = new bool[VoxelConstants.CellsPerChunk];
    public int SolidCount;
    public GameObject Go;
    public Mesh Mesh;
    public bool Dirty = false;

    public CloudChunk(Transform parent, Material material)
    {
        Go = CreateChunkGo(parent, material);
    }

    GameObject CreateChunkGo(Transform parent, Material material)
    {
        GameObject go = new GameObject("Chunk");
        go.transform.SetParent(parent, false);
        go.AddComponent<MeshRenderer>().sharedMaterial = material;

        Mesh = new Mesh { indexFormat = UnityEngine.Rendering.IndexFormat.UInt32 };
        Mesh.MarkDynamic();

        go.AddComponent<MeshFilter>().sharedMesh = Mesh;
        return go;
    }

    public void SetLocation(Vector3Int location, int chunkSize, float voxelSize)
    {
        Location = location;
        Go.name = "Chunk: " + location;
        Go.transform.localPosition = (Vector3)(location * chunkSize) * voxelSize;
    }
}
