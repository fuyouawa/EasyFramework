﻿//-----------------------------------------------------------------------
// <copyright file="SerializationAbortException.cs" company="Sirenix IVS">
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

    /// <summary>
    /// An exception thrown when the serialization system has encountered an issue so severe that serialization is being aborted. If this exception is caught in the serialization system somewhere, it should be rethrown.
    /// </summary>
    public class SerializationAbortException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public SerializationAbortException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public SerializationAbortException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}