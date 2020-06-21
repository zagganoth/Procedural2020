using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CleanupTiles : MonoBehaviour
{
    Tilemap tm;
    // Start is called before the first frame update
    void Awake()
    {
        tm = GetComponent<Tilemap>();

    }
    private void Start()
    {
        EventManager.instance.OnCleanupRequiredAction += cleanupTile;
    }
    void cleanupTile(object sender, EventManager.OnCleanupRequiredArgs e)
    {
        if (tm.GetTile(e.pos) != null)
        {
            //Debug.Log(tm.GetTile(e.pos));
            tm.SetTile(e.pos, null);
        }
    }

}
