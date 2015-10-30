using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Common.Extensions;
using Game.Core.Events.Input;
using Game.Core.Interfaces;
using log4net;
using Microsoft.Xna.Framework;
using SFML.System;
using SFML.Window;

namespace Game.Core.Managers
{
  /// <summary>
  ///   Handles translation of user inputs to game input events.
  /// </summary>
  /// <remarks>
  ///   No unit testing here because of the difficulty of mocking the SFML
  ///   Window, and this class really just does very simple translation and
  ///   forwarding of events.
  /// </remarks>
  [ExcludeFromCodeCoverage]
  public class InputManager
    : IManager
  {
    private static readonly ILog Log = LogManager.GetLogger(
      MethodBase.GetCurrentMethod().DeclaringType);

    public const Mouse.Button SelectButton = Mouse.Button.Left;
    public const Mouse.Button ViewDragButton = Mouse.Button.Right;
    public const Keyboard.Key PauseKey = Keyboard.Key.P;
    public const Keyboard.Key MoveUpKey = Keyboard.Key.Up;
    public const Keyboard.Key MoveDownKey = Keyboard.Key.Down;
    public const Keyboard.Key MoveLeftKey = Keyboard.Key.Left;
    public const Keyboard.Key MoveRightKey = Keyboard.Key.Right;

    private readonly Window m_window;
    private readonly IEventManager m_eventManager;

    private readonly Dictionary<Keyboard.Key, bool> m_keyStates = 
      new Dictionary<Keyboard.Key, bool>();
    private readonly Dictionary<Mouse.Button, bool> m_buttonStates = 
      new Dictionary<Mouse.Button, bool>();
    private Vector2i m_mousePos;

    /// <summary>
    ///   Create the manager.
    /// </summary>
    /// <param name="window"></param>
    /// <param name="eventManager"></param>
    /// <exception cref="ArgumentNullException">
    ///   window is null
    ///   -or-
    ///   eventManager is null
    /// </exception>
    public InputManager(Window window, IEventManager eventManager)
    {
      if (window == null) throw new ArgumentNullException("window");
      if (eventManager == null) throw new ArgumentNullException("eventManager");

      m_window = window;
      m_eventManager = eventManager;

      CanPause = true;
    }

    #region IManager

    public bool CanPause { get; private set; }

    public bool Paused { get; set; }

    public bool Initialize()
    {
      Log.Verbose("InputManager Initializing");

      var keys = Enum.GetValues(typeof (Keyboard.Key))
        .Cast<Keyboard.Key>();
      foreach (var key in keys)
      {
        m_keyStates[key] = false;
      }

      var buttons = Enum.GetValues(typeof (Mouse.Button))
        .Cast<Mouse.Button>();
      foreach (var button in buttons)
      {
        m_buttonStates[button] = false;
      }

      return true;
    }

    public bool PostInitialize()
    {
      Log.Verbose("InputManager Post-Initializing");

      m_window.KeyPressed += HandleKeyPressed;
      m_window.KeyReleased += HandleKeyReleased;
      m_window.MouseButtonPressed += HandleMouseButtonPressed;
      m_window.MouseButtonReleased += HandleMouseButtonReleased;
      m_window.MouseWheelMoved += HandleMouseWheelMoved;
      m_window.MouseMoved += HandleMouseMoved;

      return true;
    }
    
    public void Update(float deltaTime)
    {
      m_window.DispatchEvents();
    }

    public void Shutdown()
    {
      Log.Verbose("InputManager Shutting Down");

      m_window.KeyPressed -= HandleKeyPressed;
      m_window.KeyReleased -= HandleKeyReleased;
      m_window.MouseButtonPressed -= HandleMouseButtonPressed;
      m_window.MouseButtonReleased -= HandleMouseButtonReleased;
      m_window.MouseWheelMoved -= HandleMouseWheelMoved;
      m_window.MouseMoved -= HandleMouseMoved;
    }

    #endregion
    #region Event Handlers

    private void HandleKeyPressed(object sender, KeyEventArgs args)
    {
      // key press has been handled
      if (m_keyStates[args.Code])
      {
        return;
      }
      
      switch (args.Code)
      {
        case MoveUpKey:
          m_eventManager.QueueEvent(new MoveEvent
          {
            Action = MoveEvent.MoveAction.BeginUp
          });
          break;
        case MoveDownKey:
          m_eventManager.QueueEvent(new MoveEvent
          {
            Action = MoveEvent.MoveAction.BeginDown
          });
          break;
        case MoveLeftKey:
          m_eventManager.QueueEvent(new MoveEvent
          {
            Action = MoveEvent.MoveAction.BeginLeft
          });
          break;
        case MoveRightKey:
          m_eventManager.QueueEvent(new MoveEvent
          {
            Action = MoveEvent.MoveAction.BeginRight
          });
          break;
        case PauseKey:
          Paused = !Paused;
          m_eventManager.QueueEvent(new GamePausedEvent
          {
            IsPaused = Paused
            });
          break;
      }

      m_keyStates[args.Code] = true;
    }

    private void HandleKeyReleased(object sender, KeyEventArgs args)
    {
      // key release has been handled
      if (!m_keyStates[args.Code])
      {
        return;
      }
      
      switch (args.Code)
      {
        case MoveUpKey:
          m_eventManager.QueueEvent(new MoveEvent
          {
            Action = MoveEvent.MoveAction.EndUp
          });
          break;
        case MoveDownKey:
          m_eventManager.QueueEvent(new MoveEvent
          {
            Action = MoveEvent.MoveAction.EndDown
          });
          break;
        case MoveLeftKey:
          m_eventManager.QueueEvent(new MoveEvent
          {
            Action = MoveEvent.MoveAction.EndLeft
          });
          break;
        case MoveRightKey:
          m_eventManager.QueueEvent(new MoveEvent
          {
            Action = MoveEvent.MoveAction.EndRight
          });
          break;
        case PauseKey:
          // intentionally empty
          break;
      }

      m_keyStates[args.Code] = false;
    }
    
    private void HandleMouseButtonPressed(object sender, 
      MouseButtonEventArgs args)
    {
      // button press has been handled
      if (m_buttonStates[args.Button])
      {
        return;
      }

      switch (args.Button)
      {
        case SelectButton:
          m_eventManager.QueueEvent(new SelectEvent
          {
            Position = new Vector2i(args.X, args.Y)
          });
          break;
        case ViewDragButton:
          m_mousePos = new Vector2i(args.X, args.Y);
          break;
      }

      m_buttonStates[args.Button] = true;
    }

    private void HandleMouseButtonReleased(object sender, 
      MouseButtonEventArgs args)
    {
      m_buttonStates[args.Button] = false;
    }

    private void HandleMouseWheelMoved(object sender, MouseWheelEventArgs args)
    {
      var @event = new ViewZoomEvent {Delta = args.Delta};
      m_eventManager.QueueEvent(@event);
    }

    private void HandleMouseMoved(object sender, MouseMoveEventArgs args)
    {
      if (!m_buttonStates[ViewDragButton])
      {
        return;
      }

      var position = new Vector2i(args.X, args.Y);
      var deltaPosition = m_mousePos - position;
      var @event = new ViewDragEvent
      {
        Delta = new Vector2
        {
          X = (float) deltaPosition.X / m_window.Size.X,
          Y = (float) deltaPosition.Y / m_window.Size.Y
        }
      };
      m_eventManager.QueueEvent(@event);

      // update the position
      m_mousePos = position;
    }

    #endregion
  }
}
