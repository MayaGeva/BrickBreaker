using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Preferences;
using Android.Views;
using Android.Widget;
using System;

namespace BrickBreaker.Activities
{
    public class SettingDialog : Dialog, View.IOnClickListener
    {
        SeekBar musicVolBar;
        SeekBar sfxVolBar;
        Switch muteSwitch;
        Spinner level;
        Spinner ballColor;
        Spinner barColor;
        Switch vibSwitch;
        Button save;
        ISharedPreferences sp;
        public SettingDialog(Context context) : base(context) { }

        private void SettingActivity_DismissEvent(object sender, EventArgs e)
        {
            Toast.MakeText(Context, "Settings Saved", ToastLength.Short).Show();
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            Window.RequestFeature(WindowFeatures.ActionBar);
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.setting);
            // Create your application here

            DismissEvent += SettingActivity_DismissEvent;
            sp = PreferenceManager.GetDefaultSharedPreferences(Application.Context);
            ballColor = FindViewById<Spinner>(Resource.Id.ballColorSpinner);
            barColor = FindViewById<Spinner>(Resource.Id.barColorSpinner);
            musicVolBar = FindViewById<SeekBar>(Resource.Id.musicVolBar);
            musicVolBar.Max = 5;
            sfxVolBar = FindViewById<SeekBar>(Resource.Id.sfxVolBar);
            sfxVolBar.Max = 5;
            muteSwitch = FindViewById<Switch>(Resource.Id.muteSwitch);
            vibSwitch = FindViewById<Switch>(Resource.Id.vibSwitch);
            level = FindViewById<Spinner>(Resource.Id.diffSpinner);
            save = FindViewById<Button>(Resource.Id.saveSet);
            Init();
        }
        //initializes all the values of the setting rows
        //from the shared preference
        public void Init()
        {
            muteSwitch.Checked = sp.GetBoolean(Context.Resources.GetString(
                Resource.String.is_volume), false);
            vibSwitch.Checked = sp.GetBoolean(Context.Resources.GetString(
                Resource.String.is_vibrate), true);
            musicVolBar.Progress = sp.GetInt(Context.Resources.GetString(
                Resource.String.music_volume), 5);
            sfxVolBar.Progress = sp.GetInt(Context.Resources.GetString(
                Resource.String.sfx_volume), 5);
            if (muteSwitch.Checked)
            {
                musicVolBar.Enabled = false;
                sfxVolBar.Enabled = false;
            }
            level.SetSelection(sp.GetInt(Context.Resources.GetString(
                Resource.String.diff_level), 1));
            ballColor.SetSelection(sp.GetInt(Context.Resources.GetString(
                Resource.String.ball_color), 6));
            barColor.SetSelection(sp.GetInt(Context.Resources.GetString(
                Resource.String.bar_color), 0));
            musicVolBar.ProgressChanged += MusicVolBar_ProgressChanged;
            sfxVolBar.ProgressChanged += SfxVolBar_ProgressChanged;
            muteSwitch.CheckedChange += MuteSwitch_CheckedChange;
            vibSwitch.CheckedChange += VibSwitch_CheckedChange;
            level.ItemSelected += Level_ItemSelected;
            ballColor.ItemSelected += BallColor_ItemSelected;
            barColor.ItemSelected += BarColor_ItemSelected;
            save.SetOnClickListener(this);
        }

        private void BallColor_ItemSelected(object sender,
            AdapterView.ItemSelectedEventArgs e)
        {
            ISharedPreferencesEditor editor = sp.Edit();
            editor.PutInt(Context.Resources.GetString(Resource.String.ball_color),
                ballColor.SelectedItemPosition);
            editor.Commit();
        }

        private void BarColor_ItemSelected(object sender,
            AdapterView.ItemSelectedEventArgs e)
        {
            ISharedPreferencesEditor editor = sp.Edit();
            editor.PutInt(Context.Resources.GetString(Resource.String.bar_color),
                barColor.SelectedItemPosition);
            editor.Commit();
        }

        public void Level_ItemSelected(object sender,
            AdapterView.ItemSelectedEventArgs e)
        {
            ISharedPreferencesEditor editor = sp.Edit();
            editor.PutInt(Context.Resources.GetString(Resource.String.diff_level),
                level.SelectedItemPosition);
            editor.Commit();
        }
        public void VibSwitch_CheckedChange(object sender,
            CompoundButton.CheckedChangeEventArgs e)
        {
            ISharedPreferencesEditor editor = sp.Edit();
            editor.PutBoolean(Context.Resources.GetString(Resource.String.is_vibrate),
                vibSwitch.Checked);
            editor.Commit();
        }

        public void SfxVolBar_ProgressChanged(object sender,
            SeekBar.ProgressChangedEventArgs e)
        {
            ISharedPreferencesEditor editor = sp.Edit();
            editor.PutInt(Context.Resources.GetString(Resource.String.sfx_volume),
                sfxVolBar.Progress);
            editor.Commit();
        }
        public void MuteSwitch_CheckedChange(object sender,
            CompoundButton.CheckedChangeEventArgs e)
        {
            musicVolBar.Enabled = !muteSwitch.Checked;
            sfxVolBar.Enabled = !muteSwitch.Checked;
            ISharedPreferencesEditor editor = sp.Edit();
            editor.PutBoolean(Context.Resources.GetString(Resource.String.is_volume),
                muteSwitch.Checked);
            editor.Commit();
        }
        public void MusicVolBar_ProgressChanged(object sender,
            SeekBar.ProgressChangedEventArgs e)
        {
            ISharedPreferencesEditor editor = sp.Edit();
            editor.PutInt(Context.Resources.GetString(Resource.String.music_volume),
                musicVolBar.Progress);
            editor.Commit();
        }
        public void OnClick(View v)
        {
            if (v == save)
            {
                Dismiss();
            }
        }
    }
}