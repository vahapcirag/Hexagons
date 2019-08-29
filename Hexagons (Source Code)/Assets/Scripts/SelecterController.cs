using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SelecterController : MonoBehaviour
{
    [SerializeField] Transform firstRefTransform;
    [SerializeField] Transform secondRefTransform;
    [SerializeField] Transform thirdRefTransform;
    [SerializeField] Transform QuaternionRefTransform;

    [SerializeField] GameObject[] children = new GameObject[3];
    [SerializeField] GameObject[] fixedChildren = new GameObject[3];
    [SerializeField] List<GameObject> neighbors = new List<GameObject>();

    Vector2 startPos = Vector2.zero;
    Vector2 endPos = Vector2.zero;

    GameManager gameManager;

    public bool rotating = false;

    [SerializeField] int quaternionIndex;
    [SerializeField] int rotationIndex;
    readonly int hexagonChildStartIndex = 5;
    int sign = 0;
    float directionMagnitude = 0f;

    

    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        quaternionIndex = 0;
        rotationIndex = 0;

        transform.GetChild(4).parent = null;

        FindHexagons();

        
        Invoke("Ray", 0.7f);

    }

    
    void Ray()
    {
        if (gameObject.tag == "Group")
            RotateWithChildren(endPos.y - startPos.y, quaternionIndex, rotationIndex);
    }

    void FixedUpdate()
    {
        if (gameObject.tag == "Group")
            return;

        if (rotating || gameManager.checking)
            return;
        Vector2 angleVect = endPos - startPos;
        float tan = angleVect.y / angleVect.x;
        float angle = Mathf.Atan(tan) * 57.2958f;
        Debug.DrawLine(startPos, endPos, Color.red);

        if ((angle < 45f) && (angle > -45f))
        {
            DeattachHexagonsAndRotateAroundRefPoint();

            gameManager.RayCastToFindOtherHexagons(quaternionIndex, children, true);
            

            startPos = Vector2.zero;
            endPos = Vector2.zero;
        }
        else if (startPos != Vector2.zero && !rotating)
        {
            rotating = true;

            if (endPos.y < startPos.y)
                angle *= -1f;

            RotateWithChildren(endPos.y - startPos.y, quaternionIndex, rotationIndex);

            Debug.Log("Angle: " + angle + "Abs Angle: " + Mathf.Abs(angle));

            startPos = Vector2.zero;
            endPos = Vector2.zero;
        }
       
    }


    public void DestroySelecter()
    {
        while (transform.childCount != 0)
        {
            transform.GetChild(0).parent = null;
        }
        GameObject[] selecterChilds = GameObject.FindGameObjectsWithTag("SelecterChilds");

        foreach (GameObject item in selecterChilds)
        {
            Destroy(item);
        }

        Destroy(gameObject);
    }

    public void GetInputInformation(Vector2 start, Vector2 end, float magnitude)
    {
        startPos = start;
        endPos = end;
        directionMagnitude = magnitude;
    }

    void RotateWithChildren(float f, int quaternionIndex, int rotationIndex_)
    {
        sign = 1;


        if (f > 0)
            sign *= -1;

        if (rotationIndex == 2 && sign < 0)
        {
            rotationIndex = 0;
        }

        else if (rotationIndex == 0 && sign > 0)
        {
            rotationIndex = 3;
        }
        if (gameObject.tag == "Selecter")
        {
            StartCoroutine(RotationWithAnimation(gameObject, sign, quaternionIndex));
        }
        else
        {
            StartCoroutine(RotateWithoutAnimation(gameObject, sign, quaternionIndex));
        }
    }

    void SetCollidersAndShotringLayers()
    {
        Debug.Log(children.Length);
        for (int i = 0; i < children.Length; i++)
        {
            children[i].GetComponent<PolygonCollider2D>().enabled = false;
            children[i].GetComponent<SpriteRenderer>().sortingOrder = 1;
        }

    }

    IEnumerator RotationWithAnimation(GameObject g, int sign, int quaternionIndex)
    {
        int turnCount = 0;
        while (turnCount < 3)
        {
            if (sign > 0)
            {
                rotationIndex--;
            }
            else if (sign < 0)
            {
                rotationIndex++;
            }

            SetCollidersAndShotringLayers();
            for (int i = 0; i < 40; i++)
            {
                transform.RotateAround(transform.position, Vector3.forward, sign*3);
                yield return new WaitForSeconds(float.Epsilon);
            }

            FixCollidersAndSpriteShortingLayer();

            yield return new WaitForSeconds(0.1f);
            SetChildOrder(quaternionIndex, rotationIndex);
            gameManager.RayCastToFindOtherHexagons(quaternionIndex, children, true);
            //gameManager.FallHexes();
            turnCount++;
        }
        rotationIndex = 0;
        rotating = false;
        yield return null;
    }

    IEnumerator RotateWithoutAnimation(GameObject g, int sign, int quaternionIndex)
    {
        int turnCount = 0;
        while (turnCount < 3)
        {

            if (sign > 0)
            {
                rotationIndex--;
            }
            else if (sign < 0)
            {
                rotationIndex++;
            }

            SetCollidersAndShotringLayers();
            for (int i = 0; i < 8; i++)
            {
                transform.RotateAround(transform.position, Vector3.forward, sign * 15);
                yield return new WaitForSeconds(float.Epsilon);
            }

            FixCollidersAndSpriteShortingLayer();

            yield return new WaitForSeconds(0.001f);
            SetChildOrder(quaternionIndex, rotationIndex);
            gameManager.RayCastToFindOtherHexagons(quaternionIndex, children, false);
            turnCount++;
        }
        DestroySelecter();
        rotationIndex = 0;
        rotating = false;
    }

    void FixCollidersAndSpriteShortingLayer()
    {
        for (int i = 0; i < children.Length; i++)
        {
            children[i].GetComponent<PolygonCollider2D>().enabled = true;
            children[i].GetComponent<SpriteRenderer>().sortingOrder = 0;
            Debug.Log(children[i].name);
        }
    }

    void DeattachHexagonsAndRotateAroundRefPoint()
    {

        while (transform.childCount != hexagonChildStartIndex)
        {
            transform.GetChild(hexagonChildStartIndex).parent = null;
        }

        short degreeForSelectingOtherHexes = 60;

        if (endPos.x - startPos.x < 0)
            degreeForSelectingOtherHexes *= -1;
        Debug.LogWarning(degreeForSelectingOtherHexes);
        transform.RotateAround(QuaternionRefTransform.position, Vector3.forward, degreeForSelectingOtherHexes);
        FindHexagons();
        if (quaternionIndex == 5 && degreeForSelectingOtherHexes < 0)
        {
            quaternionIndex = 0;
        }

        else if (quaternionIndex == 0 && degreeForSelectingOtherHexes > 0)
        {
            quaternionIndex = 5;
        }
        else
        {
            if (degreeForSelectingOtherHexes > 0)
            {
                quaternionIndex--;
            }
            else
            {
                quaternionIndex++;
            }
        }
        rotationIndex = 0;
        SetChildOrder(quaternionIndex, rotationIndex);
    }

    void FindHexagons()
    {
        bool a = false;
        RaycastHit2D hit = Physics2D.Raycast(firstRefTransform.position, -Vector3.up, 0.1f);
        if (hit.collider != null && (hit.collider.gameObject.tag == "Hexagon" || hit.collider.gameObject.tag == "Bomb"))
        {
            hit.collider.transform.parent = gameObject.transform;
            children[0] = hit.collider.gameObject;
            fixedChildren[0] = hit.collider.gameObject;
            Debug.Log("Child: " + hit.collider.GetComponent<Renderer>().material.color + "to : " + fixedChildren[0].GetComponent<Renderer>().material.color + "0 added.");
        }
        else
            a = true;



        hit = Physics2D.Raycast(secondRefTransform.position, -Vector3.up, 0.1f);
        if (hit.collider != null && (hit.collider.gameObject.tag == "Hexagon" || hit.collider.gameObject.tag == "Bomb"))
        {
            hit.collider.transform.parent = gameObject.transform;
            children[1] = hit.collider.gameObject;
            fixedChildren[1] = hit.collider.gameObject;
            Debug.Log("Child " + hit.collider.GetComponent<Renderer>().material.color + "to : " + fixedChildren[1].GetComponent<Renderer>().material.color + 1 + " added.");

        }
        else
            a = true;


        hit = Physics2D.Raycast(thirdRefTransform.position, -Vector3.up, 0.1f);
        if (hit.collider != null && (hit.collider.gameObject.tag == "Hexagon" || hit.collider.gameObject.tag == "Bomb"))
        {
            hit.collider.transform.parent = gameObject.transform;
            children[2] = hit.collider.gameObject;
            fixedChildren[2] = hit.collider.gameObject;
            Debug.Log("Child " + hit.collider.GetComponent<Renderer>().material.color + "to : " + fixedChildren[2].GetComponent<Renderer>().material.color + "2 added.");
        }
        else
            a = true;

        if (a)
            DestroySelecter();

    }


    void SetChildOrder(int quaternionIndex_, int rotationIndex_)
    {
        GameObject[] temporary = new GameObject[3];
        Debug.Log("rgba quaternionIndex: " + quaternionIndex + "rotationIndex: " + rotationIndex);
        //children = fixedChildren;
        switch (quaternionIndex)
        {
            case 0:
                Debug.Log("rgba-----------------");
                break;
            case 1:
                for (int i = 0; i < temporary.Length; i++)
                {
                    temporary[i] = children[i];
                    Debug.Log(temporary[i].GetComponent<Renderer>().material.color);
                }
                children[0] = fixedChildren[2];
                children[1] = fixedChildren[1];
                children[2] = fixedChildren[0];
                break;
            case 2:
                for (int i = 0; i < temporary.Length; i++)
                {
                    temporary[i] = children[i];
                    Debug.Log(temporary[i].GetComponent<Renderer>().material.color);
                }
                children[0] = fixedChildren[2];
                children[1] = fixedChildren[0];
                children[2] = fixedChildren[1];
                break;
            case 3:
                for (int i = 0; i < temporary.Length; i++)
                {
                    temporary[i] = children[i];
                    Debug.Log(temporary[i].GetComponent<Renderer>().material.color);
                }
                children[0] = fixedChildren[1];
                children[1] = fixedChildren[0];
                children[2] = fixedChildren[2];
                break;
            case 4:
                for (int i = 0; i < temporary.Length; i++)
                {
                    temporary[i] = children[i];
                    Debug.Log(temporary[i].GetComponent<Renderer>().material.color);
                }
                children[0] = fixedChildren[1];
                children[1] = fixedChildren[2];
                children[2] = fixedChildren[0];
                break;
            case 5:

                children[0] = fixedChildren[0];
                children[1] = fixedChildren[2];
                children[2] = fixedChildren[1];
                for (int i = 0; i < temporary.Length; i++)
                {
                    temporary[i] = children[i];
                    Debug.Log(fixedChildren[i].GetComponent<Renderer>().material.color);
                }
                break;
        }


        for (int i = 0; i < temporary.Length; i++)
        {
            temporary[i] = children[i];
            Debug.Log(temporary[i].GetComponent<Renderer>().material.color);
        }


        children = children.OrderBy(child => -child.transform.position.x).ToArray();
        children = children.OrderBy(child => -child.transform.position.y).ToArray();

    }
}
