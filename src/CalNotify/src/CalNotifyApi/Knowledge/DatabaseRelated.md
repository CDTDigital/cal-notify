# Build up our database from our schema

make sure we are in the directory `\src\CalNotify` 

```
dotnet ef database update
```


# Update Schema

Make sure your build is up to date and then make the command line call:

```
dotnet ef migrations add {name}
```

Where `{name}` is the new migration name

Then enter in the command: 

```
dotnet ef database update
```

