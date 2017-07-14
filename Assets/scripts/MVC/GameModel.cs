using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using thelab.mvc;
using UnityEngine.SceneManagement;

public class GameModel : Model<Game> {
	public class LevelData
	{
		public int rows;
		public int cols;
		public int []tiles;
		public int targetScore;
		public int moves;
        public int timer;
	}
    private int score = 0;
    private int highscore = 0;
    private int longstreak = 0;
    private int multiplier = 50;
	public LevelData levels = new LevelData();
    private List<int> levelData = new List<int>();
    public List<int> GetBoard() { return levelData; }
	public List<int> FillItemList = new List<int>();
    List<int> FillTileList = new List<int>();
    public GamePlayController gamePlayController;

    public int GetRemainingMoves()
    {
		return gamePlayController.GetRemainingSteps();
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
		return gamePlayController.GetTotalMoves();
    }
    public void Update()
    {
       // if (gamePlayController != null)
        //    gamePlayController.tick();
    }
    public void DecMoves()
    {
        //movesRemaining--;
		if (gamePlayController != null)
			gamePlayController.tick();
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
       // movesRemaining = totalMoves;
        score = 0;
        highscore = high;
        longstreak = streak;
        int cur_level = Random.Range(0, 4);
		string current_level = "Levels/Level_" + cur_level;
		TextAsset lvl = Resources.Load<TextAsset>(current_level);
		levels = JsonUtility.FromJson<LevelData> (lvl.text);
		Utils.width = levels.cols;
		Utils.height = levels.rows;
        for (int i = 0; i < levels.cols * levels.rows; i++)
            levelData.Add(-1);
        if (levels.timer != 0) InitGameplayController("timer");
        else if (levels.moves != 0) InitGameplayController("moves");
    }
    void InitGameplayController(string type)
    {
        if (type == "timer")
        {
            GameObject temp = GameObject.Instantiate(Resources.Load("TimerLogic")) as GameObject;
            gamePlayController = temp.GetComponent<GamePlayController>();
            Dictionary<string, int> param = new Dictionary<string, int>();
            param.Add("timer", levels.timer);
            gamePlayController.init(param);
        }
        else if (type == "moves")
        {
            GameObject temp = GameObject.Instantiate(Resources.Load("MovesLogic")) as GameObject;
            gamePlayController = temp.GetComponent<GamePlayController>();
            Dictionary<string, int> param = new Dictionary<string, int>();
            param.Add("moves", levels.moves);
            gamePlayController.init(param);
        }
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
    void LineCount(int row, int col, int dir, ref int count)
    {
        if ((row < 0) || (row >= Utils.height)) return;
        if ((col < 0) || (col >= Utils.width)) return;
        int id = Utils.GetID(row, col);
        bool found = false;
        foreach (int item in FillTileList)
        {
            if (item == id)
                return;
        }
        foreach (int item in FillItemList)
        {
            if (item == id)
            {
                found = true;
                break;
            }
        }
        if (!found)
        {
            if (levelData[id] >= 0)
            {
                FillItemList.Add(id);
                count++;
            }
            else
            {
                FillTileList.Add(id);
            }
        }
        else
            return;
        int dr = 0;
        int dc = 0;
        if (dir == 0) { dr = 1; }
        if (dir == 1) { dc = 1; }
        LineCount(row + dr, col + dc,dir,ref count);
        LineCount(row - dr, col - dc,dir,ref count);

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
			foreach (int item in FillItemList) {
				if (item == id) {
					found = true;
					break;
				}
			}
			if (!found) {
				FillItemList.Add (id);
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
                if (levels.tiles[Utils.GetID(row, col)] == 1)
                {
                    levelData[Utils.GetID(row, col)] = Utils.GetValidGem();
                }
                else
                {
                    levelData[Utils.GetID(row, col)] = -1;
                }
            }
        }
    }

    public bool ValidateBoard()
    {
        for (int i = 0; i < levelData.Count; i++)
        {
            FillItemList.Clear();
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
	public void FloodFill1(int tile)
	{
		FillItemList.Clear ();
		int row = 0, col = 0, count = 0;
		Utils.GetRowCol(tile, out row, out col);
		ChainCount(row, col, levelData[tile], false, ref count);
	}
    public void FloodFill(int tile)
    {
        FillItemList.Clear();
        FillTileList.Clear();
        int row = 0, col = 0, count = 0;
        Utils.GetRowCol(tile, out row, out col);
        // LineCount(row, col, 1, ref count);
        Sweep(tile, 2);
//        ChainCount(row, col, levelData[tile], false, ref count);
    }
    void checkAndAdd(int row, int col)
    {
        int id = Utils.GetID(row, col);
        if (!Utils.IsValidID(id)) return;
        bool found = false;
        foreach (int item in FillTileList)
        {
            if (item == id)
                return;
        }
        foreach (int item in FillItemList)
        {
            if (item == id)
            {
                found = true;
                break;
            }
        }
        if (!found)
        {
            if (levelData[id] >= 0)
            {
                FillItemList.Add(id);
            }
            else
            {
                FillTileList.Add(id);
            }
        }
        else
            return;

    }

    void Sweep(int tile, int dir)
    {
        int row = 0, col = 0;
        Utils.GetRowCol(tile, out row, out col);
        if (dir == 0)
        {
            for(int i = 0; i < Utils.height; i++)
            {
                int r = 0, c = 0;
                int id = Utils.GetID(row, col + i);
                Utils.GetRowCol(id, out r, out c);
                if (r == row) checkAndAdd(row, col + i);
                id = Utils.GetID(row, col - i);
                Utils.GetRowCol(id, out r, out c);
                if (r == row) checkAndAdd(row, col - i);
            }
        }
        else if (dir == 1)
        {
            for (int i = 0; i < Utils.width; i++)
            {
                int r = 0, c = 0;
                int id = Utils.GetID(row + i, col);
                Utils.GetRowCol(id, out r, out c);
                if (c == col) checkAndAdd(row + i, col);
                id = Utils.GetID(row - i, col);
                Utils.GetRowCol(id, out r, out c);
                if (c == col) checkAndAdd(row - i, col);
            }

        }
        else if (dir == 2)
        {
            for (int i = 0; i < Utils.height; i++)
            {
                int r = 0, c = 0;
                int id = Utils.GetID(row, col + i);
                Utils.GetRowCol(id, out r, out c);
                if (r == row) checkAndAdd(row, col + i);
                id = Utils.GetID(row + i, col);
                Utils.GetRowCol(id, out r, out c);
                if (c == col) checkAndAdd(row + i, col);
                id = Utils.GetID(row, col - i);
                Utils.GetRowCol(id, out r, out c);
                if (r == row) checkAndAdd(row, col - i);
                id = Utils.GetID(row - i, col);
                Utils.GetRowCol(id, out r, out c);
                if (c == col) checkAndAdd(row - i, col);
            }

        }
    }
    public bool IsTargetAchieved()
    {
        return score >= levels.targetScore;
    }

}
