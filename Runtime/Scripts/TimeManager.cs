using System;
using System.Collections;
using Bakery.Saves;
using Holypastry.Bakery.Flow;
using UnityEngine;
namespace Bakery.TimeOfDay
{
    public class TimeManager : Service
    {
        [SerializeField] private int _multiplier = 100;

        [SerializeField] private int _startHour = 8;

        [SerializeField] private string _startDate = "07/05/1848";

        private DateTime _currentTime;

        private int _pausedMultiplier;

        private DateTime _lastHourTime;



        void OnDisable()
        {

            TimeServices.WaitUntilReady = () => new WaitUntil(() => true);

            TimeServices.SetTimeOfDay = delegate { };
            TimeServices.GetTimeOfDay = () => default;

            TimeServices.SetHour = delegate { };
            TimeServices.AddHours = delegate { };
            TimeServices.WaitForHours = (hours) => null;
            TimeServices.SetTimeFactor = delegate { };

            TimeServices.PauseTimeOfDay = delegate { };
            TimeServices.ResumeTimeOfDay = delegate { };

            TimeServices.IsTimeBetween = delegate { return false; };

        }
        void OnEnable()
        {

            TimeServices.WaitUntilReady = () => WaitUntilReady;

            TimeServices.SetTimeOfDay = (time) => SetTime(time);
            TimeServices.GetTimeOfDay = () => new(_currentTime.Ticks);

            TimeServices.SetHour = SetHour;
            TimeServices.AddHours = SkipAheadHours;
            TimeServices.WaitForHours = WaitForHours;
            TimeServices.SetTimeFactor = SetMultiplier;

            TimeServices.PauseTimeOfDay = Pause;
            TimeServices.ResumeTimeOfDay = Resume;

            TimeServices.IsTimeBetween = IsTimeBetween;

        }

        private void Resume()
        {
            SetMultiplier(_pausedMultiplier);
        }

        private void Pause()
        {
            _pausedMultiplier = _multiplier;
            SetMultiplier(0);
        }

        protected override IEnumerator Start()
        {
            yield return FlowServices.WaitUntilReady();
            var serialTime = SaveServices.Load<SerialTime>("TimeOfDay");

            if (serialTime != null)
                _currentTime = DateTime.Parse(serialTime.Time);
            else
                _currentTime = DateTime.Parse(_startDate) + TimeSpan.FromHours(_startHour);

            _lastHourTime = _currentTime;
            _isReady = true;
        }

        private void SetHour(int hour)
        {
            if (_currentTime.Hour == hour) return;

            SetTime(_currentTime.AddHours(hour - _currentTime.Hour));
        }

        private CustomYieldInstruction WaitForHours(int hours)
        {
            var targetTime = _currentTime.AddHours(hours);
            return new WaitUntil(() => _currentTime >= targetTime);
        }

        private bool IsTimeBetween(int fromHour, int fromMinute, int toHour, int toMinute)
        {
            var currentTime = _currentTime.TimeOfDay;
            var fromTime = new TimeSpan(fromHour, fromMinute, 0);
            var toTime = new TimeSpan(toHour, toMinute, 0);

            if (fromTime < toTime)
                return currentTime >= fromTime && currentTime <= toTime;
            else
                return currentTime >= fromTime || currentTime <= toTime;
        }

        private void SetMultiplier(int multiplier)
        {
            _multiplier = multiplier;
            TimeEvents.OnTimeFactorChanged(_multiplier);
        }

        private void SkipAheadHours(int hours) => SetTime(_currentTime.AddHours(hours));

        void Update()
        {
            if (!_isReady) return;
            UpdateTimeOfDay();
        }

        private void UpdateTimeOfDay() => SetTime(_currentTime.AddSeconds(Time.deltaTime * _multiplier));

        private void SetTime(DateTime dateTime)
        {
            var previousTime = new DateTime(_currentTime.Ticks);
            _currentTime = dateTime;

            var totalHours = (int)(_currentTime - _lastHourTime).TotalHours;
            if (totalHours >= 1)
            {
                TimeEvents.OnHourTicked(totalHours);
                _lastHourTime = _currentTime;
            }


            if (previousTime.Day != _currentTime.Day)
            {
                TimeEvents.OnDayChanged();
            }

            SaveServices.Save(new SerialTime(_currentTime));
            TimeEvents.OnTimeChanged(_currentTime);
        }

        private static TimeSpan CalculateTimeDifference(TimeSpan from, TimeSpan to)
        {
            if (from < to)
                return to - from + TimeSpan.FromHours(1);
            else
                return TimeSpan.FromHours(25) + to - from;

        }


    }
}
