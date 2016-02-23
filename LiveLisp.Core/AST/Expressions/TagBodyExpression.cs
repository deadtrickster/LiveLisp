namespace LiveLisp.Core.AST.Expressions
{
    using LiveLisp.Core.AST;
    using System;
    using System.Collections.Generic;
using LiveLisp.Core.Compiler;
    using System.Reflection.Emit;
    using System.Linq;

    public class TagBodyExpression : Expression
    {
        private List<Expression> _nontaggedProlog;
        private List<TaggedStatements> _taggedStatements;

        public List<Object> tags = new List<object>();

        public TagBodyExpression(List<Expression> nontaggedProlog, List<TaggedStatements> statements, ExpressionContext context)
            : base(context)
        {
            this._nontaggedProlog = nontaggedProlog;
            this._taggedStatements = statements;

            foreach (var item in _taggedStatements)
	        {
                tags.Add(item.Tag);
            }
        }

        public override void Visit(IASTWalker visitor, ExpressionContext context)
        {
            visitor.TagbodyExpression(this, context);
        }

        public List<Expression> NontaggedProlog
        {
            get
            {
                return this._nontaggedProlog;
            }
        }

        public List<TaggedStatements> TaggedStatements
        {
            get
            {
                return this._taggedStatements;
            }
        }

        internal override Expression Copy(ExpressionContext new_context)
        {
            List<Expression> new_nontagged = new List<Expression>(_nontaggedProlog.Count);
            foreach (var item in _nontaggedProlog)
            {
                new_nontagged.Add(item.Copy(new ExpressionContext(new_context)));
            }

            List<TaggedStatements> new_tagged = new List<TaggedStatements>();

            foreach (var item in _taggedStatements)
            {
                List<Expression> exps = new List<Expression>(item.Statements.Count);
                foreach (var stm in item.Statements)
                {
                    exps.Add(stm.Copy(new ExpressionContext(new_context)));
                }

                new_tagged.Add(new TaggedStatements(item.Tag, exps));
            }

            return new TagBodyExpression(new_nontagged, new_tagged, new_context);
        }

        #region IImplicitProgn Members

        public List<Expression> Forms
        {
            get { throw new NotImplementedException(); }
        }

        public Expression this[int index]
        {
            get { throw new NotImplementedException(); }
        }

        public int Count
        {
            get { throw new NotImplementedException(); }
        }

        public Expression Last
        {
            get { throw new NotImplementedException(); }
        }

        #endregion


        public override object Eval(LiveLisp.Core.Interpreter.IEvalWalker evaluator, LiveLisp.Core.Interpreter.EvaluationContext context)
        {
            return evaluator.TagbodyExpression(this, context);
        }
    }
    /*
    class TagBodyGenerator : IGenerator
    {
        TagBodyExpression tagBody;
        public TagBodyGenerator(TagBodyExpression tagbody)
        {
            tagBody = tagbody;

            localGenerators = new List<List<GoLocalGenerator>>();

            for (int i = 0; i < tagBody.tags.Count; i++)
            {
                localGenerators.Add(new List<GoLocalGenerator>());
            }
        }

        List<List<GoLocalGenerator>> localGenerators;

        public void RegisterLocalGoGenerator(object tag, GoLocalGenerator goGenerator)
        {
            var list = localGenerators[tagBody.tags.IndexOf(tag)];
            list.Add(goGenerator);
        }

        Dictionary<object, GoNonlocalGenerator> nonlocalGenerators = new Dictionary<object, GoNonlocalGenerator>();

        public void RegisterNonLocalGoGenerator(object tag, GoNonlocalGenerator goGenerator)
        {
            nonlocalGenerators.Add(tag, goGenerator);
        }
        #region IGenerator Members

        public void Generate(ILGenerator gen)
        {
            List<Label> labels = new List<Label>();
            for (int i = 0; i < tagBody.tags.Count; i++)
            {
                Label label = gen.DefineLabel();

                labels.Add(label);

                for (int j = 0; j < localGenerators[i].Count; j++)
                {
                    localGenerators[i][j].go_location = label;
                }
            }

            for (int i = 0; i < tagBody.NontaggedProlog.Count; i++)
			{
                DefaultASTCompiler.JustWalk(tagBody.NontaggedProlog[i], gen);
			}

            for (int i = 0; i < tagBody.TaggedStatements.Count; i++)
            {
                gen.MarkLabel(labels[i]);

                for (int j = 0; j < tagBody.TaggedStatements.ElementAt(i).Statements.Count; j++)
                {
                    Expression taggedExp = tagBody.TaggedStatements.ElementAt(i).Statements[j];
                    DefaultASTCompiler.JustWalk(taggedExp, gen);
                }
            }

            if (!tagBody.Context.VoidReturn)
                gen.EmitNIL();
        }

        #endregion
    }
    */
}

