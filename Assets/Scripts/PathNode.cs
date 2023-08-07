/* ds18635 2101128
 * ======================
 * This class is used for each object in the grid Pathfinding uses, holding all the information for that specific object
 * in the grid such as if it is walkable or not.
 * ======================
 */
public class PathNode {
    public PathNode cameFromNode;
    public int fCost; // Cost remaining

    public int gCost; // Move cost

    //Type used for each object in the grid frame holding all the information for that specific object in the grid
    private readonly GridFrame<PathNode> gridFrame;
    public int hCost; // Assumption to reach end

    public bool isWalkable;
    public int x;
    public int y;

    public PathNode(GridFrame<PathNode> gridFrame, int x, int y) {
        isWalkable = false;
        this.gridFrame = gridFrame;
        this.x = x;
        this.y = y;
    }

    public void CalculateFCost() {
        fCost = gCost + hCost;
    }

    public void SetIsWalkable(bool isWalkable) {
        this.isWalkable = isWalkable;
        gridFrame.TriggerGridObjectChanged(x, y);
    }

    public override string ToString() {
        return x + "," + y;
    }
} 