using MqttRouter;
Console.Clear();

var route = new Route("device/{id:int}/sensor/{name}", (message, parameters) =>
{
    var deviceId = (int)parameters["id"];
    var sensorName = parameters["name"];
    //parameters.Values.ToList().ForEach(Console.WriteLine);
});

string[] matches = {
    "device/524587/sensor/temperature",
    "device/021167/sensor/light",
    "device/121094/sensor/humidity",
    "device/926941/sensor",
    "device/124490/battery",
    "device/626870/wifi"
};

foreach (var item in matches)
{
    route.Match(item);
}

new Route("home/kitchen/temp").Match("home/kitchen/temperature");