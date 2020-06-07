using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeOverrideComponent : ItemComponent
{
    [SerializeField]
    public bool infiniteRange = false;
    [SerializeField]
    public float range;
}
