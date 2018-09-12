# Netflix_Analysis
Uses a N-Tier Design to Analyze a CTA Database using SQL and C#


* N-Tier Design
* Data-Access Tier allows for the return scalar values and sets of data, as well as changing data in the database.
* The Data-Access Tier is the only Tier that directly accesses the Database through SQL.
* Business Tier calls the Data-Access Tier and parses data after it is returned from the database.
* The Data is returned in functions with return types in the form of lists or scalar values.
* Presentation Tier deals with C# and calls functions in Business Tier and presents them.
* The GUI is made that way for grading purposes and size limits.

# Design Scheme:
![Design_Scheme_Picture_here](Netflix_Analysis/Readme_Diagram.PNG?raw=true "")
