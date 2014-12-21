using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestRunner.Asserts
{
    public static class ThisIsExtensiooooon
    {
        public static SuperIntTestLibrary Expect(this int originalValue)
        {
            return new SuperIntTestLibrary(originalValue);
        }

        public static SuperActionTestLibrary Expect(this Action thisIsAction)
        {
            return new SuperActionTestLibrary(thisIsAction);
        }

        public static SuperObjectTestLibrary<T> Expect<T>(this T objectify)
        {
            return new SuperObjectTestLibrary<T>(objectify);
        }
    }
}
