using System;
using System.Collections.Generic;
using System.Linq;
using Game.Core.Extensions;
using NUnit.Framework;

// ReSharper disable CollectionNeverUpdated.Local
// ReSharper disable RedundantCast
// ReSharper disable PossibleMultipleEnumeration

namespace Game.Core.Tests.ExtensionsTests
{
  [TestFixture]
  public class ListExtensionsTests
  {
    [Test]
    public void RemoveAllItems_Equals_toRemoveIsNull()
    {
      var list = new List<int>();
      TestDelegate func = () =>
        list.RemoveAllItems(null);

      Assert.Throws<ArgumentNullException>(func);
    }

    [Test]
    public void RemoveAllItems_Equals_BothListsEmpty()
    {
      var list = new List<int>();
      var toRemove = new List<int>();

      var result = list.RemoveAllItems(toRemove);

      Assert.IsEmpty(list);
      Assert.IsEmpty(result);
    }

    [Test]
    public void RemoveAllItems_Equals_toRemoveEmpty()
    {
      var list = new List<int> {1, 2, 3};
      var toRemove = new List<int>();

      var result = list.RemoveAllItems(toRemove);

      Assert.AreEqual(3, list.Count);
      Assert.IsEmpty(result);
    }

    [Test]
    public void RemoveAllItems_Equals_NoneFound()
    {
      var expectedList = new[] {1, 2, 3};
      var list = new List<int>(expectedList);
      var toRemove = new List<int> {4, 5};

      var result = list.RemoveAllItems(toRemove);

      CollectionAssert.AreEqual(expectedList, list);
      Assert.AreEqual(2, result.Count());
      CollectionAssert.Contains(result, 4);
      CollectionAssert.Contains(result, 5);
    }

    [Test]
    public void RemoveAllItems_Equals_SomeFound_ListNotEmptied()
    {
      var expectedList = new[] { 1, 3 };
      var list = new List<int> {1, 2, 3};
      var toRemove = new List<int> {2, 4, 5};

      var result = list.RemoveAllItems(toRemove);

      CollectionAssert.AreEqual(expectedList, list);
      Assert.AreEqual(2, result.Count());
      CollectionAssert.Contains(result, 4);
      CollectionAssert.Contains(result, 5);
    }

    [Test]
    public void RemoveAllItems_Equals_SomeFound_ListEmptied()
    {
      var list = new List<int> {1, 2, 3};
      var toRemove = new List<int> {1, 2, 3, 4};

      var result = list.RemoveAllItems(toRemove);

      Assert.IsEmpty(list);
      Assert.AreEqual(1, result.Count());
      CollectionAssert.Contains(result, 4);
    }

    [Test]
    public void RemoveAllItems_Equals_AllFound_ListNotEmptied()
    {
      var list = new List<int> {1, 2, 3, 4};
      var toRemove = new List<int> {1, 2, 4};

      var result = list.RemoveAllItems(toRemove);

      CollectionAssert.AreEqual(new [] {3}, list);
      Assert.IsEmpty(result);
    }

    [Test]
    public void RemoveAllItems_Equals_AllFound_ListEmptied()
    {
      var list = new List<int> {1, 2, 3};
      var toRemove = new List<int>(list);

      var result = list.RemoveAllItems(toRemove);

      Assert.IsEmpty(list);
      Assert.IsEmpty(result);
    }

    [Test]
    public void RemoveAllItems_Comparer_toRemoveIsNull()
    {
      var list = new List<int>();
      TestDelegate func = () =>
        list.RemoveAllItems(null, (IComparer<int>)null);

      Assert.Throws<ArgumentNullException>(func);
    }

    [Test]
    public void RemoveAllItems_Comparer_ComparerIsNull()
    {
      var list = new List<int>();
      var toRemove = new List<int>();

      TestDelegate func = () => 
        list.RemoveAllItems(toRemove, (IComparer<int>)null);

      Assert.Throws<ArgumentNullException>(func);
    }

    [Test]
    public void RemoveAllItems_Comparer_BothListsEmpty()
    {
      var list = new List<int>();
      var toRemove = new List<int>();

      var result = list.RemoveAllItems(toRemove, Comparer<int>.Default);

      Assert.IsEmpty(list);
      Assert.IsEmpty(result);
    }

    [Test]
    public void RemoveAllItems_Comparer_toRemoveEmpty()
    {
      var list = new List<int> { 1, 2, 3 };
      var toRemove = new List<int>();

      var result = list.RemoveAllItems(toRemove, Comparer<int>.Default);

      Assert.AreEqual(3, list.Count);
      Assert.IsEmpty(result);
    }

    [Test]
    public void RemoveAllItems_Comparer_NoneFound()
    {
      var expectedList = new[] { 1, 2, 3 };
      var list = new List<int>(expectedList);
      var toRemove = new List<int> { 4, 5 };

      var result = list.RemoveAllItems(toRemove, Comparer<int>.Default);

      CollectionAssert.AreEqual(expectedList, list);
      Assert.AreEqual(2, result.Count());
      CollectionAssert.Contains(result, 4);
      CollectionAssert.Contains(result, 5);
    }

