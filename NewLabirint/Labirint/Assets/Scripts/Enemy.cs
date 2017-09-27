using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Enemy : MonoBehaviour {

    public bool IsMymi;

    public static Enemy Instance;

    private int MapWidht = 12;

    private int MapHeight = 12;

    int[,] WayMap;

    int[,] map1;

    Transform Player;

    Manager map = Manager.Instance;

    int[,] cMap = new int[Manager.height, Manager.width];

    void Start() {
        MapWidht = Manager.width;
        MapHeight = Manager.height;
        Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        //StartDoIt(); //Start walk
        StartCoroutine(RandomWalk());
    }
    void Awake() {
        Instance = this;
    }

    IEnumerator RandomWalk()
    {
        while (true)
        {
            if (Manager.Instance.playerCoins >= 20)
            {
                StartDoIt();
                yield break;
            }
            yield return new WaitForSeconds(IsMymi==true ? Manager.Instance.Gerts/2 : Manager.Instance.Gerts);
            Vector2 temp = FromgameToCode(transform.position);
            int temper = Random.Range(1, 3);
            if (!map.blocks[Mathf.RoundToInt(temp.x)+1, Mathf.RoundToInt(temp.y)].isWall&&temper==1)
            {
                transform.position=(map.blocks[Mathf.RoundToInt(temp.x) + 1, Mathf.RoundToInt(temp.y)].posIngame);
            }
            else if (!map.blocks[Mathf.RoundToInt(temp.x) - 1, Mathf.RoundToInt(temp.y)].isWall && temper == 2)
            {
                transform.position = (map.blocks[Mathf.RoundToInt(temp.x) - 1, Mathf.RoundToInt(temp.y)].posIngame);
            }
            else if (!map.blocks[Mathf.RoundToInt(temp.x) , Mathf.RoundToInt(temp.y) + 1].isWall && temper == 1)
            {
                transform.position = (map.blocks[Mathf.RoundToInt(temp.x) , Mathf.RoundToInt(temp.y) + 1].posIngame);
            }
            else if (!map.blocks[Mathf.RoundToInt(temp.x) , Mathf.RoundToInt(temp.y) - 1].isWall && temper == 2)
            {
                transform.position = (map.blocks[Mathf.RoundToInt(temp.x) , Mathf.RoundToInt(temp.y) - 1].posIngame);
            }
             yield return null;
        }
    }

    public void StartDoIt() //start Graph
    {
        RebildMap();
        StartCoroutine(Mover());

    }
    void RebildMap()
    {
        map1 = new int[MapWidht, MapHeight];
        for (int i = 0; i < MapHeight; i++)
        {
            for (int j = 0; j < MapWidht; j++)
            {
                if (map.blocks[i,j].isWall)
                {
                    map1[i, j] = -2;
                }
                else 
                {
                    map1[i, j] = -1;
                }
            }
        }
    }
    Vector2 FromgameToCode(Vector2 pos)
    {
        foreach (var x in map.blocks)
        {
            if (x.posIngame == new Vector2(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y)))
            {
                return x.posIngame;
            }
        }
        return Vector2.zero;
    }
    Vector2 FromCodeToGame(Vector2 pos)
    {
        foreach (var x in map.blocks)
        {
            if (x.x == pos.x && x.y == pos.y)
            {
                return x.posIngame;
            }
        }
        return Vector2.zero;
    }
    void AddItemOnIndex(int i, int j, ref int[,] mas, int item)
    {
        mas[i, j] = item;
    }
    public void ReadMap()
    {
        MapWidht = Manager.width;
        MapHeight = Manager.height;
    }
    void UnityFindWave()
    {
        Vector2 player = FromgameToCode(new Vector2(Player.position.x,Player.position.y));
        Vector2 enemy = FromgameToCode(transform.position);
        FindWave(Mathf.RoundToInt(enemy.x), Mathf.RoundToInt(enemy.y), Mathf.RoundToInt(player.x), Mathf.RoundToInt(player.y));
    }

    public void FindWave(int startX, int startY, int targetX, int targetY)
    {
        cMap = map1;
        bool add = true;
        
        int x, y, step = 0;
        for (y = 0; y < MapHeight; y++)
            for (x = 0; x < MapWidht; x++)
            {
                if (map1[x, y] == -2)
                    cMap[x, y] = -2;//индикатор стены
                else
                    cMap[x, y] = -1;//индикатор еще не ступали сюда
            }
        cMap[targetX, targetY] = 0;//Начинаем с финиша
        while (add == true)
        {
            add = false;
            for (y = 1; y < MapHeight-1; y++)
                for (x = 1; x < MapWidht-1; x++)
                {
                    if (cMap[x, y] == step)
                    {
                        //Ставим значение шага+1 в соседние ячейки (если они проходимы)
                        if (y - 1 >= 0 && cMap[x - 1, y] != -2 && cMap[x - 1, y] == -1)
                            cMap[x - 1, y] = step + 1;
                        if (x - 1 >= 0 && cMap[x, y - 1] != -2 && cMap[x, y - 1] == -1)
                            cMap[x, y - 1] = step + 1;
                        if (y + 1 < MapWidht && cMap[x + 1, y] != -2 && cMap[x + 1, y] == -1)
                            cMap[x + 1, y] = step + 1;
                        if (x + 1 < MapHeight && cMap[x, y + 1] != -2 && cMap[x, y + 1] == -1)
                            cMap[x, y + 1] = step + 1;
                    }
                }
            step++;
            add = true;
            if (cMap[startX, startY] != -1)//решение найдено
                add = false;
            if (step > MapWidht * MapHeight)//решение не найдено
                add = false;
            for (x = 0; x < MapHeight; x++)
            {
                for (y = 0; y < MapWidht; y++)
                    if (map1[x, y] == -1)
                        cMap[x, y] = -1;
                    else
                        if (map1[x, y] == -2)
                    {
                        cMap[x, y] = -2;
                    }
                    else
                        if (y == startY && x == startX)
                    {
                        cMap[x, y] = -3;
                    }
                    else
                        if (y == targetY && x == targetX)
                    {
                        cMap[x, y] = 0;
                    }
                    else
                    if (map1[y, x] > -1)
                    {
                        cMap[y, x] = map1[y, x];
                    }

            }
        }
    }
    Vector2 finder()
    {
        List<int> s = new List<int>();
        List<Vector2> vects = new List<Vector2>();
        UnityFindWave();
        for (int i = 0; i < Manager.height; i++)
        {
            for (int j = 0; j < Manager.width; j++)
            {
                if (cMap[i,j] == -3)
                {
                    s.Clear() ;
                    vects.Clear();
                    if (cMap[i + 1, j] > 0)
                    {
                        vects.Add(new Vector2(i + 1, j));
                        s.Add(cMap[i + 1, j]);
                    }
                    if (cMap[i - 1, j] > 0 )
                    {

                        vects.Add(new Vector2(i - 1, j));
                        s.Add(cMap[i - 1, j]);
                    }
                    if (cMap[i, j + 1] > 0)
                    {

                        vects.Add(new Vector2(i , j + 1));
                        s.Add(cMap[i, j + 1]);
                    }
                    if (cMap[i, j - 1] > 0)
                    {

                        vects.Add(new Vector2(i, j - 1));
                        s.Add(cMap[i, j - 1]);
                    }
                    if (vects.Count == 0)
                    {

                        print("TheGameIsEnd");
                        return Player.transform.position;
                    }
                    return vects[0];      
                }
            }
        }
        return Player.transform.position;
    }
    IEnumerator Mover()
    {
        while (true)
        {
            transform.position = (finder());
            yield return new WaitForSeconds(IsMymi == true ? Manager.Instance.Gerts / 2 : Manager.Instance.Gerts);
            yield return null;
        }
    }
}