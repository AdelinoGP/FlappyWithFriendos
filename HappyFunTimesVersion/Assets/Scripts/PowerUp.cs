using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private Sprite stone;
    [SerializeField] private Sprite baloon;
    [SerializeField] private Sprite bigPill;
    [SerializeField] private Sprite smallPill;

    private SpriteRenderer sprtRenderer;

    private void Awake()
    {
        sprtRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        int number = Random.Range(1, 5);
        if(number == 1)
        {
            sprtRenderer.sprite = stone;
            this.name = "Stone";
        }
        if (number == 2)
        {
            sprtRenderer.sprite = baloon;
            this.name = "Baloon";
        }
        if (number == 3)
        {
            sprtRenderer.sprite = bigPill;
            this.name = "BigPill";
        }
        if (number == 4)
        {
            sprtRenderer.sprite = smallPill;
            this.name = "SmallPill";
        }

    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
