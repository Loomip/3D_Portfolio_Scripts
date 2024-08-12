using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TitleScene : MonoBehaviour
{
    [Header("텍스트")]
    [SerializeField] private TextMeshProUGUI txt_Title;
    [SerializeField] private TextMeshProUGUI txt_Start;
    [SerializeField] private TextMeshProUGUI txt_Option;
    [SerializeField] private TextMeshProUGUI txt_Exit;

    [Header("다음씬")]
    [SerializeField] private string nextScene;

    [Header("매뉴게임오브젝트")]
    [SerializeField] private GameObject option;

    // "School" 씬으로 전환
    public void SceneStart()
    {
        // 넘어갈때 창이 켜져있으면 꺼줌
        option.SetActive(false);
        // 다음씬으로 넘어감
        LoadSceneManager.LoadScene(nextScene);
    }

    // 애플리케이션 종료
    public void Clear()
    {
        Application.Quit();
    }

    public void OptionButten()
    {
        option.SetActive(true);
    }

    public void ExitButten()
    {
        option.SetActive(false);
    }

    // 이스케이프 키를 누르면 옵션 메뉴를 닫습니다.
    void Start()
    {
        txt_Title.text = DataManager.instance.GetWordData("Title");
        txt_Start.text = DataManager.instance.GetWordData("Start");
        txt_Option.text = DataManager.instance.GetWordData("Option");
        txt_Exit.text = DataManager.instance.GetWordData("Exit");

        SoundManager.instance.PlayBgm(e_Bgm.Title);

        option.SetActive(false);
    }
}
