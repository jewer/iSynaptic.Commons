﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace iSynaptic.Commons.Collections.Generic
{
    public static class EnumerableExtensions
    {
        public static void CopyTo<T>(this IEnumerable<T> source, T[] destination, int index)
        {
            Guard.NotNull(source, "source");
            Guard.NotNull(destination, "destination");

            if ((index < 0) || (index > destination.Length))
                throw new ArgumentOutOfRangeException(
                    "index", "Number must be either non-negative and less than or equal to Int32.MaxValue or -1.");

            if ((destination.Length - index) < source.Count())
                throw new ArgumentException("Destination array is not long enough to copy all the items in the collection. Check array index and length.", "index");

            foreach (var item in source)
                destination[index++] = item;
        }

        public static IEnumerable<IndexedValue<T>> WithIndex<T>(this IEnumerable<T> self)
        {
            int index = 0;

            foreach (T item in self)
                yield return new IndexedValue<T>(index++, item);
        }

        public static IEnumerable<LookAheadableValue<T>> AsLookAheadable<T>(this IEnumerable<T> self)
        {
            Guard.NotNull(self, "self");
            return new LookAheadEnumerable<T>(self);
        }

        public static IEnumerable<T> Buffer<T>(this IEnumerable<T> self)
        {
            Guard.NotNull(self, "self");

            return self.ToArray();
        }

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> self)
        {
            Guard.NotNull(self, "self");

            return self.ToDictionary(x => x.Key, x => x.Value);
        }

        public static ReadOnlyDictionary<TKey, TValue> ToReadOnlyDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> self)
        {
            return self
                .ToDictionary()
                .ToReadOnlyDictionary();
        }

        public static string Delimit<T>(this IEnumerable<T> self, string delimiter)
        {
            return Delimit(self, delimiter, item => item.ToString());
        }

        public static string Delimit<T>(this IEnumerable<T> self, string delimiter, Func<T, string> selector)
        {
            Guard.NotNull(self, "self");
            Guard.NotNull(delimiter, "delimiter");
            Guard.NotNull(selector, "selector");

            var builder = new StringBuilder();
            bool isFirst = true;

            foreach (T item in self)
            {
                if (isFirst)
                    isFirst = false;
                else
                    builder.Append(delimiter);

                builder.Append(selector(item));
            }

            return builder.ToString();
        }

        public static IEnumerable<T[]> Zip<T>(this IEnumerable<IEnumerable<T>> iterables)
        {
            return ZipCore(iterables);
        }

        public static IEnumerable<T[]> Zip<T>(this IEnumerable<T>[] iterables)
        {
            return ZipCore(iterables);
        }
        
        public static IEnumerable<T[]> Zip<T>(this IEnumerable<T> first, params IEnumerable<T>[] iterables)
        {
            Guard.NotNull(first, "first");
            Guard.NotNull(iterables, "iterables");

            return ZipCore(new[] { first }.Concat(iterables));
        }

        private static IEnumerable<T[]> ZipCore<T>(IEnumerable<IEnumerable<T>> iterables)
        {
            IEnumerator<T>[] enumerators = iterables
                .Select(x => x != null ? x.GetEnumerator() : null)
                .ToArray();

            while (enumerators.Where(x => x != null).Count() > 0)
            {
                int index = 0;
                T[] values = new T[enumerators.Length];

                bool anyIsAvailable = false;
                foreach (IEnumerator<T> enumerator in enumerators)
                {
                    if (enumerator == null)
                        continue;

                    bool isAvailable = enumerator.MoveNext();

                    if (isAvailable != true)
                    {
                        enumerators[index] = null;
                        index++;

                        continue;
                    }

                    anyIsAvailable = true;
                    values[index++] = enumerator.Current;
                }

                if (anyIsAvailable)
                    yield return values;
            }
        }

        public static void ForceEnumeration<T>(this IEnumerable<T> self)
        {
            Guard.NotNull(self, "self");

            self.All(x => true);
        }

        public static IEnumerable<T> MeetsSpecifcation<T>(this IEnumerable<T> candidates, Specification<T> specification)
        {
            return candidates.Where(specification.IsSatisfiedBy);
        }

        public static IEnumerable<T> FailsSpecification<T>(this IEnumerable<T> candidates, Specification<T> specification)
        {
            return candidates.Where(x => specification.IsSatisfiedBy(x) != true);
        }

        public static bool AllSatisfy<T>(this IEnumerable<T> candidates, Specification<T> specification)
        {
            return candidates.All(specification.IsSatisfiedBy);
        }
    }
}
