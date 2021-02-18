﻿// Copyright © 2012-2020 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System;

namespace Vlingo.Xoom.Annotation.AutoDispatch
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class AutoDispatch : Attribute
    {
        private string _path;
        private Type _handler;
        public AutoDispatch(string path, Type handler)
        {
            _path = path;
            _handler = handler;
        }
    }
}
