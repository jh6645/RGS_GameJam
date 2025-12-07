using UnityEngine;

[CreateAssetMenu(fileName = "SO_EnemyDataBase", menuName = "Scriptable Objects/SO_EnemyDataBase")]
public class SO_EnemyDataBase : ScriptableObject
{
    public EnemyPrefabEntry[] enemies;

    public GameObject GetPrefab(EnemyType type)
    {
        for (int i = 0; i < enemies.Length; i++)
            if (enemies[i].type == type) return enemies[i].prefab;
        Debug.LogError($"No prefab for {type}");
        return null;
    }
}
