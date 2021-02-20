﻿// Copyright © 2012-2020 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System;

namespace Vlingo.Xoom.Codegen
{
    public class CodeGenerationException : SystemException
    {

        public CodeGenerationException(Exception exception) : base(exception.Message, exception)
        {
        }

        public CodeGenerationException(string message) : base(message)
        {
        }
    }
}
