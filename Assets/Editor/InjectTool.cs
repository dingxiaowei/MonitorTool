using System;
using System.Linq;
using UnityEditor;
using Mono.Cecil;
using Mono.Cecil.Cil;
using UnityEngine;
using MonitorLib.GOT.Editor;

public class TestInjectAttribute : Attribute
{
}

public static class InjectTool
{
    private const string AssemblyPath = "./Library/ScriptAssemblies/Assembly-CSharp.dll";

    [MenuItem("Custom/Inject")]
    public static void Inject()
    {
        Debug.Log("InjectTool Inject  Start");

        if (Application.isPlaying || EditorApplication.isCompiling)
        {
            Debug.Log("You need stop play mode or wait compiling finished");
            return;
        }

        // 按路径读取程序集
        var readerParameters = new ReaderParameters { ReadSymbols = false };
        var assembly = AssemblyDefinition.ReadAssembly(AssemblyPath, readerParameters);
        if (assembly == null)
        {
            Debug.LogError(string.Format("InjectTool Inject Load assembly failed: {0}", AssemblyPath));
            return;
        }

        try
        {
            var module = assembly.MainModule;
            foreach (var type in module.Types)
            {
                // 找到module中需要注入的类型
                var needInjectAttr = typeof(TestInjectAttribute).FullName;
                bool needInject = type.CustomAttributes.Any(typeAttribute => typeAttribute.AttributeType.FullName == needInjectAttr);
                if (!needInject)
                {
                    continue;
                }
                // 只对公有方法进行注入
                foreach (var method in type.Methods)
                {
                    if (method.IsConstructor || method.IsGetter || method.IsSetter || !method.IsPublic)
                        continue;
                    InjectMethod(module, method);
                }
            }
            assembly.Write(AssemblyPath, new WriterParameters { WriteSymbols = true });
        }
        catch (Exception ex)
        {
            Debug.LogError(string.Format("InjectTool Inject failed: {0}", ex));
            throw;
        }
        finally
        {
            if (assembly.MainModule.SymbolReader != null)
            {
                Debug.Log("InjectTool Inject SymbolReader.Dispose Succeed");
                assembly.MainModule.SymbolReader.Dispose();
            }
        }

        Debug.Log("InjectTool Inject End");
    }

    private static void InjectMethod(ModuleDefinition module, MethodDefinition method)
    {
        // 定义稍后会用的类型
        var objType = module.ImportReference(typeof(System.Object));
        var intType = module.ImportReference(typeof(System.Int32));
        var logFormatMethod = module.ImportReference(typeof(Debug).GetMethod("LogFormat", new[] { typeof(string), typeof(object[]) }));
        var profilerBeginSample = module.ImportReference(typeof(UnityEngine.Profiling.Profiler).GetMethod("BeginSample", new[] { typeof(string) }));
        var profilerEndSample = module.ImportReference(typeof(UnityEngine.Profiling.Profiler).GetMethod("EndSample"));

        var hookUtilBegin = module.ImportReference(typeof(HookUtil).GetMethod("Begin", new[] { typeof(string) }));
        var hookUtilEnd = module.ImportReference(typeof(HookUtil).GetMethod("End", new[] { typeof(string) }));
        // 开始注入IL代码
        var insertPoint = method.Body.Instructions[0];
        var ilProcessor = method.Body.GetILProcessor();
        // 设置一些标签用于语句跳转
        var label1 = ilProcessor.Create(OpCodes.Ldarg_1);
        var label2 = ilProcessor.Create(OpCodes.Stloc_0);
        var label3 = ilProcessor.Create(OpCodes.Ldloc_0);

        Instruction lastPoint = method.Body.Instructions[method.Body.Instructions.Count - 1];

        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Nop));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Ldstr, "Hook.Test"));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Call, hookUtilBegin));

        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Nop));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Ldstr, "------BeginSample"));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Call, profilerBeginSample));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Nop));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Call, profilerEndSample));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Nop));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Ldstr, "a = {0}, b = {1}"));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Ldc_I4_2));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Newarr, objType));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Dup));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Ldc_I4_0));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Ldarg_0));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Box, intType));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Stelem_Ref));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Dup));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Ldc_I4_1));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Ldarg_1));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Box, intType));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Stelem_Ref));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Call, logFormatMethod));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Ldarg_0));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Ldarg_1));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Ble, label1));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Ldarg_0));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Br, label2));
        ilProcessor.InsertBefore(insertPoint, label1);
        ilProcessor.InsertBefore(insertPoint, label2);
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Br, label3));
        ilProcessor.InsertBefore(insertPoint, label3);
        

        //在最后插入
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Nop));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Ldstr, "Hook.Test"));
        ilProcessor.InsertBefore(insertPoint, Instruction.Create(OpCodes.Call, hookUtilEnd));

        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Ret));
    }
}
