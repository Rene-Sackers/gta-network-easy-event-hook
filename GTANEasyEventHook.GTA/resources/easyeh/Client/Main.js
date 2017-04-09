/// <reference path="../../../types-gtanetwork/index.d.ts" />
API.onUpdate.connect(function () {
    if (API.isControlJustPressed(51 /* Context */)) {
        /*
        public void ExampleEvent(
            Client sender,
            NetHandle netHandleArgument,
            string stringArgument,
            int intArgument,
            ExampleEnum enumArgument)
        */
        var currentVehicle = API.getPlayerVehicle(API.getLocalPlayer());
        API.triggerServerEvent("ExampleEvent", currentVehicle, "sample string arg", 1337, 1);
    }
});
//# sourceMappingURL=Main.js.map