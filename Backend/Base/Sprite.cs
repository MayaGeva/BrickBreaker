using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using System.Numerics;

namespace BrickBreaker.Backend.Base
{
    abstract class Sprite
    {
        public Vector2 Loc { get; set; }
        protected Paint paint;
        protected Rect srcImg;
        protected RectF Rec;
        protected Resources Res;
        protected Context Con;
        protected SpriteColor color;
        public Sprite(Context context)
        {
            Con = context;
            Res = context.Resources;
            paint = new Paint
            {
                AntiAlias = false,
                FilterBitmap = false
            };
        }
        public abstract void Draw(Canvas canvas);
        protected abstract Rect GetSrcImage(SpriteColor color);
        protected abstract RectF GetRect(float mult);
        public abstract void ChangeColor(SpriteColor newColor);
    }
}