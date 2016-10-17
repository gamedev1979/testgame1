using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Mono.Data.Sqlite;
using System.Data;
using System;
using UnityStandardAssets.Characters.FirstPerson;

public class InventoryManager : MonoBehaviour
{

    public int ItemCount;

    public  GameObject InventoryContentGUI;

    private int MAX_ITEM = 8;

    private GameMaster gameMaster;

    GameObject itemSlot;

    private GameObject ItemMenuGUI;
    private GameObject ItemMenuContentGUI;

    private GameObject EquippedContentGUI;

    private CanvasGroup itemMenuContentCanvasGroup;

    private ItemAttribute senderAttribute;

    private GameObject senderItem;

    private GameObject equippedItem;

    private GameObject player;

    private bool combineState;

    private Text avvisiInventario;

    public GameObject EquipGUI;
    public GameObject UseGUI;
    public GameObject CombineGUI;
    private GameObject InspectGUI;
    public GameObject LeaveGUI;


  
    void Start()
    {
        gameMaster = (GameMaster)transform.GetComponent<GameMaster>();

        player = GameObject.FindGameObjectWithTag("Player");

        InventoryContentGUI = GameObject.Find("InventoryContentGUI");
       
        ItemMenuGUI = GameObject.Find("ItemMenuGUI");
        itemMenuContentCanvasGroup = ItemMenuGUI.GetComponent<CanvasGroup>();
        ItemMenuContentGUI = GameObject.Find("ItemMenuContentGUI");

        avvisiInventario = GameObject.Find("ToolTipInventoryGUI").GetComponent<Text>();

        EquippedContentGUI = GameObject.Find("EquippedContentGUI");

        InspectGUI = GameObject.Find("InspectGUI");

        equippedItem = null;

        ItemCount = 0;

        combineState = false;

        /*
        btnEquip = GameObject.Find("EquipGUI");
        btnUse = GameObject.Find("UseGUI");
        btnCombine = GameObject.Find("CombineGUI");
        btnDelete = GameObject.Find("LeaveGUI");
        */
    }

   
    void Update()
    {
        //calcola gli item nellínventario
        ItemCount = InventoryContentGUI.transform.childCount;
       
    }


    #region Utility Functions

    public bool canPickUpItem(ItemAttribute itemAttribute)
    {
        bool ret = false;

        //Se l'oggetto non è stackable posso solo verificare che ci sia spazio nell'inventario
        if (itemAttribute.Stackable == false)
        {
            ret = !isInventoryFull();
        }
        else
        {      
            //verifico se è già presente un item dello stesso tipo 
            //che sia stackable e a cui possa aggiungere la qta senza superare qtaMax

            List<GameObject> itemList = getItemsFromInventory(itemAttribute.Name);

            if (itemList.Count > 0)
            {
                //Debug.Log("canPickUpItem - itemList.Count " + itemList.Count);
                //ciclo sugli oggetti dello stesso tipo. 
                int i = 0;

                while (i < itemList.Count && ret == false)
                {
                    GameObject itemSameType = itemList[i];
                    //aggiorno il campo qta dell'oggetto nell'inventario
                    ItemAttribute myAttr = itemSameType.GetComponent<ItemAttribute>();

                    int max = myAttr.Qta + itemAttribute.Qta;

                    if (max <= myAttr.MaxQta)
                    {
                        ret = true;
                    }

                    i++;
                }

                if (ret == false)
                {
                    ret = !isInventoryFull();
                }

            }
            else //è il primo oggetto di quel tipo che trovo. Devo solo verificare che l'inventario non sia pieno
            {
                ret = !isInventoryFull();
            }
        }

        return ret;
    }

    public bool isInventoryFull()
    {
        Debug.Log("isInventoryFull : " + string.Format("{0}", InventoryContentGUI.transform.childCount < MAX_ITEM)); 
        return (InventoryContentGUI.transform.childCount >= MAX_ITEM);
    }

    private bool isItemEquipped()
    {
        //Debug.Log(EquippedContentGUI.transform.childCount == 1);
        return (EquippedContentGUI.transform.childCount == 1);
    }

