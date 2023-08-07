/* ds18635 2101128
 * ======================
 * This class creates the grid that the Pathfinding class uses as the locations for the Pathfidning nodes.
 * ======================
 */
using System;
using UnityEngine;

public class GridFrame<TGridObject> //Generic for which type to use, in this case PathNode for movement.
{
    private readonly float cellSize;
    private readonly TGridObject[,] gridArray;
    private readonly int height;
    private readonly Vector3 origin;

    private readonly int width;

    public GridFrame(int width, int height, float cellSize, Vector3 origin,
        Func<GridFrame<TGridObject>, int, int, TGridObject> createGridObject) {
        //Func is a delegate that returns a value
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.origin = origin;

        gridArray = new TGridObject[width, height];

        for (var i = 0; i < width; i++) //Initialise the grid to be used
        for (var j = 0; j < height; j++)
            gridArray[i, j] = createGridObject(this, i, j);

        var debugGrid = false;
        if (debugGrid) //Visual for grid objects created
        {
            for (var i = 0; i < width; i++)
            for (var j = 0; j < height; j++) {
                Debug.DrawLine(GetWorldPostion(i, j), GetWorldPostion(i, j + 1), Color.green, 100f);
                Debug.DrawLine(GetWorldPostion(i, j), GetWorldPostion(i + 1, j), Color.green, 100f);
            }

            Debug.DrawLine(GetWorldPostion(0, height), GetWorldPostion(width, height), Color.green, 100f);
            Debug.DrawLine(GetWorldPostion(width, 0), GetWorldPostion(width, height), Color.green, 100f);
        }
    } //Handles the creation of the grid.

    public event EventHandler<OnGridObjectChangedEventArgs> OnGridObjectChanged;

    private Vector3 GetWorldPostion(int x, int y) {
        return new Vector3(x, y) * cellSize + origin;
    }

    public void GetXY(Vector3 worldPosition, out int x, out int y) {
        //Converts the world position to the gridframes x,y position
        x = Mathf.FloorToInt((worldPosition.x - origin.x) / cellSize);
        y = Mathf.FloorToInt((worldPosition.y - origin.y) / cellSize);
    }

    public void SetValue(int x, int y, TGridObject value) {
        if (x >= 0 && y >= 0 && x < width && y < height) {
            gridArray[x, y] = value;
            if (OnGridObjectChanged != null) OnGridObjectChanged(this, new OnGridObjectChangedEventArgs {x = x, y = y});
        }
    }

    public void TriggerGridObjectChanged(int x, int y) { //Checks if there is any change in the grid.
        if (OnGridObjectChanged != null) OnGridObjectChanged(this, new OnGridObjectChangedEventArgs {x = x, y = y});
    }

    public void SetValue(Vector3 worldPosition, TGridObject value) { //Used to set the value for different grid objects 
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetValue(x, y, value);
        Debug.Log(x + ", " + y + " = " + value);
    }

    public TGridObject GetValue(int x, int y) {
        if (x >= 0 && y >= 0 && x < width && y < height)
            return gridArray[x, y];
        return default;
    }

    public TGridObject GetValue(Vector3 worldPosition) {
        int x, y;
        GetXY(worldPosition, out x, out y);
        return GetValue(x, y);
    }

    public int GetWidth() {
        return width;
    }

    public int GetHeight() {
        return height;
    }

    public float GetCellSize() {
        return cellSize;
    }

    public class OnGridObjectChangedEventArgs : EventArgs {
        public int x;
        public int y;
    }
} 