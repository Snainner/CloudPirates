using System;
using System.Collections.Generic;
using UnityEngine;

public static class MeshBuilder 
{
    static readonly List<Vector3> _verts = new List<Vector3>(4096);
    static readonly List<Vector3> _normals = new List<Vector3>(4096);
    static readonly List<int> _tris = new List<int>(8192);
    public static void BuildChunkMesh(CloudChunk chunk,float voxelSize, Func<CloudChunk, int, int, int, bool> neighbourSolid)
    {
        _verts.Clear();
        _normals.Clear();
        _tris.Clear();
        int chunkSize = VoxelConstants.ChunkSize;
        for (int lx = 0; lx < chunkSize; lx++)
            for(int ly = 0; ly < chunkSize; ly++)
                for(int lz = 0; lz < chunkSize; lz++)
                {
                    if(!chunk.Cells[VoxelConstants.LocalPosIndex(lx, ly, lz)])
                            continue;

                    var local = new Vector3(lx, ly, lz);
                    for (int f = 0; f < 6; f++)
                    {
                        var dir = VoxelConstants.Dirs[f];
                        //Continue when true as face is hidden 
                        if (neighbourSolid(chunk, lx + dir.x, ly +dir.y, lz + dir.z))
                                continue;
                        
                        int start = _verts.Count;
                        var corners = VoxelConstants.FaceCorners[f];
                        for(int k = 0; k < 4;  k++)
                        {
                            _verts.Add((local + corners[k]) * voxelSize);
                            _normals.Add(new Vector3(dir.x, dir.y, dir.z));
                        }
                        _tris.Add(start); _tris.Add(start + 1); _tris.Add(start + 2);
                        _tris.Add(start); _tris.Add(start + 2); _tris.Add(start + 3);
                    }
                }

        var mesh = chunk.Mesh;
        mesh.Clear();

        if (_verts.Count == 0)
        {
            chunk.Go.SetActive(false);
            return;
        }

        chunk.Go.SetActive(true);
        mesh.SetVertices(_verts);
        mesh.SetNormals(_normals);
        mesh.SetTriangles(_tris, 0);
        mesh.RecalculateBounds();

    }

}
