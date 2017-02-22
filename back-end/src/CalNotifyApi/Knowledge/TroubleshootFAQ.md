# FAQ's



### Why are the external services not working? Or why are all the api keys "NOTSETHERE"

We do not save our secret keys in git, so to setup the project make sure you add in your specific api keys.

On startup we pull in a `appsettings.Local.json`, last which would allow you to set settings for your local environment.



### Why are my model properties still capitalized when converted to JSON?

Make sure your model has the `[DataContract]` attribute on it and has a `ToJson()` property defined

```c#
[DataContract]
    public class ResponseResult
    {

        [DataMember(Name = "meta")]
        public ErrorModel Meta { get; set; }

        [DataMember(Name = "result")]
        public object Result { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
```



### What is the Common Error Response Format?

```json
{
  "result": null,
  "meta": {
    "code": 400,
    "message": "Failure on insert",
    "details": [
      "User name 'customer@test.com' is already taken."
    ]
  }
}
```

The meta code will match the header response value, with the message property being a single line of text giving the developer some insight into the error. 

If possible or needed on complex endpoints taking a lot of parameters or having a few possible error scenarios, the details property will be a more explicit array of errors described in a more natural sounding form. 



## Off limits status code for ObjectResult

The status code "401" if used in action filters, request filters, etc will cause the request threads to block.  So don't use the status code of 401. 

However one place where it can be used is in the `OnChallenge` handler for `UseJwtBearerAuthentication`. Outside of this, its off limits.