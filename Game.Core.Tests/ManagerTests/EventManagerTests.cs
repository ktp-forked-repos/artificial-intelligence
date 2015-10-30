using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Game.Core.Events;
using Game.Core.Managers;
using NUnit.Framework;

namespace Game.Core.Tests.ManagerTests
{
  [TestFixture]
  public class EventManagerTests
  {
    private class EventStub
      : EventBase
    {
    }

    private class EventStub2
      : EventBase
    {
    }

    [Test]
    public void Initialize_Succeeds()
    {
      var em = new EventManager();

      var result = em.Initialize();

      Assert.IsTrue(result);
    }

    [Test]
    public void PostInitialize_Succeeds()
    {
      var em = new EventManager();
      em.Initialize();

      var result = em.PostInitialize();

      Assert.IsTrue(result);
    }

    [Test]
    public void Update_DoesNothingWithEmptyQueue()
    {
      var em = new EventManager();
      em.Initialize();
      em.PostInitialize();
      var listenerFired = false;
      Action<EventBase> listener = e => listenerFired = true;
      em.AddListener<EventStub>(listener);
      em.AddListener<EventStub2>(listener);

      em.Update(1f);

      Assert.IsFalse(listenerFired);
    }

    [Test]
    public void Update_DiscardsEventsWithNoListener()
    {
      var em = new EventManager();
      em.Initialize();
      em.PostInitialize();
      em.QueueEvent(new EventStub());
      em.QueueEvent(new EventStub2());

      em.Update(1f);

      Assert.IsEmpty(em.PendingEvents);
    }

    [Test]
    public void Update_FiresEvents()
    {
      var em = new EventManager();
      em.Initialize();
      em.PostInitialize();
      var listenerCalled = false;
      Action<EventBase> listener = e => listenerCalled = true;
      em.QueueEvent(new EventStub());
      em.AddListener<EventStub>(listener);

      em.Update(1f);

      Assert.IsTrue(listenerCalled);
      Assert.IsEmpty(em.PendingEvents);
    }

    [Test]
    public void Update_DefersEventsWhenTimeExpires()
    {
      var em = new EventManager();
      em.Initialize();
      em.PostInitialize();
      var listenerCalled = false;
      Action<EventBase> listener = e => listenerCalled = true;
      em.QueueEvent(new EventStub());
      em.AddListener<EventStub>(listener);

      em.Update(1f);

      Assert.IsFalse(listenerCalled);
      Assert.AreEqual(1, em.PendingEvents.Count);
    }

    [Test]
    public void Update_MaintainsOrderOfDeferredEvents()
    {
      var em = new EventManager();
      em.Initialize();
      em.PostInitialize();
      DateTime? listener1Time = null;
      Action<EventBase> listener1 = e =>
      {
        listener1Time = DateTime.Now;
        Thread.Sleep(10);
      };
      DateTime? listener2Time = null;
      Action<EventBase> listener2 = e => listener2Time = DateTime.Now;
      em.QueueEvent(new EventStub());
      em.QueueEvent(new EventStub2());
      em.AddListener<EventStub>(listener1);
      em.AddListener<EventStub2>(listener2);

      em.Update(1f); // defer
      em.Update(1f); // fire

      Assert.IsEmpty(em.PendingEvents);
      Assert.IsNotNull(listener1Time);
      Assert.IsNotNull(listener2Time);
      Assert.Less(listener1Time, listener2Time);
    }

    [Test]
    public void Shutdown_ClearsEvents()
    {
      var em = new EventManager();
      em.Initialize();
      em.PostInitialize();
      em.QueueEvent(new EventStub());

      em.Shutdown();

      Assert.IsEmpty(em.PendingEvents);
    }

    [Test]
    public void Shutdown_ClearsListeners()
    {
      var em = new EventManager();
      em.Initialize();
      em.PostInitialize();
      var listenerCalled = false;
      Action<EventBase> listener = e => listenerCalled = true;
      em.AddListener<EventStub>(listener);

      em.Shutdown();
      em.TriggerEvent(new EventStub());

      Assert.IsFalse(listenerCalled);
    }

    [Test]
    public void AddListener_HandlesNull()
    {
      var em = new EventManager();
      em.Initialize();
      em.PostInitialize();

      TestDelegate func = () => em.AddListener<EventStub>(null);

      Assert.Throws<ArgumentNullException>(func);
      Assert.AreEqual(0, em.GetListenerCount<EventStub>());
    }

    [Test]
    public void AddListener_AddsListeners()
    {
      var em = new EventManager();
      em.Initialize();
      em.PostInitialize();
      Action<EventBase> listener = e => {};

      em.AddListener<EventStub>(listener);
      em.AddListener<EventStub2>(listener);

      Assert.AreEqual(1, em.GetListenerCount<EventStub>());
      Assert.AreEqual(1, em.GetListenerCount<EventStub2>());
    }

    [Test]
    public void AddListener_HandlesMultipleAdds()
    {
      var em = new EventManager();
      em.Initialize();
      em.PostInitialize();
      Action<EventBase> listener1 = e => { };
      Action<EventBase> listener2 = e => { };

      em.AddListener<EventStub>(listener1);
      em.AddListener<EventStub>(listener2);

      Assert.AreEqual(2, em.GetListenerCount<EventStub>());
    }

