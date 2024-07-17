using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingPanel : MonoBehaviour
{
    public GameObject loadingPanel;
    // Start is called before the first frame update
    void ClosePanel()
    {
        loadingPanel.SetActive(false);
        Debug.Log(loadingPanel.activeSelf);
    }
}
