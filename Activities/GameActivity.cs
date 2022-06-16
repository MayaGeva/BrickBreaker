using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Android.Widget;
using BrickBreaker.Backend;

namespace BrickBreaker.Activities
{
    [Activity(Label = "GameActivity",
        ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class GameActivity : Activity, IDialogInterfaceOnDismissListener,
        View.IOnClickListener
    {
        DrawSurfaceView ds;
        FrameLayout main;
        ImageButton btnPause;
        UsbReciever usb;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your application here
            usb = new UsbReciever();
            ds = new DrawSurfaceView(this);
            main = new FrameLayout(this);
            LinearLayout layout = new LinearLayout(this);
            ActionBar.Hide();
            int pauseSize = (int)Resources.GetDimension(Resource.Dimension.pause_size);
            layout.LayoutParameters = new LinearLayout.LayoutParams(pauseSize, pauseSize);
            layout.SetX((Resources.DisplayMetrics.WidthPixels / 2) - (pauseSize / 2));
            btnPause = new ImageButton(this, null, Resource.Style.pause_button_style)
            { Background = new BitmapDrawable(BitmapFactory.DecodeResource(
                    Resources, Resource.Drawable.pause_icon,
                    new BitmapFactory.Options { InScaled = false }))};
            btnPause.SetOnClickListener(this);
            SetContentView(main);
            layout.AddView(btnPause);
            main.AddView(ds);
            main.AddView(layout);
            ds.gameThread.Start();
            ds.UpdateSoundManager();
        }
        //called when the user presses the back button
        public override void OnBackPressed()
        {
            OnPause();
        }
        //resumes the game thread
        public void ResumeGame()
        {
            ds.Resume();
        }
        protected override void OnResume()
        {
            base.OnResume();
            RegisterReceiver(usb, new
                IntentFilter("android.intent.action.ACTION_POWER_CONNECTED"));
            RegisterReceiver(usb, new
                IntentFilter("android.intent.action.ACTION_POWER_DISCONNECTED"));
        }
        //called at the start of the activity
        protected override void OnStart()
        {
            base.OnStart();
            ds.StartGame();
        }
        //called when the activity is paused
        protected override void OnPause()
        {
            base.OnPause();
            ds.Pause();
            PauseDialog();
        }
        public void GoldMode(bool isCharge) { ds.GoldMode(isCharge); }
        //creates and shows the game over dialog
        public void GameOverDialog()
        {
            DrawSurfaceView.points += ds.diff * 50 + DrawSurfaceView.lives * 100;
            GameOverDialog gameOver = new GameOverDialog(this);
            gameOver.Show();
            gameOver.SetOnDismissListener(this);
        }
        //creates and shows the pause dialog
        public void PauseDialog()
        {
            PauseDialog pauseDialog = new PauseDialog(this);
            pauseDialog.Show();
            pauseDialog.SetOnDismissListener(this);
        }
        //called when a dialog gets dismissed
        //and does actions according to the dialog dismissed
        public void OnDismiss(IDialogInterface dialog)
        {
            if (dialog is PauseDialog dialog1)
            {
                if (dialog1.resume)
                {
                    ds.UpdateSoundManager();
                    ResumeGame();
                }
                else if (dialog1.quit)
                {
                    ds.Destroy();
                    Finish();
                    UnregisterReceiver(usb);
                }
                else if (dialog1.restart)
                {
                    ds.Destroy();
                    Finish();
                    StartActivity(Intent);
                }
            }
            if (dialog is GameOverDialog)
            {
                Finish();
                UnregisterReceiver(usb);
            }
        }
        //listens for clicks on the pause button
        public void OnClick(View v)
        {
            if (v == btnPause)
            {
                OnPause();
            }
        }
    }
}