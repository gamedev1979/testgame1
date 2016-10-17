using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerLifeManager : MonoBehaviour
{

    private float life = 100f;
    private const float ALPHA_VALUE = 100f;

    public float alertLevelValue = 40f;
    private CanvasGroup PlayerLifeIndicatorGUICanvasGroup;
    private Text LifePercentageGUIText;

    void Start()
    {
        PlayerLifeIndicatorGUICanvasGroup = GameObject.Find("PlayerLifeIndicatorGUI").GetComponent<CanvasGroup>();
        LifePercentageGUIText = GameObject.Find("LifePercentageGUI").GetComponent<Text>();
        UpdatePlayerLifeIndicator();
    }
	
    // Update is called once per frame
    void Update()
    {
	
    }


    public void SetLife(float newValue)
    {
        if (newValue < 0)
        {
            life = 0;
        }
        else if (newValue > 100f)
        {
            life = 100f;
        }
        else
        {
            life = newValue;
        }
    }

    public float getLife()
    {
        return life;
    }

    private void UpdatePlayerLifeIndicator()
    {
        LifePercentageGUIText.text = string.Format("{0}%", life);


        //solo se sto sotto il livelloalert di vita
        if (life <= alertLevelValue)
        {
            float alpha = (ALPHA_VALUE - life) / 300f;

            //Debug.Log("Alpha " + alpha); 

            PlayerLifeIndicatorGUICanvasGroup.alpha = alpha;
        }

        if (life <= 0)
        {
            //TODO: MORTO!
        }

    }

 
    void OnCollisionEnter(Collision col)
    {
        Debug.Log("Collisione con :" + col.gameObject.name); 

        if (col.gameObject.tag == "Enemy")
        {
            SetLife(life - 20);
        }

        UpdatePlayerLifeIndicator();
    }
 
}