    [Test]
    public void RemoveListener_HandlesNull()
    {
      var em = new EventManager();
      em.Initialize();
      em.PostInitialize();

      TestDelegate func = () => em.RemoveListener<EventStub>(null);

      Assert.Throws<ArgumentNullException>(func);
    }

    [Test]
    public void RemoveListener_HandlesNonExistingListener()
    {
      var em = new EventManager();
      em.Initialize();
      em.PostInitialize();
      Action<EventBase> listener = e => { };

      em.RemoveListener<EventStub>(listener);

      Assert.AreEqual(0, em.GetListenerCount<EventStub>());
    }

    [Test]
    public void RemoveListener_RemovesListeners()
    {
      var em = new EventManager();
      em.Initialize();
      em.PostInitialize();
      Action<EventBase> listener = e => { };
      em.AddListener<EventStub>(listener);

      em.RemoveListener<EventStub>(listener);

      Assert.AreEqual(0, em.GetListenerCount<EventStub>());
    }

    [Test]
    public void TriggerEvent_HandlesNull()
    {
      var em = new EventManager();
      em.Initialize();
      em.PostInitialize();

      TestDelegate func = () => em.TriggerEvent(null);

      Assert.Throws<ArgumentNullException>(func);
    }

    [Test]
    public void TriggerEvent_FiresEvent()
    {
      var em = new EventManager();
      em.Initialize();
      em.PostInitialize();
      var listenerFired = false;
      Action<EventBase> listener = e => listenerFired = true;
      em.AddListener<EventStub>(listener);

      em.TriggerEvent(new EventStub());

      Assert.IsTrue(listenerFired);
      Assert.IsEmpty(em.PendingEvents);
    }

    [Test]
    public void QueueEvent_HandlesNull()
    {
      var em = new EventManager();
      em.Initialize();
      em.PostInitialize();

      TestDelegate func = () => em.QueueEvent(null);

      Assert.Throws<ArgumentNullException>(func);
    }

    [Test]
    public void QueueEvent_QueuesEvents()
    {
      var em = new EventManager();
      em.Initialize();
      em.PostInitialize();

      em.QueueEvent(new EventStub());

      Assert.AreEqual(1, em.PendingEvents.Count);
    }

    [Test]
    public void AbortFirstEvent_DoesNothingWhenQueueIsEmpty()
    {
      var em = new EventManager();
      em.Initialize();
      em.PostInitialize();

      var result = em.AbortFirstEvent<EventStub>();

      Assert.IsFalse(result);
    }

    [Test]
    public void AbortFirstEvent_AbortsFirstOfMultipleEvents()
    {
      var em = new EventManager();
      em.Initialize();
      em.PostInitialize();
      em.QueueEvent(new EventStub());
      var @event = new EventStub();
      em.QueueEvent(@event);

      var result = em.AbortFirstEvent<EventStub>();

      Assert.IsTrue(result);
      Assert.AreEqual(1, em.PendingEvents.Count);
      Assert.AreSame(@event, em.PendingEvents.First());
    }

    [Test]
    public void AbortFirstEvent_AbortsFirstOfMultipleTypes()
    {
      var em = new EventManager();
      em.Initialize();
      em.PostInitialize();
      var @event = new EventStub();
      em.QueueEvent(@event);
      em.QueueEvent(new EventStub2());

      var result = em.AbortFirstEvent<EventStub2>();

      Assert.IsTrue(result);
      Assert.AreEqual(1, em.PendingEvents.Count);
      Assert.AreSame(@event, em.PendingEvents.First());
    }

    [Test]
    public void AbortEvents_DoesNothingWhenQueueIsEmpty()
    {
      var em = new EventManager();
      em.Initialize();
      em.PostInitialize();

      var result = em.AbortEvents<EventStub>();

      Assert.AreEqual(0, result);
      Assert.IsEmpty(em.PendingEvents);
    }

    [Test]
    public void AbortEvents_ClearsMultipleEvents()
    {
      var em = new EventManager();
      em.Initialize();
      em.PostInitialize();
      em.QueueEvent(new EventStub());
      em.QueueEvent(new EventStub());

      var result = em.AbortEvents<EventStub>();

      Assert.AreEqual(2, result);
      Assert.IsEmpty(em.PendingEvents);
    }

    [Test]
    public void AbortEvents_ClearsEventsOfCorrectType()
    {
      var em = new EventManager();
      em.Initialize();
      em.PostInitialize();
      em.QueueEvent(new EventStub());
      var @event = new EventStub2();
      em.QueueEvent(@event);
      em.QueueEvent(new EventStub());

      var result = em.AbortEvents<EventStub>();

      Assert.AreEqual(2, result);
      Assert.AreEqual(1, em.PendingEvents.Count);
      Assert.AreSame(@event, em.PendingEvents.First());
    }

    [Test]
    public void AbortAllEvents_DoesNothingWhenQueueIsEmpty()
    {
      var em = new EventManager();
      em.Initialize();
      em.PostInitialize();

      var result = em.AbortAllEvents();

      Assert.AreEqual(0, result);
      Assert.IsEmpty(em.PendingEvents);
    }

    [Test]
    public void AbortAllEvents_ClearsQueue()
    {
      var em = new EventManager();
      em.Initialize();
      em.PostInitialize();
      em.QueueEvent(new EventStub());
      em.QueueEvent(new EventStub2());
      em.QueueEvent(new EventStub());

      var result = em.AbortAllEvents();

      Assert.AreEqual(3, result);
      Assert.IsEmpty(em.PendingEvents);
    }
  }
}
