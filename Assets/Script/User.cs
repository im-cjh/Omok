using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User : MonoBehaviour
{
    public string user_name;
    
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
    }
}
