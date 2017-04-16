// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="SetByValue.cs">
// //     Copyright 2016
// //           Thomas PIERRAIN (@tpierrain)    
// //     Licensed under the Apache License, Version 2.0 (the "License");
// //     you may not use this file except in compliance with the License.
// //     You may obtain a copy of the License at
// //         http://www.apache.org/licenses/LICENSE-2.0
// //     Unless required by applicable law or agreed to in writing, software
// //     distributed under the License is distributed on an "AS IS" BASIS,
// //     WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// //     See the License for the specific language governing permissions and
// //     limitations under the License.b 
// // </copyright>
// // --------------------------------------------------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;

namespace Value
{
    /// <summary>
    ///     A Set with equality based on its content and not on the Set's reference 
    ///     (i.e.: 2 different instances containing the same items will be equals whatever their storage order).
    /// </summary>
    /// <remarks>This type is not thread-safe (for hashcode updates).</remarks>
    /// <typeparam name="T">Type of the listed items.</typeparam>
    public class SetByValue<T> : ISet<T>
    {
        private readonly ISet<T> hashSet;

        private int? hashCode;

        public SetByValue(ISet<T> hashSet)
        {
            this.hashSet = hashSet;
        }

        public SetByValue() : this(new HashSet<T>())
        {
        }

        public int Count => hashSet.Count;

        public bool IsReadOnly => hashSet.IsReadOnly;

        public void Add(T item)
        {
            ResetHashCode();
            hashSet.Add(item);
        }

        public void Clear()
        {
            ResetHashCode();
            hashSet.Clear();
        }

        public bool Contains(T item)
        {
            return hashSet.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            hashSet.CopyTo(array, arrayIndex);
        }

        public override bool Equals(object obj)
        {
            var other = obj as SetByValue<T>;
            if (other == null)
            {
                return false;
            }

            return hashSet.SetEquals(other);
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            ResetHashCode();
            hashSet.ExceptWith(other);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return hashSet.GetEnumerator();
        }

        public override int GetHashCode()
        {
            if (hashCode == null)
            {
                var code = 0;

                // Two instances with same elements added in different order must return the same hashcode
                // Let's compute and sort hashcodes of all elements (always in the same order)
                var sortedHashCodes = new SortedSet<int>();
                foreach (var element in hashSet)
                {
                    sortedHashCodes.Add(element.GetHashCode());
                }

                foreach (var elementHashCode in sortedHashCodes)
                {
                    code = (code * 397) ^ elementHashCode;
                }

                // Cache the result in a field
                hashCode = code;
            }

            return hashCode.Value;
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            ResetHashCode();
            hashSet.IntersectWith(other);
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            return hashSet.IsProperSubsetOf(other);
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            return hashSet.IsProperSupersetOf(other);
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            return hashSet.IsSubsetOf(other);
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            return hashSet.IsSupersetOf(other);
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            return hashSet.Overlaps(other);
        }

        public bool Remove(T item)
        {
            ResetHashCode();
            return hashSet.Remove(item);
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            return hashSet.SetEquals(other);
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            ResetHashCode();
            hashSet.SymmetricExceptWith(other);
        }

        public void UnionWith(IEnumerable<T> other)
        {
            ResetHashCode();
            hashSet.UnionWith(other);
        }

        bool ISet<T>.Add(T item)
        {
            ResetHashCode();
            return hashSet.Add(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) hashSet).GetEnumerator();
        }

        protected virtual void ResetHashCode()
        {
            hashCode = null;
        }
    }
}