using UnityEngine;

public enum EnemyType
{
    NormalPerson,

}

[System.Serializable]
public struct EnemyData
{
    public EnemyType enemyType;
    public float percent;
}

[System.Serializable]
public struct EnemyPrefabEntry
{
    public EnemyType type;
    public GameObject prefab;
}
