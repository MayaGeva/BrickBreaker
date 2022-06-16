using System;
using System.Numerics;

namespace BrickBreaker.Backend.Base
{
    enum HitType
    {
        Left,
        Right,
        Top,
        Bottom,
        Bar,
        None
    }
    //class that represents the type of hit 
    //that occured and the vector of the hit
    class HitPoint
    {
        public HitType hit;
        public Vector2 vec;
        public HitPoint(HitType hit, Vector2 v = default)
        {
            vec = v;
            this.hit = hit;
        }
    }
}