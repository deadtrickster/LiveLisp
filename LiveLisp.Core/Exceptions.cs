using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.CLOS;
using LiveLisp.Core.BuiltIns.Conditions;
using LiveLisp.Core.Types;
using System.Reflection;
using LiveLisp.Core.AST;

namespace LiveLisp.Core
{

    #region FormattedException
#if !SILVERLIGHT
    [global::System.Serializable]
#endif
    public class FormattedException : Exception
    {
        public FormattedException() { }
        public FormattedException(string template, object arg0)
            : base(string.Format(template, arg0)) { }
        public FormattedException(string template, object arg0, object arg1)
            : base(string.Format(template, arg0, arg1)) { }


        public FormattedException(string template, object arg0, object arg1, object arg2)
            : base(string.Format(template, arg0, arg1, arg2))
        {
        }

        public FormattedException(string template, params object[] args)
            : base(string.Format(template, args))
        {

        }
        public FormattedException(string message) : base(message) { }
        public FormattedException(string message, Exception inner) : base(message, inner) { }
#if !SILVERLIGHT
        protected FormattedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
#endif
    }
    #endregion // FormattedException

    #region LiveLispCondition
#if !SILVERLIGHT
    [global::System.Serializable]
#endif
    public class LiveLispCondition : FormattedException
    {
        LispCondition condition;

        public LispCondition Condition
        {
            get { return condition; }
            set { condition = value; }
        }

        public LiveLispCondition(LispCondition condition)
        {
            this.condition = condition;
        }
    }
    #endregion // LiveLispException

    #region ASTException
    #if !SILVERLIGHT
    [global::System.Serializable]
#endif
    public class ASTException : FormattedException
    {
        public ASTException() { }
        public ASTException(string template, object arg0)
            : base(string.Format(template, arg0)) { }
        public ASTException(string template, object arg0, object arg1)
            : base(string.Format(template, arg0, arg1)) { }


        public ASTException(string template, object arg0, object arg1, object arg2)
            : base(string.Format(template, arg0, arg1, arg2))
        {
        }

        public ASTException(string template, params object[] args)
            : base(string.Format(template, args))
        {

        }
        public ASTException(string message) : base(message) { }
        public ASTException(string message, Exception inner) : base(message, inner) { }
    #if !SILVERLIGHT
        protected ASTException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
#endif
    }
    #endregion // ASTException

    #region CompilerException
    #if !SILVERLIGHT
    [global::System.Serializable]
#endif
    public class CompilerException : FormattedException
    {
        public CompilerException() { }
        public CompilerException(string template, object arg0)
            : base(string.Format(template, arg0)) { }
        public CompilerException(string template, object arg0, object arg1)
            : base(string.Format(template, arg0, arg1)) { }


        public CompilerException(string template, object arg0, object arg1, object arg2)
            : base(string.Format(template, arg0, arg1, arg2))
        {
        }


        public CompilerException(string template, params object[] args)
            : base(string.Format(template, args))
        {

        }
        public CompilerException(string message) : base(message) { }
        public CompilerException(string message, Exception inner) : base(message, inner) { }
    #if !SILVERLIGHT
        protected CompilerException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
#endif
    }
    #endregion // CompilerException

    #region SimpleTypeException
    #if !SILVERLIGHT
    [global::System.Serializable]
#endif
    public class SimpleTypeException : FormattedException
    {
        public SimpleTypeException() { }
        public SimpleTypeException(string template, object arg0)
            : base(string.Format(template, arg0)) { }
        public SimpleTypeException(string template, object arg0, object arg1)
            : base(string.Format(template, arg0, arg1)) { }


        public SimpleTypeException(string template, object arg0, object arg1, object arg2)
            : base(string.Format(template, arg0, arg1, arg2))
        {
        }

        public SimpleTypeException(string template, params object[] args)
            : base(string.Format(template, args))
        {

        }
        public SimpleTypeException(string message) : base(message) { }
        public SimpleTypeException(string message, Exception inner) : base(message, inner) { }
    #if !SILVERLIGHT
        protected SimpleTypeException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
#endif
    }
    #endregion // SimpleTypeException

    #region UnboundVariableException
    #if !SILVERLIGHT
    [global::System.Serializable]
#endif
    public class UnboundVariableException : FormattedException
    {
        public UnboundVariableException() { }
        public UnboundVariableException(string template, object arg0)
            : base(string.Format(template, arg0)) { }
        public UnboundVariableException(string template, object arg0, object arg1)
            : base(string.Format(template, arg0, arg1)) { }


