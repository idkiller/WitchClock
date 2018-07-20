using System;
using Tizen.Applications;
using Tizen.Wearable.CircularUI.Forms.Renderer.Watchface;

namespace WitchClock
{
    class WitchWatch : FormsWatchface
    {
        bool initialized;
        WitchWatchApplication witchWatch;
        protected override void OnCreate()
        {
            base.OnCreate();
            witchWatch = new WitchWatchApplication();
            LoadWatchface(witchWatch);
        }

        protected override void OnTick(TimeEventArgs time)
        {
            // UtcTimestamp has not millseconds information!!!!
            witchWatch.Time = time.Time.UtcTimestamp + TimeSpan.FromMilliseconds(time.Time.Millisecond);
        }

        protected override void OnResume()
        {
            if (!initialized)
            {
                witchWatch.MoveHands(900).ContinueWith(t => witchWatch.Run());
                initialized = true;
                Console.WriteLine("----------------- 1");
            }
            else
            {
                witchWatch.Run();
                Console.WriteLine("----------------- 2");
            }

            base.OnResume();
        }

        protected override void OnPause()
        {
            base.OnPause();
            witchWatch.StopHands();
        }

        protected override void OnAmbientChanged(AmbientEventArgs mode)
        {
            base.OnAmbientChanged(mode);
        }

        protected override void OnAmbientTick(TimeEventArgs time)
        {
            base.OnAmbientTick(time);
        }

        static void Main(string[] args)
        {
            var watch = new WitchWatch();
            global::Xamarin.Forms.Platform.Tizen.Forms.Init(watch);
            global::Tizen.Wearable.CircularUI.Forms.Renderer.FormsCircularUI.Init();
            watch.Run(args);
        }
    }
}
