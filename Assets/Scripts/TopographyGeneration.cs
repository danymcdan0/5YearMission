/* ds18635 2101128
 * ======================
 * This class works off the grid created in MapGeneration and handles the generation and removal of tiles in the
 * interactable layer of the game. This class handles a large number of cases to dictate which tile and at what
 * orientation it is placed, allowing for the environment to visually update according to changes the player makes to
 * the topography.
 * ======================
 */
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = System.Random;

public class TopographyGeneration : MonoBehaviour {
    public int width, height;
    public string seed;
    public Tilemap tilemap;
    public TileBase test_tile, mountain_center;

    public TileBase mountain1_North,
        mountain1_South,
        mountain1_East,
        mountain1_West,
        mountain1_NorthEast,
        mountain1_NorthWest,
        mountain1_SouthEast,
        mountain1_SouthWest,
        mountain1_NCliff,
        mountain1_SCliff,
        mountain1_SOLO,
        mountain1_CornerJoin,
        mountain1_EastWestJoin,
        mountain1_NorthSouthJoin,
        mountain1_DoubleCornerJoin,
        mountain1_PlusJoin,
        mountain1_InnerCornerJoin,
        mountain1_Connector,
        mountain1_TJoin;

    public TileBase mountain2_North,
        mountain2_South,
        mountain2_East,
        mountain2_West,
        mountain2_NorthEast,
        mountain2_NorthWest,
        mountain2_SouthEast,
        mountain2_SouthWest,
        mountain2_NCliff,
        mountain2_SCliff,
        mountain2_SOLO,
        mountain2_CornerJoin,
        mountain2_EastWestJoin,
        mountain2_NorthSouthJoin,
        mountain2_DoubleCornerJoin,
        mountain2_PlusJoin,
        mountain2_InnerCornerJoin,
        mountain2_Connector,
        mountain2_TJoin;

    public TileBase mountain3_North,
        mountain3_South,
        mountain3_East,
        mountain3_West,
        mountain3_NorthEast,
        mountain3_NorthWest,
        mountain3_SouthEast,
        mountain3_SouthWest,
        mountain3_NCliff,
        mountain3_SCliff,
        mountain3_SOLO,
        mountain3_CornerJoin,
        mountain3_EastWestJoin,
        mountain3_NorthSouthJoin,
        mountain3_DoubleCornerJoin,
        mountain3_PlusJoin,
        mountain3_InnerCornerJoin,
        mountain3_Connector,
        mountain3_TJoin;

    public TileBase wall;
    public TileBase food_1, food_2, food_3, food_4;
    public TileBase research_1, research_2, research_3, research_4;
    public TileBase ship_1, ship_2, ship_3, ship_4;
    public TileBase hub_1, hub_2, hub_3, hub_4, hub_5, hub_6, hub_7, hub_8, hub_9;
    private int[,] compareGrid, CGrid;
    private Pathfinding pathfinding;
    private Vector3Int position;
    public int[,] topography, basemap;
    //todo DOUBLE CRYSTAL GAIN

    public void Start() {
        pathfinding = new Pathfinding(width, height);
    }

    public void Update() {
        PathUpdate();
    }

    public void SetTile(Vector3 v, int type) {
        var gridX = (int) v.x;
        var gridY = (int) v.y;
        if (type == 1)
            if (topography[gridX, gridY] == 0)
                topography[gridX, gridY] = 50;
        if (type == 5) {
            //Check Neighbours 3x3 space
            //HUB
            topography[gridX, gridY] = 10;
            topography[gridX + 1, gridY] = 11;
            topography[gridX + 2, gridY] = 12;
            topography[gridX, gridY - 1] = 13;
            topography[gridX + 1, gridY - 1] = 14;
            topography[gridX + 2, gridY - 1] = 15;
            topography[gridX, gridY - 2] = 16;
            topography[gridX + 1, gridY - 2] = 17;
            topography[gridX + 2, gridY - 2] = 18;
        }

        if (type == 2) {
            //Research
            topography[gridX, gridY] = 30;
            topography[gridX + 1, gridY] = 31;
            topography[gridX, gridY - 1] = 32;
            topography[gridX + 1, gridY - 1] = 33;
        }

        if (type == 3) {
            //Food
            topography[gridX, gridY] = 20;
            topography[gridX + 1, gridY] = 21;
            topography[gridX, gridY - 1] = 22;
            topography[gridX + 1, gridY - 1] = 23;
        }

        if (type == 4) {
            //Shipyard
            topography[gridX, gridY] = 40;
            topography[gridX + 1, gridY] = 41;
            topography[gridX, gridY - 1] = 42;
            topography[gridX + 1, gridY - 1] = 43;
        }

        PathUpdate();
    }

