using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.Collections.Generic;
using UnityStandardAssets.Characters.FirstPerson;

public  class GameMaster:MonoBehaviour
{
    private string conn;

    public IDbConnection dbconn;

    private FirstPersonController fpsController;
    private CanvasGroup PauseMenuCanvasGroup;
    private CanvasGroup inventoryContentCanvasGroup;
    private Text inventoryAlert;
    private CanvasGroup itemMenuContentCanvasGroup;
    private GameObject MiniMapCamera;
    private CanvasGroup EquippedGUICanvasGroup;
    private CanvasGroup InspectGUICanvasGroup;
    private GameObject LifePercentageGUI;

    private GameObject tooltipGUI;
    private TipStyle tipStyle;
    private Text Tip;

    public static string DATABASE_PATH = "/Resources/DB/DBGame.db";
    public static string PREFABS_ITEM_PATH = "PREFABS/ITEMS/";


    void Start()
    {
        conn = "URI=file:" + Application.dataPath + DATABASE_PATH;
        dbconn = null;
       

        Init();

    }


    void Init()
    {
        //Inizializzazioni varie

        Texture2D cursorTexture = (Texture2D)Resources.Load<Texture2D>("IMG/black-cursor");
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
        // Cursor.lockState = CursorLockMode.Locked;

        fpsController = GameObject.Find("FPSController").GetComponent<FirstPersonController>();
        PauseMenuCanvasGroup = GameObject.Find("PauseMenuGUI").GetComponent<CanvasGroup>();
        inventoryContentCanvasGroup = GameObject.Find("InventoryGUI").GetComponent<CanvasGroup>();
        inventoryAlert = GameObject.Find("ToolTipInventoryGUI").GetComponent<Text>();
        itemMenuContentCanvasGroup = GameObject.Find("ItemMenuGUI").GetComponent<CanvasGroup>();
        EquippedGUICanvasGroup = GameObject.Find("EquippedGUI").GetComponent<CanvasGroup>();
        InspectGUICanvasGroup = GameObject.Find("InspectGUI").GetComponent<CanvasGroup>();
        MiniMapCamera = GameObject.Find("MiniMapCamera");
        tooltipGUI = GameObject.Find("ToolTipGUI");
        Tip = tooltipGUI.transform.FindChild("Tip").GetComponent<Text>();
        tipStyle = tooltipGUI.transform.FindChild("Tip").GetComponent<TipStyle>();
        LifePercentageGUI = GameObject.Find("LifePercentageGUI");
        ShowHidePausemenu();
       
    }


    #region DataBase Function

    void DBConnect()
    {
        dbconn = (IDbConnection)new SqliteConnection(conn);

        dbconn.Open(); 

        Debug.Log("Connessione al DB effettuata.");
    }

    void DBDisconnect()
    {
        dbconn.Close();
        dbconn = null;
        Debug.Log("Disconnesso dal DB.");
    }

    public void DeleteAllItems()
    {
        try
        {
            DBConnect();
            Debug.Log("DeleteAllItems start");
            IDbCommand dbcmd = dbconn.CreateCommand();
            dbcmd.CommandType = CommandType.Text;
            string sqlQuery = "DELETE FROM INVENTORY;";
            dbcmd.CommandText = sqlQuery;
            int deletedRow = dbcmd.ExecuteNonQuery();
            Debug.Log("Record rimossi: " + string.Format("{0}", deletedRow));
            Debug.Log("DeleteAllItems end");
            DBDisconnect();
        }
        catch (Exception ex)
        {
            Debug.Log("Errore nell'esecuzione della query!:\n" + ex); 
            DBDisconnect();
        }
       
    }

    public  void  SaveItemToDB(ItemAttribute itemAttribute)
    {
        try
        {
            DBConnect();
            Debug.Log("SaveItemToDB start");
            string Type = itemAttribute.Type.ToString();
            string Stackable = (itemAttribute.Stackable) ? "S" : "N";
            string Usable = (itemAttribute.Usable) ? "S" : "N";
            string Equipable = (itemAttribute.Equipable) ? "S" : "N";
            string Destroyable = (itemAttribute.Destroyable) ? "S" : "N";
            string CombineWith = itemAttribute.CombineWith.ToString();

            IDbCommand dbcmd = dbconn.CreateCommand();
            dbcmd.CommandType = CommandType.Text;

            string values = string.Format("'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}'", itemAttribute.ObjectTag, itemAttribute.Name,
                                Type, itemAttribute.Description, itemAttribute.Qta, itemAttribute.MaxQta, Stackable, Usable, Equipable, Destroyable, itemAttribute.Note, itemAttribute.ImagePath, CombineWith, itemAttribute.InventoryPosition);

            Debug.Log("Valori: " + values); 

            string sqlQuery = string.Format("INSERT INTO INVENTORY ('ObjectTag','Name','Type','Description','Qta','MaxQta','Stackable','Usable','Equipable','Destroyable','Note','ImagePath','CombineWith','InventoryPosition')" +
                                  " VALUES ({0});", values);

            Debug.Log("Query: " + sqlQuery);

            dbcmd.CommandText = sqlQuery;

            int insertedRow = dbcmd.ExecuteNonQuery();
            if (insertedRow == 0)
            {
                Debug.Log("Errore nell'esecuzione della query!"); 
            }
            else
            {
                Debug.Log("Query eseguita!"); 
            }

            Debug.Log("SaveItemToDB end");

            DBDisconnect();
        }
        catch (Exception ex)
        {
            Debug.Log("Errore nell'esecuzione della query!:\n" + ex); 
            DBDisconnect();
        }           
    }

