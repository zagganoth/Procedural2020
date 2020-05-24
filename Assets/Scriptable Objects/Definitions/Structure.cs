using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;
[CreateAssetMenu(fileName = "New Structure", menuName = "Custom/Worldgen/Structure")]
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
    public List<Vector2Int> list;
    public Vector3List(List<Vector2Int> initList)
    {
        list = initList;
    }
    public Vector3List()
    {

    }
}
