/* ds18635 2101128
 * ======================
 * This class handles displaying an overlay for the player to indicate if the task is allowed or not (green or red)
 * during edit mode or mining mode. Further sets a Boolean to indicate to the PlayerTaskHandler whether the task is
 * legal or not.
 * ======================
 */
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EditHandler : MonoBehaviour {
    public TileBase tile;
    public Tilemap tilemap;
    public bool Editactive;
    public bool Mineactive;
    public MapGeneration mapGeneration;
    public GameObject topGeneration;
    private int[,] basemap;
    private int height;
    private Vector3 preV;
    private int valueCheck;
    private int width;
    public bool validAction;
    private List<int> illegalMine;

    public void Start() {
        Editactive = false;
        Mineactive = false;
        width = mapGeneration.width;
        height = mapGeneration.height;
        basemap = new int[width, height];
        illegalMine = new List<int>();
        illegalMine.Add(0);
        illegalMine.Add(11);
        illegalMine.Add(12);
        illegalMine.Add(13);
        illegalMine.Add(14);
        illegalMine.Add(15);
        illegalMine.Add(16);
        illegalMine.Add(17);
        illegalMine.Add(18);
        illegalMine.Add(21);
        illegalMine.Add(22);
        illegalMine.Add(23);
        illegalMine.Add(31);
        illegalMine.Add(32);
        illegalMine.Add(33);
        illegalMine.Add(41);
        illegalMine.Add(42);
        illegalMine.Add(43);
    } 

    private void Update() {
        if (Editactive) {
            RenderEditLocation(tile);
        }

        if (Mineactive) {
            RenderMineLocation(tile);
        }
    }

    public void CurrentTile(Vector3 v, int tileType) {
        if (v != preV) {
            basemap[(int) preV.x, (int) preV.y] = 0;
            preV = v;
        }

        basemap[(int) v.x, (int) v.y] = 1;
    }

    private void RenderEditLocation(TileBase tile) {
        tilemap.ClearAllTiles();
        for (var i = 0; i < width; i++) {
            for (var j = 0; j < height; j++) {
                if (basemap[i, j] == 1) {
                    tilemap.SetTile(new Vector3Int(i, j, 0), tile);
                    if (topGeneration.GetComponent<TopographyGeneration>().basemap[i, j] != 0) {
                        tilemap.SetTileFlags(new Vector3Int(i, j, 0), TileFlags.None);
                        tilemap.SetColor(new Vector3Int(i, j, 0), Color.red);
                        validAction = false;
                    }
                    else {
                        tilemap.SetTileFlags(new Vector3Int(i, j, 0), TileFlags.None);
                        tilemap.SetColor(new Vector3Int(i, j, 0), Color.green);
                        validAction = true;
                        if ((i == 32 && j == 32 || i == 31 && j == 33 || i == 32 && j == 33 || i == 33 && j == 33 ||
                              i == 31 && j == 32 || i == 33 && j == 32 || i == 31 && j == 31 || i == 32 && j == 31 ||
                              i == 33 && j == 31)) {
                            tilemap.SetTileFlags(new Vector3Int(i, j, 0), TileFlags.None);
                            tilemap.SetColor(new Vector3Int(i, j, 0), Color.red);
                            validAction = false;
                        }
                    }
                }
            }
        }
    }

    private void RenderMineLocation(TileBase tile) {
        tilemap.ClearAllTiles();
        for (var i = 0; i < width; i++) {
            for (var j = 0; j < height; j++) {
                if (basemap[i, j] == 1) {
                    tilemap.SetTile(new Vector3Int(i, j, 0), tile);
                    if (!illegalMine.Contains(topGeneration.GetComponent<TopographyGeneration>().topography[i, j])) {
                        tilemap.SetTileFlags(new Vector3Int(i, j, 0), TileFlags.None);
                        tilemap.SetColor(new Vector3Int(i, j, 0), Color.green);
                        validAction = true;
                    }
                    else {
                        tilemap.SetTileFlags(new Vector3Int(i, j, 0), TileFlags.None);
                        tilemap.SetColor(new Vector3Int(i, j, 0), Color.red);
                        validAction = false;
                    }
                }
            }
        }
    }

    public void ExitMode() {
        for (var i = 0; i < width; i++)
        for (var j = 0; j < height; j++)
            basemap[i, j] = 0;
        tilemap.ClearAllTiles();
    }
}