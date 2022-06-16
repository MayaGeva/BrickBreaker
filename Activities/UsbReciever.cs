using Android.Content;
using Android.Widget;

namespace BrickBreaker.Activities
{
    [BroadcastReceiver]
    public class UsbReciever : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            //Toast.MakeText(context, "Received intent!", ToastLength.Short).Show();
            string action = intent.Action;
            if (action.Equals("android.intent.action.ACTION_POWER_CONNECTED"))
            {
                if (context is GameActivity game)
                    game.GoldMode(true);
                Toast.MakeText(context, "Gold Mode Unlocked!",
                    ToastLength.Long).Show();
            }
            else if (action.Equals("android.intent.action.ACTION_POWER_DISCONNECTED"))
            {
                if (context is GameActivity game)
                    game.GoldMode(false);
            }
        }
    }
}