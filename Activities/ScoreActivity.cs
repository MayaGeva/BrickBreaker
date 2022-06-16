using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Preferences;
using Android.Views;
using Android.Widget;
using Android.Util;

namespace BrickBreaker.Activities
{
    [Activity(Label = "Scores")]
    public class ScoreActivity : Activity
    {
        TableLayout layout;
        ISharedPreferences sp;
        TextView[] scores;
        int scoresNum;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            //var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            //SetActionBar(toolbar);
            ActionBar.SetHomeButtonEnabled(true);
            ActionBar.SetDisplayHomeAsUpEnabled(true);
            sp = PreferenceManager.GetDefaultSharedPreferences(Application.Context);
            layout = new TableLayout(this);
            layout.SetGravity(GravityFlags.Center);
            TextView title = new TextView(this);
            title.SetTextSize(ComplexUnitType.Sp, 45);
            title.Text = "Top Five Scores";
            title.Typeface = MainActivity.font;
            title.SetTextColor(Color.White);
            title.Gravity = GravityFlags.CenterHorizontal;
            title.SetPadding(0, 20, 0, 40);
            layout.AddView(title);
            layout.SetBackgroundDrawable(Resources.GetDrawable(Resource.Drawable.back));
            scores = new TextView[5];
            Init();
            SetContentView(layout);
        }
        //builds the layout of the score activity
        void Init()
        {
            scoresNum = 0;
            for (int i = 0; i < 5; i++)
            {
                scores[i] = new TextView(this) { Gravity = GravityFlags.Left };
                scores[i].SetTextSize(ComplexUnitType.Sp, 30);
                scores[i].SetTextColor(Color.White);
                TableRow row = new TableRow(this);
                row.SetPadding(0, 20, 0, 40);
                row.SetGravity(GravityFlags.CenterHorizontal);
                int scr = sp.GetInt("top" + (i + 1) + "score", 0);
                if (scr > 0)
                {
                    scores[i].Text = "#" + (i + 1) + " " +
                        sp.GetString("top" + (i + 1), "") + ": " + scr;
                    scores[i].Typeface = MainActivity.font;
                    scores[i].Gravity = GravityFlags.CenterHorizontal;
                    row.AddView(scores[i]);
                    layout.AddView(row);
                    scoresNum++;
                }
                else if (i == 0)
                {
                    scores[i].Text = "There Aren\'t Any Scores Yet.";
                }
            }
        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.score_menu, menu);
            return true;
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.action_delete)
                DeleteScores();
            else if (item.ItemId == Android.Resource.Id.Home)
                Finish();
            return base.OnOptionsItemSelected(item);
        }
        void DeleteScores()
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetTitle("Delete Scores");
            builder.SetMessage("Are you sure you wan\'t to delete the scores?\nThis is unreversable.");
            builder.SetPositiveButton("Ok", OkAction);
            builder.SetNegativeButton("Cancel", CancelAction);
            builder.Create().Show();
        }

        private void OkAction(object sender, DialogClickEventArgs e)
        {
            ISharedPreferencesEditor ed = sp.Edit();
            for (int i = 0; i < scoresNum; i++)
            {
                ed.PutString("top" + (i + 1), null);
                ed.PutInt("top" + (i + 1) + "score", 0);
            }
            ed.Commit();
            Finish();
        }

        private void CancelAction(object sender, DialogClickEventArgs e)
        {
            if (sender is AlertDialog dialog)
                dialog.Cancel();
        }
    }
}