    private GameObject getItemFromInventory(String itemName)
    {
        Transform found = InventoryContentGUI.transform.FindChild(itemName);
        if (found != null)
            return found.gameObject;
        else
            return null;
    }

    private GameObject getItemFromInventoryBySlotNumber(int slotNumber)
    {
        GameObject found = null;

        foreach (Transform item in InventoryContentGUI.transform)
        {
            ItemAttribute attr = item.GetComponent<ItemAttribute>();

            Debug.Log("slotNumber " + slotNumber + " attr.InventoryPosition " + attr.InventoryPosition);
            if (attr.InventoryPosition == slotNumber)
            {
                found = item.gameObject;
            }
        }
        return found;
    }

    private List<GameObject> getItemsFromInventory(String itemName)
    {
        List<GameObject> listaItems = new List<GameObject>();

        foreach (Transform item in InventoryContentGUI.transform)
        {
            if (item.gameObject.name == itemName)
            {
                listaItems.Add(item.gameObject);
            }
        }

        return listaItems;
    }

    private List<GameObject> getItemsFromInventoryBySlotNumber(int slotNumber)
    {
        List<GameObject> listaItems = new List<GameObject>();

        foreach (Transform item in InventoryContentGUI.transform)
        {
            ItemAttribute attr = item.GetComponent<ItemAttribute>();

           
            if (attr.InventoryPosition == slotNumber)
            {
                listaItems.Add(item.gameObject);
            }
        }
        return listaItems;
    }

    private void DeleteAllItemsFromInventoryGUI()
    {
        foreach (Transform child in InventoryContentGUI.transform)
        {
            DestroyObject(child.gameObject);
        }
    }

    private void DeleteAllItemsFromEquippedGUI()
    {
        //svuoto il contenitore
        foreach (Transform child in EquippedContentGUI.transform)
        {
            DestroyImmediate(child.gameObject);
        }
    }

    private void DisableItemSlotButton(Transform item)
    {
        item.gameObject.GetComponent<Button>().interactable = false;
    }

    private void EnableItemSlotButton(Transform item)
    {
        item.gameObject.GetComponent<Button>().interactable = true;
    }

    private ItemAttribute  CopyAttribute(ItemAttribute attrOrigin, ItemAttribute attrDest)
    {
        attrDest.ObjectTag = attrOrigin.ObjectTag;
        attrDest.Name = attrOrigin.Name;
        attrDest.Type = attrOrigin.Type;
        attrDest.Description = attrOrigin.Description;
        attrDest.Qta = attrOrigin.Qta;
        attrDest.MaxQta = attrOrigin.MaxQta;
        attrDest.Stackable = attrOrigin.Stackable;
        attrDest.Usable = attrOrigin.Usable;
        attrDest.Equipable = attrOrigin.Equipable;
        attrDest.Destroyable = attrOrigin.Destroyable;
        attrDest.Note = attrOrigin.Note;
        attrDest.ImagePath = attrOrigin.ImagePath;
        attrDest.CombineWith = attrOrigin.CombineWith;
        attrDest.InventoryPosition = attrOrigin.InventoryPosition;

        return attrDest;
    }


    #endregion

    public void LoadItems()
    {
        //svuota prima di tutto l'inventario attuale
        DeleteAllItemsFromInventoryGUI();

        List<ItemAttribute> listAttribute = gameMaster.LoadAllItemFromDB();

        //Debug.Log("Numero attr restituiti dal db: " + listAttribute.Count);

        foreach (ItemAttribute itemAttr in listAttribute)
        {
            //Debug.Log("Ciclo for listAttribute - itemAttr.Name: " + itemAttr.Name);
            CreateItemSlot(itemAttr, true);
            SetItemButtonHandler(itemAttr);
        }

        //SetAllItemsButtonHandler();
    }



