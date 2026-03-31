using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public BulletData bullet;
    protected Animator anim;
    protected virtual void Move(Vector3 direction)
    {
        transform.position += direction * bullet.speed * Time.deltaTime;
    }

    protected void DestroyBullet()
    {
        Destroy(gameObject, bullet.lifeTime);
    }

    protected IEnumerator DestroyBulletHaveAnim(float time, string explosion)
    {
        yield return new WaitForSeconds(time);

        anim.Play(explosion);

        yield return new WaitForSeconds(0.5f);

        Destroy(gameObject);
    }

}
