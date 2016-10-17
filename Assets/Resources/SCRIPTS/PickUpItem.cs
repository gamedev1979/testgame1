using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;


public class PickUpItem : MonoBehaviour
{
    public GameObject InventoryContentGUI;
    private Transform item;
    private CharacterController charCtrl;
    private Camera myCam;
    private bool itemFound;
    private InventoryManager inventoryManager;
    private GameMaster gameMaster;

    // Use this for initialization
    void Start()
    {
        charCtrl = (CharacterController)gameObject.GetComponent<CharacterController>();
       
        InventoryContentGUI = GameObject.FindGameObjectWithTag("InventoryContentGUI");

        myCam = transform.GetComponentInChildren<Camera>();

        inventoryManager = GameObject.Find("Master").GetComponent<InventoryManager>();
        gameMaster = GameObject.Find("Master").GetComponent<GameMaster>();

        itemFound = false;

        item = null;
    }
	
    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Ray ray = myCam.ScreenPointToRay(Input.mousePosition);
        //Vector3 targetPosition = myCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z));
        // detect = Physics.SphereCast(transform.position, charCtrl.height / 2, transform.forward, out hit, 5f);
        //(Physics.SphereCast(transform.position, charCtrl.height / 2, transform.forward, out hit, 5f)
        //Physics.SphereCast(ray, charCtrl.height, out hit, 10f)
        if ((Physics.SphereCast(transform.position, charCtrl.height / 2, transform.forward, out hit, 5f) ||
            Physics.SphereCast(ray, charCtrl.height / 2, out hit, 5f)) &&
            hit.transform.gameObject.tag.Equals("Item"))
        {
            if (hit.distance <= 2f)
            {
                item = hit.transform;

                Transform itemSlot = item.FindChild(item.name);
                ItemAttribute itemAttribute = (ItemAttribute)itemSlot.GetComponent<ItemAttribute>();

                gameMaster.ShowHint(itemAttribute.Description + " - Raccogliere (R)", 
                    gameMaster.getHintStyle().FoundItemTextSize,
                    gameMaster.getHintStyle().FoundItemStyle, 
                    gameMaster.getHintStyle().FoundItemColor);

                if (Input.GetButtonDown("Raccogliere"))
                {
                    if (itemFound == false)
                    {
                        gameMaster.HideHint();
                        itemFound = true;
                        AddSlotToInventory(itemAttribute);
                    }    
                }
            }
            else
            {
                gameMaster.ShowHint("Qui c'é qualcosa...", 
                    gameMaster.getHintStyle().DefaultTextSize,
                    gameMaster.getHintStyle().DefaultStyle, 
                    gameMaster.getHintStyle().DefaultColor);
            }
        }
        else
        {
            gameMaster.HideHint();
        }

    }


   

    void AddSlotToInventory(ItemAttribute itemAttribute)
    {   
        if (itemAttribute != null)
        {
            Debug.Log("canPickUpItem(itemAttribute): " + inventoryManager.canPickUpItem(itemAttribute));
            if (inventoryManager.canPickUpItem(itemAttribute))
            {
                ((AudioSource)item.GetComponent<AudioSource>()).Play();

                inventoryManager.CreateItemSlot(itemAttribute, false);
                inventoryManager.SetItemButtonHandler(itemAttribute);

                DestroyObject(item.gameObject);

                itemFound = false;
            }
        }
        else
        {
            Debug.Log("Non trovo l'oggetto!");
        }
    }
}
