/* ds18635 2101128
 * ======================
 * This class is not utilised in the final version of the game,
 * but was designed to find pockets to spawn colonists in open random locations.
 * ======================
 */
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class SpawnSystem : MonoBehaviour {
    private readonly int[,] _map;
    private readonly int _mapSize;
    private readonly int _spawnPoints;
    private readonly int _spawnSpread;

    private int _biggestPocket = -1;
    private readonly List<HashSet<Vector3>> _pockets;
    private readonly List<Vector3> _spawnLocations;
    private readonly bool[,] _visited;

    /**
     * Search through a 2D array and find pockets where the player can spawn
     * using depth first search and from this we find the biggest pocket and
     * find a random spawn location to spawn our colonists at.
     */
    public SpawnSystem(int[,] map, int mapSize = 64, int spawnPoints = 3, int spawnSpread = 2) {
        _map = map;
        _mapSize = mapSize;
        _spawnPoints = spawnPoints;
        _spawnSpread = spawnSpread;
        _visited = new bool[_mapSize, _mapSize];
        _pockets = new List<HashSet<Vector3>>();
        _spawnLocations = new List<Vector3>();
    } 

    public List<Vector3> GenerateSpawnPoints() {
        FindPockets();
        IdentifyLargestPocket();
        var origin = SelectRandomSpawnOrigin();
        SetSpawnLocations(origin);
        return _spawnLocations;
    }

    #region Helper Methods

    private bool InBounds(int x, int y) {
        return InBounds(_mapSize, _mapSize, x, y);
    }

    private static bool InBounds(int maxX, int maxY, int x, int y) {
        return x >= 0 && x < maxX &&
               y >= 0 && y < maxY;
    }

    #endregion

    #region Algorithm Methods

    private void FindPockets() {
        for (var i = 0; i < _mapSize; i++)
        for (var j = 0; j < _mapSize; j++)
            if (_map[i, j] == 0 && !_visited[i, j]) {
                var newPocket = new HashSet<Vector3>();
                DepthFirstSearch(_map, _visited, i, j, newPocket);
                _pockets.Add(newPocket);
            }
    }

    private void IdentifyLargestPocket() {
        // Find largest pocket
        var largestCount = 0;
        for (var g = 0; g < _pockets.Count; g++)
            if (_pockets[g].Count > largestCount) {
                largestCount = _pockets[g].Count;
                _biggestPocket = g;
            }

        if (_biggestPocket == -1) Debug.LogError("Unable to locate spawnable pocket!");
    }

    private Vector3 SelectRandomSpawnOrigin() {
        // Pick random location within pocket
        var rand = new Random();
        var t = rand.Next(_pockets[_biggestPocket].Count);
        var index = 0;
        var chosenLocation = Vector3.zero;

        // Find specific Vector2 and save x,y
        foreach (var p in _pockets[_biggestPocket]) {
            if (index == t) {
                chosenLocation = p;
                break;
            }

            index++;
        }

        if (chosenLocation == Vector3.zero) Debug.LogError("Unable to find spawn origin point in pocket.");
        return chosenLocation;
    }

    private void SetSpawnLocations(Vector3 origin) {
        // Find random spawn points around origin
        for (var i = 0; i < _spawnPoints; i++)
            _spawnLocations.Add(GetRandomNearByPoint((int) origin.x, (int) origin.y, _pockets[_biggestPocket],
                _spawnLocations));
    }

    private Vector3 GetRandomNearByPoint(int x, int y, ICollection<Vector3> pocketLocations,
        ICollection<Vector3> spawnLocations) {
        int ranX = -1, ranY = -1;
        var foundRandomLocationNearby = false;
        while (!foundRandomLocationNearby) {
            ranX = UnityEngine.Random.Range(x - _spawnSpread, x + _spawnSpread);
            ranY = UnityEngine.Random.Range(y - _spawnSpread, y + _spawnSpread);
            if (InBounds(ranX, ranY)
                && pocketLocations.Contains(new Vector3(ranX, ranY))
                && !spawnLocations.Contains(new Vector3(ranX, ranY))) foundRandomLocationNearby = true;
        }

        return new Vector3(ranX, ranY);
    }

    private static void DepthFirstSearch(int[,] map, bool[,] visited, int i, int j, ISet<Vector3> pocket) {
        try {
            if (i < 0 || i >= map.GetLength(0)) return;
            if (j < 0 || j >= map.GetLength(1)) return;
            if (map[i, j] != 0 || visited[i, j]) return;
        }
        catch (IndexOutOfRangeException ex) {
            Debug.LogErrorFormat("Index out of range inside DFS. Possible map size mismatch " + ex.Message);
        }


        visited[i, j] = true;
        pocket.Add(new Vector3(i, j));

        DepthFirstSearch(map, visited, i + 1, j, pocket);
        DepthFirstSearch(map, visited, i - 1, j, pocket);
        DepthFirstSearch(map, visited, i, j + 1, pocket);
        DepthFirstSearch(map, visited, i, j - 1, pocket);
    }

    #endregion
}