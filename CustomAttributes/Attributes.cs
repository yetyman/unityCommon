using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.CustomAttributes
{

    [System.AttributeUsage(System.AttributeTargets.Field)] public class BeginHorizontal : System.Attribute { }
    [System.AttributeUsage(System.AttributeTargets.Field)] public class EndHorizontal : System.Attribute { }
    [System.AttributeUsage(System.AttributeTargets.Field)] public class BeginVertical : System.Attribute { }
    [System.AttributeUsage(System.AttributeTargets.Field)] public class EndVertical : System.Attribute { }
}
