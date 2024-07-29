using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InventoryController : MonoBehaviour
{
    [SerializeField]
    private InventoryPage inventory;

    public GameObject ExitButton;

    [SerializeField]
    private InventorySO inventoryData;

    public List<InventoryItemStruct> inventoryItems = new List<InventoryItemStruct>();

    private MonoBehaviour cameraController;
    private MonoBehaviour GameWorld;

    public GameObject player; // Добавьте ссылку на игрока
    public GameObject game;

    public void Start()
    {
        PrepareUI();
        PrepareInventoryData();

        cameraController = player.GetComponent<MonoBehaviour>();

        GameWorld = game.GetComponent<MonoBehaviour>();
    }

    public void LoadMenu()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("Menu");
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
        inventory.UpdateDescription(obj, item.ItemImage, item.Name, item.Description);
    }

    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.I))
        {
            if(inventory.isActiveAndEnabled == false)
            {
                Time.timeScale = 0f;
                inventory.Show();
                ExitButton.SetActive(true);
                Cursor.lockState = CursorLockMode.None; // Разблокировать курсор
                cameraController.enabled = false; // Отключить управление камерой
                GameWorld.enabled = false;
                foreach (var item in inventoryData.GetCurrentInventoryState())
                {
                    inventory.UpdateData(item.Key,
                        item.Value.item.ItemImage,
                        item.Value.quantity);
                }
            }
            else
            {
                inventory.Hide();
                ExitButton.SetActive(false);
                Time.timeScale = 1.0f;
                Cursor.lockState = CursorLockMode.Locked; // Заблокировать курсор
                cameraController.enabled = true; // Включить управление камерой
                GameWorld.enabled = true;
            }
        }
    }
}
