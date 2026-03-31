 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[CreateAssetMenu(fileName = "New Bullet Type", menuName = "Bullet Type")]
public class BulletData : ScriptableObject
{
    public string bulletName;
    public GameObject bulletPrefab;
    public float speed;
    public float damage;
    public float lifeTime;
    public float maxDamage;
}
