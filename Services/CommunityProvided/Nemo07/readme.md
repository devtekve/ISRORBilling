# Made by Nemo07

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
