using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public GameObject OnEnemyDeathParticle;
    public GameObject EnemyHealthBar;
    public LootTableEntry[] LootTable;

    private static ItemDataBaseList inventoryItems;

    [System.Serializable]
    public class LootTableEntry
    {
        public EnemyController.EnemyType enemyType;
        public int ExperienceFromKilling;
        public LootTableEntryItems[] items;
    }

    [System.Serializable]
    public class LootTableEntryItems
    {
        public int itemId;
        public int count = 1;
    }


    #region Singleton
    public static GameManager instance;

    void Awake()
    {
        instance = this;
        Cursor.visible = false;

        inventoryItems = (ItemDataBaseList)Resources.Load("ItemDatabase");
    }
    #endregion

    public static ProgressBar GetEnemyHealthBarCopy(GameObject enemy)
    {
      
        var hb = Instantiate(instance.EnemyHealthBar, enemy.transform);
        hb.SetActive(true);
        var y = enemy.GetComponent<BoxCollider>().size.y + .2f ;
        hb.GetComponent<RectTransform>().localPosition = new Vector3(0, y, 0);
        return hb.GetComponentInChildren<ProgressBar>();
    }


    public static void NotifyPickedUpItem(Item item)
    {
        //TODO: item picked up
    }

    public static void EnemyDied(Enemy enemy)
    {
        foreach(var entry in instance.LootTable){
            if(entry.enemyType == enemy.EnemyController.enemyType)
            {
                PlayerManager.GainExperience(entry.ExperienceFromKilling);

                float offset = 0;
                foreach (var item in entry.items)
                {
                    var inventoryItem = inventoryItems.getItemByID(item.itemId);
                    var drop = Instantiate(inventoryItem.itemModel);
                    if (!drop.activeSelf) drop.SetActive(true);
                    drop.transform.position = new Vector3(  enemy.transform.position.x + offset++, 
                                                            drop.transform.position.y, 
                                                                enemy.transform.position.z);

                    var pickUpItemScript = drop.GetComponent<PickUpItem>();
                    if (!pickUpItemScript)
                        pickUpItemScript = drop.AddComponent<PickUpItem>();
                    
                    pickUpItemScript.item = inventoryItem;
                    pickUpItemScript.item.itemValue = item.count;

                    pickUpItemScript.Start();
                }


                break;
            }
        }
       
    }
}