    [Test]
    public void RemoveAllItems_Comparer_SomeFound_ListNotEmptied()
    {
      var expectedList = new[] { 1, 3 };
      var list = new List<int> { 1, 2, 3 };
      var toRemove = new List<int> { 2, 4, 5 };

      var result = list.RemoveAllItems(toRemove, Comparer<int>.Default);

      CollectionAssert.AreEqual(expectedList, list);
      Assert.AreEqual(2, result.Count());
      CollectionAssert.Contains(result, 4);
      CollectionAssert.Contains(result, 5);
    }

    [Test]
    public void RemoveAllItems_Comparer_SomeFound_ListEmptied()
    {
      var list = new List<int> { 1, 2, 3 };
      var toRemove = new List<int> { 1, 2, 3, 4 };

      var result = list.RemoveAllItems(toRemove, Comparer<int>.Default);

      Assert.IsEmpty(list);
      Assert.AreEqual(1, result.Count());
      CollectionAssert.Contains(result, 4);
    }

    [Test]
    public void RemoveAllItems_Comparer_AllFound_ListNotEmptied()
    {
      var list = new List<int> { 1, 2, 3, 4 };
      var toRemove = new List<int> { 1, 2, 4 };

      var result = list.RemoveAllItems(toRemove, Comparer<int>.Default);

      CollectionAssert.AreEqual(new[] { 3 }, list);
      Assert.IsEmpty(result);
    }

    [Test]
    public void RemoveAllItems_Comparer_AllFound_ListEmptied()
    {
      var list = new List<int> { 1, 2, 3 };
      var toRemove = new List<int>(list);

      var result = list.RemoveAllItems(toRemove, Comparer<int>.Default);

      Assert.IsEmpty(list);
      Assert.IsEmpty(result);
    }

    [Test]
    public void RemoveAllItems_Func_toRemoveIsNull()
    {
      var list = new List<int>();
      TestDelegate func = () =>
        list.RemoveAllItems(null, (Func<int, int, bool>)null);

      Assert.Throws<ArgumentNullException>(func);
    }

    [Test]
    public void RemoveAllItems_Func_compareFuncIsNull()
    {
      var list = new List<int>();
      var toRemove = new List<int>();

      TestDelegate func = () =>
        list.RemoveAllItems(toRemove, (Func<int, int, bool>)null);

      Assert.Throws<ArgumentNullException>(func);
    }

    [Test]
    public void RemoveAllItems_Func_BothListsEmpty()
    {
      var list = new List<int>();
      var toRemove = new List<int>();
      Func<int, int, bool> compare = (a, b) => a == b;

      var result = list.RemoveAllItems(toRemove, compare);

      Assert.IsEmpty(list);
      Assert.IsEmpty(result);
    }

    [Test]
    public void RemoveAllItems_Func_toRemoveEmpty()
    {
      var list = new List<int> { 1, 2, 3 };
      var toRemove = new List<int>();
      Func<int, int, bool> compare = (a, b) => a == b;

      var result = list.RemoveAllItems(toRemove, compare);

      Assert.AreEqual(3, list.Count);
      Assert.IsEmpty(result);
    }

    [Test]
    public void RemoveAllItems_Func_NoneFound()
    {
      var expectedList = new[] { 1, 2, 3 };
      var list = new List<int>(expectedList);
      var toRemove = new List<int> { 4, 5 };
      Func<int, int, bool> compare = (a, b) => a == b;

      var result = list.RemoveAllItems(toRemove, compare);

      CollectionAssert.AreEqual(expectedList, list);
      Assert.AreEqual(2, result.Count());
      CollectionAssert.Contains(result, 4);
      CollectionAssert.Contains(result, 5);
    }

    [Test]
    public void RemoveAllItems_Func_SomeFound_ListNotEmptied()
    {
      var expectedList = new[] { 1, 3 };
      var list = new List<int> { 1, 2, 3 };
      var toRemove = new List<int> { 2, 4, 5 };
      Func<int, int, bool> compare = (a, b) => a == b;

      var result = list.RemoveAllItems(toRemove, compare);

      CollectionAssert.AreEqual(expectedList, list);
      Assert.AreEqual(2, result.Count());
      CollectionAssert.Contains(result, 4);
      CollectionAssert.Contains(result, 5);
    }

    [Test]
    public void RemoveAllItems_Func_SomeFound_ListEmptied()
    {
      var list = new List<int> { 1, 2, 3 };
      var toRemove = new List<int> { 1, 2, 3, 4 };
      Func<int, int, bool> compare = (a, b) => a == b;

      var result = list.RemoveAllItems(toRemove, compare);

      Assert.IsEmpty(list);
      Assert.AreEqual(1, result.Count());
      CollectionAssert.Contains(result, 4);
    }

    [Test]
    public void RemoveAllItems_Func_AllFound_ListNotEmptied()
    {
      var list = new List<int> { 1, 2, 3, 4 };
      var toRemove = new List<int> { 1, 2, 4 };
      Func<int, int, bool> compare = (a, b) => a == b;

      var result = list.RemoveAllItems(toRemove, compare);

      CollectionAssert.AreEqual(new[] { 3 }, list);
      Assert.IsEmpty(result);
    }

    [Test]
    public void RemoveAllItems_Func_AllFound_ListEmptied()
    {
      var list = new List<int> { 1, 2, 3 };
      var toRemove = new List<int>(list);
      Func<int, int, bool> compare = (a, b) => a == b;

      var result = list.RemoveAllItems(toRemove, compare);

      Assert.IsEmpty(list);
      Assert.IsEmpty(result);
    }
  }
}
