using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

public class Saver : MonoBehaviour {

    public GameObject item;

    XmlSerializer formatter;

    public Transform trnsfrm;

    public static Saver instance;

    internal Save save;
    private void Start()
    {
        instance = this;
        save = new Save();
        formatter = new XmlSerializer(typeof(Save));
    }

    public void Read()
    {
        using (FileStream fs = new FileStream("persons.xml", FileMode.OpenOrCreate))
        {
            if (fs.Length > 1)
            {
                Save saver = (Save)formatter.Deserialize(fs);
                save = saver;
                //Console.WriteLine("Объект десериализован");
                foreach (var x in saver.data)
                {
                    //Console.WriteLine("Имя: {0} --- Монеты: {1}", x.name, x.coin);
                }
            }

        }
    }
    public void Score()
    {
        using (FileStream fs = new FileStream("data.xml", FileMode.OpenOrCreate))
        {
            if (fs.Length > 1)
            {
                Save saver = (Save)formatter.Deserialize(fs);

                save = saver;

                foreach (var x in save.data)
                {
                    var item = Instantiate(this.item, trnsfrm);
                    item.GetComponent<Text>().text = x.name + " " + x.coins + "\r\n" + x.dieReson + "\r\n" + x.date + "\r\n, Time in Game " + x.time;
                }
            }

        }
    }

    public void AddAtEndGame(Data dat)
    {
        Manager.Instance.endText.text = dat.dieReson + " \r\n" + "Coins : " + dat.coins;
        Read();
        save.Add(dat);
        Write();
        Invoke("ReturnMenu", 3f);
    }
    public void ReturnMenu()
    {
        SceneManager.LoadScene("Menu");
    }
    public void Write()
    {
        
        using (FileStream fs = new FileStream("data.xml", FileMode.OpenOrCreate))
        {
            formatter.Serialize(fs, save);
        }
    }

}
[SerializeField]
public class Save
{
    public List<Data> data = new List<Data>();
    public void Add(Data d)
    {
        data.Add(d);
    }
    public Save()
    {

    }
}
[SerializeField]
public class Data
{
    public string name;
    public string time;
    public int coins;
    public string dieReson;
    public string date;
    public Data()
    {

    }
    public Data(string name, string time, int coins, string dieReson)
    {
        this.name = name;
        this.time = time;
        this.coins = coins;
        this.dieReson = dieReson;
        date = System.DateTime.Now.ToString("dd MMMM yyyy");
    }
}
