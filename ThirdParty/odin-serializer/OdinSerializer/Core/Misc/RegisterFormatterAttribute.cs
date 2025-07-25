﻿//-----------------------------------------------------------------------
// <copyright file="RegisterFormatterAttribute.cs" company="Sirenix IVS">
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

namespace EasyToolKit.ThirdParty.OdinSerializer
{
    using System;

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class RegisterFormatterAttribute : Attribute
    {
        public Type FormatterType { get; private set; }
        public Type WeakFallback { get; private set; }
        public int Priority { get; private set; }

        public RegisterFormatterAttribute(Type formatterType, int priority = 0)
        {
            this.FormatterType = formatterType;
            this.Priority = priority;
        }

        public RegisterFormatterAttribute(Type formatterType, Type weakFallback, int priority = 0)
        {
            this.FormatterType = formatterType;
            this.WeakFallback = weakFallback;
            this.Priority = priority;
        }
    }
}