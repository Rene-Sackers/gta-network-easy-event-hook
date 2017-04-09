# Simpler way of hooking client events

Instead of having to manually parse arguments from a client event trigger, like so:

	public MyScript()
	{
		API.onClientEventTrigger += OnClientEventTrigger;
	}

	private void OnClientEventTrigger(Client sender, string eventName, params object[] arguments)
	{
		if (eventName != "MyEvent") return;

		var firstArgument = arguments[0] as NetHandle?;
		var secondArgument = arguments[1] as string;

		if (!firstArgument.HasValue) return;
		if (string.IsNullOrWhiteSpace(secondArgument)) return;

		// Do stuff with the net handle and string arguments.
	}

This utility allows it to be completely automatically mapped, turning the code into this:

	public MainEntryPoint()
	{
		var easyEventHook = new EasyEventHook(API);
		easyEventHook.RegisterHandler(this, nameof(MyEvent));
	}

	private void MyEvent(Client sender, NetHandle firstArgument, string secondArgument)
	{
		// Do stuff with the net handle and string argument
	}

The `firstArgument` and `secondArgument` will automatically be casted to `NetHandle` and `string`, if the client triggers the event with the respectful arguments.

In the above example, it'll use the name of the method you provided (`nameof(MyEvent)`) as the event name, but it's also possible to specify a custom event name, by specifying the 3rd argument:

    easyEventHook.RegisterHandler(this, nameof(MyEvent), "my_event");

Relevant file: [EasyEventHook.cs](https://github.com/Rene-Sackers/gta-network-easy-event-hook/blob/master/src/GTANEasyEventHook.GTA/resources/easyeh/Server/EasyEventHook.cs)
    
**Please note:**
In this example, I am using a pre-compiled .dll. However, GTA Network compiles using a lower version of C#, so `nameof(...)` will not work. Simply specify the name of the method it should call as the second argument in `.RegisterHandler(...)` instead.

    easyEventHook.RegisterHandler(this, "MyEvent", "my_event");

