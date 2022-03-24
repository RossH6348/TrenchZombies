using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Name: Ross Hutchins
//ID: HUT18001284

public class Entity : MonoBehaviour
{

    //How much maximum health can they have?
    [SerializeField] private int MaxHealth = 100;
    public int Health = 100;

    //What item do they have equipped, and what's in their inventory?
    public bool equipItem = false;
    private int currentItem = 1;
    private int InventorySize = 5;
    [SerializeField] private List<GameObject> inventory = new List<GameObject>();


    //Where do they mount the item, as well what part of the mesh to hide when equipping.
    [SerializeField] private List<GameObject> nodes = new List<GameObject>();
    [SerializeField] private Transform mountPoint;

    //If they got an item equip, store its animator component if possible.
    private Animator itemAnimator = null;

    private void OnEnable()
    {
        //Reset health and all.
        Health = MaxHealth;
    }

    //This toggles whether to open their inventory and equip, or unequip and close inventory.
    public void openInventory()
    {
        //Check if they even have anything in inventory to equip with.
        if(!equipItem && inventory.Count > 0)
        {
            equipItem = true;
            currentItem = (currentItem > inventory.Count ? inventory.Count : currentItem);
            mountItem(inventory[currentItem - 1]);
        }
        else
        {
            unmountItem();
            equipItem = false;
        }
    }

    public void addItem(GameObject item)
    {
        inventory.Add(item);
    }

    public void dropItem()
    {
        //Check if they actually equipping an item right now.
        if (equipItem)
        {
            //Unmount the item first to be dropped.
            unmountItem();
            equipItem = false;

            //Now remove the item from their inventory.
            inventory.RemoveAt(currentItem - 1);

            if(inventory.Count < 1)
            {
                //No more items in inventory, might of well forcibly toggle inventory to close.
                openInventory();
            }
        }
    }

    public void scrollItem(int direction)
    {
        //See if they are actually equipping something right now.
        if (equipItem)
        {

            int newCurrentItem = currentItem + direction;
            newCurrentItem = (newCurrentItem > inventory.Count ? inventory.Count : newCurrentItem);

            //Check if it is actually a different item available.
            if(currentItem != newCurrentItem)
            {
                unmountItem();
                currentItem = newCurrentItem;
                mountItem(inventory[currentItem - 1]);
            }
        }
    }


    //Trigger the animator to change pullTrigger to true or false, triggering the firing animation.
    public void setMountTrigger(bool trigger)
    {
        if(mountPoint.childCount > 0)
            itemAnimator.SetBool("pullTrigger", trigger);
    }

    //This function will sort out unequipping an item, and making the nodes visible again.
    public void unmountItem()
    {
        if (mountPoint.childCount > 0)
        {
            GameObject item = mountPoint.GetChild(0).gameObject;
            item.transform.SetParent(null);
            item.SetActive(false);

            foreach (GameObject node in nodes)
                node.SetActive(true);

            itemAnimator = null;
        }
    }

    //This will equip said item given, while hiding nodes.
    public void mountItem(GameObject item)
    {
        //Unequip whatever they may have.
        if (mountPoint.childCount > 0)
            unmountItem();

        //Equip this new possible item.
        item.transform.SetParent(mountPoint);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        item.SetActive(true);

        itemAnimator = item.GetComponent<Animator>();

        foreach (GameObject node in nodes)
            node.SetActive(false);
    }

    public void Heal(int HP)
    {
        Health += HP;
        if (Health > MaxHealth)
            Health = MaxHealth;
    }

    public void Damage(int dmg)
    {
        Health -= dmg;
        if (Health <= 0)
            gameObject.SetActive(false);
    }

}
