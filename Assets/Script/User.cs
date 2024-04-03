using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User : MonoBehaviour
{
    private string _user;
    
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
    }
}
