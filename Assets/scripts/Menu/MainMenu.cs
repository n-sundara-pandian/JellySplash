using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using EZObjectPools;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public void OnStartGame()
    {
        SceneManager.LoadScene("mvc");
    }
}
