# ShipStation
How to communicate and POST to shipstation's JSON RESTful endpoints in C# WCF.  There is no examples or even the correct order of how to call their API. 


At first, you must setup your settings for shipping and generate your api key at shipstation.com.  This was done for me, so I cannot go into detail on what was done.  But its pretty simple.  What is not simple is how your are suppose to impliment it (no matter what code your are going to use).  

they have a good api tool(3rd party) at http://www.shipstation.com/developer-api/  which instead of using Postman or any browser tool to test, they have thier own which you can choose to post to test servers or production.

The only thing confusing about this interface, is clicking on the left menu bar pulls up the documentation of the api call in the middle of the page, but only clicking on the /shipments/createlabel  in the middle of the page, will render the test api client in the right frame to render or load for the call you are trying to test.

Another mistake I made was I used /shipments/createlabel  to get a shipping label with all the data I specified, but your order number does not get stored so that was a waste of time.

the correct order (minimalistally) is to call...

1) orders/createorder
2) orders/createlabelfororder

If you get 404, 401 or 500, you actually have to read the response body as the api does give you specific error messages about your api like  invalid shipTo.   What was invalid I had to use the wonderful email support to find out i.e. you cannot put an address in the company field 



