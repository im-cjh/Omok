using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct Game
{
    public string name;
    public string description;
    public Sprite Icon;
}

public class Demo : MonoBehaviour
{
    public List<Game> allGames = new List<Game>();
    
    // Start is called before the first frame update
    void Start()
    {
        GameObject buttonTemplate = transform.GetChild(0).gameObject;
        GameObject g;

        int N = allGames.Count;

        for (int i = 0; i < N; i++)
        {
            g = Instantiate(buttonTemplate, transform);
            g.transform.GetChild(0).GetComponent<Image>().sprite = allGames[i].Icon;
            g.transform.GetChild(1).GetComponent<Text>().text = allGames[i].name;
            g.transform.GetChild(2).GetComponent<Text>().text = allGames[i].description;
        }

        Destroy( buttonTemplate );
    }
}
