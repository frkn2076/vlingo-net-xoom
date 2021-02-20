﻿// Copyright © 2012-2020 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System;

namespace Vlingo.Xoom.Annotation.Persistence
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class Adapters : Attribute
    {
        Type[] types;
        void Value()
        {
        }
    }
}
