using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Common.Extensions;
using Common.Game.Components;
using log4net;

namespace Common.Game
{
  /// <summary>
  ///   An entity is a container for one or more components.  The entity 
  ///   manages the state of all its components, and the components provide
  ///   all the functionality for the entity.
  /// </summary>
  public class Entity
    : EntityLifeCycleBase
  {
    private static readonly ILog Log = LogManager.GetLogger(
      MethodBase.GetCurrentMethod().DeclaringType);

    private readonly Dictionary<Type, ComponentBase> m_components = 
      new Dictionary<Type, ComponentBase>(); 
    private readonly List<ComponentBase> m_updateComponents = 
      new List<ComponentBase>();

    public Entity(int id, string name = "")
    {
      Id = id;
      Name = string.Format("Entity {0}{1}", id,
        string.IsNullOrEmpty(name)
          ? string.Empty
          : " " + name);
    }

    /// <summary>
    ///   The unique identifier for the entity.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    ///   The name of the entity.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    ///   If true, the entity requires per frame updates when Active.
    /// </summary>
    public bool NeedsUpdate { get; private set; }

    // TODO: TransformComponent

    /// <summary>
    ///   Performs a frame update when then entity is Active and NeedsUpdate 
    ///   is true.
    /// 
    ///   The base class function must always be called if overridden.
    /// </summary>
    /// <param name="deltaTime">
    ///   Time since the last update.
    /// </param>
    public virtual void Update(double deltaTime)
    {
      Debug.Assert(IsActive);
      Debug.Assert(NeedsUpdate);

      foreach (var component in m_updateComponents)
      {
        component.Update(deltaTime);
      }
    }

    #region Component Accessors

    /// <summary>
    ///   Adds a component to the entity.  Components must be added before
    ///   initialization.
    /// </summary>
    /// <param name="component"></param>
    /// <exception cref="ArgumentNullException">
    ///   component is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///   The entity already has a component with the same type.
    /// </exception>
    public void AddComponent(ComponentBase component)
    {
      if (component == null) throw new ArgumentNullException("component");
      if (m_components.ContainsKey(component.GetType()))
        throw new InvalidOperationException(
          "Duplicate component " + component.GetType().Name);

      Debug.Assert(!IsInitialized);
      Debug.Assert(component.Parent.Id == Id);

      if (component.NeedsUpdate)
      {
        m_updateComponents.Add(component);
        NeedsUpdate = true;
      }

      m_components.Add(component.GetType(), component);
      Log.VerboseFmt("{0} added {1}", Name, component.GetType().Name);
    }

    /// <summary>
    ///   Returns true if the entity has components that can be cast to type T.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public bool HasComponent<T>()
      where T : ComponentBase
    {
      var type = typeof (T);
      return m_components.ContainsKey(type);
    }

    /// <summary>
    ///   Attempts to get a component of type T from the entity.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="component">
    ///   If the method returns true, is set to the component retrieved.  
    ///   Otherwise is null.
    /// </param>
    /// <returns>
    ///   True if the component was found.
    /// </returns>
    public bool TryGetComponent<T>(out T component)
      where T : ComponentBase
    {
      component = GetComponent<T>();
      return component != null;
    }

    /// <summary>
    ///   Gets a component of type T from the entity.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>
    ///   The component if found, otherwise null.
    /// </returns>
    public T GetComponent<T>()
      where T : ComponentBase
    {
      var type = typeof(T);
      ComponentBase component;
      if (m_components.TryGetValue(type, out component))
      {
        return (T) component;
      }

      Log.DebugFmt("{0} does not have requested component {1}",
        Name, type.Name);
      return null;
    }

    /// <summary>
    ///   Returns all components in the entity that can be cast to type T.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public IReadOnlyList<T> GetComponentsByBase<T>()
      where T : ComponentBase
    {
      return m_components.Values.OfType<T>().ToList();
    }

    #endregion
    #region EntityLifeCycleBase

    protected override bool DoInitialize()
    {
      // TODO: get transform component

      foreach (var component in m_components.Values)
      {
        if (component.Initialize())
        {
          if (component.NeedsUpdate)
          {
            m_updateComponents.Add(component);
          }
          continue;
        }

        Log.ErrorFmt("Failed to initialize {0} in {1}",
          component.GetType().Name, Name);
        Dispose();
        return false;
      }

      Log.VerboseFmt("{0} initialized {1} components",
        Name, m_components.Count);
      return true;
    }

    protected override void DoActivate()
    {
      foreach (var component in m_components.Values)
      {
        component.Activate();
      }

      Log.VerboseFmt("{0} activated", Name);
    }

    protected override void DoDeactivate()
    {
      foreach (var component in m_components.Values)
      {
        component.Deactivate();
      }

      Log.VerboseFmt("{0} deactivated", Name);
    }

    protected override void DoDestroy()
    {
      foreach (var component in m_components.Values)
      {
        component.Destroy();
      }

      Log.VerboseFmt("{0} destroyed", Name);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        foreach (var component in m_components.Values)
        {
          component.Dispose();
        }
      }

      m_components.Clear();
      m_updateComponents.Clear();
    }
    
    #endregion
  }
}
