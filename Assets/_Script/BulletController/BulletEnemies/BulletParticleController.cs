using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletParticleController : BulletController
{
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

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.CompareTag("Plane"))
        {
            collider.GetComponent<PlayerController>().TakenDamaged(bullet.damage);
            Destroy(gameObject);
        }
    }
}
