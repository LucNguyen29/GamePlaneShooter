using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletLaserSpinControler : BulletController
{
    private float radius = 360f;
    // Start is called before the first frame update
    private Rigidbody2D rb;
    void Start()
    {
        DestroyBullet();
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.up * bullet.speed;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, radius * Time.deltaTime);
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
