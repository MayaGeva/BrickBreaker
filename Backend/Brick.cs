using Android.Content;
using Android.Graphics;
using BrickBreaker.Activities;
using BrickBreaker.Backend.Base;
using System;
using System.Numerics;

namespace BrickBreaker.Backend
{
    class Brick : Sprite
    {
        protected int points;
        protected int destroyPoints;
        protected int lives;
        public bool draw;
        public Brick(SpriteColor color, PointF loc, float width, float height,
            Context context) : base(context)
        {
            this.color = color;
            lives = 3;
            draw = true;
            Loc = new Vector2(loc.X, loc.Y);
            srcImg = GetSrcImage(color);
            points = Res.GetInteger(Resource.Integer.brick_points);
            destroyPoints = Res.GetInteger(Resource.Integer.brick_break_points);
            Rec = new RectF(Loc.X, Loc.Y, Loc.X + width, Loc.Y + height);
        }
        //either changes the color or kills the brick
        public void Destroy()
        {
            lives--;
            if (lives == 0)
            {
                DrawSurfaceView.points += destroyPoints;
                draw = false;
            }
            else
            {
                DrawSurfaceView.points += points;
                color = NextColor();
                srcImg = GetSrcImage(color);
            }
        }
        //returns the next color of the brick
        SpriteColor NextColor()
        {
            return lives switch
            {
                2 => SpriteColor.Bronze,
                1 => SpriteColor.Grey,
                _ => SpriteColor.Red,
            };
        }
        //draws the brick
        public override void Draw(Canvas canvas)
        {
            canvas.DrawBitmap(DrawSurfaceView.spriteSheet, srcImg, Rec, paint);
        }
        protected override RectF GetRect(float mult) { return null; }
        public override void ChangeColor(SpriteColor newColor)
        {
            if (lives == 3)
            {
                color = newColor;
                srcImg = GetSrcImage(color);
            }
        }
        //gets the rect out of the sprite sheet of the brick by it's color
        protected override Rect GetSrcImage(SpriteColor color)
        {
            int x = Res.GetInteger(Resource.Integer.brick_sprite_x);
            int width = Res.GetInteger(Resource.Integer.brick_width);
            int height = Res.GetInteger(Resource.Integer.brick_height);
            int y = color switch
            {
                SpriteColor.Blue => Res.GetInteger(Resource.Integer.blue_brick_y),
                SpriteColor.Green => Res.GetInteger(Resource.Integer.green_brick_y),
                SpriteColor.Red => Res.GetInteger(Resource.Integer.red_brick_y),
                SpriteColor.Purple => Res.GetInteger(Resource.Integer.purple_brick_y),
                SpriteColor.Gold => Res.GetInteger(Resource.Integer.gold_brick_y),
                SpriteColor.Grey => Res.GetInteger(Resource.Integer.grey_brick_y),
                SpriteColor.Bronze => Res.GetInteger(Resource.Integer.bronze_brick_y),
                _ => Res.GetInteger(Resource.Integer.blue_brick_y),
            };
            return new Rect(x, y, x + width, y + height);
        }
        //calculates the distance the ball goes 
        //into the brick from different directions
        //and returns the type of hit along with the vector of the hit
        public virtual HitPoint TouchBall(float r, Vector2 c, Vector2 ballSpeed)
        {
            //calculates the distance the ball goes 
            //into the brick from different directions

            float dy1 = r - (c.Y - Rec.Bottom);
            float dy0 = r - (Rec.Top - c.Y);
            float dx0 = r - (Rec.Left - c.X);
            float dx1 = r - (c.X - Rec.Right);
            if (dy1 > 0 && dy0 > 0 && dx1 > 0 && dx0 > 0)
            {
                DrawSurfaceView.points += points;
                float lengthB = ballSpeed.Length();
                Vector2 d = c - new Vector2(Rec.CenterX(), Rec.CenterY());
                float lengthV0 = d.Length();
                Vector2 v = lengthB * d / lengthV0;
                return new HitPoint(HitType.Bar, v);
            }
            return new HitPoint(HitType.None);
        }
    }
}