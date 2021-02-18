﻿// Copyright © 2012-2020 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Vlingo.Xoom.Storage
{
    public class DatabaseParameters
    {
        private static string _xoomPrefix = "VLINGO_XOOM";
        private static string _queryModelPrefix = "query";
        private static string _combinationPattern = "{0}.{1}";
        private static readonly List<string> PropertiesKeys = new List<string>
        { "database", "database.name", "database.driver", "database.url",
                    "database.username", "database.password", "database.originator" };

        public Model Model { get; }
        public string? Database { get; }
        public string? Name { get; }
        public string? Driver{ get; }
        public string? Url { get; }
        public string? Username { get; }
        public string? Password { get; }
        public string? Originator { get; }
        public List<string> Keys { get; }
        public bool AutoCreate { get; }

        public DatabaseParameters(Model model, IReadOnlyDictionary<string, string> properties) : this(model, properties, true)
        {
        }

        public DatabaseParameters(Model model, IReadOnlyDictionary<string, string> properties, bool autoCreate)
        {
            Model = model;
            Keys = PrepareKeys();
            Database = ValueFromIndex(0, properties);
            Name = ValueFromIndex(1, properties);
            Driver = ValueFromIndex(2, properties);
            Url = ValueFromIndex(3, properties);
            Username = ValueFromIndex(4, properties);
            Password = ValueFromIndex(5, properties);
            Originator = ValueFromIndex(6, properties);
            AutoCreate = autoCreate;
        }

        private string? ValueFromIndex(int index, IReadOnlyDictionary<string, string> properties)
        {
            if (Keys.Count <= index)
            {
                return null;
            }
            return ApplicationProperty.ReadValue(Keys[index], properties);
        }

        private void Validate()
        {
            if (Database == null)
            {
                throw new DatabaseParameterNotFoundException(Model);
            }
            if (!string.Equals(Database, DatabaseCategory.InMemory.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                if (Name == null)
                {
                    throw new DatabaseParameterNotFoundException(Model, "name");
                }
                if (Driver == null)
                {
                    throw new DatabaseParameterNotFoundException(Model, "driver");
                }
                if (Url == null)
                {
                    throw new DatabaseParameterNotFoundException(Model, "url");
                }
                if (Username == null)
                {
                    throw new DatabaseParameterNotFoundException(Model, "username");
                }
                if (Originator == null)
                {
                    throw new DatabaseParameterNotFoundException(Model, "originator");
                }
            }
        }

        private List<string> PrepareKeys() => 
            PropertiesKeys.Where(x => Model.IsQueryModel).Select(key => string.Format(_combinationPattern, _queryModelPrefix, key)).ToList();

        public void MapToConfiguration() => Validate();
    }
}
