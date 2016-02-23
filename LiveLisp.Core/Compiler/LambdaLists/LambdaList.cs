using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using LiveLisp.Core.Types;
using System.Reflection.Emit;
using LiveLisp.Core.Compiler;
using LiveLisp.Core.AST;
using System.Reflection;
using LiveLisp.Core.Eval;
using LiveLisp.Core.Runtime;
using LiveLisp.Core.AST.Expressions;

namespace LiveLisp.Core.Compiler
{

    public enum ParamsKind
    {
        None,
        Params,
        Rest
    }
    public enum ParameterKind
    {
        Required,
        Optional,
        Rest,
        Key,
        Params,
        Environment,
        Whole,
        Aux
    }

    public class LambdaParameter
    {
        #region Common Fields
        protected Symbol _Name;

        public Symbol Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        protected Int32 _Position;

        public Int32 Position
        {
            get { return _Position; }
            set { _Position = value; }
        }

        protected Symbol _Class;

        public Symbol Class
        {
            get { return _Class; }
            set { _Class = value; }
        }
        #endregion

        protected virtual string Label
        {
            get { return ""; }
        }

        public virtual ParameterKind Kind
        {
            get { throw new NotImplementedException(); }
        }

        protected bool ValidateName(object proto)
        {
            if (proto is Symbol)
            {
                if (proto is KeywordSymbol)
                {
                    throw new Exception(String.Format("Keyword symbol as {0} parameter name not allowed", Label));
                }

                _Name = proto as Symbol;

                return true;
            }

            return false;
        }

        protected LambdaParameter(Int32 position)
        {
            _Position = position;
            _Class = DefinedSymbols.T;
        }

        public static LambdaParameter Construct(object proto, ParameterKind kind, int position)
        {
            switch (kind)
            {
                case ParameterKind.Required:
                    return new RequiredParameter(proto, position);
                case ParameterKind.Optional:
                    return new OptionalParameter(proto, position);
                case ParameterKind.Rest:
                    return new RestParameter(proto, position);
                case ParameterKind.Key:
                    return new KeyParameter(proto, position);
                case ParameterKind.Params:
                    return new ParamsParameter(proto, position);
                case ParameterKind.Environment:
                    return new EnvironmentParameter(proto, position);
                case ParameterKind.Whole:
                    return new WholeParameter(proto, position);
                case ParameterKind.Aux:
                    return new AuxParameter(proto, position);
                default:
                    throw new Exception("Unknown parameter kind: " + kind);
            }
        }
    }

    public class RequiredParameter : LambdaParameter
    {
        protected override string Label
        {
            get
            {
                return "required";
            }
        }

        public override ParameterKind Kind
        {
            get
            {
                return ParameterKind.Required;
            }
        }

        public RequiredParameter(object proto, Int32 position)
            : base(position)
        {
            if (!ValidateName(proto))
            {
                throw new Exception("Required parameter name is not a non-keyword symbol");
            }
        }

        public RequiredParameter(Int32 position)
            : base(position)
        {

        }
    }

    public class ParameterWithInitForm : LambdaParameter
    {
        protected Expression _InitForm = new ConstantExpression(DefinedSymbols.NIL);

        public Expression DefaultValue
        {
            get { return _InitForm; }
            set
            {
                if (value == null)
                    return;
                if (value == _InitForm)
                    return;
                _InitForm = value;
                //  _Suppliedp = true;
            }
        }

        protected bool _Suppliedp;

        public bool Suppliedp
        {
            get { return _Suppliedp; }
            set { _Suppliedp = value; }
        }

        public Symbol SuppliedpName
        {
            get;
            set;
        }


        /* public ParameterWithInitForm(object initForm, Int32 position)
         {
             _InitForm = DefinedSymbols.NIL;

             if (initForm != DefinedSymbols.NIL)
             {
                 _InitForm = initForm;
                 _Suppliedp = true;
                 _HideSuppliedpFromBody = true;
             }
         }

         public ParameterWithInitForm(object initForm, bool suppliedp, Int32 position)
             : this(initForm, position)
         {
             if (_Suppliedp)
             {
                 _HideSuppliedpFromBody = !suppliedp;
             }
             else
             {
                 _Suppliedp = suppliedp;
                 _HideSuppliedpFromBody = !suppliedp;
             }
         }*/

