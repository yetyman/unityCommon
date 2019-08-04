using Assets.CommonLibrary.GenericClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Maps
{
    public class GridFileMap : IMap<char[,]>
    {

        public Vector3 Size { get { return new Vector2(Values.GetLength(0), Values.GetLength(1)); } }

        private char[,] _values;
        public char[,] Values => _values;

        public GridFileMap(char[,] mapvalues)
        {
            _values = mapvalues;
        }
        public char this[int x,int y]
        {
            get { return _values.Get(x, y, '1'); }
            set { _values[x, y] = value; }
        }
        public char Get(int x, int y, char defaultValue = '1')
        {
            return _values.Get(x, y, defaultValue);
        }
        public char Get(Vector2Int pos, char defaultValue = '1')
        {
            return _values.Get(pos, defaultValue);
        }
    }
}
