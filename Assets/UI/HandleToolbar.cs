using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleToolbar : MonoBehaviour
{
    [SerializeField]
    private PlayerController playerController;
    [SerializeField]
    private List<SlotUI> slots = new List<SlotUI>();

    public int selectedSlot;

    private float previousScroll;

    private void Start()
    {
        selectedSlot = 0;
        previousScroll = Input.mouseScrollDelta.y;
        GameEvents.current.onStacksChanged += UpdateSlots;
    }

    // Update is called once per frame
    void Update()
    {
        // selection handling
        if (Input.mouseScrollDelta.y > previousScroll)
        {
            UpdateSelectedSlot(1);
        }
        else if( Input.mouseScrollDelta.y < previousScroll)
        {
            UpdateSelectedSlot(-1);
        }
        slots[selectedSlot].Select();
    }

    // For each stack in the inventory, allocate a slot in the toolbar UI
    public void UpdateSlots(List<SeedStack> stacks)
    {
        int i;
        for (i = 0; i < stacks.Count; i++)
        {
            UpdateSlot(true, stacks[i], i);
        }
        for(int j = i;  j < slots.Count; j++)
        {
            UpdateSlot(false, null, j);
        }
    }

    private void UpdateSlot(bool active, SeedStack stack, int index)
    {
        slots[index].icon.gameObject.SetActive(active);
        if (stack != null && active)
        {
            slots[index].SetIcon(stack.flowerInformation);
        }
    }

    private void UpdateSelectedSlot(int change)
    {
        slots[selectedSlot].Deselect();
        selectedSlot = selectedSlot + change;
        if (selectedSlot == slots.Count)
        {
            selectedSlot = 0;
        }
        else if(selectedSlot < 0)
        {
            selectedSlot = slots.Count - 1;
        }
        GameEvents.current.NewSelection(selectedSlot);
    }

    private void OnDisable()
    {
        GameEvents.current.onStacksChanged -= UpdateSlots;
    }
}
