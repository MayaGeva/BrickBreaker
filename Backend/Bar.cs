using Android.Content;
using Android.Graphics;
using BrickBreaker.Backend.Base;
using System.Numerics;

namespace BrickBreaker.Backend
{
    class Bar : MovingSprite
    {
        public Bar(SpriteColor color, Context context, 
            int height, int width, float mult)
            : base(context)
        {
            srcImg = GetSrcImage(color);
            float x = (width / 2) - (Res.GetInteger(Resource.Integer.bar_width)
                * mult / 2);
            float y = height - Res.GetDimension(Resource.Dimension.bottomSection)
                - (srcImg.Height() * mult / 2);
            Loc = new Vector2(x, y);

            Rec = GetRect(mult);
        }
        //calculates and returns the rect on screen where the bar will be drawn
        protected override RectF GetRect(float mult)
        {
            float height = Res.GetInteger(Resource.Integer.bar_height) * mult;
            float width = Res.GetInteger(Resource.Integer.bar_width) * mult;
            return new RectF(Loc.X, Loc.Y, Loc.X + width, Loc.Y + height);
        }
        //returns the rect out of the spritesheet where 
        //the sprite of the bar is according to it's selected color
        protected override Rect GetSrcImage(SpriteColor color)
        {
            int x = Res.GetInteger(Resource.Integer.bar_sprite_x);
            int width = Res.GetInteger(Resource.Integer.bar_width);
            int height = Res.GetInteger(Resource.Integer.bar_height);
            var y = color switch
            {
                SpriteColor.Blue => Res.GetInteger(Resource.Integer.blue_bar_y),
                SpriteColor.Green => Res.GetInteger(Resource.Integer.green_bar_y),
                SpriteColor.Red => Res.GetInteger(Resource.Integer.red_bar_y),
                SpriteColor.Purple => Res.GetInteger(Resource.Integer.purple_bar_y),
                SpriteColor.Gold => Res.GetInteger(Resource.Integer.gold_bar_y),
                _ => Res.GetInteger(Resource.Integer.blue_bar_y),
            };
            return new Rect(x, y, x + width, y + height);
        }
        //draws the bar
        public override void Draw(Canvas canvas)
        {
            canvas.DrawBitmap(DrawSurfaceView.spriteSheet, srcImg, Rec, paint);
        }
        //updates the x value of the bar
        public override void Move(int screenWidth, int screenHeight, double dt, int x)
        {
            int width = (int)Rec.Width();
            if (x + width / 2 >= screenWidth)
                Rec.Set(screenWidth - width, Rec.Top, screenWidth, Rec.Bottom);
            else if (x <= width / 2)
                Rec.Set(0, Rec.Top, width, Rec.Bottom);
            else
                Rec.Set(x - width / 2, Rec.Top, x + width / 2, Rec.Bottom);
        }
        //checks if the bar touched the ball
        //and returns the type of hit and the vector of the hit
        //also updates the angle of the ball by subtracting the vectors
        public HitPoint TouchBall(float r, Vector2 c, Vector2 ballSpeed)
        {
            //calculates the distance the ball goes 
            //into the brick from different directions
            float dy1 = r - (c.Y - Rec.Bottom);
            float dy0 = r - (Rec.Top - c.Y);
            float dx0 = r - (Rec.Left - c.X);
            float dx1 = r - (c.X - Rec.Right);
            if (dy1 > 0 && dy0 > 0 && dx1 > 0 && dx0 > 0)
            {
                float lengthB = ballSpeed.Length();
                Vector2 d = c - new Vector2(Rec.CenterX(), Rec.CenterY());
                float lengthV0 = d.Length();
                Vector2 v = lengthB * d / lengthV0;
                return new HitPoint(HitType.Bar, v);
            }
            return new HitPoint(HitType.None);
        }
        public override void ChangeColor(SpriteColor newColor)
        {
            color = newColor;
            srcImg = GetSrcImage(color);
        }
    }
}