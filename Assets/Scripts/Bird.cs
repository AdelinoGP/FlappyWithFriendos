using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bird : MonoBehaviour
{
    private const float JUMP_AMOUNT = 120f;

    private Rigidbody2D birdRigidbody2D;
    public Text keyText;

    private KeyCode keySetting;

    public event System.EventHandler OnDied;

    private static Object instance;
    public static Object GetInstance()
    {
        return instance;
    }

    private bool dead;
    private bool colorChangeable;

    void Awake()
    {
        instance = this;
        dead = true;
        colorChangeable = false;
        birdRigidbody2D = GetComponent<Rigidbody2D>();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(keySetting))
        {
            if (colorChangeable)
            {
                SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
                spriteRenderer.color = new Color(Random.Range(0f, 2f), Random.Range(0f, 2f), Random.Range(0f, 2f));

            }
            else
                Jump();
        }

        if (dead && transform.position.x > -150f )
            transform.position += new Vector3(-0.5f, 0, 0);
    }

    
    private void Jump()
    {
        if(birdRigidbody2D.bodyType == RigidbodyType2D.Dynamic)
            birdRigidbody2D.velocity = Vector2.up * JUMP_AMOUNT;
    }

    public void Restart()
    {
        colorChangeable = false;
        dead = false;
        birdRigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        birdRigidbody2D.velocity = new Vector2(0, 0);
        transform.position = new Vector3(-75,0,0);
        birdRigidbody2D.simulated = true;
        keyText.text = "";
    }

    public void Create(KeyCode key)
    {
        dead = false;
        colorChangeable = true;
        keyText.text = key.ToString();

        keySetting = key;

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(Random.Range(0f, 2f), Random.Range(0f, 2f), Random.Range(0f, 2f));
        birdRigidbody2D.velocity = new Vector2(0, 0);
        transform.position = new Vector3(-75, 0, 0);
        birdRigidbody2D.bodyType = RigidbodyType2D.Static;

    }

    public void Change(float x, float y,bool staticState)
    {
        dead = false;
        transform.position = new Vector3(x, y, 0);
        if (staticState)
            birdRigidbody2D.bodyType = RigidbodyType2D.Static;
        else
            birdRigidbody2D.bodyType = RigidbodyType2D.Dynamic;

        colorChangeable = false;
    }

    public void Change(bool color, float x, float y, bool staticState)
    {
        colorChangeable = color;
        transform.position = new Vector3(x, y, 0);

        if (staticState)
            birdRigidbody2D.bodyType = RigidbodyType2D.Static;
        else
            birdRigidbody2D.bodyType = RigidbodyType2D.Dynamic;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        birdRigidbody2D.bodyType =  RigidbodyType2D.Static;
        birdRigidbody2D.simulated = false;
        dead = true;
        OnDied?.Invoke(this, System.EventArgs.Empty);
        keyText.text = Level.GetInstance().GetCoins().ToString();
    }
}