        public ParameterWithInitForm(int position)
            : base(position)
        {

        }
    }

    public class OptionalParameter : ParameterWithInitForm
    {
        protected override string Label
        {
            get
            {
                return "optional";
            }
        }

        public override ParameterKind Kind
        {
            get
            {
                return ParameterKind.Optional;
            }
        }

        public OptionalParameter(object proto, int position)
            : base(position)
        {

            if (proto is Cons)
            {
                Cons longForm = proto as Cons;

                if (longForm.Count > 3)
                    throw new Exception("too long optional parameter spec");

                if (longForm.Count >= 1)
                {
                    if (!ValidateName(longForm.First))
                    {
                        throw new Exception("Invalid optional parameter name " + longForm.First);
                    }
                }
                else
                {
                    throw new Exception("too short (list) optional paramter spec");
                }

                if (longForm.Count >= 2)
                {
                    if (longForm.Count == 3)
                    {
                        if (longForm[2] is Symbol)
                        {
                            if (longForm[2] is KeywordSymbol)
                            {
                                throw new Exception(String.Format("Keyword symbol as {0} suppliedp parameter name not allowed", Label));
                            }

                            _Suppliedp = true;
                            SuppliedpName = longForm[2] as Symbol;
                        }
                    }

                    DefaultValue = LispParser.ParseExpression(longForm.Second, ExpressionContext.Root);
                }
            }
            else
            {
                if (!ValidateName(proto))
                {
                    throw new Exception("&optional parameter name is not a non-keyword symbol");
                }
            }

        }

        public OptionalParameter(int position)
            : base(position)
        {
        }
    }

    public class RestParameter : LambdaParameter
    {
        public override ParameterKind Kind
        {
            get
            {
                return ParameterKind.Rest;
            }
        }

        public RestParameter(object proto, int position)
            : base(position)
        {
            if (!ValidateName(proto))
            {
                throw new Exception("&rest (&body) parameter name is not a non-keyword symbol");
            }
        }

        public RestParameter(int position)
            : base(position)
        {

        }
    }

    public class ParamsParameter : LambdaParameter
    {
        public override ParameterKind Kind
        {
            get
            {
                return ParameterKind.Params;
            }
        }

        public ParamsParameter(object proto, int position)
            : base(position)
        {
            if (!ValidateName(proto))
            {
                throw new Exception("&params parameter name is not a non-keyword symbol");
            }
        }

        public ParamsParameter(int position)
            : base(position)
        {
        }
    }

    public class KeyParameter : ParameterWithInitForm
    {

        public override ParameterKind Kind
        {
            get
            {
                return ParameterKind.Key;
            }
        }

        Symbol _Keyword;

        public Symbol Keyword
        {
            get { return _Keyword; }
            set { _Keyword = value; }
        }

        public KeyParameter(object proto, int position)
            : base(position)
        {
            if (proto is Cons)
            {
                Cons longForm = proto as Cons;

                if (longForm.Count > 3)
                    throw new Exception("too long &key parameter spec");

                if (longForm.Count >= 1)
                {
                    if (longForm.First is Cons)
                    {
                        var longName = longForm.First as Cons;

                        if (longName.Count > 2)
                        {
                            throw new Exception("Too long composite &key parameter name");
                        }
                        if (longName.Count >= 1)
                        {
                            if (!ValidateName(longName.First))
                            {
                                throw new Exception("Invalid &key parameter name " + longName.First);
                            }

                            if (longName.Count == 2)
                            {
                                _Keyword = longName.Second as Symbol;

                                if (_Keyword == null)
                                {
                                    throw new Exception("keyword-name must be a symbol");
                                }
                            }
                        }
                        else
                        {
                            throw new Exception("Too short composite &key parameter name");
                        }
                    }
                    else
                    {
                        if (!ValidateName(longForm.First))
                        {
                            throw new Exception("Invalid &key parameter name " + longForm.First);
                        }
                    }
                }
                else
                {
                    throw new Exception("too short (list) optional paramter spec");
                }

                if (longForm.Count >= 2)
                {
                    if (longForm.Count == 3)
                    {
                        if (longForm[3] is Symbol)
                        {
                            if (longForm[3] is KeywordSymbol)
                            {
                                throw new Exception(String.Format("Keyword symbol as {0} suppliedp parameter name not allowed", Label));
                            }

                            _Suppliedp = true;

                            SuppliedpName = longForm[3] as Symbol;
                        }
                    }

                    DefaultValue = LispParser.ParseExpression(longForm.Second, ExpressionContext.Root);
                }
            }
            else
            {
                if (!ValidateName(proto))
                {
                    throw new Exception("&key parameter name is not a non-keyword symbol");
                }
            }

            if (_Keyword == null)
            {
                Keyword = KeywordPackage.Instance.Intern(Name.Name, true);
            }
        }

