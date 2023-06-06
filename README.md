# Chat project

Group :

- Lazare K ASSIE
- Maximilien DENIS
- Alexandre Ernotte

Message that the client can send :

```bash
# connection
/connect
/disconnect
/ping
# authentication
/signup "username" "password"
/signin "username" "password"
/signout
# users
/newuser "username" "password"
/userlist
/invite "username"
/setpassword "username" "oldpassword" "newpassword"
# rooms
/messagetoroom "room" "message"
/createroom "name"
/join "room"
/roomlist
```

Messages that the server can reply :

````bash
/SUCCESS <information>
/FAILED <reason>
/NOTICE <from> <room> <message>
````

