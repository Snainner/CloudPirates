using NaughtyAttributes;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    GameObject _enemyPrefab;
    List<Enemy> _enemies;
    Vector2Int _enemyCountRange;
    [SerializeField]
    float _spawnDist = -20f;
    [SerializeField]
    int _spawnPoints = 5;

    List<Vector3> _spawns = new List<Vector3>();
    //Set an enemy timer
    //After timer runs out 

    //Ch


    [Button]
    public void SpawnEnemy()
    {
        var spawn = GetSpawnLocation();
        Instantiate(_enemyPrefab,spawn, Quaternion.identity);
    }

    void ShowSpawnPoint()
    {
        _spawns.Clear();
        for (int i = 0; i < _spawnPoints; i++)
        {
            _spawns.Add(GetSpawnLocation());
        }
    }
    Vector3 GetSpawnLocation()
    {

        var leftEdge = Camera.main.ViewportToWorldPoint(new Vector2(0, 1));
        var spawnPoint = leftEdge + (_spawnDist * Vector3.forward);
        return spawnPoint;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if (_spawns.Count <= 0)
            return;

        foreach(var s in _spawns )
        {
            Gizmos.DrawSphere(s, .4f);
        }
    }
}
