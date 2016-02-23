using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.AST;
using LiveLisp.Core.Types;
using LiveLisp.Core.AST.Expressions;
using LiveLisp.Core.Compiler;
using LiveLisp.Core.AST.Expressions.CLR;
using System.Reflection.Emit;

namespace LiveLisp.Core.Eval
{
    public partial class LispParser
    {
        public static Expression ParseExpression(object form, ExpressionContext context)
        {
            context = new ExpressionContext(context);
            Cons cons = form as Cons;

            if (cons != null)
            {
                return Parselist(cons, context);
            }
            KeywordSymbol keyword = form as KeywordSymbol;
            if (keyword != null)
            {
                return new ConstantExpression(keyword, context);
            }

            Symbol symbol = form as Symbol;
            if (symbol != null)
            {
                switch (symbol.Id)
                {
                    case 31: //nil
                    case 32: //t
                        return new ConstantExpression(symbol, context);
                    default:
                        break;
                }
                return new VariableExpression(symbol, context);
            }

            return new ConstantExpression(form, context);
        }

        private static Expression Parselist(Cons cons, ExpressionContext context)
        {
            Symbol car = cons.Car as Symbol;

            if (car != null)
            {
                return Dispatcher(car, cons.Cdr, context);
            }

            Cons possiblyLambda = cons.Car as Cons;

            if (possiblyLambda != null)
            {
                if(possiblyLambda.Car == DefinedSymbols.Function)
                    return new CallExpression(ParseFunction(possiblyLambda.Child, context).Designator, BuildBody(cons.Child, new ExpressionContext(context), "call"), context);
            }

            throw new SyntaxErrorException("invalid form " + cons);
        }

        private static Expression Dispatcher(Symbol car, object args, ExpressionContext context)
        {
            Cons rest = args as Cons;

            if (rest == null)
            {
                if (args == DefinedSymbols.NIL)
                    rest = new SurrogateNilCons();  // TODO: mark SurrogateNILCons as obsolete
                else
                    throw new SyntaxErrorException("dotted parameter list is invalid");
            }
            /*
             *  block      let*                  return-from      
                catch      load-time-value       setq             
                eval-when  locally               symbol-macrolet  
                flet       macrolet              tagbody          
                function   multiple-value-call   the              
                go         multiple-value-prog1  throw            
                if         progn                 unwind-protect   
                labels     progv                                  
                let        quote               
             */
            switch (car.Id)
            {
                case 0:
                    return ParseBlock(rest, context);
                case 1:
                    return ParseCatch(rest, context);
                case 2:
                    return ParseEvalWhen(rest, context);
                case 3:
                    return ParseFlet(rest, context);
                case 4:
                    return ParseFunction(rest, context);
                case 5:
                    return ParseGo(rest, context);
                case 6:
                    return ParseIf(rest, context);
                case 7:
                    return ParseLabels(rest, context);
                case 8:
                    return ParseLet(rest, context);
                case 9:
                    return ParseLetStar(rest, context);
                case 10:
                    return ParseLoadTimeValue(rest, context);
                case 11:
                    return ParseLocally(rest, context);
                case 12:
                    return ParseMacrolet(rest, context);
                case 13:
                    return ParseMultipleValueCall(rest, context);
                case 14:
                    return ParseMultipleValueProg1(rest, context);
                case 15:
                    return ParseProgn(rest, context);
                case 16:
                    return ParseProgv(rest, context);
                case 17:
                    return ParseQuote(rest, context);
                case 18:
                    return ParseReturnFrom(rest, context);
                case 19:
                    return ParseSetq(rest, context);
                case 20:
                    return ParseSymbolMacrolet(rest, context);
                case 21:
                    return ParseTagbody(rest, context);
                case 22:
                    return ParseThe(rest, context);
                case 23:
                    return ParseThrow(rest, context);
                case 24:
                    return ParseUnwindProtect(rest, context);
                case 25:
                    return ParseILCode(rest, context);
                case 26:
                    return ParseClrTry(rest, context);
                case 27:
                    return ParseClrClass(rest, context);
                case 28:
                    return ParseClrDelegate(rest, context);
                case 29:
                    return ParseClrEnum(rest, context);
                case 30: 
                    return ParseClrTypeExpression(rest, context);
                default:
                    // non special form
                    object expanded;
                    if (TryMacroExpansion(car, context, rest, out expanded))
                        return ParseExpression(expanded, context);
                    else return ParseCallExpression(car, rest, context);
            }
        }

