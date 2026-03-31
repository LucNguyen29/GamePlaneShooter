using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BulletChangeItem : ItemController
{
    public int newBulletIndex;

    public override void ApplyEffect(PlayerController player)
    {
        if(player.bulletCount < 5)
        {
            player.bulletCount++;
        }
        player.ChangeBulletType(newBulletIndex);
        Debug.Log($"🔄 Changed to bullet index: {newBulletIndex} and bullet count: {player.bulletCount}");

        if(player.bulletCount >= 5)
        {
            if(player.lastItemType == newBulletIndex.ToString())
            {
                player.sameItemCount++;
                Debug.Log("Player entering state overlocking");
            }
            else
            {
                player.sameItemCount = 1;
                player.lastItemType = newBulletIndex.ToString();
            }

            Debug.Log("lastItem: " + player.sameItemCount);
        }

        Destroy(gameObject);
    }
}
