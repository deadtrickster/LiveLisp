using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LiveLisp.Core.Types;
using LiveLisp.Core.Compiler;

namespace LiveLisp.Core.Tests.Compiler
{
    [TestClass()]
    public class LambdaGeneratorTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        static Symbol WriteSymbol;
        static Symbol WriteLineSymbol;

        [ClassInitialize()]
        public static void CLRMethodImporterTestInitialize(TestContext testContext)
        {
            Initialization.Initialize();
            // нужно создать пакет и символы
            Package TestClassPackage = PackagesCollection.CreatePackage("TestClass");
        }

        [TestMethod]
        public void SimpleTest()
        {
            object form = DefinedSymbols.Read.Invoke( /*@"#'(lambda () (block named
                          (setq func #'(lambda (x y) (if x (+ x y) (return-from named 100))))
                          (print (funcall func 1 2))
                          (funcall func nil nil)))"*/
                /*@"#'(lambda ()(tagbody (setq func #'(lambda () (go out))) (print 1) (print 2) 
(funcall func) (print 3) (print 4) out (print 5)))"*/
                /*"#'(lambda () '(1 2))"*/@"#'(lambda () (let ((qwe 1))
                                     (declare (special qwe))
                                     (Symbol-value 'qwe)))");
            object result = DefinedSymbols.Eval.Invoke(form);
            CompilerImpl.SaveAssembly();
        }

        [TestMethod]
        public void Test()
        {
            object form = DefinedSymbols.Read.Invoke(@"(system::set-symbol-macro 
	'defun
	#'(lambda (x env)
		(declare (ignore env))
		`(progn
			(system::set-symbol-function				
			',(car (cdr x))
			(function ,(car (cdr x)) 
				(lambda ,(car (cdr (cdr x)))
						(block ,(car (cdr x)) ,@(cdr (cdr (cdr x)))))))	
			',(car (cdr x)))))");
            DefinedSymbols.Eval.Invoke(form);
            form = DefinedSymbols.Read.Invoke(@"(defun macroexpand (x &optional env)
	(tagbody loop
		(if (consp x)
			(if (symbolp (car x))
				(if (macro-function (car x))
					(progn 
						(setq x (funcall (macro-function (car x)) x env))
						(go loop))))))
	x)");
            DefinedSymbols.Eval.Invoke(form);
            CompilerImpl.SaveAssembly();
        }
    }
}
