using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Globalization;


public class QLearning : MonoBehaviour {

	private Dictionary<string, float> board1 = new Dictionary<string, float>();
	private Dictionary<string, float> board2 = new Dictionary<string, float>();
	private float epsilon = 1.0f, gamma = 0.6f, alpha = 0.8f;

    public TextAsset LearningTable1;
    public TextAsset LearningTable2;

	private string boardInGame;
	int NumFicheros = 1, NumPartidas = 200000; 
	bool finish;
	string lastAction1, lastAction2 ;
    
    public void Train()
    {
        Debug.Log("ENTRENANDO");
        for (int i = 0; i < NumFicheros; i++)
        {
            for (int j = 0; j < NumPartidas; j++)
            {
                Partida();
                epsilon -= 1.0f/(float) NumPartidas;

            }
            SaveDictionary();
        }
    }

    void Partida()
    {
        finish = false;
        bool turn = false;
        boardInGame = "---------";
        lastAction2 = "---------";

        lastAction1 = Action(1);

        while (!finish)
        {
            if (turn)
            {
                lastAction1 = Action(1);
                turn = false;
            }
            else if (!finish)
            {
                lastAction2 = Action(2);
                turn = true;
            }
        }
    }

	public int Action(string board, int player){
		float maxValue = -Mathf.Infinity;
		boardInGame = board;
		int bestAction = -1;

        if(player == 1)
        {
            for (int i = 0; i < 9; i++)
            {
                if (board1.ContainsKey(boardInGame + i))
                {
                    if (board1[boardInGame + i] >= maxValue)
                    {
                        bestAction = i;
                        maxValue = board1[boardInGame + i];
                    }
                }
            }
        }

        else if (player == 2)
        {
            for (int i = 0; i < 9; i++)
            {
                if (board2.ContainsKey(boardInGame + i))
                {
                    if (board2[boardInGame + i] >= maxValue)
                    {
                        bestAction = i;
                        maxValue = board2[boardInGame + i];
                    }
                }
            }
        }
        Debug.Log("Beast Action " + bestAction);

        if (bestAction == -1) {
            List<int> pos = new List<int>();
            for (int i = 0; i < 9; i++)
            {
                if (boardInGame[i] == '-')
                {
                    pos.Add(i);
                }
            }
            if (pos.Count > 0) bestAction = pos[Random.Range(0, pos.Count - 1)]; 
        }
		return bestAction;
	}

	string Action(int board){
		//int randAction = Random.Range(0,9);
		int newAction = 0;
		string play = "";
        float maxValue = -Mathf.Infinity;
        string lastBoard = boardInGame;

        //HACER UNA JUGADA NO ALEATORIA
       if (Random.Range(0f,1f) > epsilon){//Si el valor aleatorio es > e, buscamos la mejor recompensa en el diccionario
			int bestAction = 0;
			if(board == 1){
				for(int i = 0; i < 9; i++){
					if(board1.ContainsKey(boardInGame + i)){
						if(board1[boardInGame + i] >= maxValue){
							bestAction = i;
							maxValue = board1[boardInGame + i];
						}
					}
				}
			}else if(board == 2){
				for(int i = 0; i < 9; i++){
					if(board2.ContainsKey(boardInGame + i)){
						if(board2[boardInGame + i] >= maxValue){
							bestAction = i;
							maxValue = board2[boardInGame + i];
						}
					}
				}
			}
			if(maxValue > -1) { //Si existe alguna jugada conocida
                play = boardInGame + bestAction;

                if (board == 1)
                {
                    if (board1.ContainsKey(boardInGame + bestAction))
                    {
                        newBoard(bestAction, 1);
                        return play;
                    }
                }
                else if (board == 2)
                {
                    if (board2.ContainsKey(boardInGame + bestAction))
                    {
                        newBoard(bestAction, 2);
                        return play;
                    }
                }

			}
	    }

       //HACER UNA JUGADA ALEATORIA

		List<int> pos= new List<int>();
		for (int i = 0; i < 9; i++)                 //Añadimos las casillas vacias
		{
			if(boardInGame[i]== '-'){
				pos.Add(i);
			}
		}
		newAction = pos[Random.Range(0, pos.Count)]; //Elegimos una casilla que este vacia y tiramos ahi nuestra ficha
		play = boardInGame + newAction;
	
		if(board == 1){
			if(!board1.ContainsKey(play)){
				board1.Add(play,0);
			}
			newBoard(newAction, 1);
			return play;
		}else if(board == 2){
			if(!board2.ContainsKey(play)){
				board2.Add(play, 0);
			}	
			newBoard(newAction, 2);
			return play;
		}
		return "error";
	}

