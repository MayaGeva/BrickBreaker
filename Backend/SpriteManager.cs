using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.Preferences;
using BrickBreaker.Backend.Base;
using System;

namespace BrickBreaker.Backend
{
    class SpriteManager
    {
        //frame handlers
        int frameCount = 0;
        long lastTicks;

        //sprites
        public Ball ball;
        Bar bar;
        Brick[,] bricks;
        float spriteScale;
        GameDesigner designer;

        //general
        Context context;
        int screenWidth;
        int screenHeight;
        ISharedPreferences sp;

        //sound
        SoundPool mp;
        int beep;
        float beepVol;

        public SpriteManager(int screenWidth, int screenHeight, int rows, int columns,
            Context context, SoundPool mp, int beep)
        {
            this.beep = beep;
            this.mp = mp;
            sp = PreferenceManager.GetDefaultSharedPreferences(Application.Context);
            this.context = context;
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            lastTicks = 0;
            bool vib = sp.GetBoolean(context.Resources.GetString(
                Resource.String.is_vibrate), true);
            bricks = new Brick[rows, columns];
            spriteScale = CalcDifference();
            SpriteColor ballColor = (SpriteColor)sp.GetInt(context.Resources.GetString(
                Resource.String.ball_color), 6);
            SpriteColor barColor = (SpriteColor)sp.GetInt(context.Resources.GetString(
                Resource.String.bar_color), 0);
            bar = new Bar(barColor, context, screenHeight, screenWidth, spriteScale);
            ball = new Ball(ballColor, context, spriteScale, vib);
            AddBricks(screenWidth, screenHeight);
        }
        //updates the sprites positions and checks for collision on every frame
        public void Update(Canvas canvas)
        {
            DrawSprites(canvas);
            CheckCollisions();
        }
        //updates the SoundPool volume from the SharedPreference
        public void UpdateVolume()
        {
            int maxVolume = 5;
            bool mute = sp.GetBoolean(context.Resources.GetString(
                Resource.String.is_volume), false);
            int currVolume = sp.GetInt(context.Resources.GetString(
                Resource.String.sfx_volume), 5);
            if (maxVolume != currVolume && !mute)
                beepVol = (float)(1 - (Math.Log(maxVolume - currVolume)
                    / Math.Log(maxVolume)));
            else if (mute)
                beepVol = 0;
            else
                beepVol = 1;
        }
        //calculates the location of the bricks on the screen by it's size
        //and initiates all of the brick objects
        void AddBricks(int screenWidth, int screenHeight)
        {
            float space = context.Resources.GetDimension(Resource.Dimension.between);
            float top = context.Resources.GetDimension(Resource.Dimension.topSection);
            float sides = context.Resources.GetDimension(Resource.Dimension.sides);
            float bottom = context.Resources.GetDimension(
                Resource.Dimension.bottomSection);
            designer = new GameDesigner(bricks.GetLength(0),
                bricks.GetLength(1));
            int[,] colors = designer.GetLevel();
            float width = (screenWidth - sides * 2 - (bricks.GetLength(1) - 1) * space) /
                bricks.GetLength(1);
            float height = width / 2;
            top += (screenHeight - top - bottom) / 17;
            PointF p;
            float x, y;
            for (int i = 0; i < bricks.GetLength(0); i++)
            {
                for (int j = 0; j < bricks.GetLength(1); j++)
                {
                    Brick brick;
                    int color = colors[i, j];
                    x = sides + j * (width + space);
                    y = top + i * (height + space);
                    p = new PointF(x, y);
                    brick = new Brick((SpriteColor)color, p, width, height, context);
                    bricks[i, j] = brick;
                }
            }
        }
        //checks collision of the ball with either the bar or one of the bricks
        void CheckCollisions()
        {
            HitPoint hit;
            for (int i = 0; i < bricks.GetLength(0); i++)
            {
                for (int j = 0; j < bricks.GetLength(1); j++)
                {
                    if (bricks[i, j] != null)
                    {
                        hit = bricks[i, j].TouchBall(ball.Radius, ball.Loc, ball.Speed);
                        if (hit.hit != HitType.None)
                        {
                            ball.Hit(hit);
                            bricks[i, j].Destroy();
                            if (!bricks[i, j].draw)
                                bricks[i, j] = null;
                            mp.Play(beep, beepVol, beepVol, 1, 0, 1);
                        }
                    }
                }
            }
            hit = bar.TouchBall(ball.Radius, ball.Loc, ball.Speed);
            if (hit.hit != HitType.None)
            {
                ball.Hit(hit);
            }
        }
        //calculates the number of ticks between the current frame 
        //and the previous one to determine the delta time 
        //that will be multiplied with the ball speed 
        //in order to keep it moving smoothly regardless of fps
        public double CalcTicks()
        {
            frameCount++;
            double dt = 0;
            DateTime time = DateTime.Now;
            long ticks = time.Ticks;
            if (frameCount >= 100)
            {
                if (lastTicks != 0)
                {
                    long difference = (ticks - lastTicks);
                    dt = difference * 1.0e-7;
                }
                lastTicks = ticks;
            }
            return dt;
        }
        //calls all the sprites' draw method and the ball's move method (if unpaused)
        public void DrawSprites(Canvas canvas)
        {
            double dt = CalcTicks();
            while (dt > 0)
            {
                double slice = Math.Min(0.001, dt);
                ball.Move(canvas.Height, canvas.Width, slice, 0);
                dt -= slice;
            }
            ball.Draw(canvas);

            bar.Draw(canvas);
            foreach (Brick brick in bricks)
            {
                if (brick != null)
                    brick.Draw(canvas);
            }
        }
        public void ChargeMode(bool isCharge)
        {
            if (isCharge)
            {
                ball.ChangeColor(SpriteColor.Gold);
                bar.ChangeColor(SpriteColor.Gold);
                foreach (Brick brick in bricks)
                {
                    brick.ChangeColor(SpriteColor.Gold);
                }
            }
            else
            {
                SpriteColor ballColor = (SpriteColor)sp.GetInt(context.Resources.GetString(
                Resource.String.ball_color), 6);
                SpriteColor barColor = (SpriteColor)sp.GetInt(context.Resources.GetString(
                Resource.String.bar_color), 0);
                ball.ChangeColor(ballColor);
                bar.ChangeColor(barColor);
                int[,] colors = designer.GetLevel();
                for (int i = 0; i < bricks.GetLength(0); i++)
                {
                    for (int j = 0; j < bricks.GetLength(1); j++)
                    {
                        if (bricks[i, j] != null)
                        {
                            bricks[i, j].ChangeColor((SpriteColor)colors[i, j]);
                        }
                    }
                }
            }
        }
        //recieves a new location for the bar and updates it
        public void MoveBar(int x)
        {
            bar.Move(screenWidth, screenHeight, 0, x);
        }
        //calculates the difference between the brick's size on the spritesheet 
        //and the brick's size on screen
        public float CalcDifference()
        {
            float space = context.Resources.GetDimension(Resource.Dimension.between);
            float sides = context.Resources.GetDimension(Resource.Dimension.sides);
            int OGwidth = context.Resources.GetInteger(Resource.Integer.brick_width);
            float width = (screenWidth - sides * 2 - (bricks.GetLength(1) - 1) * space) /
                bricks.GetLength(1);
            return width / OGwidth;
        }
        //returns true if the game is over or false otherwise 
        public bool GameOver()
        {
            if (DrawSurfaceView.lives <= 0)
                return true;
            for (int i = 0; i < bricks.GetLength(0); i++)
            {
                for (int j = 0; j < bricks.GetLength(1); j++)
                {
                    if (bricks[i, j] != null)
                        return false;
                }
            }
            return true;
        }
    }
}
