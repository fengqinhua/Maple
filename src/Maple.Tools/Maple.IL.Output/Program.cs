using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Maple.IL.Output
{
    class Program
    {
        static void Main(string[] args)
        {
            AssemblyName MyAssemblyName = new AssemblyName("AssemblyName");
            AssemblyBuilder MyAssemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(MyAssemblyName, AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder MyModuleBuilder = MyAssemblyBuilder.DefineDynamicModule("MyModule", "MyModule.dll");//创建模块
            TypeBuilder MyType = MyModuleBuilder.DefineType("MyClass", TypeAttributes.Public);//创建类型(类)


            Console.WriteLine("输出完成!");
        }
    }
}
