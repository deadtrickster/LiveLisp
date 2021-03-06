/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. 
 *
 * This source code is subject to terms and conditions of the Microsoft Public License. A 
 * copy of the license can be found in the License.html file at the root of this distribution. If 
 * you cannot locate the  Microsoft Public License, please send an email to 
 * dlr@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
 * by the terms of the Microsoft Public License.
 *
 * You must not remove this notice, or any other, from this software.
 *
 *
 * ***************************************************************************/
using System; using Microsoft;
using System.Collections.Generic;
using LiveLisp.Core.BuiltIns.Numbers;
using LiveLisp.Core.BuiltIns.Characters;


namespace LiveLisp.Core.Runtime
{

    public class StrongBox<T> : IStrongBox where T: struct{
        /// <summary>
        /// Gets the strongly typed value associated with the StrongBox.  This is explicitly
        /// exposed as a field instead of a property to enable loading the address of the field.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
        public T Value;

        /// <summary>
        /// Creates a new StrongBox with the specified value.
        /// </summary>
        /// <param name="value"></param>
        protected StrongBox(T value) {
            Value = value;
        }

        protected StrongBox()
        {

        }

        object IStrongBox.Value
        {
            get
            {
                return Value;
            }
        }

        public static implicit operator T(StrongBox<T> boxed)
        {
            return boxed.Value;
        }

        public static implicit operator StrongBox<T>(T value)
        {
            return new StrongBox<T>(value);
        }
        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public interface IStrongBox {
        object Value { get;}
    }
}
