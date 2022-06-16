using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace BrickBreaker.Activities
{
    public class PauseDialog : Dialog, View.IOnClickListener
    {
        Button btnRes;
        Button btnQuit;
        Button btnSetting;
        Button btnRestart;
        public bool quit;
        public bool resume;
        public bool restart;
        int width;
        int height;
        public PauseDialog(Context context) : base(context)
        {
            restart = false;
            quit = false;
            resume = false;
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.paused);
            SetTitle("Game Paused");
            SetCancelable(false);
            Window.SetBackgroundDrawableResource(Resource.Drawable.back);
            width = Context.Resources.DisplayMetrics.WidthPixels;
            height = Context.Resources.DisplayMetrics.HeightPixels;
            btnRes = SetButton(btnRes, Resource.Id.btnRes);
            btnSetting = SetButton(btnSetting, Resource.Id.btnSetting);
            btnRestart = SetButton(btnRestart, Resource.Id.btnRestart);
            btnQuit = SetButton(btnQuit, Resource.Id.btnQuit);
            Window.SetLayout(width, height);
            TextView title = FindViewById<TextView>(Resource.Id.pause_title);
            title.Typeface = MainActivity.font;
        }
        //initializes all of the buttons and sets their font and background
        Button SetButton(Button b, int idRes)
        {
            b = FindViewById<Button>(idRes);
            var drawable = new StateListDrawable();
            drawable.AddState(new[] { Android.Resource.Attribute.StatePressed },
                Context.Resources.GetDrawable(Resource.Drawable.button_pressed));
            drawable.AddState(drawable.GetState(),
                Context.Resources.GetDrawable(Resource.Drawable.button_reg));
            b.SetOnClickListener(this);
            b.Typeface = MainActivity.font;
            b.Background = drawable;
            return b;
        }
        //listens for button clicks
        public void OnClick(View v)
        {
            if (v == btnQuit)
            {
                Dismiss();
                quit = true;
            }
            else if (v == btnSetting)
            {
                SettingDialog settingActivity = new SettingDialog(Context);
                settingActivity.Show();
            }
            else if (v == btnRestart)
            {
                Dismiss();
                restart = true;
            }
            else if (v == btnRes)
            {
                Dismiss();
                resume = true;
            }
        }
    }
}