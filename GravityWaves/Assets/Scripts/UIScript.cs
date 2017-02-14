using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using XInputDotNetPure;
public class UIScript : MonoBehaviour
{

    // Use this for initialization
    public Button StartButton;
	void Start ()
    {
        if (StartButton != null)
            EventSystem.current.SetSelectedGameObject(StartButton.gameObject);
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 4; i++)
        {
            GamePadState state= GamePad.GetState((PlayerIndex)i);
            if (state.Buttons.B == ButtonState.Pressed)
                GoToMainMenu();
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene(2);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void GoToCredit()
    {
        SceneManager.LoadScene(3);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void PlayerCount2()
    {
        PlayerCountSelected(2);
    }

    public void PlayerCount3()
    {
        PlayerCountSelected(3);
    }

    public void PlayerCount4()
    {
        PlayerCountSelected(4);
    }

    private void PlayerCountSelected(int count)
    {
        GameObject obj = GameObject.FindGameObjectWithTag("Menu");
        Destroy(obj);
        GlobalReferences.PlayerCount = count;
        SceneManager.LoadScene(1);
    }
}
