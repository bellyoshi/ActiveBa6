﻿using System;
using System.Reflection;
using System.Reflection.Emit;

class Program
{
    static void Main()
    {
        
        //アセンブリの設定
        AssemblyName aName = new AssemblyName("DynamicAssemblyExample");
        AssemblyBuilder ab =
            AssemblyBuilder.DefineDynamicAssembly(
                aName,
                AssemblyBuilderAccess.Run);

        // The module name is usually the same as the assembly name.
        ModuleBuilder mb =
            ab.DefineDynamicModule(aName.Name);

        TypeBuilder tb = mb.DefineType(
            "MyDynamicType",
             TypeAttributes.Public);

        // Add a private field of type int (Int32).
        FieldBuilder fbNumber = tb.DefineField(
            "m_number",
            typeof(int),
            FieldAttributes.Private);

        // Define a constructor that takes an integer argument and
        // stores it in the private field.
        Type[] parameterTypes = { typeof(int) };
        ConstructorBuilder ctor1 = tb.DefineConstructor(
            MethodAttributes.Public,
            CallingConventions.Standard,
            parameterTypes);

        ILGenerator ctor1IL = ctor1.GetILGenerator();
        // For a constructor, argument zero is a reference to the new
        // instance. Push it on the stack before calling the base
        // class constructor. Specify the default constructor of the
        // base class (System.Object) by passing an empty array of
        // types (Type.EmptyTypes) to GetConstructor.
        ctor1IL.Emit(OpCodes.Ldarg_0);
        ctor1IL.Emit(OpCodes.Call,
            typeof(object).GetConstructor(Type.EmptyTypes));
        // Push the instance on the stack before pushing the argument
        // that is to be assigned to the private field m_number.
        ctor1IL.Emit(OpCodes.Ldarg_0);
        ctor1IL.Emit(OpCodes.Ldarg_1);
        ctor1IL.Emit(OpCodes.Stfld, fbNumber);
        ctor1IL.Emit(OpCodes.Ret);

        // Define a default constructor that supplies a default value
        // for the private field. For parameter types, pass the empty
        // array of types or pass null.
        ConstructorBuilder ctor0 = tb.DefineConstructor(
            MethodAttributes.Public,
            CallingConventions.Standard,
            Type.EmptyTypes);

        ILGenerator ctor0IL = ctor0.GetILGenerator();
        // For a constructor, argument zero is a reference to the new
        // instance. Push it on the stack before pushing the default
        // value on the stack, then call constructor ctor1.
        ctor0IL.Emit(OpCodes.Ldarg_0);
        ctor0IL.Emit(OpCodes.Ldc_I4_S, 42);
        ctor0IL.Emit(OpCodes.Call, ctor1);
        ctor0IL.Emit(OpCodes.Ret);

        // Define a property named Number that gets and sets the private
        // field.
        //
        // The last argument of DefineProperty is null, because the
        // property has no parameters. (If you don't specify null, you must
        // specify an array of Type objects. For a parameterless property,
        // use the built-in array with no elements: Type.EmptyTypes)
        PropertyBuilder pbNumber = tb.DefineProperty(
            "Number",
            PropertyAttributes.HasDefault,
            typeof(int),
            null);

        // The property "set" and property "get" methods require a special
        // set of attributes.
        MethodAttributes getSetAttr = MethodAttributes.Public |
            MethodAttributes.SpecialName | MethodAttributes.HideBySig;

        // Define the "get" accessor method for Number. The method returns
        // an integer and has no arguments. (Note that null could be
        // used instead of Types.EmptyTypes)
        MethodBuilder mbNumberGetAccessor = tb.DefineMethod(
            "get_Number",
            getSetAttr,
            typeof(int),
            Type.EmptyTypes);

        ILGenerator numberGetIL = mbNumberGetAccessor.GetILGenerator();
        // For an instance property, argument zero is the instance. Load the
        // instance, then load the private field and return, leaving the
        // field value on the stack.
        numberGetIL.Emit(OpCodes.Ldarg_0);
        numberGetIL.Emit(OpCodes.Ldfld, fbNumber);
        numberGetIL.Emit(OpCodes.Ret);

        // Define the "set" accessor method for Number, which has no return
        // type and takes one argument of type int (Int32).
        MethodBuilder mbNumberSetAccessor = tb.DefineMethod(
            "set_Number",
            getSetAttr,
            null,
            new Type[] { typeof(int) });

        ILGenerator numberSetIL = mbNumberSetAccessor.GetILGenerator();
        // Load the instance and then the numeric argument, then store the
        // argument in the field.
        numberSetIL.Emit(OpCodes.Ldarg_0);
        numberSetIL.Emit(OpCodes.Ldarg_1);
        numberSetIL.Emit(OpCodes.Stfld, fbNumber);
        numberSetIL.Emit(OpCodes.Ret);

        // Last, map the "get" and "set" accessor methods to the
        // PropertyBuilder. The property is now complete.
        pbNumber.SetGetMethod(mbNumberGetAccessor);
        pbNumber.SetSetMethod(mbNumberSetAccessor);

        // Define a method that accepts an integer argument and returns
        // the product of that integer and the private field m_number. This
        // time, the array of parameter types is created on the fly.
        MethodBuilder meth = tb.DefineMethod(
            "MyMethod",
            MethodAttributes.Public,
            typeof(int),
            new Type[] { typeof(int) });

        ILGenerator methIL = meth.GetILGenerator();
        // To retrieve the private instance field, load the instance it
        // belongs to (argument zero). After loading the field, load the
        // argument one and then multiply. Return from the method with
        // the return value (the product of the two numbers) on the
        // execution stack.
        methIL.Emit(OpCodes.Ldarg_0);
        methIL.Emit(OpCodes.Ldfld, fbNumber);
        methIL.Emit(OpCodes.Ldarg_1);
        methIL.Emit(OpCodes.Mul);
        methIL.Emit(OpCodes.Ret);

        Assembly assembly = ab.GetSatelliteAssembly(System.Globalization.CultureInfo.CurrentCulture);
        var generator = new Lokad.ILPack.AssemblyGenerator();


        // direct serialization to disk
        generator.GenerateAssembly(assembly, "a.dll");
    }
}