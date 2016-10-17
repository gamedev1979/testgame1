using UnityEngine;
using System.Collections;
using UnityEditor;

public class GUIManager : MonoBehaviour
{

    public RenderTexture MiniMapTexture;
    [Header("Modificare  questa texture per la trasparenza o un colore solido")]
    public Material MiniMapMaterial;
    [Space]
    [Header("Il seguent paramentro indica la spaziatura della mini mappa rispetto agli angoli dello schermo")]
    public float offset = 10f;
    [Space]
    public float larghezza = 100f;
    public float altezza = 80f;

    public enum Posizione
    {
        AltoDestra,
        AltoSinistra,
        BassoDestra,
        BassoSinistra
    }

    [Space]
    public Posizione posizioneMappa;
    // Update is called once per frame

    void OnGUI()
    {

        Rect Map_Rectangle = new Rect(); 

        switch (posizioneMappa)
        {
            case (Posizione.AltoDestra):
                Map_Rectangle = new Rect(Screen.width - larghezza - offset, offset, larghezza, altezza);
                break;
            case (Posizione.AltoSinistra):
                Map_Rectangle = new Rect(offset, offset, larghezza, altezza);
                break;
            case(Posizione.BassoDestra):
                Map_Rectangle = new Rect(Screen.width - larghezza - offset, Screen.height - altezza - offset, larghezza, altezza);
                break;
            case(Posizione.BassoSinistra):
                Map_Rectangle = new Rect(offset, Screen.height - altezza - offset, larghezza, altezza);
                break;
        }

        DrawMap(Map_Rectangle);
    }


    void DrawMap(Rect Map_Rectangle)
    {
        if (Event.current.type == EventType.Repaint)
        {
            Graphics.DrawTexture(Map_Rectangle, MiniMapTexture, MiniMapMaterial);
        }
    }
}
