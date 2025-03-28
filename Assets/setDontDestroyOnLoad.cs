using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class setDontDestroyOnLoad : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
