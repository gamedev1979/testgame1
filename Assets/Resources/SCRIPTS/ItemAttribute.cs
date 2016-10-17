using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEditor;

public class ItemAttribute : MonoBehaviour
{
   

    public enum TypeItem
    {
        Nothing,
        ArmaPistola,
        MunizioniPistola,
        ArmaFucile,
        MunizioniFucile,
        CuraPiccola,
        CuraGrande,
        Importante,
        Note
    }

    [Header("Questo campo puó essere calcolato a runtime")] public string ObjectTag = "";
    [Header("Questo campo puó essere calcolato a runtime")] public string Name = "";
    [Space]
    public TypeItem Type;
    public string Description;
    [Space]
    public int Qta;
    public int MaxQta;
    [Space]
    public bool Stackable;
    public bool Usable;
    public bool Equipable;
    public bool Destroyable;
    [Space]
    public string Note;
    [Space]
    [Header("Questo campo puó essere calcolato a runtime")] public string ImagePath = "Posso calcolarlo";
    [Space]
    public TypeItem CombineWith;
    [Space]
    //VA CALCOLATO DOPO: lo nascondo dall'inspector
    [HideInInspector]  public int InventoryPosition;


    // Use this for initialization
    void Start()
    {
        if (string.IsNullOrEmpty(ObjectTag))
            ObjectTag = transform.tag;

        if (string.IsNullOrEmpty(Name))
            Name = transform.name;

        if (string.IsNullOrEmpty(ImagePath))
        {
            Image itemimage = (Image)transform.GetComponent<Image>();

            ImagePath = AssetDatabase.GetAssetPath(itemimage.mainTexture);
        }

        //setta la Qta nellóggetto figlio
     
        Transform qta = transform.FindChild("Qta");
        if (qta != null)
        {
            Text qtaText = (Text)qta.gameObject.GetComponent<Text>();
            qtaText.text = string.Format("{0}", Qta);
        }

    }


    void Update()
    {
        //setta la Qta nellóggetto figlio

        Transform qta = transform.FindChild("Qta");
        if (qta != null)
        {
            Text qtaText = (Text)qta.gameObject.GetComponent<Text>();
            qtaText.text = string.Format("{0}", Qta);
        }
    }

    public TypeItem getTypeFromString(string strType)
    {
        //Debug.Log("getTypeFromString strType: " + strType);
        return  (TypeItem)System.Enum.Parse(typeof(TypeItem), strType); 
    }

        
}
