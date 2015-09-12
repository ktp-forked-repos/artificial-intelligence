namespace Common.Game.Interfaces
{
  /// <summary>
  ///   The base interface for all game managers.
  /// </summary>
  public interface IManager
  {
    /// <summary>
    ///   Indicates if the manager can be paused or not.
    /// </summary>
    bool CanPause { get; }

    /// <summary>
    ///   If <see cref="CanPause"/> is true, the manager does not process 
    ///   updates when set to true;
    /// </summary>
    bool Paused { get; set; }

    /// <summary>
    ///   Performs the main initialization of the manager.
    /// </summary>
    /// <returns>
    ///   Success or failure of the initialization.
    /// </returns>
    bool Initialize();

    /// <summary>
    ///   Performs any initialization that couldn't be done in 
    ///   <see cref="Initialize"/>.  Generally actions that require interaction 
    ///   with other managers that can't be completed until they have done their 
    ///   initialization.
    /// </summary>
    /// <returns>
    ///   Success or failure of the initialization.
    /// </returns>
    bool PostInitialize();

    /// <summary>
    ///   Does a frame update of the manager.
    /// </summary>
    /// <param name="deltaTime">
    ///   Elapsed game time, in seconds.
    /// </param>
    /// <param name="maxTime">
    ///   The max time the manager can spend on its update, in seconds.
    /// </param>
    void Update(double deltaTime, double maxTime);

    /// <summary>
    ///   Shuts down the manager without releasing resources.
    /// </summary>
    void Shutdown();
  }
}
