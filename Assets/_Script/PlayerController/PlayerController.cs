using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class PlayerController : PlaneController
{
    public Joystick joystick;
    public BulletData[] bullet;
    public GameObject shiled;
    public int currentIndexBullet = 0;
    [SerializeField] private Transform attackPosition;
    private float overclockingTime = 5.0f;
    [SerializeField] private float coolDownTimer;
    private float fireRate = 0.5f;
    private BulletData[] bulletRuntime;
    private float Speed = 20;
    private Rigidbody2D rb;
    private Animator anim;
    private float moveHorizontal;
    private float moveVertical;
    public float minX;
    public float maxX;
    public float minY, maxY;
    public int bulletCount;
    public int sameItemCount;
    public string lastItemType = "";
    public bool isDemo = false;
    public bool isInvincible = false;
    private bool isOverClocking = false;
    private bool followMouse = true;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        health = 10f;
        currentHealth = health;
        bulletCount = 1;

        bulletRuntime = new BulletData[bullet.Length];
        for (int i = 0; i < bullet.Length; i++)
        {
            bulletRuntime[i] = ScriptableObject.CreateInstance<BulletData>();
            bulletRuntime[i].damage = bullet[i].damage;
            bulletRuntime[i].speed = bullet[i].speed;
            bulletRuntime[i].lifeTime = bullet[i].lifeTime;
            bulletRuntime[i].bulletPrefab = bullet[i].bulletPrefab;
            bulletRuntime[i].maxDamage = bullet[i].maxDamage;
        }

        bulletRuntime[currentIndexBullet].speed =
        sameItemCount = 0;
        if (!isDemo)
        {
            StartCoroutine(ShootRoutine());
        }


    }

    void Update()
    {
        MoveByJoystick();

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) ||
            Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            MoveByKeyBoard();
        }
    }

    void MoveByKeyBoard()
    {
        moveHorizontal = Input.GetAxis("Horizontal");
        moveVertical = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(moveHorizontal, moveVertical);

        transform.position += direction * Speed * Time.deltaTime;
    }

    void MoveByJoystick()
    {
        moveHorizontal = joystick.Horizontal;
        moveVertical = joystick.Vertical;

        Vector2 movement = new Vector2(moveHorizontal, moveVertical) * Speed * Time.deltaTime;

        transform.position = new Vector3(
                        Mathf.Clamp(transform.position.x, minX, maxX),
                        Mathf.Clamp(transform.position.y, minY, maxY),
                        transform.position.z
                        );

        rb.MovePosition(rb.position + movement);
    }

    void MoveByMouse()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Mathf.Abs(Camera.main.transform.position.z);
        Vector3 direction = (transform.position - Camera.main.ScreenToWorldPoint(mousePos)).normalized;

        transform.position += direction * Time.deltaTime * Speed;

        if (Vector3.Distance(transform.position, Camera.main.ScreenToWorldPoint(mousePos)) > 0.1f)
        {
            transform.position = Camera.main.ScreenToWorldPoint(mousePos);
        }

        transform.position = new Vector3(
                        Mathf.Clamp(transform.position.x, minX, maxX),
                        Mathf.Clamp(transform.position.y, minY, maxY),
                        transform.position.z
                        );
    }

    IEnumerator ShootRoutine()
    {
        while (true)
        {
            BulletData bullet = bulletRuntime[currentIndexBullet];

            CreateBullet();
            float baseSpeed = bullet.speed;

            if (sameItemCount >= 2 && !isOverClocking)
            {
                fireRate = 0.2f;
                bullet.speed *= 1.5f;
                isOverClocking = true;
                StartCoroutine(ResetOverClocking(bullet, baseSpeed));
            }

            yield return new WaitForSeconds(fireRate);
        }
    }

    IEnumerator ResetOverClocking(BulletData bullet, float baseSpeed)
    {
        yield return new WaitForSeconds(overclockingTime);
        sameItemCount = 0;
        bullet.speed = baseSpeed;
        fireRate = 0.5f;

        isOverClocking = false;
    }

    public void CreateBullet()
    {
        GameObject bulletObj = Instantiate(bulletRuntime[currentIndexBullet].bulletPrefab, attackPosition.position, transform.rotation);
        BulletPlayerController bulletScript = bulletObj.GetComponent<BulletPlayerController>();
        if (bulletScript != null)
        {
            bulletScript.InitFromData(bulletRuntime[currentIndexBullet]);
        }
    }

    public void ChangeBulletType(int newBulletIndex)
    {
        if (newBulletIndex >= 0 && newBulletIndex < bullet.Length)
        {
            currentIndexBullet = newBulletIndex;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !isInvincible)
        {
            anim.Play("Die");
            GetComponent<Collider2D>().enabled = false;
            Destroy(gameObject, 1f);

            GameController.Instance.PlayerDie();

            GameController.Instance.StartCoroutine(GameController.Instance.DeplayRevived());
        }

        if (collision.CompareTag("Item"))
        {
            ItemController item = collision.GetComponent<ItemController>();
            if (item != null)
            {
                item.ApplyEffect(this);
            }

            for (int i = 0; i < bulletRuntime.Length; i++)
            {
                bulletRuntime[i].damage *= 1.5f;

                if (bulletCount == 5)
                {
                    break;
                }
            }
        }
    }


    public void TakenDamaged(float damage)
    {
        if (!isInvincible)
        {
            currentHealth -= (int)damage;
            if (currentHealth <= 0)
            {
                anim.Play("Die");
                GetComponent<Collider2D>().enabled = false;
                Destroy(gameObject, 1f);

                GameController.Instance.PlayerDie();

                GameController.Instance.StartCoroutine(GameController.Instance.DeplayRevived());
            }
        }
        else
        {
            return;
        }
    }

}
