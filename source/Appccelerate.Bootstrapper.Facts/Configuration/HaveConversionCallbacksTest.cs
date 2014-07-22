//-------------------------------------------------------------------------------
// <copyright file="HaveConversionCallbacksTest.cs" company="Appccelerate">
//   Copyright (c) 2008-2014
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
//-------------------------------------------------------------------------------

namespace Appccelerate.Bootstrapper.Configuration
{
    using System.Collections.Generic;

    using Appccelerate.Bootstrapper.Configuration.Internals;
    using FakeItEasy;
    using FluentAssertions;

    using Xunit;

    public class HaveConversionCallbacksTest
    {
        [Fact]
        public void ConversionCallbacks_ExtensionNotIHaveConversionCallbacks_ShouldUseEmptyOne()
        {
            var extension = A.Fake<IExtension>();

            var testee = new HaveConversionCallbacks(extension);
            testee.ConversionCallbacks.Should().BeEmpty();
        }

        [Fact]
        public void ConversionCallbacks_ExtensionIsIHaveConversionCallbacks_ShouldAcquireCallbacksFromExtension()
        {
            var extension = A.Fake<IExtension>(builder => builder.Implements(typeof(IHaveConversionCallbacks)));
            var consumer = extension as IHaveConversionCallbacks;
            var expected = new KeyValuePair<string, IConversionCallback>("Value", A.Fake<IConversionCallback>());

            A.CallTo(() => consumer.ConversionCallbacks).Returns(new Dictionary<string, IConversionCallback> { { expected.Key, expected.Value } });

            var testee = new HaveConversionCallbacks(extension);
            testee.ConversionCallbacks.Should().Contain(expected);
        }
    }
}