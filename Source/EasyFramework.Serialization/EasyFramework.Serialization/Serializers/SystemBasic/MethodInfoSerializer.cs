// using System.Linq;
// using System;
// using System.Reflection;
//
// namespace EasyFramework.Serialization
// {
//     //TODO 其他情况的处理
//     [EasySerializerConfig(EasySerializerProiority.SystemBasic)]
//     public class MethodInfoSerializer : EasySerializer<MethodInfo>
//     {
//         class Store
//         {
//             public Type DeclaringType;
//             public string Name;
//             public Type[] Parameters;
//         }
//
//         private static readonly EasySerializer<Type> TypeSerializer = GetSerializer<Type>();
//         private static readonly EasySerializer<string> StringSerializer = GetSerializer<string>();
//         private static readonly EasySerializer<Type[]> TypeArraySerializer = GetSerializer<Type[]>();
//
//         public override void Process(string name, ref MethodInfo value, IArchive archive)
//         {
//             var store = new Store();
//             if (archive.ArchiveIoType == ArchiveIoTypes.Output)
//             {
//                 store.DeclaringType = value.DeclaringType;
//                 store.Name = value.Name;
//                 store.Parameters = value.GetParameters().Select(p => p.ParameterType).ToArray();
//             }
//
//             archive.StartNode();
//
//             TypeSerializer.Process(nameof(Store.DeclaringType), ref store.DeclaringType, archive);
//             StringSerializer.Process(nameof(Store.Name), ref store.Name, archive);
//             TypeArraySerializer.Process(nameof(Store.Parameters), ref store.Parameters, archive);
//
//             archive.FinishNode();
//
//             if (archive.ArchiveIoType == ArchiveIoTypes.Input)
//             {
//                 var all = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic |
//                           BindingFlags.Static;
//
//                 value = store.DeclaringType.GetMethods(all)
//                     .FirstOrDefault(m =>
//                     {
//                         if (m.Name != store.Name)
//                             return false;
//                         var parameters = m.GetParameters();
//                         if (parameters.Length != store.Parameters.Length)
//                             return false;
//                         for (int i = 0; i < parameters.Length; i++)
//                         {
//                             if (store.Parameters[i] != parameters[i].ParameterType)
//                                 return false;
//                         }
//
//                         return true;
//                     });
//             }
//         }
//     }
// }