    public void CreateItemSlot(ItemAttribute itemAttr, bool dbLoad)
    {
        if (dbLoad)
        {
            //Debug.Log("CreateItemSlot chiamata da DB - itemAttr.Name itemAttr.Qta: " + itemAttr.Name + " " + itemAttr.Qta);
            CreateItemFromPrefab(itemAttr);
        }
        else
        {
            if (itemAttr.Stackable == false)
            {
                CreateItemFromPrefab(itemAttr);
            }
            else
            {
                //Cerco l'oggetto dello stesso tipo e incremento la qta senza creare un nuovo oggetto
                List<GameObject> itemSameTypeList = getItemsFromInventory(itemAttr.Name);

                if (itemSameTypeList.Count > 0)
                {
                    bool found = false;
                    int i = 0;

                    while (i < itemSameTypeList.Count && found == false)
                    {
                        GameObject itemSameType = itemSameTypeList[i];
                        //aggiorno il campo qta dell'oggetto nell'inventario
                        ItemAttribute myAttr = itemSameType.GetComponent<ItemAttribute>();

                        int max = myAttr.Qta + itemAttr.Qta;

                        if (max <= myAttr.MaxQta)
                        {
                            myAttr.Qta = max;
                            found = true;
                        }

                        i++;
                    }

                    if (found == false)
                    {
                        CreateItemFromPrefab(itemAttr);
                    }
                }
                else //è il primo oggetto. Lo creo da zero
                { 
                    CreateItemFromPrefab(itemAttr);
                }  
            }
        }
    }


    private void CreateItemFromPrefab(ItemAttribute itemAttr)
    {
        GameObject prefabObj = (GameObject)Resources.Load(GameMaster.PREFABS_ITEM_PATH + itemAttr.Name);
        GameObject itemContainer = new GameObject();
        GameObject itemCopy = (GameObject)Instantiate(prefabObj, itemContainer.transform);
        Transform itemChild = itemCopy.transform.FindChild(itemAttr.Name);
        ItemAttribute attr = itemChild.gameObject.GetComponent<ItemAttribute>();

        //sostituisco l'ItemAttribute predefinito con quello in ingresso
        DestroyImmediate(attr);

        ItemAttribute newAttr = (ItemAttribute)itemChild.gameObject.AddComponent(itemAttr.GetType());

        newAttr = CopyAttribute(itemAttr, newAttr);

        itemChild.SetParent(InventoryContentGUI.transform, true);

        GameObject.DestroyObject(itemContainer);
    }



    public void SetAllItemsButtonHandler()
    {
        ItemAttribute itemAttribute = null;

        foreach (Transform child in InventoryContentGUI.transform)
        {
            itemAttribute = (ItemAttribute)child.gameObject.GetComponent<ItemAttribute>();

            //Collego l'evento onclick al tasto
            Button btn = (Button)child.gameObject.GetComponent<Button>();

            btn.onClick.AddListener(() => ShowItemMenu(itemAttribute));
        }
    }


    public void SetItemButtonHandler(ItemAttribute itemAttribute)
    {
        //Debug.Log("SetItemButtonHandler chiamata per " + itemAttribute.Name);
       
        List<GameObject> itemList = getItemsFromInventoryBySlotNumber(itemAttribute.InventoryPosition);

        if (itemList.Count > 0)
        {
            foreach (GameObject item in itemList)
            {
                //Collego l'evento onclick al tasto
                Button btn = (Button)item.GetComponent<Button>();

                btn.onClick.RemoveAllListeners();

                btn.onClick.AddListener(() => ShowItemMenu(itemAttribute));
            }
        }
        // Debug.Log("SetItemButtonHandler terminata per " + itemAttribute.Name);
    }

  
    public void SaveItems()
    {
        ItemAttribute itemAttribute = null;
        List<ItemAttribute> listaItemAttribute = new List<ItemAttribute>();
        int count = 1;
       
        foreach (Transform child in InventoryContentGUI.transform)
        {
            itemAttribute = (ItemAttribute)child.GetComponent<ItemAttribute>();
            //assegna l'attuale poisizione nell'inventario ad ogni item
            itemAttribute.InventoryPosition = count;
            count++;
            listaItemAttribute.Add(itemAttribute);
        }

        if (listaItemAttribute.Count > 0)
        {
            gameMaster.SaveAllItemToDB(listaItemAttribute);
        }
    }


