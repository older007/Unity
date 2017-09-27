using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour {
    public int second = new int();
    public int minute = new int();
    public int hours = new int();
    // Use this for initialization
    public static Timer Instance;
    void Start()
    {
        Instance = this;
        StartCoroutine(Time());
    }

    public string TheGameIsEnd()
    {
        StopCoroutine("Time");
        string time = hours + " : " + minute + " : " + second;
        return time;
    }
    
    IEnumerator Time()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            second += 1;
            if (second == 60)
            {
                second = 0;
                minute += 1;
            }
            if (minute == 60)
            {
                minute = 0;
                hours += 1;
            }
            yield return null;
        }
    }
	// Update is called once per frame
	void Update () {
		
	}
}
