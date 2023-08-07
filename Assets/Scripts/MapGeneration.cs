/* ds18635 2101128
 * ======================
 * This class uses Perlin noise & Moore’s Neighbours to be able to generate a base map for the player to play in.
 * Initially, the script creates a grid and assigns values between 0-4 (different TileBase) followed by rendering the
 * Tiles according to the grid values. Generation is hash-seeded.
 * ======================
 */
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = System.Random;

public class MapGeneration : MonoBehaviour {
    [Range(0, 256)] public int width, height;
    [Range(0, 100)] public float smoothness;
    [Range(0, 100)] public int randomFillPercent;
    [Range(0, 100)] public int resourceFillPercent;
    [Range(0, 100)] public int specialFillPercent;

    public string seed;
    public bool useRandomSeed;

    public Tilemap tilemap;
    public TileBase dirt_tile, stone_tile, resource_tile, special_tile, ice_tile;
    private bool imVein;
    public int[,] map;
    private int[,] map2;
    private int perlinH;
    private int[,] perlinMap;
    private int[,] perlinMap2;

    private void OnDrawGizmos() {
        //Testing to visualise map
        if (map != null)
            for (var i = 0; i < width; i++)
            for (var j = 0; j < height; j++) {
                Gizmos.color = map[i, j] == 1 ? Color.black : Color.white;
                var pos = new Vector3(-width / 2 + i + 0.5f, 0, -height / 2 + j + 0.5f);
                Gizmos.DrawCube(pos, Vector3.one);
            }
    } 

    public void GenerateMap() {
        map = new int[width, height];
        map2 = new int[width, height];
        perlinMap = new int[width, height];
        perlinMap2 = new int[width, height];
        imVein = false;
        RandomFillMap();
        for (var i = 0; i < 5; i++) SmoothMap();
        imVein = true;
        ResourceVeins();
        IceVeins();
        SpecialVeins();
        SmoothVeins();
        RenderMap(map, tilemap, dirt_tile, stone_tile, resource_tile, special_tile, ice_tile);
    }

    public void setSeed(string sentSeed) {
        seed = sentSeed;
    }

    private void RandomFillMap() {
        if (useRandomSeed) seed = Time.time.ToString();
        var prng = new Random(seed.GetHashCode());
        for (var i = 0; i < width; i++)
        for (var j = 0; j < height; j++) {
            perlinH = Mathf.RoundToInt(Mathf.PerlinNoise(i / smoothness, prng.Next(0, 100)));
            if (i == 0 || i == width - 1 || j == 0 || j == height - 1) {
                map[i, j] = 1;
                perlinMap[i, j] = 0;
                perlinMap2[i, j] = 0;
            }
            else {
                var value = prng.Next(0, 100);
                var value2 = prng.Next(0, 100);
                map[i, j] = value < randomFillPercent ? 1 : 0;
                perlinMap[i, j] = value < resourceFillPercent && prng.Next(0, 1) == perlinH ? 1 : 0;
                perlinMap2[i, j] = value2 < specialFillPercent && prng.Next(0, 1) == perlinH ? 1 : 0;
            }
        }
        map2 = map;
    }

    private void SmoothMap() {
        for (var i = 0; i < width; i++)
        for (var j = 0; j < height; j++) {
            var neighbourWallTiles = MooreNeighbours(i, j);
            if (neighbourWallTiles > 4)
                map2[i, j] = 1;
            else if (neighbourWallTiles < 4) map2[i, j] = 0;
        }

        map = map2;
    }

    public void ResourceVeins() {
        for (var x = 0; x < width; x++)
        for (var y = 0; y < height; y++)
            if (perlinMap[x, y] == 1 && map[x, y] == 1)
                map2[x, y] = 2;

        map = map2;
    }

    public void IceVeins() {
        for (var x = 0; x < width; x++)
        for (var y = 0; y < height; y++)
            if (perlinMap[x, y] == 1 && map[x, y] == 0)
                map2[x, y] = 3;

        map = map2;
    }

    public void SpecialVeins() {
        for (var x = 0; x < width; x++)
        for (var y = 0; y < height; y++)
            if (perlinMap2[x, y] == 1 && map[x, y] == 1)
                map2[x, y] = 4;
        map = map2;
    }

    private void SmoothVeins() {
        for (var i = 0; i < width; i++)
        for (var j = 0; j < height; j++) {
            if (map[i, j] == 2) {
                var neighbourVeinTiles = MooreNeighbours(i, j);
                if (neighbourVeinTiles < 2)
                    map2[i, j] = 1;
                else if (neighbourVeinTiles > 2) map2[i, j] = 2;
            }

            if (map[i, j] == 3) {
                var neighbourVeinTiles = MooreNeighbours(i, j);
                if (neighbourVeinTiles < 2)
                    map2[i, j] = 0;
                else if (neighbourVeinTiles > 2) map2[i, j] = 3;
            }

            if (map[i, j] == 4) {
                var neighbourVeinTiles = MooreNeighbours(i, j);
                if (neighbourVeinTiles < 3)
                    map2[i, j] = 1;
                else if (neighbourVeinTiles > 3) map2[i, j] = 4;
            }
        }

        map = map2;
    }

    private int MooreNeighbours(int gridX, int gridY) {
        var wallCount = 0;
        var oreCount = 0;
        for (var neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
        for (var neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height) {
                if (neighbourX != gridX || neighbourY != gridY) {
                    if (!imVein && map[neighbourX, neighbourY] <2 )
                        //Check if 1 or 0 for dirt or stone
                        wallCount += map[neighbourX, neighbourY];
                    else if (map[neighbourX, neighbourY] >= 2) oreCount += 1;
                }
            }
            else {
                wallCount++;
            }
        if (imVein) return oreCount;
        return wallCount;
    }

    public void RenderMap(int[,] map, Tilemap tilemap, TileBase dirt_tile, TileBase stone_tile, TileBase resource_tile,
        TileBase special_tile, TileBase ice_tile) {
        //Render tilemap
        tilemap.ClearAllTiles();
        var prng = new Random(seed.GetHashCode());
        var angle = 90f;
        for (var i = 0; i < width; i++)
        for (var j = 0; j < height; j++) {
            if (map[i, j] == 0) tilemap.SetTile(new Vector3Int(i, j, 0), dirt_tile);

            if (map[i, j] == 1) tilemap.SetTile(new Vector3Int(i, j, 0), stone_tile);

            if (map[i, j] == 2) tilemap.SetTile(new Vector3Int(i, j, 0), resource_tile);

            if (map[i, j] == 3) tilemap.SetTile(new Vector3Int(i, j, 0), ice_tile);

            if (map[i, j] == 4) tilemap.SetTile(new Vector3Int(i, j, 0), special_tile);

            var index = prng.Next(4);
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
            tilemap.SetTransformMatrix(new Vector3Int(i, j, 0), matrix);
        }
    }
}