    public void RemoveTile(Vector3 v) {
        var gridX = (int) v.x;
        var gridY = (int) v.y;
        if (topography[gridX, gridY] != 0 || topography[gridX, gridY] != 3) {
            if (topography[gridX, gridY] == 20 || topography[gridX, gridY] == 30 || topography[gridX, gridY] == 40) {
                topography[gridX, gridY] = 0;
                topography[gridX + 1, gridY] = 0;
                topography[gridX, gridY - 1] = 0;
                topography[gridX + 1, gridY - 1] = 0;
            }
            else if (topography[gridX, gridY] == 10) {
                topography[gridX, gridY] = 0;
                topography[gridX + 1, gridY] = 0;
                topography[gridX + 2, gridY] = 0;
                topography[gridX, gridY - 1] = 0;
                topography[gridX + 1, gridY - 1] = 0;
                topography[gridX + 2, gridY - 1] = 0;
                topography[gridX, gridY - 2] = 0;
                topography[gridX + 1, gridY - 2] = 0;
                topography[gridX + 2, gridY - 2] = 0;
            }
            else if (topography[gridX, gridY] < 9 || topography[gridX, gridY] == 50) {
                topography[gridX, gridY] = 0;
            }
            PathUpdate();
            Debug.Log("Tile Removed at: " + v);
        }
    }

    public void Init() {
        topography = new int[width, height];
        basemap = new int[width, height];
    }

    private void PathUpdate() {
        for (var i = 0; i < width; i++)
        for (var j = 0; j < height; j++) {
            if (basemap[i, j] == 0)
                if (pathfinding.GetNode(i, j).isWalkable == false && pathfinding.GetNode(i, j) != null)
                    pathfinding.GetNode(i, j).SetIsWalkable(!pathfinding.GetNode(i, j).isWalkable);
            if (basemap[i, j] == 1)
                if (pathfinding.GetNode(i, j).isWalkable && pathfinding.GetNode(i, j) != null)
                    pathfinding.GetNode(i, j).SetIsWalkable(!pathfinding.GetNode(i, j).isWalkable);
        }
    }

    public void SpawnClearing() {
        topography[32, 32] = 0;
        topography[32, 31] = 0;
        topography[32, 30] = 0;
        topography[32, 33] = 0;
        topography[32, 34] = 0;
        topography[31, 32] = 0;
        topography[30, 32] = 0;
        topography[33, 32] = 0;
        topography[34, 32] = 0;
        topography[31, 33] = 0;
        topography[33, 33] = 0;
        topography[31, 31] = 0;
        topography[33, 31] = 0;
    }

    public void Generate() {
        if (topography != null) {
            for (var i = 0; i < width; i++)
            for (var j = 0; j < height; j++)
                if (topography[i, j] == 0 || topography[i, j] == 3) //Ice and Dirt
                    basemap[i, j] = 0;
                else //Mountains and resources
                    basemap[i, j] = 1;
            RenderMap(topography, tilemap);
        }
    }

    public void setMap(int[,] loadedMap) {
        topography = loadedMap;
    }

