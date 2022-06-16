using System;

namespace BrickBreaker.Backend.Base
{
    class Player
    {
        string name;
        int score;

        public Player(int score, string name)
        {
            this.name = name;
            this.score = score;
        }
        public Player Compare(Player other)
        {
            if (score > other.GetScore())
                return this;
            return other;
        }
        public string GetName() { return name; }
        public int GetScore() { return score; }
    }
}