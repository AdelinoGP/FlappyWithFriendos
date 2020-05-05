using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour
{
    private const float CAMERA_ORTHO_SIZE = 50f;
    private const float PIPE_WIDTH = 2.6f;
    private const float PIPE_HEAD_HEIGHT = 2f;
    private const float PIPE_MOVE_SPEED = 50f;

    private static Level instance;
    public static Level GetInstance()
    {
        return instance;
    }

    private List<Pipe> pipeList;
    private List<Bird> birdList;
    private float coins;
    private float highScoreCoins;

    public float GetCoins()
    {
        return coins;
    }

    public string GetCoinsString()
    {
        return "HighScore:" + highScoreCoins.ToString()+ "\nScore:" + coins.ToString();
    }

    private float pipeSpawnTimer;
    private float previousGapY;
    private int liveBirdCount;
    private bool gameRunning;
    private bool gameRestarted;

    public event System.EventHandler Ended;

    private void Awake()
    {
        instance = this;
        pipeList = new List<Pipe>();
        birdList = new List<Bird>();
        coins = 0;
        pipeSpawnTimer = 0;
        previousGapY = 50;
        Random.InitState((int)Time.time);
        gameRunning = false;
        gameRestarted = false;
    }

    private void Level_OnDied(object sender, System.EventArgs e)
    {
        liveBirdCount--;
    }

    private void Update()
    {
        if (Input.anyKeyDown && gameRestarted == true)
        {
            gameRunning = true;
            gameRestarted = false;

            foreach (Bird bird in birdList)
                bird.Restart();
        }
        if (gameRunning)
        {
            HandlePipeSpawn();
            HandlePipeMovement();
            if (liveBirdCount <= 0)
            {
                gameRunning = false;
                if (coins > highScoreCoins)
                    highScoreCoins = coins;
                Ended?.Invoke(this, System.EventArgs.Empty);
            }
        }
        if (Input.GetKey("escape"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    
    public void CreateBird(KeyCode keyPressed)
    {
        Bird bird = Instantiate(GameAssets.GetInstance().pfBird).GetComponent<Bird>();
        bird.Create(keyPressed);
        birdList.Add(bird);
        bird.OnDied += Level_OnDied;
    }

    private void HandlePipeSpawn()
    {
        pipeSpawnTimer += Time.deltaTime;
        if (pipeSpawnTimer > 1)
        {
            previousGapY += Random.Range(-5f * coins/10f, 5f * coins/10f);
            if (previousGapY > 80f)
                previousGapY = 80f;
            if (previousGapY < 20f)
                previousGapY = 20f;
            float gapSize = Random.Range(40f - coins/5f, 70f - coins/2f);
            if (gapSize <20f)
                previousGapY = 20f;
            CreateGapPipes(previousGapY, gapSize, 140f);
            Debug.Log("Coins: " + coins + " GapY:" + previousGapY + " GapSize:" + gapSize);
            pipeSpawnTimer = 0;
        }
    }

    private void HandlePipeMovement()
    {
        for (int i=0; i< pipeList.Count; i++)
        {
            Pipe pipe = pipeList[i];
            pipe.Move(PIPE_MOVE_SPEED * Time.deltaTime * (coins/100f + 1f));
            if (pipe.GetXPosition() < -150f)
            {
                pipe.Remove();
                pipeList.Remove(pipe);
                i++;
            }
            if (pipe.GetXPosition() < -75f && pipe.onTheRightOfTheBird)
            {
                coins += 0.5f;
                pipe.onTheRightOfTheBird = false;
                
            }
        }
    }

    public void BirdShow()
    {
        for (int i = 0;i<birdList.Count ;i++)
        {
            Bird bird = birdList[i];
            float x = i / 5f;
            bird.Change(true, -20f * x , 40f - 18 * (x - i/5)*5, true);
        }
    }

    public bool RestartLevel()
    {
        liveBirdCount = birdList.Count;
        if (liveBirdCount > 0)
        {
            while (pipeList.Count > 0)
            {
                Pipe pipe = pipeList[0];
                pipe.Remove();
                pipeList.Remove(pipe);
            }
            for (int i = 0;i < birdList.Count ;i++)
            {
                Bird bird = birdList[i];

                bird.Change(-75f, 40-(90/birdList.Count)*i,true);
            }
            gameRestarted = true;
            coins = 0;
            Debug.Log("level reiniciado");
            return true;
        }
        else
            return false;

    }

    private void CreateGapPipes(float gapY,float gapSize,float xPosition)
    {
        CreatePipe(gapY - gapSize * .5f, xPosition, true);
        CreatePipe(CAMERA_ORTHO_SIZE * 2f -gapY -gapSize * .5f, xPosition, false);
    }
    
    private void CreatePipe(float height, float xPosition, bool bottom)
    {
        float upOrDown = bottom ? -1 : 1;
        //Set up Head 
        Transform pipeHead = Instantiate(GameAssets.GetInstance().pfPipeHead);
        pipeHead.position = new Vector3(xPosition, (CAMERA_ORTHO_SIZE - height + PIPE_HEAD_HEIGHT) * upOrDown);
        if (!bottom)
            pipeHead.localRotation = new Quaternion(180f, 0f, 0f,0);

        //Set up Body
        Transform pipeBody = Instantiate(GameAssets.GetInstance().pfPipeBody);
        pipeBody.localScale = new Vector3(5, -upOrDown, 1);
        pipeBody.position = new Vector3(xPosition,CAMERA_ORTHO_SIZE * upOrDown);

        SpriteRenderer pipeBodySpriteRenderer = pipeBody.GetComponent<SpriteRenderer>();
        pipeBodySpriteRenderer.size = new Vector2(PIPE_WIDTH, height);

        BoxCollider2D pipeBodyBoxCollider = pipeBody.GetComponent<BoxCollider2D>();
        pipeBodyBoxCollider.size = new Vector2(PIPE_WIDTH, height);
        pipeBodyBoxCollider.offset = new Vector2(0f, height * 0.5f);
        pipeList.Add(new Pipe(pipeHead, pipeBody));
    }

    private class Pipe
    {
        private Transform pipeHead;
        private Transform pipeBody;
        public bool onTheRightOfTheBird;

        public Pipe(Transform pipeHead, Transform pipeBody)
        {
            this.pipeHead = pipeHead;
            this.pipeBody = pipeBody;
            onTheRightOfTheBird = true;
    }

        public void Remove()
        {
            Destroy(pipeHead.gameObject);
            Destroy(pipeBody.gameObject);
        }

        public void Move(float moveSpeed)
        {
            pipeHead.position += new Vector3(-1f, 0, 0) * moveSpeed;
            pipeBody.position += new Vector3(-1f, 0, 0) * moveSpeed;
        }

        public float GetXPosition()
        {
            return pipeHead.position.x;
        }

    }
}