        public KeyParameter(int position)
            : base(position)
        {

        }
    }

    public class AuxParameter : LambdaParameter
    {
        public override ParameterKind Kind
        {
            get
            {
                return ParameterKind.Aux;
            }
        }

        Expression _InitForm;

        public Expression InitForm
        {
            get { return _InitForm; }
            set { _InitForm = value; }
        }

        public AuxParameter(object proto, int position)
            : base(position)
        {
            if (proto is Cons)
            {
                var longSpec = proto as Cons;

                if (longSpec.Count > 2)
                {
                    throw new Exception("Too long &aux parameter spec");
                }
                else
                {
                    if (!ValidateName(longSpec.First))
                    {
                        throw new Exception("Aux parameter name is not a non-keyword symbol");
                    }

                    if (longSpec.Count == 2)
                    {
                        _InitForm = LispParser.ParseExpression(longSpec.Second, ExpressionContext.Root);
                    }

                    return;
                }
            }

            if (!ValidateName(proto))
            {
                throw new Exception("Aux parameter name is not a non-keyword symbol");
            }

        }
    }

    public class WholeParameter : LambdaParameter
    {
        public override ParameterKind Kind
        {
            get
            {
                return ParameterKind.Whole;
            }
        }

        protected override string Label
        {
            get
            {
                return "whole";
            }
        }

        public WholeParameter(object proto, int position)
            : base(position)
        {
            if (!ValidateName(proto))
            {
                throw new Exception("Whole parameter name is not a non-keyword symbol");
            }
        }
    }

    public class EnvironmentParameter : LambdaParameter
    {
        public override ParameterKind Kind
        {
            get
            {
                return ParameterKind.Environment;
            }
        }

        public EnvironmentParameter(object proto, int position)
            : base(position)
        {
            if (!ValidateName(proto))
            {
                throw new Exception("&Environment parameter name is not a non-keyword symbol");
            }
        }
    }

    public class CLSConstrainedParameter : LambdaParameter
    {

        StaticTypeResolver type;

        public StaticTypeResolver Type
        {
            get { return type; }
            set { type = value; }
        }

        public List<Attribute> Attributes;

        public bool IsParams;

        public CLSConstrainedParameter(Symbol name, StaticTypeResolver type, int position)
            : base(position)
        {
            this._Name = name;
            this.type = type;
        }
    }

    public class CLSConstrainedParametersCollection : KeyedCollection<Symbol, CLSConstrainedParameter>
    {
        protected override Symbol GetKeyForItem(CLSConstrainedParameter item)
        {
            return item.Name;
        }
    }

    public class LambdaList
    {
        protected List<LambdaParameter> _parameters = new List<LambdaParameter>();

        protected int _min_params_count;
        internal int MinParamsCount
        {
            get { return _min_params_count; }
        }

        protected int _max_params_count;
        internal int  MaxParamsCount
        {
            get { return _max_params_count; }
        }

        protected ParamsKind _paramsKind = ParamsKind.None;
        internal ParamsKind ParamsKind
        {
            get { return _paramsKind; }
        }

        protected bool _requireEnvironmet;
        internal bool RequireEnvironment
        {
            get { return _requireEnvironmet; }
        }

        protected bool _allowOtherKeys;

        public bool AllowOtherKeys
        {
            get { return _allowOtherKeys; }
            set { _allowOtherKeys = value; }
        }

        protected int _params_index = -1;
        internal Int32 ParamsIndex
        {
            get { return _params_index; }
        }

        protected int _whole_index;
        internal Int32 WholeIndex
        {
            get { throw new NotImplementedException(); }
        }

        internal virtual LambdaParameter this[int index]
        {
            get { return _parameters[index]; }
        }

