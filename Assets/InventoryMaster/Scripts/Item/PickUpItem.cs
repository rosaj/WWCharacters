using UnityEngine;
using System.Collections;
public class PickUpItem : Interactable
{
    public Item item;
    private Inventory _inventory;
    // Use this for initialization

    public override void Start()
    {
        base.Start();
        _inventory = PlayerManager.instance.GetComponent<PlayerInventory>().MainInventory;
    }

    public override void Interact()
    {
        base.Interact();
        bool check = _inventory.checkIfItemAllreadyExist(item.itemID, item.itemValue);

        if (check) Destroy(this.gameObject);
        else if (_inventory.ItemsInInventory.Count < (_inventory.width * _inventory.height))
        {
            _inventory.addItemToInventory(item.itemID, item.itemValue);
            _inventory.updateIconSize();
            _inventory.updateItemList();
            _inventory.stackableSettings();
            Destroy(this.gameObject);
            GameManager.NotifyPickedUpItem(item);
        }

    }

    /*
    // Update is called once per frame
    void Update()
    {
        if (_inventory != null && Input.GetKeyDown(KeyCode.E))
        {
            float distance = Vector3.Distance(this.gameObject.transform.position, _player.transform.position);

            if (distance <= 3)
            {
                bool check = _inventory.checkIfItemAllreadyExist(item.itemID, item.itemValue);
                if (check)
                    Destroy(this.gameObject);
                else if (_inventory.ItemsInInventory.Count < (_inventory.width * _inventory.height))
                {
                    _inventory.addItemToInventory(item.itemID, item.itemValue);
                    _inventory.updateItemList();
                    _inventory.stackableSettings();
                    Destroy(this.gameObject);
                }

            }
        }
    }
    */
}