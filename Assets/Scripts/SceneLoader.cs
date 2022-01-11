using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
 public void LoadGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Combat() 
    {
        SceneManager.LoadScene("Combat");
    }

    public void Shop()
    {
        SceneManager.LoadScene("Shop");
    }

    public void BackToBoard() {
        DontDestroy.Instance.gameObject.SetActive(true);
        SceneManager.LoadScene("Board");
    }

    public void Board()
    {
        SceneManager.LoadScene("Board");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit!");
    }
}