    public  void  SaveAllItemToDB(List<ItemAttribute> itemAttributeList)
    {
        try
        {
            //Per comodita'cancello tutto l'inventario
            DeleteAllItems();

            DBConnect();

            Debug.Log("SaveAllItemToDB start");

            IDbCommand dbcmd = dbconn.CreateCommand();
            dbcmd.CommandType = CommandType.Text;

            foreach (ItemAttribute itemAttribute  in itemAttributeList)
            {
                
                string Type = itemAttribute.Type.ToString();
                string Stackable = (itemAttribute.Stackable) ? "S" : "N";
                string Usable = (itemAttribute.Usable) ? "S" : "N";
                string Equipable = (itemAttribute.Equipable) ? "S" : "N";
                string Destroyable = (itemAttribute.Destroyable) ? "S" : "N";
                string CombineWith = itemAttribute.CombineWith.ToString();



                string values = string.Format("'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}'", itemAttribute.ObjectTag, itemAttribute.Name,
                                    Type, itemAttribute.Description, itemAttribute.Qta, itemAttribute.MaxQta, Stackable, Usable, Equipable, Destroyable, itemAttribute.Note, itemAttribute.ImagePath, CombineWith, itemAttribute.InventoryPosition);

                //Debug.Log("Valori: " + values); 

                string sqlQuery = string.Format("INSERT INTO INVENTORY ('ObjectTag','Name','Type','Description','Qta','MaxQta','Stackable','Usable','Equipable','Destroyable','Note','ImagePath','CombineWith','InventoryPosition')" +
                                      " VALUES ({0});", values);

                //Debug.Log("Query: " + sqlQuery);

                dbcmd.CommandText = sqlQuery;

                int insertedRow = dbcmd.ExecuteNonQuery();

                if (insertedRow == 0)
                {
                    Debug.Log("Errore nell'esecuzione della query!"); 
                }
                else
                {
                    Debug.Log("Query eseguita!"); 
                }

            }

            dbcmd.Dispose();
            dbcmd = null;

            Debug.Log("SaveAllItemToDB end");
            DBDisconnect();
        }
        catch (Exception ex)
        {
            Debug.Log("Errore nell'esecuzione della query!:\n" + ex); 
            DBDisconnect();
        }           
    }

    public void LoadItemFromDB(ItemAttribute itemAttribute)
    {
        try
        {
            IDbCommand dbcmd = dbconn.CreateCommand();
            dbcmd.CommandType = CommandType.Text;

        }
        catch (Exception ex)
        {
            Debug.Log("Errore nell'esecuzione della query!:\n" + ex); 
        }
    }

    public List<ItemAttribute> LoadAllItemFromDB()
    {
        List<ItemAttribute> listAttribute = new List<ItemAttribute>();
        IDataReader reader = null;
        try
        {
            DBConnect();

            Debug.Log("LoadAllItemFromDB start");

            IDbCommand dbcmd = dbconn.CreateCommand();
            dbcmd.CommandType = CommandType.Text;

            string sqlQuery = "SELECT * FROM INVENTORY ORDER BY InventoryPosition asc;";

            dbcmd.CommandText = sqlQuery;

            reader = dbcmd.ExecuteReader();

            //creo un contenitore
            GameObject itemContainer = new GameObject();

            while (reader.Read())
            {
                ItemAttribute attr = itemContainer.AddComponent<ItemAttribute>();
      
                attr.ObjectTag = reader.GetString(0);
                attr.Name = reader.GetString(1);
                attr.Type = attr.getTypeFromString(reader.GetString(2));
                attr.Description = reader.GetString(3);
                attr.Qta = reader.GetInt32(4);
                attr.MaxQta = reader.GetInt32(5);
                attr.Stackable = reader.GetString(6).Equals("S") ? true : false;
                attr.Usable = reader.GetString(7).Equals("S") ? true : false;
                attr.Equipable = reader.GetString(8).Equals("S") ? true : false;
                attr.Destroyable = reader.GetString(9).Equals("S") ? true : false;
                attr.Note = reader.GetString(10);
                attr.ImagePath = reader.GetString(11);
                attr.CombineWith = attr.getTypeFromString(reader.GetString(12));
                attr.InventoryPosition = reader.GetInt32(13);
                listAttribute.Add(attr);
                //Debug.Log("attr.name restituito dal db: " + attr.Name);
            }


            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;

            //cancello il container
            DestroyObject(itemContainer, 1);

            Debug.Log("LoadAllItemFromDB end");
            DBDisconnect();
        }
        catch (Exception ex)
        {
            Debug.Log("Errore nell'esecuzione della query!:\n" + ex); 
            DBDisconnect();
        }

        return listAttribute;
    }

