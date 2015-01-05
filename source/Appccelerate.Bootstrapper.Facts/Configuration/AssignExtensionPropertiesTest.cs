//-------------------------------------------------------------------------------
// <copyright file="AssignExtensionPropertiesTest.cs" company="Appccelerate">
//   Copyright (c) 2008-2015
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
    using System.Reflection;
    using Appccelerate.Bootstrapper.Configuration.Internals;
    using Appccelerate.Formatters;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class AssignExtensionPropertiesTest
    {
        private const string SomeExtensionPropertyName = "SomeProperty";

        private const string SomeExtensionPropertyValue = "AnyValue";

        private readonly IConsumeConfiguration consumer;
        private readonly IHaveConversionCallbacks conversionCallbacksProvider;
        private readonly IReflectExtensionProperties extensionPropertyReflector;
        private readonly IConversionCallback conversionCallback;
        private readonly IHaveDefaultConversionCallback defaultConversionCallbackProvider;

        private readonly AssignExtensionProperties testee;

        public AssignExtensionPropertiesTest()
        {
            this.consumer = A.Fake<IConsumeConfiguration>();
            this.extensionPropertyReflector = A.Fake<IReflectExtensionProperties>();
            this.conversionCallbacksProvider = A.Fake<IHaveConversionCallbacks>();
            this.defaultConversionCallbackProvider = A.Fake<IHaveDefaultConversionCallback>();
            this.conversionCallback = A.Fake<IConversionCallback>();

            this.testee = new AssignExtensionProperties();
        }

        [Fact]
        public void Assign_ShouldReflectPropertiesOfExtensions()
        {
            this.SetupEmptyConsumerConfiguration();

            this.testee.Assign(this.extensionPropertyReflector, A.Fake<IExtension>(), this.consumer, this.conversionCallbacksProvider, this.defaultConversionCallbackProvider);

            A.CallTo(() => this.extensionPropertyReflector.Reflect(A<IExtension>._)).MustHaveHappened();
        }

        [Fact]
        public void Assign_ShouldAcquireConversionCallbacks()
        {
            this.SetupEmptyConsumerConfiguration();

            this.testee.Assign(this.extensionPropertyReflector, A.Fake<IExtension>(), this.consumer, this.conversionCallbacksProvider, this.defaultConversionCallbackProvider);

            A.CallTo(() => this.conversionCallbacksProvider.ConversionCallbacks).MustHaveHappened();
        }

        [Fact]
        public void Assign_ShouldAcquireDefaultConversionCallback()
        {
            this.SetupEmptyConsumerConfiguration();

            this.testee.Assign(this.extensionPropertyReflector, A.Fake<IExtension>(), this.consumer, this.conversionCallbacksProvider, this.defaultConversionCallbackProvider);

            A.CallTo(() => this.defaultConversionCallbackProvider.DefaultConversionCallback).MustHaveHappened();
        }

        [Fact]
        public void Assign_WhenReflectedPropertyMatchesConfiguration_ShouldAcquireCallback()
        {
            this.SetupConversionCallbackReturnsInput();

            var dictionary = new Dictionary<string, IConversionCallback> { { SomeExtensionPropertyName, this.conversionCallback } };
            A.CallTo(() => this.conversionCallbacksProvider.ConversionCallbacks).Returns(dictionary);

            PropertyInfo propertyInfo = GetSomePropertyPropertyInfo();
            A.CallTo(() => this.extensionPropertyReflector.Reflect(A<IExtension>._)).Returns(new List<PropertyInfo> { propertyInfo });
            A.CallTo(() => this.consumer.Configuration).Returns(new Dictionary<string, string> { { SomeExtensionPropertyName, SomeExtensionPropertyValue } });

            var someExtension = new SomeExtension();
            this.testee.Assign(this.extensionPropertyReflector, someExtension, this.consumer, this.conversionCallbacksProvider, this.defaultConversionCallbackProvider);

            A.CallTo(() => this.conversionCallback.Convert(SomeExtensionPropertyValue, propertyInfo)).MustHaveHappened();
            someExtension.SomeProperty.Should().Be(SomeExtensionPropertyValue);
        }

        [Fact]
        public void Assign_WhenNoConversionCallbackFound_ShouldUseDefaultCallback()
        {
            this.SetupConversionCallbackReturnsInput();

            A.CallTo(() => this.conversionCallbacksProvider.ConversionCallbacks).Returns(new Dictionary<string, IConversionCallback>());
            A.CallTo(() => this.defaultConversionCallbackProvider.DefaultConversionCallback).Returns(this.conversionCallback);

            PropertyInfo propertyInfo = GetSomePropertyPropertyInfo();
            A.CallTo(() => this.extensionPropertyReflector.Reflect(A<IExtension>._)).Returns(new List<PropertyInfo> { propertyInfo });
            A.CallTo(() => this.consumer.Configuration).Returns(new Dictionary<string, string> { { SomeExtensionPropertyName, SomeExtensionPropertyValue } });

            var someExtension = new SomeExtension();
            this.testee.Assign(this.extensionPropertyReflector, someExtension, this.consumer, this.conversionCallbacksProvider, this.defaultConversionCallbackProvider);

            A.CallTo(() => this.conversionCallback.Convert(SomeExtensionPropertyValue, propertyInfo)).MustHaveHappened();
            someExtension.SomeProperty.Should().Be(SomeExtensionPropertyValue);
        }

        private static PropertyInfo GetSomePropertyPropertyInfo()
        {
            return typeof(SomeExtension).GetProperty(SomeExtensionPropertyName);
        }

        private void SetupEmptyConsumerConfiguration()
        {
            A.CallTo(() => this.consumer.Configuration).Returns(new Dictionary<string, string>());
        }

        private void SetupConversionCallbackReturnsInput()
        {
            string interceptedValue = null;
            A.CallTo(() => this.conversionCallback.Convert(A<string>._, A<PropertyInfo>._))
                .Invokes((string value, PropertyInfo info) => interceptedValue = value)
                .ReturnsLazily(() => interceptedValue);
        }

        private class SomeExtension : IExtension
        {
            public string SomeProperty { get; private set; }

            /// <inheritdoc />
            public string Name
            {
                get
                {
                    return this.GetType().FullNameToString();
                }
            }

            public string Describe()
            {
                return string.Empty;
            }
        }
    }
}