    public int CountBuildings(int v) {
        int count = 0;
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                if (topography[i,j] == v) {
                    count++;
                }
            }
        }
        return count;
    } 

    public List<Vector3> spawnPoints() {
        var openLocations = new List<Vector3>();
        openLocations.Add(new Vector3(32, 32));
        openLocations.Add(new Vector3(33, 32));
        openLocations.Add(new Vector3(31, 32));
        return openLocations;
    }

    public List<int> gridToInt() {
        var topographylist = new List<int>();
        for (var i = 0; i < width; i++)
        for (var j = 0; j < height; j++)
            topographylist.Add(topography[i, j]);
        return topographylist;
    }

    public void RenderMap(int[,] map, Tilemap tilemap) { //Render tilemap
        tilemap.ClearAllTiles();
        var angle = 90f;
        var rand = new Random(seed.GetHashCode());
        for (var i = 0; i < width; i++)
        for (var j = 0; j < height; j++) {
            var transform = false;
            var selection = TileSelect(basemap, i, j);
            var yflip = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 180f, 0f), Vector3.one);
            var z90 = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, 90f), Vector3.one);
            var z270 = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, 270f), Vector3.one);
            var yzflip = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 180f, 180f), Vector3.one);
            var z180 = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, 180f), Vector3.one);
            position = new Vector3Int(i, j, 0);
            if (map[i, j] == 1)
                switch (selection) {
                    case 1:
                        tilemap.SetTile(position, mountain1_North);
                        break;
                    case 2:
                        tilemap.SetTile(position, mountain1_South);
                        break;
                    case 3:
                        tilemap.SetTile(position, mountain1_West);
                        break;
                    case 4:
                        tilemap.SetTile(position, mountain1_East);
                        break;
                    case 5:
                        tilemap.SetTile(position, mountain_center);
                        transform = true;
                        break;
                    case 6:
                        tilemap.SetTile(position, mountain1_NorthEast);
                        break;
                    case 7:
                        tilemap.SetTile(position, mountain1_SouthEast);
                        break;
                    case 8:
                        tilemap.SetTile(position, mountain1_NorthWest);
                        break;
                    case 9:
                        tilemap.SetTile(position, mountain1_SouthWest);
                        break;
                    case 10:
                        tilemap.SetTile(position, mountain1_CornerJoin);
                        break;
                    case 11:
                        tilemap.SetTile(position, mountain1_CornerJoin);
                        tilemap.SetTransformMatrix(position, z180);
                        break;
                    case 12:
                        tilemap.SetTile(position, mountain1_CornerJoin);
                        tilemap.SetTransformMatrix(position, yflip);
                        break;
                    case 13:
                        tilemap.SetTile(position, mountain1_CornerJoin);
                        tilemap.SetTransformMatrix(position, yzflip);
                        break;
                    case 14:
                        tilemap.SetTile(position, mountain1_SOLO);
                        break;
                    case 15:
                        tilemap.SetTile(position, mountain1_SCliff);
                        tilemap.SetTransformMatrix(position, z90);
                        break;
                    case 16:
                        tilemap.SetTile(position, mountain1_NCliff);
                        tilemap.SetTransformMatrix(position, z90);
                        break;
                    case 17:
                        tilemap.SetTile(position, mountain1_NCliff);
                        break;
                    case 18:
                        tilemap.SetTile(position, mountain1_SCliff);
                        break;
                    case 19:
                        tilemap.SetTile(position, mountain1_PlusJoin);
                        break;
                    case 20:
                        tilemap.SetTile(position, mountain1_Connector);
                        break;
                    case 21:
                        tilemap.SetTile(position, mountain1_Connector);
                        tilemap.SetTransformMatrix(position, z90);
                        break;
                    case 22:
                        tilemap.SetTile(position, mountain1_InnerCornerJoin);
                        tilemap.SetTransformMatrix(position, yzflip);
                        break;
                    case 23:
                        tilemap.SetTile(position, mountain1_InnerCornerJoin);
                        break;
                    case 24:
                        tilemap.SetTile(position, mountain1_InnerCornerJoin);
                        tilemap.SetTransformMatrix(position, z180);
                        break;
                    case 25:
                        tilemap.SetTile(position, mountain1_InnerCornerJoin);
                        tilemap.SetTransformMatrix(position, z90);
                        break;
                    case 26:
                        tilemap.SetTile(position, mountain1_DoubleCornerJoin);
                        tilemap.SetTransformMatrix(position, yzflip);
                        break;
                    case 27:
                        tilemap.SetTile(position, mountain1_DoubleCornerJoin);
                        break;
                    case 28:
                        tilemap.SetTile(position, mountain1_NorthSouthJoin);
                        tilemap.SetTransformMatrix(position, z270);
                        break;
                    case 29:
                        tilemap.SetTile(position, mountain1_NorthSouthJoin);
                        tilemap.SetTransformMatrix(position, z90);
                        break;
                    case 30:
                        tilemap.SetTile(position, mountain1_EastWestJoin);
                        tilemap.SetTransformMatrix(position, yzflip);
                        break;
                    case 31:
                        tilemap.SetTile(position, mountain1_EastWestJoin);
                        break;
                    case 32:
                        tilemap.SetTile(position, mountain1_TJoin);
                        tilemap.SetTransformMatrix(position, z270);
                        break;
                    case 33:
                        tilemap.SetTile(position, mountain1_TJoin);
                        tilemap.SetTransformMatrix(position, z90);
                        break;
                    case 34:
                        tilemap.SetTile(position, mountain1_TJoin);
                        tilemap.SetTransformMatrix(position, yzflip);
                        break;
                    case 35:
                        tilemap.SetTile(position, mountain1_TJoin);
                        break;
                }

            if (map[i, j] == 2)
                switch (selection) {
                    case 1:
                        tilemap.SetTile(position, mountain2_North);
                        break;
                    case 2:
                        tilemap.SetTile(position, mountain2_South);
                        break;
                    case 3:
                        tilemap.SetTile(position, mountain2_West);
                        break;
                    case 4:
                        tilemap.SetTile(position, mountain2_East);
                        break;
                    case 5:
                        tilemap.SetTile(position, mountain_center);
                        transform = true;
                        break;
                    case 6:
                        tilemap.SetTile(position, mountain2_NorthEast);
                        break;
                    case 7:
                        tilemap.SetTile(position, mountain2_SouthEast);
                        break;
                    case 8:
                        tilemap.SetTile(position, mountain2_NorthWest);
                        break;
                    case 9:
                        tilemap.SetTile(position, mountain2_SouthWest);
                        break;
                    case 10:
                        tilemap.SetTile(position, mountain2_CornerJoin);
                        break;
                    case 11:
                        tilemap.SetTile(position, mountain2_CornerJoin);
                        tilemap.SetTransformMatrix(position, z180);
                        break;
                    case 12:
                        tilemap.SetTile(position, mountain2_CornerJoin);
                        tilemap.SetTransformMatrix(position, yflip);
                        break;
                    case 13:
                        tilemap.SetTile(position, mountain2_CornerJoin);
                        tilemap.SetTransformMatrix(position, yzflip);
                        break;
                    case 14:
                        tilemap.SetTile(position, mountain2_SOLO);
                        break;
                    case 15:
                        tilemap.SetTile(position, mountain2_SCliff);
                        tilemap.SetTransformMatrix(position, z90);
                        break;
                    case 16:
                        tilemap.SetTile(position, mountain2_NCliff);
                        tilemap.SetTransformMatrix(position, z90);
                        break;
                    case 17:
                        tilemap.SetTile(position, mountain2_NCliff);
                        break;
                    case 18:
                        tilemap.SetTile(position, mountain2_SCliff);
                        break;
                    case 19:
                        tilemap.SetTile(position, mountain2_PlusJoin);
                        break;
                    case 20:
                        tilemap.SetTile(position, mountain2_Connector);
                        break;
                    case 21:
                        tilemap.SetTile(position, mountain2_Connector);
                        tilemap.SetTransformMatrix(position, z90);
                        break;
                    case 22:
                        tilemap.SetTile(position, mountain2_InnerCornerJoin);
                        tilemap.SetTransformMatrix(position, yzflip);
                        break;
                    case 23:
                        tilemap.SetTile(position, mountain2_InnerCornerJoin);
                        break;
                    case 24:
                        tilemap.SetTile(position, mountain2_InnerCornerJoin);
                        tilemap.SetTransformMatrix(position, z180);
                        break;
                    case 25:
                        tilemap.SetTile(position, mountain2_InnerCornerJoin);
                        tilemap.SetTransformMatrix(position, z90);
                        break;
                    case 26:
                        tilemap.SetTile(position, mountain2_DoubleCornerJoin);
                        tilemap.SetTransformMatrix(position, yzflip);
                        break;
                    case 27:
                        tilemap.SetTile(position, mountain2_DoubleCornerJoin);
                        break;
                    case 28:
                        tilemap.SetTile(position, mountain2_NorthSouthJoin);
                        tilemap.SetTransformMatrix(position, z270);
                        break;
                    case 29:
                        tilemap.SetTile(position, mountain2_NorthSouthJoin);
                        tilemap.SetTransformMatrix(position, z90);
                        break;
                    case 30:
                        tilemap.SetTile(position, mountain2_EastWestJoin);
                        tilemap.SetTransformMatrix(position, yzflip);
                        break;
                    case 31:
                        tilemap.SetTile(position, mountain2_EastWestJoin);
                        break;
                    case 32:
                        tilemap.SetTile(position, mountain2_TJoin);
                        tilemap.SetTransformMatrix(position, z270);
                        break;
                    case 33:
                        tilemap.SetTile(position, mountain2_TJoin);
                        tilemap.SetTransformMatrix(position, z90);
                        break;
                    case 34:
                        tilemap.SetTile(position, mountain2_TJoin);
                        tilemap.SetTransformMatrix(position, yzflip);
                        break;
                    case 35:
                        tilemap.SetTile(position, mountain2_TJoin);
                        break;
                }

            if (map[i, j] == 4)
                switch (selection) {
                    case 1:
                        tilemap.SetTile(position, mountain3_North);
                        break;
                    case 2:
                        tilemap.SetTile(position, mountain3_South);
                        break;
                    case 3:
                        tilemap.SetTile(position, mountain3_West);
                        break;
                    case 4:
                        tilemap.SetTile(position, mountain3_East);
                        break;
                    case 5:
                        tilemap.SetTile(position, mountain_center);
                        transform = true;
                        break;
                    case 6:
                        tilemap.SetTile(position, mountain3_NorthEast);
                        break;
                    case 7:
                        tilemap.SetTile(position, mountain3_SouthEast);
                        break;
                    case 8:
                        tilemap.SetTile(position, mountain3_NorthWest);
                        break;
                    case 9:
                        tilemap.SetTile(position, mountain3_SouthWest);
                        break;
                    case 10:
                        tilemap.SetTile(position, mountain3_CornerJoin);
                        break;
                    case 11:
                        tilemap.SetTile(position, mountain3_CornerJoin);
                        tilemap.SetTransformMatrix(position, z180);
                        break;
                    case 12:
                        tilemap.SetTile(position, mountain3_CornerJoin);
                        tilemap.SetTransformMatrix(position, yflip);
                        break;
                    case 13:
                        tilemap.SetTile(position, mountain3_CornerJoin);
                        tilemap.SetTransformMatrix(position, yzflip);
                        break;
                    case 14:
                        tilemap.SetTile(position, mountain3_SOLO);
                        break;
                    case 15:
                        tilemap.SetTile(position, mountain3_SCliff);
                        tilemap.SetTransformMatrix(position, z90);
                        break;
                    case 16:
                        tilemap.SetTile(position, mountain3_NCliff);
                        tilemap.SetTransformMatrix(position, z90);
                        break;
                    case 17:
                        tilemap.SetTile(position, mountain3_NCliff);
                        break;
                    case 18:
                        tilemap.SetTile(position, mountain3_SCliff);
                        break;
                    case 19:
                        tilemap.SetTile(position, mountain3_PlusJoin);
                        break;
                    case 20:
                        tilemap.SetTile(position, mountain3_Connector);
                        break;
                    case 21:
                        tilemap.SetTile(position, mountain3_Connector);
                        tilemap.SetTransformMatrix(position, z90);
                        break;
                    case 22:
                        tilemap.SetTile(position, mountain3_InnerCornerJoin);
                        tilemap.SetTransformMatrix(position, yzflip);
                        break;
                    case 23:
                        tilemap.SetTile(position, mountain3_InnerCornerJoin);
                        break;
                    case 24:
                        tilemap.SetTile(position, mountain3_InnerCornerJoin);
                        tilemap.SetTransformMatrix(position, z180);
                        break;
                    case 25:
                        tilemap.SetTile(position, mountain3_InnerCornerJoin);
                        tilemap.SetTransformMatrix(position, z90);
                        break;
                    case 26:
                        tilemap.SetTile(position, mountain3_DoubleCornerJoin);
                        tilemap.SetTransformMatrix(position, yzflip);
                        break;
                    case 27:
                        tilemap.SetTile(position, mountain3_DoubleCornerJoin);
                        break;
                    case 28:
                        tilemap.SetTile(position, mountain3_NorthSouthJoin);
                        tilemap.SetTransformMatrix(position, z270);
                        break;
                    case 29:
                        tilemap.SetTile(position, mountain3_NorthSouthJoin);
                        tilemap.SetTransformMatrix(position, z90);
                        break;
                    case 30:
                        tilemap.SetTile(position, mountain3_EastWestJoin);
                        tilemap.SetTransformMatrix(position, yzflip);
                        break;
                    case 31:
                        tilemap.SetTile(position, mountain3_EastWestJoin);
                        break;
                    case 32:
                        tilemap.SetTile(position, mountain3_TJoin);
                        tilemap.SetTransformMatrix(position, z270);
                        break;
                    case 33:
                        tilemap.SetTile(position, mountain3_TJoin);
                        tilemap.SetTransformMatrix(position, z90);
                        break;
                    case 34:
                        tilemap.SetTile(position, mountain3_TJoin);
                        tilemap.SetTransformMatrix(position, yzflip);
                        break;
                    case 35:
                        tilemap.SetTile(position, mountain3_TJoin);
                        break;
                }

            if (map[i, j] == 10) tilemap.SetTile(position, hub_1);
            if (map[i, j] == 11) tilemap.SetTile(position, hub_2);
            if (map[i, j] == 12) tilemap.SetTile(position, hub_3);
            if (map[i, j] == 13) tilemap.SetTile(position, hub_4);
            if (map[i, j] == 14) tilemap.SetTile(position, hub_5);
            if (map[i, j] == 15) tilemap.SetTile(position, hub_6);
            if (map[i, j] == 16) tilemap.SetTile(position, hub_7);
            if (map[i, j] == 17) tilemap.SetTile(position, hub_8);
            if (map[i, j] == 18) tilemap.SetTile(position, hub_9);
            if (map[i, j] == 20) tilemap.SetTile(position, food_1);
            if (map[i, j] == 21) tilemap.SetTile(position, food_2);
            if (map[i, j] == 22) tilemap.SetTile(position, food_3);
            if (map[i, j] == 23) tilemap.SetTile(position, food_4);
            if (map[i, j] == 30) tilemap.SetTile(position, research_1);
            if (map[i, j] == 31) tilemap.SetTile(position, research_2);
            if (map[i, j] == 32) tilemap.SetTile(position, research_3);
            if (map[i, j] == 33) tilemap.SetTile(position, research_4);
            if (map[i, j] == 40) tilemap.SetTile(position, ship_1);
            if (map[i, j] == 41) tilemap.SetTile(position, ship_2);
            if (map[i, j] == 42) tilemap.SetTile(position, ship_3);
            if (map[i, j] == 43) tilemap.SetTile(position, ship_4);

            if (map[i, j] == 50) tilemap.SetTile(position, wall);
            var index = rand.Next(4);
            if (transform) {
                switch (index) {
                    case 0:
                        angle = 0f;
                        break;
                    case 1:
                        angle = 90f;
                        break;
                    case 2:
                        angle = 180f;
                        break;
                    case 3:
                        angle = 270f;
                        break;
                    case 4:
                        angle = 360f;
                        break;
                }

                var matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, angle), Vector3.one);
                tilemap.SetTransformMatrix(position, matrix);
            }
        }
    }

    private bool WithinArrayBounds(int x, int y) {
        // Inclusive Lower Bound
        // Exclusive Upper Bound
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    private int TileSelect(int[,] map, int x, int y) {
        CGrid = new[,] {
            {1, 1, 1},
            {1, 1, 1},
            {1, 1, 1}
        };

        int a, b, c, d, e, f, g, h;
        a = WithinArrayBounds(x - 1, y + 1) ? map[x - 1, y + 1] : 1;
        b = WithinArrayBounds(x, y + 1) ? map[x, y + 1] : 1;
        c = WithinArrayBounds(x + 1, y + 1) ? map[x + 1, y + 1] : 1;
        d = WithinArrayBounds(x - 1, y) ? map[x - 1, y] : 1;
        e = WithinArrayBounds(x + 1, y) ? map[x + 1, y] : 1;
        f = WithinArrayBounds(x - 1, y - 1) ? map[x - 1, y - 1] : 1;
        g = WithinArrayBounds(x, y - 1) ? map[x, y - 1] : 1;
        h = WithinArrayBounds(x + 1, y - 1) ? map[x + 1, y - 1] : 1;

        compareGrid = new[,] {
            {a, b, c},
            {d, map[x, y], e},
            {f, g, h}
        };
        var counter = 0;
        for (var i = 0; i < 3; i++)
        for (var j = 0; j < 3; j++)
            if (CGrid[i, j] == compareGrid[i, j]) {
                counter++;
                if (counter == 9) return 5;
            }

        if (compareGrid[0, 1] == 1 && compareGrid[1, 0] == 1 && compareGrid[2, 1] == 1 && compareGrid[1, 2] == 1 &&
            compareGrid[0, 0] == 0 && compareGrid[2, 0] == 0 && compareGrid[0, 2] == 0 && compareGrid[2, 2] == 0)
            return 19;
        if (compareGrid[0, 1] == 1 && compareGrid[2, 1] == 1 && compareGrid[1, 0] == 0 &&
            compareGrid[1, 2] == 0) return 20;
        if (compareGrid[1, 0] == 1 && compareGrid[1, 2] == 1 && compareGrid[2, 1] == 0 &&
            compareGrid[0, 1] == 0) return 21;
        if (compareGrid[1, 0] == 1 && compareGrid[0, 1] == 1 && compareGrid[0, 0] == 0 && compareGrid[2, 1] == 0 &&
            compareGrid[1, 2] == 0)
            return 22;
        if (compareGrid[1, 0] == 1 && compareGrid[2, 1] == 1 && compareGrid[2, 0] == 0 && compareGrid[0, 1] == 0 &&
            compareGrid[1, 2] == 0)
            return 23;
        if (compareGrid[0, 1] == 1 && compareGrid[1, 2] == 1 && compareGrid[0, 2] == 0 && compareGrid[2, 1] == 0 &&
            compareGrid[1, 0] == 0)
            return 24;
        if (compareGrid[2, 1] == 1 && compareGrid[1, 2] == 1 && compareGrid[1, 0] == 0 && compareGrid[0, 1] == 0 &&
            compareGrid[2, 2] == 0)
            return 25;
        if (compareGrid[0, 1] == 1 && compareGrid[1, 0] == 1 && compareGrid[2, 1] == 1 && compareGrid[1, 2] == 1 &&
            compareGrid[0, 0] == 0 && compareGrid[2, 2] == 0)
            return 26;
        if (compareGrid[0, 1] == 1 && compareGrid[1, 0] == 1 && compareGrid[2, 1] == 1 && compareGrid[1, 2] == 1 &&
            compareGrid[2, 0] == 0 && compareGrid[0, 2] == 0)
            return 27;
        if (compareGrid[1, 0] == 1 && compareGrid[0, 1] == 1 && compareGrid[2, 1] == 1 && compareGrid[0, 0] == 0 &&
            compareGrid[2, 0] == 0 && compareGrid[1, 2] == 0)
            return 32;
        if (compareGrid[2, 1] == 1 && compareGrid[0, 1] == 1 && compareGrid[1, 2] == 1 && compareGrid[0, 2] == 0 &&
            compareGrid[2, 2] == 0 && compareGrid[1, 0] == 0)
            return 33;
        if (compareGrid[1, 0] == 1 && compareGrid[0, 1] == 1 && compareGrid[1, 2] == 1 && compareGrid[0, 2] == 0 &&
            compareGrid[0, 0] == 0 && compareGrid[2, 1] == 0)
            return 34;
        if (compareGrid[1, 0] == 1 && compareGrid[2, 1] == 1 && compareGrid[1, 2] == 1 && compareGrid[2, 0] == 0 &&
            compareGrid[2, 2] == 0 && compareGrid[0, 1] == 0)
            return 35;
        if (compareGrid[1, 0] == 1 && compareGrid[0, 1] == 1 && compareGrid[1, 2] == 1 && compareGrid[0, 2] == 0 &&
            compareGrid[0, 0] == 0)
            return 30;
        if (compareGrid[1, 0] == 1 && compareGrid[2, 1] == 1 && compareGrid[1, 2] == 1 && compareGrid[2, 0] == 0 &&
            compareGrid[2, 2] == 0)
            return 31;
        if (compareGrid[1, 0] == 1 && compareGrid[0, 1] == 1 && compareGrid[2, 1] == 1 && compareGrid[0, 0] == 0 &&
            compareGrid[2, 0] == 0)
            return 28;
        if (compareGrid[2, 1] == 1 && compareGrid[0, 1] == 1 && compareGrid[1, 2] == 1 && compareGrid[0, 2] == 0 &&
            compareGrid[2, 2] == 0)
            return 29;
        if (compareGrid[1, 0] == 1 && compareGrid[2, 1] == 1 && compareGrid[1, 2] == 1 &&
            compareGrid[0, 1] == 0) return 1;
        if (compareGrid[1, 0] == 1 && compareGrid[0, 1] == 1 && compareGrid[1, 2] == 1 &&
            compareGrid[2, 1] == 0) return 2;
        if (compareGrid[0, 1] == 1 && compareGrid[1, 0] == 1 && compareGrid[2, 1] == 1 &&
            compareGrid[1, 2] == 0) return 3;
        if (compareGrid[0, 1] == 1 && compareGrid[1, 2] == 1 && compareGrid[2, 1] == 1 &&
            compareGrid[1, 0] == 0) return 4;
        if (compareGrid[2, 1] == 1 && compareGrid[2, 2] == 1 && compareGrid[1, 2] == 1 && compareGrid[0, 1] == 0
            && compareGrid[1, 0] == 0)
            return 6;
        if (compareGrid[0, 1] == 1 && compareGrid[1, 2] == 1 && compareGrid[1, 0] == 0 &&
            compareGrid[2, 1] == 0) return 7;
        if (compareGrid[1, 0] == 1 && compareGrid[2, 1] == 1 && compareGrid[1, 2] == 0 &&
            compareGrid[0, 1] == 0) return 8;
        if (compareGrid[1, 0] == 1 && compareGrid[0, 1] == 1 && compareGrid[1, 2] == 0 &&
            compareGrid[2, 1] == 0) return 9;
        if (compareGrid[1, 0] == 1 && compareGrid[1, 2] == 1 && compareGrid[2, 1] == 1 && compareGrid[0, 1] == 1 &&
            compareGrid[2, 0] == 0)
            return 10;
        if (compareGrid[1, 0] == 1 && compareGrid[1, 2] == 1 && compareGrid[2, 1] == 1 && compareGrid[0, 1] == 1 &&
            compareGrid[0, 2] == 0)
            return 11;
        if (compareGrid[1, 0] == 1 && compareGrid[1, 2] == 1 && compareGrid[2, 1] == 1 && compareGrid[0, 1] == 1 &&
            compareGrid[2, 2] == 0)
            return 12;
        if (compareGrid[1, 0] == 1 && compareGrid[1, 2] == 1 && compareGrid[2, 1] == 1 && compareGrid[0, 1] == 1 &&
            compareGrid[0, 0] == 0)
            return 13;
        if (compareGrid[1, 0] == 0 && compareGrid[0, 1] == 0 && compareGrid[2, 1] == 0 &&
            compareGrid[1, 2] == 0) return 14;
        if (compareGrid[1, 0] == 1 && compareGrid[0, 1] == 0 && compareGrid[2, 1] == 0 &&
            compareGrid[1, 2] == 0) return 15;
        if (compareGrid[1, 2] == 1 && compareGrid[0, 1] == 0 && compareGrid[2, 1] == 0 &&
            compareGrid[1, 0] == 0) return 16;
        if (compareGrid[2, 1] == 1 && compareGrid[1, 0] == 0 && compareGrid[1, 2] == 0 &&
            compareGrid[0, 1] == 0) return 17;
        if (compareGrid[0, 1] == 1 && compareGrid[1, 0] == 0 && compareGrid[1, 2] == 0 &&
            compareGrid[2, 1] == 0) return 18;
        return 0;
    }
}