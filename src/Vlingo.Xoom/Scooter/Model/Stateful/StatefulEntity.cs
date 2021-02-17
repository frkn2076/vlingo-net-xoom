﻿// Copyright © 2012-2020 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System.Collections.Generic;
using System.Text;
using Vlingo.Symbio;

namespace Vlingo.Xoom.Scooter.Model.Stateful
{
    public abstract class StatefulEntity<S, C> : Entity<S, C> where S : class where C : class
    {
        private int _currentVersion;

        /// <summary>
        /// Answer my <see cref="currentVersion"/>, which, if zero, indicates that the
        /// receiver is being initially constructed or reconstituted.
        /// </summary>
        /// <returns><see cref="int"/></returns>
        public override int CurrentVersion() => _currentVersion;

        /// <summary>
        /// Answer my unique identity, which much be provided by
        /// my concrete extender by overriding.
        /// </summary>
        /// <returns><see cref="string"/></returns>
        public override abstract string Id();

        /// <summary>
        /// Construct my default state.
        /// </summary>
        protected StatefulEntity() => _currentVersion = 0;

        /// <summary>
        /// Apply my current <see cref="state"/> and <see cref="metadataValye"/> that was modified
        /// due to the descriptive <see cref="operation"/>.
        /// </summary>
        /// <param name="state"> the S typed state to apply</param>
        /// <param name="sources"> the <see cref="List<Source<C>>"/> instances to apply</param>
        /// <param name="metadataValue"> the string metadata value to apply along with the state</param>
        /// <param name="operation"> the string descriptive name of the operation that caused the state modification</param>
        protected void Apply(S state, List<Source<C>> sources, string metadataValue, string operation) => Apply(new Applied<S, C>(state, _currentVersion + 1, sources, Metadata(state, metadataValue, operation)));

        /// <summary>
        /// Apply my current <see cref="state"/> and <see cref="metadataValye"/> that was modified
        /// due to the descriptive <see cref="operation"/>.
        /// </summary>
        /// <param name="state"> the S typed state to apply</param>
        /// <param name="metadataValue"> the string metadata value to apply along with the state</param>
        /// <param name="operation"> the string descriptive name of the operation that caused the state modification</param>
        protected void Apply(S state, string metadataValue, string operation) => Apply(new Applied<S, C>(state, _currentVersion + 1, new List<Source<C>>(), Metadata(state, metadataValue, operation)));

        /// <summary>
        /// Apply my current <see cref="state"/> that was modified due to the descriptive <see cref="operation"/>.
        /// </summary>
        /// <param name="state"> state the S typed state to apply</param>
        /// <param name="sources"> the <see cref="List<Source<C>>"/> instances to apply</param>
        /// <param name="operation"> the string descriptive name of the operation that caused the state modification</param>
        protected void Apply(S state, List<Source<C>> sources, string operation) => Apply(new Applied<S, C>(state, _currentVersion + 1, sources, Metadata(state, null, operation)));

        /// <summary>
        /// Apply my current <see cref="state"/> that was modified due to the descriptive <see cref="operation"/>.
        /// </summary>
        /// <param name="state"> the S typed state to apply</param>
        /// <param name="operation"> the string descriptive name of the operation that caused the state modification</param>
        protected void Apply(S state, string operation) => Apply(new Applied<S, C>(state, _currentVersion + 1, new List<Source<C>>(), Metadata(state, null, operation)));

        /// <summary>
        /// Apply my current <see cref="state"/> and <see cref="sources"/>.
        /// </summary>
        /// <param name="state"> the S typed state to apply</param>
        /// <param name="sources"> the <see cref="List<Source<C>>"/> instances to apply</param>
        protected void Apply(S state, List<Source<C>> sources) => Apply(new Applied<S, C>(state, _currentVersion + 1, sources, Metadata(state, null, null)));

        /// <summary>
        /// Apply my current <see cref="state"/> and <see cref="source"/>.
        /// </summary>
        /// <param name="state"> the S typed state to apply</param>
        /// <param name="source"> the <see cref="Source<C>"/> instances to apply</param>
        protected void Apply(S state, Source<C> source) => Apply(new Applied<S, C>(state, _currentVersion + 1, new List<Source<C>>() { source }, Metadata(state, null, null)));

        /// <summary>
        /// Apply my current <see cref="state"/>.
        /// </summary>
        /// <param name="state"> the S typed state to apply</param>
        protected void Apply(S state) => Apply(new Applied<S, C>(state, _currentVersion + 1, new List<Source<C>>(), Metadata(state, null, null)));

        /// <summary>
        /// Answer a representation of a number of segments as a
        /// composite id. The implementor of <see cref="Id()"/> would use
        /// this method if the its id is built from segments.
        /// </summary>
        /// <param name="separator"> the string separator the insert between segments</param>
        /// <param name="idSegments"> the varargs String of one or more segments</param>
        /// <returns> <see cref="string"/> </returns>
        protected string IdFrom(string separator, params string[] idSegments)
        {
            var builder = new StringBuilder();
            builder.Append(idSegments[0]);
            for (int idx = 1; idx < idSegments.Length; ++idx)
            {
                builder.Append(separator).Append(idSegments[idx]);
            }
            return builder.ToString();
        }

        /// <summary>
        /// Received by my extender when my current state has been applied and restored.
        /// Must be overridden by my extender.
        /// </summary>
        /// <param name="state"> the S typed state</param>
        protected abstract void State(S state);

        /// <summary>
        /// Apply by setting <see cref="Applied{S, C}()"/> and setting state.
        /// </summary>
        /// <param name="applied">the <see cref="Applied<S, C>"/> to apply</param>
        private void Apply(Applied<S, C> applied)
        {
            _currentVersion = applied.stateVersion;
            Applied(applied);
            State(applied.state);
        }

        private Metadata Metadata(S state, string metadataValue, string operation) => Symbio.Metadata.With(state, metadataValue == null ? "" : metadataValue, operation == null ? "" : operation);
    }
}
