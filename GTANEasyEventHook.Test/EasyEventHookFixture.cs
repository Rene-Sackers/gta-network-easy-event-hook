﻿using System;
using GTANetworkServer;
using GTANEasyEventHook.GTA.resources.easyeh.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace GTANEasyEventHook.Test
{
	[TestClass]
	public class EasyEventHookFixture
	{
		private const string StringArgument = "example string";
		private const int IntArgument = 1337;
		private const ExampleEnum ExampleEnumValue = ExampleEnum.EnumItem2;

		private event API.ServerEventTrigger ServerEventTriggerEvent;

		public enum ExampleEnum
		{
			EnumItem1 = 0,
			EnumItem2 = 1,
			EnumItem3 = 2
		}

		public class EventHandlingClass
		{
			public virtual void HandlerWithVariousParameters(
				string stringArgument,
				int intArgument,
				ExampleEnum enumArgument)
			{
			}
		}

		private EasyEventHook CreateEasyEventHook()
		{
			return new EasyEventHook(handler => ServerEventTriggerEvent += handler);
		}

		private Mock<EventHandlingClass> RegisterTestHandler(EasyEventHook easyEventHook)
		{
			var eventHandlingClassMock = new Mock<EventHandlingClass>();
			easyEventHook.RegisterHandler(eventHandlingClassMock.Object, nameof(EventHandlingClass.HandlerWithVariousParameters));

			return eventHandlingClassMock;
		}

		private void TriggerDefaultEvent()
		{
			ServerEventTriggerEvent(null, nameof(EventHandlingClass.HandlerWithVariousParameters), StringArgument, IntArgument, ExampleEnumValue);
		}

		private Mock<EventHandlingClass> RegisterAndTriggerDefaultHandler(EasyEventHook easyEventHook)
		{
			var eventHandlingClassMock = RegisterTestHandler(easyEventHook);
			TriggerDefaultEvent();
			return eventHandlingClassMock;
		}

		[TestMethod]
		public void EventHandlerProperlyCalledWithcastedValues()
		{
			var eventHandlingClassMock = RegisterAndTriggerDefaultHandler(CreateEasyEventHook());

			eventHandlingClassMock.Verify(ehc => ehc.HandlerWithVariousParameters(StringArgument, IntArgument, ExampleEnumValue));
		}

		[TestMethod]
		public void CanRegisterHandler()
		{
			var eventHandlingClassMock = RegisterAndTriggerDefaultHandler(CreateEasyEventHook());

			eventHandlingClassMock.Verify(ehc => ehc.HandlerWithVariousParameters(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<ExampleEnum>()), Times.Once);
		}

		[TestMethod]
		public void CanUnregisterHandler()
		{
			var easyEvenHook = CreateEasyEventHook();
			var eventHandlingClassMock = RegisterAndTriggerDefaultHandler(easyEvenHook);

			easyEvenHook.UnregisterHandler(eventHandlingClassMock.Object, nameof(EventHandlingClass.HandlerWithVariousParameters));

			TriggerDefaultEvent();

			eventHandlingClassMock.Verify(ehc => ehc.HandlerWithVariousParameters(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<ExampleEnum>()), Times.Once);
		}

		[TestMethod]
		[ExpectedException(typeof(IndexOutOfRangeException))]
		public void CallingWithTooLittleArgumentsThrowsError()
		{
			var easyEvenHook = CreateEasyEventHook();
			RegisterTestHandler(easyEvenHook);

			ServerEventTriggerEvent(null, nameof(EventHandlingClass.HandlerWithVariousParameters), StringArgument, IntArgument);
		}

		[TestMethod]
		[ExpectedException(typeof(IndexOutOfRangeException))]
		public void CallingWithTooManyArgumentsThrowsError()
		{
			var easyEvenHook = CreateEasyEventHook();
			RegisterTestHandler(easyEvenHook);

			ServerEventTriggerEvent(null, nameof(EventHandlingClass.HandlerWithVariousParameters), StringArgument, IntArgument, ExampleEnumValue, "One Too Many");
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void CallingWithWrongTypeArgumentsThrowsError()
		{
			var easyEvenHook = CreateEasyEventHook();
			RegisterTestHandler(easyEvenHook);

			ServerEventTriggerEvent(null, nameof(EventHandlingClass.HandlerWithVariousParameters), IntArgument, StringArgument, ExampleEnumValue);
		}
	}
}