using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadEngine.Architecture
{
    public class SingletonField<T> where T : class
    {
        private string _fieldName;

        private T _i = null;

        /// <summary>
        /// Stored singleton instance.
        /// </summary>
        public T I 
        {
            get => _i; 
            set
            {
                if (_i != null) throw new InvalidOperationException($"{_fieldName} of type {typeof(T)} is already assigned!");
                else _i = value;
            }
        }

        public SingletonField(string fieldName)
        {
            _fieldName = fieldName;
        }

        public void Set(T value) => I = value;
    }
}
