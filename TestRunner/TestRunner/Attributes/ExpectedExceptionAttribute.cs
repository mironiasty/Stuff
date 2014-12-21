using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestRunner.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple=false)]
    class ExpectedExceptionAttribute : Attribute
    {
        private readonly Type typeOfException;

        public ExpectedExceptionAttribute(Type typeOfException)
        {
            this.typeOfException = typeOfException;
        }

        public Type Exception
        {
            get
            {
                return typeOfException;
            }
        }
    }
}
