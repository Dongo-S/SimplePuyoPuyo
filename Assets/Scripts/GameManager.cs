using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DS.Movements;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Color[] puyosColors;
    public List<Shape> shapes = new List<Shape>(){
      { new Shape(new int[1,4] { { 1, 1, 1, 1} }) }, // | shape
      { new Shape(new int[,] { { 1, 2 }, { 3, 4 }, { 5, 6 }, { 7, 8 } }) }
    };

   // public int[,] boardArray = new int[8, 16];
    public PuyoObject[,] boardArrayPuyos = new PuyoObject[8, 16];
    public ObjectPool puyosPool;
    public bool inStartMenu = true;
    public GameObject gamePanel;
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverScoreLabel;
    public TextMeshProUGUI gamePlayScoreLabel;
    public Transform puyosSpawnerPoint;
    public UnityEngine.UI.Button pauseButton;


    [SerializeField] Transform startPositionDummyMenu;
    [SerializeField] Transform endPositionDummyMenu;
    float timeToSpawnEnemy;
    [SerializeField] float spawnPuyoMenuRate = 1f;

    [SerializeField] bool inPause = false;
    [SerializeField] bool gameOver = false;

    PuyoPair puyoPair;

    Camera mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        puyoPair = new PuyoPair();

       // boardArray = new int[8, 16];
        boardArrayPuyos = new PuyoObject[8, 16];

    }

    // Update is called once per frame
    void Update()
    {
        if (inStartMenu)
        {
            timeToSpawnEnemy += Time.deltaTime;
            if (timeToSpawnEnemy >= spawnPuyoMenuRate)
            {
                timeToSpawnEnemy -= spawnPuyoMenuRate;

                SpawnPuyoMenu();
            }
        }
        else
        {
            if (gameOver)
                return;

            if (inPause)
                return;

           

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                pauseButton.onClick.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                MoveLeft();
            }
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                MoveRight();
            }
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                RotateRight();
            }
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                RotateLeft();
            }
        }
    }


    void InstantiatePuyoPair()
    {
        if (gameOver)
            return;


        GameObject g1 = puyosPool.GetObject();
        GameObject g2 = puyosPool.GetObject();

        PuyoObject puyo1 = g1.GetComponent<PuyoObject>();
        PuyoObject puyo2 = g2.GetComponent<PuyoObject>();

        puyo1.transform.position = new Vector3(4f, puyosSpawnerPoint.position.y);
        int colorindex = Random.Range(0, 4);
        puyo1.image.color = puyosColors[colorindex];
        puyo1.colorIndex = colorindex;

        float random1 = Random.value > 0.5f ? 4f : 5f;

        puyo2.transform.position = new Vector3(random1, random1 == 4 ? puyosSpawnerPoint.position.y + 1f : puyosSpawnerPoint.position.y);
        colorindex = Random.Range(0, 4);
        puyo2.image.color = puyosColors[colorindex];
        puyo2.colorIndex = colorindex;


        puyoPair.Init(puyo1, puyo2);
    }

    public void OnMovePuyo(PuyoObject puyo)
    {
        if (inStartMenu)
            return;

        if (gameOver)
        {
            puyoPair.Pause(true);
            return;
        }

        //Debug.Log($"Position {puyo.transform.position} Name: {puyo.name}");
        Vector2Int position = Vector2Int.zero;
        position.x = (int)puyo.transform.position.x;
        position.y = (int)puyo.transform.position.y;
        // Debug.Log(boardArrayPuyos.GetLength(1) + " Altura");

        if (position.y >= boardArrayPuyos.GetLength(1))
            return;

        if (puyo.transform.position.y - position.y >= 0.1f)
            return;

        //
        if (position.y == 0)
        {
            if (puyoPair.IsBelow(puyo, out PuyoObject otherPuyo))
            {
                Debug.Log("Is Below");
                Vector2Int position2 = Vector2Int.zero;
                position2.x = (int)otherPuyo.transform.position.x;
                position2.y = (int)otherPuyo.transform.position.y;
                StopPuyo(otherPuyo, position2);

            }

            StopPuyo(puyo, position);

        }



        if (!ValidateMovement(position, Directions.Down))
        {
            StopPuyo(puyo, position);
        }

    }

    public void StopPuyo(PuyoObject puyo, Vector2Int position)
    {
        if (inStartMenu)
            return;

        puyo.movement.StopMoving();

      


        boardArrayPuyos[position.x, position.y] = puyo;
        puyo.transform.position = (Vector2)position;

        //Check link
        CheckLink(position);


       // Debug.Log($"Stop Position {puyo.transform.position} Name: {puyo.name}");



        if (puyoPair.DeletePuyo(puyo))
        {
            InstantiatePuyoPair();
        }
    }

    void CheckLink(Vector2Int pos)
    {

        








        int lenghtX = boardArrayPuyos.GetLength(0);
        int lenghtY = boardArrayPuyos.GetLength(1)-1;

        for (int i = 0; i < lenghtX; i++)
        {
            //Debug.Log($"X: {i} Y: {lenghtY}");
            if (boardArrayPuyos[i,lenghtY] != null)
            {
                gameOver = true;
                gameOverPanel.gameObject.SetActive(true);
            }
        }

    }

    public void RotateRight()
    {

    }


    public void RotateLeft()
    {

    }


    public void ValidateRotation()
    {

    }

    public void MoveLeft()
    {
        Vector2 left = puyoPair.Left;

        if (puyoPair.Down.y >= boardArrayPuyos.GetLength(1))
            return;

        if (left.x == 0)
            return;
        if (ValidateMovement(left, Directions.Left))
            puyoPair.Move(-1);
    }

    public void MoveRight()
    {
        Vector2 right = puyoPair.Right;
        if (puyoPair.Down.y >= boardArrayPuyos.GetLength(1))
            return;

        if (right.x == 7)
            return;

        if (ValidateMovement(right, Directions.Right))
            puyoPair.Move(1);

    }


    public bool ValidateMovement(Vector2 currentPos, Directions direction)
    {
        Vector2Int newPos = Vector2Int.zero;

        newPos.x = (int)currentPos.x + (int)MoveGameObject.Direction(direction).x;
        newPos.y = (int)currentPos.y + (int)MoveGameObject.Direction(direction).y;
        //Debug.Log("New Pos: " + newPos);


        try
        {
            if (boardArrayPuyos[newPos.x, newPos.y] == null)
            {
                return true;
            }
        }
        catch (System.IndexOutOfRangeException)
        {

            return false;
        }



        return false;
    }

    public void Pause(bool value)
    {
        inPause = value;
        puyoPair.Pause(value);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }

    public void StartGame()
    {
        inStartMenu = false;
        gamePanel.SetActive(true);
        puyosPool.DeactivateAllPoolObjects();

        InstantiatePuyoPair();
    }

    public void InStartMenu(bool value)

    {
        inStartMenu = value;
    }

    public void SpawnPuyoMenu()
    {

        GameObject g = puyosPool.GetObject();
        g.GetComponent<PuyoObject>().image.color = puyosColors[Random.Range(0, 4)];
        Vector2 p;
        Vector3 posStart = mainCamera.ScreenToWorldPoint(startPositionDummyMenu.position);
        Vector3 posEnd = mainCamera.ScreenToWorldPoint(endPositionDummyMenu.position);
        p.x = (int)Random.Range(posStart.x, posEnd.x);
        p.y = posStart.y;
        g.transform.position = p;

        //g.GetComponent<MoveGameObject>().speed = enemiesSpeed;
    }

    public void CheckRotation(Vector3 direction)
    {

    }

    public void ReturnToPool(Collider2D gObject)
    {
        gObject.transform.parent.gameObject.SetActive(false);

        // gObject.gameObject.transform.parent = puyosPool.transform;
        gObject.transform.parent.position = new Vector3(-10, 20);
    }


    private void OnApplicationFocus(bool focus)
    {
        if (gameOver)
            return;

        if (!focus && !inStartMenu && !inPause)
        {
            pauseButton.onClick.Invoke();
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (gameOver)
            return;

        if (pause && !inStartMenu && !inPause)
        {
            pauseButton.onClick.Invoke();
        }
    }
}



[System.Serializable]
public class Shape
{
    public int[,] shape;

    public Shape(int[,] shape)
    {
        this.shape = shape;
    }
}