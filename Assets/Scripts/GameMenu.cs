using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour
{
    private List<KeyCode> keyList;
    private Text keyPressText;

    void Start()
    {
        keyList = new List<KeyCode>();
        keyPressText = transform.Find("KeyPressText").GetComponent<Text>();
    }

    void Update()
    {
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
        if (Input.anyKeyDown)
        {
            try
            {
                KeyCode keyPressed = (KeyCode)System.Enum.Parse(typeof(KeyCode), Input.inputString.ToUpper());
                if (!keyList.Contains(keyPressed))
                {
                    keyPressText.text = "Tecla " + keyPressed.ToString() + " Pressionada";
                    keyList.Add(keyPressed);
                    Level.GetInstance().CreateBird(keyPressed);
                }
                else
                {
                    keyPressText.text = "Tecla " + keyPressed.ToString() + " Já Adicionada";
                }
            }
            catch (System.Exception)
            {
                keyPressText.text = "Tecla Invalida Pressionada";
            }
            Level.GetInstance().BirdShow();
        }
    }

    public void Iniciar()
    {
        if(Level.GetInstance().RestartLevel())
            gameObject.SetActive(false);
        else
            keyPressText.text = "Adicione um Jogador";

    } 
}

