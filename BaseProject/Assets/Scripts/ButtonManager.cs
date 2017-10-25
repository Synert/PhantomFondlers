using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    public void NewGame(string NewGameLevel)
    {
		
        SceneManager.LoadScene(NewGameLevel);
    }

    public void ExitGame()
	{
			#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
			#else
			Application.Quit();
			#endif
    }
}
