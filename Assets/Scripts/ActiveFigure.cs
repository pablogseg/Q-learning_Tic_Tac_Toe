using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveFigure : MonoBehaviour {

	public GameObject X,O;
	public bool active;
	public int figureType;

	void Start(){
		active = false;
		figureType = 2;
	}

	public void activateFigure(string figure){
		if(active) return;
		if(figure == "X"){
			X.SetActive(true);
			figureType = 1;
			active = true;
		}else if(figure == "O"){
			O.SetActive(true);
			figureType = 0;
			active = true;
		}
		
	}
}
