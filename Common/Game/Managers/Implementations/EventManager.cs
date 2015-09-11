using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Common.Extensions;
using Common.Game.Events;
using Common.Game.Managers.Interfaces;
using log4net;

namespace Common.Game.Managers.Implementations
{
  public class EventManager
    : IEventManager
  {
    private const int InitialQueueSize = 10;

    private static readonly ILog Log = LogManager.GetLogger(
      MethodBase.GetCurrentMethod().DeclaringType);

    private readonly Stopwatch m_eventDispatchTimer = new Stopwatch();
    private readonly Dictionary<Type, Action<Event>> m_listeners =
      new Dictionary<Type, Action<Event>>();
    private readonly List<Event>[] m_queue = {
      new List<Event>(InitialQueueSize),
      new List<Event>(InitialQueueSize)
    };
    private int m_readIndex = 0;
    private int m_writeIndex = 1;

    private List<Event> ReadQueue
    {
      get { return m_queue[m_readIndex]; }
    }

    private List<Event> WriteQueue
    {
      get { return m_queue[m_writeIndex]; }
    }

    #region IManager

    public bool Initialize()
    {
      return true;
    }

    public bool PostInitialize()
    {
      return true;
    }

    public void Update(float deltaTime, float maxTime)
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
        if (m_eventDispatchTimer.Elapsed.TotalSeconds >= maxTime)
        {
          Log.VerboseFmt("Processing aborted with {0} events in the queue",
            ReadQueue.Count);

          WriteQueue.InsertRange(0, ReadQueue);
          ReadQueue.Clear();
          break;
        }

        var evt = ReadQueue.First();
        ReadQueue.RemoveAt(0);
        TriggerEvent(evt);
        count++;
      }

      Log.VerboseFmt("Processed {0} events in {1:F4}s", count,
        m_eventDispatchTimer.Elapsed.TotalSeconds);
    }

    public void Shutdown()
    {
    }

    #endregion
    #region IEventManager

    public void AddListener<T>(Action<Event> listener) 
      where T : Event
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

      Log.VerboseFmt("{0} listener added", type.Name);
    }

    public void RemoveListener<T>(Action<Event> listener) 
      where T : Event
    {
      if (listener == null) throw new ArgumentNullException("listener");

      var type = typeof(T);
      if (!m_listeners.ContainsKey(type))
      {
        Log.WarnFmt("Tried to remove {0} listener but none exist", type.Name);
        return;
      }

      // ReSharper disable once DelegateSubtraction
      m_listeners[type] -= listener;
      Log.VerboseFmt("Removed {0} listener", type.Name);
    }

    public void TriggerEvent(Event evt)
    {
      if (evt == null) throw new ArgumentNullException("evt");

      var type = evt.GetType();
      Action<Event> listener;
      if (!m_listeners.TryGetValue(type, out listener) || listener == null)
      {
        Log.VerboseFmt("Discarding {0}, no listeners", type.Name);
        return;
      }

      Log.VerboseFmt("Dispatching {0}", type.Name);
      listener(evt);
    }

    public void QueueEvent(Event evt)
    {
      if (evt == null) throw new ArgumentNullException("evt");

      WriteQueue.Add(evt);
      Log.VerboseFmt("Queued {0}", evt.GetType().Name);
    }

    public bool AbortFirstEvent<T>() 
      where T : Event
    {
      var type = typeof(T);
      var toRemove = WriteQueue.First(e => e.GetType() == type);
      if (toRemove == null)
      {
        return false;
      }

      WriteQueue.Remove(toRemove);
      Log.VerboseFmt("Aborted {0}", type.Name);
      return true;
    }

    public int AbortEvents<T>() where T : Event
    {
      var type = typeof(T);
      var count = WriteQueue.RemoveAll(e => e.GetType() == type);
      Log.VerboseFmtIf(count > 0, "Aborted {0} events of type {1}", count, 
        type.Name);
      return count;
    }

    public int AbortAllEvents()
    {
      var count = WriteQueue.Count;
      WriteQueue.Clear();
      Log.VerboseFmtIf(count > 0, "Cleared {0} events from queue", count);
      return count;
    }

    #endregion
  }
}
