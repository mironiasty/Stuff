using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestRunner.Asserts
{
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

}
