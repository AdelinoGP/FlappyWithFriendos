using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour
{
    private Text keyPressText;

    void Start()
    {
        keyPressText = transform.Find("KeyPressText").GetComponent<Text>();
    }

    void Update()
    {
        if (Input.GetKey("escape"))
        {
            Application.Quit();
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

