# OnFire
 Quick,Dirty library for Firebase Email Auth and Database Usage.
```
OnFire 
```
| List             |      type    |   decriptions |
| :--------:           |     :-:      | :-:         |  
| Database    | class  | add ,insert, delete data in realtime on firebase data  |
| EMailAuth    | class  | signup,login,verify,reset password for users     |
| Cache             |      class    |  used to cache firebase data on device ram     |
| RestClient        |   class | A Rest Http Client with get ,put ,delete etc                         |




### Example Usage

##### Read From Realtime DataBase

```cs
Database dbref = new Database("xyz.firebaseio.com");
var userName=dbref.Child("Scores").Child("highestScore").GetValue<string>();
Console.WriteLine($"user Name : {userName}");
```

##### Save in Database

```cs
int Score = 9099;
Database dbref = new Database("xyz.firebaseio.com");
dbref.Child("Scores").Child("highestScore").SaveValue(Score);
```

##### Delete from Database

```cs
Database dbref = new Database("xyz.firebaseio.com");
dbref.Child("Scores").Child("highestScore").DeleteValue();
```
