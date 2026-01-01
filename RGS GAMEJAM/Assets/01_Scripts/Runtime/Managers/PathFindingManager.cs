using System.Collections.Generic;
using UnityEngine;

public class PathFindingManager : MonoBehaviour
{
    private static readonly Vector2Int[] directions =
    {
        new Vector2Int( 1,  0),
        new Vector2Int(-1,  0),
        new Vector2Int( 0,  1),
        new Vector2Int( 0, -1),
        new Vector2Int( 1,  1),
        new Vector2Int(-1,  1),
        new Vector2Int( 1, -1),
        new Vector2Int(-1, -1)
    };
    private PathNode[,] pathNodeList = new PathNode[48, 48];
    private List<PathNode> debugPath;

    private void Start()
    {
        for (int i = 0; i < 48; i++)
        {
            for (int j = 0; j < 48; j++)
            {
                pathNodeList[i, j] = new PathNode(i, j, true, 1);
            }
        }
    }
    public void AddWall(int x, int y)
    {
        pathNodeList[x, y] = new PathNode(x, y, false, 0);
    }
    public void AddTowerNode(int x, int y, int weight)
    {
        pathNodeList[x, y] = new PathNode(x, y, true, weight);
    }
    public void RemoveNode(int x, int y)
    {
        pathNodeList[x, y] = new PathNode(x, y, true, 1);
    }
    private int GetHeuristic(PathNode a, PathNode b)
    {
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);

        int distance = Mathf.Max(dx, dy);
        return distance;
    }

    public List<Vector2Int> FindPath(Vector2Int start, Vector2Int end)
    {
        Debug.Log(start+" "+end);
        if (start.x < 0 || end.x < 0 || start.x >= 48 || end.x >= 48)
        {
            return null;
        }
        if (start.y < 0 || end.y < 0 || start.y >= 48 || end.y >= 48)
        {
            return null;
        }
        PathNode startNode = pathNodeList[start.x, start.y];
        PathNode endNode = pathNodeList[end.x, end.y];

        PathNodeHeap openSet = new PathNodeHeap(48 * 48);
        HashSet<PathNode> closedSet = new HashSet<PathNode>();

        for (int i = 0; i < 48; i++)
        {
            for (int j = 0; j < 48; j++)
            {
                pathNodeList[i, j].gCost = int.MaxValue;
                pathNodeList[i, j].parent = null;
            }
        }

        startNode.gCost = 0;
        startNode.hCost = GetHeuristic(startNode, endNode);

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            PathNode current = openSet.RemoveFirst();
            closedSet.Add(current);

            if (current == endNode)
                return RetracePath(startNode, endNode);

            foreach (Vector2Int dir in directions)
            {
                int nx = current.x + dir.x;
                int ny = current.y + dir.y;

                if (nx < 0 || ny < 0 || nx >= 48 || ny >= 48)
                    continue;

                PathNode neighbor = pathNodeList[nx, ny];

                if (!neighbor.walkable || closedSet.Contains(neighbor))
                    continue;

                if (dir.x != 0 && dir.y != 0)
                {
                    PathNode sideA = pathNodeList[current.x + dir.x, current.y];
                    PathNode sideB = pathNodeList[current.x, current.y + dir.y];

                    if (!sideA.walkable || !sideB.walkable)
                        continue;
                    if (current.weight != 1 || neighbor.weight != 1 || sideA.weight != 1 || sideB.weight != 1)
                        continue;
                }

                int newCost = current.gCost + neighbor.weight;

                if (newCost < neighbor.gCost)
                {
                    neighbor.gCost = newCost;
                    neighbor.hCost = GetHeuristic(neighbor, endNode);
                    neighbor.parent = current;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                    else
                        openSet.UpdateItem(neighbor);
                }
            }
        }

        return null;
    }
    private List<Vector2Int> RetracePath(PathNode start, PathNode end)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        PathNode current = end;

        while (current != start)
        {
            path.Add(new Vector2Int(current.x, current.y));
            current = current.parent;
        }

        path.Reverse();
        return path;
    }
    public void SetDebugPath(List<Vector2Int> path)
    {
        if (path == null)
        {
            debugPath = null;
            return;
        }

        debugPath = new List<PathNode>();
        foreach (var v in path)
        {
            debugPath.Add(pathNodeList[v.x, v.y]);
        }
    }

    private void OnDrawGizmos()
    {
        if (debugPath == null) return;

        Gizmos.color = Color.red;
        foreach (var node in debugPath)
        {
            Vector3 worldPos = GameManager.Instance.gridRenderer.ToCartesian2D(node.x-24, node.y-24);
            worldPos.y -= 0.25f;
            Gizmos.DrawSphere(worldPos, 0.1f);
        }

        for (int i = 0; i < debugPath.Count - 1; i++)
        {
            Vector3 a = GameManager.Instance.gridRenderer.ToCartesian2D(debugPath[i].x-24, debugPath[i].y-24);
            Vector3 b = GameManager.Instance.gridRenderer.ToCartesian2D(debugPath[i + 1].x - 24, debugPath[i + 1].y - 24);
            a.y -= 0.25f;b.y -= 0.25f;
            Gizmos.DrawLine(a, b);
        }
    }

}
