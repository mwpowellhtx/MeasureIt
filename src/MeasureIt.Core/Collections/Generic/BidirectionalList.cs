using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MeasureIt.Collections.Generic
{
    internal class BidirectionalList<T> : IBidirectionalList<T>
    {
        private readonly IList<T> _list;

        private readonly AfterOperationDelegate<T> _onAfterAdded;
        private readonly AfterOperationDelegate<T> _onAfterRemoved;
        private readonly BeforeOperationDelegate<T> _onBeforeAdded;
        private readonly BeforeOperationDelegate<T> _onBeforeRemoved;

        private static readonly BeforeOperationDelegate<T> DefaultBefore = _ => { };
        private static readonly AfterOperationDelegate<T> DefaultAfter = _ => { };

        internal BidirectionalList(
            AfterOperationDelegate<T> onAfterAdded = null, AfterOperationDelegate<T> onAfterRemoved = null
            , BeforeOperationDelegate<T> onBeforeAdded = null, BeforeOperationDelegate<T> onBeforeRemoved = null
            ) : this(null, onAfterAdded, onAfterRemoved, onBeforeAdded, onBeforeRemoved)
        {
        }

        internal BidirectionalList(IList<T> list
            , AfterOperationDelegate<T> onAfterAdded = null, AfterOperationDelegate<T> onAfterRemoved = null
            , BeforeOperationDelegate<T> onBeforeAdded = null, BeforeOperationDelegate<T> onBeforeRemoved = null
            )
        {
            _list = list ?? new List<T>();
            _onAfterAdded = onAfterAdded ?? DefaultAfter;
            _onAfterRemoved = onAfterRemoved ?? DefaultAfter;
            _onBeforeAdded = onBeforeAdded ?? DefaultBefore;
            _onBeforeRemoved = onBeforeRemoved ?? DefaultBefore;
        }

        private void ListAction(Action<IList<T>> action)
        {
            action(_list);
        }

        private TResult ListFunc<TResult>(Func<IList<T>, TResult> func)
        {
            return func(_list);
        }

        public int IndexOf(T item)
        {
            return ListFunc(l => l.IndexOf(item));
        }

        public void Insert(int index, T item)
        {
            ListAction(l =>
            {
                _onBeforeAdded(item);
                l.Insert(index, item);
                _onAfterAdded(item);
            });
        }

        public void RemoveAt(int index)
        {
            ListAction(l =>
            {
                var item = l[index];
                _onBeforeRemoved(item);
                l.RemoveAt(index);
                _onAfterRemoved(item);
            });
        }

        public T this[int index]
        {
            get { return ListFunc(l => l[index]); }
            set
            {
                ListAction(l =>
                {
                    var old = l[index];
                    _onBeforeRemoved(old);
                    _onBeforeAdded(value);
                    l[index] = value;
                    _onAfterRemoved(old);
                    _onAfterAdded(value);
                });
            }
        }

        public void Add(T item)
        {
            ListAction(l =>
            {
                _onBeforeAdded(item);
                l.Add(item);
                _onAfterAdded(item);
            });
        }

        public void Clear()
        {
            ListAction(l =>
            {
                var items = l.ToArray();
                foreach (var item in items) _onBeforeRemoved(item);
                l.Clear();
                foreach (var item in items) _onAfterRemoved(item);
            });
        }

        public bool Contains(T item)
        {
            return ListFunc(l => l.Contains(item));
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            ListAction(l =>
            {
                l.CopyTo(array, arrayIndex);
            });
        }

        public int Count
        {
            get { return ListFunc(l => l.Count); }
        }

        public bool IsReadOnly
        {
            get { return ListFunc(l => l.IsReadOnly); }
        }

        public bool Remove(T item)
        {
            return ListFunc(l =>
            {
                var contains = Contains(item);
                if (contains) _onBeforeRemoved(item);
                var removed = l.Remove(item);
                if (contains) _onAfterRemoved(item);
                return removed;
            });
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ListFunc(l => l.GetEnumerator());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
