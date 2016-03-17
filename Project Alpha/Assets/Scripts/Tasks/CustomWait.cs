using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace BehaviorDesigner.Runtime.Tasks
{
    [System.Serializable]
    public class SharedFloatList : SharedVariable<List<float>>
    {
        public static implicit operator SharedFloatList(List<float> value) { return new SharedFloatList { Value = value }; }
    }

    public class CustomWait : Action
    {
        [Tooltip("Should the wait be randomized?")]
        public SharedBool randomWait = false;
        [Tooltip("The minimum wait time if random wait is enabled")]
        public SharedFloat randomWaitMin = 1;
        [Tooltip("The maximum wait time if random wait is enabled")]
        public SharedFloat randomWaitMax = 1;

        public SharedFloatList waitTimeList;
        public SharedInt WaypointCounter;
        // The time to wait
        private float waitDuration;
        // The time that the task started to wait.
        private float startTime;
        // Remember the time that the task is paused so the time paused doesn't contribute to the wait time.
        private float pauseTime;

        private int IntwaypointCounter;
        public override void OnStart()
        {
            IntwaypointCounter = WaypointCounter.Value - 1;
            IntwaypointCounter = IntwaypointCounter % waitTimeList.Value.Count;
            if (IntwaypointCounter < 0)
            {
                IntwaypointCounter += waitTimeList.Value.Count;
            }
            // Remember the start time.
            startTime = Time.time;
            if (randomWait.Value)
            {
                waitDuration = Random.Range(randomWaitMin.Value, randomWaitMax.Value);
            }
            else
            {
                waitDuration = waitTimeList.Value[IntwaypointCounter];
            }
        }

        public override TaskStatus OnUpdate()
        {
            // The task is done waiting if the time waitDuration has elapsed since the task was started.
            if (startTime + waitDuration < Time.time)
            {
                return TaskStatus.Success;
            }
            // Otherwise we are still waiting.
            return TaskStatus.Running;
        }

        public override void OnPause(bool paused)
        {
            if (paused)
            {
                // Remember the time that the behavior was paused.
                pauseTime = Time.time;
            }
            else
            {
                // Add the difference between Time.time and pauseTime to figure out a new start time.
                startTime += (Time.time - pauseTime);
            }
        }

        public override void OnReset()
        {
            // Reset the public properties back to their original values
            randomWait = false;
            randomWaitMin = 1;
            randomWaitMax = 1;
        }
    }
}