        public UnboundVariableException(string template, object arg0, object arg1, object arg2)
            : base(string.Format(template, arg0, arg1, arg2))
        {
        }

        public UnboundVariableException(string template, params object[] args)
            : base(string.Format(template, args))
        {

        }
        public UnboundVariableException(string message) : base(message) { }
        public UnboundVariableException(string message, Exception inner) : base(message, inner) { }
    #if !SILVERLIGHT
        protected UnboundVariableException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
#endif
    }
    #endregion // UnboundVariableException


    #region UnboundFunctionException
    #if !SILVERLIGHT
    [global::System.Serializable]
#endif
    public class UnboundFunctionException : FormattedException
    {
        public UnboundFunctionException() { }
        public UnboundFunctionException(string template, object arg0)
            : base(string.Format(template, arg0)) { }
        public UnboundFunctionException(string template, object arg0, object arg1)
            : base(string.Format(template, arg0, arg1)) { }


        public UnboundFunctionException(string template, object arg0, object arg1, object arg2)
            : base(string.Format(template, arg0, arg1, arg2))
        {
        }

        public UnboundFunctionException(string template, params object[] args)
            : base(string.Format(template, args))
        {

        }
        public UnboundFunctionException(string message) : base(message) { }
        public UnboundFunctionException(string message, Exception inner) : base(message, inner) { }
    #if !SILVERLIGHT
        protected UnboundFunctionException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
#endif
    }
    #endregion // UnboundFunctionException


    #region SimpleErrorException
   #if !SILVERLIGHT
    [global::System.Serializable]
#endif
    public class SimpleErrorException : FormattedException
    {
        public SimpleErrorException() { }
        public SimpleErrorException(string template, object arg0)
            : base(string.Format(template, arg0)) { }
        public SimpleErrorException(string template, object arg0, object arg1)
            : base(string.Format(template, arg0, arg1)) { }


        public SimpleErrorException(string template, object arg0, object arg1, object arg2)
            : base(string.Format(template, arg0, arg1, arg2))
        {
        }

        public SimpleErrorException(string template, params object[] args)
            : base(string.Format(template, args))
        {

        }
        public SimpleErrorException(string message) : base(message) { }
        public SimpleErrorException(string message, Exception inner) : base(message, inner) { }
    #if !SILVERLIGHT
        protected SimpleErrorException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
#endif
    }
    #endregion // SimpleErrorException

    #region ReaderErrorException
    #if !SILVERLIGHT
    [global::System.Serializable]
#endif
    public class ReaderErrorException : FormattedException
    {
        bool _recoverable = true;
        public bool Recoverable
        {
            get
            {
                return _recoverable;
            }
        }
        public ReaderErrorException(string message) : base(message) { }

        public ReaderErrorException(bool recoverable, string message)
            : base(message) { _recoverable = recoverable; }

        public ReaderErrorException(bool recoverable, string template, object arg0)
            : base(string.Format(template, arg0)) { _recoverable = recoverable; }

        public ReaderErrorException(string template, object arg0)
            : base(string.Format(template, arg0)) {}

          public ReaderErrorException(string template, object arg0, object arg1)
            : base(string.Format(template, arg0, arg1)) { }


        public ReaderErrorException(string template, object arg0, object arg1, object arg2)
            : base(string.Format(template, arg0, arg1, arg2))
        {
        }

        public ReaderErrorException(string template, params object[] args)
            : base(string.Format(template, args))
        {

        }
    }
    #endregion // SyntaxException

    #region TooFewArgsException
    #if !SILVERLIGHT
    [global::System.Serializable]
#endif
    public class TooFewArgsException : FormattedException
    {
        public TooFewArgsException() { }
        public TooFewArgsException(string template, object arg0)
            : base(string.Format(template, arg0)) { }
        public TooFewArgsException(string template, object arg0, object arg1)
            : base(string.Format(template, arg0, arg1)) { }


        public TooFewArgsException(string template, object arg0, object arg1, object arg2)
            : base(string.Format(template, arg0, arg1, arg2))
        {
        }

        public TooFewArgsException(string template, params object[] args)
            : base(string.Format(template, args))
        {

        }
        public TooFewArgsException(string message) : base(message) { }
        public TooFewArgsException(string message, Exception inner) : base(message, inner) { }
    #if !SILVERLIGHT
        protected TooFewArgsException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
#endif

