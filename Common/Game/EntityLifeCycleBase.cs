using System;
using System.Diagnostics;

namespace Common.Game
{
  /// <summary>
  ///   The current state of the entity object in its lifetime.
  /// </summary>
  public enum EntityState
  {
    /// <summary>
    ///   Initial, inactive state.
    /// </summary>
    NotInitialized,
    /// <summary>
    ///   The initialized and actively running state.
    /// </summary>
    Active,
    /// <summary>
    ///   The initialized but inactive state.  Can be returned to the Active
    ///   state.
    /// </summary>
    Deactivated,
    /// <summary>
    ///   Destroyed state.
    /// </summary>
    Destroyed
  }

  /// <summary>
  ///   Entities and their components share a common life cycle.
  /// 
  ///   Initially the entity is NotInitialized.  After initialization it 
  ///   is set to Deactivated.  The entity may then be activated and deactivated
  ///   as necessary.  When the entity is no longer needed, it is Destroyed.
  /// </summary>
  public abstract class EntityLifeCycleBase
    : IDisposable
  {
    /// <summary>
    ///   Handles an entity's change of state.
    /// </summary>
    /// <param name="sender"></param>
    public delegate void StateChangeHandler(EntityLifeCycleBase sender);

    protected EntityLifeCycleBase()
    {
      State = EntityState.NotInitialized;
    }

    #region Events

    /// <summary>
    ///   Fires when the entity successfully initializes.
    /// </summary>
    public event StateChangeHandler Initialized;

    /// <summary>
    ///   Fires when the entity transitions to Activated.
    /// </summary>
    public event StateChangeHandler Activated;

    /// <summary>
    ///   Fires when the entity transitions to DeActivated.
    /// </summary>
    public event StateChangeHandler DeActivated;

    /// <summary>
    ///   Fires when the entity transitions to Destroyed.
    /// </summary>
    public event StateChangeHandler Destroyed;

    #endregion
    /// <summary>
    ///   Current state of the entity.
    /// </summary>
    public EntityState State { get; private set; }

    /// <summary>
    ///   Has the entity been initialized, but not disposed.
    /// </summary>
    public bool IsInitialized
    {
      get
      {
        return State == EntityState.Active ||
               State == EntityState.Deactivated;
      }
    }

    #region State Check Properties

    /// <summary>
    ///   Is the entity Active.
    /// </summary>
    public bool IsActive
    {
      get { return State == EntityState.Active; }
    }

    /// <summary>
    ///   Is the entity Deactivated.
    /// </summary>
    public bool IsDeactivated
    {
      get { return State == EntityState.Deactivated; }
    }

    /// <summary>
    ///   Has the entity been Destroyed.
    /// </summary>
    public bool IsDestroyed
    {
      get { return State == EntityState.Destroyed; }
    }

    #endregion
    #region Control Methods

    /// <summary>
    ///   Initialize the entity to a deactivated state.
    /// </summary>
    /// <returns>
    ///   Success or failure of initialization.
    /// </returns>
    public bool Initialize()
    {
      Debug.Assert(!IsInitialized);

      if (!DoInitialize())
      {
        return false;
      }

      State = EntityState.Deactivated;
      OnInitialized();
      return true;
    }

    /// <summary>
    ///   Move the entity from Deactivated -> Active.
    /// </summary>
    public void Activate()
    {
      Debug.Assert(IsInitialized);

      if (IsActive)
      {
        return;
      }

      State = EntityState.Active;
      DoActivate();
      OnActivated();
    }

    /// <summary>
    ///   Move the entity from Active -> Deactivated.
    /// </summary>
    public void Deactivate()
    {
      Debug.Assert(IsInitialized);

      if (IsDeactivated)
      {
        return;
      }

      State = EntityState.Deactivated;
      DoDeactivate();
      OnDeActivated();
    }

    /// <summary>
    ///   Destroys the entity.
    /// </summary>
    public void Destroy()
    {
      State = EntityState.Destroyed;
      DoDestroy();
      OnDestroyed();
    }

    #endregion

    /// <summary>
    ///   Performs the initialization action.
    /// </summary>
    /// <returns>
    ///   Success or failure of initialization.
    /// </returns>
    protected abstract bool DoInitialize();

    /// <summary>
    ///   Performs the activation action.
    /// </summary>
    protected abstract void DoActivate();

    /// <summary>
    ///   Performs the Deactivation action.
    /// </summary>
    protected abstract void DoDeactivate();

    /// <summary>
    ///   Performs the Destroy action.
    /// </summary>
    protected abstract void DoDestroy();

    #region Event Invokers

    private void OnInitialized()
    {
      if (Initialized != null)
      {
        Initialized(this);
      }
    }

    private void OnActivated()
    {
      if (Activated != null)
      {
        Activated(this);
      }
    }

    private void OnDeActivated()
    {
      if (DeActivated != null)
      {
        DeActivated(this);
      }
    }

    private void OnDestroyed()
    {
      if (Destroyed != null)
      {
        Destroyed(this);
      }
    }

    #endregion
    #region IDisposable

    /// <summary>
    ///   Destroys, then disposes of the entity.
    /// </summary>
    public void Dispose()
    {
      Destroy();
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    /// <summary>
    ///   Perform disposal action.
    /// </summary>
    /// <param name="disposing"></param>
    protected abstract void Dispose(bool disposing);

    ~EntityLifeCycleBase()
    {
      Dispose(false);
    }

    #endregion
  }
}
