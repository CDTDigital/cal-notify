# Cal-Notify

Cal-Notify provides real time updates on emergencies and other events in your area, using official sources that you can trust.

# The Explicit RFI Requirements

## Team Leader
SymSoft Solutions assigned one person - Daniel Calzada - as the leader of the prototype delivery and gave that person the authority and responsibility to complete the work. Daniel was held accountable for the quality of the SymSoft Solutions prototype by our executive leadership.

## Multidisciplinary Team

assembled a multidisciplinary team to deliver the prototype. We created a “war room” where team members worked side-by-side and freely collaborated across all aspects of the solution from the user experience design to the technical programming and usability testing. The team included the following labor categories / team members:

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
[Placeholder]

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
We developed and executed automated unit and integration tests for code using xUnit library. Test results [can be viewed here](https://github.com/SymSoftSolutions/cal-notify/tree/master/back-end/src/Tests)
