using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YG;
using TMPro;

public class translator : MonoBehaviour
{
    public string ru;
    public string en;
    private void OnEnable()
    {
        if (YandexGame.SDKEnabled)
            translate();
        else
        YandexGame.GetDataEvent += translate;
    }
    private void OnDisable()
    {
        YandexGame.GetDataEvent -= translate;
    }
    public void translate()
    {
        if (YandexGame.EnvironmentData.language == "ru")
        {
            GetComponent<TMP_Text>().text = ru;
        }
        else
        {
            GetComponent<TMP_Text>().text = en;
        }
    }
}
