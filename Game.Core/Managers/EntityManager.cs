using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common.Extensions;
using Game.Core.Entities;
using Game.Core.Events.Entity;
using Game.Core.Interfaces;
using log4net;

namespace Game.Core.Managers
{
  public class EntityManager
    : IEntityManager
  {
    private static readonly ILog Log = LogManager.GetLogger(
      MethodBase.GetCurrentMethod().DeclaringType);

    private readonly IEventManager m_eventManager;

    private readonly Dictionary<int, Entity> m_entities = 
      new Dictionary<int, Entity>(); 
    private readonly List<Entity> m_updateEntities = new List<Entity>(); 

    /// <summary>
    ///   Create the manager.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    ///   eventManager is null.
    /// </exception>
    public EntityManager(IEventManager eventManager)
    {
      if (eventManager == null) throw new ArgumentNullException("eventManager");

      m_eventManager = eventManager;
      CanPause = true;
    }

    #region IManager

    public bool CanPause { get; private set; }

    public bool Paused { get; set; }

    public bool Initialize()
    {
      Log.Verbose("EntityManager Initializing");

      return true;
    }

    public bool PostInitialize()
    {
      Log.Verbose("EntityManager Post-Initializing");

      return true;
    }

    public void Update(float deltaTime)
    {
      if (Paused)
      {
        return;
      }

      foreach (var entity in m_updateEntities)
      {
        entity.Update(deltaTime);
      }
    }

    public void Shutdown()
    {
      Log.Verbose("EntityManager Shutting Down");

      foreach (var entity in Entities)
      {
        entity.Destroy();
      }

      m_entities.Clear();
      m_updateEntities.Clear();
    }

    #endregion
    #region IEntityManager

    public IReadOnlyCollection<Entity> Entities
    {
      get { return m_entities.Values.ToList(); }
    }

    public Entity GetEntity(int id)
    {
      Entity result;
      return TryGetEntity(id, out result) ? result : null;
    }

    public bool TryGetEntity(int id, out Entity entity)
    {
      return m_entities.TryGetValue(id, out entity);
    }

    public void AddEntity(Entity entity)
    {
      if (entity == null) throw new ArgumentNullException("entity");
      if (!entity.IsInitialized)
        throw new InvalidOperationException("entity is not initialized");
      if (m_entities.ContainsKey(entity.Id)) 
        throw new InvalidOperationException(string.Format(
          "Entity id {0} already exists", entity.Id));

      entity.Activated += HandleEntityActivated;
      entity.DeActivated += HandleEntityDeactivated;
      entity.Destroyed += HandleEntityDestroyed;

      if (entity.IsActive && entity.NeedsUpdate)
      {
        m_updateEntities.Add(entity);
      }

      m_entities.Add(entity.Id, entity);
      m_eventManager.QueueEvent(new EntityAddedEvent { EntityId = entity.Id });
      Log.DebugFmt("Added entity {0}", entity.Name);
    }

    public void ActivateEntity(int id)
    {
      Entity entity;
      if (TryGetEntity(id, out entity))
      {
        entity.Activate();
      }
    }

    public void DeActivateEntity(int id)
    {
      Entity entity;
      if (TryGetEntity(id, out entity))
      {
        entity.Deactivate();
      }
    }

    public void DestroyEntity(int id)
    {
      Entity entity;
      if (TryGetEntity(id, out entity))
      {
        entity.Destroy();
      }
    }

    #endregion
    #region Event Handlers

    private void HandleEntityActivated(object sender, EventArgs e)
    {
      var entity = (Entity) sender;

      if (entity.NeedsUpdate)
      {
        m_updateEntities.Add(entity);
      }
    }

    private void HandleEntityDeactivated(object sender, EventArgs e)
    {
      var entity = (Entity)sender;

      m_updateEntities.Remove(entity);
    }

    private void HandleEntityDestroyed(object sender, EventArgs e)
    {
      var entity = (Entity)sender;

      entity.Activated -= HandleEntityActivated;
      entity.DeActivated -= HandleEntityDeactivated;
      entity.Destroyed -= HandleEntityDestroyed;

      m_entities.Remove(entity.Id);
      m_updateEntities.Remove(entity);
    }

    #endregion
  }
}
