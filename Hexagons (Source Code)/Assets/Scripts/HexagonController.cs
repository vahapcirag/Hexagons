using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonController : MonoBehaviour
{

    readonly float horizontalDistance = .88f;

    public void ChangeYPos(float i)
    {
        transform.position -= new Vector3(0f, 0f, horizontalDistance) * i;
    }

    public void CheckDown()
    {
        GetComponent<PolygonCollider2D>().enabled = false;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector3(0f, -1f, 0f), 0.5f);
        if (hit.collider == null)
        {
            transform.position -= new Vector3(0f, 0.5f, 0f);
        }
        GetComponent<PolygonCollider2D>().enabled = true;
    }

    public IEnumerator Fall()
    {

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

}
