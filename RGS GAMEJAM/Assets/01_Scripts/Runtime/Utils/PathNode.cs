using UnityEngine;

public class PathNode
{
    public int x;
    public int y;

    public bool walkable;
    public int weight;

    public int gCost;
    public int hCost;
    public int fCost => gCost + hCost;

    public int heapIndex;

    public PathNode parent;

    public PathNode(int x, int y, bool walkable, int weight)
    {
        this.x = x;
        this.y = y;
        this.walkable = walkable;
        this.weight = weight;
    }
}

