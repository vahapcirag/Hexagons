using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public byte horizontalGridCount;
    public byte verticalGridCount;
    public byte fixedColourCount;
    public byte colourCount;

    readonly float verticalDistance = .25f;
    readonly float horizontalDistance = .88f;

    float time;

    public int score;
    public int fixedScore;
    public int scorePerHex;

    Vector2 startPos = Vector2.zero;
    Vector2 endPos = Vector2.zero;

    public bool[] selectedColours;
    public bool[] possibilities;
    public bool firstPossibilityCheck = false;
    public bool isGameOver=false;
    public bool checking=false;

    [SerializeField] SelecterController selecterController;

    public GameObject[] colours;
    GameObject[] hexes;
    [SerializeField] List<GameObject> blowableHexagons = new List<GameObject>();
    [SerializeField] List<GameObject> hits;

    [SerializeField] Material[] materials;

    

    GameObject nullGameObject;

    

    public List<float> xPoses = new List<float>();
    public float yPosMin;
    public List<int> blowedCountOnXpose = new List<int>();
    int hexesIndex;

    Text scoreText;

    public void OnSettingsActivated()
    {
        fixedColourCount = 5;
        materials = Resources.LoadAll<Material>("Materials");
        selectedColours = new bool[8];
        colours = GameObject.FindGameObjectsWithTag("Colour");
        SetColoursRandomly();
    }

    private void Start()
    {
        score = 0;
        fixedScore = 1000;
        scorePerHex = 5;
        isGameOver = false;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (isGameOver || checking)
            return;

        if (SceneManager.GetActiveScene().buildIndex != 1)
            return;
        if (isGameOver || checking)
            return;
        time += Time.deltaTime;

        if (Input.touchCount != 0)
        {
            Touch touch = Input.GetTouch(0);



            if (touch.phase == TouchPhase.Began && time > 0.5f)
            {
                startPos = Camera.main.ScreenToWorldPoint(touch.position);
                //Debug.Log("Start Pos: " + startPos);
                time = 0;
            }


            else if (touch.phase == TouchPhase.Ended)
            {
                endPos = Camera.main.ScreenToWorldPoint(touch.position);

                Vector2 direction = endPos - startPos;
                //Debug.Log("End Pos: " + endPos + " || Direction:" + direction);
                //Debug.Log(direction.magnitude);
                //Debug.DrawLine(startPos, endPos, Color.red);


                if (direction.magnitude <= 0.5f)
                {
                    if (GameObject.FindGameObjectsWithTag("Selecter").Length >= 1)
                        GameObject.FindGameObjectsWithTag("Selecter")[0].GetComponent<SelecterController>().DestroySelecter();

                    //foreach (KeyValuePair<Vector3,GameObject> item in hexagons)
                    //{
                    //    if (endPos == (Vector2)item.Key)
                    //    {
                    //        Debug.Log("dfsfdsd");
                    //    }
                    //    else
                    //        Debug.Log(endPos + "     " + (Vector3)item.Key);
                    //}

                    RaycastHit2D hit = Physics2D.Raycast(endPos, -Vector2.up, 0.25f);
                    if (hit.collider != null)
                    {
                        Vector3 selecterPos = new Vector3(hit.collider.transform.position.x + -.15f, hit.collider.transform.position.y - 0.25f, hit.collider.transform.position.z);

                        GameObject selecter = Instantiate(Resources.Load<GameObject>("Selecter"), selecterPos, Quaternion.identity);
                        Debug.Log("Selecter Created");
                        selecterController = selecter.GetComponent<SelecterController>();
                    }
                }
                else if (!selecterController.rotating)
                {
                    selecterController.GetInputInformation(startPos, endPos, direction.magnitude);
                }
            }
        }


    }


    public void SetColour(int i)
    {
        GameObject thick = colours[i].transform.GetChild(0).gameObject;

        if (colourCount == fixedColourCount && thick.GetComponent<Image>().enabled)
            return;

        if (thick.GetComponent<Image>().enabled)
            colourCount--;
        else
            colourCount++;

        thick.GetComponent<Image>().enabled = !thick.GetComponent<Image>().enabled;
        selectedColours[i] = !selectedColours[i];
    }

    public void SetColoursRandomly()
    {
        for (byte i = 0; i < colours.Length; i++) // Restoring everything because the function is recursive.
        {
            GameObject thick = colours[i].transform.GetChild(0).gameObject;
            thick.GetComponent<Image>().enabled = false;
            selectedColours[i] = false;
        }

        colourCount = 0;
        for (byte i = 0; i < colours.Length; i++)
        {


            if (colourCount == fixedColourCount)
                break;

            byte rand = (byte)Random.Range(0, 2);

            GameObject thick = colours[i].transform.GetChild(0).gameObject;

            if (rand == 1)
            {
                thick.GetComponent<Image>().enabled = true;
                selectedColours[i] = true;
                colourCount++;
            }
        }

        if (colourCount < fixedColourCount)
            SetColoursRandomly();
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene(1);
    }

    private void OnLevelWasLoaded(int level)
    {
        if (level != 1)
            return;
        scoreText = GameObject.FindGameObjectWithTag("Score").GetComponent<Text>();
        StartCoroutine(GenerateGrids());
        nullGameObject = GameObject.FindGameObjectWithTag("Null");
    }

    IEnumerator GenerateGrids()
    {
        checking = true;

        for (int i = -(verticalGridCount / 2); i <= (verticalGridCount / 2); i++)
        {
            for (int j = -(horizontalGridCount / 2); j < (horizontalGridCount / 2); j++)
            {
                Vector3 pos;
                if (i % 2 == 0)
                    pos = new Vector3(horizontalDistance / 2 + horizontalDistance * j, verticalDistance * i, 0f);
                else
                    pos = new Vector3(horizontalDistance * j, verticalDistance * i, 0f);

                if (i <= -(verticalGridCount / 2) + 1)
                {
                    Instantiate(Resources.Load<GameObject>("Obstacle"), pos - new Vector3(0f, 0.5f, 0f), Quaternion.identity);
                }

                if (i >= (verticalGridCount / 2) - 1)
                {
                    Instantiate(Resources.Load<GameObject>("Obstacle"), pos + new Vector3(0f, 0.5f, 0f), Quaternion.identity);
                }

                //if (i <= -(verticalGridCount / 2) + 1)
                //{
                //    xPoses.Add(pos.x);
                //    blowedCountOnXpose.Add(0);
                //}

                if ((i == -(verticalGridCount / 2)) && (j == -(horizontalGridCount / 2)))
                {
                    yPosMin = pos.y;
                }

                SetHexColorAndSpawn(pos, "Hexagon");
                yield return new WaitForSeconds(0.03f);
            }
        }
        StartCoroutine(IsGameOver());
    }

    public GameObject SetHexColorAndSpawn(Vector3 pos, string a)
    {
        GameObject hexagon = Instantiate(Resources.Load<GameObject>(a), pos, Quaternion.identity);
        int k;
        while (true)
        {
            k = Random.Range(0, colours.Length);
            if (selectedColours[k])
                break;
        }

        hexagon.GetComponent<Renderer>().material = Resources.LoadAll<Material>("Materials")[k];

        return hexagon;
    }

    void CreateObstacle(Vector3 pos)
    {
        Vector3 down = new Vector3(0f, -0.27f, 0f);
        Instantiate(Resources.Load<GameObject>("Obstacle"), pos + down, Quaternion.identity);
    }

    public void RayCastToFindOtherHexagons(int quaternionIndex_, GameObject[] children, bool checkCaller)
    {
        if (isGameOver)
            return;
        hits = new List<GameObject>();

        for (int i = 0; i < children.Length; i++)
        {

            children[i].GetComponent<PolygonCollider2D>().enabled = false;
            for (int j = 0; j < 3; j++)
            {
                Vector2 rayDirection = Vector2.zero;
                switch (i)
                {
                    case 0:
                        switch (j)
                        {
                            case 0:
                                rayDirection = RayDirections.UpRight;
                                break;
                            case 1:
                                rayDirection = RayDirections.Up;
                                break;
                            case 2:
                                rayDirection = RayDirections.UpLeft;
                                break;
                        }
                        break;
                    case 1:
                        switch (j)
                        {
                            case 0:
                                rayDirection = RayDirections.UpLeft;
                                break;
                            case 1:
                                rayDirection = RayDirections.DownLeft;
                                break;
                            case 2:
                                rayDirection = RayDirections.Down;
                                break;
                        }
                        break;
                    case 2:
                        switch (j)
                        {
                            case 0:
                                rayDirection = RayDirections.Down;
                                break;
                            case 1:
                                rayDirection = RayDirections.DownRight;
                                break;
                            case 2:

                                rayDirection = RayDirections.UpRight;
                                break;
                        }
                        break;
                }
                if (quaternionIndex_ % 2 != 0)
                    rayDirection *= new Vector2(-1f, 1f);
                Debug.Log(rayDirection);
                Debug.DrawLine(children[i].transform.position, children[i].transform.position + (Vector3)rayDirection, Color.red);
                RaycastHit2D hit = Physics2D.Raycast(children[i].transform.position, rayDirection, 0.5f);
                if (hit.collider != null)
                    hits.Add(hit.collider.transform.gameObject);
                else
                    hits.Add(nullGameObject);




            }
            children[i].GetComponent<PolygonCollider2D>().enabled = true;
        }
        SetBlowableHexagons(children, checkCaller);
    }

    public void SetBlowableHexagons(GameObject[] children, bool checkCaller)
    {
        blowableHexagons = new List<GameObject>();
        int index;

        int indexLastHit;

        for (int i = 0; i < 8; i++)
        {
            if (i >= 0 && i < 2)
            {
                index = 0;
                indexLastHit = 1;
            }
            else if (i >= 2 && i < 5)
            {
                indexLastHit = 4;
                index = 1;
            }
            else
            {
                indexLastHit = 7;
                index = 2;
            }


            if (true)
            {
                if ((hits[i] != nullGameObject) && (hits[i + 1] != nullGameObject) && (hits[i].tag != "Obstacle") && (hits[i + 1].tag != "Obstacle"))
                {
                    Color child = children[index].GetComponent<Renderer>().material.color;

                    if (i == 0)
                    {
                        if (hits[hits.Count - 1] != nullGameObject && hits[hits.Count - 1].GetComponent<Renderer>().material.color == hits[0].GetComponent<Renderer>().material.color)
                        {
                            if (child == hits[hits.Count - 1].GetComponent<Renderer>().material.color)
                            {
                                blowableHexagons.Add(hits[0]);
                                blowableHexagons.Add(hits[5]);
                                blowableHexagons.Add(children[index]);
                                Debug.Log("Added in zero situation");
                            }
                            else
                            {

                                Debug.Log("Other hexes mats are not same with mine" + "index:" + i);
                            }
                        }
                        else
                        {

                            Debug.Log("Other hexes mats are not same" + "index:" + i);
                        }

                    }
                    int k = index + 1;
                    Color first = hits[i].GetComponent<Renderer>().material.color;
                    Color second = hits[i + 1].GetComponent<Renderer>().material.color;
                    if ((first == second) && (second == child))
                    {

                        if (i == indexLastHit && i != 7 && second == children[k].GetComponent<Renderer>().material.color)
                        {
                            blowableHexagons.Add(children[k]);
                            Debug.Log("Added nextChild in sameChild sameOther situation" + "index" + i + "childIndex" + index);

                        }
                        else if (i == indexLastHit && i == 7 && second == children[0].GetComponent<Renderer>().material.color)
                        {
                            blowableHexagons.Add(children[0]);

                            Debug.Log("Added nextChild" + "index" + i + "childIndex" + index);
                        }

                        blowableHexagons.Add(children[index]);
                        blowableHexagons.Add(hits[i]);
                        blowableHexagons.Add(hits[i + 1]);

                        Debug.Log("Added normally" + "index" + i + "childIndex" + index);
                    }
                    else if (first != second && i != 7 && i == indexLastHit && second == child && second == children[k].GetComponent<Renderer>().material.color)
                    {
                        blowableHexagons.Add(children[index]);
                        blowableHexagons.Add(children[k]);
                        blowableHexagons.Add(hits[i + 1]);



                        Debug.Log("Added nextChild in sameChild differentOther situation" + "index" + i + "childIndex" + index);

                    }
                    else if (first != second && i == 7 && i == indexLastHit && second == child && second == children[0].GetComponent<Renderer>().material.color)
                    {
                        blowableHexagons.Add(children[index]);
                        blowableHexagons.Add(children[0]);
                        blowableHexagons.Add(hits[i + 1]);

                        Debug.Log("Added nextChild in sameChild sameOther situation" + "index" + i + "childIndex" + index);
                    }


                    else
                    {
                        Debug.Log((first != second) + " " + (i == 7) + " " + (second == child) + " " + (second == children[1].GetComponent<Renderer>().material.color));
                        Debug.Log("Other hexes mats are not same " + "first: " + first + " second: " + second + " child: " + child + "index:" + i);
                    }



                }
                else
                {
                    Debug.Log("There is a null object" + index);

                }
            }
        }
        if (checkCaller)
        {
            for (int i = 0; i < blowableHexagons.Count; i++)
            {
                for (int j = blowableHexagons.Count - 1; j <= 0; j--)
                {
                    if ((j <= blowableHexagons.Count) && (i < blowableHexagons.Count) && (i != j))
                        blowableHexagons.Remove(blowableHexagons[j]);
                }
            }

            if (blowableHexagons.Count > 2)
            {
                for (int i = 0; i < children.Length; i++)
                {
                    if (children[i].gameObject.name == null)
                        return;
                }

                GameObject.FindGameObjectWithTag("Selecter").GetComponent<SelecterController>().DestroySelecter();

                for (int i = 0; i < blowableHexagons.Count; i++)
                {
                    score += scorePerHex;
                    scoreText.text = "Score: " + score.ToString();
                    Destroy(blowableHexagons[i]);
                }


               


                StartCoroutine(I());

                if(GameObject.FindGameObjectsWithTag("Bomb").Length == 1)
                {
                    GameObject.FindGameObjectsWithTag("Bomb")[0].GetComponent<BombManager>().SetBombCounter();
                }
            }
        }
        else
        {

            if (blowableHexagons.Count > 2)
                possibilities[hexesIndex] = true;
            checking = false;
        }

    }

    public IEnumerator I()
    {
        checking = true;
        yield return new WaitForSeconds(1f);

        foreach (GameObject item in GameObject.FindGameObjectsWithTag("Hexagon"))
        {
            item.GetComponent<PolygonCollider2D>().enabled = true;
        }

        if (GameObject.FindGameObjectsWithTag("Bomb").Length != 0)
            GameObject.FindGameObjectsWithTag("Bomb")[0].GetComponent<PolygonCollider2D>().enabled = true;

        StartCoroutine(FallHexes(verticalGridCount, yPosMin, 0));
    }

    public IEnumerator FallHexes(byte verticalGridCount, float yPosMax, float fixedYposMax)
    {

        GameObject[] hexes = GameObject.FindGameObjectsWithTag("Hexagon");

        if (GameObject.FindGameObjectsWithTag("Bomb").Length != 0)
            StartCoroutine(GameObject.FindGameObjectsWithTag("Bomb")[0].GetComponent<BombManager>().Fall());
        for (int j = 0; j < verticalGridCount; j++)
        {
            yPosMax = fixedYposMax;
            for (int k = 0; k < verticalGridCount + 1; k++)
            {
                foreach (GameObject item in hexes)
                {
                    item.GetComponent<HexagonController>().CheckDown();
                }
               
                yPosMax += 0.25f;
            }
            yield return new WaitForSeconds(0.15f);
            
        }
        if (GameObject.FindGameObjectsWithTag("Bomb").Length != 0)
            StartCoroutine(GameObject.FindGameObjectsWithTag("Bomb")[0].GetComponent<BombManager>().Fall());
        Debug.LogWarning("Done");
        GameObject[] go = GameObject.FindGameObjectsWithTag("Obstacle");

        bool a = false;

        foreach (GameObject item in go)
        {
            ObstacleController obstacleController = item.GetComponent<ObstacleController>();
            if (obstacleController.RayCast_().collider==null && score>fixedScore && !a &&GameObject.FindGameObjectsWithTag("Bomb").Length==0)
            {
                fixedScore += fixedScore;
                obstacleController.SpawnBomb();
                a = true;
                yield return new WaitForSeconds(0.5f);
            }
           
            obstacleController.Spawn();

        }
        StartCoroutine(IsGameOver());
    }

    public IEnumerator IsGameOver()
    {
        yield return new WaitForSeconds(2f);
        hexes = GameObject.FindGameObjectsWithTag("Hexagon");
        possibilities = new bool[hexes.Length];

        for (hexesIndex = horizontalGridCount*2; hexesIndex < hexes.Length; hexesIndex++)
        {
            Vector3 selecterPos = new Vector3(hexes[hexesIndex].transform.position.x + -.15f, hexes[hexesIndex].transform.position.y - 0.25f, 0);
            if (GameObject.FindGameObjectsWithTag("Group").Length >= 1)
                GameObject.FindGameObjectsWithTag("Group")[0].GetComponent<SelecterController>().DestroySelecter();
            GameObject selecter = Instantiate(Resources.Load<GameObject>("Group"), selecterPos, Quaternion.identity);

            if (hexesIndex > 0)
                if (possibilities[hexesIndex - 1] == true)
                {
                    break;
                }
            
            for (int i = 0; i < possibilities.Length; i++)
            {
                if (possibilities[i])
                    break;
            }
            yield return new WaitForSeconds(1.6f);
        }


        for (int i = 0; i < possibilities.Length; i++)
        {
            if (possibilities[i])
                break;
            if (i == possibilities.Length)
                isGameOver = true;
        }
        checking = false;
    }
}



