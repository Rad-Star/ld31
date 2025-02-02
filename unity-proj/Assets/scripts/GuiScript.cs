﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GuiScript : MonoBehaviour {

	public MegaCarrot carrot;
	public RectTransform bar;
	public GameController controller;
	public DayNightCycleManager dayManager;

	public Text nbDay;
	public Text nbCarrot;
	public Text nbRabbitKilled;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		float t = (float)carrot.mLife / (float)carrot.maxLife;
		bar.transform.localScale = new Vector3(t,1.0f,1.0f);

		nbDay.text = "" + dayManager.mCurrentDay;
		nbCarrot.text = "x" + controller.nbCarrot;
		nbRabbitKilled.text = "x" + controller.getNbRabbitKilled ();
	}
}
