Goal:
Create an email scheduler application.



Constraints:
Use existing project structure and patterns
Do not introduce new frameworks or major dependencies
Do not modify unrelated modules
Ask before making database schema changes in dacpac project



Plan:
Analyze current project structure.
I already have an example in Reservation at Lymm Village Apartment for 25 October 2024 - 29 October 2024.eml.

Look at the current dacpac project and database schema.

I would like to create an email that is sent to the guest say five days prior to arrival. I think this should be configurable at the property level, i.e. property 1 send an email 5 says before arrival.

The email content can be stored in the database. It would say Hello {guest name} and all the content that's in the database. ie alarm, bins, general, local area, wifi, heating water, parking. Some dynamic data will be required such as check in date and time. I cant think of anything else that needs to be dynamic. Unless you can suggest anything.

At the top include the lymm holiday lets logo that's on BookingConfirmationCustomer.html.

Keep the styling consistent with BookingConfirmationCustomer.html.

Include options to contact us and details of the checkin and checkout out time. Include the address.

Ensure the html is w3 valid and will work across email clients.

I think the best structure would be to have an exe that sends a message to the notification worker.

I.e. 5 Days before guest arrives, send a message to notification worker that builds up the email and sends to a guest.



Add basic tests:
Validate login success/failure cases
Test invalid input handling
Review and refine:
Improve readability and structure
Output:
Summary of added/modified files
Notes on any assumptions or limitations

