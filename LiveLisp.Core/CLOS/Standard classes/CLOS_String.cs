using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.Types;
using System.Collections;
using LiveLisp.Core.BuiltIns.Conditions;
using System.Dynamic;

using exp = System.Linq.Expressions.Expression;
using System.Linq.Expressions;

namespace LiveLisp.Core.CLOS.Standard_classes
{
    public delegate DynamicMetaObject Fallback(DynamicMetaObject errorSuggestion);


    public class CLOS_T : CLOSClass
    {
        public CLOS_T()
        {
            _name = DefinedSymbols.T;
            CLOSClass.T = this;
        }

        public override bool Is(object obj)
        {
            return true;
        }

        static CLOS_T instance;

        public static CLOS_T Instance
        {
            get { return CLOS_T.instance; }
        }

        static CLOS_T()
       {
           instance = new CLOS_T();
       }
    }

    public class CLOS_Sequence : CLOSClass
    {
        private CLOS_Sequence()
        {
            _name = DefinedSymbols.Sequence;
        }

        static CLOS_Sequence instance;

        public static CLOS_Sequence Instance
        {
            get { return CLOS_Sequence.instance; }
        }

        static CLOS_Sequence()
       {
           instance = new CLOS_Sequence();
       }

        public override bool Is(object obj)
        {
            if (obj is string)
                return true;
            else if (obj is List<object>)
                return true;
            else if (obj is IList)
                return true;
            if (obj is Array)
            {
                var ar = obj as Array;
                return ar.Rank == 1;
            }
            else
            {
                var type = obj.GetType();
                return type.GetGenericTypeDefinition() == typeof(IList<>);
            }
        }

        public override CLOSInstance The(object obj)
        {
            if (!Is(obj))
            {
                ConditionsDictionary.TypeError(obj + " is not a sequence");
            }

            if (obj is string)
            {
                return new StringWrapper(obj as string);
            }

            return new CLRCLOSInstance(obj, this);
        }

        internal override DynamicMetaObject  GetDynamicMetaObject(System.Linq.Expressions.Expression parameter, CLOSInstance instance)
        {
            if (instance is StringWrapper)
            {
                return new StringDynamicWrapper(instance as StringWrapper).GetMetaObject(parameter);
            }

 	        return base.GetDynamicMetaObject(parameter, instance);
        }

       

        class CustomInvokeBinder : InvokeMemberBinder
        {
            InvokeMemberBinder fallback;
            public CustomInvokeBinder(string name, InvokeMemberBinder fallback)
                :base(name, fallback.IgnoreCase, fallback.CallInfo)
            {
                this.fallback = fallback;
            }

            public override DynamicMetaObject FallbackInvoke(DynamicMetaObject target, DynamicMetaObject[] args, DynamicMetaObject errorSuggestion)
            {
                return fallback.FallbackInvoke(target, args, errorSuggestion);
            }

            public override DynamicMetaObject FallbackInvokeMember(DynamicMetaObject target, DynamicMetaObject[] args, DynamicMetaObject errorSuggestion)
            {
                return fallback.FallbackInvokeMember(target, args, errorSuggestion);
            }
        }

        public override dynamic CreateInstance()
        {
            return new List<object>();
        }
    }

    class StringDynamicWrapper : DynamicObject
    {
        StringWrapper wrapper;

        public StringDynamicWrapper(StringWrapper wrapper)
        {
            this.wrapper = wrapper;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            if (binder.Name == "Count")
            {
                result = wrapper.str.Length;

                return true;
            }
            return base.TryInvokeMember(binder, args, out result);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (binder.Name == "Count")
            {
                result = wrapper.str.Length;

                return true;
            }
            return base.TryGetMember(binder, out result);
        }
    }

        class StringWrapper : CLOSInstance
        {
            
            internal string str;

