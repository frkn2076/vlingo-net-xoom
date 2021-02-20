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
    public class AggregateArgumentsFormat
    {
        AggregateArgumentsFormat METHOD_INVOCATION = new MethodInvocation("stage");
        AggregateArgumentsFormat SIGNATURE_DECLARATION = new SignatureDeclaration();

        public string Format(CodeGenerationParameter parameter)
        {
            return Format(parameter, MethodScopeType.Instance);
        }

        public virtual string Format(CodeGenerationParameter parameter, MethodScopeType scope) => throw new NotImplementedException();

        public class SignatureDeclaration : AggregateArgumentsFormat
        {
            private static readonly string _signaturePatttern = "final {0} {1}";
            private static readonly string _stateArgument = string.Format(_stateArgument, "Stage", "stage");

            public string Format(CodeGenerationParameter parameter, MethodScopeType scope)
            {
                var args = scope == MethodScopeType.Static ? new List<string>() { _stateArgument } : new List<string>();
                return string.Join(", ", new List<List<string>>() { args, FormatMethodParameters(parameter) }.SelectMany(x => x));
            }

            private List<string> FormatMethodParameters(CodeGenerationParameter parameter)
            {
                return parameter.RetrieveAllRelated(ResolveFieldsLabel(parameter)).Select(param =>
                {
                    var paramType = StateFieldDetail.TypeOf(param.Parent(Label.Aggregate), param.value);
                    return string.Format(_signaturePatttern, paramType, param.value);
                }).ToList();
            }

            private Label ResolveFieldsLabel(CodeGenerationParameter parameter) => parameter.IsLabeled(Label.Aggregate) ? Label.StateField : Label.MethodParameter;
        }

        public class MethodInvocation : AggregateArgumentsFormat
        {
            private readonly string _carrier;
            private readonly string _stageVariableName;
            private static readonly string _filedAccessPattern = "{0}.{1}";

            public MethodInvocation(string stageVariableName) : this(stageVariableName, "")
            {
            }

            public MethodInvocation(string stageVariableName, string carrier)
            {
                _carrier = carrier;
                _stageVariableName = stageVariableName;
            }

            public override string Format(CodeGenerationParameter method, MethodScopeType scope)
            {
                var args = scope == MethodScopeType.Static ? new List<string>() { _stageVariableName } : new List<string>();
                return string.Join(", ", new List<List<string>>() { args, FormatMethodParameters(method) }.SelectMany(x => x));
            }

            private List<string> FormatMethodParameters(CodeGenerationParameter method) => method.RetrieveAllRelated(Label.MethodParameter).Select(param => string.IsNullOrEmpty(_carrier) ? param.value : string.Format(_filedAccessPattern, _carrier, param.value)).ToList();
        }
    }
}
