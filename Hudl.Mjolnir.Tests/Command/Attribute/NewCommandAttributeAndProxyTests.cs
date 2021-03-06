﻿using System.Threading;
using Hudl.Mjolnir.Command;
using Hudl.Mjolnir.Command.Attribute;
using Hudl.Mjolnir.Tests.Helper;
using Xunit;
using Hudl.Config;

namespace Hudl.Mjolnir.Tests.Command.Attribute
{
    public sealed class NewCommandAttributeAndProxyTests : TestFixture
    {
        [Attributes.Command("foo", "bar", CancellableWithTimeoutPreserved.Timeout)]
        public interface ICancellableTimeoutPreserved
        {
            string CancellableMethod(CancellationToken token);
        }

        public class CancellableWithTimeoutPreserved : ICancellableTimeoutPreserved
        {
            public const int Timeout = 500;
            public CancellationToken TokenReceivedFromProxy { get; private set; }
            public bool CallMade { get; private set; }
            private readonly string _returnResult;
            public CancellableWithTimeoutPreserved(string returnResult)
            {
                _returnResult = returnResult;
            }

            public string CancellableMethod(CancellationToken token)
            {
                CallMade = true;
                TokenReceivedFromProxy = token;
                return _returnResult;
            }
        }

        public class CancellableWithOverrunningMethod : ICancellableTimeoutPreserved
        {
            public string CancellableMethod(CancellationToken token)
            {
                Thread.Sleep(CancellableWithTimeoutPreserved.Timeout + 500);
                token.ThrowIfCancellationRequested();
                return string.Empty;
            }
        }

        // Want to make sure that a CancellationToken with timeout is passed by the proxy when we haven't flagged the command as having timeouts ignored.
        [Fact]
        public void ProxyPassesATimeoutTokenToMethod_WhenTimeoutsNotIgnored()
        {
            var expectedResult = "test";
            var classToProxy = new CancellableWithTimeoutPreserved(expectedResult);
            var proxy = CommandInterceptor.CreateProxy<ICancellableTimeoutPreserved>(classToProxy);
            // If we pass CancellationToken.None to the proxy then it should pass a timeout tokem to the method call.
            var result = proxy.CancellableMethod(CancellationToken.None);
            Assert.True(classToProxy.CallMade && classToProxy.TokenReceivedFromProxy != CancellationToken.None);
            // This shouldn't be cancelled yet.
            Assert.False(classToProxy.TokenReceivedFromProxy.IsCancellationRequested);
            // Now sleep past the timeout.
            Thread.Sleep(CancellableWithTimeoutPreserved.Timeout + 50); 
            Assert.True(classToProxy.TokenReceivedFromProxy.IsCancellationRequested);
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void ProxyPassesOnTokenToMethod_WhenTimeoutsNotIgnored()
        {
            ConfigProvider.Instance.Set(IgnoreTimeoutsKey, false);

            var expectedResult = "test";
            var classToProxy = new CancellableWithTimeoutPreserved(expectedResult);
            var proxy = CommandInterceptor.CreateProxy<ICancellableTimeoutPreserved>(classToProxy);
            // If we pass a valid token to the proxy then it should pass the token to the method call.
            var token = new CancellationTokenSource(500).Token;
            var result = proxy.CancellableMethod(token);
            Assert.True(classToProxy.CallMade);
            Assert.Equal(classToProxy.TokenReceivedFromProxy, token);
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void MethodShouldTimeout_WhenTimeoutsAreNotIgnored()
        {
            ConfigProvider.Instance.Set(IgnoreTimeoutsKey, false);

            var classToProxy = new CancellableWithOverrunningMethod();
            var proxy = CommandInterceptor.CreateProxy<ICancellableTimeoutPreserved>(classToProxy);
            Assert.Throws<CommandTimeoutException>(() => proxy.CancellableMethod(CancellationToken.None));
        }
    }
}
