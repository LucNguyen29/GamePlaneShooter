using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile_1_Controller : BulletController
{
    private Transform player;
    private float trackingDistance = 4f;
    private Vector3 initialDirection;
    private Vector3 moveDirection;
    private float rotateSpeed = 180.0f;
    private bool isTracking = false;
    private bool isCollider = false;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Plane")?.transform;
        transform.rotation = Quaternion.Euler(0, 0, 180);

        if (player != null)
        {
            initialDirection = (player.transform.position - transform.position).normalized;
            moveDirection = initialDirection;
        }

        StartCoroutine(DestroyBulletHaveAnim(2f,"Missile_Explosion"));
    }

    // Update is called once per frame
    void Update()
    {
        RushToPlayer();
    }

    void RushToPlayer()
    {
        if (player == null) return;

        if (!isCollider)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (!isTracking && distanceToPlayer <= trackingDistance)
            {
                isTracking = true;
            }

            Vector3 targetDirection;

            if (isTracking)
            {
                // Hướng bay sẽ cong dần về phía player
                targetDirection = (player.position - transform.position).normalized;

                // Xoay mượt về target
                moveDirection = Vector3.RotateTowards(
                    moveDirection,
                    targetDirection,
                    rotateSpeed * Mathf.Deg2Rad * Time.deltaTime,
                    0f
                );

                // Xoay đầu đạn theo hướng bay
                float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }

            // Move (luôn di chuyển theo moveDirection)
            Move(moveDirection);
        }
        else
        {
            anim.Play("Missile_Explosion");
            Destroy(gameObject, 0.6f);
        }
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Plane"))
        {
            Debug.Log("collider");
            isCollider = true;
            collider.GetComponent<PlayerController>().TakenDamaged(bullet.damage);
        }
    }
}
