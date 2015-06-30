# BarUTomaREST
Server application providing REST API for managing bars. Implemented using Entity Framework in .NET 4.5.

RESTFul API:

Note: *Emphasized* is only available for admin

Note: **Bold** is public API, without requiring any authentication

URL             | GET                           | POST                          | DELETE
----------------|-------------------------------|-------------------------------|--------------------
**/bar/**       | **Get list of all bars**      | *Save bar*                    |
/bar/{id}       | **Retrieve info about bar**   |                               | *Delete bar*
*/bar/getMyBars*|*List all bars, that are owned by logged user (for superadmin, return all bars)* | |
/bar/{id}/drink | **List all drinks in bar**    | *Add new custom drink to bar*   |
/bar/{id}/drink/{id2} | **Gets info about drink on bar** | *Adds new system drink to bar* | *Delete drink from bar*
*/bar/{id}/users* | *Gets list of user, that have performed at least one order* | |
/bar/{id}/order/*{userName}* [FromBody] List<string> where the list looks like this: ["(drinkId(int), quantity(int))"] | | Order given amount of given drink from bar *(for given user)* | | 
/bar/{id}/order/{id} | Get info about order | | *Cancel order*
/bar/{id}/order | List customer's orders in given bar | |
**/bar/{barId}/order/user [FromBody] Username** | **List all orders for specific customer for admin.**
/user/          | Retrieve info about self       | |
/bar/{id}/notification/ | **List all ongoing notifications** | *Create new / edit notification* | 
**/bar/{id}/notification/before/{id}** | **List defined amount of notifications before given notification** | |
**/bar/{id}/notification/{id2}** | **Get detail about notification** | |
**/ingredient/{id}** | **Get info about ingredient** | |
/bar/{id}/bottle/ingredient/{id2} | | Add new bottle of given ingredient to specified bar |
/bar/{id}/bottle/ | Get info about bought bottles | |
**/Account/Register** | | **Register new user.** |
/Account/Token    | Get token for authorized actions. | |

Register new user by sending application/json HTTP Post request in this format:

  {
    "Email" : "asdf@asdf.com",
    "Username" : "asdf",
    "Password" : "Asdf123!",
    "ConfirmPassword" : "Asdf123!",
    "DefaultPriceUnitId" : 1,
    "Imperial" : false 
  }

In order to get an authorization token, send application/x-www-form-urlencoded with plaintext in format: grant_type=password&username=username&password=password
