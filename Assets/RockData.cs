using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RockData : MonoBehaviour
{
    float counter;
    float delta;
    [SerializeField]
    public GameObject projectilePrefab;
    bool incDist;
    bool decDist;
    Tilemap structureWalls;
    private void Start()
    {
        counter = 0;
        delta = Mathf.PI * 2 / 360;
        incDist = false;
        decDist = false;
        //structureWalls = GameObject.FindWithTag("StructureWalls").GetComponent<Tilemap>();
    }
    private IEnumerator moveInArc(List<GameObject> projectiles, float smallestAngle, float baseProjectileDistance, Animator animator)
    {
        while (true)
        {
            yield return new WaitForSeconds(0.01f);
            Vector3 destPos;
            int i = 0;
            foreach (var projectile in projectiles)
            {
                destPos = new Vector3(Mathf.Cos(smallestAngle * i + counter) * baseProjectileDistance, Mathf.Sin(smallestAngle * i + counter) * baseProjectileDistance);
                projectile.transform.position = Vector3.Lerp(projectile.transform.position, animator.gameObject.transform.position + destPos, 1f);
                i++;
            }
            counter += delta;
            if(counter>=(Mathf.PI * 2 /360 * 480))
            {
                //Debug.Log("Flip!");
                delta = Mathf.Clamp(-delta * 1.5f,-0.02f,0.02f);
                decDist = false;
                incDist = true;
            }
            else if(counter <0)
            {
                //Debug.Log("break!");
                delta = delta = Mathf.Clamp(-delta * 1.75f,-0.02f,0.02f);
                decDist = true;
                incDist = false;
            }
            if(incDist)
            {
                baseProjectileDistance += delta/3;
            }
            else if(decDist)
            {
                baseProjectileDistance += delta/3;
            }
        }
    }
    public void call_arc(List<GameObject> projectiles,float smallestAngle,float baseProjectileDistance, Animator animator)
    {
        StartCoroutine(moveInArc(projectiles, smallestAngle, baseProjectileDistance, animator));
    }
}
