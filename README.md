MinusEngine
===========

A fully asynchronous C# library to access Minus API (http://min.us/pages/api).

Code sample
-----------
    // create the API
    MinusApi api = new MinusApi("someDummyKey"); // api keys aren't working yet

    // setup the success handler for GetItems operation
    api.GetItemsComplete += delegate(MinusApi sender, GetItemsResult result)
    {
        Console.WriteLine("Gallery items successfully retrieved!\n---");
        Console.WriteLine("Read-only URL: " + result.ReadonlyUrl);
        Console.WriteLine("Title: " + result.Title);
        Console.WriteLine("Items:");
        foreach (String item in result.Items)
        {
            Console.WriteLine(" - " + item);
        }
    };

    // setup the failure handler for the GetItems operation
    api.GetItemsFailed += delegate(MinusApi sender, Exception e)
    {
        // don't do anything else...
        Console.WriteLine("Failed to get items from gallery...\n" + e.Message);
    };

    // trigger the GetItems operation - notice the extra "m" in there.
    // while the REAL reader id is "vgkRZC", the API requires you to put the extra "m" in there
    api.GetItems("mvgkRZC");

If you have any questions, please drop me a line on twitter @brunodecarvalho or email me at bruno@biasedbit.com