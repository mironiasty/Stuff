using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestRunner.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    class TestMethodAttribute : Attribute
    {
    }
}
