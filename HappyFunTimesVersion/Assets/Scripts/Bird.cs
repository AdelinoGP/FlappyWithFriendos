using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Bird : MonoBehaviour
{
    public Level level;
    
    private HFTInput m_hftInput;
    private HFTGamepad m_gamepad;

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
    
    void Start()
    {
        instance = this;
        dead = true;
        birdRigidbody2D = GetComponent<Rigidbody2D>();

        m_hftInput = GetComponent<HFTInput>();
        m_gamepad = GetComponent<HFTGamepad>();

        level = Level.GetInstance().GetComponent<Level>();

        dead = false;
        keyText.text = m_gamepad.playerName;
        m_gamepad.OnNameChange += ChangeName;
        Debug.Log(m_gamepad.playerName);
        m_gamepad.OnDisconnect += Remove;


        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = m_gamepad.color;

        birdRigidbody2D.velocity = new Vector2(0, 0);
        transform.position = new Vector3(-75, 0, 0);
        birdRigidbody2D.bodyType = RigidbodyType2D.Static;

        level.CreateBird(this);
    }

    void Remove()
    {
        Destroy(this);
    }

    void ChangeName(object sender, System.EventArgs e)
    {
        if(!dead)
            keyText.text = m_gamepad.playerName;
        Debug.Log("new name is: " + m_gamepad.playerName);
    }


    void Update()
    {
        if (!dead && birdRigidbody2D.bodyType != RigidbodyType2D.Static)
            transform.Rotate(Vector3.forward * -75f * Time.deltaTime, Space.Self);

        if (m_hftInput.GetButtonDown("fire1"))
                Jump();

        if (m_hftInput.GetButtonDown("fire2"))
            level.Restart();

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

    public void Change(float x, float y,bool staticState)
    {
        dead = false;
        transform.rotation = Quaternion.identity;
        transform.position = new Vector3(x, y, 0);
        if (staticState)
            birdRigidbody2D.bodyType = RigidbodyType2D.Static;
        else
            birdRigidbody2D.bodyType = RigidbodyType2D.Dynamic;

    }

    public void Change(bool color, float x, float y, bool staticState)
    {
        transform.position = new Vector3(x, y, 0);

        if (staticState)
            birdRigidbody2D.bodyType = RigidbodyType2D.Static;
        else
            birdRigidbody2D.bodyType = RigidbodyType2D.Dynamic;
    }

    public void Kill()
    {
        birdRigidbody2D.bodyType = RigidbodyType2D.Static;
        birdRigidbody2D.simulated = false;

        dead = true;

        SoundManager.GetInstance().SoundBirdDied();

        OnDied?.Invoke(this, System.EventArgs.Empty);

        keyText.text = Level.GetInstance().GetCoins().ToString();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name == "Stone")
        {
            birdRigidbody2D.gravityScale *= 2;
            jumpAmount *= 1.5f;
            level.KillPowerUp();
        }
        else if (collision.gameObject.name == "Baloon")
        {
            birdRigidbody2D.gravityScale /= 2;
            jumpAmount /= 1.5f;
            level.KillPowerUp();
        }
        else if (collision.gameObject.name == "BigPill")
        {
            transform.localScale = transform.localScale * 1.5f;
            jumpAmount /= 1.25f;
            level.KillPowerUp();
        }
        else if (collision.gameObject.name == "SmallPill")
        {
            transform.localScale = transform.localScale / 1.5f;
            jumpAmount *= 1.25f;
            level.KillPowerUp();
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