        public TooFewArgsException(string function_name, int actual_args_count, int required_min_args_count)
            : base(string.Format("{0}: too few args {1}, minimum required {2}", function_name, actual_args_count, required_min_args_count))
        {

        }
    }
    #endregion // TooFewArgsException

    #region TooManyArgsException
    #if !SILVERLIGHT
    [global::System.Serializable]
#endif
    public class TooManyArgsException : FormattedException
    {
        public TooManyArgsException() { }
        public TooManyArgsException(string template, object arg0)
            : base(string.Format(template, arg0)) { }
        public TooManyArgsException(string template, object arg0, object arg1)
            : base(string.Format(template, arg0, arg1)) { }


        public TooManyArgsException(string template, object arg0, object arg1, object arg2)
            : base(string.Format(template, arg0, arg1, arg2))
        {
        }

        public TooManyArgsException(string template, params object[] args)
            : base(string.Format(template, args))
        {

        }
        public TooManyArgsException(string message) : base(message) { }
        public TooManyArgsException(string message, Exception inner) : base(message, inner) { }
     #if !SILVERLIGHT
    protected TooManyArgsException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
#endif

        public TooManyArgsException(string function_name, int actual_args_count, int max_expected_args_count)
            : base(string.Format("{0}: too many args {1}, maximum expected {2}", function_name, actual_args_count, max_expected_args_count))
        {

        }
    }
    #endregion // TooManyArgsException


    #region ConstantException
    #if !SILVERLIGHT
    [global::System.Serializable]
#endif
    public class ConstantException : SimpleErrorException
    {
        public ConstantException() { }
        public ConstantException(string template, object arg0)
            : base(string.Format(template, arg0)) { }
        public ConstantException(string template, object arg0, object arg1)
            : base(string.Format(template, arg0, arg1)) { }


        public ConstantException(string template, object arg0, object arg1, object arg2)
            : base(string.Format(template, arg0, arg1, arg2))
        {
        }

        public ConstantException(string template, params object[] args)
            : base(string.Format(template, args))
        {

        }
        public ConstantException(string message) : base(message) { }
        public ConstantException(string message, Exception inner) : base(message, inner) { }
    #if !SILVERLIGHT
        protected ConstantException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
#endif
    }
    #endregion // ConstantException


    #region ReadErrorException
    #if !SILVERLIGHT
    [global::System.Serializable]
#endif
    public class ReadErrorException : FormattedException
    {
        public ReadErrorException() { }
        public ReadErrorException(string template, object arg0)
            : base(string.Format(template, arg0)) { }
        public ReadErrorException(string template, object arg0, object arg1)
            : base(string.Format(template, arg0, arg1)) { }


        public ReadErrorException(string template, object arg0, object arg1, object arg2)
            : base(string.Format(template, arg0, arg1, arg2))
        {
        }

        public ReadErrorException(string template, params object[] args)
            : base(string.Format(template, args))
        {

        }
        public ReadErrorException(string message) : base(message) { }
        public ReadErrorException(string message, Exception inner) : base(message, inner) { }
    #if !SILVERLIGHT
        protected ReadErrorException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
#endif
    }
    #endregion // ReadErrorException




    #region SyntaxErrorException
    #if !SILVERLIGHT
    [global::System.Serializable]
#endif
    public class SyntaxErrorException : FormattedException
    {
        public SyntaxErrorException() { }
        public SyntaxErrorException(string template, object arg0)
            : base(string.Format(template, arg0)) { }
        public SyntaxErrorException(string template, object arg0, object arg1)
            : base(string.Format(template, arg0, arg1)) { }


        public SyntaxErrorException(string template, object arg0, object arg1, object arg2)
            : base(string.Format(template, arg0, arg1, arg2))
        {
        }

        public SyntaxErrorException(string template, params object[] args)
            : base(string.Format(template, args))
        {

        }
        public SyntaxErrorException(string message) : base(message) { }
        public SyntaxErrorException(string message, Exception inner) : base(message, inner) { }
    #if !SILVERLIGHT
        protected SyntaxErrorException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
#endif
    }
    #endregion // SyntaxErrorException



