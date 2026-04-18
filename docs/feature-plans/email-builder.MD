Goal:
Create an email template that is dynamically built based on the booking for a customer.



Constraints:
Use existing project structure and patterns
Do not introduce new frameworks or major dependencies
Do not modify unrelated modules
Ask before making database schema changes in dacpac project



Plan:
Analyze current project structure.
I already have an email template BookingConfirmationCustomer. 

Look at the file CustomerBookingConfirmation. I would like you take this file, work out all the dynamic parts that are associated with a booking, property and host.

Look at the current dacpac project and database schema and the current email that is built BookingConfirmationCustomer.

Firstly suggest and show me in the plan all the database changes you suggest that would fulfil filling in the contents of CustomerBookingConfirmation and BookingConfirmationCustomer. 

Once we have proposed a plan of all the missing fields and database schema changes lets copy the content of CustomerBookingConfirmation and replace BookingConfirmationCustomer with all the placeholders.

Lets strip out any references to Airbnb.

Ensure the html is w3 valid and will work across email clients. Suggest improvements to the html.

Suggest options for improving both the data in the email template and the html itself.


Add basic tests:
Validate login success/failure cases
Test invalid input handling
Review and refine:
Improve readability and structure
Output:
Summary of added/modified files
Notes on any assumptions or limitations

