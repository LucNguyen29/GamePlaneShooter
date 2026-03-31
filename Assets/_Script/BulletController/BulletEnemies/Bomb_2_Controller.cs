using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb_2_Controller : BulletController
{
    [SerializeField] private float targetY = -2f; // vị trí sẽ nổ
    private float explosionRadius = 0.5f;
    [SerializeField] private float expandDuration = 0.3f;
    // Start is called before the first frame update
    private CircleCollider2D col;
    private bool hasExploded = false;

    void Start()
    {
        col = GetComponent<CircleCollider2D>();
        anim = GetComponent<Animator>();
        col.isTrigger = true; // để bắt va chạm không vật lý
    }

    void Update()
    {
        if (!hasExploded && transform.position.y <= targetY)
        {
            Explode();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasExploded && other.CompareTag("Plane")) // tag máy bay là "Player"
        {
            Explode();
            other.GetComponent<PlayerController>().TakenDamaged(bullet.damage);
        }
    }

    void Explode()
    {
        hasExploded = true;
        StartCoroutine(ExpandCollider());
        anim.Play("Bomb_2_Explotion");
    }

    IEnumerator ExpandCollider()
    {
        float timer = 0f;
        float startRadius = col.radius;

        while (timer < expandDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / expandDuration;
            col.radius = Mathf.Lerp(startRadius, explosionRadius, progress);
            yield return null;
        }

        col.radius = explosionRadius;

        yield return new WaitForSeconds(0.2f); // cho collider tồn tại 1 chút
        Destroy(gameObject); // huỷ bom
    }
}
