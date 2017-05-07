﻿using System;
using System.Threading;
using System.Threading.Tasks;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerialization.Graph.ValueGraph
{
    internal class EnumValueNode : ValueValueNode
    {
        public EnumValueNode(Node parent, string name, TypeNode typeNode) : base(parent, name, typeNode)
        {
        }

        internal override void SerializeOverride(BoundedStream stream, EventShuttle eventShuttle)
        {
            var typeNode = (EnumTypeNode) TypeNode;
            var enumInfo = typeNode.EnumInfo;
            var value = enumInfo.EnumValues != null ? enumInfo.EnumValues[(Enum) BoundValue] : BoundValue;
            Serialize(stream, value, enumInfo.SerializedType, enumInfo.EnumValueLength);
        }

        internal override void DeserializeOverride(BoundedStream stream, EventShuttle eventShuttle)
        {
            var enumInfo = GetEnumInfo();
            Deserialize(stream, enumInfo.SerializedType, enumInfo.EnumValueLength);
            SetValueFromEnum();
        }

        internal override async Task DeserializeOverrideAsync(BoundedStream stream, EventShuttle eventShuttle, CancellationToken cancellationToken)
        {
            var enumInfo = GetEnumInfo();
            await DeserializeAsync(stream, enumInfo.SerializedType, enumInfo.EnumValueLength, cancellationToken).ConfigureAwait(false);
            SetValueFromEnum();
        }

        private EnumInfo GetEnumInfo()
        {
            var typeNode = (EnumTypeNode) TypeNode;
            var enumInfo = typeNode.EnumInfo;
            return enumInfo;
        }

        private void SetValueFromEnum()
        {
            var enumInfo = GetEnumInfo();
            var value = GetValue(enumInfo.SerializedType);

            if (enumInfo.ValueEnums != null)
            {
                value = enumInfo.ValueEnums[(string) value];
            }

            var underlyingValue = value.ConvertTo(enumInfo.UnderlyingType);

            Value = Enum.ToObject(TypeNode.BaseSerializedType, underlyingValue);
        }
    }
}