using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject craftingSlotObj;
    public GameObject craftingResultObj;

    public GameObject[] itemsInventory;
    public GameObject[] itemsDropped;

    public int[] hotbar = new int[9];
    public GameObject hotbarObj;
    public Transform itemContainer;
    
    GameObject itemObj;
    GameObject[,] itemObjArray = new GameObject[9,5];

    public int nextFreeSlotColumn = -1;
    public int nextFreeSlotRow = -1;
    public bool isInventoryFull = false;
    
    public int selectedItem;
    public int selectedSlot = 0;
    public GameObject selectedSlotPrefab;
    [SerializeField] Vector2[] slotPos;

    bool shouldUpdateInv = false;

    public bool isStoneNear = false;
    public Stone stoneRange;

    public bool isInvOpen = false;
    public GameObject inventoryObj;
    public int[,] inventory = new int[9, 5];
    bool isDragging;
    public Vector3 oldMousePos;
    GameObject draggedItem;

    public Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        selectedItem = hotbar[selectedSlot];
        inventoryObj.SetActive(false);

        for (int x = 0; x < 9; x++)
        {
            
            for (int y = 0; y < 3; y++)
            {
                inventory[x, y] = 0;
                if(y == 0)
                {
                    inventory[x, y] = 2;
                }
            }
        }

        

        UpdateInventory();
        UpdateSelectedSlot();

        oldMousePos = new Vector3(cam.ScreenToWorldPoint(Input.mousePosition).x, cam.ScreenToWorldPoint(Input.mousePosition).y, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
        if(oldMousePos != Input.mousePosition && isDragging)
        {
            oldMousePos = new Vector3(cam.ScreenToWorldPoint(Input.mousePosition).x, cam.ScreenToWorldPoint(Input.mousePosition).y, 0);

            draggedItem.transform.position = oldMousePos;
        }

        if(shouldUpdateInv)
        {
            UpdateInventory();
            shouldUpdateInv = false;
        }

        if(Input.GetKeyDown(KeyCode.Q) && selectedItem != 0 && !isInvOpen)
        {
            DropItem();
            UpdateInventory();
            UpdateSelectedSlot();
        }
        if(Input.GetKeyDown(KeyCode.B) && isStoneNear)
        {
            if(stoneRange.lives > 1)
            {
                stoneRange.lives--;
            }
            else
            {
                stoneRange.DestroyStone(itemsDropped[3], itemContainer);
                isStoneNear = false;
            }
        }
        if(Input.mouseScrollDelta.y != 0)
        {
            UpdateSelectedSlot();
        }

        if(Input.GetKeyDown(KeyCode.E) && isInvOpen)
        {
            CloseInventory();
        }
        else if(Input.GetKeyDown(KeyCode.E))
        {
            OpenInventory();
        }
        if(isInvOpen && Input.GetMouseButtonDown(0))
        {
            Debug.Log("Open...");
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            Debug.Log(hit.collider.transform.gameObject.name);
            if (hit.collider != null && hit.collider.transform.gameObject.TryGetComponent<InventoryItem>(out InventoryItem item) && !isDragging)
            {
                Debug.Log("Hit Item...");
                isDragging = true;
                draggedItem = item.gameObject;
                inventory[item.inventoryIndexX, item.inventoryIndexY] = 0;
                draggedItem.GetComponent<BoxCollider2D>().enabled = false;
            }
            else if(hit.collider != null && hit.collider.gameObject.TryGetComponent<InventorySlot>(out InventorySlot slot) && isDragging)
            {
                Debug.Log("Hit Slot...");
                if (inventory[slot.posX, slot.posY] == 0)
                {
                    Debug.Log("It's empty...");

                    draggedItem.GetComponent<InventoryItem>().isInCraftingSlot = false;
                    inventory[slot.posX, slot.posY] = draggedItem.GetComponent<InventoryItem>().itemIndex;
                    UpdateInventory();
                    UpdateSelectedSlot();
                    isDragging = false;
                    draggedItem.GetComponent<BoxCollider2D>().enabled = true;
                    draggedItem.GetComponent<InventoryItem>().inventoryIndexX = slot.posX;
                    draggedItem.GetComponent<InventoryItem>().inventoryIndexY = slot.posY;
                    Destroy(draggedItem);
                }
            }
            
        }
        //selecteditem = hotbar[selectedslot];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.TryGetComponent<Item>(out Item collected) && !isInventoryFull)
        {
            Debug.Log("Item collected...");
            inventory[nextFreeSlotColumn, nextFreeSlotRow] = collected.itemindex;
            Destroy(collision.gameObject.transform.parent.gameObject);
            UpdateInventory();
            UpdateSelectedSlot();
        }
        else if (collision.gameObject.TryGetComponent<Stone>(out stoneRange))
        {
            Debug.Log("Stone...");
            isStoneNear = true;
        }
    }

    void UpdateHotbar()
    {
        Debug.Log("Update Inventory...");
        nextFreeSlotColumn = -1;

        for (int i = 0; i < 9; i++)
        {
            Destroy(itemObjArray[i,i]);
        }

        for (int slot = 0; slot < 9; slot++)
        {
            itemObj = Instantiate(itemsInventory[hotbar[slot]], position: new Vector3(0,0,0), Quaternion.identity);
            itemObj.transform.SetParent(hotbarObj.transform.GetChild(slot + 1).transform);
            itemObjArray[slot,slot] = itemObj;
            if (hotbar[slot] != 0)
            {
                itemObj.transform.localPosition = new Vector3(0,0,0);
            }
            else if(nextFreeSlotColumn == -1)
            {
                nextFreeSlotColumn = slot;
            }
        }

        if(nextFreeSlotColumn == -1)
        {
            isInventoryFull = true;
        }

    }

    private void UpdateInventory()
    {
        Debug.Log("Update Inventory...");
        nextFreeSlotColumn = -1;

        

        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                Destroy(itemObjArray[i, j]);
            }
        }

        for (int slotRow = 0; slotRow < 5; slotRow++)
        {
            for (int slotColumn = 0; slotColumn < 9; slotColumn++)
            {

                itemObj = Instantiate(itemsInventory[inventory[slotColumn, slotRow]], position: new Vector3(0, 0, 0), Quaternion.identity);
                itemObj.GetComponent<InventoryItem>().itemIndex = inventory[slotColumn, slotRow];
                if (slotRow == 0)
                {
                    itemObj.transform.SetParent(hotbarObj.transform.GetChild(slotColumn + 1).transform);
                    itemObj.GetComponent<InventoryItem>().inventoryIndexX = slotColumn;
                    itemObj.GetComponent<InventoryItem>().inventoryIndexY = slotRow;
                }
                else if(slotRow == 4)
                {
                    itemObj.transform.SetParent(craftingSlotObj.transform.GetChild(slotColumn).transform);
                    itemObj.GetComponent<InventoryItem>().inventoryIndexX = slotColumn;
                    itemObj.GetComponent<InventoryItem>().inventoryIndexY = slotRow;
                }
                else
                {
                    itemObj.transform.SetParent(inventoryObj.transform.GetChild(slotRow).GetChild(slotColumn).transform);
                    itemObj.GetComponent<InventoryItem>().inventoryIndexX = slotColumn;
                    itemObj.GetComponent<InventoryItem>().inventoryIndexY = slotRow;
                }

                itemObjArray[slotColumn, slotRow] = itemObj;

                if (inventory[slotColumn, slotRow] != 0)
                {
                    itemObj.transform.localPosition = new Vector3(0, 0, 0);
                }
                else if (nextFreeSlotColumn == -1)
                {
                    nextFreeSlotColumn = slotColumn;
                    nextFreeSlotRow = slotRow;
                }
            }
        }
        try
        {
            Destroy(craftingResultObj.transform.GetChild(0).gameObject);
        }
        catch { }
        CheckCraftingSlots();

        if (nextFreeSlotColumn == -1)
        {
            isInventoryFull = true;
        }
    }

    void CheckCraftingSlots()
    {
        GameObject result;

        Debug.Log("Check Crafting Slots...");

        int[,] craftingSlotArray = new int[3, 3];

        for (int x = 0; x < 9; x++)
        {
            if(x < 3)
            {
                craftingSlotArray[0, x] = inventory[x, 4];
            }
            else if(x < 6)
            {
                craftingSlotArray[1, x - 3] = inventory[x, 4];
            }
            else
            {
                craftingSlotArray[2, x - 6] = inventory[x, 4];
            }
        }

        

        if(craftingSlotArray[1,1] == 2)
        {
            Debug.Log("StickRecipe...");
            result = Instantiate(itemsInventory[1], position: new Vector3(0,0,0), Quaternion.identity);
            result.transform.SetParent(craftingResultObj.transform);
            result.transform.localPosition = new Vector3(0, 0, 0);
        }
        else
        {
            result = Instantiate(itemsInventory[0], position: new Vector3(0, 0, 0), Quaternion.identity);
            result.transform.SetParent(craftingResultObj.transform);

        }
    }
                    

    void UpdateSelectedSlot()
    {
        if (selectedSlot == 8 && Mathf.RoundToInt(Input.mouseScrollDelta.y * -1) > 0)
            selectedSlot = 0;
        else if (Mathf.RoundToInt(Input.mouseScrollDelta.y * -1) < 0 && selectedSlot == 0)
            selectedSlot = 8;
        else
            selectedSlot += Mathf.RoundToInt(Input.mouseScrollDelta.y * -1);

        selectedItem = inventory[selectedSlot, 0];
        selectedSlotPrefab.transform.localPosition = slotPos[selectedSlot];

        Debug.Log(selectedSlot);
        Debug.Log(selectedItem);
    }

    void DropItem()
    {
        GameObject obj;
        Debug.Log("Drop...");
        shouldUpdateInv = true;
        isInventoryFull = false;
        inventory[selectedSlot, 0] = 0;
        UpdateInventory();
        obj = Instantiate(itemsDropped[selectedItem], position: new Vector3(transform.position.x, transform.position.y - 1.3f, transform.position.z), Quaternion.identity);
        obj.transform.SetParent(itemContainer);
        selectedItem = 0;
    }

    void OpenInventory()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        inventoryObj.SetActive(true);
        isInvOpen = true;
    }
    void CloseInventory()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        inventoryObj.SetActive(false);
        isInvOpen = false;
    }
}
