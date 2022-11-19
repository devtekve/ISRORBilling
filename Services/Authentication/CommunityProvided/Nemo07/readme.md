# Made by Nemo07
- [Made by Nemo07](#made-by-nemo07)
    * [VipSimple Authority](#vipsimple-authority)
    * [Installation](#installation)
    * [Registering a new user](#registering-a-new-user)
- [If you need to Switch to another Authority :](#if-you-need-to-switch-to-another-authority--)

<small><i><a href='http://ecotrust-canada.github.io/markdown-toc/'>Table of contents generated with markdown-toc</a></i></small>


## VipSimple Authority

It's a feature that make you use **VIP system** with simple authorizer for people who can't deal with tables in **GB_JoymaxPortal** , you will only use the **[TB_User]** table in **Silkroad_R_Account** Database.

## Installation

**First :**
you have to change the `AuthService` to the type `Nemo` on the **appsettings.json**

`appsettings.json`
```json
{
  ...
  "AuthService": "Nemo", // 👈 Changed here to Nemo
  "DbConfig": {...},
  ...
}
```


**Second :**
Editing SQL Table and procedure
* Adding the new columns to [TB_User] by using [UpdateTbUser.sql](/Database/CommunityProvided/Nemo07/Scripts/UpdateTbUser.sql) in `/Database/CommunityProvided/Nemo07/Scripts` of this repo.

* Editing the Rigid Register Procedure **_Rigid_Register_User**  in [ SILKROAD_R_ACCOUNT ] Database  by using [UpdateRigidRegisterUserSproc.sql](/Database/CommunityProvided/Nemo07/Scripts/UpdateRigidRegisterUserSproc.sql) in `/Database/CommunityProvided/Nemo07/Scripts` of this repo

## Registering a new user
You can use the following example to register a new user using this Auhtorizer method.
```sql
exec _Rigid_Register_User 'YOURusername','YOURpassword(md5)','YOURemail@gmail.com','Y','Y','EU','127.0.0.1', VipExpireTime
```


# If you need to Switch to another Authority :
![enter image description here](https://i.imgur.com/hguwJlt.png)

**Simple :**
It's simple Authority that manage players to login with Username/Password Or Email/Password without using "GB_JoymaxPortal" Database.

**Bypass:**
It's Bypassed Authority that manage players to login with Username/Password without using "GB_JoymaxPortal" Database.

**Nemo:**
It's Vip Simple Authority that manage players to login with VIP Feature without using "GB_JoymaxPortal" Database.

**Full:**
It's Full Authority that manage players to use the Full A.LoginUser Authorities in  "GB_JoymaxPortal" Database.  (Not For Beginners)
