using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.Preferences;
using Android.Views;
using Android.Widget;
using BrickBreaker.Activities;
using BrickBreaker.Backend.Base;
using System;
using System.Threading;

namespace BrickBreaker.Backend
{
    class DrawSurfaceView : SurfaceView
    {
        //general
        public static int points = 0;
        public static int lives = 3;
        Context context;
        Rect backSrc;
        Rect screen;
        ISharedPreferences sp;
        public int diff;

        //threads
        public Thread gameThread;
        ThreadStart ts;
        public bool threadRunning = true;
        bool isRunning = true;

        //visuals
        public static Bitmap spriteSheet;
        Bitmap back;
        public SpriteManager Sprites { get; }

        //sound
        MediaPlayer p;
        SoundPool mp;
        int beep;
        public DrawSurfaceView(Context context) : base(context)
        {
            this.context = context;
            sp = PreferenceManager.GetDefaultSharedPreferences(Application.Context);
            screen = new Rect(0, 0, Resources.DisplayMetrics.WidthPixels,
                Resources.DisplayMetrics.HeightPixels);
            BitmapFactory.Options option = new BitmapFactory.Options { InScaled = false };
            spriteSheet = BitmapFactory.DecodeResource(context.Resources,
                Resource.Drawable.breakout_pieces, option);
            backSrc = CreateBack(Resource.Drawable.earth_pixel);
            sp = PreferenceManager.GetDefaultSharedPreferences(Application.Context);
            mp = new SoundPool(3, Stream.Music, 1);
            p = MediaPlayer.Create(context, Resource.Raw.music);
            p.Looping = true;
            beep = mp.Load(context, Resource.Raw.beep, 1);
            diff = GetLevel();
            Sprites = new SpriteManager(screen.Width(), screen.Height(), diff, 7, context,
                mp, beep);

            ts = new ThreadStart(Run);
            gameThread = new Thread(ts);
        }
        public void UpdateSoundManager()
        {
            bool mute = sp.GetBoolean(Resources.GetString(Resource.String.is_volume),
                false);
            float log1 = 0;
            if (!mute)
            {
                int maxVolume = 5;
                int currVolume = sp.GetInt(Resources.GetString(
                    Resource.String.music_volume), 5);
                log1 = 1;
                if (maxVolume != currVolume)
                    log1 = (float)(1 - (Math.Log(maxVolume - currVolume) /
                        Math.Log(maxVolume)));
            }
            Sprites.UpdateVolume();
            p.SetVolume(log1, log1);
        }
        //calculates the Rect of the background image
        Rect CreateBack(int idRes)
        {
            Rect r = new Rect();
            BitmapFactory.Options option = new BitmapFactory.Options { InScaled = false };
            string b = sp.GetString(Resources.GetString(Resource.String.back_path), null);
            if (b == null)
                back = BitmapFactory.DecodeResource(context.Resources, idRes, option);
            else
            {
                Java.IO.File file = new Java.IO.File(b);
                back = BitmapFactory.DecodeFile(file.AbsolutePath);
            }
            float ratioWidth = screen.Width() / back.Width;
            float ratioHeight = screen.Height() / back.Height;
            if (ratioHeight > ratioWidth)
            {
                r.Top = 0;
                r.Bottom = back.Height;
                float excess = ((back.Width * ratioHeight) - screen.Width()) /
                    ratioHeight;
                r.Right = back.Width;
                r.Left = (int)excess;
            }
            else
            {
                r.Left = 0;
                r.Right = back.Width;
                float excess = ((back.Height * ratioWidth) - screen.Height()) /
                    ratioWidth;
                r.Bottom = back.Height;
                r.Top = (int)excess;
            }
            return r;
        }
        //the method that runs on the game thread
        public void Run()
        {
            while (gameThread.IsAlive)
            {
                if (isRunning)
                {
                    if (!Holder.Surface.IsValid)
                        continue;
                    Canvas c = Holder.LockCanvas(); ;
                    try
                    {
                        c.DrawBitmap(back, backSrc, screen, null);
                        Sprites.Update(c);
                        DrawText(c);
                        GameOver();
                    }
                    catch (Exception e)
                    {
                        Toast.MakeText(context, e.Message, ToastLength.Long).Show();
                    }
                    finally
                    {
                        if (c != null)
                        {
                            Holder.UnlockCanvasAndPost(c);
                        }
                    }
                }
            }
        }
        //get the number of rows by checking the selected difficulty
        int GetLevel()
        {
            LevelDifficulty level = (LevelDifficulty)sp.GetInt(
                context.Resources.GetString(Resource.String.diff_level), 1);
            return level switch
            {
                LevelDifficulty.Easy => 5,
                LevelDifficulty.Normal => 7,
                LevelDifficulty.Difficult => 10,
                LevelDifficulty.Hard => 12,
                LevelDifficulty.VeryHard => 15,
                _ => 7,
            };
        }
        //draws the score and lives indicator on screen
        public void DrawText(Canvas canvas)
        {
            Paint p = new Paint { Color = Color.Gold };
            p.TextAlign = Paint.Align.Left;
            p.TextSize = context.Resources.GetDimension(Resource.Dimension.gameText);
            p.SetTypeface(MainActivity.font);
            float y = context.Resources.GetDimension(Resource.Dimension.gamePointsY);
            float sx = context.Resources.GetDimension(Resource.Dimension.gamePointsX);
            float lx = context.Resources.GetDimension(Resource.Dimension.gameLivesX);
            canvas.DrawText("Score: " + DrawSurfaceView.points, sx, y, p);
            canvas.DrawText("Lives: " + DrawSurfaceView.lives, lx, y, p);
        }
        public void GoldMode(bool isCharge) { Sprites.ChargeMode(isCharge); }
        //destroy the game
        public void Destroy()
        {
            p.Stop();
            isRunning = false;
        }
        //pause the game
        public void Pause()
        {
            p.Pause();
            isRunning = false;
            mp.AutoPause();
        }
        //resume the game
        public void Resume()
        {
            p.Start();
            isRunning = true;
            mp.AutoResume();
            Sprites.ball.Wait();
        }
        //start the game
        public void StartGame()
        {
            p.Start();
            isRunning = true;
            points = 0;
            lives = 3;
        }
        //reset the game and the game thread
        public void ResetGame()
        {
            threadRunning = false;
            while (true)
            {
                try
                {
                    gameThread.Join();
                }
                catch (System.Exception e1)
                {
                    Toast.MakeText(context, e1.Message, ToastLength.Long).Show();
                }
                break;
            }
            //Game Thread
            threadRunning = true;
            isRunning = true;
            ts = new ThreadStart(Run);
            gameThread = new System.Threading.Thread(ts);
        }
        //checks if the game is over and initiates 
        //the game over dialog if it is
        public void GameOver()
        {
            if (Sprites.GameOver())
            {
                Pause();
                ((GameActivity)context).RunOnUiThread(() =>
                { ((GameActivity)context).GameOverDialog(); });
            }
        }
        public override bool OnTouchEvent(MotionEvent e)
        {
            if (MotionEventActions.Move == e.Action)
            {
                Sprites.MoveBar((int)e.GetX());
            }
            return true;
        }
    }
}