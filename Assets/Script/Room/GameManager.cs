using PimDeWitte.UnityMainThreadDispatcher;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    private bool isStarted = false;

    [SerializeField]
    GameObject _panel;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
            }
            if (_instance == null)
            {
                Debug.Log("GameManager is null");
            }
            return _instance;
        }
   }

    private void Update()
    {
        //Debug.Log(isStarted + " is stared");
        if (isStarted)
        {
            _panel.SetActive(false);
        }
    }

    public void CloseLoadingPanel()
    {
        Debug.Log("CloseLoadingPanel");
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            try
            {
                if (_panel == null)
                {
                    Debug.LogError("_panel is null");
                    return;
                }

                _panel.SetActive(false);
                Instance.isStarted = true;
            }
            catch (Exception ex)
            {
                Debug.LogError("Exception caught: " + ex.Message);
                Utilities.WriteErrorLog(ex);
            }
        });
    }


}