    #endregion



    private void PlayerMovementLock()
    {
        Time.timeScale = 0;
        fpsController.m_MouseLook.SetCursorLock(false);
        fpsController.m_MouseLook.UpdateCursorLock();
        fpsController.enabled = false;
        Cursor.visible = true;
    }

    private void PlayerMovementUnlock()
    {
        Time.timeScale = 1;
        fpsController.enabled = true;
        fpsController.m_MouseLook.SetCursorLock(true);
        fpsController.m_MouseLook.UpdateCursorLock();
        Cursor.visible = false;
    }

    public void UpdateEquippedStatus(bool value)
    {
        fpsController.m_Equipped = value;
    }

    private void ShowMiniMap()
    {
        MiniMapCamera.SetActive(true);
    }

    private void HideMiniMap()
    {
        MiniMapCamera.SetActive(false);
    }


    #region PauseMenu Function

    public void ShowHidePausemenu()
    {
        HideInventory();
        HideLifePercentage();

        if (IsPauseMenuActive() == false)
        {
            PlayerMovementLock();
            ShowPauseMenu();          
            HideMiniMap();
            HideEquippedGUI();
        }
        else
        {
            PlayerMovementUnlock();
            HidePauseMenu();
            ShowMiniMap();
            ShowEquippedGUI();
        }
       
    }

    private void ShowPauseMenu()
    {
        PauseMenuCanvasGroup.alpha = 1f;
        PauseMenuCanvasGroup.blocksRaycasts = true;
    }

    private void HidePauseMenu()
    {
        PauseMenuCanvasGroup.alpha = 0f;
        PauseMenuCanvasGroup.blocksRaycasts = false;
    }

    private bool IsPauseMenuActive()
    {
        if (PauseMenuCanvasGroup.alpha == 1f)
            return true;
        else
            return false;
    }


    #endregion

    #region Inventory Function

    public void ShowHideInventory()
    {
        if (IsPauseMenuActive() == false)
        {
            if (isInventoryActive() == false)
            {
                PlayerMovementLock();
                ShowInventory();
                ShowLifePercentage();
            }
            else
            {
                PlayerMovementUnlock();
                HideInventory();
                HideLifePercentage();
            }
        }
    }

    private void ShowLifePercentage()
    {
        LifePercentageGUI.SetActive(true);
    }

    private void HideLifePercentage()
    {
        LifePercentageGUI.SetActive(false);
    }

    private void ShowInventory()
    {
        inventoryContentCanvasGroup.alpha = 1f;
        inventoryContentCanvasGroup.blocksRaycasts = true;
    }

    private void HideInventory()
    {
        inventoryContentCanvasGroup.alpha = 0f;
        inventoryContentCanvasGroup.blocksRaycasts = false;
        HideItemMenu();
        HideInspectGUI();
        inventoryAlert.text = "";
    }

    private bool isInventoryActive()
    {
        if (inventoryContentCanvasGroup.alpha == 1f)
            return true;
        else
            return false;
    }

    #endregion




    #region Misc Function

    public TipStyle getHintStyle()
    {
        return tipStyle;
    }

    //mostra un tooltip in basso vicino l'icona()
    public void ShowHint(String msg, int fontSize, FontStyle fontStyle, Color color)
    {
        Tip.enabled = true;
        Tip.color = color;
        Tip.fontSize = fontSize;
        Tip.fontStyle = fontStyle;
        Tip.text = msg;
    }
    //nasconde il tooltip
    public void HideHint()
    {
        Tip.enabled = false;
    }
   
    //menu che si apre quando si clicca su un oggetto inventario
    public void ShowItemMenu()
    {
        itemMenuContentCanvasGroup.alpha = 1f;
        itemMenuContentCanvasGroup.blocksRaycasts = true;
    }

    public void HideItemMenu()
    {
        itemMenuContentCanvasGroup.alpha = 0f;
        itemMenuContentCanvasGroup.blocksRaycasts = false;
        Tip.text = "";
        Tip.enabled = false;
    }

    //casella che mostra lóggetto attualmente equipaggiato
    public void HideEquippedGUI()
    {
        EquippedGUICanvasGroup.alpha = 0;
        EquippedGUICanvasGroup.blocksRaycasts = false;
    }

    public void ShowEquippedGUI()
    {
        EquippedGUICanvasGroup.alpha = 1f;
        EquippedGUICanvasGroup.blocksRaycasts = true;
    }

    public void ShowInspectGUI()
    {
        InspectGUICanvasGroup.alpha = 1f;
        InspectGUICanvasGroup.blocksRaycasts = true;
    }

    public void HideInspectGUI()
    {
        InspectGUICanvasGroup.alpha = 0f;
        InspectGUICanvasGroup.blocksRaycasts = false;
    }

    #endregion




}
