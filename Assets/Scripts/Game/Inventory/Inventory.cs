using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using JetBrains.Annotations;
public class Inventory : MonoBehaviour
{
    [SerializeField] private int capacity;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform slotStartPos;
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
            var slotObj = Instantiate(slotPrefab, slotStartPos.position + new Vector3(i * 1.5f, 0, 0), Quaternion.identity, transform);
            var slot = slotObj.GetComponent<Slot>();
            if (slot == null)
            {
                Debug.LogError("Slot prefab does not have a Slot component.");
                continue;
            }
            slots.Add(slot);
        }
    }

    public void AddTurtle(Turtle turtle)
    {
        foreach (var slot in slots)
        {
            if (slot.IsEmpty)
            {
                slot.SetTurtle(turtle);
                CheckIfTurtlesCompleted();
                return;
            }
        }

        Debug.LogWarning("No empty slots available to add the turtle.");
    }

    public void CheckIfTurtlesCompleted()
    {
        List<Texture> checkedTextures = new List<Texture>();
        for (int i = 0; i < slots.Count; i++)
        {
            bool IsTextureInList = false;
            foreach (var texture in checkedTextures) 
            {
                if (slots[i].Turtle.Texture == texture)
                {
                    IsTextureInList = true;
                }
            }
            if (!IsTextureInList)
            {
                checkedTextures.Add(slots[i].Turtle.Texture);
            }
            else
            {
                continue;
            }
            int TextureCount = 1;
            for (int j = i + 1; j < slots.Count; j++)
            {
                if (slots[i].Turtle.Texture == slots[j].Turtle.Texture)
                {
                    TextureCount++;
                }
            }
            if (TextureCount >= 3)
            {
                GameEvents.OnTurtlesCompleted?.Invoke();
                Debug.Log("Turtles completed!");
                return;
            }
        }

    }
}