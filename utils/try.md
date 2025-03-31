```c#
// Approach 1: Just see what we're working with first
Console.WriteLine("Raw result: " + result.Substring(0, Math.Min(100, result.Length)) + "...");

// Try to parse it as a simple dynamic object
try {
    dynamic jsonObj = JsonSerializer.Deserialize<dynamic>(result);
    Console.WriteLine("Success as dynamic");
}
catch (Exception ex) {
    Console.WriteLine("Failed dynamic: " + ex.Message);
}

// Try as JObject (if you have Newtonsoft)
try {
    var jObj = Newtonsoft.Json.Linq.JObject.Parse(result);
    Console.WriteLine("Success as JObject");
    // You can access it like: jObj["propertyName"]
}
catch (Exception ex) {
    Console.WriteLine("Failed JObject: " + ex.Message);
}

// Try as JArray (if you have Newtonsoft)
try {
    var jArr = Newtonsoft.Json.Linq.JArray.Parse(result);
    Console.WriteLine("Success as JArray");
    // You can access first item like: jArr[0]
}
catch (Exception ex) {
    Console.WriteLine("Failed JArray: " + ex.Message);
}
```