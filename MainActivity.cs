using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using BrickBreaker.Activities;


namespace BrickBreaker
{
    [Activity(Label = "@string/app_name",
        Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, View.IOnClickListener,
        IDialogInterfaceOnClickListener
    {
        Button btnPlay;
        Button btnSetting;
        Button btnTopFive;
        Button newBack;
        TextView title;
        public static Typeface font;

        Button count;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            Window.RequestFeature(WindowFeatures.NoTitle);
            Window.SetFlags(WindowManagerFlags.Fullscreen,
                WindowManagerFlags.Fullscreen);
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            font = Typeface.CreateFromAsset(Assets, "fonts/november.ttf");
            title = FindViewById<TextView>(Resource.Id.mainTitle);
            title.SetTextColor(Color.LightBlue);
            title.Typeface = font;
            btnPlay = SetButton(btnPlay, Resource.Id.btnPlay);
            btnSetting = SetButton(btnSetting, Resource.Id.btnSetting);
            btnTopFive = SetButton(btnTopFive, Resource.Id.btnTopFive);
            newBack = SetButton(newBack, Resource.Id.buttonBack);
            count = SetButton(count, Resource.Id.buttonCount);
        }

        //initializes each of the buttons and sets their font and background
        Button SetButton(Button b, int idRes)
        {
            var drawable = new StateListDrawable();
            drawable.AddState(new[] { Android.Resource.Attribute.StatePressed },
                Resources.GetDrawable(Resource.Drawable.button_pressed));
            drawable.AddState(drawable.GetState(),
                Resources.GetDrawable(Resource.Drawable.button_reg));
            b = FindViewById<Button>(idRes);
            b.SetOnClickListener(this);
            b.Typeface = font;
            b.Background = drawable;
            return b;
        }
        public override void OnRequestPermissionsResult(int requestCode,
            string[] permissions,
            [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode,
                permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        //listens for button clicks and acts 
        //according the button that was clicked
        public void OnClick(View v)
        {
            if (v == btnPlay)
            {
                Intent intent = new Intent(this, typeof(GameActivity));
                StartActivity(intent);
            }
            else if (v == btnSetting)
            {
                SettingDialog setting = new SettingDialog(this);
                setting.Show();
            }
            else if (v == newBack)
            {
                Intent intent = new Intent(this, typeof(BackChanger));
                StartActivity(intent);
            }
            else if (v == count)
            {
                Intent intent = new Intent(this, typeof(CountActivity));
                StartActivity(intent);
            }
            else if (v == btnTopFive)
            {
                ISharedPreferences sp = PreferenceManager.
                    GetDefaultSharedPreferences(Application.Context);
                if (sp.GetInt(Resources.GetString(Resource.String.top1_score),
                    -1) > 0)
                {
                    Intent intent = new Intent(this, typeof(ScoreActivity));
                    StartActivity(intent);
                }
                else
                {
                    Android.App.AlertDialog.Builder builder =
                        new Android.App.AlertDialog.Builder(this);
                    builder.SetTitle("Top Score");
                    builder.SetMessage("There aren\'t any scores yet");
                    builder.SetPositiveButton("Ok", this);
                    builder.Create().Show();
                }
            }
        }
        //listens for when an alert dialog button is pressed
        public void OnClick(IDialogInterface dialog, int which)
        {
            dialog.Dismiss();
        }
    }
}