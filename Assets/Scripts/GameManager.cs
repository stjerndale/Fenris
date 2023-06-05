using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.Experimental.GraphView.GraphView;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    bool saveOnSceneExit;
    [SerializeField]
    bool loadOnSceneEnter;

    [SerializeField]
    GameObject grid;
    GridHandler gridHandler;

    private SaveToJson saver;


    [SerializeField]
    private GameInformation gameInfo;
    [SerializeField]
    private SceneLoader sceneLoader;

    // Start is called before the first frame update
    void Start()
    {
        gridHandler = grid.GetComponent<GridHandler>();
        saver = new SaveToJson();
        saver.LoadGameInformationFromJson(); // Load Game Information at start
        if (loadOnSceneEnter)
        {
            StartCoroutine(LoadMap()); // Load Map
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            gridHandler.SaveMap(saver); // Save Map
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            saver.LoadMapFromJson(grid.transform); // Load Map
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            saver.SaveGameInformationToJson(); // Save Game Information
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            gameInfo.Reset(); // Reset Game Information
        }


        if (Input.GetKeyDown(KeyCode.N)) // Change scene
        {
            switch (SceneManager.GetActiveScene().buildIndex)
            {
                case 0:
                    sceneLoader.LoadScene(1);
                    break;
                case 1:
                    sceneLoader.LoadScene(0);
                    break;
            }
            if(saveOnSceneExit)
            {
                gridHandler.SaveMap(saver); // Save Map
            }
        }
    }

    private IEnumerator LoadMap()
    {
        yield return null;
        saver.LoadMapFromJson(grid.transform);
    }
}
