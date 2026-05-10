using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
public class Inventory : MonoBehaviour
{
    [SerializeField] private int capacity;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform slotStartPos;
    [SerializeField] private float slotsOffset;
    [SerializeField] private ParticleSystem turtlesMatchEffect;
    private List<Slot> slots = new();
    [SerializeField] private List<Texture> turtleTextures;
    public bool IsResolvingMatches { get; private set; }
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

    public void AddTurtle(Turtle turtle)
    {
        int insertIndex = -1;

        int countSameColor = 0;

        for (int i = 0; i < slots.Count; i++)
        {
            if (!slots[i].IsEmpty &&
                slots[i].Turtle.Texture == turtle.Texture)
            {
                insertIndex = i;
                countSameColor++;
            }
        }

        if (insertIndex != -1)
        {
            insertIndex++;
            if (insertIndex >= slots.Count)
            {
                Debug.LogError($"Insert index out of range: {insertIndex}");
                return;
            }
            DG.Tweening.Sequence sequence = DOTween.Sequence();

            for (int j = slots.Count - 1; j > insertIndex; j--)
            {
                if (!slots[j - 1].IsEmpty)
                {
                    Turtle movingTurtle = slots[j - 1].Turtle;

                    slots[j].SetTurtle(movingTurtle);
                    slots[j - 1].ClearSlot();
                    sequence.Join(
                        movingTurtle.transform
                            .DOMove(
                                slots[j].transform.position,
                                0.25f
                            )
                            .SetEase(Ease.InOutSine)
                 );
                }
            }

            slots[insertIndex].SetTurtle(turtle);
            sequence.Join(
                turtle.transform.DOScale(1f, 0.5f)
                    .SetEase(Ease.OutBack)
            );
            sequence.OnComplete(() =>
            {
                CheckMatches();
            });

            return;
        }

        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].IsEmpty)
            {
                slots[i].SetTurtle(turtle);
                turtle.transform.DOScale(1f, 0.5f)
                    .SetEase(Ease.OutBack);

                return;
            }
        }

        Debug.Log("NO SLOTS AVAILABLE");
    }
    private void CheckMatches()
    {
        for (int i = 0; i < slots.Count - 2; i++)
        {
            if (slots[i].IsEmpty ||
                slots[i + 1].IsEmpty ||
                slots[i + 2].IsEmpty)
                continue;

            var t0 = slots[i].Turtle;
            var t1 = slots[i + 1].Turtle;
            var t2 = slots[i + 2].Turtle;

            if (t0.Texture == t1.Texture &&
                t1.Texture == t2.Texture)
            {
                TurtlesCompleted(i);
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
            Turtle left = slots[index].Turtle;
            Turtle middle = slots[index + 1].Turtle;
            Turtle right = slots[index + 2].Turtle;

            TurtlesCompleteAnimation(left, middle, right, index);

            return;
        }
    }
    public void TurtlesCompleteAnimation(Turtle left, Turtle middle, Turtle right, int index)
    {
        IsResolvingMatches = true;
        Vector3 upOffset = new Vector3(0, 0, 0.5f);

        Vector3 leftUpPos = left.transform.position + upOffset;
        Vector3 middleUpPos = middle.transform.position + upOffset;
        Vector3 rightUpPos = right.transform.position + upOffset;

        slots[index].ClearSlot();
        slots[index + 1].ClearSlot();
        slots[index + 2].ClearSlot();
        DG.Tweening.Sequence sequence = DOTween.Sequence();
        CollapseSlots();

        // Take Up All Three
        sequence.Join(
            left.transform.DOMove(leftUpPos, 0.25f)
                .SetEase(Ease.OutCubic)
        );

        sequence.Join(
            middle.transform.DOMove(middleUpPos, 0.25f)
                .SetEase(Ease.OutCubic)
        );

        sequence.Join(
            right.transform.DOMove(rightUpPos, 0.25f)
                .SetEase(Ease.OutCubic)
        );

        // Point to be collected
        Vector3 mergePoint = middleUpPos;

        // After Take Over
        sequence.Append(
            left.transform.DOMove(mergePoint, 0.25f)
                .SetEase(Ease.InOutSine)
        );

        sequence.Join(
            right.transform.DOMove(mergePoint, 0.25f)
                .SetEase(Ease.InOutSine)
        );
        sequence.Append(
            left.transform.DOScale(Vector3.zero, 0.2f)
                .SetEase(Ease.InBack)
        );

        sequence.Join(
            middle.transform.DOScale(Vector3.zero, 0.2f)
                .SetEase(Ease.InBack)
        );

        sequence.Join(
            right.transform.DOScale(Vector3.zero, 0.2f)
                .SetEase(Ease.InBack)
        );
        sequence.Append(
            right.transform.DOPunchScale(
                Vector3.one * 0.3f,
                0.15f,
                10,
                1
            )
        );
        sequence.Join(
            left.transform.DOPunchScale(
                Vector3.one * 0.3f,
                0.15f,
                10,
                1
            )
        );
        sequence.Join(
            middle.transform.DOPunchScale(
                Vector3.one * 0.3f,
                0.15f,
                10,
                1
            )
        );
        sequence.AppendCallback(() =>
        {
            var turtleEffect = Instantiate(
                turtlesMatchEffect,
                middle.transform.position,
                Quaternion.identity,
                transform
            );

            var main = turtleEffect.main;

            main.startColor = new ParticleSystem.MinMaxGradient(GetColorOfTurtle(middle.Texture));
            AudioManager.Instance.PlaySound("TurtlesMatch");
            CheckMatches();
            turtleEffect.Play();
        });
        sequence.OnComplete(() =>
        {
            left.transform.DOKill();
            middle.transform.DOKill();
            right.transform.DOKill();
            left.gameObject.SetActive(false);
            middle.gameObject.SetActive(false);
            right.gameObject.SetActive(false);
            IsResolvingMatches = false;
            CheckIfInventoryIsFulled();
            GameEvents.OnTurtleDissapear?.Invoke();
        });
        
    }
    public Color GetColorOfTurtle(Texture turtleTexture)
    {
        if (turtleTexture == turtleTextures[0])
        {
            Debug.Log("Purple");
            return Color.purple;
        }
        else if (turtleTexture == turtleTextures[1])
        {
            Debug.Log("Orange");
            return Color.orange;
        }
        else if (turtleTexture == turtleTextures[2])
        {
            Debug.Log("Yellow");
            return Color.yellow;
        }
        Debug.Log("White");
        return Color.black;
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
        GameEvents.OnGameCondition?.Invoke(false);
    }
    private void CollapseSlots()
    {
        int writeIndex = 0;
        DG.Tweening.Sequence sequence = DOTween.Sequence();

        for (int readIndex = 0; readIndex < slots.Count; readIndex++)
        {
            if (!slots[readIndex].IsEmpty)
            {
                if (writeIndex != readIndex)
                {
                    sequence.Join(slots[readIndex].Turtle.transform.DOMove(
                        slots[writeIndex].transform.position, 0.25f)
                        .SetEase(Ease.InOutSine)
                        );
                    Turtle turtle = slots[readIndex].Turtle;

                    slots[readIndex].ClearSlot();

                    slots[writeIndex].SetTurtle(turtle);
                }

                writeIndex++;
            }
        }
        //sequence.OnComplete(() => InventoryShake());
    }
    public void InventoryShake()
    {
        DG.Tweening.Sequence sequence = DOTween.Sequence();
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].IsEmpty) continue;
            sequence.Join(slots[i].Turtle.transform.DOPunchPosition(
                    Vector3.forward * 0.1f,
                    0.3f,
                    10,
                    1f
                ));
        }
    }
    public bool HasFreeSlot()
    {
        for (int i = 0; i < capacity; i++)
        {
            if (slots[i].IsEmpty)
                return true;
        }

        return false;
    }
}