    public void ShowItemMenu(ItemAttribute itemAttribute)
    {
        if (combineState)
        {
            CombineItems(itemAttribute); 
           
        }
        else
        {
            //resetta il tooltip dellínventario
            avvisiInventario.text = "";

            gameMaster.HideItemMenu();

            gameMaster.ShowItemMenu();

            senderAttribute = itemAttribute;

            senderItem = getItemFromInventoryBySlotNumber(senderAttribute.InventoryPosition);

            //Configuro i tasti in base all'itemAttribute


           
            EquipGUI.SetActive(senderAttribute.Equipable);
            UseGUI.SetActive(senderAttribute.Usable);
           
            if (senderAttribute.CombineWith.Equals(ItemAttribute.TypeItem.Nothing))
            {
                CombineGUI.SetActive(false);
            }
            else
            {
                CombineGUI.SetActive(true);
            }

            LeaveGUI.SetActive(senderAttribute.Destroyable);

            gameMaster.ShowItemMenu();
        }
    }



    #region Menu Item functions

    private void CombineItems(ItemAttribute itemAttribute)
    {
        Debug.Log("combineState " + combineState); 
        //prendo i 2 oggetti da combinare: l'origine è senderAttribute mentre la destinazione viene da itemAttribute

        GameObject itemOrig = senderItem;
        GameObject itemDest = getItemFromInventoryBySlotNumber(itemAttribute.InventoryPosition);


        ItemAttribute attrOrig = itemOrig.GetComponent<ItemAttribute>();
        ItemAttribute attrDest = itemDest.GetComponent<ItemAttribute>();

        //TODO: Controlli sulle qta e sulla effettiva possibilità di combinare i 2 item (type=combineWith oppure stackable)
        bool combinato = false;

        if (attrOrig.Type == attrDest.CombineWith || attrOrig.Type == attrDest.Type)
        {
            //inizio controlli
            //dest può accettare una qta <= dest.MaxQta

            //se dest non è pieno calcolo quanto devo scalare da origin ed aggiungere a dest

            Debug.Log("attrOrig.Qta: " + attrOrig.Qta);
            Debug.Log("attrDest.Qta: " + attrDest.Qta);
            if (attrDest.Qta < attrDest.MaxQta)
            {
                int qtaDiff = attrDest.MaxQta - attrDest.Qta;
                Debug.Log("qtaDiff: " + qtaDiff);
                //orig deve possedere una qta>= a quella che serve dest
                if (attrOrig.Qta >= qtaDiff)
                {
                    attrDest.Qta += qtaDiff;
                    attrOrig.Qta -= qtaDiff;

                }
                else
                {
                    attrDest.Qta += attrOrig.Qta;
                    attrOrig.Qta = 0;
                }

                combinato = true;
                gameMaster.HideItemMenu();

                //distrugge l'oggetto se la qta ==0 e se è stackable
                if (attrOrig.Qta <= 0 && attrOrig.Stackable)
                {
                    DestroyObject(itemOrig);
                }
            }
        }

        if (combinato == false)
        {
            avvisiInventario.text = "Non si possono combinare...";
            avvisiInventario.fontSize = 23;
            avvisiInventario.fontStyle = FontStyle.Normal;
            avvisiInventario.color = Color.red;
        }    


        //quando ha finito risetto combineState a false e tolgo l'outline dal sender e riabilito tutti gli item
        combineState = false;

        ((Outline)itemOrig.GetComponent<Outline>()).enabled = false;

        foreach (Transform item in InventoryContentGUI.transform)
        {
            EnableItemSlotButton(item);
        }

    }




