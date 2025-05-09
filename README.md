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
      <a href="#project-overview">Project Overview</a>
      <ul>
        <li><a href="#functionality">Functionality</a></li>
        <li><a href="#architecture">Architecture</a></li>
        <li><a href="#web-pages">Web Pages</a></li>
        <li><a href="#data">Data</a></li>
          <ul>
            <li><a href="#sql-database">SQL Database</a></li>
            <li><a href="#c#-data-classes">C# Data Classes</a></li>
        </ul>
      </ul>
      <li>
        <a href="#project-planning">Project Planning</a>
      </li>  
      <li>
        <a href="#project-planning">API Documentation</a>
      </li> 
    </li>
  </ol>

<!-- Portal Home Page -->
[![F1 Driver Portal Home Page](images/HomePage.png)](https://f1driversportal-bpbzhjebbfahe5fj.canadacentral-01.azurewebsites.net/)


<!-- ABOUT THE PROJECT -->

## About The Project
This is a .NET Web Application that supports `Create`, `Read`, `Update` and `Delete` (CRUD) operations for Formula One Driver members data.
The application is built using C#, HTML, CSS, SQL Server, following Model-View-Controller (MVC) architecture.
The web app is auto-deployed using Github actions with Microsoft Azure. More details can be found in the <a href="#project-overview">Project Overview</a> section.
<p align="right">(<a href="#readme-top">back to top</a>)</p>


<!-- PROJECT OVERVIEW -->
## Project Overview

  ### Tech Stack
    - Web App - HTML, CSS, .NET
    - Back End - SQL Server, Microsoft Azure
    - Source Code and CI/CD - Github Actions

## Functionality
The user can access the following functionalities on the portal: 
1. Drivers - Add Driver, View All Drivers, Delete Driver, Modify Driver Data, Find Driver using their Driver Number (doesn't have to be unique)
    - Drivers data is stored in Azure SQL Database. All changes are saved directly to the database table.  
2. Teams - View Team data, Find Team using Team Rank
    - Team data is also saved in the Azure SQL database. There is no option to edit it from the portal.
3. Races - View All Race Sessions so far, View Race Sessions in a time window
    - This data is retrieved live from a public API [https://api.openf1.org/v1/sessions].
<p align="right">(<a href="#readme-top">back to top</a>)</p>

## Architecture
<image here>

 ### Web Pages
  The portal is divided in 4 pages:
 1. **Home**
    - This is the welcome screen. You'll find a `From` and `To` date prompts to filter latest race session data.
    - `Filter` button click redirects you to Previous Races page.
    - If no dates are selected, it will automatically retrieve the entire json (sessions starting from 2023 till today).

<p align="right">(<a href="#readme-top">back to top</a>)</p>

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
    - This page displays all teams by default.
    - You can filter teams by `Team Rank`.
    - There are only 10 teams in F1, hence 1 to 10 ranks. 0 shows all teams and any other number less than 0 and more than displays an empty table.
    - Clicking on the `driver image` in this table takes you to that driver's information page.

 4. **Previous Races**
    - This page displays all the race sessions using the https://api.openf1.org/v1/sessions API.
    - The earliest available race is from 2023 and the latest is updated every weekend after the race.
    - You can filter the race from a `start date` to `end date`. Shows an empty table is there are no races in the date range.
    - It shows an error if you select `start date` later than `end date`.
    - A future `end date` is also accepted.

  UI Restrictions
      - The `DriverNumber`, `Find Team` fields only allow numbers.
      - Edit and Add Driver form fields also allows only numbers in `Phone` and `Driver Number` fields.
      - If you resize the browser window to really slim (to match mobile device), the Drivers page and Teams page tables shrink to only display the important columns.

<p align="right">(<a href="#readme-top">back to top</a>)</p>


 ### Data
  ##### SQL Database
  The SQL Database consists of two tables:
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

  > All the driver headshot images are public URLs, stored as `ImageUrl` in the `Drivers` database.
    The team car images are stored on my Azure Blob Storage and their SAS token URIs are stored in the `Teams` database.

  #### C# Data Classes
  The .Net project has five data classes:
      1. Driver - used to map driver data from the database. Same fields as the database.
      2. JsonDriverData - this was used to deserialize data from the drivers API, which was then used to populate the SQL `Drivers` table.
      3. Race - used for race session data. The API json data is deserialized into Race object.
      4. Team - this is same as the Team SQL table fields.
      5. TeamView - the `Team` object data is converted to `TeamView` object for displaying on the Teams page.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

### Project Planning
I planned the project exactly how I plan my sprint tasks. The problem statement broken down into smaller tasks and tracked in this Google Document.
The tasks were sorted in decreasing priority order and were updated if an unforeseen issue or feature came up.
I tracked my task progress using a checklist (similat to a Sprint board). 

Some tasks were stretch goals - marked as *Bonus*.
> Best practice - I add comment summary to my Github commits that contain a big change set. This summary are helpful when create release notes and detailed Pull Request descriptions.


Future Improvements
  - Live race data (team positions and driver lap times) on home page.
  - Find Driver and Teams by name.
  - Edit Team and Add Team features.
  - More test cases to cover model controller.
  - Fix the Github CI/CD test actions.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

## API Documentation 












<p align="right">(<a href="#readme-top">back to top</a>)</p>
