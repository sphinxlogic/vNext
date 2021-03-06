/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. 
 *
 * This source code is subject to terms and conditions of the Apache License, Version 2.0. A 
 * copy of the license can be found in the License.html file at the root of this distribution. If 
 * you cannot locate the  Apache License, Version 2.0, please send an email to 
 * dlr@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
 * by the terms of the Apache License, Version 2.0.
 *
 * You must not remove this notice, or any other, from this software.
 *
 *
 * ***************************************************************************/

using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

#if CLR2
namespace Microsoft.Scripting.Ast.Compiler {
#else
namespace System.Linq.Expressions.Compiler {
#endif

    internal sealed partial class CompilerScope {

        private abstract class Storage {
            internal readonly LambdaCompiler Compiler;
            internal readonly ParameterExpression Variable;

            internal Storage(LambdaCompiler compiler, ParameterExpression variable) {
                Compiler = compiler;
                Variable = variable;
            }

            internal abstract void EmitLoad();
            internal abstract void EmitAddress();
            internal abstract void EmitStore();

            internal virtual void EmitStore(Storage value) {
                value.EmitLoad();
                EmitStore();
            }

            internal virtual void FreeLocal() {
            }
        }

        private sealed class LocalStorage : Storage {
            private readonly LocalBuilder _local;

            internal LocalStorage(LambdaCompiler compiler, ParameterExpression variable)
                : base(compiler, variable) {
                // ByRef variables are supported. This is used internally by
                // the compiler when emitting an inlined lambda invoke, to 
                // handle ByRef parameters. BlockExpression prevents this
                // from being exposed to user created trees.
                _local = compiler.GetNamedLocal(variable.IsByRef ? variable.Type.MakeByRefType() : variable.Type, variable);
            }

            internal override void EmitLoad() {
                Compiler.IL.Emit(OpCodes.Ldloc, _local);
            }

            internal override void EmitStore() {
                Compiler.IL.Emit(OpCodes.Stloc, _local);
            }

            internal override void EmitAddress() {
                Compiler.IL.Emit(OpCodes.Ldloca, _local);
            }
        }

        private sealed class ArgumentStorage : Storage {
            private readonly int _argument;

            internal ArgumentStorage(LambdaCompiler compiler, ParameterExpression p)
                : base(compiler, p) {
                _argument = compiler.GetLambdaArgument(compiler.Parameters.IndexOf(p));
            }

            internal override void EmitLoad() {
                Compiler.IL.EmitLoadArg(_argument);
            }

            internal override void EmitStore() {
                Compiler.IL.EmitStoreArg(_argument);
            }

            internal override void EmitAddress() {
                Compiler.IL.EmitLoadArgAddress(_argument);
            }
        }

        private sealed class ElementBoxStorage : Storage {
            private readonly int _index;
            private readonly Storage _array;
            private readonly Type _boxType;
            private readonly FieldInfo _boxValueField;

            internal ElementBoxStorage(Storage array, int index, ParameterExpression variable)
                : base(array.Compiler, variable) {
                _array = array;
                _index = index;
                _boxType = typeof(StrongBox<>).MakeGenericType(variable.Type);
                _boxValueField = _boxType.GetField("Value");
            }

            internal override void EmitLoad() {
                EmitLoadBox();
                Compiler.IL.Emit(OpCodes.Ldfld, _boxValueField);
            }

            internal override void EmitStore() {
                LocalBuilder value = Compiler.GetLocal(Variable.Type);
                Compiler.IL.Emit(OpCodes.Stloc, value);
                EmitLoadBox();
                Compiler.IL.Emit(OpCodes.Ldloc, value);
                Compiler.FreeLocal(value);
                Compiler.IL.Emit(OpCodes.Stfld, _boxValueField);
            }

            internal override void EmitStore(Storage value) {
                EmitLoadBox();
                value.EmitLoad();
                Compiler.IL.Emit(OpCodes.Stfld, _boxValueField);
            }

            internal override void EmitAddress() {
                EmitLoadBox();
                Compiler.IL.Emit(OpCodes.Ldflda, _boxValueField);
            }

            internal void EmitLoadBox() {
                _array.EmitLoad();
                Compiler.IL.EmitInt(_index);
                Compiler.IL.Emit(OpCodes.Ldelem_Ref);
                Compiler.IL.Emit(OpCodes.Castclass, _boxType);
            }
        }

        private sealed class LocalBoxStorage : Storage {
            private readonly LocalBuilder _boxLocal;
            private readonly Type _boxType;
            private readonly FieldInfo _boxValueField;

            internal LocalBoxStorage(LambdaCompiler compiler, ParameterExpression variable)
                : base(compiler, variable) {
                _boxType = typeof(StrongBox<>).MakeGenericType(variable.Type);
                _boxValueField = _boxType.GetField("Value");
                _boxLocal = compiler.GetNamedLocal(_boxType, variable);
            }

            internal override void EmitLoad() {
                Compiler.IL.Emit(OpCodes.Ldloc, _boxLocal);
                Compiler.IL.Emit(OpCodes.Ldfld, _boxValueField);
            }

            internal override void EmitAddress() {
                Compiler.IL.Emit(OpCodes.Ldloc, _boxLocal);
                Compiler.IL.Emit(OpCodes.Ldflda, _boxValueField);
            }

            internal override void EmitStore() {
                LocalBuilder value = Compiler.GetLocal(Variable.Type);
                Compiler.IL.Emit(OpCodes.Stloc, value);
                Compiler.IL.Emit(OpCodes.Ldloc, _boxLocal);
                Compiler.IL.Emit(OpCodes.Ldloc, value);
                Compiler.FreeLocal(value);
                Compiler.IL.Emit(OpCodes.Stfld, _boxValueField);
            }

            internal override void EmitStore(Storage value) {
                Compiler.IL.Emit(OpCodes.Ldloc, _boxLocal);
                value.EmitLoad();
                Compiler.IL.Emit(OpCodes.Stfld, _boxValueField);
            }

            internal void EmitStoreBox() {
                Compiler.IL.Emit(OpCodes.Stloc, _boxLocal);
            }
        }
    }
}