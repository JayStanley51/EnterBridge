EnterBridge POC
===============
README
Authored by James Stanley

Installation is pulling the code and running it provided you have .Net 10 installed
For the purpose of this demo the database for storing orders is an in-memory database that read/writes to a csv since that is the easiest way for the reviewer to get up and running.
(In a production scenario this would be a SQL database with a ORM like dapper to handle everything.)

For the architecture I went with ASP.NET Core MVC as it utilizes the .Net framework and has bootstrap baked it which makes it optimal for rendering on different size screens like an iPad or phone.
The data models were made by Claude through the swagger documentation/ or created ad hoc as I progressed through the project 
On the home page we have a dropdown menu with Users to select and login as. I did it this way to be able to simulate some role based access features within the time constraints.

After fulfilling the must have requirements, the next priority was the order review/edit and approval system for the foreman user role. This is the most ‘bang for your buck’ feature,
they can approve, edit, reject all orders from the order history screen and there is a numbered indicator on their landing page to show them that there is orders waiting for approval as well.

The second priority is making the frequently ordered products “easier” for this I added a reorder option from the purchasers’ order history with the ability to add or remove items as needed.
I didn't have time to build out historical ordering data to build a "Common Purchase" section.

The procurement manager’s price trends were the last priority. I would need more information about how they would like trends to be calculated and displayed (Historical Data back X amount of months, Graphs etc.)