    public void EquipButtonOnClick()
    {
        DeleteAllItemsFromEquippedGUI();

        //prendo l'oggetto da copiare 
        GameObject original = getItemFromInventory(senderAttribute.Name);

        equippedItem = (GameObject)Instantiate(original, EquippedContentGUI.transform);

        RectTransform itemPos = equippedItem.GetComponent<RectTransform>();

        itemPos.localPosition = Vector3.zero;

        //collego l'evento all'item per rimuoverlo dall'oggetto EquippedContentGUI
        Button btn = equippedItem.GetComponent<Button>();
       
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => RemoveEquippedItem());

        gameMaster.UpdateEquippedStatus(true);
        gameMaster.HideItemMenu();
    }

    public void UseButtonOnClick()
    {
        
    }

    public void CombineButtonOnClick()
    {
        GameObject selectedItem = getItemFromInventoryBySlotNumber(senderAttribute.InventoryPosition);

        //evidenzio l'oggetto selezionato
        Outline outLine = selectedItem.GetComponent<Outline>();
        outLine.enabled = true;

        //Disabilito tutti gli oggetti con CombineWith = Nothing 
        //o che non hanno Type = CombineWith dell'oggetto selezionato e che non sono stackable

        foreach (Transform item in InventoryContentGUI.transform)
        {
            ItemAttribute itemAttr = item.GetComponent<ItemAttribute>();
            if (itemAttr.CombineWith == ItemAttribute.TypeItem.Nothing ||
                (itemAttr.Type != senderAttribute.CombineWith &&
                itemAttr.Type != senderAttribute.Type))
            {
                DisableItemSlotButton(item);
            }
        }
                
        combineState = true;
        //disattivo anche l'oggetto che ha richiesto il combine
        selectedItem.GetComponent<Button>().interactable = false;
       
    }

    public void InspectButtonOnClick()
    {
        Text title = InspectGUI.transform.FindChild("TitleInspectGUI").GetComponent<Text>();
        title.text = senderAttribute.Description;
        InputField description = InspectGUI.transform.FindChild("DescriptionInspectGUI").GetComponent<InputField>();
        description.text = senderAttribute.Note;
        RawImage image = InspectGUI.transform.FindChild("ImageInspectorGUI").FindChild("Image").GetComponent<RawImage>();
        image.texture = (Texture2D)UnityEditor.AssetDatabase.LoadAssetAtPath(senderAttribute.ImagePath, typeof(Texture2D));
        gameMaster.ShowInspectGUI();
        gameMaster.HideItemMenu();
    }

    public void LeaveButtonOnClick()
    {
        GameObject prefabObj = (GameObject)Resources.Load(GameMaster.PREFABS_ITEM_PATH + senderAttribute.Name);
        GameObject itemContainer = new GameObject();
        GameObject itemCopy = (GameObject)Instantiate(prefabObj, itemContainer.transform);

        //setto i valori del prefab in base a quelli dell'oggetto nell'inventario
        Transform itemChild = itemCopy.transform.FindChild(senderAttribute.Name);
        ItemAttribute attr = itemChild.gameObject.GetComponent<ItemAttribute>();

        //sostituisco l'ItemAttribute predefinito con quello in ingresso
        DestroyImmediate(attr);

        ItemAttribute newAttr = (ItemAttribute)itemChild.gameObject.AddComponent(attr.GetType());

        newAttr = CopyAttribute(senderAttribute, newAttr);

        itemCopy.name = newAttr.Name;
        itemCopy.transform.SetParent(null, false);
        itemCopy.transform.position = new Vector3(player.transform.position.x - 2f, player.transform.rotation.y, player.transform.position.z + 3f);

        //elimino l'oggetto nell'inventario
        Transform item = InventoryContentGUI.transform.FindChild(senderAttribute.Name);
       
        GameObject.DestroyObject(item.gameObject);
        //distruggo il container del prefab
        GameObject.DestroyObject(itemContainer);

        gameMaster.ShowHideInventory();

    }

    #endregion


    #region EquippedContentGUI functions

    private void RemoveEquippedItem()
    {
        DestroyObject(equippedItem);
        gameMaster.HideItemMenu();
        equippedItem = null;
        gameMaster.UpdateEquippedStatus(false);
    }

    #endregion


}
