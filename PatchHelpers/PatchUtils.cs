#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static FTK_MultiMax_Rework_v2.Main;
using AccessTools = HarmonyLib.AccessTools;
using HarmonyMethod = HarmonyLib.HarmonyMethod;

namespace FTK_MultiMax_Rework_v2.PatchHelpers
{
    // Attributes for declarative Harmony patching

    [AttributeUsage(AttributeTargets.Class)]
    public class PatchType : Attribute
    {
        public readonly Type type;
        public PatchType(Type type) { this.type = type; }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class PatchMethod : Attribute
    {
        public readonly string methodName;
        public PatchMethod(string methodName) { this.methodName = methodName; }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class PatchPosition : Attribute
    {
        public readonly PatchPositions position;
        public PatchPosition(PatchPositions position) { this.position = position; }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class PatchParams : Attribute
    {
        public readonly Type[] parameters;
        public PatchParams(params Type[] parameters) { this.parameters = parameters; }
    }

    public enum PatchPositions
    {
        Prefix,
        Postfix,
        Transpiler,
        Finalizer,
        ILManipulator
    }

    // Reflection-based patch discovery and application

    public static class PatchUtils
    {
        public static void PatchClass(Type type)
        {
            Type patchedClass = GetAttribute<PatchType>(type).type;
            Log($"Patching class {patchedClass.Name} with {type.Name}");

            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
            {
                var patchAttr = GetAttribute<PatchMethod>(method);
                if (patchAttr == null) continue;

                var position = GetAttribute<PatchPosition>(method)?.position ?? PatchPositions.Prefix;
                var parameters = GetAttribute<PatchParams>(method)?.parameters;

                MethodInfo original = AccessTools.Method(patchedClass, patchAttr.methodName, parameters);
                HarmonyMethod harmony = new HarmonyMethod(method);

                Harmony.Patch(original,
                    position == PatchPositions.Prefix ? harmony : null,
                    position == PatchPositions.Postfix ? harmony : null,
                    position == PatchPositions.Transpiler ? harmony : null,
                    position == PatchPositions.Finalizer ? harmony : null,
                    position == PatchPositions.ILManipulator ? harmony : null);

                Log($"    Patched method {patchAttr.methodName} with {method.Name}");
            }
        }

        public static Type[] GetTypesWithAttribute<T>(this Assembly assembly) where T : Attribute
        {
            return assembly.GetTypes().Where(t => t.GetCustomAttributes(typeof(T), false).Length > 0).ToArray();
        }

        private static T? GetAttribute<T>(MemberInfo member) where T : Attribute
        {
            var attrs = (T[])member.GetCustomAttributes(typeof(T), false);
            return attrs.Length > 0 ? attrs[0] : null;
        }
    }
}
