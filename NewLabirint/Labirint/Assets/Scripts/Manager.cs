using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    public string pName;

    public const int height = 20;

    public const int width = 20;
    
    public cell[,] blocks = new cell[height, width];

    public GameObject wall;

    public GameObject flour;

    public GameObject Player;

    public GameObject Enemy;

    public GameObject Mymi;

    public GameObject Coin;

    public Text endText;

    public int Pcoin=0;

    public int playerCoins
    {
        get
        {
            return Pcoin;
        }
        set
        {
            if(Pcoin > 20)
            {
                Gerts = Gerts - Gerts * 5 / 100;
            }
            Pcoin = value;
        }
    }

    public static Manager Instance;

    public Transform parent;

    public List<GameObject> coins = new List<GameObject>();

    public float Gerts=2;
    Vector2 PlayerSpawn = new Vector2(1, 1);

    Vector2[] EnemySpawn = { new Vector2(1, height - 2), new Vector2(height - 2, width - 2), new Vector2(height - 2, 1) };


    IEnumerator SpawnCoin()
    {
        while (true)
        {
            yield return new WaitForSeconds(5);
            if (coins.Count < 10)
            {
                point:
                int x = UnityEngine.Random.Range(0, height);
                int y = UnityEngine.Random.Range(0, height);
                if (!(blocks[x, y].isWall))
                {
                    var obj = Instantiate(Coin, blocks[x, y].posIngame, Quaternion.identity);
                    coins.Add(obj);
                }
                else
                {
                    goto point;
                }
            }
            yield return null;
        }
    }
    public void PlayerDead()
    {
        StopAllCoroutines();
        Time.timeScale = 0;
    }
    void Awake()
    {
        pName = PlayerPrefs.GetString("Name","Player");
        playerCoins = 0;
        Instance = this;
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                blocks[i, j] = new cell();
                blocks[i, j].x = i;
                blocks[i, j].y = j;
            }
        }
        for (int i = 1; i < height-1; i++)
        {
            for (int j = 1; j < width-1; j++)
            {
                if ((i % 2 != 0 && j % 2 != 0) && //если ячейка нечетная по x и y, 
                   (i < height - 1 && j < width - 1))   //и при этом находится в пределах стен лабиринта
                    blocks[i, j].isWall = false;       //то это КЛЕТКА
                else blocks[i, j].isWall = true;           //в остальных случаях это СТЕНА.
            }
        }
        ChangeBox();
        Checker();
        SpawnMass();
        StartCoroutine(SpawnCoin());
    }
    struct Vlozh
    {
        public bool added;
        public int x, y;
    }
    void Checker()
    {
        for (int i = 1; i < height - 1; i++)
        {
            for (int j = 1; j < width - 1; j++)
            {
                if (!blocks[i, j].isWall)
                {
                    Vlozh[] dots = new Vlozh[4];
                    for (int d = 0; d < 4; d++)
                    {
                        dots[d] = new Vlozh();
                    }
                    if (blocks[i + 1, j].isWall)
                    {
                        dots[0].added = true;
                        dots[0].x = i;
                        dots[0].y = j;
                    }
                    if (blocks[i - 1, j].isWall)
                    {
                        dots[1].added = true;
                        dots[1].x = i;
                        dots[1].y = j;
                    }
                    if (blocks[i, j + 1].isWall)
                    {
                        dots[2].added = true;
                        dots[2].x = i;
                        dots[2].y = j;
                    }
                    if (blocks[i, j - 1].isWall)
                    {
                        dots[3].added = true;
                        dots[3].x = i;
                        dots[3].y = j;
                    }
                    foreach (var x in dots)
                    {
                        if (x.added == false)
                        {
                            blocks[x.x, x.y].isWall = true;
                        }
                    }
                }

            }
        }
        blocks[0, 0].isWall = true;
        for (int i = 1; i < height - 1; i++)
        {
            for (int j = 1; j < width - 1; j++)
            {
                if (blocks[i, j].isWall && !blocks[i + 1, j].isWall && !blocks[i - 1, j].isWall && !blocks[i, j + 1].isWall && !blocks[i, j - 1].isWall)
                {
                    blocks[i, j].isWall = false;
                }
            }
        }
        for (int i = 1; i < width - 1; i++)
        {
            blocks[1, i].isWall = false;
            blocks[i, 1].isWall = false;
            blocks[height - 2, i].isWall = false;
            blocks[i, width - 2].isWall = false;
        }
        for (int i = 1; i < height - 1; i++)
        {
            for (int j = 1; j < width - 1; j++)
            {
                if (!blocks[i, j].isWall && blocks[i + 1, j].isWall && blocks[i - 1, j].isWall && blocks[i, j + 1].isWall && blocks[i, j - 1].isWall)
                    blocks[i, j].isWall = true;
            }
        }
    }
    void SpawnMass()
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (blocks[i, j].isWall)
                {
                    var x = Instantiate(wall, new Vector2(i, j), Quaternion.identity);
                    x.transform.SetParent(parent);
                    blocks[i, j].posIngame = x.transform.position;
                    
                }
                else
                {
                    var x = Instantiate(flour, new Vector3(i, j,1), Quaternion.identity);
                    x.transform.SetParent(parent);
                    blocks[i, j].posIngame = x.transform.position;
                }
                
            }
        }
        Instantiate(Player, PlayerSpawn, Quaternion.identity);

        Instantiate(Enemy, EnemySpawn[0], Quaternion.identity);
       // Instantiate(Enemy, EnemySpawn[2], Quaternion.identity);
       // Instantiate(Enemy, EnemySpawn[1], Quaternion.identity);
    }
    public void SpawnEnemy(int coin)
    {
        Instantiate(coin == 5 ? Enemy : Mymi, EnemySpawn[coin == 5 ? 1 : 2], Quaternion.identity);
    }
    void ChangeBox()
    {
        for (int i = 1; i < height - 1; i++)
        {
            for (int j = 1; j < width - 1; j++)
            {
                if (blocks[i,j].isWall)
                {
                    if (UnityEngine.Random.Range(0, 2) == 1)
                    {
                        blocks[i, j].isWall = false;
                    }
                }
            }
        }           
    }

    Vector2 Position(int x, int y)
    {
        return new Vector2(x, y);
    }
}
public static class NewGameObject
{
    public static void GetComp(this Rigidbody2D x, GameObject gm, ref Rigidbody2D orig)
    {
        if (orig == null)
            gm.AddComponent<Rigidbody2D>();
        orig = gm.GetComponent<Rigidbody2D>();
    }
    public static void GetComp(this Rigidbody x, ref Rigidbody orig, GameObject gm)
    {
        orig = gm.GetComponent<Rigidbody>();
    }
    public static void Move(this Rigidbody2D x, Rigidbody2D orig, GameObject gm, float direct)
    {
        orig.transform.position = new Vector2(orig.transform.position.x + direct, orig.transform.position.y);
    }
}
public class cell
{
    public int x = new int();
    public int y = new int();
    public bool isWall=true;
    public Vector2 posIngame = new Vector2();
}