            public StringWrapper(string str)
            {
                this.str = str;
                _class = CLOS_String.Instance;
            }
            public override object  this[Symbol name]
            {
            	  get 
            	{ 
            		throw new NotImplementedException(); 
            	}
            	  set 
            	{ 
            		throw new NotImplementedException(); 
            	}
            }

            public override bool  Eq(object obj)
            {
             	throw new NotImplementedException();
            }

            public override IEnumerable<Symbol>  GetAllFields()
            {
             	throw new NotImplementedException();
            }

            public override object  ToNearestCLRType()
            {
                return str;
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                if (binder.Name == "Count")
                {
                    result = str.Length;
                    return true;
                }

                dynamic stri = str;

                IDynamicMetaObjectProvider dmp = stri as IDynamicMetaObjectProvider;

                return base.TryGetMember(binder, out result);
            }            
        }
        class MyDynamicMetaObject : DynamicMetaObject
        {

            public MyDynamicMetaObject(System.Linq.Expressions.Expression expression, object value)
                : base(expression, BindingRestrictions.Empty, value)
            {
            }

            public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
            {
                DynamicMetaObject obj4;
                string name = "";
                if (binder.Name == "Count")
                {

                    name = "Length";
                }
                else
                {
                    name = binder.Name;
                }
                exp exp1 = exp.PropertyOrField(exp.Field(Convert(this.Expression, typeof(StringWrapper)), "str"), name);

                //DynamicMetaObject dmo = new DynamicMetaObject(this.Expression, this.Restrictions, (this.Value as StringWrapper).str);

                Fallback fallback = e => binder.FallbackGetMember(this, e);
                exp[] args = new exp[0];
                DynamicMetaObject obj2 = fallback(null);
                var expression = exp.Parameter(typeof(object), "$arg0");
                exp[] destinationArray = new exp[args.Length + 2];
                Array.Copy(args, 0, destinationArray, 1, args.Length);
                destinationArray[0] = Constant(binder);
                destinationArray[destinationArray.Length - 1] = expression;
                DynamicMetaObject errorSuggestion = new DynamicMetaObject(expression, BindingRestrictions.Empty);
                obj4 = new DynamicMetaObject(exp1, obj2.Restrictions);

                return fallback(obj4);
            }

            public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
            {
                DynamicMetaObject obj4;
                string name = "";
                if (binder.Name == "Count")
                {

                    name = "Length";
                }
                else
                {
                    name = binder.Name;
                }
                exp exp1 = exp.PropertyOrField(exp.Field(Convert(this.Expression, typeof(StringWrapper)), "str"), name);

                //DynamicMetaObject dmo = new DynamicMetaObject(this.Expression, this.Restrictions, (this.Value as StringWrapper).str);

                Fallback fallback = e => binder.FallbackInvokeMember(this, args, e);
                DynamicMetaObject obj2 = fallback(null);
                var expression = exp.Parameter(typeof(object), "$arg0");
                exp[] destinationArray = new exp[args.Length + 2];
                Array.Copy(args, 0, destinationArray, 1, args.Length);
                destinationArray[0] = Constant(binder);
                destinationArray[destinationArray.Length - 1] = expression;
                DynamicMetaObject errorSuggestion = new DynamicMetaObject(expression, BindingRestrictions.Empty);
                obj4 = new DynamicMetaObject(exp1, obj2.Restrictions);

                return fallback(obj4);
            }

            /*  private DynamicMetaObject CallMethodWithResult(string methodName, DynamicMetaObjectBinder binder, Expression[] args, Fallback fallback, Fallback fallbackInvoke)
              {
                  DynamicMetaObject obj2 = fallback(null);
                  ParameterExpression expression = Expression.Parameter(typeof(object), null);
                  Expression[] destinationArray = new Expression[args.Length + 2];
                  Array.Copy(args, 0, destinationArray, 1, args.Length);
                  destinationArray[0] = Constant(binder);
                  destinationArray[destinationArray.Length - 1] = expression;
                  DynamicMetaObject errorSuggestion = new DynamicMetaObject(expression, BindingRestrictions.Empty);
                  if (fallbackInvoke != null)
                  {
                      errorSuggestion = fallbackInvoke(errorSuggestion);
                  }
                  DynamicMetaObject obj4 = new DynamicMetaObject(Expression.Block(new ParameterExpression[] { expression }, new Expression[] { Expression.Condition(Expression.Call(this.GetLimitedSelf(), typeof(DynamicObject).GetMethod(methodName), destinationArray), errorSuggestion.Expression, DynamicMetaObjectBinder.Convert(obj2.Expression, typeof(object))) }), this.GetRestrictions().Merge(errorSuggestion.Restrictions).Merge(obj2.Restrictions));
                  return fallback(obj4);
              }*/






