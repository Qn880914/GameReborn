using FrameWork.Manager;

namespace FrameWork.Event
{
    public delegate void EventDelegate<T>(T e) where T : GameEvent;

    public class GameEvent
    {
        public bool log { get; set; }

        public GameEvent()
        {
            log = false;
        }

        public void Fire()
        {
            EventManager.instance.DispatchEvent(this);
        }

        public virtual void LogEvent()
        {
            if(log)
            {
                UnityEngine.Debug.Log("Event Dispathced : " + this.GetType().Name);
            }
        }
    }
}
