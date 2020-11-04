using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
   [SerializeField]
   private GameObject debugTools;

    private void Awake()
    {
        // if the game being run in the Editor 
        if (Application.isEditor)
        {
            debugTools.SetActive(true);    // show debug tools 
        }
        else
        {                                  // else 
            debugTools.SetActive(false);   // Do not show tools
        }
    }
}
