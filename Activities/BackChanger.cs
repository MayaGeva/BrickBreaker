using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Preferences;
using Android.Views;
using Android.Widget;
using System;
using System.IO;

namespace BrickBreaker.Activities
{
    [Activity(Label = "Change Background")]
    public class BackChanger : Activity, View.IOnClickListener
    {
        Button btn;
        ImageView img;
        string backPath;
        ISharedPreferences sp;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.background_changer);
            sp = PreferenceManager.GetDefaultSharedPreferences(Application.Context);
            ActionBar.SetHomeButtonEnabled(true);
            ActionBar.SetDisplayHomeAsUpEnabled(true);
            btn = FindViewById<Button>(Resource.Id.btnBack);
            btn.SetOnClickListener(this);
            btn.Typeface = MainActivity.font;
            btn.SetTextColor(Color.White);
            TextView tv = FindViewById<TextView>(Resource.Id.backtitle);
            tv.Typeface = MainActivity.font;
            tv.SetTextColor(Color.White);
            img = FindViewById<ImageView>(Resource.Id.backView);
            img.SetMinimumHeight(img.Height);
            img.SetMinimumWidth(img.Width);
            backPath = sp.GetString(Resources.GetString(Resource.String.back_path), null);
            if (backPath != null)
                img.SetImageBitmap(BitmapFactory.DecodeFile(backPath));
        }
        public void OnClick(View v)
        {
            if (v == btn)
            {
                Intent intent = new Intent(Android.Provider.MediaStore.ActionImageCapture);
                StartActivityForResult(intent, 0);
            }
        }
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == 0)//coming from camera
            {
                if (resultCode == Result.Ok)
                {
                    Bitmap bitmap = (Bitmap)data.Extras.Get("data");
                    img.SetImageBitmap(bitmap);
                    SaveImageToExternalStorage(bitmap);
                }
            }
        }
        private void SaveImageToExternalStorage(Bitmap finalBitmap)
        {
            string root = Android.OS.Environment.GetExternalStoragePublicDirectory(
                Android.OS.Environment.DirectoryPictures).ToString();
            Java.IO.File myDir = new Java.IO.File(root + "/saved_images");
            myDir.Mkdirs();
            Random generator = new Random();
            int n = 10000;
            n = generator.Next(n);
            string fname = "Image-" + n + ".jpg";
            Java.IO.File file = new Java.IO.File(myDir, fname);

            if (file.Exists())
                file.Delete();
            try
            {
                string path = System.IO.Path.Combine(myDir.AbsolutePath, fname);
                var fs = new FileStream(path, FileMode.Create);
                if (fs != null)
                {
                    finalBitmap.Compress(Bitmap.CompressFormat.Png, 90, fs);
                    ISharedPreferencesEditor ed = sp.Edit();
                    ed.PutString(Resources.GetString(Resource.String.back_path), path);
                    ed.Commit();
                }
                fs.Flush();
                fs.Close();
            }
            catch (Exception e)
            {
                Toast.MakeText(this, e.Message, ToastLength.Long).Show();
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.back_menu, menu);
            return true;
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.action_default_back)
            {
                ISharedPreferencesEditor ed = sp.Edit();
                ed.PutString(Resources.GetString(Resource.String.back_path), null);
                ed.Commit();
                img.SetImageDrawable(Resources.GetDrawable(Resource.Drawable.earth_pixel));
            }
            else if (item.ItemId == Android.Resource.Id.Home)
                Finish();
            return base.OnOptionsItemSelected(item);
        }
    }
}