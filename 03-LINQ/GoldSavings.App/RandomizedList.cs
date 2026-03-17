using System;
using System.Collections.Generic;

namespace GoldSavings.App
{
    public class RandomizedList<T>
    {
        private readonly List<T> _items = new List<T>();
        private readonly Random _random = new Random();

        public void Add(T element)
        {
            if (_random.Next(2) == 0)
            {
                _items.Insert(0, element);
            }
            else
            {
                _items.Add(element);
            }
        }

        public T Get(int index)
        {
            if (IsEmpty)
            {
                throw new InvalidOperationException("Cannot get an element from an empty collection.");
            }

            int maxPossibleIndex = Math.Min(index, _items.Count - 1);

            int randomIndex = _random.Next(0, maxPossibleIndex + 1);

            return _items[randomIndex];
        }

        public bool IsEmpty => _items.Count == 0;

        public int Count => _items.Count;
    }
}