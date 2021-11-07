using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderGlobal : MonoBehaviour
{
    public void Shop()
    {
        SceneManager.LoadScene("Shop");
    }

    public void Board()
    {
        SceneManager.LoadScene("BoardSysteme");
    }
}