using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Inventory))]
[RequireComponent(typeof(Hotbar))]
public class DragAndDrop : MonoBehaviour
{
    [SerializeField] private UIItemSlot cursorSlot;
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private GraphicRaycaster raycaster;

    private Inventory inventory;
    private Hotbar hotbar;
    private InventoryEventSystem inventoryEventSystem;
    private PointerEventData pointerEventData;
    private ItemSlot cursorItemSlot;

    private readonly string uiItemSlotTag = "UIItemSlot";

    private void Start() {
        cursorItemSlot = new ItemSlot(cursorSlot, null, new ItemStack(0, 0));
        
        inventory = Inventory.Instance;
        hotbar = Hotbar.Instance;
        inventoryEventSystem = InventoryEventSystem.Instance;
    }

    private void Update() {
        if(!inventory.InUI) return;
        cursorSlot.transform.position = Input.mousePosition;

        UIItemSlot currentSlot = CheckForSlot();
        if(currentSlot != null) currentSlot.Highlighted = true;
        
        if(Input.GetMouseButtonDown(0)) {
            HandleSlotClick(CheckForSlot());
        }

        if(Input.GetMouseButtonDown(1)) {
            HandleSlotAlternativeClick(CheckForSlot());
        }
    }

    private void HandleSlotClick(UIItemSlot clickedSlot) {
        if(clickedSlot == null) return;
        if(!cursorItemSlot.HasItem && !clickedSlot.HasItem) return;

        ItemSlot clickedItemSlot = clickedSlot.ItemSlot;
        ItemStack clickedItemStack = clickedItemSlot.Stack;
        ItemStack cursorItemStack = cursorSlot.ItemSlot.Stack;

        if(!cursorItemSlot.HasItem && clickedSlot.HasItem) {
            cursorSlot.ItemSlot.Stack = clickedItemSlot.TakeAll();
            cursorSlot.UpdateItemSlot(true);
            CheckHotbarSlot(clickedItemSlot);

            return;
        }

        if(cursorItemSlot.HasItem && !clickedSlot.HasItem) {
            clickedSlot.ItemSlot.Stack = cursorItemSlot.TakeAll();
            clickedSlot.UpdateItemSlot(true);
            CheckHotbarSlot(clickedItemSlot);

            return;
        }

        if(cursorItemSlot.HasItem && clickedSlot.HasItem) {
            if(cursorItemStack.ID != clickedItemStack.ID) {
                ItemStack oldCursorStack = cursorItemSlot.TakeAll();
                ItemStack oldStack = clickedItemSlot.TakeAll();

                clickedItemSlot.InsertStack(oldCursorStack);
                cursorItemSlot.InsertStack(oldStack);

                CheckHotbarSlot(clickedItemSlot);
            }

            else {
                ushort amount = clickedItemSlot.Give(cursorItemStack.Amount);
                ItemStack stack = new ItemStack(cursorItemStack.ID, amount);
                
                cursorItemSlot.InsertStack(stack);

                cursorSlot.UpdateItemSlot(true);
                clickedSlot.UpdateItemSlot(true);
            }
        }
    }

    private void HandleSlotAlternativeClick(UIItemSlot clickedSlot) {
        if(clickedSlot == null) return;

        ItemSlot clickedItemSlot = clickedSlot.ItemSlot;
        ItemStack clickedItemStack = clickedItemSlot.Stack;
        ItemStack cursorItemStack = cursorSlot.ItemSlot.Stack;

        if(!cursorItemSlot.HasItem && clickedSlot.HasItem) {
            int clickedStackSize = clickedItemStack.Amount;
            if(clickedStackSize <= 1) return;
            
            if(cursorItemStack.ID != 0) {
                if(cursorItemStack.ID != clickedItemStack.ID) {
                    return;
                }
            }

            ushort clickedStackSizeHalved = (ushort) (clickedStackSize / 2);
            clickedItemSlot.Take(clickedStackSizeHalved);

            cursorItemSlot.InsertStack(new ItemStack(clickedItemStack.ID, clickedStackSizeHalved));
            cursorSlot.UpdateItemSlot(true);

            return;
        }

        if(cursorItemSlot.HasItem) {
            if(clickedItemStack.ID != 0) {
                if(cursorItemStack.ID != clickedItemStack.ID) {
                    return;
                }
            }

            if(clickedItemStack.Amount >= InventoryProperties.maxStackSize) return;
            
            if(!clickedSlot.HasItem) {
                clickedItemSlot.InsertStack(new ItemStack(cursorItemStack.ID, 0));
            }

            clickedItemSlot.Give(1);
            cursorItemSlot.Take(1);
            
            cursorSlot.UpdateItemSlot(true);
            clickedSlot.UpdateItemSlot(true);
        }
    }

    private UIItemSlot CheckForSlot() {
        pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, results);

        foreach(RaycastResult result in results) {
            if(result.gameObject.tag == uiItemSlotTag) {
                return result.gameObject.GetComponent<UIItemSlot>();
            }
        }

        return null;
    }

    private void CheckHotbarSlot(ItemSlot itemSlot) {
        if(!itemSlot.IsHotbarSlot) return;
        if(!hotbar.CurrentSlot.Equals(itemSlot)) return;

        SwitchedItemStack switchedItemStack = new SwitchedItemStack(itemSlot.Stack, 1);
        inventoryEventSystem.InvokeModifyHeldSlot(switchedItemStack);
    }
}