	void newBoard(int pos, int player){
		string newboard = "";
		string lastBoard = boardInGame;
        float maxValue = -Mathf.Infinity;
        
        for (int i = 0; i < 9; i++)
		{
			if(i == pos){
				if(player == 1){
					newboard += "x";
				}else{
					newboard += "o";
				}
			}else{
				newboard += boardInGame[i];
			}	
		}

        int res = Win(newboard, player); // 1 si gana 1 ,2 si gana 2, 0 si es empate, -1 si no se ha acabado la partido

        if (res >= 0){ //GANA ALGUIEN DE LOS 2
			finish = true;
			if(res==1)//VICTORIA JUGADOR 1
            {
                board1[boardInGame + pos] = board1[boardInGame + pos] + gamma * (1 + alpha * 0 - board1[boardInGame + pos]);
                board2[lastAction2] = board2[lastAction2] + gamma*(-1 + alpha * 0 - board2[lastAction2]);							
			}
            else if(res==2)//VICTORIA JUGADOR 2
            {																									
                board1[lastAction1] = board1[lastAction1] + gamma*(-1 + alpha * 0 - board1[lastAction1]);
                board2[boardInGame + pos] = board2[boardInGame + pos] + gamma*(1 + alpha * 0 - board2[boardInGame + pos]);			
            }
            else if (res==0)//EMPATE
            {																								
				board1[lastAction1] = board1[lastAction1] + gamma * (0 + alpha * 0 - board1[lastAction1]);
				board2[lastAction2] = board2[lastAction2] + gamma * (0 + alpha * 0 - board2[lastAction2]); 							
			}
		}

        else {
			if(player == 1){
                for (int i = 0; i < 9; i++)
                {   
                    if (board2.ContainsKey(newboard + i) && board2[newboard + i] >= maxValue)
                    {
                        maxValue = board2[newboard + i];
                    }                    
                }
                if (maxValue == -Mathf.Infinity) maxValue = 0;
                if(lastAction2 != null && board2.ContainsKey(lastAction2))
                {
					board2[lastAction2] = board2[lastAction2] + gamma*(0+alpha*maxValue - board2[lastAction2]);
				} 
                
			}else if(player == 2){
				for(int i = 0; i < 9; i++)
                {
				    if(board1.ContainsKey(newboard + i) && board1[newboard + i] >= maxValue)
                    {
					    maxValue = board1[newboard + i];
					}
				}	
				if(maxValue == -Mathf.Infinity) maxValue = 0;
                if (board1.ContainsKey(lastAction1))
                {
                    board1[lastAction1] = board1[lastAction1] + gamma * (0 + alpha * maxValue - board1[lastAction1]);
                }
            }
        }
        boardInGame = newboard;

    }

    int Win(string newboard, int player){   // 1 si gana 1 ,2 si gana 2, 0 si es empate, -1 si no se ha acabado la partido

		char[,] boardMatrix = new char[3,3];
		for(int i = 0; i<3; i++){
			for(int j = 0; j<3; j++){
				boardMatrix[i,j]= newboard[i*3+j];
			}
		}
		for(int i = 0; i<3; i++){
			if(boardMatrix[i,0] == boardMatrix[i,1] && boardMatrix[i,0] == boardMatrix[i,2] && boardMatrix[i,0] != '-'){
				return player;
			}
			if(boardMatrix[0,i] == boardMatrix[1,i] && boardMatrix[0,i] == boardMatrix[2,i] && boardMatrix[0,i] != '-'){
			return player;
			}
		}
		if(boardMatrix[0,0] == boardMatrix[1,1] && boardMatrix[0,0] == boardMatrix[2,2] && boardMatrix[1,1] != '-' || boardMatrix[2,0] == boardMatrix[1,1] && boardMatrix[2,0] == boardMatrix[0,2] && boardMatrix[1,1] != '-'){
			return player;
		}
		else{
			for (int i = 0; i < 9; i++)
			{
				if(newboard[i] == '-'){
					return -1;
				}
			}
			return 0;
		}

	}

    public void ReadDictionary()
    {
        string path = Application.dataPath + "/" + LearningTable1.name + ".txt";

        StreamReader reader = new StreamReader(path);

        var styles = NumberStyles.AllowParentheses | 
            NumberStyles.AllowLeadingSign | 
            NumberStyles.AllowTrailingSign | 
            NumberStyles.Float | 
            NumberStyles.AllowDecimalPoint;

        string aux = reader.ReadLine();
        string boardToSave = "";
        string rewardString = "";

        while (aux != "")
        {
            for (int i = 0; i < aux.Length; i++)
            {
                if (i < 10) boardToSave += aux[i];
                else if (i > 10)
                {
                    rewardString += aux[i];
                }
            }

            float reward = float.Parse(rewardString, styles);
            board1[boardToSave] = (float) reward;

            aux = reader.ReadLine();
            boardToSave = "";
            rewardString = "";
        }

        Debug.Log("Board1 size = " + board1.Count);

        reader.Close();

        path = Application.dataPath + "/" + LearningTable2.name + ".txt";

        reader = new StreamReader(path);

        aux = reader.ReadLine();
        boardToSave = "";
        rewardString = "";

        while (aux != "")
        {
            for (int i = 0; i < aux.Length; i++)
            {
                if (i < 10) boardToSave += aux[i];
                else if (i > 10)
                {
                    rewardString += aux[i];
                }
            }

            float reward = float.Parse(rewardString, styles);
            board2[boardToSave] = (float)reward;

            aux = reader.ReadLine();
            boardToSave = "";
            rewardString = "";
        }

        Debug.Log("Board2 size = " + board2.Count);


    }

    void SaveDictionary()
    {
        File.Create(Application.dataPath + "/QL1.txt").Dispose();
        if (board1 != null)
        {
            string fileContent = "";

            foreach (var item in board1)
            {
                fileContent += item.Key + "," + item.Value + '\n';
            }
            StreamWriter writer = new StreamWriter(Application.dataPath + "/QL1.txt");

            writer.WriteLine(fileContent);
            writer.Close();
        }
        File.Create(Application.dataPath + "/QL2.txt").Dispose();
        if (board2 != null)
        {
            string fileContent = "";

            foreach (var item in board2)
            {
                fileContent += item.Key + "," + item.Value + '\n';
            }
            StreamWriter writer = new StreamWriter(Application.dataPath + "/QL2.txt");

            writer.WriteLine(fileContent);
            writer.Close();
        }

    }
}
