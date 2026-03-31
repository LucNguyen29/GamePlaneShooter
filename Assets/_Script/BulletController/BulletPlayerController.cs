using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class BulletPlayerController : BulletController
{
    [SerializeField] private GameObject hit;
    // Start is called before the first frame update
    void Start()
    {
        DestroyBullet();
    }

    // Update is called once per frame
    void Update()
    {
        Move(transform.up);
    }

    public void InitFromData(BulletData bulletData)
    {
        bullet = bulletData;
    }

    void CreateHit(Vector3 position)
    {
        GameObject vfx = Instantiate(hit, position , Quaternion.identity);
        Destroy(vfx, 0.3f); // Hủy bản instance sau 0.5s
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Enemy"))
        {
            collider.GetComponent<EnemyController>().TakenDamaged(bullet.damage);
            CreateHit(collider.transform.position);
            Destroy(gameObject);
        }
        else if (collider.CompareTag("Boss"))
        {
            collider.GetComponent<EnemyController>().BossTakenDamage(bullet.damage, "Boss_Explosion");
            CreateHit(collider.transform.position);
            Destroy(gameObject);
        }
    }
}
