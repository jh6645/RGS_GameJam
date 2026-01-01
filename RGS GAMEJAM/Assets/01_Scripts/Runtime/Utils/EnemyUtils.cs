using UnityEngine;

public enum EnemyType
{
    NormalPerson,
}

[System.Serializable]
public enum EnemyState
{
    Chasing_Tree,
    Chasing_Target,
    Chasing_Player,
    Attacking,
}
[System.Serializable]
public struct EnemyData
{
    public EnemyType enemyType;
    public int count;
}

