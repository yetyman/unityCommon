using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Animation
{
    public class AnimationSummary
    {
        public object target;
        public string setter;
        public string getter;

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = (AnimationSummary)obj;
            return target == other.target && setter == other.setter && getter == other.getter;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            if (target != null && setter != null && getter != null)
                return target.GetHashCode() ^ setter.GetHashCode() ^ getter.GetHashCode();
            if (target != null && setter != null)
                return target.GetHashCode() ^ setter.GetHashCode();
            else
                return base.GetHashCode();
        }
    }

}
