public class PathNodeHeap
{
    private PathNode[] items;
    private int count;

    public int Count => count;

    public PathNodeHeap(int maxSize)
    {
        items = new PathNode[maxSize];
    }

    public void Add(PathNode node)
    {
        node.heapIndex = count;
        items[count] = node;
        SortUp(node);
        count++;
    }

    public PathNode RemoveFirst()
    {
        PathNode first = items[0];
        count--;

        items[0] = items[count];
        items[0].heapIndex = 0;
        SortDown(items[0]);

        return first;
    }

    public void UpdateItem(PathNode node)
    {
        SortUp(node);
    }

    public bool Contains(PathNode node)
    {
        return items[node.heapIndex] == node;
    }

    private void SortDown(PathNode node)
    {
        while (true)
        {
            int left = node.heapIndex * 2 + 1;
            int right = node.heapIndex * 2 + 2;
            int swapIndex = 0;

            if (left < count)
            {
                swapIndex = left;

                if (right < count &&
                    items[left].fCost > items[right].fCost)
                {
                    swapIndex = right;
                }

                if (node.fCost > items[swapIndex].fCost)
                {
                    Swap(node, items[swapIndex]);
                }
                else
                    return;
            }
            else
                return;
        }
    }

    private void SortUp(PathNode node)
    {
        int parentIndex = (node.heapIndex - 1) / 2;

        while (true)
        {
            PathNode parent = items[parentIndex];
            if (node.fCost < parent.fCost)
            {
                Swap(node, parent);
            }
            else
                break;

            parentIndex = (node.heapIndex - 1) / 2;
        }
    }

    private void Swap(PathNode a, PathNode b)
    {
        items[a.heapIndex] = b;
        items[b.heapIndex] = a;

        int temp = a.heapIndex;
        a.heapIndex = b.heapIndex;
        b.heapIndex = temp;
    }
}
