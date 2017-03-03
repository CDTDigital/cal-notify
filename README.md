# Cal-Notify

Cal-Notify is an emergency notifications system.  Users can register with the system and choose to be notified about various emergency and non-emergency (but important) events in their area via email or phone text messages.  Administrators of Cal-Notify can create the notification and publish them. The system also gets the information about potential emergencies from open data sources such as USGS (earthquakes) and NOAA (river levels).  Administrators of the system can edit and broadcast the notifications.  

[Signup on Cal-Notify Website to be notified for emergencies in your area!](http://cal-notify.symsoftsolutions.com).  
If you wish to see the administrative interface please login with username testAdmin1@test.com and password 123testadmin

## Technical Appraoch

To quickly design and develop a working prototype, we used a combination of user-centered design and agile development methods. We used Scrum as our Agile framework while we adhered to the principles of the agile manifesto for clear communication and workflow amongst the team.

We developed the prototype functionality through two Sprints one-week long. By the end of each sprint we generated a potentially shippable product, which put in front of real users for getting their feedback and include that feedback in the next iteration.
Here is an overview of the activities executed during each sprint.

### Sprint 0

- Defined the [initial Product Backlog](https://github.com/SymSoftSolutions/cal-notify/blob/master/documentation/agile-process/initial-product-backlog.pdf) with Acceptance Criteria, and refined that backlog 
- Agreed on Sprint and Release Level Definition of Done
- Defined code standards and [style guide](http://cal-notify.symsoftsolutions.com/style-guide/index.html)
- Defined the Application Architecture and Technology Stack
- Defined Continuous Integration Server and Continuous deployment strategy
- Setup Agile tracking tools (Jira)
- Research and initial [users interviews] (https://github.com/SymSoftSolutions/cal-notify/tree/master/documentation/ux/user-interviews)
- Generated [initial wireframes](https://github.com/SymSoftSolutions/cal-notify/blob/master/images/symsoft-solutions-cal-notify_1-wireframes.jpg) and gather users’ feedback.
- Identify and document [user's journey](https://github.com/SymSoftSolutions/cal-notify/blob/master/documentation/ux/user-research/cal-notify-user-journey.pdf)

#### The technical architecture and approach
Following decisions were made:

- Prototype would be deployed in the Amazon Web Services cloud.
- Prototype would use ASP.NET Core, Swagger API.
- Database would be PostgreSQL/PostGIS (to support geospatial queries).
- Front-end using Bootstrap and Angular.
- Application flow would be from the front-end, to the Swagger API, and then to the database. [See detailed diagram](https://github.com/SymSoftSolutions/cal-notify/blob/master/images/architecture-diagram.pdf)
- Unit tests would be conducted with xUnit.
- Continuous integration would be performed by Jenkins.

By the end of Sprint 0, we generated an HTML version of the application simulating the end-to-end functionality. Then we performed usability tests with users, and collected valuable feedback which was used to improve the next iteration of the prototype. We also had all of our continuous build processes in place, with issue tracking for bugs and usability issues identified from actual users.

### Sprint 1

-	Developed prioritized user stories
- Utilized automated tests and continuous build
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
- Daily Scrums
- Sprint Review
- Sprint Retrospective

We also created a Slack channel for quick communication, and integrated that channel with our continuous testing, integration and delivery tools.  

## User-Centered Design

We designed the prototype with focus on users. We gathered information about user needs and expectations by running a survey and conducting user interviews. We then worked with the users using  condensed version of the Google Ventures Design Sprint for user experience design of the application. We created storyboards, sketches, wireframes and an interactive prototype with input from users. We tested this prototype with users and refined based on feedback. 

# The RFI Requirements

## Team Leader
SymSoft Solutions assigned Daniel Calzada as the leader of the prototype delivery and gave that person the authority and responsibility to complete the work. Daniel was held accountable for the quality of the SymSoft Solutions prototype by our executive leadership.

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

## Understood what people needed, by including people in the prototype development and design process

We utilized user-centered design that included real world users throughout the development of the Cal-Notify prototype. At the outset, we conducted user interviews and a survey to understand user needs and continually tested the emergent design with actual users after new features were added. The resulting application, thus, was developed with real users, based on an understanding of what they felt was critical, how they would want to complete tasks and how they would want to receive information and interact with the Cal-Notify service beyond the initial signup. Artifacts from [our user-centered design process can be found here:] (https://github.com/SymSoftSolutions/cal-notify/tree/master/documentation/ux)


## Used at least a minimum of three (3) “user-centric design” techniques and/or tools

- User-driven Development. We used Iterative User-driven Development throughout the design and development process to continually test the validity of the design with actual users at each stage of the process from statement of requirements to conceptual story-boarding to interactive prototyping and completed code. We optimized the product based on how users indicated they would want to see the product function versus building it and expecting them to adapt to the user experience we ourselves envisioned. Our multi-disciplinary team brought together a variety of skills and perspectives so that user reactions to the product were propagated across all aspects of the system including the user interface, but also the business rules and the supporting API and data layers of the application. The entire team was informed by user-centered design inputs throughout the iterative development of the solution.

- User Personas. Based on our user research, we created key personas that were used for mapping the user journies for conceptual and detailed design. We used these personas to generate refined versions of users with different characteristics in order to design the user experience. The users we involved in our user-centered design process possessed key elements of these personas. For example, the users included individuals who had been impacted personally by a natural disaster, and individuals whose loved ones were impacted, and individuals who had no first-hand experience were included in the studies and testing. The other persona we looked at was the administrative user who has the responsibility of creating, managing, boradcasting and tracking the emergency and non-emergency notifications.

- Storyboarding, Use Cases and Scenarios. We used storyboarding with real world users that reflected our User Personas in order to identify specific use cases and more detailed scenarios to address in our prototype. We focused storyboarding sessions on generating design concepts that would resolve specific use cases and scenarios that we jointly identified with pool of users. These included “happy path” best case scenarios and also more problematic scenarios such as users who for one reason or another wanted to opt-out of receiving alerts. Our primary focus for the prototype due mainly to time constraints was on high likelihood “typical” scenarios for the User Personas we created. 	

- [Online Survey](https://www.surveymonkey.com/r/H5RVP5Z) In order to establish a broad basis for persona and use case development at the outset of our user-centered design process, we conducted an online survey to develop some quantitative data about the potential pool of likely users. This survey which included basic user research such as whether not users had personal experience with natural disasters and how they would want to receive alerts provided foundational data for us to begin the process of identifying Personas and Use Cases. The survey yielded results such as the facts that while the majority of potential users had never been personally impacted by a natural disaster, the vast majority of potential users would still sign up for alerts. This was important for Persona development in that one key persona characteristic was that of being “more curious than concerned” with regard to Cal-Notify alerts.

## Use GitHub to Document Code Commits
We used GitHub to document code commits as shown in our GitHub repository

## Use Swagger to document the RESTful API
We used Swagger to document the RESTful API and provided a link to the Swagger API as follows:

[Cal-Notify  API](http://api-cal-notify.symsoftsolutions.com/swagger/index.html)

For further details, such as how to authenticate, common responses, etc. Read the introduction located at the top of the API documentation.

## Comply with Section 508 of the Americans with Disabilities Act and WCAG 2.0
We used a both automated and manual testing to ensure that the Cal-Notify application complies with Section 508 and WCAG 2.0 AA priority. No one tool or process can resolve all the issues one might experience using assistive technology. While we use automated tools like [WAVE](http://wave.webaim.org/extension/) to catch many of the obvious errors, we also rely on code reviews by senior staff members (or those with deeper knowledge of accessibility best practices), and screen reader testing using JAWS and [NVDA](https://www.nvaccess.org/).

## Create or Used a Design Style Guide and/or a Pattern Library

We created the following style guide:

[Cal-Notify Style-Guide](http://cal-notify.symsoftsolutions.com/style-guide/index.html)

## Perform Usability Tests with People
We conducted [usability testing](https://github.com/SymSoftSolutions/cal-notify/blob/master/images/symsoft-solutions-cal-notify_3-usability-tests.jpg) with actual users througout the development starting with the [interactive prototype](https://github.com/SymSoftSolutions/cal-notify/blob/master/images/symsoft-solutions-cal-notify_0-mockups.jpg), at various times when new features are ready and finally with the final product.

## Use an Iterative Approach, Where Feedback Informed Subsequent Work or Versions of the Prototype
We developed Cal-Notify functionality through two Sprints one-week long each. During each iteration, we followed the [U.S. Digital Services Playbook](https://playbook.cio.gov) and by the end of each Sprint, we released an end-to-end solution which solved our user’s key needs and we conducted multiple usability test with real users. From the collected feedback we improved the following iterations of the prototype.

We used Scrum as our framework while adhered to the principles of the agile manifesto, and we rigorously executed all the Scrum Activities. We also used multiple agile tools to facilitate collaboration, for example the integration of GitHub, Jira, Jenkins, AWS and Slack during the development and testing processes. 

## Create a Prototype that works on multiple devices, and presents a responsive design
We used the open source framework Bootstrap for the prototype sites. Bootstrap is a mobile first framework that provides user interface elements that adapt their presentation for a myriad of devices and viewport sizes. 

To test responsive compatibility, We make use of a “Device Lab” to simultaneously check multiple devices at once. Our device lab consists of number of popular platforms and software versions. We maintain a mix of Android phones and tablets, iOS phones and tablets, and Windows based phones.

## Use at Least Five Modern and Open-source Technologies
Our solution included the following modern and open-source technologies:

1. ASP.NET core
2. Postgres/PostGIS
3. Swagger
4. AngularJS
5. Bootstrap
6. Jenkins CI
7. xUnit
8. LESS

## Deploy the prototype on an IaaS or PaaS provider,

We deployed the prototype on Amazon Web Services (AWS)

## Develop automated unit tests for their code
We developed and executed automated unit and integration tests for code using xUnit library. Test scripts [can be viewed here](https://github.com/SymSoftSolutions/cal-notify/tree/master/back-end/src/Tests)
