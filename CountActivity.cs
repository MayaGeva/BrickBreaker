using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace BrickBreaker
{
    [Activity(Label = "CountActivity")]
    public class CountActivity : Activity
    {
        //CountDown count;
        public Thread thread;
        ThreadStart ts;
        bool isRunning;

        LinearLayout layout;
        TextView countView;
        int count;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            layout = new LinearLayout(this);
            count = 10;
            ts = new ThreadStart(Count);
            thread = new Thread(ts);
            isRunning = true;
            thread.Start();
            countView = new TextView(this) { Text = count.ToString() };
            layout.AddView(countView);
            SetContentView(layout);
        }
        public void Count()
        {
            while (thread.IsAlive)
            {
                if (isRunning)
                {
                    count--;
                    countView.Text = count.ToString();
                    Thread.Sleep(1000);
                    if (count <= 0)
                        isRunning = false;
                }
            }
        }
        void UpdateCount()
        {
            
        }
    }
}