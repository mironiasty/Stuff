using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestRunner.Asserts
{
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

}
