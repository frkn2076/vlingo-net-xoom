﻿// Copyright © 2012-2020 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Vlingo.Xoom.Annotation.Initializer.ContentLoader;
using Vlingo.Xoom.Codegen.Content;
using Vlingo.Xoom.Codegen.Parameter;
using Vlingo.Xoom.Codegen.Template;
using Vlingo.Xoom.Codegen.Template.Storage;

namespace Vlingo.Xoom.Codegen
{
    public class CodeGenerationContext
    {
        private FileStream _filer;
        private Type _source;
        private readonly CodeGenerationParameters _parameters;
        private readonly List<ContentBase> _contents = new List<ContentBase>();
        private readonly List<TemplateData> _templatesData = new List<TemplateData>();

        public static CodeGenerationContext Empty() => new CodeGenerationContext();

        public static CodeGenerationContext Using(FileStream filer, Type source) => new CodeGenerationContext(filer, source);

        public static CodeGenerationContext With(IReadOnlyDictionary<Label, string> parameters) => new CodeGenerationContext().On(parameters);

        public static CodeGenerationContext With(CodeGenerationParameters parameters) => new CodeGenerationContext().On(parameters);

        private CodeGenerationContext() : this(null, null)
        {
        }

        private CodeGenerationContext(FileStream filer, Type source)
        {
            _filer = filer;
            _source = source;
            _parameters = CodeGenerationParameters.From(Label.GenerationLocation, filer == null ? CodeGenerationLocationType.EXTERNAL.ToString() : CodeGenerationLocationType.INTERNAL.ToString());
        }

        public CodeGenerationContext Contents(List<ContentLoaderBase<string>> loaders)
        {
            loaders.Where(x => x.ShouldLoad).ToList().ForEach(x => x.Load(this));
            return this;
        }

        public CodeGenerationContext Contents(params ContentBase[] contents)
        {
            _contents.AddRange(contents);
            return this;
        }

        public CodeGenerationContext With(Label label, string value)
        {
            On(CodeGenerationParameters.From(label, value));
            return this;
        }

        public CodeGenerationContext On(IReadOnlyDictionary<Label, string> parameters)
        {
            _parameters.AddAll(parameters);
            return this;
        }

        public CodeGenerationContext On(CodeGenerationParameters parameters)
        {
            _parameters.AddAll(parameters);
            return this;
        }

        public T ParameterOf<T>(Label label) => ParameterOf<T>(label, value => (T)Convert.ChangeType(value, typeof(T)));

        public T ParameterOf<T>(Label label, Func<string, T> mapper)
        {
            var value = _parameters.RetrieveValue(label);
            return mapper(value);
        }

        public IEnumerable<TemplateData> TemplateParametersOf(TemplateStandardType standard) => _templatesData.Where(templateData => templateData.HasStandard(standard));

        public void registerTemplateProcessing(TemplateData templateData, string text)
        {
            AddContent(templateData.Standard(), new TemplateFile(this, templateData), text);
            _templatesData.Add(templateData);
        }

        public CodeGenerationContext AddContent(TemplateStandard standard, TemplateFile file, string text)
        {
            _contents.Add(ContentBase.With(standard, file, _filer, _source, text));
            return this;
        }

        public CodeGenerationContext AddContent(TemplateStandard standard, Type type)
        {
            _contents.Add(ContentBase.With(standard, type));
            return this;
        }

        public CodeGenerationContext AddContent(TemplateStandard standard, Type protocolType, Type actorType)
        {
            _contents.Add(ContentBase.With(standard, protocolType, actorType));
            return this;
        }

        public IDictionary<ModelType, Storage.DatabaseCategory> Databases()
        {
            var databases = new Dictionary<ModelType, Storage.DatabaseCategory>() { };
            if (ParameterOf<bool>(Label.Cqrs))
            {
                databases.Add(ModelType.Ccommand, ParameterOf(Label.CommandModelDatabase, name => DatabaseType.GetOrDefault(name, Storage.DatabaseCategory.InMemory)));
                databases.Add(ModelType.Query, ParameterOf(Label.CommandModelDatabase, name => DatabaseType.GetOrDefault(name, Storage.DatabaseCategory.InMemory)));
                return databases;
            }

            databases.Add(ModelType.Domain, ParameterOf(Label.CommandModelDatabase, name => DatabaseType.GetOrDefault(name, Storage.DatabaseCategory.InMemory)));
            return databases;
        }

        public bool IsInternalGeneration => CodeGenerationLocation.IsInternal(ParameterOf<CodeGenerationLocationType>(Label.GenerationLocation));

        public IEnumerable<CodeGenerationParameter> ParametersOf(Label label) => _parameters.RetrieveAll(label);

        public bool HasParameter(Label label) => ParameterOf<string>(label) != null && ParameterOf<string>(label).Trim() != string.Empty;

        public IReadOnlyList<ContentBase> Contents() => _contents;

        public CodeGenerationParameters Parameters() => _parameters;
    }
}
