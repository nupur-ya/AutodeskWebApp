<a id="readme-top"></a>

<br />
<div align="center">
  <h1 align="center">Formula One Driver Portal</h1>    
  <h3 align="center">Autodesk Development Task</h3>    
  </p>
</div>



<!-- TABLE OF CONTENTS -->

  <h2>Table of Contents</h2>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
    </li>
    <li>
      <a href="#getting-started">Project Design</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
        <li><a href="#installation">Installation</a></li>
      </ul>
      <li>
        <a href="#project-planning">Project Planning</a>
      </li>  
      <li>
        <a href="#references">References</a>
      </li>
    </li>
  </ol>




<!-- ABOUT THE PROJECT -->
## About The Project
This is a .NET Web Application project created to support Create, Read, Update and Delete (CRUD) operations for Formula One Driver members data.
The application is built using C#, HTML, CSS, SQL Server, following Model-View-Controller (MVC) architecture.
The web app is auto-deployed using Github actions with Microsoft Azure. More details can be found in the <a href="#project-design">Project Design</a> section.
<p align="right">(<a href="#readme-top">back to top</a>)</p>


<!-- PROJECT OVERVIEW -->
## Project Overview


### Functionality
The user can access the following functionalities on the portal: 
1. Drivers - Add Driver, View All Drivers, Delete Driver, Modify Driver Data, Find Driver using their Driver Number (doesn't have to be unique)
    - Drivers data is stored in Azure SQL Database. All changes are saved directly to the database table.  
2. Teams - View Team data, Find Team using Team Rank
    - Team data is also saved in the Azure SQL database. There is no option to edit it from the portal.
3. Races - View All Race Sessions so far, View Race Sessions in a time window
    - This data is retrieved live from a public API [https://api.openf1.org/v1/sessions].

### Architecture
<image here>

 ### Web Pages
  The portal is divided in 4 pages:
 1. **Home**
    - This is the welcome screen. You'll find a `From` and `To` date prompts to filter latest race session data.
    - `Filter` button click redirects you to Previous Races page.
    - If no dates are selected, it will automatically retrieve the entire json (sessions starting from 2023 till today).

 2. **Drivers**
    - This page displays all drivers by default.
    - You can find drivers by entering their Driver Number. If the Driver Number doesn't exist, it displays an empty table.
    - You can also `Add Driver` and `View All` drivers on this page.
    - In every row, you'll find an `Edit` button, which takes you to the `Edit Driver` page.
    - The `Edit Driver` page supports the following functionalities:
      - `Submit` - writes the changes the the database and redirects to Drivers page.
      - `Reset` - Clears the changes you made to the form. Original data remains the same.
      - `Cancel` - Stashes your changes and redirects to Drivers page. 
      - `Delete Driver` - This button is slightly hidden and you need to scroll all the way down to the page to see it. On click, shows you a prompt to confirm deletion and then deletes the driver. Also redirects to Drivers page.
 3. **Teams**
 4. **Previous Races**

  UI Restrictions
      - The `DriverNumber`, `Find Team` fields only allow numbers.
      - Edit and Add Driver form fields also allows only numbers in `Phone` and `Driver Number` fields.
      - 

 ### Data
  #### SQL Database
  The SQL Database consists of two Tables:
  1. Drivers Table
      The data in this table was populated once, using the `HomeController.InitDatabase()` by deserializing the json data from Open F1's public API: https://api.openf1.org/v1/drivers
      Constraints:
        - Each driver has a unique ID. 
        - The DriverNumber can be same for multiple drivers.

      | Column | Description |
      | --- | --- |
      [Id]               |   UNIQUEIDENTIFIER NOT NULL
      [Name]             |   VARCHAR (100)    NOT NULL
      [Email]            |   VARCHAR (200)    NULL
      [Phone]            |   VARCHAR (10)     NULL
      [Team]             |   VARCHAR (100)    NOT NULL
      [HomeCountry]      |   VARCHAR (100)    CONSTRAINT [DEFAULT_Drivers_HomeCountry] DEFAULT ((200)) NULL
      [IsRacingThisYear] |   BIT  CONSTRAINT [DEFAULT_Drivers_IsRacingThisYear] DEFAULT ((0)) NULL
      [ImageUrl]         |   VARCHAR (3000)  NULL
      [DriverNumber]     |   INT NULL

  2. Teams Table
      This table contains information about the 10 F1 teams of 2024 season. The data in this table was populated manually.
      | Column | Description |
      | --- | --- |
      [Id]              |   UNIQUEIDENTIFIER NOT NULL
      [Name]            |   VARCHAR (100)    NOT NULL
      [Driver1]         |   UNIQUEIDENTIFIER NULL
      [Driver2]         |   UNIQUEIDENTIFIER NULL
      [TeamChief]       |   VARCHAR (100)    NOT NULL
      [CarImageUrl]     |   VARCHAR (3000)   NULL
      [Rank]            |   INT              NULL
      [PolePositions]   |   INT              NULL

  #### C# Data Classes
    - 


### Tech Stack
  - Web App - HTML, CSS, .NET
  - Back End - SQL Server, Microsoft Azure
  - Source Code and CI/CD - Github Actions


## API Documentation 












<p align="right">(<a href="#readme-top">back to top</a>)</p>
