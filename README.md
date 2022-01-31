# Atamai.Slice

Proof-of-concept solution for slicing minimal-api and using source generator to resolve the slices without runtime reflection or manual registrations.

[Atamai.Slice.Generator](/Atamai.Slice.Generator) generates something like the following:
```c#
public static class GeneratedAtamaiSliceRegistrations 
{ 
    [System.Runtime.CompilerServices.ModuleInitializer]
    public static void Init() 
    {
        Atamai.Slice.Extensions.Add<Atamai.Slice.Sample.Slices.Root>();
        Atamai.Slice.Extensions.Add<Atamai.Slice.Sample.Slices.Session.Create>();
        Atamai.Slice.Extensions.Add<Atamai.Slice.Sample.Slices.Session.Delete>();
    }
}
```

This will register all slices on application startup so it's available for the `builder.AddSlice();` and `app.UseSlice();` 

Take a look at [Atamai.Slice.Sample](/Atamai.Slice.Sample) to see it in action.