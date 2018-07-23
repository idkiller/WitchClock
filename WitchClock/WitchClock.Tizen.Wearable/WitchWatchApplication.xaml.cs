using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WitchClock
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WitchWatchApplication : Application
    {
        bool moving;
        public WitchWatchApplication()
        {
            InitializeComponent();
        }

        public DateTime Time { get; set; }

        public double ShadowX { get; set; }
        public double ShadowY { get; set; }

        public void Run()
        {
            Stopwatch stopwatch = new Stopwatch();
            DateTime time = default(DateTime);
            DateTime now = time;
            Func<bool> callback = () =>
            {
                if (time != Time)
                {
                    time = Time;
                    now = time;
                    stopwatch.Restart();
                }
                now = time + TimeSpan.FromMilliseconds(stopwatch.ElapsedMilliseconds);

                double hour = HourRotation(now);
                double min = MinuteRotation(now);
                double sec = SecondRotation(now);

                moonstick.Rotation = hour;
                witch.Rotation = min;
                witchShadow.Rotation = min;
                star.Rotation = sec;
                starShadow.Rotation = sec;

                bgShadow.TranslationX = ShadowX;
                witchShadow.TranslationX = ShadowX;
                starShadow.TranslationX = ShadowX;
                bgShadow.TranslationY = ShadowY;
                witchShadow.TranslationY = ShadowY;
                starShadow.TranslationY = ShadowY;

                return moving;
            };
            Device.BeginInvokeOnMainThread(() =>
            {
                moving = true;
                stopwatch.Start();
                if (callback())
                    Device.StartTimer(TimeSpan.FromMilliseconds(16), callback);
            });
        }

        public void StopHands() => moving = false;

        public Task<bool> MoveHands(uint length)
        {
            double hour = HourRotation(Time);
            double min = MinuteRotation(Time);
            double sec = SecondRotation(Time);

            var tsc = new TaskCompletionSource<bool>();

            var taskMoonstick = moonstick.RotateTo(hour, length);
            var taskWitch = witch.RotateTo(min, length);
            var taskWitchShadow = witchShadow.RotateTo(min, length);
            var taskStar = star.RotateTo(sec, length);
            var taskStarShadow = starShadow.RotateTo(sec, length);

            Task.WhenAll(taskMoonstick, taskWitch, taskWitchShadow, taskStar, taskStarShadow)
                .ContinueWith(t => tsc.SetResult(t.Result.All(x => x)));

            return tsc.Task;
        }

        double HourRotation(DateTime t) => (t.Hour >= 12 ? t.Hour - 12 : t.Hour) * 30 + t.Minute / 2.0 + t.Second / 12.0 + t.Millisecond / 120000.0;
        double MinuteRotation(DateTime t) => t.Minute * 6 + t.Second / 10.0 + t.Millisecond / 10000.0;
        double SecondRotation(DateTime t) => t.Second * 6 + t.Millisecond / 1000.0 * 6.0;
    }
}