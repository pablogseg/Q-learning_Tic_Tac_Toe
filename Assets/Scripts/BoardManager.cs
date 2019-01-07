using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour {

	public GameObject[] Cells = new GameObject[9]; 
	bool Turn, Win;
	public Text PlayerTurnText;
	public Camera MainCamera;

    public enum Players
    {
        Player,
        IA,
        Training
    };

    public Players Player1 = Players.Player;
    public Players Player2 = Players.Player;


	string tablero = "---------";

	public QLearning qLearning;
	
	void Start () {
        if(Player2 == Players.Training)
        {
            Debug.Log("Player 2 Training");
            qLearning.Train();
        }
        if(Player2 == Players.IA)
        {
            Debug.Log("Player 2 IA");
            qLearning.ReadDictionary();
        }
		Turn = true;
		Win = false;
		PlayerTurnText.text = "Player 1";
        resetGame();
    }

	void Update(){
        if (!Win)
        {
            turnManager();
            WinManager();
            ReadTablero();
        }
        else {
            if (Input.GetMouseButtonDown(1))
            {
                //Debug.Log("Test");
                resetGame();
            }
        }

	}

    void resetGame()
    {
        Turn = true;
        Win = false;
        PlayerTurnText.text = "Player 1";

        for (int i = 0; i < Cells.Length; i++)
        {
            ActiveFigure table = Cells[i].GetComponent<ActiveFigure>();
            table.figureType = 0;
            table.active = false;
            table.X.SetActive(false);
            table.O.SetActive(false);

        }

    }

    void ReadTablero()
    {
        tablero = "";
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            ActiveFigure AF = gameObject.transform.GetChild(i).GetComponent<ActiveFigure>();
            if (AF.active)
            {
                if (AF.figureType == 0)
                {
                    tablero += "o";
                }
                else
                {
                    tablero += "x";
                }
            }
            else
            {
                tablero += "-";
            }
        }
    }
    
    void updateTablero(int position, int player)
    {
		//tablero = "";
        if(position != -1)
        {
            Debug.Log(position);

            Transform table = gameObject.transform.GetChild(position);

            if (player == 1) table.GetComponent<ActiveFigure>().activateFigure("X");

            else if (player == 2) table.GetComponent<ActiveFigure>().activateFigure("O");
        }
    }

    void turnManager(){

        if (Turn)
        {
            if (Player1 == Players.IA)
            {
                updateTablero(qLearning.Action(tablero, 1), 1);
                Turn = false;
                PlayerTurnText.text = "Player 2";
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit))
                    {
                        ActiveFigure AF = hit.collider.GetComponent<ActiveFigure>();
                        if (!AF.active)
                        {
                            AF.activateFigure("X");
                            Turn = false;
                            PlayerTurnText.text = "Player 2";
                        }
                    }
                }
            }
        }
        else
        {
            if (Player2 == Players.IA)
            {
                updateTablero(qLearning.Action(tablero, 2), 2);
                Turn = true;
                PlayerTurnText.text = "Player 1";
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit))
                    {
                        ActiveFigure AF = hit.collider.GetComponent<ActiveFigure>();
                        if (!AF.active)
                        { 
                            AF.activateFigure("O");
                            Turn = true;
                            PlayerTurnText.text = "Player 1";
                        }
                    }
                }
            }
        }
          
	}

    void WinManager()
    {
        ActiveFigure[] AFs = new ActiveFigure[9];
        bool draw = true;
        for (int k = 0; k < 9; k++) {
            AFs[k] = Cells[k].GetComponent<ActiveFigure>();
            if (!AFs[k].active) {
                draw = false;
            }
        }
        Win = draw;


        for (int i = 0; i < 3; i++)
        {
            if (AFs[i].figureType == AFs[i+3].figureType && AFs[i].figureType == AFs[i + 6].figureType && AFs[i].active == true && AFs[i + 3].active == true && AFs[i + 6].active == true)
            {
                if (AFs[i].figureType == 1)
                {
                    PlayerTurnText.text = "Win player1 V";
                    Win = true;
                    //resetGame();
                    return;

                }
                else if (AFs[i].figureType == 0)
                {
                    PlayerTurnText.text = "Win player2 V";
                    Win = true;
                    //resetGame();
                    return;

                }

            }
            else if (AFs[i*3].figureType == AFs[(i * 3) + 1].figureType && AFs[(i * 3) + 1].figureType == AFs[(i * 3) + 2].figureType && AFs[i * 3].active == true && AFs[(i * 3) + 1].active == true && AFs[(i * 3) + 2].active == true)
            {
                if (AFs[i * 3].figureType == 1)
                {               
                    PlayerTurnText.text = "Win player1 H";
                    Win = true;
                    //resetGame();
                    return;

                }
                else if (AFs[i * 3].figureType == 0)
                {
                    PlayerTurnText.text = "Win player2 H";
                    Win = true;
                    //resetGame();
                    return;

                }
            }
        }

        if (AFs[0].figureType == AFs[4].figureType && AFs[0].figureType == AFs[8].figureType && AFs[0].active == true && AFs[4].active == true && AFs[8].active == true)
        {
            if (AFs[0].figureType == 1)
            {
                PlayerTurnText.text = "Win player1 D";
                Win = true;
                //resetGame();
                return;

            }
            else if (AFs[0].figureType == 0)
            {
                PlayerTurnText.text = "Win player2 D";
                Win = true;
                //resetGame();
                return;

            }
        }
        else if (AFs[2].figureType == AFs[4].figureType && AFs[2].figureType == AFs[6].figureType && AFs[2].active == true && AFs[4].active == true && AFs[6].active == true)
        {
            if (AFs[2].figureType == 1)
            {
                PlayerTurnText.text = "Win player1 D";
                Win = true;
                //resetGame();
                return;

            }
            else if (AFs[2].figureType == 0)
            {
                PlayerTurnText.text = "Win player2 D";
                Win = true;
                //resetGame();
                return;

            }
        }

        if (draw)
        {
            PlayerTurnText.text = "Draw";
            return;
        }
    }
	
}
