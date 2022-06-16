using Android.Content;
using Android.Graphics;
using Android.OS;
using BrickBreaker.Activities;
using BrickBreaker.Backend.Base;
using System;
using System.Numerics;

namespace BrickBreaker.Backend
{
    class Ball : MovingSprite
    {
        public float Radius;
        public bool paused;
        public int pauseFrames = 0;
        public int finalFrames;
        Vector2 startPoint;
        bool vibrate;
        public Ball(SpriteColor color, Context context, float mult, bool vib) :
            base(context)
        {
            vibrate = vib;
            int width = context.Resources.DisplayMetrics.WidthPixels;
            int height = context.Resources.DisplayMetrics.HeightPixels;
            srcImg = GetSrcImage(color);
            float x = width / 2;
            float y = height - Res.GetDimension(Resource.Dimension.ballY)
                - (srcImg.Height() * mult / 1.5f);
            int sx = Res.GetInteger(Resource.Integer.ball_start_speed_x);
            int sy = Res.GetInteger(Resource.Integer.ball_start_speed_y);
            Speed = new Vector2(sx, sy);
            startPoint = new Vector2(x, y);
            Loc = new Vector2(x, y);
            Rec = GetRect(mult);
            paused = false;
        }
        //sets the ball to freeze in place
        //for the given frames
        public void Wait(int frames = 100)
        {
            paused = true;
            pauseFrames = 0;
            finalFrames = frames;
        }
        //returns the rect where it'll be drawn on screen
        protected override RectF GetRect(float mult)
        {
            Radius = (srcImg.Width() * mult) / 1.5f;
            return new RectF(Loc.X - Radius, Loc.Y - Radius, Loc.X + Radius,
                Loc.Y + Radius);
        }
        //returns the rect of the sprite image
        //from the sprite sheet
        protected override Rect GetSrcImage(SpriteColor color)
        {
            int y = Res.GetInteger(Resource.Integer.ball_sprite_y);
            int side = Res.GetInteger(Resource.Integer.ball_side);
            int x = color switch
            {
                SpriteColor.Blue => Res.GetInteger(Resource.Integer.blue_ball_x),
                SpriteColor.Green => Res.GetInteger(Resource.Integer.green_ball_x),
                SpriteColor.Red => Res.GetInteger(Resource.Integer.red_ball_x),
                SpriteColor.Purple => Res.GetInteger(Resource.Integer.purple_ball_x),
                SpriteColor.Gold => Res.GetInteger(Resource.Integer.gold_ball_x),
                SpriteColor.Grey => Res.GetInteger(Resource.Integer.green_ball_x),
                SpriteColor.Bronze => Res.GetInteger(Resource.Integer.bronze_ball_x),
                _ => Res.GetInteger(Resource.Integer.blue_ball_x),
            };
            return new Rect(x, y, x + side, y + side);
        }
        //draws the ball on the canvas
        //if paused, increases the count of frames
        public override void Draw(Canvas canvas)
        {
            if (paused && pauseFrames < finalFrames)
                pauseFrames++;
            else if (paused && pauseFrames >= finalFrames)
                paused = false;
            canvas.DrawBitmap(DrawSurfaceView.spriteSheet, srcImg, Rec, paint);
        }
        //determines the angle of the ball
        //according to it's hit type
        public void Hit(HitPoint hit)
        {
            switch (hit.hit)
            {
                case HitType.Left:
                    Speed.X = -Math.Abs(Speed.X);
                    break;
                case HitType.Right:
                    Speed.X = Math.Abs(Speed.X);
                    break;
                case HitType.Top:
                    Speed.Y = -Math.Abs(Speed.Y);
                    break;
                case HitType.Bottom:
                    Speed.Y = Math.Abs(Speed.Y);
                    break;
                case HitType.Bar:
                    Speed = hit.vec;
                    break;
            }
        }
        //makes the ball teleport to it's start location
        public void Teleport()
        {
            int sx = Res.GetInteger(Resource.Integer.ball_start_speed_x);
            int sy = Res.GetInteger(Resource.Integer.ball_start_speed_y);
            Loc = startPoint;
            Speed = new Vector2(sx, sy);
            Rec.Set(Loc.X - Radius, Loc.Y - Radius, Loc.X + Radius, Loc.Y + Radius);
        }

        //updates the location of the ball
        //uses delta time to make the movement smoother
        public override void Move(int screenHeight, int screenWidth, double dt, int x)
        {
            if (!paused)
            {
                Loc += Speed * (float)dt;
                float top = Res.GetDimension(Resource.Dimension.topSection);
                if (Loc.X < Radius) //right
                    Hit(new HitPoint(HitType.Right));
                if (Loc.X > screenWidth - Radius)//left
                    Hit(new HitPoint(HitType.Left));
                if (Loc.Y < top + Radius)//top
                    Hit(new HitPoint(HitType.Bottom));
                if (Loc.Y > screenHeight - Radius)//bottom
                {
                    Hit(new HitPoint(HitType.Top));
                    DrawSurfaceView.lives--;
                    Wait();
                    Teleport();
                    if (vibrate)
                    {
                        Vibrator vibrator = 
                            (Vibrator)Con.GetSystemService("vibrator");
                        vibrator.Vibrate(100);
                    }
                }
                Rec.Set(Loc.X - Radius, Loc.Y - Radius,
                    Loc.X + Radius, Loc.Y + Radius);
            }
        }

        public override void ChangeColor(SpriteColor newColor)
        {
            color = newColor;
            srcImg = GetSrcImage(color);
        }
    }
}