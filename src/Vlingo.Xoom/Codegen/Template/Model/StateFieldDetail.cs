﻿// Copyright © 2012-2020 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Linq;
using Vlingo.Xoom.Codegen.Parameter;

namespace Vlingo.Xoom.Codegen.Template.Model
{
    public class StateFieldDetail
    {
        private static string _unknownFieldMessage = "{0} is not a field in {1} state";
        private static readonly List<string> _numericTypes = new List<string>() { "byte", "short", "int", "integer", "long", "double", "float" };

        public static string TypeOf(CodeGenerationParameter aggregate, string stateFieldName) => aggregate.RetrieveAllRelated(Label.StateField).Where(stateField => stateField.value == stateFieldName).Select(stateField => stateField.RetrieveRelatedValue(Label.FieldType)).FirstOrDefault()
            ?? throw new ArgumentException();

        public static string ResolveDefaultValue(CodeGenerationParameter aggregate, string stateFieldName)
        {
            var type = TypeOf(aggregate, stateFieldName);
            if (string.Equals(type, typeof(bool).Name, StringComparison.OrdinalIgnoreCase))
            {
                return "false";
            }
            if (_numericTypes.Contains(TypeOf(aggregate, stateFieldName).ToLower()))
            {
                return "0";
            }
            return "null";
        }
    }
}
