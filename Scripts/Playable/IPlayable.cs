using UnityEngine;

namespace Assets.Scripts
{
    public interface IPlayable
    {
        MonoBehaviour Parent { get; set; }
        float? EndTime { get; set; }
        float? PercentageCompleted { get; set; }
        float? TimeRemaining { get; set; }
        float? TimeSpanned { get; set; }
        float? TimeSpanSeconds { get; }

        bool IsCancelled { get; }

        event MilestoneEventHandler TimeSpanChanged;
        event MilestoneEventHandler Created;
        event MilestoneEventHandler Beginning;
        event MilestoneEventHandler Ended;
        event MilestoneEventHandler Cancelled;
        event MilestoneEventHandler Undone;


        void CancelImmediately();
        void Create(float timeSpanInSeconds);
        void FinishImmediately();
        void SetTimeSpan(float? timeSpanSeconds);
        void Start();
        void UndoImmediately();
    }
}