        internal virtual int Count
        {
            get { return _parameters.Count; }
        }

        protected Cons _keysList = new SurrogateNilCons();

        public Cons KeysList
        {
            get { return _keysList; }
            set { _keysList = value; }
        }

        public int AuxIndex = -1;
    }

    public class OrdinaryLambdaList : LambdaList
    {

        private OrdinaryLambdaList(Cons lambdaList)
        {        
            ParameterKind currentParameterKind = ParameterKind.Required;
            int poscounter = 0;
            bool _Rest = false;
            bool _Params = false;

           // bool _AllowOtherKeys = false;
            foreach (var item in lambdaList)
            {
                if (item == DefinedSymbols.LambdaOptional)
                {
                    if (currentParameterKind != ParameterKind.Required)
                    {
                        throw new SyntaxErrorException("Optional parameters not allowed after " + currentParameterKind + " in ordinary lambda list");
                    }
                    else
                    {
                        currentParameterKind = ParameterKind.Optional;
                    }
                }
                else if (item == DefinedSymbols.LambdaRest)
                {
                    switch (currentParameterKind)
                    {
                        case ParameterKind.Required:
                        case ParameterKind.Optional:
                            if (!_Params)
                            {
                                _Rest = true;
                                currentParameterKind = ParameterKind.Rest;
                                break;
                            }
                            else
                            {
                                throw new SyntaxErrorException("&rest and &params at same time not allowed");
                            }
                        case ParameterKind.Rest:
                            throw new SyntaxErrorException("Only one &rest parameter allowed in ordinary lambda list");
                        case ParameterKind.Key:
                        case ParameterKind.Params:
                        case ParameterKind.Aux:
                            throw new SyntaxErrorException("Rest parameter not allowed after " + currentParameterKind + " in ordinary lambda list");
                        case ParameterKind.Environment:
                        case ParameterKind.Whole:
                            throw new SyntaxErrorException("Never reached in ordinary lambda list");
                    }
                }
                else if (item == DefinedSymbols.LambdaKey)
                {
                    switch (currentParameterKind)
                    {
                        case ParameterKind.Required:
                        case ParameterKind.Optional:
                        case ParameterKind.Rest:
                        case ParameterKind.Params:
                            currentParameterKind = ParameterKind.Key;
                            break;
                        case ParameterKind.Key:
                            throw new SyntaxErrorException("Only one &key parameters group allowed in ordinary lambda list");
                        case ParameterKind.Aux:
                            throw new SyntaxErrorException("Key parameters not allowed after " + currentParameterKind + " in ordinary lambda list");
                        case ParameterKind.Environment:
                        case ParameterKind.Whole:
                            throw new SyntaxErrorException("Never reached in ordinary lambda list");
                    }
                }
                else if (item == DefinedSymbols.LambdaAux)
                {
                    currentParameterKind = ParameterKind.Aux;
                    continue;
                }
                else if (item == DefinedSymbols.LambdaParams)
                {
                    switch (currentParameterKind)
                    {
                        case ParameterKind.Required:
                        case ParameterKind.Optional:
                            if (!_Rest)
                            {
                                _Params = true;
                                currentParameterKind = ParameterKind.Params;
                                break;
                            }
                            else
                            {
                                throw new SyntaxErrorException("&rest and &params at same time not allowed");
                            }
                        case ParameterKind.Params:
                            throw new SyntaxErrorException("Only one &params parameter allowed in ordinary lambda list");
                        case ParameterKind.Key:
                        case ParameterKind.Rest:
                        case ParameterKind.Aux:
                            throw new SyntaxErrorException("&Params parameter not allowed after " + currentParameterKind + " in ordinary lambda list");
                        case ParameterKind.Environment:
                        case ParameterKind.Whole:
                            throw new SyntaxErrorException("Never reached in ordinary lambda list");
                    }
                }
                else if (item == DefinedSymbols.LambdaAllowOtherKeys)
                {
                  //  if (currentParameterKind == ParameterKind.Key)
                    //{
                      //  _AllowOtherKeys = true;
                    //}
                    //else
                    //{
                        //throw new SyntaxErrorException("&allow-other-keys outside &key parameters group not allowed");
                    //}

                    _allowOtherKeys = true;
                    continue;
                }
                else if (item == DefinedSymbols.LambdaBody)
                {
                    throw new SyntaxErrorException("&body parameter not allowed in ordinary lambda list");
                }
                else if (item == DefinedSymbols.LambdaEnvironment)
                {
                    throw new SyntaxErrorException("&environment parameter not allowed in ordinary lambda list");
                }
                else if (item == DefinedSymbols.LambdaWhole)
                {
                    throw new SyntaxErrorException("&whole parrameter not allowed in ordinary lambda list");
                }
                else
                {
                    LambdaParameter parameter = LambdaParameter.Construct(item, currentParameterKind, poscounter);
                    if (currentParameterKind == ParameterKind.Key)
                    {
                        _keysList.Append((parameter as KeyParameter).Keyword);
                    }
                    _parameters.Add(parameter);
                    poscounter++;
                }
            }

            _calcualteMinandMax();

            if (_max_params_count > 13)
            {
                _max_params_count = int.MaxValue;
            }
        }

