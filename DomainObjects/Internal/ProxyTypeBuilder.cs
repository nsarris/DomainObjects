﻿using Dynamix.Helpers;
using Dynamix.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Internal
{
    static class ProxyTypeBuilder
    {
        public static Type PropertyChangedProxy<T>() where T : class
        {
            var type = typeof(T);
            var assemblyName = type.FullName + "_Proxy";
            
            var name = new AssemblyName(assemblyName);
            var assembly = AssemblyBuilder.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
            var module = assembly.DefineDynamicModule(assemblyName);

            var typeBuilder = module.DefineType(type.Name + "Proxy",
                TypeAttributes.Class | TypeAttributes.Public, type);

            foreach (var attribute in BuildCustomAttributes(type.GetCustomAttributesData()))
                typeBuilder.SetCustomAttribute(attribute);

            if (!type.HasAttribute<SerializableAttribute>())
                typeBuilder.SetCustomAttribute(
                    new CustomAttributeBuilder(typeof(SerializableAttribute).GetConstructor(new Type[] { }), new object[] { }));
            //Force Add Serializable

            CreatePassThroughConstructors(typeBuilder, type);

            var onPropertyChangedMethod = type.GetMethod("OnPropertyChanged",
                BindingFlags.Instance | BindingFlags.NonPublic);

            var propertyInfos = type.GetProperties().Where(p => p.CanRead && p.CanWrite && p.GetIndexParameters().Length == 0 && p.GetMethod.IsVirtual);
            foreach (var item in propertyInfos)
            {
                var baseMethod = item.SetMethod;
                var setAccessor = typeBuilder.DefineMethod
                       (baseMethod.Name, baseMethod.Attributes, typeof(void), new[] { item.PropertyType });
                var il = setAccessor.GetILGenerator();
                var retLabel = il.DefineLabel();

                il.DeclareLocal(typeof(object));
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Call, item.GetMethod);
                il.Emit(OpCodes.Stloc_0);

                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Beq, retLabel);

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Call, baseMethod);

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldstr, item.Name);
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Call, onPropertyChangedMethod);

                il.MarkLabel(retLabel);
                il.Emit(OpCodes.Ret);
                typeBuilder.DefineMethodOverride(setAccessor, baseMethod);
            }
            return typeBuilder.CreateType();
        }

        private static void CreatePassThroughConstructors(this TypeBuilder builder, Type baseType)
        {
            foreach (var constructor in baseType.GetConstructors())
            {
                var parameters = constructor.GetParameters();
                if (parameters.Length > 0 && parameters.Last().IsDefined(typeof(ParamArrayAttribute), false))
                {
                    throw new InvalidOperationException("Variadic constructors are not supported");
                }

                var parameterTypes = parameters.Select(p => p.ParameterType).ToArray();
                var requiredCustomModifiers = parameters.Select(p => p.GetRequiredCustomModifiers()).ToArray();
                var optionalCustomModifiers = parameters.Select(p => p.GetOptionalCustomModifiers()).ToArray();

                var ctor = builder.DefineConstructor(MethodAttributes.Public, constructor.CallingConvention, parameterTypes, requiredCustomModifiers, optionalCustomModifiers);
                for (var i = 0; i < parameters.Length; ++i)
                {
                    var parameter = parameters[i];
                    var parameterBuilder = ctor.DefineParameter(i + 1, parameter.Attributes, parameter.Name);
                    if (((int)parameter.Attributes & (int)ParameterAttributes.HasDefault) != 0)
                    {
                        parameterBuilder.SetConstant(parameter.RawDefaultValue);
                    }

                    foreach (var attribute in BuildCustomAttributes(parameter.GetCustomAttributesData()))
                    {
                        parameterBuilder.SetCustomAttribute(attribute);
                    }
                }

                foreach (var attribute in BuildCustomAttributes(constructor.GetCustomAttributesData()))
                {
                    ctor.SetCustomAttribute(attribute);
                }

                var emitter = ctor.GetILGenerator();
                emitter.Emit(OpCodes.Nop);

                // Load `this` and call base constructor with arguments
                emitter.Emit(OpCodes.Ldarg_0);
                for (var i = 1; i <= parameters.Length; ++i)
                {
                    emitter.Emit(OpCodes.Ldarg, i);
                }
                emitter.Emit(OpCodes.Call, constructor);

                emitter.Emit(OpCodes.Ret);
            }
        }

        private static CustomAttributeBuilder[] BuildCustomAttributes(IEnumerable<CustomAttributeData> customAttributes)
        {
            return customAttributes.Select(attribute => {
                var attributeArgs = attribute.ConstructorArguments.Select(a => a.Value).ToArray();
                var namedPropertyInfos = attribute.NamedArguments.Select(a => a.MemberInfo).OfType<PropertyInfo>().ToArray();
                var namedPropertyValues = attribute.NamedArguments.Where(a => a.MemberInfo is PropertyInfo).Select(a => a.TypedValue.Value).ToArray();
                var namedFieldInfos = attribute.NamedArguments.Select(a => a.MemberInfo).OfType<FieldInfo>().ToArray();
                var namedFieldValues = attribute.NamedArguments.Where(a => a.MemberInfo is FieldInfo).Select(a => a.TypedValue.Value).ToArray();
                return new CustomAttributeBuilder(attribute.Constructor, attributeArgs, namedPropertyInfos, namedPropertyValues, namedFieldInfos, namedFieldValues);
            }).ToArray();
        }

        public static T CreateInstance<T>(Expression<Func<T>> ctorExpression, Func<T,T> initializer = null)
            where T : class
        {
            if (ctorExpression.Body.NodeType != ExpressionType.New)
                throw new ArgumentException("CreateInstance needs a constructor expression", nameof(ctorExpression));

            var newExpression = (NewExpression)ctorExpression.Body;

            var ctor = newExpression.Constructor;

            var newType = PropertyChangedProxy<T>();
            var newCtor = newType.GetConstructorsEx()
                .FirstOrDefault(x => x.ConstructorInfo.GetParameters().Select(p => p.ParameterType).SequenceEqual(ctor.GetParameters().Select(p => p.ParameterType)));

            T instance = null;

            if (newExpression.Arguments.All(x => x is ConstantExpression))
                instance = (T)newCtor.Invoke(newExpression.Arguments.Select(x => ((ConstantExpression)x).Value).ToArray());
            else
            {
                var l = new CtorReplacerVisitor(ctor, newCtor).Visit(ctorExpression) as LambdaExpression;
                var ll = (new EvaluationVisitor().Visit(l.Body) as ConstantExpression).Value;
                //return ((Expression<Func<T>>)l).Compile().Invoke();
                instance =(T)ll;
            }

            initializer?.Invoke(instance);

            return instance;
        }

        private class CtorReplacerVisitor : ExpressionVisitor
        {
            readonly ConstructorInfo findCtor;
            readonly ConstructorInfo replaceCtor;
            public CtorReplacerVisitor(ConstructorInfo findCtor, ConstructorInfo replaceCtor)
            {
                this.findCtor = findCtor;
                this.replaceCtor = replaceCtor;
            }
            protected override Expression VisitNew(NewExpression node)
            {
                if (node.Constructor == findCtor)
                {
                    if (node.Members != null)
                        return Expression.New(replaceCtor, node.Arguments, node.Members);
                    else
                        return Expression.New(replaceCtor, node.Arguments);
                }
                else
                    return base.VisitNew(node);
            }
        }

        public class EvaluationVisitor : ExpressionVisitor
        {
            protected override Expression VisitUnary(UnaryExpression node)
            {
                var operand = Visit(node.Operand);
                return Expression.Constant(node.Method.Invoke(operand, Array.Empty<object>()), node.Type);
            }

            protected override Expression VisitBinary(BinaryExpression node)
            {
                var left = Evaluate(node.Left);
                var right = Evaluate(node.Right);
                return Expression.Constant(node.Method.GetMethodEx().Invoke(left, right));
            }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                var args = node.Arguments.Select(a => Evaluate(a)).Select(a => a.Value).ToArray();
                return Expression.Constant(node.Method.GetMethodEx().Invoke(Evaluate(node.Object).Value, args));
            }

            protected override Expression VisitConstant(ConstantExpression node)
            {
                return node;
            }

            protected override Expression VisitDefault(DefaultExpression node)
            {
                return Expression.Constant(node.Type.DefaultOf());
            }

            protected override Expression VisitExtension(Expression node)
            {
                return base.VisitExtension(node.ReduceExtensions());
            }

            protected override Expression VisitTypeBinary(TypeBinaryExpression node)
            {
                return Expression.Constant(ConvertEx.ConvertTo(Evaluate(node.Expression).Value, node.TypeOperand));
            }

            protected override Expression VisitConditional(ConditionalExpression node)
            {
                var test = true.Equals(Evaluate(node.Test).Value);
                var ifTrue = Evaluate(node.IfTrue);
                var ifFalse = Evaluate(node.IfFalse);
                return Expression.Constant(test ? ifTrue : ifFalse);
            }

            protected override Expression VisitMember(MemberExpression node)
            {
                var instance = Evaluate(node.Expression);
                if (node.Member.MemberType == MemberTypes.Field)
                    return Expression.Constant(((FieldInfo)node.Member).GetFieldEx().Get(instance.Value));
                else if (node.Member.MemberType == MemberTypes.Property)
                    return Expression.Constant(((PropertyInfo)node.Member).GetPropertyEx().Get(instance.Value));
                else
                    throw new InvalidOperationException("Unsupported member type");
            }

            protected override Expression VisitNew(NewExpression node)
            {
                var args = node.Arguments.Select(a => Evaluate(a)).Select(a => a.Value).ToArray();
                return Expression.Constant(node.Constructor.GetConstructorEx().Invoke(args));
            }

            protected override Expression VisitDynamic(DynamicExpression node)
            {
                throw new NotSupportedException("Dynamic is not supported in lambda evaluator");
                //return base.VisitDynamic(node);
            }

            //protected override ElementInit VisitElementInit(ElementInit node)
            //{
            //    var args = node.Arguments.Select(a => Evaluate(a)).Select(a => a.Value).ToArray();
            //    return Expression.Constant(node.AddMethod.GetMethodEx().Invoke(args.First(), args.Skip(1).ToArray()));
            //}

            //protected override Expression VisitInvocation(InvocationExpression node)
            //{

            //    return Expression.Constant(Expression.Invoke(node.Expression, node.Arguments.Select(x => Evaluate(x)).ToArray()));
                
            //    return base.VisitInvocation(node);
            //}

            protected override Expression VisitIndex(IndexExpression node)
            {
                var args = node.Arguments.Select(a => Evaluate(a)).Select(a => a.Value).ToArray();
                return Expression.Constant(node.Indexer.GetPropertyEx().Get(Evaluate(node.Object), args));
            }

            protected override Expression VisitNewArray(NewArrayExpression node)
            {
                var array = Array.CreateInstance(node.Type, node.Expressions.Count);
                var i = 0;
                foreach (var e in node.Expressions)
                    array.SetValue(Evaluate(e).Value, i);
                return Expression.Constant(array);
            }

            private ConstantExpression Evaluate(Expression node)
            {
                return (ConstantExpression)Visit(node);
            }
        }
    }
}