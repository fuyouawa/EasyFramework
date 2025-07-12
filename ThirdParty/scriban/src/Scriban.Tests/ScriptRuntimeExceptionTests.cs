using System;
using System.Threading.Tasks;
using EasyToolKit.ThirdParty.Scriban.Parsing;
using EasyToolKit.ThirdParty.Scriban.Syntax;
using NUnit.Framework;
using EasyToolKit.ThirdParty.Scriban.Runtime;
using NSubstitute;

namespace EasyToolKit.ThirdParty.Scriban.Tests
{
    public class ScriptRuntimeExceptionTests
    {
        [Test]
        public void TestInnerExceptionExists()
        {
            ScriptRuntimeException.EnableDisplayInnerException = true;
            SourceSpan testSourcespanObject = new SourceSpan("fileName", new TextPosition(0, 0, 0), new TextPosition(0, 0, 0));
            Exception exception = Substitute.For<Exception>();
            exception.StackTrace.Returns("TestStacTrace");
            exception.Message.Returns("Test RunTime message");

            ScriptRuntimeException testScriptruntimeObject = new ScriptRuntimeException(testSourcespanObject, "Any string", exception);
            
            Assert.True(testScriptruntimeObject.ToString().Contains("TestStacTrace"));
            Assert.True(testScriptruntimeObject.ToString().Contains("Test RunTime message"));
        }

        [Test]
        public void TestInnerExceptiondosentExists()
        {
            ScriptRuntimeException.EnableDisplayInnerException = true;
            SourceSpan testSourcespanObject = new SourceSpan("fileName", new TextPosition(0, 0, 0), new TextPosition(0, 0, 0));

            ScriptRuntimeException testScriptruntimeObject = new ScriptRuntimeException(testSourcespanObject, "Any string");

            Assert.AreEqual(testScriptruntimeObject.ToString(), testScriptruntimeObject.Message);
        }

        [Test]
        public void TestInnerExceptionDisabled()
        {
            ScriptRuntimeException.EnableDisplayInnerException = false;
            SourceSpan testSourcespanObject = new SourceSpan("fileName", new TextPosition(0, 0, 0), new TextPosition(0, 0, 0));
            Exception exception = Substitute.For<Exception>();
            exception.StackTrace.Returns("TestStacTrace");
            exception.Message.Returns("Test RunTime message");

            ScriptRuntimeException testScriptruntimeObject = new ScriptRuntimeException(testSourcespanObject, "Any string", exception);

            Assert.AreEqual(testScriptruntimeObject.ToString(), testScriptruntimeObject.Message);           
        }
    }
}
