using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombManager : MonoBehaviour
{
    public int value;
    GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        int k;
        value = 8;
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        while (true)
        {
            k = Random.Range(0,gameManager.colours.Length);
            if (gameManager.selectedColours[k])
                break;
        }
        

        Renderer bombRenderer = GetComponent<Renderer>();

        bombRenderer.material = Resources.LoadAll<Material>("Materials")[k];
        StartCoroutine(Fall());
    }

    private void Update()
    {
        
    }

    public IEnumerator Fall()
    {
        gameObject.GetComponent<PolygonCollider2D>().enabled = false;
        while (true)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.down, 0.5f);
            if (hit.collider == null)
            {
                transform.position += Vector3.down / 4;
            }
            else
            {
                gameObject.GetComponent<PolygonCollider2D>().enabled = true;
                break;
            }

            yield return new WaitForSeconds(0.15f);
        }
        gameObject.GetComponent<PolygonCollider2D>().enabled = true;
    }

    public void SetBombCounter()
    {
        value--;
        if (value == 0)
        {
            gameManager.isGameOver = true;
            Destroy(gameObject);
        }
        GetComponent<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("BombSprites")[value-1];
    }
}
