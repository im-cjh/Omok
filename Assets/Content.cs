using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Content : MonoBehaviour
{
    ScrollRect scrollView;
    // Start is called before the first frame update
    void Start()
    {
        scrollView = GetComponent<ScrollRect>();
        LobbyManager.Instance.OnSceneChanged(scrollView);
        
    }
}
