using System;
using Tizen.Applications;
using Tizen.Sensor;
using Tizen.Wearable.CircularUI.Forms.Renderer.Watchface;

namespace WitchClock
{
    class WitchWatch : FormsWatchface
    {
        bool initialized;
        WitchWatchApplication witchWatch;
        GravitySensor gravity;

        protected override void OnCreate()
        {
            base.OnCreate();
            witchWatch = new WitchWatchApplication();
            LoadWatchface(witchWatch);
            initialized = false;

            var now = GetCurrentTime().UtcTimestamp;
            var initTime = new DateTime(now.Year, now.Month, now.Day, 8, 31, 3);
            witchWatch.Time = initTime;
            witchWatch.MoveHands();

            if (GravitySensor.IsSupported)
            {
                gravity = new GravitySensor();
                gravity.DataUpdated += OnSensorUpdate;
            }
        }

        void OnSensorUpdate(object sender, GravitySensorDataUpdatedEventArgs e)
        {
            var dx = 20.0 * e.X / 9.8;
            var dy = 20.0 * e.Y / 9.8;

            witchWatch.ShadowX = -dx;
            witchWatch.ShadowY = -dy;
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
                var time = GetCurrentTime();
                witchWatch.Time = time.UtcTimestamp + TimeSpan.FromMilliseconds(time.Millisecond);
                witchWatch.MoveHands(900).ContinueWith((Action<System.Threading.Tasks.Task<bool>>)((System.Threading.Tasks.Task<bool> t) => {
                    witchWatch.Run();
                    this.gravity.Start();
                }));
                initialized = true;
            }
            else
            {
                witchWatch.Run();
                gravity.Start();
            }

            base.OnResume();
        }

        protected override void OnPause()
        {
            base.OnPause();
            witchWatch.StopHands();
            gravity.Stop();
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
