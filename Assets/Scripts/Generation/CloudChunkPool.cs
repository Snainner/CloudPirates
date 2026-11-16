using System.Collections.Generic;
using UnityEngine;

public class CloudChunkPool 
{
    readonly Stack<CloudChunk> _pool = new Stack<CloudChunk>();

    public CloudChunk AcquireChunk(Vector3Int coord, Material material, Transform parent, float voxelSize)
    {
        CloudChunk chunk;
        if (_pool.Count > 0)
            chunk = _pool.Pop();
        else
        {
            chunk = new CloudChunk(parent, material);
        }
        chunk.SetLocation(coord, VoxelConstants.ChunkSize, voxelSize);
        
        return chunk;
    }

    //Don't forget to remove the chunk from the local dictionary
    public void ReleaseChunk(CloudChunk chunk)
    {
        chunk.Mesh.Clear();
        chunk.Go.SetActive(false);
        System.Array.Clear(chunk.Cells, 0, chunk.Cells.Length);
        chunk.SolidCount = 0;
        chunk.Dirty = false;
        _pool.Push(chunk);
    }

    private void OnDestroy()
    {
        foreach(var chunk in _pool)
        {
            if (chunk.Mesh)
            {
                //System.Destroy(chunk.Mesh);
                //Destroy mesh object for each chunk 
            }
        }
    }
}
