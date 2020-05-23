using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Loot Pool", menuName = "Custom/Loot Pool")]
public class LootPool : ScriptableObject
{
    [SerializeField]
    public List<ItemObject> possibleItems;
    [SerializeField]
    private List<float> probabilities;

    public ItemObject getRandomItem()
    {
        return possibleItems[0];
    }
}