    #region TypeErrorException
    #if !SILVERLIGHT
    [global::System.Serializable]
#endif
    public class TypeErrorException : FormattedException
    {
        public TypeErrorException() { }
        public TypeErrorException(string template, object arg0)
            : base(string.Format(template, arg0)) { }
        public TypeErrorException(string template, object arg0, object arg1)
            : base(string.Format(template, arg0, arg1)) { }


        public TypeErrorException(string template, object arg0, object arg1, object arg2)
            : base(string.Format(template, arg0, arg1, arg2))
        {
        }

        public TypeErrorException(string template, params object[] args)
            : base(string.Format(template, args))
        {

        }
        public TypeErrorException(string message) : base(message) { }
        public TypeErrorException(string message, Exception inner) : base(message, inner) { }
    #if !SILVERLIGHT
        protected TypeErrorException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
#endif
    }
    #endregion // TypeErrorException


    #if !SILVERLIGHT
    [global::System.Serializable]
#endif
    public class BlockNonLocalTransfer : Exception
    {

        public BlockNonLocalTransfer(Guid code, Symbol name, Object value) { TagId = code; Name = name; Result = value; }

        public BlockNonLocalTransfer(Guid code, Symbol name) { TagId = code; Name = name;}

        public Guid TagId;

        public Symbol Name;

        public object Result;

        public static FieldInfo TagIdField = typeof(BlockNonLocalTransfer).GetField("TagId");

        public static FieldInfo NameField = typeof(BlockNonLocalTransfer).GetField("Name");

        public static StaticFieldResolver ResultField = typeof(BlockNonLocalTransfer).GetField("Result");

        public static StaticConstructorResolver NonVoidBlockNonLocalTransferConstructor = typeof(BlockNonLocalTransfer).GetConstructor(new Type[] { typeof(Guid), typeof(Symbol), typeof(Object) });
        public static StaticConstructorResolver VoidBlockNonLocalTransferConstructor = typeof(BlockNonLocalTransfer).GetConstructor(new Type[] { typeof(Guid), typeof(Symbol)});
    }



    #region CatchThrowException
    #if !SILVERLIGHT
    [global::System.Serializable]
#endif
    public class CatchThrowException : FormattedException
    {
        public object Tag;
        public object Result;

        public CatchThrowException(object tag, object result) { Tag = tag; Result = result; }

        public static StaticFieldResolver TagField = typeof(CatchThrowException).GetField("Tag");
        public static StaticFieldResolver ResultField = typeof(CatchThrowException).GetField("Result");
        public static StaticConstructorResolver CatchThrowExceptionConstructor = typeof(CatchThrowException).GetConstructor(new Type[]{typeof(object), typeof(object)});
    }
    #endregion // CatchThrowException

#if !SILVERLIGHT
    [global::System.Serializable]
#endif
    public class TagBodyNonLocalTransfer : Exception
    {

        public TagBodyNonLocalTransfer(Guid id, object tagname, int tagid) { Id = id; Tagname = tagname; TagId = tagid; }

        public Guid Id;
        public readonly Object Tagname;
        public readonly int TagId;

        public static FieldInfo TagIdField = typeof(TagBodyNonLocalTransfer).GetField("TagId");

        public static FieldInfo TagnameField = typeof(TagBodyNonLocalTransfer).GetField("Tagname");

        public static StaticFieldResolver IdField = typeof(TagBodyNonLocalTransfer).GetField("Id");

        public static StaticConstructorResolver Constructor = typeof(TagBodyNonLocalTransfer).GetConstructor(new Type[] { typeof(Guid), typeof(object), typeof(int) });

    }


    #region OperatorNotFoundException
    [global::System.Serializable]
    public class OperatorNotFoundException : FormattedException
    {
        public OperatorNotFoundException() { }
        public OperatorNotFoundException(string template, object arg0)
            : base(string.Format(template, arg0)) { }
        public OperatorNotFoundException(string template, object arg0, object arg1)
            : base(string.Format(template, arg0, arg1)) { }


        public OperatorNotFoundException(string template, object arg0, object arg1, object arg2)
            : base(string.Format(template, arg0, arg1, arg2))
        {
        }

        public OperatorNotFoundException(string template, params object[] args)
            : base(string.Format(template, args))
        {

        }
        public OperatorNotFoundException(string message) : base(message) { }
        public OperatorNotFoundException(string message, Exception inner) : base(message, inner) { }
        protected OperatorNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
    #endregion // OperatorNotFoundException

}
