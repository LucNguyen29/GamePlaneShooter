using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : PlaneController
{
    public EnemyData enemyData;
    public HealthBarController healthBar;
    [SerializeField] private GameObject[] itemPrefabs;
    private bool hasBeenCounted = false;
    protected Animator anim;
    private bool hasDropItem = false;
    protected bool isHandlingOutOfScreen = false;
    private float dropChance = 40.0f;
    protected void SetupHealth(float value)
    {
        enemyData.health = value;
        currentHealth = enemyData.health;
    }

    protected void SetupHealthBoss(float value)
    {
        value = Mathf.Max(0, value);
        health = value;
        currentHealth = value;
        Debug.Log($"Boss health setup: {currentHealth}/{health}");
    }

    public void BossTakenDamage(float damage, string deathAnimName)
    {
        currentHealth -= (int)damage;
        healthBar.UpdateHealthBar(currentHealth, health);
        if (currentHealth <= 0)
        {
            anim.Play(deathAnimName);
            GetComponent<Collider2D>().enabled = false;
            GameController.Instance.BossDie();
            Destroy(gameObject, 1f);
        }
    }

    public void TakenDamaged(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0 && gameObject != null)
        {
            anim.Play("Explosion");
            StartCoroutine(DelayedDisableCollider());
            StartCoroutine(DelayedDestroy());

            DropItem();

        }
    }

    IEnumerator DelayedDisableCollider()
    {
        // Đợi cho đến khi animation kết thúc
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);

        GetComponent<Collider2D>().enabled = false;
    }

    IEnumerator DelayedDestroy()
    {
        yield return new WaitForSeconds(0.5f);
        if (!hasBeenCounted)
        {
            GameController.Instance.EnemyDestroyedByBullet();
            hasBeenCounted = true;
        }
        Destroy(gameObject);
    }

    protected bool IsVisibleOnScreen()
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(transform.position);
        return screenPoint.x > -0.2f && screenPoint.x < 1.2f && screenPoint.y > -0.2f && screenPoint.y < 1.2f;
    }

    void DropItem()
    {
        if (hasDropItem)
            return;
        if (Random.Range(0f, 100f) < dropChance)
        {
            int randomIndex = Random.Range(0, itemPrefabs.Length);
            Instantiate(itemPrefabs[randomIndex], transform.position, Quaternion.identity);
        }

        hasDropItem = true;
    }

    public void SetHealthBar(GameObject bar)
    {
        healthBar = bar.GetComponent<HealthBarController>();

        StartCoroutine(InitHealthBarNextFrame());
    }

    IEnumerator InitHealthBarNextFrame()
    {
        yield return null; // đợi 1 frame
        if (healthBar != null)
        {
            healthBar.UpdateHealthBar(currentHealth, health);
        }
    }
}
