using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bird : MonoBehaviour
{
    private float jumpAmount = 120f;

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
        if (!dead && birdRigidbody2D.bodyType != RigidbodyType2D.Static)
            transform.Rotate(Vector3.forward * -75f * Time.deltaTime, Space.Self);

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
        
        if (birdRigidbody2D.bodyType == RigidbodyType2D.Dynamic)
        {
            birdRigidbody2D.velocity = Vector2.up * jumpAmount;
            SoundManager.GetInstance().SoundBirdWing();
            transform.rotation = Quaternion.identity;
            transform.rotation = Quaternion.Euler(0f, 0f, 35f);
        }
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

        jumpAmount = 120f;
        birdRigidbody2D.gravityScale = 30;
        transform.localScale = new Vector3(5f, 5f, 1f);
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
        transform.rotation = Quaternion.identity;
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
        if(collision.gameObject.name == "Stone")
        {
            birdRigidbody2D.gravityScale *= 2;
            jumpAmount *= 1.5f;
            Level.GetInstance().KillPowerUp();
        }
        else if (collision.gameObject.name == "Baloon")
        {
            birdRigidbody2D.gravityScale /= 2;
            jumpAmount /= 1.5f;
            Level.GetInstance().KillPowerUp();
        }
        else if (collision.gameObject.name == "BigPill")
        {
            transform.localScale = transform.localScale * 1.5f;
            jumpAmount /= 1.25f;
            Level.GetInstance().KillPowerUp();
        }
        else if (collision.gameObject.name == "SmallPill")
        {
            transform.localScale = transform.localScale / 1.5f;
            jumpAmount *= 1.25f;
            Level.GetInstance().KillPowerUp();
        }
        else
        {
            birdRigidbody2D.bodyType = RigidbodyType2D.Static;
            birdRigidbody2D.simulated = false;

            dead = true;

            SoundManager.GetInstance().SoundBirdDied();

            OnDied?.Invoke(this, System.EventArgs.Empty);

            keyText.text = Level.GetInstance().GetCoins().ToString();
        }
       
    }
}
