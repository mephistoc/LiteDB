﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace LiteDB
{
    internal class EnumerableResolver : ITypeResolver
    {
        public string ResolveMethod(MethodInfo method)
        {
            // all methods in Enumerable are Extensions (static methods), so first parameter is IEnumerable
            var name = Reflection.MethodName(method, 1); 

            switch (name)
            {
                // get all items
                case "AsEnumerable()": return "@0[*]";

                // get fixed index item
                case "get_Item(int)": return "#[@0]";
                case "ElementAt(int)": return "@0[@1]";
                case "Single()":
                case "First()":
                case "SingleOrDefault()":
                case "FirstOrDefault()": return "@0[0]";
                case "Last()":
                case "LastOrDefault()": return "@0[-1]";

                // get single item but with predicate function
                case "Single(Func<T,TResult>)":
                case "First(Func<T,TResult>)":
                case "SingleOrDefault(Func<T,TResult>)":
                case "FirstOrDefault(Func<T,TResult>)": return "FIRST(@0[@1])";
                case "Last(Func<T,TResult>)":
                case "LastOrDefault(Func<T,TResult>)": return "LAST(@0[@1])";

                // filter
                case "Where(Func<T,TResult>)": return "@0[@1]";
                
                // map
                case "Select(Func<T,TResult>)": return "(@0 => @1)";

                // aggregate
                case "Count()": return "COUNT(@0)";
                case "Sum()": return "SUM(@0)";
                case "Average()": return "AVG(@0)";
                case "Max()": return "MAX(@0)";
                case "Min()": return "MIN(@0)";

                // aggregate with map function
                case "Count(Func<T,TResult>)": return "COUNT(@0 => @1)";
                case "Sum(Func<T,TResult>)": return "SUM(@0 => @1)";
                case "Average(Func<T,TResult>)": return "AVG(@0 => @1)";
                case "Max(Func<T,TResult>)": return "MAX(@0 => @1)";
                case "Min(Func<T,TResult>)": return "MIN(@0 => @1)";

                // convert to array
                case "ToList()": 
                case "ToArray()": return "ARRAY(@0)";

                //case "Any(Func<T,TResult>)": return "@0 ANY @1";
            };

            return null;
        }

        public string ResolveMember(MemberInfo member)
        {
            // this both members are not from IEnumerable:
            // but any IEnumerable type will run this resolver (IList, ICollection)
            switch(member.Name)
            {
                case "Length": return "LENGTH(#)";
                case "Count": return "COUNT(#)";
            }

            return null;
        }

        public string ResolveCtor(ConstructorInfo ctor) => null;
    }
}