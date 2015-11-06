using System;
using System.Collections.Generic;

namespace Game.Core.Factories
{
  /// <summary>
  ///   The interface for all factories in the game.
  /// </summary>
  /// <typeparam name="T">
  ///   The type that this factory creates.
  /// </typeparam>
  public interface IFactory<out T>
  {
    /// <summary>
    ///   All templates available from this factory.
    /// </summary>
    IReadOnlyCollection<string> Templates { get; }

    /// <summary>
    ///   Create an object from a factory template.
    /// </summary>
    /// <param name="name"></param>
    /// <returns>
    ///   The object or null if it fails to create.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///   name is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///   name does not match a template in the factory.
    /// </exception>
    T Create(string name);
  }
}
