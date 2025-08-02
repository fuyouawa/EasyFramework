using System;
using System.Text;
using EasyToolKit.ThirdParty.OdinSerializer;

namespace EasyToolKit.Core.Editor
{
    public static class SerializationDataExtensions
    {
        public static int CalculateNearlyBinarySize(this SerializationData serializationData)
        {
            if (!serializationData.ContainsData)
                return 0;

            switch (serializationData.SerializedFormat)
            {
                case DataFormat.Binary:
                    return serializationData.SerializedBytes.Length;
                case DataFormat.JSON:
                    return Encoding.UTF8.GetBytes(serializationData.SerializedBytesString).Length;
                case DataFormat.Nodes:
                    {
                        int size = 0;
                        foreach (var node in serializationData.SerializationNodes)
                        {
                            var sb = new StringBuilder();
                            sb.Append(nameof(node.Name) + ": ");
                            if (node.Name != null)
                                sb.AppendLine(node.Name);

                            sb.Append(nameof(node.Entry) + ": ");
                            sb.AppendLine(((int)node.Entry).ToString());

                            sb.Append(nameof(node.Data) + ": ");
                            sb.AppendLine(node.Data);

                            size += Encoding.UTF8.GetBytes(sb.ToString()).Length;
                        }
                        return size;
                    }
                default:
                    throw new ArgumentOutOfRangeException(nameof(serializationData.SerializedFormat), serializationData.SerializedFormat, "Unknown serialized format");
            }
        }
    }
}