using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    private Dictionary<GameObject, Queue<GameObject>> pools = new();


    public GameObject Get(GameObject originalPrefab)
    {
        if (!pools.ContainsKey(originalPrefab)) pools[originalPrefab] = new Queue<GameObject>();

        var q = pools[originalPrefab];
        if (q.Count > 0)
        {
            var obj = q.Dequeue();
            obj.SetActive(true);
            return obj;
        }

        var go = Instantiate(originalPrefab);
        var p = go.GetComponent<PooledEnemy>();
        if (p != null) p.originalPrefab = originalPrefab;
        return go;
    }

    public void Return(GameObject originalPrefab, GameObject instance)
    {
        instance.SetActive(false);
        if (!pools.ContainsKey(originalPrefab)) pools[originalPrefab] = new Queue<GameObject>();
        pools[originalPrefab].Enqueue(instance);
    }
}
