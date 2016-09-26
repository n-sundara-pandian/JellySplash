using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using thelab.mvc;
using UnityEngine.SceneManagement;

public class GameModel : Model<Game> {
	public class LevelData
	{
		public int []tiles;
		public int targetScore;
		public int moves;
	}
    private int score = 0;
    private int highscore = 0;
    private int longstreak = 0;
    private int movesRemaining = 20;
    private int totalMoves = 20;
    private int multiplier = 50;
	private LevelData levels = new LevelData();
    private List<int> levelData = new List<int>();
    public List<int> GetBoard() { return levelData; }
	public List<int> floodFillItemList = new List<int>();
    public int GetRemainingMoves()
    {
        return movesRemaining;
    }
	public int GetScoreForBlockCount(int index)
	{
		int result = 0;
		for (int i = 1; i <= index; i++)
		{
			result += i * multiplier;
		}
		return result;
	}
    public void AddScore(int index)
    {
		score += index * multiplier;
        if (score > highscore)
        {
            highscore = score;
            Notify("model.highscore", highscore);
        }
    }
    public int GetScore()
    {
        return score;
    }
    public int GetTotalMoves()
    {
        return totalMoves;
    }
    public void DecMoves()
    {
        movesRemaining--;
    }
    public void SetStreak(int streak)
    {
        if (streak > longstreak)
        {
            longstreak = streak;
            Notify("model.streak", longstreak);
        }
    }
    public void Init(int high, int streak)
    {
        movesRemaining = totalMoves;
        score = 0;
        highscore = high;
        longstreak = streak;
		string current_level = "Levels/Level_" + Utils.current_level;
		TextAsset lvl = Resources.Load<TextAsset>(current_level);
		levels = JsonUtility.FromJson<LevelData> (lvl.text);
    }
    public void Shrink()
    {
        for (int i = Utils.width - 1; i >= 0; --i)
        {
            int ktop = Utils.height - 1;
            for (int j = ktop; j >= 0; --j)
            {
                if (levelData[Utils.GetID(j, i)] == 0)
                {
                    for (int k = j - 1; k >= 0; --k)
                    {
                        if (levelData[Utils.GetID(k, i)] > 0)
                        {
                            levelData[Utils.GetID(j, i)] = levelData[Utils.GetID(k, i)];
                            levelData[Utils.GetID(k, i)] = 0;
                            ktop = j;
                            break;
                        }
                    }
                }
                else
                {
                    ktop = j;
                }
            }
        }
    }
    public void SetValue(int id, int val)
    {
        levelData[id] = val;
    }
	void ChainCount(int row, int col, long old, bool breakOnValid, ref int count)
    {
        if ((row < 0) || (row >= Utils.height)) return;
        if ((col < 0) || (col >= Utils.width)) return;
		int id = Utils.GetID(row, col);
		if (levelData [id] != old)
			return;
		if (breakOnValid && count > 2) return;
        if (levelData[id] == old)
        {
			bool found = false;
			foreach (int item in floodFillItemList) {
				if (item == id) {
					found = true;
					break;
				}
			}
			if (!found) {
				floodFillItemList.Add (id);
				count++;
			} else
				return;
			ChainCount(row + 1, col, old, breakOnValid, ref count);
			ChainCount(row - 1, col, old, breakOnValid, ref count);
			ChainCount(row, col + 1, old, breakOnValid, ref count);
			ChainCount(row, col - 1, old, breakOnValid, ref count);
			ChainCount(row + 1, col + 1, old, breakOnValid, ref count);
			ChainCount(row + 1, col - 1, old, breakOnValid, ref count);
			ChainCount(row - 1, col + 1, old, breakOnValid, ref count);
			ChainCount(row - 1, col - 1, old, breakOnValid, ref count);
        }
    }

    public void PopulateBoard()
    {
		AudioManager.Main.PlayNewSound("falldown");
        for (int row = 0; row < Utils.height; row++)
        {
            for (int col = 0; col < Utils.width; col++)
            {
				if(levels.tiles[Utils.GetID(row, col)] == 1)
                	levelData.Add(Utils.GetValidGem());
				else
					levelData.Add(-1);
            }
        }
    }

    public bool ValidateBoard()
    {
        for (int i = 0; i < levelData.Count; i++)
        {
            int row = 0, col = 0, count = 0;
            Utils.GetRowCol(i, out row, out col);
			ChainCount(row, col, levelData[i], true, ref count);
            if (count > 2)
            {
                return true;
            }
        }
        return false;
    }
    public void FillItems()
    {
        for (int i = 0; i < levelData.Count; i++)
        {
            if (levelData[i] == 0)
            {
                levelData[i] = Utils.GetValidGem();
            }
        }

    }
	public void FloodFill(int tile)
	{
		floodFillItemList.Clear ();
		int row = 0, col = 0, count = 0;
		Utils.GetRowCol(tile, out row, out col);
		ChainCount(row, col, levelData[tile], false, ref count);
	}

}
