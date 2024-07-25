using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tmpButton : MonoBehaviour
{
    // Start is called before the first frame update
    public void fn()
    {
        GameObject a = GameObject.Find("Canvas/LoadingPanelParent").transform.Find("LoadingPanel").gameObject;
        if (a != null )
        {
            a.SetActive(true);
            
            Debug.Log(a.activeSelf);
        }
        if (a == null)
            Debug.Log("sibal");
        Debug.Log("asdasdasdasdas");

    }
    
}
