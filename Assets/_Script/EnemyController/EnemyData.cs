using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Bullet Type", menuName = "Enemy Type")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public GameObject enemyPrefabs;
    public float speed;
    public float health;
}
