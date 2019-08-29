using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    GameManager gameManager;
    
    private void Start()
    {
        gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
        
    }

    public void Spawn()
    {
        if (transform.position.y < 0)
            return;
        StartCoroutine(SpawnNewGrids());
    }

    

    IEnumerator SpawnNewGrids()
    {
        
            
            
        
        gameObject.GetComponent<PolygonCollider2D>().enabled = false;
        while (true)
        {
            RaycastHit2D hit = RayCast_();
            if (hit.collider == null)
            {

                GameObject go = gameManager.SetHexColorAndSpawn(transform.position, "HexagonWithoutCollider");
                StartCoroutine(go.GetComponent<HexagonController>().Fall());
            }
            else
                break;
            yield return new WaitForSeconds(1f);
        }

    }

    public RaycastHit2D RayCast_()
    {
        return Physics2D.Raycast(transform.position, Vector3.down, 0.5f); 
    }

    public void SpawnBomb()
    {
        Instantiate(Resources.Load<GameObject>("Bomb"), transform.position, Quaternion.identity);
    }

    
}
