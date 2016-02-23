using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.Compiler;
using LiveLisp.Core.Types;
using System.Reflection;
using LiveLisp.Core.Runtime;
using LiveLisp.Core.AST.Expressions;

namespace LiveLisp.Core.Eval
{
    public class LambdaListParser
    {
        internal static List<LambdaParameter> ParseMethodInfo(MethodInfo method)
        {
            List<LambdaParameter> _parameters = new List<LambdaParameter>();

            var parms = method.GetParameters();

            int ParamsLength = parms.Length;

            bool nomorerequred = false;

            bool nomoreany = false;

            bool nomoreanythenkey = false;

            bool allowotherkeys = false;

            bool haskeyword = false;

            for (int i = 0; i < ParamsLength; i++)
            {
                ParameterInfo realParam = parms[i];

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
                  //  lparam.DefaultValue = attr.DefaultValue ?? new ConstantExpression(DefinedSymbols.NIL);
                    lparam.Suppliedp = attr.NextParamIsPresentedIndicator;

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
                   // lparam.DefaultValue = attr.DefValue ?? DefinedSymbols.NIL;
                    lparam.Keyword = KeywordPackage.Instance.Intern(attr.KeySymbolString ?? realParam.Name);

                    _parameters.Add(lparam);
                }
                else if (realParam.GetCustomAttributes(typeof(RestAttribute), false).Length != 0)
                {
                    if (nomoreany)
                    {
                        throw new NotImplementedException("params after &params not allowed");
                    }

                    if (nomoreanythenkey)
                        throw new NotImplementedException("any params after &rest then &key not allowed");


                    nomorerequred = true;
                    nomoreanythenkey = true;

                    RestParameter lparam = new RestParameter(realParam.Position);

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

                    _parameters.Add(lparam);
                }
                else
                {
                    if (nomorerequred)
                    {
                        throw new NotImplementedException("Required args not allowed after any custom args");
                    }

                    RequiredParameter lparam = new RequiredParameter(realParam.Position);

                    _parameters.Add(lparam);
                    // required
                }
            }

            var qwe/* i'm loving it */ = method.GetCustomAttributes(typeof(BuiltinAttribute), true);

            if (qwe.Length != 0)
            {
                BuiltinAttribute attri = qwe[0] as BuiltinAttribute;
                allowotherkeys = attri.AllowOtherKeys;
            }

            if (allowotherkeys && !haskeyword)
            {
                throw new SyntaxErrorException("badly placed lambda-list keyword &ALLOW-OTHER-KEYS " + method);
            }

            return _parameters;
        }

        /// <summary>
        /// note: destructuring handled in respected macros
        /// </summary>
        /// <param name="lambdaList"></param>
        /// <returns></returns>
        public static List<LambdaParameter> ParseOrdinaryList(Cons lambdaList)
        {
            List<LambdaParameter> _parameters = new List<LambdaParameter>();
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

                    throw new NotImplementedException();
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
                    _parameters.Add(LambdaParameter.Construct(item, currentParameterKind, poscounter));
                    poscounter++;
                }
            }

            return _parameters;
        }
    }
}
