# README #

This is a data infrastructure for ASP.NET MVC project that doesn't use Entity Framework and has it's own simple ORM. This is written to be used with Oracle because that is what I am using but should be easily converted for other databases.

### Why? ###

Besides being too slow, using Entity Framework was difficult to work with data that I use at my current position.

The data is not normalized but has been processed for reporting. I also use a lot of written queries to retrieve data in order to create charts, graphs, and other reports. A lot of custom object building and businees logic is needed that was too time consuming to due using entity framework.

This was written on my own time.
