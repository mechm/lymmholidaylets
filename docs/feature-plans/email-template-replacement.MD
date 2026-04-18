Goal:
Create an email template that is dynamically built based on the booking. One to the business and one to the customer making a booking.

Constraints:
Use existing project structure and patterns
Do not introduce new frameworks or major dependencies
Do not modify unrelated modules
Ask before making database schema changes in dacpac project

Plan:
Analyze current project structure.
Check the email template html file created. This is can be used as the template for the email sent to the customer.
Look at the dacpac project, the notification worker and the existing email template.
Suggest any database changes required to fulfil the email template. Alternatively suggest changes to the email template to keep it simple.
I would like two emails sending - one the user who made the booking and one to the company taking the booking with all the necessary detail.
The email to the company can be simple but must include as much detailing as possible from the booking.
I have an email template in notification worker that I need to replace with dynamic data from the database on booking.
Please remove any references from Airbnb in the template that you generate in the notification worker and tidy up any files in the notification worker.
I would expect two templates at the end of the plan. One for the business and one for the customer detailing the booking. All dynamic data rendered and replaced.

Add basic tests:
Validate login success/failure cases
Test invalid input handling
Review and refine:
Improve readability and structure
Output:
Summary of added/modified files
Notes on any assumptions or limitations