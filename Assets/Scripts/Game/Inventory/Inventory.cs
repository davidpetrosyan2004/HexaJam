using UnityEngine;
using System.Collections.Generic;

using DG.Tweening;
using System;
public class Inventory : MonoBehaviour
{
    [SerializeField] private int capacity;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform slotStartPos;
    [SerializeField] private float slotsOffset;
    private List<Slot> slots = new List<Slot>();

    private void OnEnable()
    {
        GameEvents.OnTurtleAddedInventory += AddTurtle;
    }

    private void OnDisable()
    {
        GameEvents.OnTurtleAddedInventory -= AddTurtle;
    }

    private void Start()
    {
        for (int i = 0; i < capacity; i++)
        {
            var slotObj = Instantiate(slotPrefab, slotStartPos.position + new Vector3(i * slotsOffset, 0, 0), Quaternion.identity, transform);
            var slot = slotObj.GetComponent<Slot>();
            if (slot == null)
            {
                Debug.LogError("Slot prefab does not have a Slot component.");
                continue;
            }
            slots.Add(slot);
        }
    }

    public void InventoryShake()
    {
        transform.DOMoveY(0.2f, 0.1f)
        .SetLoops(2, LoopType.Yoyo);
    }


    public void AddTurtle(Turtle turtle)
    {
        int insertIndex = -1;

        var countSameColor = 0;
        for (int i = 0; i < slots.Count; i++)
        {
            if (!slots[i].IsEmpty && slots[i].Turtle.Texture == turtle.Texture)
            {
                insertIndex = i;
                countSameColor++;
            }
        }

        if (insertIndex != -1)
        {
            insertIndex++;

            for (int j = slots.Count - 1; j > insertIndex; j--)
            {
                if (!slots[j - 1].IsEmpty)
                    slots[j].SetTurtle(slots[j - 1].Turtle);
            }

            slots[insertIndex].SetTurtle(turtle);
            if(countSameColor == 2)
            {
                TurtlesCompleted(insertIndex-2);
            }

            CheckIfInventoryIsFulled();
            return;
        }

        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].IsEmpty)
            {
                slots[i].SetTurtle(turtle);

                CheckIfInventoryIsFulled();
                return;
            }
        }
    }
    public void TurtlesCompleted(int index)
    {
        if (index < 0 || index + 2 >= slots.Count)
            return;

        var t0 = slots[index].Turtle;
        var t1 = slots[index + 1].Turtle;
        var t2 = slots[index + 2].Turtle;

        if (t0 == null || t1 == null || t2 == null)
            return;

        if (t0.Texture == t1.Texture && t1.Texture == t2.Texture)
        {
            for (int j = index; j <= index + 2; j++)
            {
                slots[j].Turtle.gameObject.SetActive(false);
                slots[j].ClearSlot();
            }

            GameEvents.OnTurtleDissapear?.Invoke();

            InventoryShake();

            CollapseSlots();

            return;
        }
    }

    private void CheckIfInventoryIsFulled()
    {
        for(int i = 0; i < capacity; i++)
        {
            if (slots[i].IsEmpty)
            {
                return;
            }
        }
        Debug.Log("Game Over");
        //GameEvents.OnGameOver?.Invoke();
    }

    void CollapseSlots()
    {
        int writeIndex = 0;

        for (int readIndex = 0; readIndex < slots.Count; readIndex++)
        {
            if (!slots[readIndex].IsEmpty)
            {
                if (writeIndex != readIndex)
                {
                    slots[writeIndex].SetTurtle(slots[readIndex].Turtle);
                    slots[readIndex].ClearSlot();
                }

                writeIndex++;
            }
        }
    }

    public bool IsInventoryFull()
    {
        foreach (var slot in slots)
        {
            if (slot.IsEmpty)
                return false;
        }
        return true;
    }
}