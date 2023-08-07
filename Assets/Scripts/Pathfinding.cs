/* ds18635 2101128
 * ======================
 * This class takes 2 positions (startPos & endPos) and finds a valid path by using an A* search.
 * The colonist receives a list of PathNodes to follow until they reach the final node in the list.
 * ======================
 */
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding {
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14; //Square root of 200 
    private List<PathNode> closedList;

    private readonly GridFrame<PathNode> grid;
    private List<PathNode> openList;

    public Pathfinding(int width, int height) {
        Instance = this;
        //Initialising the grid
        grid = new GridFrame<PathNode>(width, height, 1f, Vector3.zero, (g, x, y) => new PathNode(g, x, y));
    }
    // BUG Note can walk between 2 unwalkables --> Fix known but not feasible in timeframe 
    public static Pathfinding Instance { get; private set; } //Singleton as pathfinding for all colonists are the same

    public GridFrame<PathNode> GetGrid() {
        return grid;
    }
    public List<Vector3> FindPath(Vector3 startPos, Vector3 endPos) {
        //Gets world positions and converts to x,y positions of the grid frame
        grid.GetXY(startPos, out var startX, out var startY);
        grid.GetXY(endPos, out var endX, out var endY);

        var path = FindPath(startX, startY, endX, endY); //Finds a path with grid positions

        if (path == null) //Cannot find a path
            return null;

        var vectorPath = new List<Vector3>();
        foreach (var pathNode in path) //Converts the path nodes to vector3s for the movement to follow
            vectorPath.Add(new Vector3(pathNode.x, pathNode.y) * grid.GetCellSize() +
                           Vector3.one * grid.GetCellSize() * .5f);
        return vectorPath;
    }

    public List<PathNode> FindPath(int startX, int startY, int endX, int endY) {
        //Returns a list of path nodes for the entire path from the start position to end position
        var startNode = grid.GetValue(startX, startY);
        var endNode = grid.GetValue(endX, endY);

        if (startNode == null || endNode == null) // Invalid Path
            return null;

        openList = new List<PathNode> {startNode}; //Nodes to search
        closedList = new List<PathNode>(); //Already searched

        for (var x = 0; x < grid.GetWidth(); x++) //Cycle through the nodes
        for (var y = 0; y < grid.GetHeight(); y++) {
            var pathNode = grid.GetValue(x, y);
            pathNode.gCost = 99999999;
            pathNode.CalculateFCost();
            pathNode.cameFromNode = null;
        }

        //Set start node values
        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();


        while (openList.Count > 0) {
            var currentNode = GetLowestFCostNode(openList);
            if (currentNode == endNode) // Reached final node
                return CalculatePath(endNode);
            //Removes the node that was just search from the open list and adds to the closed list
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (var neighbourNode in GetNeighbourList(currentNode)) {
                //Checks the 8 neighbour positions for possible paths
                if (closedList.Contains(neighbourNode)) continue; //Already searched
                if (!neighbourNode.isWalkable) //Check if it is walkable, if not add to closed list and continue
                {
                    closedList.Add(neighbourNode);
                    continue;
                }
                var tentativeGCost =
                    currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode); //Checks for a better path
                if (tentativeGCost < neighbourNode.gCost) { //If it is better, update the neighbour node values
                    neighbourNode.cameFromNode = currentNode;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                    neighbourNode.CalculateFCost();

                    if (!openList.Contains(neighbourNode)) //Add this node to the open list 
                        openList.Add(neighbourNode);
                }
            }
        }

        // Out of nodes on the openList
        return null;
    }

    private List<PathNode> GetNeighbourList(PathNode currentNode) {
        var neighbourList = new List<PathNode>();

        if (currentNode.x - 1 >= 0) {
            // Left
            neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y));
            // Left Down
            if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y - 1));
            // Left Up
            if (currentNode.y + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y + 1));
        }

        if (currentNode.x + 1 < grid.GetWidth()) {
            // Right
            neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y));
            // Right Down
            if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y - 1));
            // Right Up
            if (currentNode.y + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y + 1));
        }

        // Down
        if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x, currentNode.y - 1));
        // Up
        if (currentNode.y + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x, currentNode.y + 1));

        return neighbourList;
    }

    public PathNode GetNode(int x, int y) {
        return grid.GetValue(x, y);
    }

    private List<PathNode> CalculatePath(PathNode endNode) {
        var path = new List<PathNode>(); //Final path node grid list to be returned
        path.Add(endNode); //Start with end node as we have a reference to the came from node (Work backwards)
        var currentNode = endNode;
        while (currentNode.cameFromNode != null) { //While the current node has a parent 
            path.Add(currentNode.cameFromNode);
            currentNode = currentNode.cameFromNode; //Goes to the next node
        } //Loop is exited when there is no parent, indicating the start node which does not have a came from node (root node)

        path.Reverse(); //Goes from backwards to forwards
        return path;
    }

    private int CalculateDistanceCost(PathNode a, PathNode b) { //Hcost = Distance while ignoring all the walkable areas
        var xDistance = Mathf.Abs(a.x - b.x);
        var yDistance = Mathf.Abs(a.y - b.y);
        var remaining = Mathf.Abs(xDistance - yDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private PathNode GetLowestFCostNode(List<PathNode> pathNodeList) {
        //Cycles through the entire path node list and receive the lowest fCost node.
        var lowestFCostNode = pathNodeList[0];
        for (var i = 1; i < pathNodeList.Count; i++)
            if (pathNodeList[i].fCost < lowestFCostNode.fCost)
                lowestFCostNode = pathNodeList[i];
        return lowestFCostNode;
    }
} 