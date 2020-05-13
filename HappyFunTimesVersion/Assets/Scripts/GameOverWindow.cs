using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverWindow : MonoBehaviour
{
    private Text scoreText;

    private void Awake()
    {
        scoreText = transform.Find("scoreText").GetComponent<Text>();
    }

    private void Start()
    {
        Level.GetInstance().Ended += LevelEnded;
        gameObject.SetActive(false);
    }

    private void LevelEnded(object sender, System.EventArgs e)
    {
        scoreText.text =Level.GetInstance().GetCoinsString().ToString();
        gameObject.SetActive(true);
    }

    public void Retry()
    {
        gameObject.SetActive(false);
        Level.GetInstance().RestartLevel();
    }
}
