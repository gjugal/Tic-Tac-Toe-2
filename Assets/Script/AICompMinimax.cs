using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICompMinimax : AIComp
{

    
    public AICompMinimax(int[] grid) : base(grid)
    {

    }

    override
     public int move()
    {
        int depth = (base.ROWS * base.COLS) - GameLogic.steps;
        int[] result = minimax(depth, current_chance, int.MinValue, int.MaxValue); // depth, max turn
        Debug.Log(result[1] + " " + result[0]);
        return result[1];   //best box
    }



    private int[] minimax(int depth, Chance chance, int alpha, int beta)
    {

        // Generate possible next moves in a List of int[2] of {row, col}.
        List<int> nextMoves = generateMoves();

        // compChance is maximizing; while userChance is minimizing
        //int score = (chance == Chance.COMP) ? int.MinValue : int.MaxValue;
        int score;
        int bestBox = -1;

        if (nextMoves.Count == 0 || depth == 0)
        {
            // Gameover or depth reached, evaluate score
            score = evaluate();
            return new int[] { score, bestBox };
        }
        else
        {
            foreach (int move in nextMoves)
            {
                // Try this move for the current "player"
                if (chance == Chance.COMP)
                    my_grid[move] = 2;
                else
                    my_grid[move] = 1;

                if (chance == Chance.COMP)
                {  // myChance (computer) is maximizing player
                    score = minimax(depth - 1, Chance.USER, alpha, beta)[0];
                    if (score > alpha)
                    {
                        alpha = score;
                        bestBox = move;
                    }
                }
                else
                {  // oppSeed is minimizing player
                    score = minimax(depth - 1, Chance.COMP, alpha, beta)[0];
                    if (score < beta)
                    {
                        beta = score;
                        bestBox = move;
                    }
                }
                //UNDO MOVE
                my_grid[move] = 0;
                if (alpha >= beta) break;

            }

        }
        return new int[] { (chance == Chance.COMP) ? alpha : beta, bestBox };
    }

    private List<int> generateMoves()
    {
        List<int> nextMoves = new List<int>();

        // If gameover, i.e., no next move
        //This condition may be added later

        if (hasWon(Chance.COMP) || hasWon(Chance.USER))
        {
            return nextMoves;   // return empty list
        }

        for (int i = 0; i < 9; i++)
        {
            if (my_grid[i] == 0)
            {
                nextMoves.Add(i);
            }
        }

        return nextMoves;
    }

    /** The heuristic evaluation function for the current board
       @Return +100, +10, +1 for EACH 3-, 2-, 1-in-a-line for computer.
               -100, -10, -1 for EACH 3-, 2-, 1-in-a-line for opponent.
               0 otherwise   */

    private int evaluate()
    {
        int my_score = 0;
        // Evaluate score for each of the 8 lines (3 rows, 3 columns, 2 diagonals)
        my_score += evaluateLine(0, 1, 2);  // row 0
        my_score += evaluateLine(3, 4, 5);  // row 1
        my_score += evaluateLine(6, 7, 8);  // row 2
        my_score += evaluateLine(0, 3, 6);  // col 0
        my_score += evaluateLine(1, 4, 7);  // col 1
        my_score += evaluateLine(2, 5, 8);  // col 2
        my_score += evaluateLine(0, 4, 8);  // diagonal
        my_score += evaluateLine(2, 4, 6);  // alternate diagonal
                                            //	Debug.Log(my_score);
        return my_score;
    }



    private int evaluateLine(int cell1, int cell2, int cell3)
    {
        int score = 0;

        // First cell
        if (my_grid[cell1] == 2)
        {
            score = 1;
        }
        else if (my_grid[cell1] == 1)
        {
            score = -1;
        }

        // Second cell
        if (my_grid[cell2] == 2)
        {
            if (score == 1)
            {   // cell1 is mySeed
                score = 10;
            }
            else if (score == -1)
            {  // cell1 is oppSeed
                return 0;
            }
            else
            {  // cell1 is empty
                score = 1;
            }
        }
        else if (my_grid[cell2] == 1)
        {
            if (score == -1)
            { // cell1 is oppSeed
                score = -10;
            }
            else if (score == 1)
            { // cell1 is mySeed
                return 0;
            }
            else
            {  // cell1 is empty
                score = -1;
            }
        }

        // Third cell
        if (my_grid[cell3] == 2)
        {
            if (score > 0)
            {  // cell1 and/or cell2 is mySeed
                score *= 10;
            }
            else if (score < 0)
            {  // cell1 and/or cell2 is oppSeed
                return 0;
            }
            else
            {  // cell1 and cell2 are empty
                score = 1;
            }
        }
        else if (my_grid[cell3] == 1)
        {
            if (score < 0)
            {  // cell1 and/or cell2 is oppSeed
                score *= 10;
            }
            else if (score > 1)
            {  // cell1 and/or cell2 is mySeed
                return 0;
            }
            else
            {  // cell1 and cell2 are empty
                score = -1;
            }
        }
        return score;
    }

    private bool hasWon(Chance chance)
    {
        int num = 1;
        if(chance == Chance.COMP)
        {
            num = 2;
        }
        if ((my_grid[0] == num && my_grid[1] == num && my_grid[2] == num) || (my_grid[3] == num && my_grid[4] == num && my_grid[5] == num)
            || (my_grid[6] == num && my_grid[7] == num && my_grid[8] == num) || (my_grid[0] == num && my_grid[3] == num && my_grid[6] == num)
            || (my_grid[1] == num && my_grid[4] == num && my_grid[7] == num) || (my_grid[2] == num && my_grid[5] == num && my_grid[8] == num)
            || (my_grid[0] == num && my_grid[4] == num && my_grid[8] == num) || (my_grid[2] == num && my_grid[4] == num && my_grid[6] == num))
            return true;
        else
            return false;
    }
}
