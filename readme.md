# What are ObjectBlocks?

It started as an idea to move ripple off of XML and turned into something that might be handy in configuration scenarios. FubuObjectBlocks
is just a VERY simple object notation used to express simple objects. The values are parsed and pushed through the model binding component in FubuCore
so you get all of the conventional goodness and power we know and love.

While useful for tools like ripple, FubuObjectBlocks might prove to be useful in the AppSettingsProvider scenarios as well.

### Example syntax:

Since I can't seem to figure out how to format it here, here's the link to the gist:
https://gist.github.com/jmarnold/6342496

A few things to note:

* Nested objects are denoted with ":"
* Collections can either be defined as a nested object or inline as shown in 'feed' and 'nuget'. 
* Inline objects must define property names ending with ":". The first value is known as the implicit value. See the unit tests for examples.
