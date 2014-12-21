using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace TestRunner.Asserts
{
    public class SuperObjectTestLibrary<T>
    {
        private T expectedValue;
        private string ommitedPropertyName;

        internal SuperObjectTestLibrary(T someSobject)
        {
            this.expectedValue = someSobject;
        }

        internal SuperObjectTestLibrary(T someSobject, string ommitedProperty)
            : this(someSobject)
        {
            this.ommitedPropertyName = ommitedProperty;

        }

        public SuperObjectTestLibrary<T> Properties()
        {
            return this;
        }

        public SuperObjectTestLibrary<T> PropertiesWithout(Expression<Func<T, object>> xx)
        {
            var memberExpr = xx.Body as MemberExpression;
            string memberName = memberExpr.Member.Name;
            return new SuperObjectTestLibrary<T>(this.expectedValue, memberName);
        }

        public void Eq(object another)
        {
            foreach (var p in expectedValue.GetType().GetProperties())
            {
                if (p.Name == ommitedPropertyName)
                    continue;
                var property = another.GetType().GetProperties().Where(pa => pa.Name == p.Name).FirstOrDefault();
                if (property != null && !property.GetValue(another).Equals(p.GetValue(expectedValue)))
                {
                    throw new ExpectationFailedExceptin();
                }
            }
        }
    }

}
