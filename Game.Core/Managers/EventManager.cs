using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Game.Core.Events;
using Game.Core.Managers.Interfaces;
using NLog;

namespace Game.Core.Managers
{
  /// <summary>
  ///   The general event manager implementation.
  /// </summary>
  public class EventManager
    : IEventManager
  {
    private static readonly Logger Log = LogManager.GetCurrentClassLogger();

    private const int InitialQueueSize = 10;

    
    private readonly Stopwatch m_eventDispatchTimer = new Stopwatch();
    private readonly Dictionary<Type, Action<EventBase>> m_listeners =
      new Dictionary<Type, Action<EventBase>>();
    private readonly List<EventBase>[] m_queue = {
      new List<EventBase>(InitialQueueSize),
      new List<EventBase>(InitialQueueSize)
    };
    private int m_readIndex = 0;
    private int m_writeIndex = 1;

    private List<EventBase> ReadQueue
    {
      get { return m_queue[m_readIndex]; }
    }

    private List<EventBase> WriteQueue
    {
      get { return m_queue[m_writeIndex]; }
    }

    public EventManager()
    {
      CanPause = false;
    }

    #region IManager

    public bool CanPause { get; private set; }

    public bool Paused { get; set; }

    public bool Initialize()
    {
      Log.Trace("EventManager Initializing");

      return true;
    }

    public bool PostInitialize()
    {
      Log.Trace("EventManager Post-Initializing");

      return true;
    }

    public void Update(float deltaTime)
    {
      // queues are swapped so that any new events added in response to an 
      // event firing are processed in the next frame
      m_readIndex = (m_readIndex + 1) & 1;
      m_writeIndex = (m_writeIndex + 1) & 1;

      if (ReadQueue.Count == 0)
      {
        return;
      }

      var count = 0;
      m_eventDispatchTimer.Restart();
      while (ReadQueue.Count > 0)
      {
        var evt = ReadQueue.First();
        ReadQueue.RemoveAt(0);
        TriggerEvent(evt);
        count++;
      }

      Log.Trace("Processed {0} events in {1:F4}s", count,
        m_eventDispatchTimer.Elapsed.TotalSeconds);
    }

    public void Shutdown()
    {
      Log.Trace("EventManager Shutting Down");

      m_listeners.Clear();
      ReadQueue.Clear();
      WriteQueue.Clear();
    }

    #endregion
    #region IEventManager

    public IReadOnlyCollection<EventBase> PendingEvents
    {
      get { return WriteQueue; }
    }

    public int GetListenerCount<T>()
      where T : EventBase
    {
      var type = typeof (T);
      Action<EventBase> listeners;
      if (m_listeners.TryGetValue(type, out listeners) && listeners != null)
      {
        return listeners.GetInvocationList().Count();
      }

      return 0;
    }

    public void AddListener<T>(Action<EventBase> listener) 
      where T : EventBase
    {
      if (listener == null) throw new ArgumentNullException("listener");

      var type = typeof(T);
      if (!m_listeners.ContainsKey(type))
      {
        m_listeners[type] = listener;
      }
      else
      {
        m_listeners[type] += listener;
      }

      Log.Trace("{0} listener added", type.Name);
    }

    public void RemoveListener<T>(Action<EventBase> listener) 
      where T : EventBase
    {
      if (listener == null) throw new ArgumentNullException("listener");

      var type = typeof(T);
      if (!m_listeners.ContainsKey(type))
      {
        Log.Warn("Tried to remove {0} listener but none exist", type.Name);
        return;
      }

      // ReSharper disable once DelegateSubtraction
      m_listeners[type] -= listener;
      Log.Trace("Removed {0} listener", type.Name);
    }

    public void TriggerEvent(EventBase evt)
    {
      if (evt == null) throw new ArgumentNullException("evt");

      var type = evt.GetType();
      Action<EventBase> listener;
      if (!m_listeners.TryGetValue(type, out listener) || listener == null)
      {
        Log.Debug("Discarding {0}, no listeners", type.Name);
        return;
      }

      Log.Trace("Dispatching {0}", type.Name);
      listener(evt);
    }

    public void QueueEvent(EventBase evt)
    {
      if (evt == null) throw new ArgumentNullException("evt");

      WriteQueue.Add(evt);
      Log.Trace("Queued {0}", evt.GetType().Name);
    }

    public bool AbortFirstEvent<T>() 
      where T : EventBase
    {
      var type = typeof(T);
      var toRemove = WriteQueue.FirstOrDefault(e => e.GetType() == type);
      if (toRemove == null)
      {
        return false;
      }

      WriteQueue.Remove(toRemove);
      Log.Trace("Aborted {0}", type.Name);
      return true;
    }

    public int AbortEvents<T>() 
      where T : EventBase
    {
      var type = typeof(T);
      var count = WriteQueue.RemoveAll(e => e.GetType() == type);
      Log.Trace("Aborted {0} events of type {1}", count, type.Name);
      return count;
    }

    public int AbortAllEvents()
    {
      var count = WriteQueue.Count;
      WriteQueue.Clear();
      Log.Trace("Cleared {0} events from queue", count);
      return count;
    }

    #endregion
  }
}
