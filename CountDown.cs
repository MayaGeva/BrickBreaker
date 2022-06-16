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
    [Activity(Label = "CountDown")]
    public class CountDown : SurfaceView
    {
        public Thread thread;
        ThreadStart ts;
        bool isRunning;
        int count;
        public CountDown(Context context) : base(context)
        {
            ts = new ThreadStart(Count);
            thread = new Thread(ts);
            isRunning = true;
            count = 10;
            countView = new TextView(context) { Text = count.ToString() };
        }
        public void Count()
        {
            while (thread.IsAlive)
            {
                if (isRunning)
                {
                    (()
                    Thread.Sleep(1000);
                    if (count <= 0)
                        isRunning = false;
                }
            }
        }
    }
}