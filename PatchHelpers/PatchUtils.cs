#nullable enable

using System;
using System.Linq;
using System.Reflection;
using static FTK_MultiMax_Rework_v2.Main;
using HarmonyMethod = HarmonyLib.HarmonyMethod;
using AccessTools = HarmonyLib.AccessTools;

namespace FTK_MultiMax_Rework_v2.PatchHelpers
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PatchType : Attribute { public readonly Type type; public PatchType(Type type) { this.type = type; } }

    [AttributeUsage(AttributeTargets.Method)]
    public class PatchMethod : Attribute { public readonly string methodName; public PatchMethod(string methodName) { this.methodName = methodName; } }

    [AttributeUsage(AttributeTargets.Method)]
    public class PatchPosition : Attribute { public readonly PatchPositions position; public PatchPosition(PatchPositions position) { this.position = position; } }

    [AttributeUsage(AttributeTargets.Method)]
    public class PatchParams : Attribute { public readonly Type[] parameters; public PatchParams(params Type[] parameters) { this.parameters = parameters; } }

    public enum PatchPositions { Prefix, Postfix, Transpiler, Finalizer, ILManipulator }

    public static class PatchUtils
    {
        public static void PatchClass(Type type) {
            Type target = Attr<PatchType>(type).type;
            Log($"Patching class {target.Name} with {type.Name}");

            foreach (var m in type.GetMethods(BindingFlags.Public | BindingFlags.Static)) {
                var patch = Attr<PatchMethod>(m);
                if (patch == null) continue;

                var pos = Attr<PatchPosition>(m)?.position ?? PatchPositions.Prefix;
                var h = new HarmonyMethod(m);
                Harmony.Patch(AccessTools.Method(target, patch.methodName, Attr<PatchParams>(m)?.parameters),
                    pos == PatchPositions.Prefix ? h : null,
                    pos == PatchPositions.Postfix ? h : null,
                    pos == PatchPositions.Transpiler ? h : null,
                    pos == PatchPositions.Finalizer ? h : null,
                    pos == PatchPositions.ILManipulator ? h : null);
                Log($"    Patched method {patch.methodName} with {m.Name}");
            }
        }

        public static Type[] GetTypesWithAttribute<T>(this Assembly asm) where T : Attribute =>
            asm.GetTypes().Where(t => Attr<T>(t) != null).ToArray();

        private static T? Attr<T>(MemberInfo m) where T : Attribute =>
            (T[])m.GetCustomAttributes(typeof(T), false) is T[] a && a.Length > 0 ? a[0] : null;
    }
}
