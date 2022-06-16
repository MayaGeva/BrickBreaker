using Android.Content;
using Android.Graphics;
using System.Numerics;

namespace BrickBreaker.Backend.Base
{
    abstract class MovingSprite : Sprite
    {
        public Vector2 Speed;
        public MovingSprite(Context context) : base(context)
        { }
        public override abstract void Draw(Canvas canvas);
        public abstract void Move(int sHeight, int sWidth, double dt, int x);
        protected override abstract Rect GetSrcImage(SpriteColor color);
        protected override abstract RectF GetRect(float mult);
        public override abstract void ChangeColor(SpriteColor newColor);
    }
}