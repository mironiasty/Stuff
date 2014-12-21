using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

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

    public class SuperIntTestLibrary
    {
        private int expectedValue;
        private bool denyEverything;


        internal SuperIntTestLibrary(int startValue)
            : this(startValue, false)
        {
        }

        internal SuperIntTestLibrary(int startValue, bool denyEverything)
        {
            this.expectedValue = startValue;
            this.denyEverything = denyEverything;
        }

        public void Eq(int compareTo)
        {
            MakeCondition(expectedValue != compareTo);
        }

        public void IsGreater(int compareTo)
        {
            MakeCondition(expectedValue <= compareTo);
        }

        public SuperIntTestLibrary Not()
        {
            return new SuperIntTestLibrary(expectedValue, true);
        }

        
        private void MakeCondition(bool condition)
        {
            if (denyEverything) condition = !condition;
            if (condition)
            {
                throw new ExpectationFailedExceptin();
            }
        }
    }

    public class SuperActionTestLibrary
    {
        private Action expectedAction;

        internal SuperActionTestLibrary(Action startAction)
        {
            this.expectedAction = startAction;
        }

        public void RaiseError()
        {
            try
            {
                expectedAction();
            }
            catch (Exception)
            {
                return;
            }
            throw new ExpectationFailedExceptin();
        }
    }

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

    public class ExpectationFailedExceptin : Exception
    { }
}
