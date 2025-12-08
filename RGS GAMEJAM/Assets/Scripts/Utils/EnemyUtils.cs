using UnityEngine;

public enum EnemyType
{
    NormalPerson,
}
[System.Serializable]
public enum EnemyWaveDirection
{
    None = 0,
    N = 1 << 0,
    NE = 1 << 1,
    E = 1 << 2,
    SE = 1 << 3,
    S = 1 << 4,
    SW = 1 << 5,
    W = 1 << 6,
    NW = 1 << 7
}

[System.Serializable]
public struct EnemyData
{
    public EnemyType enemyType;
    public float percent;
}

