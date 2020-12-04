To whom it may concern:

This is my C# solution to the Fruit Calculator coding challenge.  A few things I'd like to address before you run and analyze my code:

1.  This is a good example of how I write code for any production application I work on.  You'll notice that I've include a Logger class (which I wrote years ago) and simply tweaked it
    to include this exercise.  I find that using it is invaluable as the logs it can produce show me what is going on in the code at critical places.  Since it's not easy for one to
    debug an app in production, this class outputs a variety of message types (WARNING, MANDATORY, INFO and ERROR) to give a lot of messages one can peruse.  There are many messages that are coming
    from the logger when you run the application (a console app in this case where the log just writes to said console).  I have a configuration setting in the App.config file called 
    "loggerVerbosity" that is currently set to true to show all messages.  You can change this setting to false in order to eliminate all INFO messages and just see any ERROR, WARNING or MANDATORY
    messages.  To me, it's important to have something like this in any production application.  I used my exising logger class because it literally took me just a couple of minutes to incorporate it,
    but I'm also aware of the Microsoft

2.  The way I approached this challenge, as I do with all coding problems I face, was to carefully read the whole challenge statement a couple of times, walk away, think about what I've read
    and try to come up with a couple of approaches before I even try to code.  Once I feel I have a really good handle on what's being asked for I'll finally start doing some coding.

3.  In developing a solution for this problem, I found that using C# Dictionaries would lead to an easy, yet efficient one.  Although this exercise is called "Fruit Calculator", I chose to view it
    in terms of a "Fruit Basket Calculator" which guided my thought of looping the the required fruit in the basket as opposed to looping over all the available fruit and handling the ones that
    were in the basket.  You could imagine that if you had 3 million different types of fruit possible and only two types of fruit in the basket, it would be much faster to over the basket fruit
    and just "lookup" their information in the other two dictionaries I used as opposed to the other way around.  This is one thing that helped my solution to be scalable as far as performance is
    concerned.  The other is that I use simple comma delimited text files to store the input data.  Once can easily add more fruits, promotions and fruit in a basket easily without having to recompile
    application also helping the application be scalable.  For simplicity, you can bring these files up in the solution, make changes and when the project is run, these files will be copied to the 
    project output directory from where they are read by the application.

4.  As you'll notice, most of the time I spent writing this application was on error handling.  I've found time spent on error handling is significanlty recouped when the application goes to 
    production as much time can be saved if there are robust messages in the log if something untoward happens.  It also helps me during development time when I induce a bug or make a mistake.
    For this little application, I focused a lot on validation of the input data and common mistakes that could occur in when changing data in the files.  Most of this code wouldn't even have to 
    be touched if one decided to pull data from a database instead of files (of course, you'd have to modify the data access layer, but you wouldn't have to touch the validation code for the
    data itself).

5.  Finally, you'll notice that I tend to put a lot of comments in my code especially where there is more complicated logic, something tricky is going to happen, or just that I wanted to write a
    note.  I have found throughout my career that this is a good thing to do especially in the case where you're away from the application for some time (like a few months).  It's nice to have
    these comments to understand why something was done that particular way.

This was a fun exercise and I enjoyed developing this solution.  I hope you like it!

PROBLEM STATEMENT

Coding challenge

Problem: Fruit calculator

The solution needs to be written in C#. The project type, input and output it’s at the developer’s choice. The candidate should send us the best he/she could do in one hour. We do not want them to spend too much time on this.

We have an application that takes as an input some fruits and their prices, promotions applicable and a basket. The output is the total price.

 Example 1:

Input : Oranges – $10; Apples- $5 ; Promotions: No; Basket: Oranges - 5, Apples 1

Output: Total price= 55 

 Example 2:

Input : Oranges – $10; Apples- $5 ; Promotions: Oranges – 0.5;  Basket: Oranges - 5, Apples 1

Output: Total price= 30 