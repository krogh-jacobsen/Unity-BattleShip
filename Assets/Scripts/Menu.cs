using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
	#region Methods
	public void RestartGame()
    {
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

	public void BackToMenu()
    {
		SceneManager.LoadScene("Menu");
    }

	public void LoadGame(string sceneToLoad) 
	{
		SceneManager.LoadScene(sceneToLoad); 
	}

	#endregion
}
