﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class menuScript : MonoBehaviour 
{
	public Canvas quitMenu;
	public Button startText;
	public Button exitText;
	public Button creditsText;

	// Use this for initialization
	void Start () {
	
		quitMenu = quitMenu.GetComponent<Canvas> ();
		startText = startText.GetComponent<Button> ();
		exitText = exitText.GetComponent<Button> ();
		creditsText = creditsText.GetComponent<Button> ();
		quitMenu.enabled = false;

	}
	
	public void ExitPress()
	{
		quitMenu.enabled = true;
		startText.enabled = false;
		exitText.enabled = false;
	}

	public void NoPress()
	{
		quitMenu.enabled = false;
		startText.enabled = true;
		exitText.enabled = true;
	}

	public void Credits()
	{
		SceneManager.LoadScene ("CreditsScene");
	}

	public void StartLevel()
	{
		SceneManager.LoadScene (1);
	}

	public void ExitGame ()
	{
		Application.Quit ();
	}
}
