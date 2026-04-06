Goal:
Create an endpoint that returns property detail for the property detail page.

Constraints:
- Use existing project structure and patterns
- Do not introduce new frameworks or major dependencies
- Do not modify unrelated modules
- Ask before making database schema changes in dacpac project

Plan:
1. Analyze current project structure:
   - Check if property detail endpoint already exists.
   - Look at the class PropertyDetailViewModel these are the properties we want to populate. Also look at 
     PropertyDetailResult. We want to capture all the properties on PropertyDetailViewModel. We want to bring in properties for 
    Guest capacity (drives the booking form) properties, DatesBooked. Also look at D:\solutions\lymmholidaylets\src\LymmHolidayLets.Web.UI\Controllers\PropertyController.cs and the model 
    PropertyDetailViewModel to gain further context. 

2. Create a procedure:
   - Create a procedure in lymmholidaylets.db that will populate these properties on this model from the database.

3. Implement Infrastructure:
   - Create a function infrastructure project to map these properties to an object. Be mindful of which assembly this object sits in that is mapped from the database.
   - Ensure the object is populated and wired through to the front end api. Ensure there are no duplications and the objects are set at the correct level, we have infastructure, application, ui, domain models.

4. Integrate with existing code:
   - Update the controller to use this object to pass back to the front end api.

5. Add basic tests:
   - Validate login success/failure cases
   - Test invalid input handling

6. Review and refine:
   - Improve readability and structure

Output:
- Summary of added/modified files
- Notes on any assumptions or limitations