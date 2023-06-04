using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManger : MonoBehaviour
{
    public Button startButton;
    public Button shopButton;
    public Button optionButton;
    private UnityAction action;

    private void Start()
    {
        action =() => OnStartClick();
        startButton.onClick.AddListener(action);
        optionButton.onClick.AddListener(delegate { OnButtonClick(optionButton.name); });
        shopButton.onClick.AddListener(() => OnButtonClick(shopButton.name));
    }
    public void OnButtonClick(string msg)
    {
        Debug.Log($"Click Button : {msg}");
    }
    public void OnStartClick()
    {
        SceneManager.LoadScene("Level_01");
        SceneManager.LoadScene("Play", LoadSceneMode.Additive);
    }
}
