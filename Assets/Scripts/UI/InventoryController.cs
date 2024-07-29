using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [SerializeField]
    private InventoryPage inventory;

    [SerializeField]
    private InventorySO inventoryData;

    public List<InventoryItemStruct> inventoryItems = new List<InventoryItemStruct>();


    public void Start()
    {
        PrepareUI();
        PrepareInventoryData();
    }

    private void PrepareInventoryData()
    {
        inventoryData.Initialize();
        inventoryData.OnInventoryUpdated += UpdateUI;
        foreach (InventoryItemStruct item in inventoryItems)
        {
            if (item.IsEmpty) 
                continue;
            inventoryData.AddItem(item);
        }
    }

    private void UpdateUI(Dictionary<int, InventoryItemStruct> inventoryState)
    {
        inventory.ResetAllItems();
        foreach (var item in inventoryState) 
        {
            inventory.UpdateData(item.Key, item.Value.item.ItemImage, item.Value.quantity);
        }
    }

    private void PrepareUI()
    {
        inventory.InitializeInventory(inventoryData.Size);
        this.inventory.OnDescriptionRequested += HandleDescriptionRequest;
        this.inventory.OnItemActionRequested += HandleItemActionRequest;
    }

    private void HandleItemActionRequest(int obj)
    {
        InventoryItemStruct inventoryItem = inventoryData.GetItemAt(obj);
        if (inventoryItem.IsEmpty)
            return;
        inventoryData.RemoveItem(obj, inventoryItem.quantity);
    }

    private void HandleDescriptionRequest(int obj)
    {
       InventoryItemStruct inventoryItemStruct = inventoryData.GetItemAt(obj);
        if (inventoryItemStruct.IsEmpty)
        {
            inventory.ResetSelection();
            return;
        }
        BlockInfo item = inventoryItemStruct.item;
        inventory.UpdateDescription(obj, item.ItemImage, item.name, item.Description);
    }

    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.I))
        {
            if(inventory.isActiveAndEnabled == false)
            {
                inventory.Show();  
                foreach(var item in inventoryData.GetCurrentInventoryState())
                {
                    inventory.UpdateData(item.Key,
                        item.Value.item.ItemImage,
                        item.Value.quantity);
                }
            }
            else
            {
                inventory.Hide();
            }
        }
    }
}
