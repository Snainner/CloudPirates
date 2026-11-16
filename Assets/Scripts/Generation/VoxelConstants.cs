using System.Collections.Generic;
using UnityEngine;

public static class VoxelConstants 
{
    public static int Shift = 4;
    public static int ChunkSize = 1 << Shift;
    public static int Mask = ChunkSize - 1;
    public static int CellsPerChunk = ChunkSize * ChunkSize * ChunkSize;
    
    public static readonly Vector3Int[] Dirs =
{
        new(1,0,0), new(-1,0,0), new(0,1,0), new(0,-1,0), new(0,0,1), new(0,0,-1)
    };

    public static readonly Vector3[][] FaceCorners =
    {
        new[] { new Vector3(1,0,0), new Vector3(1,1,0), new Vector3(1,1,1), new Vector3(1,0,1) }, // +X
        new[] { new Vector3(0,0,0), new Vector3(0,0,1), new Vector3(0,1,1), new Vector3(0,1,0) }, // -X
        new[] { new Vector3(0,1,0), new Vector3(0,1,1), new Vector3(1,1,1), new Vector3(1,1,0) }, // +Y
        new[] { new Vector3(0,0,0), new Vector3(1,0,0), new Vector3(1,0,1), new Vector3(0,0,1) }, // -Y
        new[] { new Vector3(0,0,1), new Vector3(1,0,1), new Vector3(1,1,1), new Vector3(0,1,1) }, // +Z
        new[] { new Vector3(0,0,0), new Vector3(0,1,0), new Vector3(1,1,0), new Vector3(1,0,0) }, // -Z
    };
    
    // cell (x,y,z) -> chunk coord: shift right by 4 (= divide by 16, rounds DOWN even for negatives)
    // cell (x,y,z) -> local pos inside chunk: x & 15
    public static int LocalPosIndex(int lx, int ly, int lz)
    {
        return lx | (ly << Shift) | (lz << (Shift * 2));
    }
    public static Vector3Int ChunkCoordOf(Vector3Int cell)
    { 
        return new Vector3Int(cell.x >> Shift, cell.y >> Shift, cell.z >> Shift); 
    }
    public static bool IsSolid(Vector3Int cell, Dictionary<Vector3Int, CloudChunk> chunks)
    {

        if (!chunks.TryGetValue(ChunkCoordOf(cell), out var chunk)) 
            return false;   // no chunk = empty air

        return chunk.Cells[LocalPosIndex(cell.x & Mask, cell.y & Mask, cell.z & Mask)];
    }
}
