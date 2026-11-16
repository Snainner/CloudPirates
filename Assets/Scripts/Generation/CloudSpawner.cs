using System.Collections.Generic;
using UnityEngine;


public static class CloudSpawner 
{
    //static public GameObject CloudPrefab;
    //[SerializeField]
    //static Vector3 _areaSize = new Vector3(500, 100, 500);
    //[SerializeField]
    //static int _cloudCount = 30;
    //[SerializeField]
    //static float _minDistance = 200f;
    //[SerializeField]
    //static Vector3Int _minCloudSize = new Vector3Int(20, 8, 20);
    //[SerializeField]
    //static Vector3Int _maxCloudSize = new Vector3Int(40,16,40);

    //static CloudManager _cloudManager;
    

    //async void Start()
    //{
    //    _cloudManager = CloudManager.Instance;
    //    await GenerateClouds();

    //}
    public static List<Vector3> GenerateSpawnPoints(Vector3 origin, int amount, Vector3 areaSize, float minDistBetweenClouds)
    {
        var points = new List<Vector3>();
        int tries = 0;

        while (points.Count < amount && tries++ < amount * 20)
        {
            Vector3 p = origin + new Vector3(
                Random.Range(-areaSize.x / 2, areaSize.x / 2),
                Random.Range(-areaSize.y / 2, areaSize.y / 2),
                Random.Range(-areaSize.z / 2, areaSize.z / 2));

            bool tooClose = false;
            foreach (var other in points)
            {
                if ((other - p).sqrMagnitude < minDistBetweenClouds * minDistBetweenClouds)
                { 
                    tooClose = true;
                    break; 
                }
            }

            if (tooClose) 
                continue;

            points.Add(p);
        }
        return points;
        //await SpawnCloud(points);

        //_cloudManager.FlushQueueRoutine(int.MaxValue);
    }

    //async Task SpawnCloud(List<Vector3> positions)
    //{
    //    foreach(Vector3 p in positions)
    //        await _cloudManager.AddCloudAsync(p, RandomCloudSize(_minCloudSize, _maxCloudSize));
    //}

    public static Vector3Int RandomCloudSize(Vector3Int minSize, Vector3Int maxSize)
    {
        var rng = new System.Random();
        var size = new Vector3Int(
            rng.Next(minSize.x, maxSize.x + 1),
            rng.Next(minSize.y, maxSize.y + 1),
            rng.Next(minSize.z, maxSize.z + 1)
        );

        return size;
    }
    //void SpawnCloud(Vector3 pos)
    //{
    //    Vector3Int size = RandomCloudSize();
    //    var go = Instantiate(CloudPrefab, pos, Quaternion.identity);
    //    var cloud = go.GetComponent<VoxelCloud2>();

    //    if (cloud != null)
    //        cloud.BuildCloud(size);
    //    else
    //        Debug.Log("<color = red>CloudSpawner: No VoxelCloud2 component found on cloud prefab</color>");
    //}
}


