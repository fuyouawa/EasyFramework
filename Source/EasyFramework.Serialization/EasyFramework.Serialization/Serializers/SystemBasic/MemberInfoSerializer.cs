// using System;
// using System.Reflection;
//
// namespace EasyFramework.Serialization
// {
//     public class MemberInfoSerializer : EasySerializer<MemberInfo>
//     {
//         enum Types
//         {
//             None,
//             Field,
//             Property,
//             Method
//         }
//
//         private static readonly EasySerializer<Types> MemberTypesSerializer = GetSerializer<Types>();
//         private static readonly EasySerializer<MethodInfo> MethodInfoSerializer = GetSerializer<MethodInfo>();
//
//         public override void Process(string name, ref MemberInfo value, IArchive archive)
//         {
//             var type = Types.None;
//
//             if (archive.ArchiveIoType == ArchiveIoTypes.Output)
//             {
//                 if (value != null)
//                 {
//                     if (value is MethodInfo)
//                         type = Types.Method;
//                     else
//                         throw new NotImplementedException();
//                 }
//             }
//             
//             archive.StartNode();
//
//             MemberTypesSerializer.Process("Type", ref type, archive);
//
//             switch (type)
//             {
//                 case Types.None:
//                     value = null;
//                     return;
//                 case Types.Field:
//                     throw new NotImplementedException();
//                 case Types.Property:
//                     throw new NotImplementedException();
//                 case Types.Method:
//                     var method = (MethodInfo)value;
//                     MethodInfoSerializer.Process("MethodInfo", ref method, archive);
//                     value = method;
//                     break;
//                 default:
//                     throw new ArgumentOutOfRangeException();
//             }
//
//             archive.FinishNode();
//         }
//     }
// }
