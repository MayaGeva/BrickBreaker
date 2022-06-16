using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Preferences;
using Android.Text;
using Android.Views;
using Android.Widget;
using BrickBreaker.Backend;
using BrickBreaker.Backend.Base;
using Java.Lang;
using System;

namespace BrickBreaker.Activities
{
    public class GameOverDialog : Dialog, View.IOnClickListener, ITextWatcher
    {
        ISharedPreferences sp;
        Button btnSave;
        Button btnCont;
        EditText etName;
        TextView tv;
        public GameOverDialog(Context context) : base(context)
        {
            sp = PreferenceManager.GetDefaultSharedPreferences(Application.Context);
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.game_over);
            TextView title = FindViewById<TextView>(Resource.Id.game_over_title);
            title.Typeface = MainActivity.font;
            Window.SetBackgroundDrawableResource(Resource.Drawable.back);
            SetTitle("Game Over");
            SetCancelable(false);
            btnSave = FindViewById<Button>(Resource.Id.saveName);
            btnSave.SetOnClickListener(this);
            btnSave.Enabled = false;
            btnSave.Typeface = MainActivity.font;
            btnCont = FindViewById<Button>(Resource.Id.cont);
            btnCont.SetOnClickListener(this);
            btnCont.Typeface = MainActivity.font;
            var drawable = new StateListDrawable();
            drawable.AddState(new[] { Android.Resource.Attribute.StatePressed },
                Context.Resources.GetDrawable(Resource.Drawable.button_pressed));
            drawable.AddState(drawable.GetState(),
                Context.Resources.GetDrawable(Resource.Drawable.button_reg));
            btnSave.Background = drawable;
            etName = FindViewById<EditText>(Resource.Id.etName);
            etName.Typeface = MainActivity.font;
            tv = FindViewById<TextView>(Resource.Id.tvPoints);
            tv.Text += DrawSurfaceView.points;
            tv.Typeface = MainActivity.font;
            etName.AddTextChangedListener(this);
            int width = Context.Resources.DisplayMetrics.WidthPixels;
            int height = Context.Resources.DisplayMetrics.HeightPixels;
            Window.SetLayout(width, height);
        }
        //updates the top five players and their scores in the shared prefences
        public void UpdateTopFive(string name, int score)
        {
            ISharedPreferencesEditor edit = sp.Edit();
            Player p = new Player(score, name);
            Player[] players = new Player[6];
            for (int i = 0; i < players.Length - 1; i++)
            {
                players[i] = new Player(sp.GetInt("top" + (i + 1) + "score", -1),
                    sp.GetString("top" + (i + 1), ""));
            }
            players[5] = p;
            Array.Sort(players, (x, y) => x.GetScore().CompareTo(y.GetScore()));
            Array.Reverse(players);
            for (int i = 0; i < players.Length - 1; i++)
            {
                if (players[i].GetScore() != -1)
                {
                    edit.PutString("top" + (i + 1), players[i].GetName());
                    edit.PutInt("top" + (i + 1) + "score", players[i].GetScore());
                }
            }
            edit.Commit();
        }
        //listens for click on the confirm button or continue button
        public void OnClick(View v)
        {
            if (v == btnSave)
            {
                if (etName.Text != null)
                {
                    UpdateTopFive(etName.Text, DrawSurfaceView.points);
                    Dismiss();
                }
            }
            else if (v == btnCont)
            {
                Dismiss();
            }
        }
        public void AfterTextChanged(IEditable s) { }
        public void BeforeTextChanged(ICharSequence s, int start, int count,
            int after) { }
        //checks if the edittext is null and enables the confirm button if it isn't
        public void OnTextChanged(ICharSequence s, int start, int before, int count)
        {
            if (etName.Text.Length > 0)
            {
                btnSave.Enabled = true;
            }
            else
                btnSave.Enabled = false;
        }
    }
}