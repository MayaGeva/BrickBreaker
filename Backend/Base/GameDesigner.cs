using System;

namespace BrickBreaker.Backend.Base
{
    enum LevelDifficulty
    {
        Easy,
        Normal,
        Difficult,
        Hard,
        VeryHard
    }
    class GameDesigner
    {
        int[,] level;
        public GameDesigner(int rows, int columns)
        {
            level = new int[rows, columns];
            
            GenerateLevel();
        }
        public int[,] GetLevel()
        {
            return level;
        }
        //generates a quarter of the level and duplicates 
        //it to the rest of the quarters
        public void GenerateLevel()
        {
            int m = level.GetLength(1);
            int n = level.GetLength(0);
            int mHalf = (m + 1) / 2;
            int nHalf = (n + 1) / 2;
            Random r = new Random();
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    if (i < nHalf && j < mHalf)
                    {
                        level[i, j] = r.Next(0, 5);
                    }
                    else
                    {
                        if (i < nHalf)
                            level[i, j] = level[i, m - j - 1];
                        else
                            level[i, j] = level[n - i - 1, j];
                    }
                }
            }
        }
    }
}