using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class ShowHidePauseMenu : MonoBehaviour
{

    private GameMaster master;

    void Start()
    {
        master = GameObject.Find("Master").GetComponent<GameMaster>(); 
    }
	
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            master.ShowHidePausemenu();
        }
    }
}
