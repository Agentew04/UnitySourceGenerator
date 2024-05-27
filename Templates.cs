using System;
using System.Collections.Generic;
using System.Text;

namespace UnitySourceGenerator {
    internal static class Templates {

        public const string NotifyChangeAttributeText = @"
using System;

namespace UnitySourceGenerators {
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    internal class NotifyChangeAttribute : Attribute { 
        public NotifyChangeAttribute() { }
    }
}
";
        public const string notifyChangeNamespacedClassCode = @"
namespace {0} {{
    public partial class {1} {{
        {2}
    }}
}}
";

        public const string notifyChangeClassCode = @"
public partial class {0} {{
    {1}
}}
";

        public const string notifyChangePartialMethodsCode = @"
        partial void On{3}Changed({1} newValue);
        partial void On{3}Changing({1} oldValue, {1} newValue);
        {2} {1} {3} {{
            get => {0};
            set {{
                var oldValue = {0};
                var newValue = value;
                On{3}Changing(oldValue, newValue);
                {0} = newValue;
                On{3}Changed(newValue);
            }}
        }}
";
    }
}
