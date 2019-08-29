using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    public GameObject gameOverMenu;
    GameManager gameManager;

    public void OnMainMenuClicked()
    {
        Destroy(gameManager.gameObject);
        SceneManager.LoadScene(0);
        
    }

    private void Update()
    {
        if (gameManager.isGameOver)
            gameOverMenu.SetActive(true);
            
    }
    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }
}
