using UnityEngine;
using System.Collections;

public class GameLogic : MonoBehaviour {
	
	public GameObject cross;
	public GameObject sphere;
	public static int steps = 0;
	//represents the states of game
	public enum Game_States{ CONTINUE, PLAYER_WON, COMP_WON, TIE };
	//public GameObject camera;
	
	private int lastBox = -1;
	private bool hasUserPlayed = false;
	private bool continueComp = false;
	private int[] grid = new int[9];
	private AICompMinimax Ai_Comp;

	//Remember: 0-EMPTY 1-CROSS 2-SPHERE
	// Use this for initialization
	void Start () {
		Debug.Log ("Here");
		for (int i = 0; i<9; i++) {
			//Initialize each square of grid as empty
			grid[i] = 0;
		}
		steps = 0;
		//currently I initiate the AIComp class contructor here which in future will be elsewhere
	}
	
	// Update is called once per frame
	void Update () {
//		Debug.Log (grid [0] + " " + grid [1] + " " + grid [2] + "\n" + 
//			grid [3] + " " + grid [4] + " " + grid [5] + "\n" + 
//			grid [6] + " " + grid [7] + " " + grid [8]);
	 	if (!hasUserPlayed && Input.GetMouseButtonDown(0)) {
			//wait for the input
			hasUserPlayed = true;
			Debug.Log("User Clicked");
			Vector3 clickPos1 =  Input.mousePosition;
			clickPos1.z = 12;
			Vector3 clickPos = Camera.main.ScreenToWorldPoint(clickPos1);
			float x_pos = clickPos.x;
			float y_pos = clickPos.y;
			Debug.Log(clickPos.x + " " + clickPos.y + " " + clickPos.z);

			//here we call a function which will return a box relative to the touch coordinates

			int box = ComputeBox(x_pos,y_pos);

			if(grid[box] != 0)
			{
				hasUserPlayed = false;
			}else{
				grid[box] = 1;
				steps++;
				int[] center = ComputeCenter(box);
				InstantiateCross(center);
		}

		if (hasUserPlayed && continueComp) {//along with hasUserPlayed we need to add one another condition.I will write that later.
			continueComp = false;
			Debug.Log ("cpu turn");
			CalculateAIAndInstantiateSphere();
		}
	}
	}

	//This method will be called for comp and player both
	void InstantiateCross(int[] center)
	{
		//Here we need to check the input and detemine the center where the prefab needs to be placed
		//ReturnCenter also needs to marks the square with 1
		//Instantiate cross for the user
		Instantiate (cross, new Vector3 (center [0], center [1], 0), Quaternion.identity);
		//Now compute the logic
		Game_States current_state =  CheckPattern (1);
		if (current_state == Game_States.PLAYER_WON) {
			//End the game and display Player wins
			Debug.Log("Player Won");
		} else {
			//This will start the process for comp
			if(steps<9)
			continueComp = true;
		}
	}

	void CalculateAIAndInstantiateSphere()
	{
		//
		//Hard Strategy
		//
		Ai_Comp = new AICompMinimax (grid);
		Ai_Comp.setChance (AIComp.Chance.COMP);


		//
		//Easy Strategy
		//

		//This function performs tasks for AI and plays the computer chance
//		bool found = false;
//		int sphere_box = -1;
//		while (!found) {
//			int next_box = Random.Range(0,8);
//			if(grid[next_box] == 0)
//			{
//				sphere_box = next_box;
//				found = true;
//				grid[sphere_box] = 2;
//				steps++;
//			}
//		}

		int sphere_box = Ai_Comp.move();
		int[] center = ComputeCenter (sphere_box);
		grid[sphere_box] = 2;
		steps++;
		Debug.Log ("Instantiating Sphere");
		Instantiate(sphere, new Vector3 (center [0], center [1], 0), Quaternion.identity);
		Game_States current_state =  CheckPattern (2);
		if (current_state == Game_States.COMP_WON) {
			//End the game and display Player wins
			Debug.Log("Comp Won");
		} else {
			//This will start the process for comp
			hasUserPlayed = false;
		}

		Debug.Log ("Click User");

	}
		

