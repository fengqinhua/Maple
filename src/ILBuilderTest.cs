using Maple.Core.Data.DataProviders;
using Maple.Core.Data.DataSettings;
using Maple.Core.Data.DbMappers;
using Maple.Core.Data.DbTranslators;
using Maple.Core.Data.ModelConfiguration;
using Maple.Core.Reflection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Maple.IL.Output
{
    class ILBuilderTest
    {
        static string binaryName = "LILBuilderTestDemo.exe";
        static string namespaceName = "LILBuilderTestDemo";
        static string typeName = "LILBuilderTest";

        static AssemblyBuilder assemblyBuilder;
        static ModuleBuilder moduleBuilder;
        static TypeBuilder typeBuilder;
        static MethodBuilder testMethod;
        static MethodBuilder toEntityMethod;
 
        static void getFieldValue()
        {
            PropertyInfo vo = typeof(UserIL).GetProperties().FirstOrDefault(f => f.Name == "Address");
            PropertyInfo property = typeof(AddressIL).GetProperties().FirstOrDefault(f => f.Name == "Street");
            testMethod = typeBuilder.DefineMethod("Test", MethodAttributes.Public, typeof(object), new Type[] { typeof(object) });//创建方法
            CreatePropertyGetter(testMethod, vo, property);
        }

        static void DataReaderToEntity()
        {
            //创建方法
            toEntityMethod = typeBuilder.DefineMethod("ToEntity", MethodAttributes.Public, typeof(object), new Type[] { typeof(IDataReader) });
            ILGenerator il = toEntityMethod.GetILGenerator();

            DbDriveFactories.SetFactory<MySql.Data.MySqlClient.MySqlClientFactory>(new MySQLTranslator().ProviderInvariantName);

            EntityConfigurationFactory.SetConfiguration(typeof(UserIL), typeof(UserEntityConfigurationIL));
            IEntityMapper entityMapper = EntityMapperFactory.Instance.GetEntityMapper(typeof(UserIL));

            IDataProviderFactory DataProviderFactory = new DataProviderFactory();
            DataProviderFactory.AddDataSettings(getDefaultDataSetting());

            IDataProvider dataProvider = DataProviderFactory.CreateProvider("test");
            string sql = "SELECT Id,USERNAME,Age,Height,Six,ExtensionData,OrgId,TenantId,Address_CityId,Address_Street,ADDRESS_NUM,IsDeleted,DeleterUserId,DeletionTime,LastModificationTime,LastModifierUserId,CreationTime,CreatorUserId FROM TEST_USER WHERE Id = 13873372135800832 ORDER BY Id ASC";
            dataProvider.ExecuteReader(new Core.Data.SqlStatement(CommandType.Text, sql), (reader) =>
              {
                  while (reader.Read())
                  {
                      UserIL User = (UserIL)ToEntity(reader);
                      //Maple.Core.Reflection.TypeDeserializerEmit.CreateDataReaderDeserializer2(entityMapper, reader);

                      Maple.Core.Reflection.TypeDeserializerEmit.BuildFunction(entityMapper, reader, il);
                      try
                      {
                          UserIL obj = (UserIL)Maple.Core.Reflection.TypeDeserializerEmit.CreateDataReaderDeserializer(entityMapper, reader)(reader);

                      }
                      catch
                      {

                      }
                      

                      //DataReaderDeserializer deserializer = entityMapper.GetDataReaderDeserializer(reader);
                      //obj = (User)deserializer(reader);


                      break;
                  }
              });
        }

        public static void Generate()
        {
            InitAssembly();
            typeBuilder = moduleBuilder.DefineType(namespaceName + "." + typeName, TypeAttributes.Public);

            getFieldValue();
            DataReaderToEntity();

            //Emit_TestMethod();
            //GenerateMain();
            //assemblyBuilder.SetEntryPoint(mainMethod, PEFileKinds.ConsoleApplication);
            SaveAssembly();
            Console.WriteLine("生成成功");
        }

        public static object ToEntity(IDataReader P_0)
        {
            //IL_010c: Expected O, but got I8
            //IL_0166: Expected O, but got I8
            //IL_0195: Expected O, but got I8
            UserIL user = new UserIL();
            user.Address = new AddressIL();
            user.Id = P_0.GetInt64(0);
            if (!P_0.IsDBNull(1))
            {
                user.Name = P_0.GetString(1);
            }
            user.Age = P_0.GetInt32(2);
            user.Height = P_0.GetDouble(3);
            user.Six = (Six)P_0.GetInt32(4);
            if (!P_0.IsDBNull(5))
            {
                user.ExtensionData = P_0.GetString(5);
            }
            user.OrgId = P_0.GetInt64(6);
            user.TenantId = P_0.GetInt64(7);
            user.Address.CityId = P_0.GetGuid(8);
            if (!P_0.IsDBNull(9))
            {
                user.Address.Street = P_0.GetString(9);
            }
            user.Address.Number = P_0.GetInt32(10);
            user.IsDeleted = ((P_0.GetInt16(11) != 0) ? true : false);
            if (!P_0.IsDBNull(12))
            {
                user.DeleterUserId = P_0.GetInt64(12);
            }
            if (!P_0.IsDBNull(13))
            {
                user.DeletionTime = P_0.GetDateTime(13);
            }
            if (!P_0.IsDBNull(14))
            {
                user.LastModificationTime = P_0.GetDateTime(14);
            }
            if (!P_0.IsDBNull(15))
            {
                user.LastModifierUserId = P_0.GetInt64(15);
            }
            user.CreationTime = P_0.GetDateTime(16);
            if (!P_0.IsDBNull(17))
            {
                user.CreatorUserId = P_0.GetInt64(17);
            }
            return user;
        }

        static void InitAssembly()
        {
            AssemblyName assemblyName = new AssemblyName(namespaceName);
            assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name, binaryName);
        }

        static void SaveAssembly()
        {
            Type t = typeBuilder.CreateType(); //完成Type，这是必须的
            assemblyBuilder.Save(binaryName);
        }

        private static string CreatePropertyGetter(MethodBuilder MyMethodBuilder, PropertyInfo vo, PropertyInfo property)
        {
            ConstructorInfo constructor = vo.PropertyType.GetConstructor(Type.EmptyTypes);
            MethodInfo getVOMethod = vo.GetGetMethod(true);
            MethodInfo setVOMethod = vo.GetSetMethod(true);
            MethodInfo getMethod = property.GetGetMethod(true);

            ILGenerator il = MyMethodBuilder.GetILGenerator();

            Label IL_001c = il.DefineLabel();
            Label IL_002f = il.DefineLabel();

            LocalBuilder local0 = il.DeclareLocal(vo.ReflectedType);
            LocalBuilder local1 = il.DeclareLocal(typeof(object));
            LocalBuilder local2 = il.DeclareLocal(typeof(bool));


            il.Emit(OpCodes.Nop);
            //将索引为 1 的参数加载到计算堆栈上
            il.Emit(OpCodes.Ldarg_1);
            //测试对象引用（O 类型）是否为特定类的实例。
            il.Emit(OpCodes.Isinst, vo.ReflectedType);
            //从计算堆栈的顶部弹出当前值并将其存储到索引 1 处的局部变量列表中。
            il.Emit(OpCodes.Stloc_0);
            //将索引 1 处的局部变量加载到计算堆栈上
            il.Emit(OpCodes.Ldloc_0);
            //对对象调用后期绑定方法，并且将返回值推送到计算堆栈上。
            il.EmitCall(OpCodes.Callvirt, getVOMethod, null);
            //将空引用（O 类型）推送到计算堆栈上
            il.Emit(OpCodes.Ldnull);
            //比较两个值。如果这两个值相等，则将整数值 1 (int32) 推送到计算堆栈上；否则，将 0 (int32) 推送到计算堆栈上。
            il.Emit(OpCodes.Ceq);
            //将整数值 0 作为 int32 推送到计算堆栈上
            il.Emit(OpCodes.Ldc_I4_0);
            //比较两个值。如果这两个值相等，则将整数值 1 (int32) 推送到计算堆栈上；否则，将 0 (int32) 推送到计算堆栈上。
            il.Emit(OpCodes.Ceq);
            //从计算堆栈的顶部弹出当前值并将其存储到索引 2 处的局部变量列表中。
            il.Emit(OpCodes.Stloc_2);
            //将索引 2 处的局部变量加载到计算堆栈上
            il.Emit(OpCodes.Ldloc_2);
            //如果 value 为 true、非空或非零，则将控制转移到目标指令（短格式）
            il.Emit(OpCodes.Brtrue_S, IL_001c);

            //将空引用（O 类型）推送到计算堆栈上
            il.Emit(OpCodes.Ldnull);
            //从计算堆栈的顶部弹出当前值并将其存储到索引 1 处的局部变量列表中。
            il.Emit(OpCodes.Stloc_1);
            //无条件地将控制转移到目标指令（短格式）
            il.Emit(OpCodes.Br_S, IL_002f);

            il.MarkLabel(IL_001c);
            //将索引 1 处的局部变量加载到计算堆栈上
            il.Emit(OpCodes.Ldloc_0);
            //对对象调用后期绑定方法，并且将返回值推送到计算堆栈上。
            il.EmitCall(OpCodes.Callvirt, getVOMethod, null);
            //对对象调用后期绑定方法，并且将返回值推送到计算堆栈上
            il.EmitCall(OpCodes.Callvirt, getMethod, null);

            EmitCastToReference(il, property.PropertyType);
            //从计算堆栈的顶部弹出当前值并将其存储到索引 1 处的局部变量列表中。
            il.Emit(OpCodes.Stloc_1);
            il.Emit(OpCodes.Br_S, IL_002f);

            il.MarkLabel(IL_002f);
            il.Emit(OpCodes.Ldloc_1);
            il.Emit(OpCodes.Ret);

            return null;
        }
        private static void EmitCastToReference(ILGenerator il, Type type)
        {
            if (type.IsValueType)
                il.Emit(OpCodes.Unbox_Any, type);
            else
                il.Emit(OpCodes.Castclass, type);
        }

        private static DataSetting getDefaultDataSetting()
        {
            DataSetting ds = new DataSetting()
            {
                Name = "test",
                DataConnectionString = "Server=127.0.0.1;port=3306;Database=mapleleaf;Uid=root;Pwd=root;charset=utf8;SslMode=none;",
                DataSouceType = Core.Data.DataSouceType.MySQL
            };
            return ds;
        }
    }
}
