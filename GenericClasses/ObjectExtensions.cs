using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.CommonLibrary.GenericClasses
{
    public static class ObjectExtensions
    {

        public static T Get<T>(this T[,] array, Vector2Int position, T defaultValue = default(T))
        {
            return array.Get(position.x, position.y, defaultValue);
        }
        public static T Get<T>(this T[,] array, int x, int y, T defaultValue = default(T))
        {
            return (x>=0 && x < array.GetLength(0) && y>=0 && y < array.GetLength(1)) ? array[x, y] : defaultValue;
        }
        public static T Get<T>(this IEnumerable<T> array, int position, bool circular = false, T defaultValue = default(T))
        {
            if (array == null)
                return defaultValue;
            if (circular)
                return array.ElementAt(position.PositiveMod(array.Count()));
            else
                return (position >=0 && position < array.Count()) ? array.ElementAt(position) : defaultValue;
        }

        public static T Get<T>(this IEnumerable<IEnumerable<T>> array, Vector2Int position, T defaultValue = default(T))
        {
            return array.Get(position.x, position.y, defaultValue);
        }
        public static T Get<T>(this IEnumerable<IEnumerable<T>> array, int x, int y, T defaultValue = default(T))
        {
            return array.Get(x).Get(y, false, defaultValue);
        }

    }
}
