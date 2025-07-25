﻿//-----------------------------------------------------------------------
// <copyright file="Vector3Formatter.cs" company="Sirenix IVS">
// Copyright (c) 2018 Sirenix IVS
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//-----------------------------------------------------------------------

using EasyToolKit.ThirdParty.OdinSerializer;

[assembly: RegisterFormatter(typeof(Vector3Formatter))]

namespace EasyToolKit.ThirdParty.OdinSerializer
{
    using UnityEngine;

    /// <summary>
    /// Custom formatter for the <see cref="Vector3"/> type.
    /// </summary>
    /// <seealso cref="MinimalBaseFormatter{UnityEngine.Vector3}" />
    public class Vector3Formatter : MinimalBaseFormatter<Vector3>
    {
        private static readonly Serializer<float> FloatSerializer = Serializer.Get<float>();

        /// <summary>
        /// Reads into the specified value using the specified reader.
        /// </summary>
        /// <param name="value">The value to read into.</param>
        /// <param name="reader">The reader to use.</param>
        protected override void Read(ref Vector3 value, IDataReader reader)
        {
            value.x = Vector3Formatter.FloatSerializer.ReadValue(reader);
            value.y = Vector3Formatter.FloatSerializer.ReadValue(reader);
            value.z = Vector3Formatter.FloatSerializer.ReadValue(reader);
        }

        /// <summary>
        /// Writes from the specified value using the specified writer.
        /// </summary>
        /// <param name="value">The value to write from.</param>
        /// <param name="writer">The writer to use.</param>
        protected override void Write(ref Vector3 value, IDataWriter writer)
        {
            Vector3Formatter.FloatSerializer.WriteValue(value.x, writer);
            Vector3Formatter.FloatSerializer.WriteValue(value.y, writer);
            Vector3Formatter.FloatSerializer.WriteValue(value.z, writer);
        }
    }
}