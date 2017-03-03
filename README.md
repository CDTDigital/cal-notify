# Cal-Notify

Cal-Notify provides real time updates on emergencies and other events in your area, using official sources that you can trust. Visit the website [here](http://cal-notify.symsoftsolutions.com).

## How We Built Cal-Notify

To quickly design and develop a working prototype from start to finish, we used a combination of user-centered design and agile development. We used Scrum as our framework while we adhered to the principles of the agile manifesto for clear communication and workflow amongst the team.

We developed the prototype functionality through two Sprints one-week long. By the end of each sprint we generated a potentially shippable product, which put in front of real users for getting their feedback and include that feedback in the next iteration.
Here is an overview of the activities executed during each sprint.

### Sprint 0

- Defined the [initial Product Backlog](https://github.com/SymSoftSolutions/cal-notify/blob/master/documentation/ux/agile-process/initial-product-backlog.pdf) with Acceptance Criteria, and refined that backlog 
- Agreed on Sprint and Release Level Definition of Done
- Defined code standards and [style guide](http://cal-notify.symsoftsolutions.com/style-guide/index.html)
- Defined the Application Architecture and Technology Stack
- Defined Continuous Integration Server and Continuous deployment strategy
- Setup Agile tracking tools (Jira)
- Research and initial [users interviews] (https://github.com/SymSoftSolutions/cal-notify/tree/master/documentation/ux/user-interviews)
- Generated [initial wireframes](https://github.com/SymSoftSolutions/cal-notify/blob/docs/images/symsoft-solutions-cal-notify_0-mockups.jpg) and gather users’ feedback.
- Identify and document [user's journey](https://github.com/SymSoftSolutions/cal-notify/blob/master/documentation/ux/user-research/cal-notify-user-journey.pdf)

The technical architecture and approach was defined in Sprint 0 as follows. Justification for the technologies chosen were evaluated per the USDS playbook.

- The prototype would be deployed in the Amazon Web Services cloud.
- The backbone of the prototype would be a ASP.NET Core, Swagger API.
- The front-end would be developed using Bootstrap and Angular.
- Unit tests would be conducted with xUnit.
- Continuous integration would be performed by Jenkins.

By the end of Sprint 0, we generated an HTML version of the application simulating the end-to-end functionality. Then we performed usability tests with users, and collected valuable feedback which was used to improve the next iteration of the prototype. We also had all of our continuous build processes in place, with issue tracking for bugs and usability issues identified from actual users.

### Sprint 1

-	Developed the committed user stories and automated tests
-	Deployed the application in a production environment on Amazon Web Services by using configuration and release management tools
-	Code Review sessions
-	Pair programming and pair testing
-	Configured application monitoring by using Amazon CloudWatch detailed monitoring.
-	Executed additional usability testing with the actual application and integrated additional feedback from users.
-	Automatic and manual testing in multiple mobile devices
-	Executed regression, accessibility and load testing.

By the end of Sprint 1, we released an end to end solution which we made available for real users to test and provide feedback. Based on this feedback we implemented some of the improvements for upcoming iterations and documented some other that are out of the scope of this prototype but that were identified as functionality that provides value to users.

Across all the sprints, we executed the following activities:

-	Sprint Planning
--	Estimation (using the planning poker technique for assigning story points)
- Daily Scrums
- Sprint Review
- Sprint Retrospective

We also created a Slack channel for quick communication, and integrated that channel with our continuous testing, integration and delivery tools.  

## Our User-Centered Approach.

We developed our prototype by keeping this simple idea in mind, Cal-Notify is not our app, it is our user’s app. This allowed us to keep the focus in the users all the time. 

We started our research by conducting user interviews and a survey to generate concepts and understand user needs. After we collected the initial information we gathered the users again and conducted a condensed version of the Google Ventures Design Sprint. As a result of it, we generated an initial HTML prototype which was presented to the users, and refined based on that feedback. 

# The Explicit RFI Requirements

## Team Leader
SymSoft Solutions assigned one person - Daniel Calzada - as the leader of the prototype delivery and gave that person the authority and responsibility to complete the work. Daniel was held accountable for the quality of the SymSoft Solutions prototype by our executive leadership.

## Multidisciplinary Team

Assembled a multidisciplinary team to deliver the prototype. We created a “war room” where team members worked side-by-side and freely collaborated across all aspects of the solution from the user experience design to the technical programming and usability testing. The team included the following labor categories / team members:

1. Product Manager - Savita Farooqui
2. Technical Architect - Abdul Farooqui
3. Interaction Designer/User Researcher/Usability Tester - Mark Aplet
4. Writer/Content Designer/Content Strategist - Miranda Singler
5. Visual Designer - Mark Aplet
6. Front-end Web Developer - Matthew Clemens
7. Backend Web Developer - Andres Bolanos
8. DevOps Engineer - Bhavik Patel
9. Security Engineer - Abdul Farooqui
10. Delivery Manager - Matt Murphy
11. Agile Coach - Daniel Calzada
12. Business Analyst - Savita Farooqui
13. Digital Performance Analyst - Miranda Singler

## Understand What People Needs
[Placeholder]

## Used at least a minimum of three “user-centric design” techniques and/or tools
1. Participatory Design: We worked with California citizens, employees, and stakeholders to define and refine the user interface and user experience.  
2. Usability Testing: Provides immediate feedback at key points that effectively informed our team of issues with our interface elements and functionality. Users were asked to verbalize what they were doing and why they were doing it. 
3. Interviews: User interviews were conducted to quickly identify and address issues or gaps between users expectations and product features. 


To-Do

[Link to the User Centric Design Page](https://#)

## Use GitHub to Document Code Commits
We used GitHub to document code commits as shown in our GitHub repository

To-Do: Get feedback from Matt C.

## Use Swagger to document the RESTful API
We used Swagger to document the RESTful API and provided a link to the Swagger API as follows:

[Cal-Notify  API](http://api-cal-notify.symsoftsolutions.com/swagger/index.html)

For further details, such as how to authenticate, common responses, etc. Read the introduction located at the top of the API documentation.

## Comply with Section 508 of the Americans with Disabilities Act and WCAG 2.0

We used a mix of both automated and manual testing to ensure that the Cal-Notify application complies with Section 508 and WCAG 2.0 AA priority. No one tool or process can resolve all the issues one might experience using assistive technology. While we use automated tools like [WAVE](http://wave.webaim.org/extension/) to catch many of the obvious errors, we also rely on code reviews by senior staff members (or those with deeper knowledge of accessibility best practices), and screen reader testing using JAWS and [NVDA](https://www.nvaccess.org/).

## Create or Used a Design Style Guide and/or a Pattern Library

We created the following style guide:

[Cal-Notify Style-Guide](http://cal-notify.symsoftsolutions.com/style-guide/index.html)

## Perform Usability Tests with People
in addition to our user-driven development process, we conducted usability testing at key junctures of the process including with the interactive prototype and final product

To-Do: Provide the link to the Usability test pages (Mark?)

## Use an Iterative Approach, Where Feedback Informed Subsequent Work or Versions of the Prototype

To-Do: Daniel to add this

## Create a Prototype that works on multiple devices, and presents a responsive design
We used the open source framework Bootstrap for the prototype sites. Bootstrap is a mobile first framework that provides user interface elements that adapt their presentation for a myriad of devices and viewport sizes. 

To test responsive compatibility, We make use of a “Device Lab” to simultaneously check multiple devices at once. Our device lab consists of number of popular platforms and software versions. We maintain a mix of Android phones and tablets, iOS phones and tablets, and Windows based phones.

## Use at Least Five Modern and Open-source Technologies
Our solution included the following modern and open-source technologies:

1.	ASP.NET core
2.	Postgres/PostGIS
3.	Swagger
4.	AngularJS
5.	Bootstrap
6.	Jenkins CI
7.	xUnit

## Deploy the prototype on an IaaS or PaaS provider,

We deployed the prototype on Amazon Web Services (AWS)

## Develop automated unit tests for their code
We developed and executed automated unit and integration tests for code using xUnit library. Test scripts [can be viewed here](https://github.com/SymSoftSolutions/cal-notify/tree/master/back-end/src/Tests)
