using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class UIScreen
{
    public GameObject Root;
    public RectTransform Screen;
    public Button ActionBtn;
    public GameObject[] ToggleScreens;

    public void Setup()
    {
        Root.SetActive(false);
    }

    public void Display()
    {
        Array.ForEach(ToggleScreens, s => s.SetActive(false));        
        Root.SetActive(true);
    }

    public void Hide()
    {
        Root.SetActive(false);
    }   
}