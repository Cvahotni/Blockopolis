using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Inventory))]
public class DragAndDrop : MonoBehaviour
{
    [SerializeField] private UIItemSlot cursorSlot;
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private GraphicRaycaster raycaster;

    private Inventory inventory;
    private PointerEventData pointerEventData;
    private ItemSlot cursorItemSlot;

    private readonly string uiItemSlotTag = "UIItemSlot";

    private void Start() {
        cursorItemSlot = new ItemSlot(cursorSlot, null, new ItemStack(0, 0));
        inventory = Inventory.Instance;
    }

    private void Update() {
        if(!inventory.InUI) return;
        cursorSlot.transform.position = Input.mousePosition;
        
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

        if(!cursorItemSlot.HasItem && clickedSlot.HasItem) {
            cursorItemSlot.Stack = clickedSlot.ItemSlot.TakeAll();
            cursorSlot.UpdateItemSlot(true);

            return;
        }

        if(cursorItemSlot.HasItem && !clickedSlot.HasItem) {
            clickedSlot.ItemSlot.Stack = cursorItemSlot.TakeAll();
            clickedSlot.UpdateItemSlot(true);

            return;
        }

        if(cursorItemSlot.HasItem && clickedSlot.HasItem) {
            if(cursorItemSlot.Stack.ID != clickedSlot.ItemSlot.Stack.ID) {
                ItemStack oldCursorStack = cursorItemSlot.TakeAll();
                ItemStack oldStack = clickedSlot.ItemSlot.TakeAll();

                clickedSlot.ItemSlot.InsertStack(oldCursorStack);
                cursorItemSlot.InsertStack(oldStack);
            }

            else {
                ushort amount = clickedSlot.ItemSlot.Give(cursorItemSlot.Stack.Amount);
                ItemStack stack = new ItemStack(cursorItemSlot.Stack.ID, amount);
                
                cursorItemSlot.InsertStack(stack);

                cursorSlot.UpdateItemSlot(true);
                clickedSlot.UpdateItemSlot(true);
            }
        }
    }

    private void HandleSlotAlternativeClick(UIItemSlot clickedSlot) {
        if(clickedSlot == null) return;

        if(!cursorItemSlot.HasItem && clickedSlot.HasItem) {
            int clickedStackSize = clickedSlot.ItemSlot.Stack.Amount;
            if(clickedStackSize <= 1) return;
            
            if(cursorItemSlot.Stack.ID != 0) {
                if(cursorItemSlot.Stack.ID != clickedSlot.ItemSlot.Stack.ID) {
                    return;
                }
            }

            ushort clickedStackSizeHalved = (ushort) (clickedStackSize / 2);
            clickedSlot.ItemSlot.Take(clickedStackSizeHalved);

            cursorItemSlot.InsertStack(new ItemStack(clickedSlot.ItemSlot.Stack.ID, clickedStackSizeHalved));
            cursorSlot.UpdateItemSlot(true);

            return;
        }

        if(cursorItemSlot.HasItem) {
            if(clickedSlot.ItemSlot.Stack.ID != 0) {
                if(cursorItemSlot.Stack.ID != clickedSlot.ItemSlot.Stack.ID) {
                    return;
                }
            }

            if(clickedSlot.ItemSlot.Stack.Amount >= InventoryProperties.maxStackSize) return;
            
            if(!clickedSlot.HasItem) {
                clickedSlot.ItemSlot.InsertStack(new ItemStack(cursorItemSlot.Stack.ID, 0));
            }

            clickedSlot.ItemSlot.Give(1);
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
}
