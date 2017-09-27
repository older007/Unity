using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    public Transform Camera;
    int Coins;

	void Start () {
        Camera = GameObject.Find("Camera").GetComponent<Transform>();
        Camera.position = new Vector3(1, 1, -10);
        Camera.SetParent(transform);
	}
    public bool canMove=true;
	// Update is called once per frame
	void Update () {
        if (canMove)
            Move();
	}
    private void Move()
    {
        transform.position = new Vector2(transform.position.x + Input.GetAxis("Horizontal")*Time.deltaTime*3, transform.position.y + Input.GetAxis("Vertical") * Time.deltaTime * 3);
    }
    bool endGame=false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Coin"))
        {
            Manager.Instance.playerCoins += 1;
            Manager.Instance.coins.Remove(collision.gameObject);
            Destroy(collision.gameObject);
            if (Manager.Instance.playerCoins == 5)
            {
                Manager.Instance.SpawnEnemy(5);
            }
            else if (Manager.Instance.playerCoins == 10)
            {
                Manager.Instance.SpawnEnemy(10);
            }
            else if (Manager.Instance.playerCoins > 20)
            {
                
            }
        }
        else if (collision.CompareTag("Zombie")&&!endGame)
        {
            Saver.instance.AddAtEndGame(new Data(Manager.Instance.pName, Timer.Instance.TheGameIsEnd(), Manager.Instance.Pcoin, "Die by Zombie"));
            endGame = true;
            Manager.Instance.PlayerDead();
        }
        else if (collision.CompareTag("Mumi") && !endGame)
        {
            Manager.Instance.playerCoins = 0;
            Saver.instance.AddAtEndGame(new Data(Manager.Instance.pName, Timer.Instance.TheGameIsEnd(), Manager.Instance.Pcoin, "Die by Mumi"));
            endGame = true;
            Manager.Instance.PlayerDead();
        }
    }
    private void OnApplicationQuit()
    {
        if (!endGame)
        {
            Saver.instance.AddAtEndGame(new Data(Manager.Instance.pName, Timer.Instance.TheGameIsEnd(), Manager.Instance.Pcoin, "Exit by User"));
            endGame = true;
        }
    }
}
