using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GTANetworkServer;

namespace GTANEasyEventHook.GTA.resources.easyeh.Server
{
	public class EasyEventHook
	{
		private class EventHandlerInfo
		{
			public object ClassInstance { get; }

			public MethodInfo MethodInfo { get; }

			public EventHandlerInfo(object classInstance, MethodInfo methodInfo)
			{
				ClassInstance = classInstance;
				MethodInfo = methodInfo;
			}
		}

		private const string SenderParameterName = "sender";

		private readonly Dictionary<string, EventHandlerInfo> _eventHandlers = new Dictionary<string, EventHandlerInfo>();

		public EasyEventHook(API api)
			: this(handler => api.onClientEventTrigger += handler)
		{
		}

		public EasyEventHook(Action<API.ServerEventTrigger> eventTrigger)
		{
			eventTrigger(OnClientEventTrigger);
		}

		public void RegisterHandler<T>(T classInstance, string methodName, string eventName = null)
			where T: class
		{
			var classType = classInstance.GetType();
			var methodInfo = classType.GetMethod(methodName);

			if (methodInfo == null)
				throw new InvalidOperationException($"Could not get method info for method {methodName} on  type {classType.FullName}.");

			eventName = eventName ?? methodInfo.Name;

			if (_eventHandlers.ContainsKey(eventName))
				throw new InvalidOperationException($"Tried to re-register event {eventName}. Already registered.");

			_eventHandlers.Add(eventName, new EventHandlerInfo(classInstance, methodInfo));
		}

		public void UnregisterHandler<T>(T classInstance, string methodName)
			where T: class
		{
			var matchingHandlerKey = _eventHandlers
				.Where(hm => hm.Value.ClassInstance == classInstance && hm.Value.MethodInfo.Name == methodName)
				.Select(hm => hm.Key)
				.FirstOrDefault();

			if (matchingHandlerKey == null)
				throw new InvalidOperationException($"No registered event handler found for method {methodName} on the specified class instance.");

			_eventHandlers.Remove(matchingHandlerKey);
		}

		private void OnClientEventTrigger(Client sender, string eventName, params object[] arguments)
		{
			if (!_eventHandlers.ContainsKey(eventName)) return;

			var matchingHandler = _eventHandlers[eventName];
			var methodParameters = matchingHandler.MethodInfo.GetParameters();
			var firstParameterIsSender = IsFirstParameterSender(methodParameters);

			var methodCallParameters = new object[methodParameters.Length];

			if (firstParameterIsSender)
				methodCallParameters[0] = sender;

			if (arguments.Length < methodParameters.Length - (firstParameterIsSender ? 1 : 0))
				throw new IndexOutOfRangeException("Amount of hooked method parameters exceeds amount of event arguments.");

			for (var i = 0; i < arguments.Length; i++)
			{
				var parameterIndex = i + (firstParameterIsSender ? 1 : 0);
				var methodParameter = methodParameters[parameterIndex];
				var eventParameter = arguments[i];

				try
				{
					methodCallParameters[parameterIndex] = Convert.ChangeType(eventParameter, methodParameter.ParameterType);
				}
				catch (Exception ex)
				{
					throw new ArgumentException($"Failed to cast value \"{eventParameter}\" to type {methodParameter.ParameterType}. See inner exception for more detail.", ex);
				}
			}

			matchingHandler.MethodInfo.Invoke(matchingHandler.ClassInstance, methodCallParameters);
		}

		private static bool IsFirstParameterSender(IReadOnlyCollection<ParameterInfo> methodParameters)
		{
			if (methodParameters.Count <= 0)
				return false;

			var firstParameter = methodParameters.First();
			if (firstParameter.ParameterType != typeof(Client))
				return false;

			return firstParameter.ParameterType.Name.ToLowerInvariant() != SenderParameterName;
		}
	}
}