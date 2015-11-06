using System;
using System.Collections.Generic;

namespace Game.Core.Extensions
{
  public static class ListExtensions
  {
    /// <summary>
    ///   Removes multiple items from a list, using the default Equals function.
    /// </summary>
    /// <typeparam name="T">
    ///   The type of items in the list.
    /// </typeparam>
    /// <param name="list">
    ///   The list items will be removed from.
    /// </param>
    /// <param name="toRemove">
    ///   The collection of items that will be removed from list.
    /// </param>
    /// <returns>
    ///   A collection of the the objects from toRemove that were not removed
    ///   from list.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///   toRemove is null.
    /// </exception>
    public static ICollection<T> RemoveAllItems<T>(this IList<T> list,
      IEnumerable<T> toRemove)
    {
      return list.RemoveAllItems(toRemove, (a, b) => a.Equals(b));
    }

    /// <summary>
    ///   Removes multiple items from a list, using a comparer.
    /// </summary>
    /// <typeparam name="T">
    ///   The type of items in the list.
    /// </typeparam>
    /// <param name="list">
    ///   The list items will be removed from.
    /// </param>
    /// <param name="toRemove">
    ///   The collection of items that will be removed from list.
    /// </param>
    /// <param name="comparer">
    ///   The comparer object for type T.
    /// </param>
    /// <returns>
    ///   A collection of the the objects from toRemove that were not removed
    ///   from list.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///   toRemove is null
    ///   -or-
    ///   comparer is null.
    /// </exception>
    public static ICollection<T> RemoveAllItems<T>(this IList<T> list,
      IEnumerable<T> toRemove, IComparer<T> comparer)
    {
      if (comparer == null) throw new ArgumentNullException("comparer");

      return list.RemoveAllItems(toRemove, (a, b) => 
        comparer.Compare(a, b) == 0);
    }

    /// <summary>
    ///   Removes multiple items from a list, using a custom compare function.
    /// </summary>
    /// <typeparam name="TItemType">
    ///   The type of the items in the list.
    /// </typeparam>
    /// <typeparam name="TRemovalItemType">
    ///   The type of the items in the removal list.
    /// </typeparam>
    /// <param name="list">
    ///   The list that items will be removed from.
    /// </param>
    /// <param name="toRemove">
    ///   The collection of items (or keys, or whatever) that will be removed
    ///   from the list.
    /// </param>
    /// <param name="compareFunc">
    ///   A function that compares a TItemType and TRemovalItemType, returning
    ///   true if the two are considered equal.
    /// </param>
    /// <returns>
    ///   A collection of the the objects from toRemove that were not removed
    ///   from list.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///   toRemove is null
    ///   -or-
    ///   compareFunc is null.
    /// </exception>
    public static ICollection<TRemovalItemType> 
      RemoveAllItems<TItemType, TRemovalItemType>(this IList<TItemType> list, 
        IEnumerable<TRemovalItemType> toRemove, 
        Func<TItemType, TRemovalItemType, bool> compareFunc)
    {
      if (toRemove == null) throw new ArgumentNullException("toRemove");
      if (compareFunc == null) throw new ArgumentNullException("compareFunc");

      var remainingItems = new List<TRemovalItemType>(toRemove);
      var numRemaining = remainingItems.Count;

      if (list.Count == 0 || numRemaining == 0)
      {
        return remainingItems;
      }

      // iterate backwards to remove as we go
      for (var listIdx = list.Count - 1; listIdx >= 0; listIdx--)
      {
        // iterate items remaining to be removed
        for (var removeIdx = 0; removeIdx < numRemaining; removeIdx++)
        {
          if (!compareFunc(list[listIdx], remainingItems[removeIdx]))
          {
            continue;
          }

          list.RemoveAt(listIdx);

          // swap the removed item with the last remaining item so we
          // don't have to check it again
          remainingItems[removeIdx] = remainingItems[numRemaining - 1];
          numRemaining--;
          break;
        }

        if (numRemaining == 0)
        {
          break;
        }
      }

      var removedCount = remainingItems.Count - numRemaining;
      remainingItems.RemoveRange(numRemaining, removedCount);
      return remainingItems;
    }
  }
}
