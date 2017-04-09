using GTANetworkServer;
using GTANetworkShared;

namespace GTANEasyEventHook.GTA.resources.easyeh.Server
{
	public class MainEntryPoint : Script
	{
		public enum ExampleEnum
		{
			EnumItem1 = 0,
			EnumItem2 = 1,
			EnumItem3 = 2
		}

		public MainEntryPoint()
		{
			var easyEventHook = new EasyEventHook(API);

			easyEventHook.RegisterHandler(this, nameof(ExampleEvent));
		}

		public void ExampleEvent(
			Client sender,
			NetHandle currentVehicleHandle,
			string stringArgument,
			int intArgument,
			ExampleEnum enumArgument)
		{
			API.consoleOutput($@"== {nameof(ExampleEvent)} ==\n
				{nameof(sender)}: {sender.socialClubName}\n
				{nameof(currentVehicleHandle)}: {currentVehicleHandle.Value}\n
				{nameof(stringArgument)}: {stringArgument}\n
				{nameof(intArgument)}: {intArgument}\n
				{nameof(enumArgument)}: {enumArgument}");
		}
	}
}