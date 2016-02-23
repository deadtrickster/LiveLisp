using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using LiveLisp.Core.AST.Expressions;

namespace LiveLisp.Core.AST
{
    public enum CallOpcode
    {
        Call,
        Calli,
        Callvirt
    }

    public class MethodInfoFunctionDesignator : FunctionNameDesignator
    {
        private StaticMethodResolver _method;
        private CallOpcode _Opcode;

        public CallOpcode Opcode
        {
            get { return _Opcode; }
        }
        private CallingConventions? _CallingConvention;

        public CallingConventions? CallingConvention
        {
            get { return _CallingConvention; }
        }

        private bool _UnboxFromLisp;

        public bool UnboxFromLisp
        {
            get { return _UnboxFromLisp; }
        }

        public MethodInfoFunctionDesignator(CallOpcode opcode, CallingConventions callingConvention, StaticMethodResolver method, bool unboxFromLisp, ExpressionContext context)
            : base(context)
        {
            this._method = method;
            _Opcode = opcode;
            _CallingConvention = callingConvention;
            _UnboxFromLisp = unboxFromLisp;
        }

        public MethodInfoFunctionDesignator(StaticMethodResolver method, ExpressionContext context)
            : base(context)
        {
            _Opcode = CallOpcode.Call;
            this._method = method;
        }

        public override void Visit(IASTWalker visitor, ExpressionContext context)
        {
            visitor.MethodInfoFunctionDesignator(this, context);
        }

        public override void Visit(IASTWalker visiter, CallExpression call, ExpressionContext context)
        {
            visiter.EmitMethodInfoDesignatorCall(this, call, context);
        }

        public StaticMethodResolver Method
        {
            get
            {
                return this._method;
            }
        }
    }
}