        internal static bool TryMacroExpansion(Symbol car, ExpressionContext context, Cons rest, out object expanded)
        {
           
            expanded = null;
            LispFunction macrofunc;
            if (!context.GetMacro(car, out macrofunc))
                return false;
            else
                expanded = macrofunc.Invoke(new Cons(car, (rest.Count == 0 ? DefinedSymbols.NIL : (object)rest)), context);
            return true;
            
            throw new NotImplementedException();
        }

        private static Expression ParseCallExpression(Symbol car, Cons rest, ExpressionContext context)
        {
            var forms = BuildBody(rest, context, "call");

            return new CallExpression(new SymbolFunctionDesignator(car, context), forms, context);
        }

        /// <summary>
        /// (defun clr-add(arg1, arg2)
        /// (declare (type int arg1 arg2))  // если есть такая декларация, то в прологе метода осуществляется проверка типов.
        ///  (let (square-minus-5)
        ///     (ilcode
        ///         :load __typed_arg1      // инструкцией isinst т.к. всё равно лучьше для таких вещей создавать доп слоты 
        ///         :load __typed_arg2      // то можно их и использовать
        ///         :add
        ///         :local square int   // создаёт новую переменную в ближайшей области видимости
        ///         :dup
        ///         :mul
        ///         :set squate
        ///         :load square
        ///         :ldint 5
        ///         :sub
        ///         :box
        ///         :set square-minus-5
        ///         :load square-minus-5))) // square-minus-5 объявлена в let, поэтому физически
        ///                                  тип слота - object. соответственно автоматически эмитируется боксинг
        ///                                  а так, как это выражение последнее и не оставляет после себя ничего в стеке
        ///                                  то в добавок ещё эмитируется :load square-minus-5
        /// </summary>
        /// <param name="rest"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private static Expression ParseILCode(Cons rest, ExpressionContext context)
        {
            Symbol op_code_sym = rest.Car as Symbol;

            if (op_code_sym == null)
                throw new SyntaxErrorException("IL: opcode name is not a symbol " + rest.Car);

            ILInstruction instruction = null;
            object cdr = rest.Cdr;
            #region cases
            switch (op_code_sym.Id)
            {
                case 33:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new AddInstruction();
                    }
                    break;
                case 34:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new AddOvfInstruction();
                    }
                    break;
                case 35:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new AddOvfUnInstruction();
                    }
                    break;
                case 36:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new AndInstruction();
                    }
                    break;
                case 37:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new ArglistInstruction();
                    }
                    break;
                case 38:
                    {
                        Symbol label = ILParser.ParseLabel(cdr);
                        instruction = new BeqInstruction(label.Name);
                    }
                    break;
                case 39:
                    {
                        Symbol label = ILParser.ParseLabel(cdr);
                        instruction = new BgeInstruction(label.Name);
                    }
                    break;
                case 40:
                    {
                        Symbol label = ILParser.ParseLabel(cdr);
                        instruction = new BgeUnInstruction(label.Name);
                    }
                    break;
                case 41:
                    {
                        Symbol label = ILParser.ParseLabel(cdr);
                        instruction = new BgtInstruction(label.Name);
                    }
                    break;
                case 42:
                    {
                        Symbol label = ILParser.ParseLabel(cdr);
                        instruction = new BgtUnInstruction(label.Name);
                    }
                    break;
                case 43:
                    {
                        Symbol label = ILParser.ParseLabel(cdr);
                        instruction = new BleInstruction(label.Name);
                    }
                    break;
                case 44:
                    {
                        Symbol label = ILParser.ParseLabel(cdr);
                        instruction = new BleUnInstruction(label.Name);
                    }
                    break;
                case 45:
                    {
                        Symbol label = ILParser.ParseLabel(cdr);
                        instruction = new BltInstruction(label.Name);
                    }
                    break;
                case 46:
                    {
                        Symbol label = ILParser.ParseLabel(cdr);
                        instruction = new BltUnInstruction(label.Name);
                    }
                    break;
                case 47:
                    {
                        Symbol label = ILParser.ParseLabel(cdr);
                        instruction = new BneUnInstruction(label.Name);
                    }
                    break;
                case 48:
                    {
                        StaticTypeResolver typeResolver = ILParser.ParseType(cdr);
                        instruction = new BoxInstruction(typeResolver);
                    }
                    break;
                case 49:
                    {
                        Symbol label = ILParser.ParseLabel(cdr);
                        instruction = new BrInstruction(label.Name);
                    }
                    break;
                case 50:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new BreakInstruction();
                    }
                    break;
                case 51:
                    {
                        Symbol label = ILParser.ParseLabel(cdr);
                        instruction = new BrfalseInstruction(label.Name);
                    }
                    break;
                case 52:
                    {
                        Symbol label = ILParser.ParseLabel(cdr);
                        instruction = new BrtrueInstruction(label.Name);
                    }
                    break;
                case 53:
                    {
                        StaticMethodResolver method = ILParser.ParseMethod(cdr);
                        instruction = new CallInstruction(method);
                    }
                    break;
                case 54:
                    {
                        StaticMethodResolver method = ILParser.ParseMethod(cdr);
                        instruction = new CalliInstruction(method);
                    }
                    break;
                case 55:
                    {
                        StaticMethodResolver method = ILParser.ParseMethod(cdr);
                        instruction = new CallvirtInstruction(method);
                    }
                    break;
                case 56:
                    {
                        StaticTypeResolver type = ILParser.ParseType(cdr);
                        instruction = new CastclassInstruction(type);
                    }
                    break;
                case 57:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new CeqInstruction();
                    }
                    break;
                case 58:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new CgtInstruction();
                    }
                    break;
                case 59:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new CgtUnInstruction();
                    }
                    break;
                case 60:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new CkfiniteInstruction();
                    }
                    break;
                case 61:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new CltInstruction();
                    }
                    break;
                case 62:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Clt_UnInstruction();
                    }
                    break;
                case 63:
                    {
                        StaticTypeResolver type = ILParser.ParseType(cdr);
                        instruction = new ConstrainedInstruction(type);
                    }
                    break;
                case 64:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Conv_IInstruction();
                    }
                    break;
                case 65:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Conv_I1Instruction();
                    }
                    break;
                case 66:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Conv_I2Instruction();
                    }
                    break;
                case 67:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Conv_I4Instruction();
                    }
                    break;
                case 68:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Conv_I8Instruction();
                    }
                    break;
                case 69:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Conv_Ovf_IInstruction();
                    }
                    break;
                case 70:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Conv_Ovf_I_UnInstruction();
                    }
                    break;
                case 71:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Conv_Ovf_I1Instruction();
                    }
                    break;
                case 72:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Conv_Ovf_I1_UnInstruction();
                    }
                    break;
                case 73:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Conv_Ovf_I2Instruction();
                    }
                    break;
                case 74:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Conv_Ovf_I2_UnInstruction();
                    }
                    break;
                case 75:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Conv_Ovf_I4Instruction();
                    }
                    break;
                case 76:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Conv_Ovf_I4_UnInstruction();
                    }
                    break;
                case 77:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Conv_Ovf_I8Instruction();
                    }
                    break;
                case 78:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Conv_Ovf_I8_UnInstruction();
                    }
                    break;
                case 79:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Conv_Ovf_UInstruction();
                    }
                    break;
                case 80:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Conv_Ovf_U_UnInstruction();
                    }
                    break;
                case 81:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Conv_Ovf_U1Instruction();
                    }
                    break;
                case 82:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Conv_Ovf_U1_UnInstruction();
                    }
                    break;
                case 83:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Conv_Ovf_U2Instruction();
                    }
                    break;
                case 84:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Conv_Ovf_U2_UnInstruction();
                    }
                    break;
                case 85:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Conv_Ovf_U4Instruction();
                    }
                    break;
                case 86:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Conv_Ovf_U4_UnInstruction();
                    }
                    break;
                case 87:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Conv_Ovf_U8Instruction();
                    }
                    break;
                case 88:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Conv_Ovf_U8_UnInstruction();
                    }
                    break;
                case 89:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Conv_R_UnInstruction();
                    }
                    break;
                case 90:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Conv_R4Instruction();
                    }
                    break;
                case 91:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Conv_R8Instruction();
                    }
                    break;
                case 92:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Conv_UInstruction();
                    }
                    break;
                case 93:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Conv_U1Instruction();
                    }
                    break;
                case 94:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Conv_U2Instruction();
                    }
                    break;
                case 95:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Conv_U4Instruction();
                    }
                    break;
                case 96:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Conv_U8Instruction();
                    }
                    break;
                case 97:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new CpblkInstruction();
                    }
                    break;
                case 98:
                    {
                        StaticTypeResolver type = ILParser.ParseType(cdr);
                        instruction = new CpobjInstruction(type);
                    }
                    break;
                case 99:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new DivInstruction();
                    }
                    break;
                case 100:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Div_UnInstruction();
                    }
                    break;
                case 101:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new DupInstruction();
                    }
                    break;
                case 102:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new EndfilterInstruction();
                    }
                    break;
                case 103:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new EndfinallyInstruction();
                    }
                    break;
                case 104:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new InitblkInstruction();
                    }
                    break;
                case 105:
                    {
                        StaticTypeResolver type = ILParser.ParseType(cdr);
                        instruction = new InitobjInstruction(type);
                    }
                    break;
                case 106:
                    {
                        StaticTypeResolver type = ILParser.ParseType(cdr);
                        instruction = new IsinstInstruction(type);
                    }
                    break;
                case 107:
                    {
                        StaticMethodResolver method = ILParser.ParseMethod(cdr);
                        instruction = new JmpInstruction(method);
                    }
                    break;
                case 108:
                    {
                        short argnum = ILParser.ParseSlotNum(cdr);
                        instruction = new LdargInstruction(argnum);
                    }
                    break;
                case 109:
                    {
                        short argnum = ILParser.ParseSlotNum(cdr);
                        instruction = new LdargaInstruction(argnum);
                    }
                    break;
                case 110:
                    {
                        int argnum = ILParser.ParseInt32(cdr);
                        instruction = new Ldc_I4Instruction(argnum);
                    }
                    break;
                case 111:
                    {
                        long argnum = ILParser.ParseInt64(cdr);
                        instruction = new Ldc_I8Instruction(argnum);
                    }
                    break;
                case 112:
                    {
                        float argnum = ILParser.ParseSingle(cdr);
                        instruction = new Ldc_R4Instruction(argnum);
                    }
                    break;
                case 113:
                    {
                        double argnum = ILParser.ParseDouble(cdr);
                        instruction = new Ldc_R8Instruction(argnum);
                    }
                    break;
                case 114:
                    {
                        StaticTypeResolver type = ILParser.ParseType(cdr);
                        instruction = new LdelemInstruction(type);
                    }
                    break;
                case 115:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Ldelem_IInstruction();
                    }
                    break;
                case 116:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Ldelem_I1Instruction();
                    }
                    break;
                case 117:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Ldelem_I2Instruction();
                    }
                    break;
                case 118:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Ldelem_I4Instruction();
                    }
                    break;
                case 119:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Ldelem_I8Instruction();
                    }
                    break;
                case 120:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Ldelem_R4Instruction();
                    }
                    break;
                case 121:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Ldelem_R8Instruction();
                    }
                    break;
                case 122:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Ldelem_RefInstruction();
                    }
                    break;
                case 123:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Ldelem_U1Instruction();
                    }
                    break;
                case 124:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Ldelem_U2Instruction();
                    }
                    break;
                case 125:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Ldelem_U4Instruction();
                    }
                    break;
                case 126:
                    {
                        StaticTypeResolver type = ILParser.ParseType(cdr);
                        instruction = new LdelemaInstruction(type);
                    }
                    break;
                case 127:
                    {
                        StaticFieldResolver field = ILParser.ParseField(cdr);
                        instruction = new LdfldInstruction(field);
                    }
                    break;
                case 128:
                    {
                        StaticFieldResolver field = ILParser.ParseField(cdr);
                        instruction = new LdfldaInstruction(field);
                    }
                    break;
                case 129:
                    {
                        StaticMethodResolver method = ILParser.ParseMethod(cdr);
                        instruction = new LdftnInstruction(method);
                    }
                    break;
                case 130:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Ldind_IInstruction();
                    }
                    break;
                case 131:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Ldind_I1Instruction();
                    }
                    break;
                case 132:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Ldind_I2Instruction();
                    }
                    break;
                case 133:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Ldind_I4Instruction();
                    }
                    break;
                case 134:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Ldind_I8Instruction();
                    }
                    break;
                case 135:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Ldind_R4Instruction();
                    }
                    break;
                case 136:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Ldind_R8Instruction();
                    }
                    break;
                case 137:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Ldind_RefInstruction();
                    }
                    break;
                case 138:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Ldind_U1Instruction();
                    }
                    break;
                case 139:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Ldind_U2Instruction();
                    }
                    break;
                case 140:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Ldind_U4Instruction();
                    }
                    break;
                case 141:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new LdlenInstruction();
                    }
                    break;
                case 142:
                    {
                        Nullable<short> loc_slot_num;
                        Symbol slot_name;
                        if (!ILParser.TryParseSlotNum(cdr, out loc_slot_num))
                        {
                            slot_name = ILParser.ParseSlotName(cdr);
                            instruction = new LdlocInstruction(slot_name);
                        }
                        else
                        {
                            instruction = new LdlocInstruction(loc_slot_num.Value);
                        }
                    }
                    break;
                case 143:
                    {
                        Nullable<short> loc_slot_num;
                        Symbol slot_name;
                        if (!ILParser.TryParseSlotNum(cdr, out loc_slot_num))
                        {
                            slot_name = ILParser.ParseSlotName(cdr);
                            instruction = new LdlocaInstruction(slot_name);
                        }
                        else
                        {
                            instruction = new LdlocaInstruction(loc_slot_num.Value);
                        }
                    }
                    break;
                case 144:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new LdnullInstruction();
                    }
                    break;
                case 145:
                    {
                        StaticTypeResolver type = ILParser.ParseType(cdr);
                        instruction = new LdobjInstruction(type);
                    }
                    break;
                case 146:
                    {
                        StaticFieldResolver field = ILParser.ParseField(cdr);
                        instruction = new LdsfldInstruction(field);
                    }
                    break;
                case 147:
                    {
                        StaticFieldResolver field = ILParser.ParseField(cdr);
                        instruction = new LdsfldaInstruction(field);
                    }
                    break;
                case 148:
                    {
                        string str = ILParser.ParseString(cdr);
                        instruction = new LdstrInstruction(str);
                    }
                    break;
                case 149:
                    {
                        throw new NotImplementedException();
                    }
                    //break;
                case 150:
                    {
                        StaticMethodResolver method = ILParser.ParseMethod(cdr);
                        instruction = new LdvirtftnInstruction(method);
                    }
                    break;
                case 151:
                    {
                        Symbol label = ILParser.ParseLabel(cdr);
                        instruction = new LeaveInstruction(label.Name);
                    }
                    break;
                case 152:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new LocallocInstruction();
                    }
                    break;
                case 153:
                    {
                        StaticTypeResolver type = ILParser.ParseType(cdr);
                        instruction = new MkrefanyInstruction(type);
                    }
                    break;
                case 154:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new MulInstruction();
                    }
                    break;
                case 155:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Mul_OvfInstruction();
                    }
                    break;
                case 156:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Mul_Ovf_UnInstruction();
                    }
                    break;
                case 157:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new NegInstruction();
                    }
                    break;
                case 158:
                    {
                        StaticTypeResolver type = ILParser.ParseType(cdr);
                        instruction = new NewarrInstruction(type);
                    }
                    break;
                case 159:
                    {
                        StaticConstructorResolver method = ILParser.ParseConstructor(cdr);
                        instruction = new NewobjInstruction(method);
                    }
                    break;
                case 160:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new NopInstruction();
                    }
                    break;
                case 161:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new NotInstruction();
                    }
                    break;
                case 162:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new OrInstruction();
                    }
                    break;
                case 163:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new PopInstruction();
                    }
                    break;
                case 164:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new ReadonlyInstruction();
                    }
                    break;
                case 165:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new RefanytypeInstruction();
                    }
                    break;
                case 166:
                    {
                        StaticTypeResolver type = ILParser.ParseType(cdr);
                        instruction = new RefanyvalInstruction(type);
                    }
                    break;
                case 167:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new RemInstruction();
                    }
                    break;
                case 168:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Rem_UnInstruction();
                    }
                    break;
                case 169:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new RetInstruction();
                    }
                    break;
                case 170:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new RethrowInstruction();
                    }
                    break;
                case 171:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new ShlInstruction();
                    }
                    break;
                case 172:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new ShrInstruction();
                    }
                    break;
                case 173:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Shr_UnInstruction();
                    }
                    break;
                case 174:
                    {
                        StaticTypeResolver type = ILParser.ParseType(cdr);
                        instruction = new SizeofInstruction(type);
                    }
                    break;
                case 175:
                    {
                        short slot_num = ILParser.ParseSlotNum(cdr);
                        instruction = new StargInstruction(slot_num);
                    }
                    break;
                case 176:
                    {
                        StaticTypeResolver type = ILParser.ParseType(cdr);
                        instruction = new StelemInstruction(type);
                    }
                    break;
                case 177:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Stelem_IInstruction();
                    }
                    break;
                case 178:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Stelem_I1Instruction();
                    }
                    break;
                case 179:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Stelem_I2Instruction();
                    }
                    break;
                case 180:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Stelem_I4Instruction();
                    }
                    break;
                case 181:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Stelem_I8Instruction();
                    }
                    break;
                case 182:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Stelem_R4Instruction();
                    }
                    break;
                case 183:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Stelem_R8Instruction();
                    }
                    break;
                case 184:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Stelem_RefInstruction();
                    }
                    break;
                case 185:
                    {
                        StaticFieldResolver field = ILParser.ParseField(cdr);
                        instruction = new StfldInstruction(field);
                    }
                    break;
                case 186:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Stind_IInstruction();
                    }
                    break;
                case 187:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Stind_I1Instruction();
                    }
                    break;
                case 188:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Stind_I2Instruction();
                    }
                    break;
                case 189:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Stind_I4Instruction();
                    }
                    break;
                case 190:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Stind_I8Instruction();
                    }
                    break;
                case 191:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Stind_R4Instruction();
                    }
                    break;
                case 192:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Stind_R8Instruction();
                    }
                    break;
                case 193:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Stind_RefInstruction();
                    }
                    break;
                case 194:
                    {
                        Nullable<short> loc_slot_num;
                        Symbol slot_name;
                        if (!ILParser.TryParseSlotNum(cdr, out loc_slot_num))
                        {
                            slot_name = ILParser.ParseSlotName(cdr);
                            instruction = new StlocInstruction(slot_name);
                        }
                        else
                        {
                            instruction = new StlocInstruction(loc_slot_num.Value);
                        }
                    }
                    break;
                case 195:
                    {
                        StaticTypeResolver type = ILParser.ParseType(cdr);
                        instruction = new StobjInstruction(type);
                    }
                    break;
                case 196:
                    {
                        StaticFieldResolver field = ILParser.ParseField(cdr);
                        instruction = new StfldInstruction(field);
                    }
                    break;
                case 197:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new SubInstruction();
                    }
                    break;
                case 198:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Sub_OvfInstruction();
                    }
                    break;
                case 199:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new Sub_Ovf_UnInstruction();
                    }
                    break;
                case 200:
                    {
                        throw new NotImplementedException("Opcodes.Switch");
                    }
                    //break;
                case 201:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new TailcallInstruction();
                    }
                    break;
                case 202:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new ThrowInstruction();
                    }
                    break;
                case 203:
                    {
                        Nullable<long> loc_slot_num;
                        Symbol slot_name;
                        if (!ILParser.TryParseInt64(cdr, out loc_slot_num))
                        {
                            slot_name = ILParser.ParseSlotName(cdr);
                            instruction = new UnalignedInstruction(slot_name);
                        }
                        else
                        {
                            instruction = new UnalignedInstruction(loc_slot_num.Value);
                        }
                    }
                    break;
                case 204:
                    {
                        StaticTypeResolver type = ILParser.ParseType(cdr);
                        instruction = new UnboxInstruction(type);
                    }
                    break;
                case 205:
                    {
                        StaticTypeResolver type = ILParser.ParseType(cdr);
                        instruction = new Unbox_AnyInstruction(type);
                    }
                    break;
                case 206:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new VolatileInstruction();
                    }
                    break;
                case 207:
                    {
                        ILParser.EnsureNoArgs(cdr);
                        instruction = new XorInstruction();
                    }
                    break;
                case 208:
                    {
                        StaticTypeResolver type = ILParser.ParseType(cdr);
                        instruction = new LboxInstruction(type);
                    }
                    break;
                case 209:
                    {
                        StaticTypeResolver type = ILParser.ParseType(cdr);
                        instruction = new LunboxInstruction(type);
                    }
                    break;
                case 210:
                        {
                            ILParser.EnsureNoArgs(cdr);
                            instruction = new LdnilInstruction();
                        }
                        break;
                case 211:
                        {
                            ILParser.EnsureNoArgs(cdr);
                            instruction = new LdtInstruction();
                        }
                        break;
                case 212:
                        {
                            object constant = ILParser.ParseSingleObject(cdr);
                            instruction = new PushInstruction(constant);
                        }
                        break;
                case 220:
                    break;
                default:
                    ILParser.EnsureNoArgs(cdr);
                    instruction = new MarkLabelInstruction(new LabelDeclaration(op_code_sym.Name));
                    break;
            }
            #endregion
            return new ILCodeExpression(instruction, context);
        }

        private static Expression ParseClrTry(Cons rest, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        private static Expression ParseClrClass(Cons rest, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        private static Expression ParseClrDelegate(Cons rest, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        private static Expression ParseClrEnum(Cons rest, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        private static Expression ParseClrTypeExpression(Cons rest, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        private static List<Expression> BuildBody(object rest, ExpressionContext context, string name)
        {
            if (rest == null)
                return new List<Expression>();
            List<Expression> Body = new List<Expression>();

            Cons _exps = rest as Cons;
            if (_exps == null)
            {
                // TODO: если в самом начале проверяется что rest - правильный список, нужна ли тут эта проверка
                if (rest == DefinedSymbols.NIL)
                    Body.Add(new ConstantExpression(DefinedSymbols.NIL, context));
                else
                    throw new SyntaxErrorException(name + ": body is not a list");
            }
            else
            {
                foreach (var item in _exps)
                {
                    Body.Add(ParseExpression(item, context));
                }
            }
            return Body;
        }


        /*public static Expression ParseExpression(object form)
        {
            Cons cons = form as Cons;

            if (cons != null)
            {
                return ParseCons(cons);
            }

            Symbol symbol = form as Symbol;
            if (symbol != null)
            {
                return ParseSymbol(symbol);
            }

            return ParseConst(form);
        }

        private static Expression ParseCons(Cons cons)
        {
            if(!cons.IsProperList)
                throw new SimpleErrorException("can't evaluate non proper list {0}", cons);

            Symbol _operator = cons.Car as Symbol;

            if (_operator != null)
            {
                Expression ret;
                if (ParseSpecialOp(_operator, cons.Cdr, out ret))
                {
                    return ret;
                }
            }

            Cons _functionalForm = cons.Car as Cons;
            if(_functionalForm != null)
            {
                return ParseCompoundCall(cons, _functionalForm);
            }

            throw new SimpleErrorException("cannot compile form {0}, the car of form not either symobl nor functionalform", cons);
        }

        private static bool ParseSpecialOp(Symbol _operator, object p, out Expression ret)
        {
            throw new NotImplementedException();
        }

        private static Expression ParseCompoundCall(Cons cons, Cons _functionalForm)
        {
            FunctionalForm funcForm = ParseFunctionalForm(_functionalForm);

            if (cons.Count > 1)
            {
                Cons argslist = cons.Cdr as Cons;
                var args = new List<Expression>();
                foreach (var item in argslist)
                {
                    args.Add(ParseExpression(item));
                }

                return Expression.CallExpression(funcForm, argslist);
            }
            else
            {
                return Expression.CallExpression(funcForm);
            }
        }

        private static FunctionalForm ParseFunctionalForm(Cons _functionalForm)
        {
            throw new NotImplementedException();
        }

        private static Expression ParseSymbol(Symbol symbol)
        {
            return Expression.VariableExpression(symbol);
        }

        private static Expression ParseConst(object value)
        {
            return Expression.ConstantExpression(value);
        }*/
    }
}
