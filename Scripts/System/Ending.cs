using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Ending : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txt_ReStart;
    [SerializeField] private TextMeshProUGUI txt_Exit;
    [SerializeField] private TextMeshProUGUI txt_Ending;
    [SerializeField] private Health playerHp;

    public void ReStart()
    {
        gameObject.SetActive(false);
        LoadSceneManager.LoadScene("MainScene");
    }

    public void Exit()
    {
        Application.Quit();
    }

    // Update is called once per frame
    void Start()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        txt_ReStart.text = DataManager.instance.GetWordData("Start");
        txt_Exit.text = DataManager.instance.GetWordData("Final");

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (playerHp.hp > 0)
        {
            txt_Ending.text = DataManager.instance.GetWordData("Ending");
        }
        else if (playerHp.hp <= 0)
        {
            txt_Ending.text = DataManager.instance.GetWordData("ReStart");
        }
    }
}