        private OrdinaryLambdaList(MethodInfo method)
        {
            var parms = method.GetParameters();

            int ParamsLength = parms.Length;

            bool nomorerequred = false;

            bool nomoreany = false;

            bool nomoreanythenkey = false;

            bool haskeyword = false;

            for (int i = 0; i < ParamsLength; i++)
            {
                ParameterInfo realParam = parms[i];
                LambdaParameter param;
                if (realParam.GetCustomAttributes(typeof(OptionalAttribute), false).Length != 0)
                {

                    if (nomoreany)
                    {
                        throw new NotImplementedException("params after &params not allowed");
                    }
                    if (nomoreanythenkey)
                        throw new NotImplementedException("any params after &rest then &key not allowed");

                    nomorerequred = true;

                    OptionalAttribute attr = realParam.GetCustomAttributes(typeof(OptionalAttribute), false)[0] as OptionalAttribute;

                    if (attr.NextParamIsPresentedIndicator)
                    {
                        if (parms.Length <= i + 1)
                        {
                            throw new NotImplementedException("presented indicator parameter for optional parameter not found");
                        }

                        i++;
                    }

                    OptionalParameter lparam = new OptionalParameter(realParam.Position);
                    lparam.DefaultValue = attr.DefaultValue == null ? new ConstantExpression(DefinedSymbols.NIL) : LispParser.ParseExpression(DefinedSymbols.Read.Invoke(attr.DefaultValue), null);
                    lparam.Suppliedp = attr.NextParamIsPresentedIndicator;
                    if(lparam.Suppliedp)
                    lparam.SuppliedpName = new Symbol(parms[i++].Name);
                    param = lparam;
                    _parameters.Add(lparam);
                }
                else if (realParam.GetCustomAttributes(typeof(KeyAttribute), false).Length != 0)
                {
                    if (nomoreany)
                    {
                        throw new NotImplementedException("params after &params not allowed");
                    }

                    haskeyword = true;
                    nomorerequred = true;
                    KeyAttribute attr = realParam.GetCustomAttributes(typeof(KeyAttribute), false)[0] as KeyAttribute;

                    KeyParameter lparam = new KeyParameter(realParam.Position);
                    lparam.DefaultValue = attr.DefaultValue == null ? new ConstantExpression(DefinedSymbols.NIL) : LispParser.ParseExpression(DefinedSymbols.Read.Invoke(attr.DefaultValue), null);
                    lparam.Keyword = KeywordPackage.Instance.Intern((attr.KeySymbolString ?? realParam.Name).SetCase());
                    param = lparam;
                    lparam.Suppliedp = attr.NextParamIsPresentedIndicator;
                    if (lparam.Suppliedp)
                    lparam.SuppliedpName = new Symbol(parms[++i].Name);
                    _parameters.Add(lparam);
                    _keysList.Append(lparam.Keyword);
                }
                else if (realParam.GetCustomAttributes(typeof(RestAttribute), false).Length != 0)
                {
                    if (nomoreany)
                    {
                        throw new NotImplementedException("params after &params not allowed");
                    }

                    if (nomoreanythenkey)
                        throw new NotImplementedException("any params after &rest then &key not allowed");

                    if(haskeyword)
                        throw new NotImplementedException("rest after &key not allowed");

                    nomorerequred = true;
                    nomoreanythenkey = true;

                    RestParameter lparam = new RestParameter(realParam.Position);
                    param = lparam;
                    _parameters.Add(lparam);

                }
                else if (realParam.GetCustomAttributes(typeof(ParamArrayAttribute), false).Length != 0)
                {

                    if (nomoreany)
                    {
                        throw new NotImplementedException("params after &params not allowed");
                    }

                    if (nomoreanythenkey)
                        throw new NotImplementedException("any params after &rest then &key not allowed");

                    nomorerequred = true;
                    nomoreany = true;

                    ParamsParameter lparam = new ParamsParameter(realParam.Position);
                    param = lparam;
                    _parameters.Add(lparam);
                }
                else
                {
                    if (nomorerequred)
                    {
                        throw new NotImplementedException("Required args not allowed after any custom args");
                    }

                    RequiredParameter lparam = new RequiredParameter(realParam.Position);
                    param = lparam;
                    _parameters.Add(lparam);
                    // required
                }

                param.Name = new Symbol(realParam.Name);
            }

            var qwe/* i'm loving it */ = method.GetCustomAttributes(typeof(BuiltinAttribute), true);

            if (qwe.Length != 0)
            {
                BuiltinAttribute attri = qwe[0] as BuiltinAttribute;
                _allowOtherKeys = attri.AllowOtherKeys;
            }

            if (_allowOtherKeys && !haskeyword)
            {
                throw new SyntaxErrorException("badly placed lambda-list keyword &ALLOW-OTHER-KEYS " + method);
            }

            _calcualteMinandMax();

            if ((method.Attributes & MethodAttributes.Static) != MethodAttributes.Static)
            {
                //TODO: chech for int overflow
                _min_params_count++;
                _max_params_count++;
            }

            if (_max_params_count > 13)
            {
                _max_params_count = int.MaxValue;
            }
        }