            internal static Expression Convert(Expression expression, Type type)
            {
                if (expression.Type == type)
                {
                    return expression;
                }
                if (type == typeof(void))
                {
                    return Expression.Block(typeof(void), new Expression[] { expression });
                }
                if (expression.Type == typeof(void))
                {
                    return Expression.Block(expression, Expression.Default(type));
                }
                return Expression.Convert(expression, type);
            }




            private BindingRestrictions GetRestrictions(Expression exp)
            {
                return BindingRestrictions.GetTypeRestriction(exp, typeof(string));
            }




            private static ConstantExpression Constant(DynamicMetaObjectBinder binder)
            {
                Type baseType = binder.GetType();
                while (!baseType.IsVisible)
                {
                    baseType = baseType.BaseType;
                }
                return Expression.Constant(binder, baseType);
            }



        }
        public class DynamicString : IDynamicMetaObjectProvider
        {
            private string p;

            public DynamicString(string p)
            {
                // TODO: Complete member initialization
                this.p = p;
            }
            
            public DynamicMetaObject GetMetaObject(Expression parameter)
            {
                return new MyDynamicMetaObject(parameter, this);
            }
        }

    public class CLOS_Array : CLOSClass
    {
        public override bool Is(object obj)
        {
            if (obj is List<object>)
                return true;
            if (obj is Array)
                return true;
            else if (obj is IList)
                return true;
            else
            {
                var type = obj.GetType();
                return type.GetGenericTypeDefinition() == typeof(IList<>);
            }
        }
    }

    public sealed class CLOS_String : CLOSClass
    {
        private CLOS_String()
        {
            Bases.Add(CLOS_Sequence.Instance);
            _name = DefinedSymbols.String;
        }

        static CLOS_String instance;

        public static CLOS_String Instance
        {
            get { return CLOS_String.instance; }
        }

       static CLOS_String()
       {
           instance = new CLOS_String();
       }


        public override bool Is(object obj)
        { 
            if (obj is string)
                return true;
            if(obj is List<char>)
                return true;
            if (obj is char[])
                return true;

            return false;
        }

        public override CLOSInstance  The(object obj)
        {   
            if (!Is(obj))
            {
                ConditionsDictionary.TypeError(obj + " is not a sequence");
            }

            if (obj is string)
            {
                return new StringWrapper(obj as string);
            }

            if (obj is char[])
            {

            }

            if (obj is List<char>)
            {

            }
            
 	        return base.The(obj);
        }

        internal override DynamicMetaObject GetDynamicMetaObject(System.Linq.Expressions.Expression parameter, CLOSInstance instance)
        {
           // if (instance is StringWrapper)
           // {
            //    return new StringDynamicWrapper(instance as StringWrapper).GetMetaObject(parameter);
           // }

            return base.GetDynamicMetaObject(parameter, instance);
        }
    }
}

namespace LiveLisp.Core
{
    partial class Initialization
    {
        static void RegisterCLOSClasses()
        {

        }
    }

    partial class DefinedSymbols
    {
        public static Symbol Sequence;
        public static Symbol String;

        internal static void InitCLOSSymbols(Package cl)
        {
            Sequence = cl.Intern("SEQUENCE", true);
            String = cl.Intern("STRING", true);
        }
    }
}