	public Game_States CheckPattern(int chance)
	{
		//Now we can check Pattern here

		Debug.Log ("Checking Pattern "+chance);
		Game_States myState = Game_States.CONTINUE;
		bool state = false;

		//This is to check if any one has won
		switch(lastBox)
		{
			case 0 :
				if((grid[1] == chance && grid[2] == chance) || (grid[3] == chance && grid[6] == chance) || (grid[4] == chance && grid[8] == chance))
					state = true;
				break;
			case 1:
				if((grid[0] == chance && grid[2] == chance) || (grid[4] == chance && grid[7] == chance))
					state = true;
				break;
			case 2:
				if((grid[0] == chance && grid[1] == chance) || (grid[5] == chance && grid[8] == chance) || (grid[4] == chance && grid[6] == chance))
					state = true;
				break;
			case 3:
				if((grid[0] == chance && grid[6] == chance) || (grid[4] == chance && grid[5] == chance))
					state = true;
				break;
			case 4:
			if((grid[0] == chance && grid[8] == chance) || (grid[1] == chance && grid[7] == chance)|| (grid[2] == chance && grid[6] == chance)||(grid[3] == chance && grid[5] == chance))
					state = true;
				break;
			case 5:
				if((grid[3] == chance && grid[4] == chance) || (grid[2] == chance && grid[8] == chance))
					state = true;
				break;
			case 6:
			if((grid[0] == chance && grid[3] == chance) || (grid[7] == chance && grid[8] == chance) || (grid[2] == chance && grid[4] == chance))
					state = true;
				break;
			case 7:
				if((grid[1] == chance && grid[4] == chance) || (grid[6] == chance && grid[8] == chance))
					state = true;
				break;
			case 8:
			if((grid[2] == chance && grid[5] == chance) || (grid[6] == chance && grid[7] == chance) || (grid[0] == chance && grid[4] == chance))
				state = true;
			break;
		}
		if(state)
		{
			if(chance == 1)
			{
				myState = Game_States.PLAYER_WON;
			}
			else{
				myState = Game_States.COMP_WON;
			}
		}

		return myState;
	}

	int ComputeBox(float x, float y)
	{
		int box = -1;
		if (x > 2) {
			//then 2, 5, 8 possible
			if (y > 2) {
				box = 2;
				//Square 2
			} else if (y < -2) {
				//square 8
				box = 8;
			} else {
				//Square 5(between -2 and 2)
				box = 5;
			}
		} else if (x < -2) {
			if (y > 2) {
				//Square 0
				box = 0;
			} else if (y < -2) {
				//square 6
				box = 6;
			} else {
				//Square 3(between -2 and 2)
				box = 3;
			}
		} else {
			if (y > 2) {
				//Square 1
				box = 1;
			} else if (y < -2) {
				//square 7
				box = 7;
			} else {
				//Square 4(between -2 and 2)
				box = 4;
			}
		}
		return box;
	}

	int[] ComputeCenter(int box)
	{
			int[] center = new int[2];
			int x_pos = 0;
			int y_pos = 0;

			// should use switch
			switch (box) {
			case 0:
				x_pos =  -4;
				y_pos = 4;
				break;
			case 1:
				x_pos =  0;
				y_pos = 4;
				break;
			case 2:
				x_pos =  4;
				y_pos = 4;
				break;
			case 3:
				x_pos =  -4;
				y_pos = 0;
				break;
			case 4:
				x_pos =  0;
				y_pos = 0;
				break;
			case 5:
				x_pos =  4;
				y_pos = 0;
				break;
			case 6:
				x_pos =  -4;
				y_pos = -4;
				break;
			case 7:
				x_pos =  0;
				y_pos = -4;
				break;
			case 8:
				x_pos =  4;
				y_pos = -4;
				break;
				}
		center [0] = x_pos;
		center [1] = y_pos;
		lastBox = box;
		return center;
	}

}
