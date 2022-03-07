using MonitorLib.GOT;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class HookEditor
{
    private const string AssemblyPath = "./Library/ScriptAssemblies/Assembly-CSharp.dll";
#if ENABLE_ANALYSIS
    [MenuItem("Hook/所有函数性能分析")]
    public static void HookInject()
    {
        AssemblyPostProcessorRun();
    }

    [MenuItem("Hook/特性[ProfilerSample]性能分析")]
    public static void HookProfilerSampleInject()
    {
        AssemblyPostProcessorRun(EAnalyzeType.PROFILESAMPLE);
    }

    [MenuItem("Hook/特性[FunctionAnalysis]函数性能分析")]
    public static void HookFunctionAnalysisInject()
    {
        AssemblyPostProcessorRun(EAnalyzeType.DEFINEFUNC);
    }

    [MenuItem("Hook/输出结果")]
    public static void HookUtilsReport()
    {

    }

    [PostProcessScene] //打包的时候回自动调用下面的注入方法
    public static void AssemblyPostProcessorRun()
    {
        try
        {
            if (Application.isPlaying || EditorApplication.isCompiling)
            {
                Debug.Log("You need stop play mode or wait compiling finished");
                return;
            }
            EditorApplication.LockReloadAssemblies();
            // 按路径读取程序集
            var readerParameters = new ReaderParameters { ReadSymbols = false };
            var assembly = AssemblyDefinition.ReadAssembly(AssemblyPath, readerParameters);
            if (assembly == null)
            {
                Debug.LogError(string.Format("InjectTool Inject Load assembly failed: {0}", AssemblyPath));
                return;
            }
            if (HookEditor.ProcessAssembly(assembly))
            {
                assembly.Write(AssemblyPath, new WriterParameters { WriteSymbols = true });
            }
            else
            {
                Debug.LogError(Path.GetFileName(AssemblyPath) + "所有函数注册无法被正确注入");
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
        EditorApplication.UnlockReloadAssemblies();
        Debug.Log("注入成功");
    }

    private static bool ProcessAssembly(AssemblyDefinition assembly)
    {
        bool hasProcessed = false;
        foreach (var module in assembly.Modules)
        {
            foreach (var type in module.Types)
            {
                if (type.IsAbstract || type.IsInterface)//过滤抽象类和接口
                    continue;
                foreach (var method in type.Methods)
                {
                    //过滤构造函数
                    if (method.Name == ".ctor" || method.Name == ".cctor")
                        continue;
                    //过滤抽象方法、虚函数、get、set方法
                    if (method.IsAbstract || method.IsVirtual || method.IsGetter || method.IsSetter)
                        continue;
                    //如果注入代码失败，可以打开下面的输出看看卡在了那个方法上。
                    //Debug.Log(method.Name + "======= " + type.Name + "======= " + type.BaseType.GenericParameters +" ===== "+ module.Name);
                    var hookUtilBegin = module.ImportReference(typeof(HookUtil).GetMethod("Begin", new[] { typeof(string) }));
                    var hookUtilEnd = module.ImportReference(typeof(HookUtil).GetMethod("End", new[] { typeof(string) }));
                    ILProcessor ilProcessor = method.Body.GetILProcessor();

                    Instruction first = method.Body.Instructions[0];
                    ilProcessor.InsertBefore(first, Instruction.Create(OpCodes.Ldstr, type.FullName + "." + method.Name));
                    ilProcessor.InsertBefore(first, Instruction.Create(OpCodes.Call, hookUtilBegin));

                    //解决方法中直接 return 后无法统计的bug 
                    //https://lostechies.com/gabrielschenker/2009/11/26/writing-a-profiler-for-silverlight-applications-part-1/

                    Instruction last = method.Body.Instructions[method.Body.Instructions.Count - 1];
                    Instruction lastInstruction = Instruction.Create(OpCodes.Ldstr, type.FullName + "." + method.Name);
                    ilProcessor.InsertBefore(last, lastInstruction);
                    ilProcessor.InsertBefore(last, Instruction.Create(OpCodes.Call, hookUtilEnd));

                    var jumpInstructions = method.Body.Instructions.Cast<Instruction>().Where(i => i.Operand == lastInstruction);
                    foreach (var jump in jumpInstructions)
                    {
                        jump.Operand = lastInstruction;
                    }
                    hasProcessed = true;
                }
            }
        }
        return hasProcessed;
    }
#endif

    //[PostProcessScene]
    static void AssemblyPostProcessorRun(EAnalyzeType analyzeType)
    {
        try
        {
            if (Application.isPlaying || EditorApplication.isCompiling)
            {
                Debug.Log("You need stop play mode or wait compiling finished");
                return;
            }
            EditorApplication.LockReloadAssemblies();
            // 按路径读取程序集
            var readerParameters = new ReaderParameters { ReadSymbols = false };
            var assembly = AssemblyDefinition.ReadAssembly(AssemblyPath, readerParameters);
            if (assembly == null)
            {
                Debug.LogError(string.Format("InjectTool Inject Load assembly failed: {0}", AssemblyPath));
                return;
            }
            switch (analyzeType)
            {
                case EAnalyzeType.PROFILESAMPLE:
                    {
                        if (HookEditor.ProcessAssemblyByAttributeWithoutParam(assembly, typeof(ProfilerSampleAttribute)))
                        {
                            assembly.Write(AssemblyPath, new WriterParameters { WriteSymbols = true });
                        }
                        else
                        {
                            Debug.Log(Path.GetFileName(AssemblyPath) + "没有找到Profiler.BeginSample无参数自定义特性方法");
                        }
                    }
                    break;
                case EAnalyzeType.DEFINEFUNC:
                    {
                        if (HookEditor.ProcessAssemblyByAttributeWithoutParam(assembly, typeof(FunctionAnalysisAttribute)))
                        {
                            assembly.Write(AssemblyPath, new WriterParameters { WriteSymbols = true });
                        }
                        else
                        {
                            Debug.Log(Path.GetFileName(AssemblyPath) + "没有找到带有自定义特性函数方法");
                        }
                    }
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
        EditorApplication.UnlockReloadAssemblies();
        Debug.Log("注入成功");
    }


    private static bool ProcessAssemblyByAttributeWithoutParam(AssemblyDefinition assembly, Type attributeType)
    {
        bool hasProcessed = false;
        var profilerSampleType = typeof(ProfilerSampleAttribute);
        var functionAnalysisType = typeof(FunctionAnalysisAttribute);
        if ((attributeType != profilerSampleType) && (attributeType != functionAnalysisType))
            return hasProcessed;
        var needInjectAttr = attributeType.FullName;
        foreach (var module in assembly.Modules)
        {
            foreach (var type in module.Types)
            {
                if (type.IsAbstract || type.IsInterface)//过滤抽象类和接口
                    continue;

                foreach (var method in type.Methods)
                {
                    //过滤构造函数
                    if (method.Name == ".ctor" || method.Name == ".cctor") //或者method.IsConstructor
                        continue;
                    //过滤抽象方法、虚函数、get、set方法
                    if (method.IsAbstract || method.IsVirtual || method.IsGetter || method.IsSetter)
                        continue;
                    //如果注入代码失败，可以打开下面的输出看看卡在了那个方法上。
                    //Debug.Log(method.Name + "======= " + type.Name + "======= " + type.BaseType.GenericParameters +" ===== "+ module.Name);
                    bool needInject = method.CustomAttributes.Any(typeAttribute => typeAttribute.AttributeType.FullName == needInjectAttr);
                    if (!needInject)
                    {
                        continue;
                    }

                    var Begin = module.ImportReference(typeof(UnityEngine.Profiling.Profiler).GetMethod("BeginSample", new[] { typeof(string) }));
                    var End = module.ImportReference(typeof(UnityEngine.Profiling.Profiler).GetMethod("EndSample"));

                    if (attributeType == functionAnalysisType)
                    {
                        Begin = module.ImportReference(typeof(HookUtil).GetMethod("Begin", new[] { typeof(string) }));
                        End = module.ImportReference(typeof(HookUtil).GetMethod("End", new[] { typeof(string) }));
                    }

                    ILProcessor ilProcessor = method.Body.GetILProcessor();

                    Instruction first = method.Body.Instructions[0];
                    ilProcessor.InsertBefore(first, Instruction.Create(OpCodes.Ldstr, type.FullName + "." + method.Name));
                    ilProcessor.InsertBefore(first, Instruction.Create(OpCodes.Call, Begin));

                    //解决方法中直接 return 后无法统计的bug 
                    //https://lostechies.com/gabrielschenker/2009/11/26/writing-a-profiler-for-silverlight-applications-part-1/

                    Instruction last = method.Body.Instructions[method.Body.Instructions.Count - 1];
                    Instruction lastInstruction = Instruction.Create(OpCodes.Call, End);
                    if (attributeType == functionAnalysisType)
                    {
                        ilProcessor.InsertBefore(last, Instruction.Create(OpCodes.Ldstr, type.FullName + "." + method.Name));
                    }
                    ilProcessor.InsertBefore(last, lastInstruction);

                    var jumpInstructions = method.Body.Instructions.Cast<Instruction>().Where(i => i.Operand == lastInstruction);
                    foreach (var jump in jumpInstructions)
                    {
                        jump.Operand = lastInstruction;
                    }
                    hasProcessed = true;
                }
            }
        }
        return hasProcessed;
    }
}