        private OrdinaryLambdaList()
        {
            _min_params_count = 0;
            _min_params_count = 0;
            _parameters = new List<LambdaParameter>();
        }


        
        private void _calcualteMinandMax()
        {
            foreach (var item in _parameters)
            {
                switch (item.Kind)
                {
                    case ParameterKind.Required:
                        _min_params_count++;
                        _max_params_count++;
                        break;
                    case ParameterKind.Optional:
                        _max_params_count++;
                        break;
                    case ParameterKind.Rest:
                        _max_params_count = int.MaxValue;
                        _paramsKind = ParamsKind.Rest;
                        _params_index = _parameters.IndexOf(item);
                        return;
                    case ParameterKind.Key:
                        _max_params_count += 2;
                        if (_params_index == -1)
                        {
                            _params_index = item.Position;
                        }
                        break;
                    case ParameterKind.Params: 
                        _max_params_count = int.MaxValue;
                        _paramsKind = ParamsKind.Params;
                        _params_index = _parameters.IndexOf(item);
                        return;
                    case ParameterKind.Environment:
                        _max_params_count++;
                        _min_params_count++;
                        break;
                    case ParameterKind.Whole:
                        break;
                    case ParameterKind.Aux:
                        if (AuxIndex == -1)
                            AuxIndex = item.Position;
                        break;
                    default:
                        break;
                }
            }
        }

        

        public static OrdinaryLambdaList CreateFromMethod(MethodInfo method)
        {
            return new OrdinaryLambdaList(method);
        }

        public static OrdinaryLambdaList CreateFromList(Cons list)
        {
            return new OrdinaryLambdaList(list);
        }

        internal static LambdaList FromNil()
        {
            return new OrdinaryLambdaList();
        }
    }

    public class ClrMethodLambdaList : LambdaList
    {
        List<CLSConstrainedParameter> clrparameters;
        public ClrMethodLambdaList(List<CLSConstrainedParameter> parameters)
        {
            clrparameters = new List<CLSConstrainedParameter>(parameters);
            _whole_index = -1;
            _params_index = -1;
            if (parameters.Last().IsParams)
            {
                _max_params_count = Int32.MaxValue;
                _min_params_count = parameters.Count - 1;
            }
            else
            {
                _min_params_count = _max_params_count = parameters.Count;
            }
        }

        internal override int Count
        {
            get
            {
                return clrparameters.Count;
            }
        }

        internal override LambdaParameter this[int index]
        {
            get
            {
                return clrparameters[index];
            }
        }
    }
}
