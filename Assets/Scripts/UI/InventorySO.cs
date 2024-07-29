using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class InventorySO : ScriptableObject
{
    [SerializeField]
    private List<InventoryItemStruct> inventoryItems;

    [field: SerializeField]
    public int Size { get; private set; } = 10;

    public event Action<Dictionary<int, InventoryItemStruct>> OnInventoryUpdated;

    public void Initialize()
    {
        inventoryItems = new List<InventoryItemStruct>();
        for (int i = 0; i < Size; i++)
        {
            inventoryItems.Add(InventoryItemStruct.GetEmptyItem());
        }
    }

    public void AddItem(BlockInfo item, int quantity)
    {
        for (int i = 0; i < inventoryItems.Count; i++)
        {

            if (inventoryItems[i].IsEmpty)
            {

                inventoryItems[i] = new InventoryItemStruct
                {
                    item = item,
                    quantity = quantity,
                };
                InformAboutChange();
                return;
            }
            else
            {
                if (inventoryItems[i].item.ID == item.ID)
                {
                    AddStackableItem(item, quantity);
                    InformAboutChange();
                    return;
                }
            }

        }
        AddStackableItem(item, quantity);
        InformAboutChange();
        return;
    }




    private int AddStackableItem(BlockInfo item, int quantity)
    {
        for(int i = 0; i < inventoryItems.Count; i++)
        {
            if (inventoryItems[i].IsEmpty)
                continue;
            if (inventoryItems[i].item.ID == item.ID)
            {
                inventoryItems[i] = inventoryItems[i].ChangeQuantity(inventoryItems[i].quantity + quantity);
                InformAboutChange();
                return 0;
            }
        }
        return quantity;
    }

    public void AddItem(InventoryItemStruct item)
    {
        AddItem(item.item, item.quantity);
    }

    private void InformAboutChange()
    {
        OnInventoryUpdated?.Invoke(GetCurrentInventoryState());
    }

    public void RemoveItem(int itemIndex, int amount)
    {
        if (inventoryItems.Count > itemIndex)
        {
            if (inventoryItems[itemIndex].IsEmpty)
                return;
            int reminder = inventoryItems[itemIndex].quantity - amount;
            if (reminder <= 0)
                inventoryItems[itemIndex] = InventoryItemStruct.GetEmptyItem();
            else
                inventoryItems[itemIndex] = inventoryItems[itemIndex]
                    .ChangeQuantity(reminder);

            InformAboutChange();
        }
    }


    public Dictionary<int, InventoryItemStruct> GetCurrentInventoryState()
    { 
        Dictionary<int, InventoryItemStruct> returnValue= new Dictionary<int, InventoryItemStruct>();
        for(int i = 0; i < inventoryItems.Count; i++)
        {
            if (inventoryItems[i].IsEmpty) 
                continue;
            returnValue[i] = inventoryItems[i];
        }
        return returnValue;
    }

    public InventoryItemStruct GetItemAt(int obj)
    {
        return inventoryItems[obj];
    }
}

[Serializable]
public struct InventoryItemStruct
{
    public int quantity;
    public BlockInfo item;

    public bool IsEmpty => item == null;

    public InventoryItemStruct ChangeQuantity(int newQuantity)
    {
        return new InventoryItemStruct
        {
            item = this.item,
            quantity = newQuantity,
        };
    }

    public static InventoryItemStruct GetEmptyItem() => new InventoryItemStruct
    {
        item = null,
        quantity = 0,
    };
}
