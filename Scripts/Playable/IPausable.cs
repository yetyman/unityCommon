namespace Assets.Scripts
{
    public interface IPausable : IPlayable
    {
        MilestoneEventHandler Pausing { get; set; }
        MilestoneEventHandler Slowing { get; set; }
        MilestoneEventHandler Resuming { get; set; }

        bool IsPaused { get; set; }

        void Create(float timeSpanInSeconds, bool playOnInitialization);
        bool Pause(bool global = false);
        bool Resume(bool global = false);
        void Slow(float scale);
    }
}