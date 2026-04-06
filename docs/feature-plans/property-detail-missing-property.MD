Goal:
Update PropertyDetailController, Detail endpoint and the downstream classes and db procedures to return more data to the user for property detail page. Add a plan and ask for approval prior to making all these changes.



Constraints:

* Use existing project structure and patterns
* Do not introduce new frameworks or major dependencies
* Do not modify unrelated modules
* Ask before making database schema changes in dacpac project



Plan:

1. Analyze current project structure:

   * Look at the property Detail end point and the modelPropertyDetailViewModel.
   * Have a look at the following property pages that are part of this site on live at the moment - https://lymmholidaylets.com/property/detail/1, https://lymmholidaylets.com/property/detail/2, https://lymmholidaylets.com/property/detail/3 to get more context.



2. Update procedure:

   * Update the procedure in Property\_Detail\_GetByID to get more details that need to be dynamic from the database in the project lymmholidaylets.db. For example we need to get image details, the order of images, images that are visible, images that are shown on site and the order of these images and alt data if available. Need to get property description, number of bedrooms, bathrooms, car parking spaces, reception rooms, bedroom configuration, i.e single of double bed, number of kitchens, Amenities. If not all these properties are in the database suggest schema updates. We need space for number of people also based on the property. Review the live websites provided and work out what other content should be dynamic and suggest recommendations on dynamic data if I am missing anything. I notice there is years experience for host on this page which I don't think we have covered in the database also. Please include.



3. Implement Infrastructure:

   * Update all the models and code throughout to return these extra details for these pages.



5. Add basic tests:

   * Ensure tests pass and we have test covering changes made if required based on previous changes across the solution.



Output:

* Summary of added/modified files
* Notes on any assumptions or limitations

