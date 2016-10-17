using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class PauseMenuManager : MonoBehaviour
{

   
    private GameObject fpsController;
    private  GameMaster masterScript;
    private InventoryManager scriptInventoryManager;

    void Start()
    {
        fpsController = GameObject.Find("FPSController");
        masterScript = transform.GetComponent<GameMaster>();

        scriptInventoryManager = transform.GetComponent<InventoryManager>();
    }


    public void ResumeGame()
    {
        Debug.Log("ResumeGame");
        masterScript.ShowHidePausemenu();
    }

    public void SaveGame()
    {
        //per ora salva solo gli oggetti dell'invnetario sul DB
        scriptInventoryManager.SaveItems();
        masterScript.ShowHidePausemenu();
    }


    public void LoadGame()
    {
        scriptInventoryManager.LoadItems();
        masterScript.ShowHidePausemenu();
    }


    public void ExitGame()
    {
        Application.Quit();
    }



}
