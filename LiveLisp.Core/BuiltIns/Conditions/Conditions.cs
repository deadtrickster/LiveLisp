using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.CLOS;
using LiveLisp.Core.BuiltIns.Strings;

namespace LiveLisp.Core.BuiltIns.Conditions
{

    public enum StandardConditions
    {
        ArithmeticError,
        CellError,
        ControlError,
        DivisionByZero,
        EndOfFile,
        Error,
        FileError,
        FloatingPointInexact,
        FloatingPointInvalidOperation,
        FloatingPointOverflow,
        FloatingPointUnderflow,
        PackageError,
        ParseError,
        PrintNotReadable,
        ProgramError,
        SeriousCondition,
        SimpleCondition,
        SimpleError,
        SimpleTypeError,
        SimpleWarning,
        StorageCondition,
        StreamError,
        StyleWarning,
        TypeError,
        UnboundSlot,
        UnboundVariable,
        UndefinedFunction,
        Warning,

        UserCondition
    }

    public interface ILispCondition
    {

    }

    public interface ISimpleCondition
    {
        [CLOSSlotNameAttribute("FORMAT-CONTROL")]
        string FormatControl
        {
            get;
            set;
        }

        [CLOSSlotNameAttribute("FORMAT-ARGUMENTS")]
        object[] FormatArguments
        {
            get;
            set;
        }
    }


    public abstract class LispCondition : CLRCLOSInstance, ILispCondition
    {
        public abstract StandardConditions CondititonType
        {
            get;
        }

        public static CLOSClass Condition;
        public static CLOSClass SeriousCondition;
        public static CLOSClass Error;
        public static CLOSClass SimpleCondition;
        public static CLOSClass SimpleError;
        public static CLOSClass TypeError;
        public static CLOSClass CellError;
        public static CLOSClass UnboundVarable;

        static LispCondition()
        {
            Condition = CLOSCLRClass.ConstructClass(typeof(LispCondition));
            SeriousCondition = CLOSCLRClass.ConstructClass(typeof(SeriousCondition));
            SeriousCondition.Bases.Add(Condition);

            Error = CLOSCLRClass.ConstructClass(typeof(Error));
            Error.Bases.Add(SeriousCondition);

            SimpleCondition = CLOSCLRClass.ConstructClass(typeof(ISimpleCondition));
            SimpleCondition.Bases.Add(Condition);

            SimpleError = CLOSCLRClass.ConstructClass(typeof(SimpleError));
            SimpleError.Bases.Add(Error);
            SimpleError.Bases.Add(SimpleCondition);

            CellError = CLOSCLRClass.ConstructClass(typeof(CellError));
            CellError.Bases.Add(Error);


            UnboundVarable = CLOSCLRClass.ConstructClass(typeof(UnboundVariable));
            CellError.Bases.Add(CellError);
        }
    }



    #region standard conditions



    public class SeriousCondition : LispCondition
    {

        public override StandardConditions CondititonType
        {
            get { throw new NotImplementedException(); }
        }
    }

    public class Error : SeriousCondition
    {

    }

    public class ArithmeticError : Error
    {

    }

    public class CellError : Error
    {
        dynamic _CellName;

        public dynamic CellName
        {
            get { return _CellName; }
            set { _CellName = value; }
        }
    }

    public class ControlError : Error
    {

    }

    public class DivisionByZero : ArithmeticError
    {

    }

    public class EndOfFile : StreamError
    {

    }

    public class FileError : Error
    {

    }

    public class FloatingPointInexact : ArithmeticError
    {

    }

    public class FloatingPointInvalidOperation : ArithmeticError
    {
    }

    public class FloatingPointOverflow : ArithmeticError
    {

    }

    public class FloatingPointUnderflow : ArithmeticError
    {

    }

    public class PackageError : Error
    {

    }

    public class ParseError : Error
    {

    }

    public class PrintNotReadable : Error
    {

    }

    public class ProgramError : Error
    {

    }

    public class SimpleCondition : LispCondition, ISimpleCondition
    {

        public override StandardConditions CondititonType
        {
            get { throw new NotImplementedException(); }
        }

        #region ISimpleCondition Members

        public string FormatControl
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

        public object[] FormatArguments
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

        #endregion

        public override string ToString()
        {
            return "SIMPLE-CONDITION: " + string.Format(FormatControl, FormatArguments) + "\r\nCondition object " + this.DisplayID;
        }
    }

    public class SimpleError : Error, ISimpleCondition
    {

        #region ISimpleCondition Members
        [CLOSSlotNameAttribute("FORMAT-CONTROL")]
        public string FormatControl
        {
            get;
            set;
        }
        [CLOSSlotNameAttribute("FORMAT-ARGUMENTS")]
        public object[] FormatArguments
        {
            get;
            set;
        }

        #endregion


        public override string ToString()
        {
            return "SIMPLE-ERROR: " + string.Format(System.Globalization.CultureInfo.InvariantCulture, FormatControl, FormatArguments) + "\r\nCondition object " + this.DisplayID;
        }
    }

    public class TypeError : SeriousCondition
    {

    }

    public class SimpleTypeError : TypeError, ISimpleCondition
    {

        #region ISimpleCondition Members

        public string FormatControl
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

        public object[] FormatArguments
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

        #endregion
    }

    public class SimpleWarning : Warning, ISimpleCondition
    {

        #region ISimpleCondition Members

        public string FormatControl
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

        public object[] FormatArguments
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

        #endregion
    }

    public class StorageCondition : SeriousCondition
    {

    }

    public class StreamError : Error
    {

    }


    public class UnboundSlot : CellError
    {

    }

    public class UnboundVariable : CellError
    {
        public UnboundVariable(LiveLisp.Core.Types.Symbol symbol)
        {
            // TODO: Complete member initialization
            base.CellName = symbol;
        }


        public override string ToString()
        {
            return "variable " + CellName + " has no value";
        }
    }

    public class UndefinedFunction : Error
    {

    }

    public class Warning : LispCondition
    {
        public override StandardConditions CondititonType
        {
            get { return StandardConditions.Warning; }
        }
    }

    #endregion

}
