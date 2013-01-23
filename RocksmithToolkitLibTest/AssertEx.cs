using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using NUnit.Framework;

namespace RocksmithToolkitLibTest
{
    public static class AssertEx
    {

        public static void PropertyValuesAreEqual(string path, object actual, object expected, StringBuilder errors)
        {
            if (expected is Array)
            {
                AssertListsAreEquals(path, (Array)actual, (Array)expected, errors);
                return;
            }

            PropertyInfo[] properties = expected.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                object expectedValue = property.GetValue(expected, null);
                object actualValue = property.GetValue(actual, null);
                if (expectedValue == null && actualValue == null)
                {
                    continue;
                }
                if (expectedValue == null)
                {
                    errors.AppendFormat("Property {0}.{1} does not match. Expected: null but was: {2}", path, property.Name, actualValue).AppendLine();
                    continue;
                }

                if (actualValue is Array)
                {
                    AssertListsAreEquals(path + "." + property.Name, (Array)actualValue, (Array)expectedValue, errors);
                }
                else if (expectedValue is float || expectedValue is double)
                {
                    if (Math.Abs((float)expectedValue - (float)actualValue) > .0011)
                    {
                        errors.AppendFormat("Property {0}.{1} does not match. Expected: {2} but was: {3}", path, property.Name, expectedValue, actualValue).AppendLine();
                    }
                }
                else if (expectedValue is IComparable)
                {
                    if (0 != ((IComparable)expectedValue).CompareTo(actualValue))
                    {
                        errors.AppendFormat("Property {0}.{1} does not match. Expected: {2} but was: {3}", path, property.Name, expectedValue, actualValue).AppendLine();
                    }
                }
                else
                {
                    PropertyValuesAreEqual(path + "." + property.Name, actualValue, expectedValue, errors);
                }
            }
        }

        private static void AssertListsAreEquals(string path, Array actualList, Array expectedList, StringBuilder errors)
        {
            if (actualList.Length != expectedList.Length)
            {
                errors.AppendFormat("Property {0} does not match. Expected array containing {1} elements but was array containing {2} elements", path, expectedList.Length, actualList.Length).AppendLine();
                return;
            }

            for (int i = 0; i < actualList.Length; i++)
            {
                PropertyValuesAreEqual(path + "[" + i + "]", actualList.GetValue(i), expectedList.GetValue(i), errors);
            }
        }
    }
}
