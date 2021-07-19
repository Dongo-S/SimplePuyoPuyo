using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DS.Movements;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Color[] puyosColors;

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
    [SerializeField] bool inCombo = false;
    [SerializeField] int score = 0;
    [SerializeField] int comboMultiplier = 1;
    [SerializeField] float puyosSpeed = 4;

    PuyoPair puyoPair;

    List<PuyoObject> puyosMovingInCombo = new List<PuyoObject>();

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

            if (inCombo)
                return;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                puyoPair.IncreaseSpeed();
            }
            else if (Input.GetKeyUp(KeyCode.Space))
            {
                puyoPair.DecreaseSpeed();
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

        comboMultiplier = 1;
        inCombo = false;


        GameObject g1 = puyosPool.GetObject();
        GameObject g2 = puyosPool.GetObject();

        PuyoObject puyo1 = g1.GetComponent<PuyoObject>();
        PuyoObject puyo2 = g2.GetComponent<PuyoObject>();

        int boardXMiddle = boardArrayPuyos.GetLength(0) / 2;
        puyo1.transform.position = new Vector3(boardXMiddle, puyosSpawnerPoint.position.y);
        int colorindex = Random.Range(0, 4);
        puyo1.image.color = puyosColors[colorindex];
        puyo1.colorIndex = colorindex;

        float random1 = Random.value > 0.5f ? boardXMiddle : boardXMiddle + 1;

        puyo2.transform.position = new Vector3(random1, random1 == boardXMiddle ? puyosSpawnerPoint.position.y + 1f : puyosSpawnerPoint.position.y);
        colorindex = Random.Range(0, 4);
        puyo2.image.color = puyosColors[colorindex];
        puyo2.colorIndex = colorindex;


        puyoPair.Init(puyo1, puyo2, puyosSpeed);
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


        ////
        //if (position.y == 0)
        //{
        //    if (puyoPair.IsBelow(puyo, out PuyoObject otherPuyo))
        //    {
        //        Debug.Log("Is Below");
        //        Vector2Int position2 = Vector2Int.zero;
        //        position2.x = (int)otherPuyo.transform.position.x;
        //        position2.y = (int)otherPuyo.transform.position.y;
        //        StopPuyo(otherPuyo, position2);

        //    }

        //    StopPuyo(puyo, position);

        //}

        //if (inCombo)
        //{

        //}

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
        CheckLink(position, puyo.colorIndex);

        if (puyosMovingInCombo.Contains(puyo))
        {
            //CheckLink(position, puyo.colorIndex);

            puyosMovingInCombo.Remove(puyo);
        }

        if (inCombo && puyosMovingInCombo.Count == 0)
        {
            CheckLink(position, puyo.colorIndex);
            //comboMultiplier = 1;
            //inCombo = false;
        }

        // Debug.Log($"Stop Position {puyo.transform.position} Name: {puyo.name}");

        if (puyoPair.DeletePuyo(puyo))
        {
            ////Check link
            //CheckLink(position, puyo.colorIndex);

            if (!inCombo)
            { 
                InstantiatePuyoPair();
            }
        }
    }




    void CheckLink(Vector2Int pos, int colorIndex)
    {
        //Vector2Int newPos = pos;

        List<PuyoObject> puyosLink = new List<PuyoObject>();

        int lenghtX = boardArrayPuyos.GetLength(0);
        int lenghtY = boardArrayPuyos.GetLength(1) - 1;

        //add the newly stopped puyo
        PuyoObject tempPuyo = boardArrayPuyos[pos.x, pos.y];
        puyosLink.Add(tempPuyo);

        CheckPuyoLink(pos, colorIndex, ref puyosLink);

        //Debug.Log($"Puyos count: { puyosLink.Count}");

        if (puyosLink.Count >= 4)
        {
            if (inCombo)
            {
                comboMultiplier++;
            }


            for (int i = 0; i < puyosLink.Count; i++)
            {
                inCombo = true;
                boardArrayPuyos[(int)puyosLink[i].transform.position.x, (int)puyosLink[i].transform.position.y] = null;
                puyosLink[i].gameObject.SetActive(false);
                AddScore(1 * comboMultiplier);
                //Check for puyosAbove

                for (int j = (int)puyosLink[i].transform.position.y + 1; j < lenghtY; j++)
                {

                    if (boardArrayPuyos[(int)puyosLink[i].transform.position.x, j] == null)
                        continue;
                    if (puyosLink.Contains(boardArrayPuyos[(int)puyosLink[i].transform.position.x, j]))
                        continue;

                    PuyoObject puyoMoving = boardArrayPuyos[(int)puyosLink[i].transform.position.x, j];
                    boardArrayPuyos[(int)puyosLink[i].transform.position.x, j] = null;
                    puyoMoving.movement.StartMoving();
                    puyosMovingInCombo.Add(puyoMoving);
                }
            }
        }

        if (puyosMovingInCombo.Count == 0)
        {
            inCombo = false;
        }

        for (int i = 0; i < lenghtX; i++)
        {
            //Debug.Log($"X: {i} Y: {lenghtY}");
            if (boardArrayPuyos[i, lenghtY] != null)
            {
                gameOver = true;
                gameOverScoreLabel.text = $"Final Score: {score}";
                gameOverPanel.gameObject.SetActive(true);
            }
        }

    }



    void CheckPuyoLink(Vector2Int newPos, int colorIndex, ref List<PuyoObject> puyosLinkList)
    {
        //Check left
        Vector2Int pos = newPos;
        pos.x += (int)MoveGameObject.Direction(Directions.Left).x;
        pos.y += (int)MoveGameObject.Direction(Directions.Left).y;
        CheckPos(pos, colorIndex, ref puyosLinkList);

        //Check right
        pos = newPos;
        pos.x += (int)MoveGameObject.Direction(Directions.Right).x;
        pos.y += (int)MoveGameObject.Direction(Directions.Right).y;
        CheckPos(pos, colorIndex, ref puyosLinkList);

        //Check Up
        pos = newPos;
        pos.x += (int)MoveGameObject.Direction(Directions.Up).x;
        pos.y += (int)MoveGameObject.Direction(Directions.Up).y;
        CheckPos(pos, colorIndex, ref puyosLinkList);

        //Check Down
        pos = newPos;
        pos.x += (int)MoveGameObject.Direction(Directions.Down).x;
        pos.y += (int)MoveGameObject.Direction(Directions.Down).y;
        CheckPos(pos, colorIndex, ref puyosLinkList);
    }

    PuyoObject CheckPos(Vector2Int pos, int colorIndex, ref List<PuyoObject> puyosListLink)
    {
        if (pos.x >= boardArrayPuyos.GetLength(0) || pos.x < 0 ||
            pos.y >= boardArrayPuyos.GetLength(1) || pos.y < 0)
        {
            return null;
        }

        if (boardArrayPuyos[pos.x, pos.y] != null && boardArrayPuyos[pos.x, pos.y].colorIndex == colorIndex)
        {
            //boardArrayPuyos[pos.x, pos.y].gameObject.SetActive(false);
            //boardArrayPuyos[pos.x, pos.y].transform.parent.position = new Vector3(-10, 20);
            if (!puyosListLink.Contains(boardArrayPuyos[pos.x, pos.y]))
            {
                puyosListLink.Add(boardArrayPuyos[pos.x, pos.y]);

                CheckPuyoLink(pos, colorIndex, ref puyosListLink);

                return CheckPos(pos, colorIndex, ref puyosListLink);
            }

        }

        return null;
    }



    public void RotateRight()
    {

        if (ValidateRotation(Directions.Right, out Directions finalDirection))
            puyoPair.Rotate(finalDirection);

    }


    public void RotateLeft()
    {
        if (ValidateRotation(Directions.Left, out Directions finalDirection))
            puyoPair.Rotate(finalDirection);
    }


    public bool ValidateRotation(Directions direction, out Directions finalDirection)
    {
        Vector2 newPos = Vector2.zero;
        Directions puyo2Direction = puyoPair.DirectionOfPuyo2();

        if (puyo2Direction == Directions.None)
        {
            finalDirection = Directions.None;
            return false;
        }

        // Debug.Log("Direction puyo2" + puyo2Direction.ToString());

        Directions puyo2FinalDirection = GetFinalDirection(direction, puyo2Direction);

        newPos.x = (int)puyoPair.Puyo1.transform.position.x +
            (int)MoveGameObject.Direction(puyo2FinalDirection).x;

        newPos.y = (int)puyoPair.Puyo1.transform.position.y +
            (int)MoveGameObject.Direction(puyo2FinalDirection).y;

        //Debug.Log($"New pos {newPos} finalDirection {puyo2FinalDirection} \n" +
        //    $" currentPos {puyoPair.Puyo1.transform.position}");

        //newPos.x = (int)currentPos.x + (int)MoveGameObject.Direction(direction).x;
        //newPos.y = (int)currentPos.y + (int)MoveGameObject.Direction(direction).y;
        //Debug.Log("New Pos: " + newPos);


        try
        {
            if (boardArrayPuyos[(int)newPos.x, (int)newPos.y] == null)
            {
                finalDirection = puyo2FinalDirection;
                return true;
            }
        }
        catch (System.IndexOutOfRangeException)
        {
            finalDirection = Directions.None;
            return false;
        }

        finalDirection = Directions.None;
        return false;
    }

    private static Directions GetFinalDirection(Directions direction, Directions puyo2Direction)
    {
        Directions finalDirection = Directions.None;
        //Rotation to the left
        if (direction == Directions.Left)
        {
            switch (puyo2Direction)
            {
                case Directions.Up:
                    finalDirection = Directions.Left;
                    break;
                case Directions.Down:
                    finalDirection = Directions.Right;
                    break;
                case Directions.Right:
                    finalDirection = Directions.Up;
                    break;
                case Directions.Left:
                    finalDirection = Directions.Down;
                    break;
                default:
                    break;
            }
        }
        else if (direction == Directions.Right)
        {
            switch (puyo2Direction)
            {
                case Directions.Up:
                    finalDirection = Directions.Right;
                    break;
                case Directions.Down:
                    finalDirection = Directions.Left;
                    break;
                case Directions.Right:
                    finalDirection = Directions.Down;
                    break;
                case Directions.Left:
                    finalDirection = Directions.Up;
                    break;
                default:
                    break;
            }
        }

        return finalDirection;
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
        gameOver = false;
        gamePanel.SetActive(true);
        gameOverPanel.SetActive(false);
        puyosPool.DeactivateAllPoolObjects();
        score = 0;
        AddScore(0);
        inCombo = false;
        comboMultiplier = 1;

        puyoPair = new PuyoPair();
        // boardArray = new int[8, 16];
        boardArrayPuyos = new PuyoObject[8, 16];

        InstantiatePuyoPair();
    }

    public void InStartMenu(bool value)

    {
        inStartMenu = value;
    }

    void AddScore(int addScore)
    {
        score += addScore;
        gamePlayScoreLabel.text = $"Score: {score}";
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