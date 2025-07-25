﻿//-----------------------------------------------------------------------
// <copyright file="UnityEventFormatter.cs" company="Sirenix IVS">
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

[assembly: RegisterFormatter(typeof(UnityEventFormatter<>), weakFallback: typeof(WeakUnityEventFormatter))] 

namespace EasyToolKit.ThirdParty.OdinSerializer
{
    using System;
    using UnityEngine.Events;

    /// <summary>
    /// Custom generic formatter for the <see cref="UnityEvent{T0}"/>, <see cref="UnityEvent{T0, T1}"/>, <see cref="UnityEvent{T0, T1, T2}"/> and <see cref="UnityEvent{T0, T1, T2, T3}"/> types.
    /// </summary>
    /// <typeparam name="T">The type of UnityEvent that this formatter can serialize and deserialize.</typeparam>
    /// <seealso cref="ReflectionFormatter{UnityEngine.Events.UnityEvent}" />
    public class UnityEventFormatter<T> : ReflectionFormatter<T> where T : UnityEventBase, new()
    {
        /// <summary>
        /// Get an uninitialized object of type <see cref="T" />.
        /// </summary>
        /// <returns>
        /// An uninitialized object of type <see cref="T" />.
        /// </returns>
        protected override T GetUninitializedObject()
        {
            return new T();
        }
    }

    public class WeakUnityEventFormatter : WeakReflectionFormatter
    {
        public WeakUnityEventFormatter(Type serializedType) : base(serializedType)
        {
        }

        protected override object GetUninitializedObject()
        {
            return Activator.CreateInstance(this.SerializedType);
        }
    }
}