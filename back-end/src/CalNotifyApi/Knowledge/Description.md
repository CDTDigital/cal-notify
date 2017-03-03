The endpoints listed below constitutes the entirety of The **CalNotify API** 

To understand the API, a number of concepts are need to be understood: Our **authentication** model, **parameter conventions**, and api  **response wrapper**. 

Each concept is described in its own section followed by a miscellaneous section describing the other minor details of the API. 

Please read and watch the example  [GIF](login.gif) on how to login in order to explore most of the swagger endpoints


## Authentication
To explore and utilize the api requires authentication for most endpoints. The details of how tokens can be obtained is described below.

**To explore the swagger endpoint follow the steps performed in this [GIF](login.gif)**


Our authentication follows the **standard bearer token** approach, where our tokens are constructed as a  **JSON web tokens** or jwt. The json web token is  compact and digtially signed, and [holds a bunch of information,](https://jwt.io/introduction/) but for the purposes of our api we can treat it as an opaque token. 

Each endpoint which accesses data requires an authentication token. This token can either be sent via an **Authorization Header** in each request:

```http
Authorization: Bearer iG9w0BAQEFAASCBKgwggSkAgEAAoIBAQDGK...
```

or it can be sent via the `auth_token` **query string parameter**

```http
/v1/endpoint?auth_token=iG9w0BAQEFAASCBKgwggSkAgEAAoIBAQDGK...
```


To get hold of this and the other service api tokens, depends on the scenario.  One scenario is **when creating new** users. The other is for when we need to  **authenticate existing** users.

### Creating new Users.

TODO

### Response Wrapper

Every response is contained by an envelope. That is, each response has a predictable set of keys with which you can expect to interact:


```json
    {
        "meta": {
            "code": 200,
              ...
        },
        "result": {
            ...
        }
    }
```

#### META
The meta key is used to communicate extra information about the response to the developer. If all goes well, you'll only ever see a code key with value 200. However, sometimes things go wrong,  in that case you might see a response like:
```json
    {
        "meta": {
            "code": 400,
             "message": "Invalid parameters",
               "details": [
                   "The technician id is invalid"
               ]
        }
    }
```

# Testing 

TODO

## Test Admins 

```
// Name, Email, Phone, Password
("testAdmin1", "testAdmin1@test.com","916-333-0001", 123testadmin")
```