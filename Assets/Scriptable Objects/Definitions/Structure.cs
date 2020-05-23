using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;
[CreateAssetMenu(fileName = "New Structure", menuName = "Custom/Structures/Structure")]
public class Structure : ScriptableObject
{
    [SerializeField]
    public List<Vector3List> positionLists;
    [SerializeField]
    public List<TileList> tileLists;
    [SerializeField]
    public float spawnOdds;
}
[System.Serializable]
public class TileList
{
    public List<TileBase> list;
    public TileList(List<TileBase> initList)
    {
        list = initList;
    }
}
[System.Serializable]
public class Vector3List
{
    public List<Vector3Int> list;
    public Vector3List(List<Vector3Int> initList)
    {
        list = initList;
    }
    public Vector3List()
    {

    }
}
