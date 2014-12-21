using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TestRunner.Attributes;

namespace TestRunner
{

    class Runner
    {
        private Assembly testedAssembly;

        public Runner(Assembly assembly)
        {
            testedAssembly = assembly;
        }

        public IEnumerable<string> RunTests()
        {
            var result = new List<string>();
            foreach (var testedTypes in GetTestClasses(testedAssembly))
            {
                result.AddRange(RunTestsInClass(testedTypes));
            }

            return result;
        }

        private IEnumerable<string> RunTestsInClass(Type type)
        {
            object objectToTest = Activator.CreateInstance(type);
            foreach (var method in GetTestMethods(type))
            {
                yield return RunTestOnMethod(method, objectToTest);
            }
        }

        private string RunTestOnMethod(MethodInfo methodToTest, object instance)
        {
            bool expectingException = methodToTest.GetCustomAttributes<ExpectedExceptionAttribute>().Any();

            try
            {
                methodToTest.Invoke(instance, new object[0]);
                if (expectingException)
                {
                    return string.Format("{0} failed: missing excpetion", methodToTest.Name);
                }
                else
                {
                    return string.Format("{0} OK", methodToTest.Name);
                }
            }
            catch (TargetInvocationException e)
            {
                if (expectingException)
                {
                    var expectedExceptionType = methodToTest.GetCustomAttribute<ExpectedExceptionAttribute>(true).Exception;
                    if (e.InnerException.GetType().IsAssignableFrom(expectedExceptionType))
                    {
                        return string.Format("{0} OK", methodToTest.Name);
                    }
                    else
                    {
                        return string.Format("{0} failed: ", e.Message);
                    }
                }
                else
                {
                    return string.Format("{0} failed: ", e.Message);
                }
            }
        }

        private IEnumerable<Type> GetTestClasses(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (type.GetCustomAttributes(typeof(TestClassAttribute), true).Length > 0)
                {
                    yield return type;
                }
            }
        }

        private IEnumerable<MethodInfo> GetTestMethods(Type testClass)
        {
            foreach (var method in testClass.GetMethods())
            {
                if (method.GetCustomAttributes(typeof(TestMethodAttribute), true).Length > 0)
                {
                    yield return method;
                }
            }
        }

    }
}
