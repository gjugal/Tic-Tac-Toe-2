using System.Collections;
using UnityEngine;

public abstract class AIComp {

	protected int ROWS = 3;  // number of rows
	protected int COLS = 3;
	public enum Chance{ COMP, USER};
	protected Chance current_chance;
	protected int[] my_grid;
		
	public AIComp(int[] grid)
	{
		my_grid = grid;
		Debug.Log (my_grid [0] + " " + my_grid [1] + " " + my_grid [2] + "\n" + 
		           my_grid [3] + " " + my_grid [4] + " " + my_grid [5] + "\n" + 
		           my_grid [6] + " " + my_grid [7] + " " + my_grid [8]);
	}

	public void setChance(Chance chance) {
		current_chance = chance;
	}

	public abstract int move();


}
