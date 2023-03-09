using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using YG;

public class Menu : MonoBehaviour
{
    public static Menu Instance { get; private set; }
    public LeaderboardYG ScoreLb;
    public LeaderboardYG MoneyeLb;

    public GameObject LoadScreen;
    public GameObject MainScreen;
    public GameObject LbScreen;
    public GameObject Cube;
    private void Awake()
    {
        Instance = this;

        

        LoadScreen.SetActive(true);

        MainScreen.SetActive(false);
        LbScreen.SetActive(false);
        Cube.SetActive(false);

        YandexGame.GetDataEvent += Load;
        if (YandexGame.SDKEnabled)
            Load();
    }
    public void Load()
    {
        MainScreen.SetActive(true);
        Cube.SetActive(true);

        LoadScreen.SetActive(false);
        LbScreen.SetActive(false);
    }
    public void OpenLb()
    {
        LbScreen.SetActive(true);
        MainScreen.SetActive(false);

        ScoreLb.UpdateLB();
        MoneyeLb.UpdateLB();
    }
    public void OpenMenu()
    {
        LbScreen.SetActive(false);
        MainScreen.SetActive(true);
    }
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
}