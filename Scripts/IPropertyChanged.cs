using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;

namespace Assets.CommonLibrary.Scripts
{
    public class PropertyChangedEvent<T> : UnityEvent<T, string> { };
    public interface IPropertyChanged<T>
    {

        PropertyChangedEvent<T> PropertyChanged { get; set; }
        void OnPropertyChanged(string name = null);
    }
}
